using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArduinoBLETemperature.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Thermometer : ContentView
    {

        public float Temperature
        {
            get { return (float)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); }
        }

        public static readonly BindableProperty TemperatureProperty =
    BindableProperty.Create(nameof(Temperature), typeof(float), typeof(Thermometer), 0.0f, propertyChanged: OnTemperatureChanged);

        public Thermometer()
        {
            InitializeComponent();
        }

        private static void OnTemperatureChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Thermometer thermometer = bindable as Thermometer;
            thermometer?.CanvasView.InvalidateSurface();
        }

        // fluid glass parameter
        private const int bottomFluidGlassCircleCenterY = -160;
        private const float bottomFluidGlassHullRadius = 100;
        private const int topFluidGlassCircleCenterY = 100;
        private const int topFluidGlassWidth = 80;

        // fluid parameter
        private const int fluidHullPadding = 20;

        // marker parameter
        private const int startMarkerDistance = 20;
        private const int markerOffsetX = 20;
        private const int markerStep = 10;

        // scale parameter
        private const int minTemp = -30;
        private const int maxTemp = 50;

        // indicator
        private const int indicatorOffset = 350;
        // from 0.0 to 1.0
        private const float indicatorPositionY = 0.8f;

        private void DrawFluidGlass(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);

            // fluid glass
            SKPoint bottomFluidGlassHullCenter = new SKPoint(info.Width / 2, info.Height + bottomFluidGlassCircleCenterY);

            SKPaint fluidGlassHullPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.LightGray,
            };

            SKRect fluidTopGlass = new SKRect();
            fluidTopGlass.Left = bottomFluidGlassHullCenter.X - topFluidGlassWidth / 2;
            fluidTopGlass.Top = 0 + topFluidGlassCircleCenterY;
            fluidTopGlass.Right = bottomFluidGlassHullCenter.X + topFluidGlassWidth / 2;
            fluidTopGlass.Bottom = bottomFluidGlassHullCenter.Y;

            // top fluid glass
            canvas.DrawRect(fluidTopGlass, fluidGlassHullPaint);
            canvas.DrawCircle(bottomFluidGlassHullCenter.X, fluidTopGlass.Top, fluidTopGlass.Width / 2, fluidGlassHullPaint);

            // bottom fluid glass
            canvas.DrawCircle(bottomFluidGlassHullCenter.X, bottomFluidGlassHullCenter.Y, bottomFluidGlassHullRadius, fluidGlassHullPaint);
        }

        private void DrawFluid(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);

            // fluid
            SKPaint fluidPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.Red,
            };

            // fluid bottom
            canvas.DrawCircle(center.X, info.Height + bottomFluidGlassCircleCenterY, bottomFluidGlassHullRadius - fluidHullPadding, fluidPaint);

            // draw live
            float livePixelsYStart = info.Height + bottomFluidGlassCircleCenterY - bottomFluidGlassHullRadius - startMarkerDistance;
            float livePixelsYEnd = topFluidGlassCircleCenterY;

            // map real temp to pixels from marker start to end
            float temperatureY = Temperature.Map(minTemp, maxTemp, livePixelsYStart, livePixelsYEnd);

            // fluid top
            SKRect fluidTop = new SKRect
            {
                Top = temperatureY,
                Left = center.X - (topFluidGlassWidth / 2) + fluidHullPadding,
                Right = center.X + (topFluidGlassWidth / 2) - fluidHullPadding,
                Bottom = info.Height + bottomFluidGlassCircleCenterY
            };

            canvas.DrawRect(fluidTop, fluidPaint);
        }

        private void DrawMarker(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);

            // draw live
            float livePixelsYStart = info.Height + bottomFluidGlassCircleCenterY - bottomFluidGlassHullRadius - startMarkerDistance;
            float livePixelsYEnd = topFluidGlassCircleCenterY;

            // map real temp to pixels from marker start to end
            float temperatureY = Temperature.Map(minTemp, maxTemp, livePixelsYStart, livePixelsYEnd);

            int tempRange = Math.Abs(minTemp) + Math.Abs(maxTemp);
            int markerCount = tempRange / markerStep;

            float thermometerPixelHeight = livePixelsYStart - livePixelsYEnd;
            float pixelStep = thermometerPixelHeight / markerCount;

            float yMarkerStep = livePixelsYStart;

            SKPaint markerPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.LightGray,
                TextSize = 40,
                TextAlign = SKTextAlign.Center,
            };

            for (int i = 0; i <= markerCount; i++)
            {
                SKRect marker = new SKRect
                {
                    Left = center.X + (topFluidGlassWidth / 2) + markerOffsetX,
                    Top = yMarkerStep + 2,
                    Bottom = yMarkerStep - 2
                };
                marker.Right = marker.Left + 100;

                canvas.DrawRect(marker, markerPaint);
                canvas.DrawText((minTemp + (i * markerStep)).ToString(), marker.Right - 30, yMarkerStep - 10, markerPaint);
                yMarkerStep -= pixelStep;
            }

            SKPaint indicatorPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColors.Black,
                StrokeWidth = 2,
                TextSize = 40,
                TextAlign = SKTextAlign.Left,
            };

            SKPoint indicatorTextPosition = new SKPoint(center.X - indicatorOffset, (livePixelsYStart - (thermometerPixelHeight * indicatorPositionY)));

            SKPoint indicatorUnderlineStart = new SKPoint(indicatorTextPosition.X, indicatorTextPosition.Y);
            SKPoint indicatorUnderlineEnd = new SKPoint(indicatorTextPosition.X + 120, indicatorTextPosition.Y);

            SKPoint connectionLineStart = new SKPoint(indicatorUnderlineEnd.X, indicatorUnderlineEnd.Y);
            SKPoint connectionLineEnd = new SKPoint(center.X, temperatureY);

            SKPoint[] points = new SKPoint[]
            {
                indicatorUnderlineStart,
                indicatorUnderlineEnd,
                connectionLineStart,
                connectionLineEnd,
            };

            canvas.DrawPoints(SKPointMode.Lines, points, indicatorPaint);
            canvas.DrawCircle(center.X, temperatureY, 10, indicatorPaint);
            canvas.DrawText(Temperature.ToString("0.00"), indicatorTextPosition.X, indicatorTextPosition.Y - 5, indicatorPaint);
        }

        public void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);

            DrawFluidGlass(e);
            DrawFluid(e);
            DrawMarker(e);
        }
    }

    public static class ExtensionMethods
    {
        public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}