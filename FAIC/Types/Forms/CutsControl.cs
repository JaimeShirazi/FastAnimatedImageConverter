using FAIC.Types.Cuts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FAIC.Types.Forms
{
    public enum CutPart
    {
        Start,
        Body,
        End
    }

    [ToolboxItem(true)]
    public class CutsControl : ScrollableControl
    {
        private const int DefaultRowHeight = 24;
        private const int EndCapRadius = 6;
        private const int HitPadding = 4;
        private const int TrackEdgePadding = 2;

        private bool _updatingLayoutMetrics;
        private bool _applyingDrag;
        private DragSession? _drag;
        private CutCollection _cuts = new();

        public CutsControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.Selectable |
                ControlStyles.UserPaint,
                true);

            AutoScroll = true;
            BackColor = SystemColors.Window;
            ForeColor = SystemColors.ControlText;
            Padding = new Padding(8, 4, 8, 4);
            TabStop = true;

            AttachCuts(_cuts);
            UpdateLayoutMetrics();
        }

        public CutsControl(CutCollection cuts)
            : this()
        {
            Cuts = cuts;
        }

        #region Layout and appearance

        private bool _autoSizeHeight = true;

        [Category("Layout")]
        [DefaultValue(true)]
        [Description("When enabled, only the height is managed automatically; the width remains caller-controlled.")]
        public bool AutoSizeHeight
        {
            get => _autoSizeHeight;
            set
            {
                if (value == _autoSizeHeight)
                {
                    return;
                }

                _autoSizeHeight = value;
                UpdateLayoutMetrics();
            }
        }

        private int _maximumVisibleSegments = 8;

        [Category("Layout")]
        [DefaultValue(8)]
        [Description("Maximum number of rows shown before vertical scrolling is used.")]
        public int MaximumVisibleSegments
        {
            get => _maximumVisibleSegments;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "At least one row must be visible.");
                }

                if (value == _maximumVisibleSegments)
                {
                    return;
                }

                _maximumVisibleSegments = value;
                UpdateLayoutMetrics();
            }
        }

        private int _rowHeight = DefaultRowHeight;

        [Category("Layout")]
        [DefaultValue(DefaultRowHeight)]
        [Description("Height of each row in logical pixels.")]
        public int RowHeight
        {
            get => _rowHeight;
            set
            {
                if (value < (EndCapRadius + HitPadding) * 2)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "RowHeight is too small for the end-cap hit area.");
                }

                if (value == _rowHeight)
                {
                    return;
                }

                _rowHeight = value;
                UpdateLayoutMetrics();
                Invalidate();
            }
        }

        private bool _showPlayhead = true;

        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowPlayhead
        {
            get => _showPlayhead;
            set
            {
                if (value == _showPlayhead)
                {
                    return;
                }

                _showPlayhead = value;
                Invalidate();
            }
        }

        private Color _selectedRowBackColor = Color.FromArgb(225, 239, 255);

        [Category("Appearance")]
        public Color SelectedRowBackColor
        {
            get => _selectedRowBackColor;
            set => SetColor(ref _selectedRowBackColor, value);
        }

        private int _snapDistancePixels = 9;

        [Category("Behavior")]
        [DefaultValue(9)]
        [Description("Distance in logical pixels within which an endpoint snaps to the playhead or another cut endpoint.")]
        public int SnapDistancePixels
        {
            get => _snapDistancePixels;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "SnapDistancePixels cannot be negative.");
                }

                _snapDistancePixels = value;
            }
        }
        #endregion

        #region Runtime data

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CutCollection Cuts
        {
            get => _cuts;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                if (ReferenceEquals(value, _cuts))
                {
                    return;
                }

                FinishDrag();
                DetachCuts(_cuts);
                _cuts = value;
                AttachCuts(_cuts);
                UpdateLayoutMetrics();

                if (IsHandleCreated)
                {
                    EnsureSelectedSegmentVisible();
                }

                Invalidate();
            }
        }

        private decimal _playheadPosition = 0.0m;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public decimal PlayheadPosition
        {
            get => _playheadPosition;
            set
            {
                if (value is < 0m or > 1m)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "PlayheadPosition must be between 0 and 1.");
                }

                if (value == _playheadPosition)
                {
                    return;
                }

                _playheadPosition = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex => Cuts.Selected;

        #endregion

        protected override Size DefaultSize => new(420, DefaultRowHeight + 8);

        public void EnsureSelectedSegmentVisible()
        {
            if (SelectedIndex < 0 || SelectedIndex >= Cuts.Total)
            {
                return;
            }

            PerformLayout();
            int rowHeight = ScaledRowHeight;
            int rowTop = Padding.Top + (SelectedIndex * rowHeight);
            int rowBottom = rowTop + rowHeight;
            int viewportTop = -AutoScrollPosition.Y;
            int viewportBottom = viewportTop + ClientSize.Height;
            int desiredTop = viewportTop;

            if (rowTop < viewportTop)
            {
                desiredTop = rowTop;
            }
            else if (rowBottom > viewportBottom)
            {
                desiredTop = rowBottom - ClientSize.Height;
            }

            int maximumScroll = Math.Max(0, ContentHeight - ClientSize.Height);
            desiredTop = Math.Clamp(desiredTop, 0, maximumScroll);
            if (desiredTop != viewportTop)
            {
                AutoScrollPosition = new Point(0, desiredTop);
                Invalidate();
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int width = proposedSize.Width > 0 ? proposedSize.Width : Math.Max(1, Width);
            return new Size(width, PreferredEditorHeight);
        }

        protected override void SetBoundsCore(
            int x,
            int y,
            int width,
            int height,
            BoundsSpecified specified)
        {
            if (AutoSizeHeight &&
                !_updatingLayoutMetrics &&
                (specified & BoundsSpecified.Height) != 0)
            {
                height = PreferredEditorHeight;
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (SelectedIndex >= 0)
            {
                int y = RowTop(SelectedIndex);
                using var brush = new SolidBrush(Enabled ? SelectedRowBackColor : SystemColors.Control);
                e.Graphics.FillRectangle(brush, 0, y, ClientSize.Width, ScaledRowHeight);
            }

            if (ShowPlayhead)
            {
                int x = ValueToX(PlayheadPosition);
                Color color = Enabled ? Palette.StaticB : SystemColors.GrayText;
                using var pen = new Pen(color, Math.Max(1f, ScaleLogical(1)))
                {
                    DashStyle = DashStyle.Dot
                };
                e.Graphics.DrawLine(pen, x, 0, x, ClientSize.Height);
            }

            int radius = ScaleLogical(EndCapRadius);
            for (int index = 0; index < Cuts.Total; index++)
            {
                int centerY = RowCenter(index);
                if (centerY + radius < 0 || centerY - radius > ClientSize.Height)
                {
                    continue;
                }

                bool selected = index == SelectedIndex;
                Color color = !Enabled
                    ? SystemColors.GrayText
                    : selected ? Palette.GetForIndex(index).onSelected : Palette.GetForIndex(index).onBright;
                Color interiorColor = selected
                    ? Enabled ? SelectedRowBackColor : SystemColors.Control
                    : BackColor;
                int startX = ValueToX(Cuts[index].Start);
                int endX = ValueToX(Cuts[index].End);

                using var linePen = new Pen(color, Math.Max(1.5f, ScaleLogical(2)))
                {
                    StartCap = LineCap.Round,
                    EndCap = LineCap.Round
                };
                e.Graphics.DrawLine(linePen, startX, centerY, endX, centerY);

                using var interiorBrush = new SolidBrush(interiorColor);
                using var capPen = new Pen(color, Math.Max(1.25f, ScaleLogical(2)));
                DrawEnd(e.Graphics, startX, centerY, radius, interiorBrush, capPen);
                DrawEnd(e.Graphics, endX, centerY, radius, interiorBrush, capPen);
            }

            if (_drag is not null &&
                _drag.Mode != CutPart.Body &&
                _drag.AffectedEnds.Count > 1)
            {
                decimal guideValue = Math.Clamp(_drag.Anchor.ValueBuffer + _drag.LastDelta, 0m, 1m);
                int x = ValueToX(guideValue);
                using var pen = new Pen(Palette.StaticA, Math.Max(1.5f, ScaleLogical(2)))
                {
                    DashStyle = DashStyle.Dash
                };
                e.Graphics.DrawLine(pen, x, 0, x, ClientSize.Height);
            }

            if (Focused && ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (!Enabled || e.Button != MouseButtons.Left || Cuts.Total < 1)
            {
                return;
            }

            Focus();
            Hit hit = HitTest(e.Location);
            if (hit.Index < 0)
            {
                return;
            }

            //Always select the cut the mouse is hovering over
            Cuts.OnNewSelectedValue(hit.Index);

            CutPart? part = hit.Kind switch
            {
                HitKind.StartCap => CutPart.Start,
                HitKind.Body => CutPart.Body,
                HitKind.EndCap => CutPart.End,
                _ => null
            };

            if (part is CutPart dragPart)
            {
                bool includeAligned = (ModifierKeys & Keys.Shift) == 0;
                _drag = DragSession.Create(
                    Cuts,
                    hit.Index,
                    dragPart,
                    includeAligned,
                    e.Location);

                if (_drag is not null)
                {
                    Capture = true;
                }
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            DragSession? drag = _drag;
            if (drag is null)
            {
                Cursor = HitTest(e.Location).Kind switch
                {
                    HitKind.StartCap or HitKind.EndCap or HitKind.Body => Cursors.SizeWE,
                    _ => Cursors.Default
                };
                return;
            }

            if (!drag.IsDragging)
            {
                if (!HasLeftDragThreshold(drag.MouseDownLocation, e.Location))
                {
                    return;
                }

                drag.IsDragging = true;
            }

            if (!IsDragSourceCompatible(drag))
            {
                FinishDrag();
                return;
            }

            decimal rawDelta = (e.X - drag.MouseDownLocation.X) / (decimal)TrackWidth;
            decimal delta = Math.Clamp(rawDelta, drag.Min, drag.Max);

            if ((ModifierKeys & Keys.Shift) == 0)
            {
                decimal snapTolerance = ScaleLogical(SnapDistancePixels) / (decimal)TrackWidth;
                decimal nearestDistance = decimal.MaxValue;

                foreach (decimal candidate in GetSnapCandidates(drag))
                {
                    decimal distance = Math.Abs(rawDelta - candidate);
                    if (distance <= snapTolerance &&
                        distance < nearestDistance &&
                        candidate >= drag.Min &&
                        candidate <= drag.Max)
                    {
                        delta = candidate;
                        nearestDistance = distance;
                    }
                }
            }

            if (delta == drag.LastDelta)
            {
                return;
            }

            if (!ApplyDragDelta(drag, delta))
            {
                return;
            }

            drag.LastDelta = delta;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_drag is not null && e.Button == MouseButtons.Left)
            {
                FinishDrag();
            }
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
            if (_drag is not null && !Capture)
            {
                FinishDrag();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_drag is null)
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            UpdateLayoutMetrics();
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateLayoutMetrics();
            EnsureSelectedSegmentVisible();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (!Enabled)
            {
                FinishDrag();
            }

            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            UpdateLayoutMetrics();
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _drag = null;
                DetachCuts(_cuts);
            }

            base.Dispose(disposing);
        }

        private int ScaledRowHeight => ScaleLogical(RowHeight);
        private int ContentHeight => Padding.Vertical + (Math.Max(1, Cuts.Total) * ScaledRowHeight);
        private int PreferredEditorHeight =>
            Padding.Vertical +
            (Math.Max(1, Math.Min(Cuts.Total, MaximumVisibleSegments)) * ScaledRowHeight);
        private int TrackLeft =>
            Padding.Left + ScaleLogical(EndCapRadius + TrackEdgePadding);
        private int TrackRight =>
            Math.Max(
                TrackLeft + 1,
                ClientSize.Width - Padding.Right - ScaleLogical(EndCapRadius + TrackEdgePadding));
        private int TrackWidth => Math.Max(1, TrackRight - TrackLeft);

        private void AttachCuts(CutCollection collection)
        {
            collection.OnNewSelectedValues += HandleCutValuesChanged;
            collection.OnNewSelectionOrSize += HandleCutSelectionOrSizeChanged;
        }

        private void DetachCuts(CutCollection collection)
        {
            collection.OnNewSelectedValues -= HandleCutValuesChanged;
            collection.OnNewSelectionOrSize -= HandleCutSelectionOrSizeChanged;
        }

        private void HandleCutValuesChanged()
        {
            if (!_applyingDrag)
            {
                FinishDrag();
            }

            Invalidate();
        }

        private void HandleCutSelectionOrSizeChanged()
        {
            FinishDrag();
            UpdateLayoutMetrics();

            if (IsHandleCreated)
            {
                EnsureSelectedSegmentVisible();
            }

            Invalidate();
        }

        private void UpdateLayoutMetrics()
        {
            if (_updatingLayoutMetrics || IsDisposed)
            {
                return;
            }

            _updatingLayoutMetrics = true;
            try
            {
                AutoScrollMinSize = new Size(0, ContentHeight);
                if (AutoSizeHeight && Height != PreferredEditorHeight)
                {
                    SetBounds(Left, Top, Width, PreferredEditorHeight, BoundsSpecified.Height);
                }

                if (Cuts.Total <= MaximumVisibleSegments && AutoScrollPosition != Point.Empty)
                {
                    AutoScrollPosition = Point.Empty;
                }
            }
            finally
            {
                _updatingLayoutMetrics = false;
            }
        }

        private Hit HitTest(Point location)
        {
            int contentY = location.Y - AutoScrollPosition.Y - Padding.Top;
            if (contentY < 0)
            {
                return Hit.None;
            }

            int index = contentY / ScaledRowHeight;
            if ((uint)index >= (uint)Cuts.Total)
            {
                return Hit.None;
            }

            int centerY = RowCenter(index);
            int hitRadius = ScaleLogical(EndCapRadius + HitPadding);
            int startX = ValueToX(Cuts[index].Start);
            int endX = ValueToX(Cuts[index].End);
            long startDistance = DistanceSquared(location, startX, centerY);
            long endDistance = DistanceSquared(location, endX, centerY);
            long radiusSquared = (long)hitRadius * hitRadius;

            if (startDistance <= radiusSquared || endDistance <= radiusSquared)
            {
                if (startDistance == endDistance)
                {
                    return new Hit(
                        location.X < startX ? HitKind.StartCap : HitKind.EndCap,
                        index);
                }

                return new Hit(
                    startDistance < endDistance ? HitKind.StartCap : HitKind.EndCap,
                    index);
            }

            int lineTolerance = ScaleLogical(5);
            if (Math.Abs(location.Y - centerY) <= lineTolerance &&
                location.X >= Math.Min(startX, endX) &&
                location.X <= Math.Max(startX, endX))
            {
                return new Hit(HitKind.Body, index);
            }

            return new Hit(HitKind.Row, index);
        }

        private IEnumerable<decimal> GetSnapCandidates(DragSession drag)
        {
            var movingIndexes = new HashSet<int>();
            foreach (End end in drag.AffectedEnds)
            {
                movingIndexes.Add(end.Index);
            }

            // Yield every playhead candidate before any endpoint candidate so
            // an exact-distance tie consistently favours the playhead.
            foreach (End movingEnd in drag.AffectedEnds)
            {
                yield return PlayheadPosition - movingEnd.ValueBuffer;
            }

            foreach (End movingEnd in drag.AffectedEnds)
            {
                decimal movingValue = movingEnd.ValueBuffer;
                for (int targetIndex = 0; targetIndex < Cuts.Total; targetIndex++)
                {
                    if (movingIndexes.Contains(targetIndex))
                    {
                        continue;
                    }

                    Cut target = Cuts[targetIndex];
                    yield return target.Start - movingValue;
                    yield return target.End - movingValue;
                }
            }
        }

        private bool ApplyDragDelta(DragSession drag, decimal delta)
        {
            if (!IsDragSourceCompatible(drag))
            {
                FinishDrag();
                return false;
            }

            var updates = new Dictionary<int, Cut>();
            foreach (End end in drag.AffectedEnds)
            {
                if ((uint)end.Index >= (uint)Cuts.Total)
                {
                    FinishDrag();
                    return false;
                }

                if (!updates.TryGetValue(end.Index, out Cut changed))
                {
                    // Preserve the crop and any non-moving endpoint from the
                    // latest authoritative collection value.
                    changed = Cuts[end.Index];
                }

                decimal value = end.ValueBuffer + delta;
                if (end.IsStart)
                {
                    changed.Start = value;
                }
                else
                {
                    changed.End = value;
                }

                updates[end.Index] = changed;
            }

            long revisionBefore = Cuts.Revision;
            bool changedValues;
            _applyingDrag = true;
            try
            {
                changedValues = Cuts.ApplyBatch(updates);
            }
            finally
            {
                _applyingDrag = false;
            }

            long expectedRevision = revisionBefore + (changedValues ? 1L : 0L);
            if (!ReferenceEquals(_drag, drag) ||
                Cuts.Revision != expectedRevision)
            {
                if (ReferenceEquals(_drag, drag))
                {
                    FinishDrag();
                }

                return false;
            }

            drag.ExpectedRevision = Cuts.Revision;
            return true;
        }

        private bool IsDragSourceCompatible(DragSession drag) =>
            ReferenceEquals(_drag, drag) &&
            Cuts.Total == drag.CutCount &&
            Cuts.Revision == drag.ExpectedRevision;

        private void FinishDrag()
        {
            if (_drag is null)
            {
                return;
            }

            // Clear first so releasing capture cannot recursively finish the
            // same session in OnMouseCaptureChanged.
            _drag = null;
            if (Capture)
            {
                Capture = false;
            }

            Cursor = Cursors.Default;
            Invalidate();
        }

        private int RowTop(int index) =>
            Padding.Top + (index * ScaledRowHeight) + AutoScrollPosition.Y;

        private int RowCenter(int index) => RowTop(index) + (ScaledRowHeight / 2);

        private int ValueToX(decimal value) =>
            TrackLeft + (int)Math.Round(value * TrackWidth, MidpointRounding.AwayFromZero);

        private int ScaleLogical(int value) => value == 0
            ? 0
            : Math.Max(1, (int)Math.Round(value * DeviceDpi / 96d));

        private void SetColor(ref Color field, Color value)
        {
            if (value == field)
            {
                return;
            }

            field = value;
            Invalidate();
        }

        private static void DrawEnd(
            Graphics graphics,
            int centerX,
            int centerY,
            int radius,
            Brush interiorBrush,
            Pen outlinePen)
        {
            var bounds = new Rectangle(
                centerX - radius,
                centerY - radius,
                radius * 2,
                radius * 2);
            graphics.FillEllipse(interiorBrush, bounds);
            graphics.DrawEllipse(outlinePen, bounds);
        }

        private static long DistanceSquared(Point point, int x, int y)
        {
            long deltaX = point.X - x;
            long deltaY = point.Y - y;
            return (deltaX * deltaX) + (deltaY * deltaY);
        }

        private static bool HasLeftDragThreshold(Point start, Point current)
        {
            Size dragSize = SystemInformation.DragSize;
            var dragBounds = new Rectangle(
                start.X - (dragSize.Width / 2),
                start.Y - (dragSize.Height / 2),
                dragSize.Width,
                dragSize.Height);
            return !dragBounds.Contains(current);
        }

        private enum HitKind
        {
            None,
            StartCap,
            Body,
            EndCap,
            Row
        }

        private readonly struct Hit
        {
            public static Hit None { get; } = new(HitKind.None, -1);

            public Hit(HitKind kind, int index)
            {
                Kind = kind;
                Index = index;
            }

            public HitKind Kind { get; }
            public int Index { get; }
        }

        private readonly struct End
        {
            public End(int index, bool isStart, CutCollection collection)
            {
                Index = index;
                IsStart = isStart;
                ValueBuffer = isStart
                    ? collection[index].Start
                    : collection[index].End;
            }

            public int Index { get; }
            public bool IsStart { get; }
            public decimal ValueBuffer { get; }
        }

        private sealed class DragSession
        {
            private DragSession(
                CutCollection cuts,
                CutPart mode,
                List<End> affectedEnds,
                End anchor,
                decimal min,
                decimal max,
                Point mouseDownLocation)
            {
                Mode = mode;
                AffectedEnds = affectedEnds;
                Anchor = anchor;
                Min = min;
                Max = max;
                MouseDownLocation = mouseDownLocation;
                CutCount = cuts.Total;
                ExpectedRevision = cuts.Revision;
            }

            public static DragSession? Create(
                CutCollection cuts,
                int target,
                CutPart part,
                bool includeAligned,
                Point mouseDownLocation,
                decimal tolerance = 0m)
            {
                if (tolerance < 0m)
                {
                    throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance cannot be negative.");
                }

                if ((uint)target >= (uint)cuts.Total)
                {
                    return null;
                }

                List<End> ends = GetEndsToDrag(
                    cuts,
                    target,
                    part,
                    includeAligned,
                    tolerance,
                    out End anchor,
                    out decimal min,
                    out decimal max);

                if (ends.Count == 0 || min > max)
                {
                    // Existing source values and the configured minimum length
                    // have no common legal delta, so refuse this gesture.
                    return null;
                }

                return new DragSession(
                    cuts,
                    part,
                    ends,
                    anchor,
                    min,
                    max,
                    mouseDownLocation);
            }

            private static List<End> GetEndsToDrag(
                CutCollection collection,
                int target,
                CutPart part,
                bool includeAligned,
                decimal tolerance,
                out End anchor,
                out decimal min,
                out decimal max)
            {
                Cut targetCut = collection[target];
                anchor = new End(
                    target,
                    part != CutPart.End,
                    collection);

                var ends = new List<End>();
                min = -1m;
                max = 1m;

                for (int i = 0; i < collection.Total; i++)
                {
                    Cut candidate = collection[i];
                    bool movesStart;
                    bool movesEnd;

                    if (part == CutPart.Body)
                    {
                        bool matchesWholeCut =
                            Math.Abs(candidate.Start - targetCut.Start) <= tolerance &&
                            Math.Abs(candidate.End - targetCut.End) <= tolerance;
                        bool movesWholeCut =
                            i == target ||
                            (includeAligned && matchesWholeCut);

                        movesStart = movesWholeCut;
                        movesEnd = movesWholeCut;
                    }
                    else
                    {
                        decimal anchorValue = part == CutPart.Start
                            ? targetCut.Start
                            : targetCut.End;
                        bool eligible = i == target || includeAligned;

                        movesStart =
                            eligible &&
                            Math.Abs(candidate.Start - anchorValue) <= tolerance;
                        movesEnd =
                            eligible &&
                            Math.Abs(candidate.End - anchorValue) <= tolerance;
                    }

                    if (movesStart)
                    {
                        ends.Add(new End(i, true, collection));
                        min = Math.Max(min, -candidate.Start);

                        if (!movesEnd)
                        {
                            max = Math.Min(
                                max,
                                candidate.End - Program.NormalizedMinimumCutLength - candidate.Start);
                        }
                    }

                    if (movesEnd)
                    {
                        ends.Add(new End(i, false, collection));
                        max = Math.Min(max, 1m - candidate.End);

                        if (!movesStart)
                        {
                            min = Math.Max(
                                min,
                                candidate.Start + Program.NormalizedMinimumCutLength - candidate.End);
                        }
                    }
                }

                return ends;
            }

            public End Anchor { get; }
            public CutPart Mode { get; }
            public List<End> AffectedEnds { get; }
            public Point MouseDownLocation { get; }
            public decimal Min { get; }
            public decimal Max { get; }
            public int CutCount { get; }
            public long ExpectedRevision { get; set; }
            public bool IsDragging { get; set; }
            public decimal LastDelta { get; set; }
        }
    }
}
