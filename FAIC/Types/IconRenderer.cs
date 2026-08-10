using System.Drawing.Drawing2D;

namespace FAIC.Types
{
    public static class IconRenderer
    {
        public enum Icon
        {
            Magnet,
            MagnetOff
        }
        public static void DrawIcon(
            Graphics graphics,
            RectangleF targetRect,
            Icon icon,
            Color color,
            float strokeWidth = 2f)
        {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));

            GraphicsState state = graphics.Save();

            try
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                using (GraphicsPath path = GetIcon(icon,
                    out RectangleF ViewBox,
                    out LineCap startCap, out LineCap endCap, out LineJoin lineJoin))
                {
                    ApplyViewBoxTransform(graphics, ViewBox, targetRect);
                    using (Pen pen = new(color, strokeWidth)
                    {
                        StartCap = startCap,
                        EndCap = endCap,
                        LineJoin = lineJoin
                    })
                    {
                        graphics.DrawPath(pen, path);
                    }
                }
            }
            finally
            {
                graphics.Restore(state);
            }
        }
        private static void ApplyViewBoxTransform(
            Graphics graphics,
            RectangleF sourceViewBox,
            RectangleF targetRect)
        {
            float scale = Math.Min(
                targetRect.Width / sourceViewBox.Width,
                targetRect.Height / sourceViewBox.Height);

            float drawWidth = sourceViewBox.Width * scale;
            float drawHeight = sourceViewBox.Height * scale;

            float offsetX = targetRect.X + (targetRect.Width - drawWidth) * 0.5f;
            float offsetY = targetRect.Y + (targetRect.Height - drawHeight) * 0.5f;

            graphics.TranslateTransform(offsetX, offsetY);
            graphics.ScaleTransform(scale, scale);
            graphics.TranslateTransform(-sourceViewBox.X, -sourceViewBox.Y);
        }
        public static GraphicsPath GetIcon(Icon type,
            out RectangleF viewBox,
            out LineCap startCap, out LineCap endCap, out LineJoin lineJoin)
        {
            GraphicsPath path = new();
            switch (type)
            {
                case Icon.Magnet:
                    //magnet from Tabler Icons (MIT)
                    //Translated from the SVG path with ChatGPT.
                    //GraphicsPath arc angles are clockwise in the screen's positive-Y-down coordinate system.
                    viewBox = new(0f, 0f, 24f, 24f);
                    startCap = LineCap.Round;
                    endCap = LineCap.Round;
                    lineJoin = LineJoin.Round;

                    // M4 13V5a2 2 0 0 1 2-2h1a2 2 0 0 1 2 2v8a2 2 0 0 0 6 0V5a2 2 0 0 1 2-2h1a2 2 0 0 1 2 2v8a8 8 0 0 1-16 0
                    path.StartFigure();
                    path.AddLine(4f, 13f, 4f, 5f);
                    path.AddArc(4f, 3f, 4f, 4f, 180f, 90f);
                    path.AddLine(6f, 3f, 7f, 3f);
                    path.AddArc(5f, 3f, 4f, 4f, -90f, 90f);
                    path.AddLine(9f, 5f, 9f, 13f);
                    // SVG expands r=2 to r=3 here because the endpoints are 6 units apart.
                    path.AddArc(9f, 10f, 6f, 6f, 180f, -180f);
                    path.AddLine(15f, 13f, 15f, 5f);
                    path.AddArc(15f, 3f, 4f, 4f, 180f, 90f);
                    path.AddLine(17f, 3f, 18f, 3f);
                    path.AddArc(16f, 3f, 4f, 4f, -90f, 90f);
                    path.AddLine(20f, 5f, 20f, 13f);
                    path.AddArc(4f, 5f, 16f, 16f, 0f, 180f);

                    // m0-5h5
                    path.StartFigure();
                    path.AddLine(4f, 8f, 9f, 8f);

                    // m6 0h4
                    path.StartFigure();
                    path.AddLine(15f, 8f, 19f, 8f);
                    break;
                case Icon.MagnetOff:
                    //magnet-off from Tabler Icons (MIT)
                    //Translated from the SVG path with ChatGPT.
                    //The non-cardinal arc values below come from the SVG endpoint-to-centre arc conversion algorithm.
                    viewBox = new(0f, 0f, 24f, 24f);
                    startCap = LineCap.Round;
                    endCap = LineCap.Round;
                    lineJoin = LineJoin.Round;

                    // M7 3a2 2 0 0 1 2 2
                    path.StartFigure();
                    path.AddArc(5f, 3f, 4f, 4f, 270f, 90f);

                    // m0 4v4a3 3 0 0 0 5.552 1.578
                    path.StartFigure();
                    path.AddLine(9f, 9f, 9f, 13f);

                    // a3 3 0 0 0 5.552 1.578
                    path.AddArc(
                        9f, 10.000884f, 6f, 6f,
                        -179.98312f, -148.30114f);

                    // M15 11V5a2 2 0 0 1 2-2h1a2 2 0 0 1 2 2v8a8 8 0 0 1-.424 2.577
                    path.StartFigure();
                    path.AddLine(15f, 11f, 15f, 5f);
                    path.AddArc(15f, 3f, 4f, 4f, 180f, 90f);
                    path.AddLine(17f, 3f, 18f, 3f);
                    path.AddArc(16f, 3f, 4f, 4f, -90f, 90f);
                    path.AddLine(20f, 5f, 20f, 13f);

                    // a8 8 0 0 1 -.424 2.577
                    path.AddArc(
                        4.000003f, 5.007122f, 16f, 16f,
                        -0.051009f, 18.788622f);

                    // m-1.463 2.584A8 8 0 0 1 4 13V5c0-.297.065-.58.181-.833
                    path.StartFigure();

                    // A8 8 0 0 1 4 13
                    path.AddArc(
                        4f, 5.000454f, 16f, 16f,
                        40.17079f, 139.83247f);

                    path.AddLine(4f, 13f, 4f, 5f);

                    // Tiny cubic continuation exactly matching the tail of the SVG path.
                    path.AddBezier(
                        new PointF(4f, 5f),
                        new PointF(4f, 4.703f),
                        new PointF(4.065f, 4.42f),
                        new PointF(4.181f, 4.167f));

                    // M4 8h4
                    path.StartFigure();
                    path.AddLine(4f, 8f, 8f, 8f);

                    // M15 8h4
                    path.StartFigure();
                    path.AddLine(15f, 8f, 19f, 8f);

                    // M3 3l18 18
                    path.StartFigure();
                    path.AddLine(3f, 3f, 21f, 21f);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            return path;
        }
    }
}
