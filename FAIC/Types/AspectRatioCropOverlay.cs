using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Cursor = System.Windows.Input.Cursor;
using Cursors = System.Windows.Input.Cursors;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;
using SystemColors = System.Windows.SystemColors;

namespace FAIC.Types.Forms
{
    /// <summary>
    /// Draws and edits a source-relative crop rectangle. The class name is
    /// retained for compatibility; aspect ratio is only enforced while Shift
    /// or Control is held during a resize, and Alt resizes from the centre.
    /// </summary>
    public sealed class AspectRatioCropOverlay
    {
        private static readonly Rect UnitBounds = new(0, 0, 1, 1);
        private readonly Brush shadeBrush = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));
        private readonly Pen borderPen = new(SystemColors.HighlightBrush, 1.5);

        private Rect sourceBounds = Rect.Empty;
        private Rect dragBounds;
        private Rect dragStartSelection;
        private Rect selection = UnitBounds;
        private Point dragStartUv;
        private Vector? dragDirection;
        private bool isVisible = true;

        private bool CanInteract => isVisible && !sourceBounds.IsEmpty;

        public event Action<Rect> SelectionChanged = _ => { };
        public event Action VisualChanged = () => { };

        public double HandleSize { get; set; } = 8;
        public double MinimumSelectionSize { get; set; } = 16;
        public double CenterSnapDistance { get; set; } = 8;

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible == value)
                    return;
                isVisible = value;
                VisualChanged.Invoke();
            }
        }

        /// <summary>
        /// Crop rectangle in visible-source UV coordinates. Width and height
        /// are independent, and values extending beyond 0..1 are clipped.
        /// </summary>
        public Rect NormalizedSelection
        {
            get => selection;
            set => SetSelection(value);
        }

        public void Draw(DrawingContext dc, Rect bounds)
        {
            sourceBounds = bounds.Width > 0 && bounds.Height > 0 ? bounds : Rect.Empty;
            if (!CanInteract)
                return;

            Rect crop = ToDisplayRect(selection, sourceBounds);

            // A single even-odd fill has no joins at the crop corners.
            GeometryGroup shade = new() { FillRule = FillRule.EvenOdd };
            shade.Children.Add(new RectangleGeometry(sourceBounds));
            shade.Children.Add(new RectangleGeometry(crop));
            dc.DrawGeometry(shadeBrush, null, shade);

            if (dragDirection is Vector direction && direction.LengthSquared == 0)
            {
                Point centre = new(sourceBounds.Left + sourceBounds.Width / 2, sourceBounds.Top + sourceBounds.Height / 2);
                if (Math.Abs(crop.Left + crop.Width / 2 - centre.X) < 0.01)
                    dc.DrawLine(borderPen, new Point(centre.X, sourceBounds.Top), new Point(centre.X, sourceBounds.Bottom));
                if (Math.Abs(crop.Top + crop.Height / 2 - centre.Y) < 0.01)
                    dc.DrawLine(borderPen, new Point(sourceBounds.Left, centre.Y), new Point(sourceBounds.Right, centre.Y));
            }

            dc.DrawRectangle(null, borderPen, crop);

            double radius = Math.Max(0, HandleSize / 2);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    Point centre = new(
                        crop.X + crop.Width * (x + 1) / 2,
                        crop.Y + crop.Height * (y + 1) / 2);
                    dc.DrawRectangle(
                        SystemColors.WindowBrush,
                        borderPen,
                        new Rect(centre.X - radius, centre.Y - radius, radius * 2, radius * 2));
                }
            }
        }

        public bool HandleMouseDown(FrameworkElement owner, Point position)
        {
            if (!CanInteract || HitTest(position) is not Vector direction)
                return false;

            dragDirection = direction;
            dragBounds = sourceBounds;
            dragStartSelection = selection;
            dragStartUv = ToUv(position, dragBounds);
            owner.CaptureMouse();
            owner.Cursor = CursorFor(direction);
            VisualChanged.Invoke();
            return true;
        }

        public bool HandleMouseMove(FrameworkElement owner, Point position)
        {
            if (!CanInteract)
            {
                owner.Cursor = Cursors.Arrow;
                return false;
            }

            if (dragDirection is not Vector direction)
            {
                owner.Cursor = CursorFor(HitTest(position));
                return false;
            }

            Point pointerUv = ToUv(position, dragBounds);
            ModifierKeys modifiers = Keyboard.Modifiers;
            bool preserveAspectRatio = (modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0;
            bool resizeFromCenter = (modifiers & ModifierKeys.Alt) != 0;
            SetSelection(direction.LengthSquared == 0
                ? MoveSelection(pointerUv)
                : ResizeSelection(pointerUv, direction, preserveAspectRatio, resizeFromCenter));
            return true;
        }

        public bool HandleMouseUp(FrameworkElement owner, Point position)
        {
            if (dragDirection == null)
                return false;

            dragDirection = null;
            owner.ReleaseMouseCapture();
            owner.Cursor = CanInteract ? CursorFor(HitTest(position)) : Cursors.Arrow;
            VisualChanged.Invoke();
            return true;
        }

        public void HandleMouseLeave(FrameworkElement owner)
        {
            if (dragDirection == null)
                owner.Cursor = Cursors.Arrow;
        }

        public void CancelDrag(FrameworkElement owner)
        {
            dragDirection = null;
            owner.ReleaseMouseCapture();
            owner.Cursor = Cursors.Arrow;
            VisualChanged.Invoke();
        }

        private Rect MoveSelection(Point pointerUv)
        {
            Vector delta = pointerUv - dragStartUv;
            double x = Math.Clamp(dragStartSelection.X + delta.X, 0, 1 - dragStartSelection.Width);
            double y = Math.Clamp(dragStartSelection.Y + delta.Y, 0, 1 - dragStartSelection.Height);
            double snapDistance = Math.Max(0, CenterSnapDistance);

            bool disableSnap = (Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != 0;
            if (!disableSnap)
            {
                if (Math.Abs(x + dragStartSelection.Width / 2 - 0.5) * dragBounds.Width <= snapDistance)
                    x = (1 - dragStartSelection.Width) / 2;
                if (Math.Abs(y + dragStartSelection.Height / 2 - 0.5) * dragBounds.Height <= snapDistance)
                    y = (1 - dragStartSelection.Height) / 2;
            }

            return new Rect(
                x,
                y,
                dragStartSelection.Width,
                dragStartSelection.Height);
        }

        private Rect ResizeSelection(
            Point pointerUv,
            Vector direction,
            bool preserveAspectRatio,
            bool resizeFromCenter)
        {
            double x = dragStartSelection.X;
            double y = dragStartSelection.Y;
            double width = dragStartSelection.Width;
            double height = dragStartSelection.Height;

            ResizeAxis(ref x, ref width, pointerUv.X, direction.X, dragBounds.Width, resizeFromCenter);
            ResizeAxis(ref y, ref height, pointerUv.Y, direction.Y, dragBounds.Height, resizeFromCenter);
            if (preserveAspectRatio)
            {
                double widthScale = width / dragStartSelection.Width;
                double heightScale = height / dragStartSelection.Height;
                double scale = direction.X == 0 ? heightScale
                    : direction.Y == 0 ? widthScale
                    : Math.Min(widthScale, heightScale); //Identical to Paint.NET
                double minimum = Math.Max(0, MinimumSelectionSize);
                double minimumScale = Math.Max(
                    minimum / (dragStartSelection.Width * dragBounds.Width),
                    minimum / (dragStartSelection.Height * dragBounds.Height));
                double maximumScale = Math.Min(
                    MaximumScale(dragStartSelection.Left, dragStartSelection.Right, direction.X, resizeFromCenter),
                    MaximumScale(dragStartSelection.Top, dragStartSelection.Bottom, direction.Y, resizeFromCenter));
                scale = Math.Clamp(scale, Math.Min(minimumScale, maximumScale), maximumScale);

                width = dragStartSelection.Width * scale;
                height = dragStartSelection.Height * scale;
                x = resizeFromCenter || direction.X == 0
                    ? dragStartSelection.Left + (dragStartSelection.Width - width) / 2
                    : direction.X < 0 ? dragStartSelection.Right - width
                    : dragStartSelection.Left;
                y = resizeFromCenter || direction.Y == 0
                    ? dragStartSelection.Top + (dragStartSelection.Height - height) / 2
                    : direction.Y < 0 ? dragStartSelection.Bottom - height
                    : dragStartSelection.Top;
            }

            return new Rect(x, y, width, height);
        }

        private static double MaximumScale(double start, double end, double direction, bool resizeFromCenter) =>
            (resizeFromCenter || direction == 0
                ? 2 * Math.Min((start + end) / 2, 1 - (start + end) / 2)
                : direction < 0 ? end : 1 - start)
            / (end - start);

        // The same formula handles X/Y and leading/trailing edges.
        private void ResizeAxis(
            ref double origin,
            ref double length,
            double pointer,
            double direction,
            double displayLength,
            bool resizeFromCenter)
        {
            if (direction == 0)
                return;

            if (resizeFromCenter)
            {
                double centre = origin + length / 2;
                double centeredAvailable = Math.Min(centre, 1 - centre);
                double centeredMinimum = Math.Min(Math.Max(0, MinimumSelectionSize) / displayLength / 2, centeredAvailable);
                double halfLength = Math.Abs(Math.Clamp(
                    pointer,
                    direction < 0 ? centre - centeredAvailable : centre + centeredMinimum,
                    direction < 0 ? centre - centeredMinimum : centre + centeredAvailable) - centre);
                origin = centre - halfLength;
                length = halfLength * 2;
                return;
            }

            double fixedEdge = origin + (direction < 0 ? length : 0);
            double available = direction < 0 ? fixedEdge : 1 - fixedEdge;
            double minimum = Math.Min(Math.Max(0, MinimumSelectionSize) / displayLength, available);
            double movedEdge = Math.Clamp(
                pointer,
                direction < 0 ? 0 : fixedEdge + minimum,
                direction < 0 ? fixedEdge - minimum : 1);

            if (direction < 0)
                origin = movedEdge;
            length = Math.Abs(movedEdge - fixedEdge);
        }

        private Vector? HitTest(Point point)
        {
            Rect crop = ToDisplayRect(selection, sourceBounds);
            double tolerance = Math.Max(6, HandleSize / 2 + 2);
            Rect hitBounds = crop;
            hitBounds.Inflate(tolerance, tolerance);
            if (!hitBounds.Contains(point))
                return null;

            Vector direction = new(
                HitAxis(point.X, crop.Left, crop.Right, tolerance),
                HitAxis(point.Y, crop.Top, crop.Bottom, tolerance));
            return direction.LengthSquared > 0 || crop.Contains(point) ? direction : null;
        }

        private static int HitAxis(double value, double start, double end, double tolerance)
        {
            double startDistance = Math.Abs(value - start);
            double endDistance = Math.Abs(value - end);
            return Math.Min(startDistance, endDistance) > tolerance
                ? 0
                : startDistance <= endDistance ? -1 : 1;
        }

        private static Cursor CursorFor(Vector? direction)
        {
            if (direction is not Vector value)
                return Cursors.Arrow;
            if (value.X == 0)
                return value.Y == 0 ? Cursors.SizeAll : Cursors.SizeNS;
            if (value.Y == 0)
                return Cursors.SizeWE;
            return value.X == value.Y ? Cursors.SizeNWSE : Cursors.SizeNESW;
        }

        private void SetSelection(Rect value)
        {
            value = ClipToSource(value);
            if (selection == value)
                return;

            selection = value;
            SelectionChanged.Invoke(value);
            VisualChanged.Invoke();
        }

        private static Rect ClipToSource(Rect value)
        {
            if (value.IsEmpty
                || !double.IsFinite(value.Left)
                || !double.IsFinite(value.Top)
                || !double.IsFinite(value.Right)
                || !double.IsFinite(value.Bottom))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Rect clipped = Rect.Intersect(UnitBounds, value);
            if (clipped.IsEmpty || clipped.Width <= 0 || clipped.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            return clipped;
        }

        private static Rect ToDisplayRect(Rect uv, Rect bounds) => new(
            bounds.X + uv.X * bounds.Width,
            bounds.Y + uv.Y * bounds.Height,
            uv.Width * bounds.Width,
            uv.Height * bounds.Height);

        private static Point ToUv(Point point, Rect bounds) => new(
            (point.X - bounds.X) / bounds.Width,
            (point.Y - bounds.Y) / bounds.Height);
    }
}