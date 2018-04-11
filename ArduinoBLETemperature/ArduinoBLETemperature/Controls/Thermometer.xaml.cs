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
        public int MinTemperature
        {
            get { return (int)GetValue(MinTemperatureProperty); }
            set { SetValue(MinTemperatureProperty, value); }
        }

        public static readonly BindableProperty MinTemperatureProperty =
    BindableProperty.Create(nameof(MinTemperature), typeof(int), typeof(Thermometer), -30);

        public int MaxTemperature
        {
            get { return (int)GetValue(MaxTemperatureProperty); }
            set { SetValue(MaxTemperatureProperty, value); }
        }

        public static readonly BindableProperty MaxTemperatureProperty =
    BindableProperty.Create(nameof(MaxTemperature), typeof(int), typeof(Thermometer), 50);

        // indicator
        private const int indicatorOffset = 350;
        // from 0.0 to 1.0
        private const float indicatorPositionY = 0.8f;

        // colors
        public Color GlassColor
        {
            get { return (Color)GetValue(GlassColorProperty); }
            set { SetValue(GlassColorProperty, value); }
        }

        public static readonly BindableProperty GlassColorProperty =
    BindableProperty.Create(nameof(GlassColor), typeof(Color), typeof(CircleScanner), Color.LightGray);

        public Color FluidColor
        {
            get { return (Color)GetValue(FluidColorProperty); }
            set { SetValue(FluidColorProperty, value); }
        }

        public static readonly BindableProperty FluidColorProperty =
    BindableProperty.Create(nameof(FluidColor), typeof(Color), typeof(CircleScanner), Color.Red);

        public Color MarkerColor
        {
            get { return (Color)GetValue(MarkerColorProperty); }
            set { SetValue(MarkerColorProperty, value); }
        }

        public static readonly BindableProperty MarkerColorProperty =
    BindableProperty.Create(nameof(MarkerColor), typeof(Color), typeof(CircleScanner), Color.LightGray);

        public Color IndicatorColor
        {
            get { return (Color)GetValue(IndicatorColorProperty); }
            set { SetValue(IndicatorColorProperty, value); }
        }

        public static readonly BindableProperty IndicatorColorProperty =
    BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(CircleScanner), Color.Black);

        public Color IndicatorTextColor
        {
            get { return (Color)GetValue(IndicatorTextColorProperty); }
            set { SetValue(IndicatorTextColorProperty, value); }
        }

        public static readonly BindableProperty IndicatorTextColorProperty =
    BindableProperty.Create(nameof(IndicatorTextColor), typeof(Color), typeof(CircleScanner), Color.Black);

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
                Color = GlassColor.ToSKColor(),
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
                Color = FluidColor.ToSKColor(),
            };

            // fluid bottom
            canvas.DrawCircle(center.X, info.Height + bottomFluidGlassCircleCenterY, bottomFluidGlassHullRadius - fluidHullPadding, fluidPaint);

            // draw live
            float livePixelsYStart = info.Height + bottomFluidGlassCircleCenterY - bottomFluidGlassHullRadius - startMarkerDistance;
            float livePixelsYEnd = topFluidGlassCircleCenterY;

            // map real temp to pixels from marker start to end
            float temperatureY = Temperature.Map(MinTemperature, MaxTemperature, livePixelsYStart, livePixelsYEnd);

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
            float temperatureY = Temperature.Map(MinTemperature, MaxTemperature, livePixelsYStart, livePixelsYEnd);

            int tempRange = Math.Abs(MinTemperature) + Math.Abs(MaxTemperature);
            int markerCount = tempRange / markerStep;

            float thermometerPixelHeight = livePixelsYStart - livePixelsYEnd;
            float pixelStep = thermometerPixelHeight / markerCount;

            float yMarkerStep = livePixelsYStart;

            SKPaint markerPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = MarkerColor.ToSKColor(),
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
                canvas.DrawText((MinTemperature + (i * markerStep)).ToString(), marker.Right - 30, yMarkerStep - 10, markerPaint);
                yMarkerStep -= pixelStep;
            }

            SKPaint indicatorGeometryPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                Color = IndicatorColor.ToSKColor(),
                StrokeWidth = 6,   
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

            canvas.DrawPoints(SKPointMode.Lines, points, indicatorGeometryPaint);
            canvas.DrawCircle(center.X, temperatureY, 10, indicatorGeometryPaint);

            SKPaint indicatorTextPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.StrokeAndFill,
                Color = IndicatorTextColor.ToSKColor(),
                TextSize = 50,
                TextAlign = SKTextAlign.Left,
            };

            canvas.DrawText(Temperature.ToString("0.00"), indicatorTextPosition.X, indicatorTextPosition.Y - 10, indicatorTextPaint);
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