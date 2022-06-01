using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Program
{
    class MainClass
    {
        /// <summary>
        /// Random dataset.
        /// </summary>
        /// <remarks>
        /// It's the weather forecast, temperature of New York, London, Hong Kong.
        /// This is used as base for generation the randomized fake data for layered diagrams.
        /// </remarks>
        private static float[,] _dataset = new float[,] { { 133600f, 7.06f, 3.4f, 14.76f }, { 137200f, 6.99f, 3.74f, 16.15f }, { 140800f, 7.34f, 4.12f, 17.33f }, { 144400f, 9.49f, 4.32f, 18.39f }, { 148000f, 10.99f, 4.53f, 19.25f }, { 151600f, 10.81f, 4.69f, 19.71f }, { 155200f, 10.5f, 4.91f, 19.71f }, { 158800f, 9.99f, 5.08f, 19.19f }, { 162400f, 9.49f, 5.51f, 18.31f }, { 166000f, 8.92f, 5.99f, 17.52f }, { 169600f, 8.24f, 6.16f, 16.86f }, { 173200f, 7.02f, 6.15f, 16.39f }, { 176800f, 5.73f, 5.95f, 16.08f }, { 180400f, 4.79f, 5.63f, 15.93f }, { 184000f, 4.72f, 5.3f, 15.54f }, { 187600f, 4.85f, 4.64f, 15.45f }, { 191200f, 5.18f, 4.17f, 15.13f }, { 194800f, 5.85f, 4.03f, 14.73f }, { 198400f, 6.48f, 4.23f, 14.29f }, { 202000f, 6.48f, 4.23f, 13.97f }, { 205600f, 5.89f, 4.07f, 13.79f }, { 209200f, 5.04f, 3.85f, 13.72f }, { 212800f, 4.58f, 3.65f, 14.39f }, { 216400f, 4.5f, 3.68f, 15.84f }, { 220000f, 4.67f, 3.68f, 17.47f }, { 223600f, 4.92f, 3.64f, 18.96f }, { 227200f, 5.15f, 3.52f, 20.04f }, { 230800f, 5.25f, 3.47f, 20.88f }, { 234400f, 5.09f, 3.31f, 21.4f }, { 238000f, 4.81f, 3.18f, 21.65f }, { 241600f, 4.38f, 3.05f, 21.38f }, { 245200f, 3.86f, 3.1f, 20.76f }, { 248800f, 3.13f, 3.45f, 19.83f }, { 252400f, 2.39f, 4.17f, 19.06f }, { 256000f, 1.8f, 4.75f, 18.4f }, { 259600f, 1.75f, 5.17f, 18.03f }, { 263200f, 2.14f, 5.23f, 17.76f }, { 266800f, 2.71f, 5.02f, 17.57f }, { 270400f, 3.48f, 4.46f, 17.43f }, { 274000f, 4.23f, 3.92f, 17.27f }, { 277600f, 4.6f, 3.4f, 17.02f }, { 281200f, 4.41f, 3.11f, 16.85f }, { 284800f, 3.99f, 2.89f, 16.79f }, { 288400f, 3.49f, 2.68f, 16.75f }, { 292000f, 3.03f, 2.32f, 16.77f }, { 295600f, 2.48f, 2.06f, 16.85f }, { 299200f, 2.23f, 1.88f, 17.4f }, { 302800f, 2.27f, 1.72f, 18.44f }, { 306400f, 2.49f, 1.44f, 19.62f }, { 310000f, 5.06f, 2.4f, 17.76f } };

        /// <summary>
        /// Path to directory for generated images.
        /// </summary>
        private static readonly string _workingDirectory = "../../out";

        /// <summary>
        /// Amount of generated images.
        /// </summary>
        private static readonly int _frames = 150;

        /// <summary>
        /// Size of output frame.
        /// </summary>
        private static readonly Size _frameSize = new Size(1920, 1080);

        /// <summary>
        /// Size of temporary images for layered diagrams.
        /// </summary>
        private static readonly Size _diagramSize = new Size(1200, 500);

        /// <summary>
        /// Index of Dataset column for the top layer.
        /// </summary>
        private static readonly int _columnA = 3;

        /// <summary>
        /// Index of Dataset column to mix with the top layer data.
        /// </summary>
        private static readonly int _columnB = 1;

        /// <summary>
        /// Index of Dataset column to mix with the top layer data.
        /// </summary>
        private static readonly int _columnC = 2;

        /// <summary>
        /// Index of selected position on layered diagrams for crossing diagram.
        /// </summary>
        private static readonly int _position = 25;

        /// <summary>
        /// Array to save the randomized values for layered diagrams.
        /// </summary>
        private static float[] _values = new float[9];

        /// <summary>
        /// Random generator of jitter for the deep layers.
        /// </summary>
        private static Random _random = new Random("Start".GetHashCode());

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            ModifyData();

            for (int i = 0; i < _frames; ++i)
            {
                Bitmap all = DrawAll(i);
                all.Save(string.Format("{0}/{1:d4}.png", _workingDirectory, i + 1));
            }
        }

        /// <summary>
        /// Changes the dataset values for better visualization.
        /// </summary>
        /// <remarks>
        /// Increases the amplitude of fake data.
        /// Replaces the values in the first column ("Time") to X coordinates.
        /// </remarks>
        private static void ModifyData()
        {
            int kx = 15;
            int ky = 10;

            for (int i = 0; i < _dataset.GetLength(0); ++i)
            {
                _dataset[i, 0] = i * kx;
                _dataset[i, _columnA] *= ky;
                _dataset[i, _columnB] *= ky * 1.5f;
                _dataset[i, _columnC] *= ky * 2;
            }
        }

        /// <summary>
        /// Draws all diagrams.
        /// </summary>
        /// <param name="tau">The index of frame.</param>
        private static Bitmap DrawAll(int tau)
        {
            Bitmap bitmap = new Bitmap(_frameSize.Width, _frameSize.Height, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            graphics.Clear(Color.Black);

            {
                Matrix myMatrix = new Matrix();
                myMatrix.Shear(0, -0.3f);
                graphics.MultiplyTransform(myMatrix);

                int layers = 10;
                float sigmaBase = 3.0f;
                float alphaBase = 0.1f;
                float sigmaDecrement = sigmaBase / layers;
                float alphaIncrement = (1 - alphaBase) / layers;
                int xIncrement = 50;
                int yIncrement = 50;
                for (int i = 0; i < layers - 1; ++i)
                {
                    Bitmap diagram = MakeDiagram(
                        tau,
                        i,
                        sigmaBase - sigmaDecrement * i,
                        alphaBase + alphaIncrement * i);
                    graphics.DrawImage(diagram, new RectangleF(
                        200 + xIncrement * i,
                        200 + yIncrement * i,
                        diagram.Width, diagram.Height));
                }

                graphics.ResetTransform();
            }

            {
                Matrix myMatrix = new Matrix();
                myMatrix.Shear(0, 0.724f);
                graphics.MultiplyTransform(myMatrix);

                Bitmap diagram = DrawCrossDiagram();
                graphics.DrawImage(diagram, new RectangleF(
                    575,
                    -500,
                    diagram.Width, diagram.Height));

                graphics.ResetTransform();
            }

            return bitmap;
        }

        /// <summary>
        /// Draws the temporary image with a diagram and modifies it as layer.
        /// </summary>
        /// <param name="tau">The index of frame.</param>
        /// <param name="zeta">The index of layer.</param>
        /// <param name="sigma">The standard deviation of the blur effect.</param>
        /// <param name="alpha">The transparency ratio.</param>
        /// <returns>Bitmap of diagram</returns>
        private static Bitmap MakeDiagram(int tau, int zeta, float sigma, float alpha)
        {
            Bitmap diagram = DrawDiagram(tau, zeta);
            diagram = Blur(diagram, sigma);
            diagram = Blend(diagram, alpha);

            return diagram;
        }

        /// <summary>
        /// Blurs the bitmap.
        /// </summary>
        /// <remarks>
        /// Does it by SkiaSharp library, but receives and returns System.Drawing.Bitmap.
        /// </remarks>
        /// <param name="bitmap">Source image</param>
        /// <param name="sigma">The standard deviation of the blur effect.</param>
        /// <returns>Destination image</returns>
        private static Bitmap Blur(Bitmap bitmap, float sigma)
        {
            SKImageInfo info = new SKImageInfo(_diagramSize.Width, _diagramSize.Height);
            SKSurface surface = SKSurface.Create(info);
            SKCanvas canvas = surface.Canvas;

            using (SKPaint paint = new SKPaint())
            {
                paint.ImageFilter = SKImageFilter.CreateBlur(sigma, sigma);
                canvas.DrawBitmap(BitmapExtensions.ToSKBitmap(bitmap), new SKPoint(0, 0), paint);
            }

            SKImage image = surface.Snapshot();
            Bitmap bitmap2 = Extensions.ToBitmap(image);

            return bitmap2;
        }

        /// <summary>
        /// Makes the image semi-transparent.
        /// </summary>
        /// <remarks>
        /// Does it by SkiaSharp library, but receives and returns System.Drawing.Bitmap.
        /// </remarks>
        /// <param name="bitmap">Source image</param>
        /// <param name="alpha">The transparency ratio.</param>
        /// <returns>Destination image</returns>
        private static Bitmap Blend(Bitmap bitmap, float alpha)
        {
            SKImageInfo info = new SKImageInfo(_diagramSize.Width, _diagramSize.Height);
            SKSurface surface = SKSurface.Create(info);
            SKCanvas canvas = surface.Canvas;

            using (SKPaint paint = new SKPaint())
            {
                paint.Color = paint.Color.WithAlpha((byte)(0xFF * Math.Min(1, alpha)));
                canvas.DrawBitmap(BitmapExtensions.ToSKBitmap(bitmap), new SKPoint(0, 0), paint);
            }

            SKImage image = surface.Snapshot();
            Bitmap bitmap2 = Extensions.ToBitmap(image);

            return bitmap2;
        }

        /// <summary>
        /// Draws the temporary image with a diagram for layer.
        /// </summary>
        /// <param name="tau">The index of frame.</param>
        /// <param name="zeta">The index of layer.</param>
        /// <returns>Bitmap of diagram</returns>
        private static Bitmap DrawDiagram(int tau, int zeta)
        {
            Bitmap bitmap = new Bitmap(_diagramSize.Width, _diagramSize.Height, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            graphics.TranslateTransform(100, _diagramSize.Height);
            graphics.ScaleTransform(1.0f, -1.0f);

            int range = (10 - zeta) * 5;
            int jitter = _random.Next(range);

            Color color = zeta == 8 ? Color.White : Color.Cyan;
            Pen pen = new Pen(color, 4);
            Color color2 = Color.FromArgb(80, color);
            Pen pen2 = new Pen(color2, 4);
            Brush brush = new SolidBrush(Color.Black);

            for (int i = 0; i < _dataset.GetLength(0); ++i)
            {
                DrawLines(graphics, pen, pen2, i, tau, zeta, jitter);
            }

            pen = new Pen(color, 2);

            for (int i = 0; i < _dataset.GetLength(0); ++i)
            {
                DrawPoint(graphics, pen, brush, i, tau, zeta, jitter);
            }

            return bitmap;
        }

        /// <summary>
        /// Draws the temporary image with a crossing diagram.
        /// </summary>
        /// <returns>Bitmap of crossing diagram</returns>
        private static Bitmap DrawCrossDiagram()
        {
            Bitmap bitmap = new Bitmap(_diagramSize.Width, _diagramSize.Height, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            graphics.TranslateTransform(100, _diagramSize.Height);
            graphics.ScaleTransform(1.0f, -1.0f);

            int d = 20; // diameter of the point mark
            float s = 50.0f; // step by X
            Color color = Color.OrangeRed;
            Pen pen = new Pen(color, 6);
            Color color2 = Color.FromArgb(40, color);
            Pen pen2 = new Pen(color2, d);
            Brush brush = new SolidBrush(color);

            for (int i = 0; i < _values.Length; ++i)
            {
                if (i != 0)
                {
                    graphics.DrawLine( // between points
                        pen,
                        new PointF(
                            (i - 1) * s,
                            _values[i - 1]),
                        new PointF(
                            i * s,
                            _values[i]));
                }

                graphics.DrawLine( // from point to zero level
                    pen2,
                    new PointF(
                        i * s,
                        _values[i]),
                    new PointF(
                        i * s,
                        0.0f));

                graphics.DrawLine( // as zero level mark
                    pen2,
                    new PointF(
                        i * s,
                        5.0f),
                    new PointF(
                        i * s,
                        0.0f));

                RectangleF rect = new RectangleF(
                    i * s - d / 2,
                    _values[i] - d / 2,
                    d, d);

                graphics.FillEllipse(brush, rect);
            }

            return bitmap;
        }

        /// <summary>
        /// Draws the lines by current point of layered diagram.
        /// </summary>
        /// <remarks>
        /// Draws the line between current point and previous point of layered diagram.
        /// Also draws the line from current point to bottom (zero level).
        /// </remarks>
        /// <param name="graphics">GDI+ device context wrapper.</param>
        /// <param name="pen">Pen for line between points.</param>
        /// <param name="pen2">Pen for line from point to zero level.</param>
        /// <param name="index">The index of the value point on diagram.</param>
        /// <param name="tau">The index of frame.</param>
        /// <param name="zeta">The index of layer.</param>
        /// <param name="jitter">Random increment for value from dataset.</param>
        private static void DrawLines(Graphics graphics, Pen pen, Pen pen2, int index, int tau, int zeta, int jitter)
        {
            int y = (index + tau) % _dataset.GetLength(0);
            int last = _dataset.GetLength(0) - 1;
            int prev = y == 0? last : y - 1;
            float z = zeta * 0.1f; // ratio for mix between columns

            if (index != 0)
            {
                graphics.DrawLine( // between points
                    pen,
                    new PointF(
                        _dataset[index - 1, 0],
                        _dataset[prev, _columnA] + _dataset[prev, _columnB] * z - _dataset[prev, _columnC] * (1 - z) + jitter),
                    new PointF(
                        _dataset[index, 0],
                        _dataset[y, _columnA] + _dataset[y, _columnB] * z - _dataset[y, _columnC] * (1 - z) + jitter));
            }

            graphics.DrawLine( // from point to zero level
                pen2,
                new PointF(
                    _dataset[index, 0],
                    _dataset[y, _columnA] + _dataset[y, _columnB] * z - _dataset[y, _columnC] * (1 - z) + jitter),
                new PointF(
                    _dataset[index, 0],
                    0.0f));
        }

        /// <summary>
        /// Draws the current point of layered diagram.
        /// </summary>
        /// <param name="graphics">GDI+ device context wrapper.</param>
        /// <param name="pen">Pen for line between points.</param>
        /// <param name="index">The index of the value point on diagram.</param>
        /// <param name="tau">The index of frame.</param>
        /// <param name="zeta">The index of layer.</param>
        /// <param name="jitter">Random increment for value from dataset.</param>
        private static void DrawPoint(Graphics graphics, Pen pen, Brush brush, int index, int tau, int zeta, int jitter)
        {
            int d = 10; // diameter of the point mark
            int y = (index + tau) % _dataset.GetLength(0);
            float z = zeta * 0.1f; // ratio for mix between columns
            float value = _dataset[y, _columnA] + _dataset[y, _columnB] * z - _dataset[y, _columnC] * (1 - z) + jitter;

            if (index == _position)
            {
                _values[zeta] = value;
            }

            RectangleF rect = new RectangleF(
                _dataset[index, 0] - d / 2,
                value - d / 2,
                d, d);

            graphics.FillEllipse(brush, rect);
            graphics.DrawEllipse(pen, rect);
        }
    }

    public static class BitmapExtensions
    {
        /// <summary>
        /// Convert System.Drawing bitmap to SkiaSharp bitmap.
        /// </summary>
        /// <param name="bitmap">Bitmap for conversion.</param>
        /// <returns>Converted bitmap.</returns>
        public static SKBitmap ToSKBitmap(this Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                return SKBitmap.Decode(stream);
            }
        }

        /// <summary>
        /// Convert System.Drawing icon to SkiaSharp bitmap.
        /// </summary>
        /// <param name="icon">Icon for conversion.</param>
        /// <returns>Converted bitmap.</returns>
        public static SKBitmap ToSKBitmap(this Icon icon)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                icon.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return SKBitmap.Decode(stream);
            }
        }
    }
}
