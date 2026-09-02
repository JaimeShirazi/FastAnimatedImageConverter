using System.ComponentModel;

namespace FAIC.Types.Forms
{
    [ToolboxItem(true)]
    public class MagnetToggleButton : Button
    {
        private bool magnetEnabled = true;
        private bool shiftHeld;
        private bool monitoringShift;

        public MagnetToggleButton()
        {
            FlatStyle = FlatStyle.Standard;
            UseVisualStyleBackColor = true;
            Text = "";
        }

        protected override Size DefaultSize => new(30, 30);

        /// <summary>
        /// Button state (doesn't account for shift)
        /// </summary>
        [DefaultValue(true)]
        public bool MagnetEnabled
        {
            get => magnetEnabled;
            set
            {
                if (magnetEnabled == value)
                    return;

                magnetEnabled = value;
                Invalidate();
                UpdateStaticValue();

                MagnetEnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Snap state (accounting for shift)
        /// </summary>
        [Browsable(false)]
        public bool EffectiveMagnetEnabled => MagnetEnabled && !shiftHeld;

        /// <summary>
        /// Padding in 96-DPI pixels
        /// </summary>
        [DefaultValue(4)]
        public int IconPadding { get; set; } = 4;

        public event EventHandler? MagnetEnabledChanged;

        protected override void OnClick(EventArgs e)
        {
            MagnetEnabled = !MagnetEnabled;
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float padding = IconPadding * DeviceDpi / 96f;

            RectangleF rect = RectangleF.Inflate(
                ClientRectangle,
                -padding,
                -padding);

            Color color =
                !Enabled || shiftHeld
                    ? SystemColors.GrayText
                    : ForeColor;

            IconRenderer.DrawIcon(
                e.Graphics,
                rect,
                EffectiveMagnetEnabled
                    ? IconRenderer.Icon.Magnet
                    : IconRenderer.Icon.MagnetOff,
                color);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (!monitoringShift)
            {
                ShiftMonitor.Instance.ShiftChanged += ShiftChanged;
                monitoringShift = true;
            }

            shiftHeld = ShiftMonitor.Instance.ShiftHeld;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (monitoringShift)
            {
                ShiftMonitor.Instance.ShiftChanged -= ShiftChanged;
                monitoringShift = false;
            }

            base.OnHandleDestroyed(e);
        }

        private void ShiftChanged(object? sender, EventArgs e)
        {
            shiftHeld = ShiftMonitor.Instance.ShiftHeld;
            Invalidate();
            UpdateStaticValue();
        }

        private void UpdateStaticValue()
        {
            Program.SnapEnabled = EffectiveMagnetEnabled;
        }

        protected override void SetBoundsCore(
            int x,
            int y,
            int width,
            int height,
            BoundsSpecified specified)
        {
            //Force 1:1
            if ((specified & BoundsSpecified.Width) != 0 &&
                (specified & BoundsSpecified.Height) == 0)
            {
                height = width;
            }
            else if ((specified & BoundsSpecified.Height) != 0 &&
                     (specified & BoundsSpecified.Width) == 0)
            {
                width = height;
            }
            else
            {
                int size = Math.Min(width, height);
                width = height = size;
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// Application-wide monitor for shift held state
        /// </summary>
        private sealed class ShiftMonitor : IMessageFilter
        {
            public static ShiftMonitor Instance { get; } = new();

            public bool ShiftHeld { get; private set; }

            public event EventHandler? ShiftChanged;

            private ShiftMonitor()
            {
                ShiftHeld = ReadShift();
                Application.AddMessageFilter(this);
            }

            public bool PreFilterMessage(ref Message m)
            {
                switch (m.Msg)
                {
                    case 0x0100: //WM_KEYDOWN
                    case 0x0101: //WM_KEYUP
                    case 0x0104: //WM_SYSKEYDOWN
                    case 0x0105: //WM_SYSKEYUP
                        SetShift(ReadShift());
                        break;

                    case 0x001C: //WM_ACTIVATEAPP
                        SetShift(
                            m.WParam != IntPtr.Zero &&
                            ReadShift());
                        break;
                }

                return false;
            }

            private static bool ReadShift() => (Control.ModifierKeys & Keys.Shift) != 0;

            private void SetShift(bool value)
            {
                if (ShiftHeld == value)
                    return;

                ShiftHeld = value;
                ShiftChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}