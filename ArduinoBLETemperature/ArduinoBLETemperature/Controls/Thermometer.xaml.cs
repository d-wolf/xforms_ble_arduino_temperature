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
    BindableProperty.Create(nameof(Temperature), typeof(float), typeof(Thermometer), 0.0f);

        public Thermometer ()
		{
			InitializeComponent ();

            Device.StartTimer(TimeSpan.FromMilliseconds(33), () =>
            {
               CanvasView.InvalidateSurface();
                return true;
            });
        }

        private void OnPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);



            // fluid glass
            SKPoint bottomFluidGlassHullCenter = new SKPoint(info.Width / 2, info.Height - 160);

            SKPaint fluidGlassHullPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.LightGray,
            };

            SKRect fluidTopGlass = new SKRect();
            fluidTopGlass.Left = bottomFluidGlassHullCenter.X - 40;
            fluidTopGlass.Top = 0 + 100;
            fluidTopGlass.Right = fluidTopGlass.Left + 80;
            fluidTopGlass.Bottom = bottomFluidGlassHullCenter.Y;

            // top fluid glass
            canvas.DrawRect(fluidTopGlass, fluidGlassHullPaint);
            canvas.DrawCircle(bottomFluidGlassHullCenter.X, fluidTopGlass.Top, fluidTopGlass.Width / 2, fluidGlassHullPaint);

            // bottom fluid glass
            canvas.DrawCircle(bottomFluidGlassHullCenter.X, bottomFluidGlassHullCenter.Y, 100f, fluidGlassHullPaint);


            // fluid
            SKPaint fluidPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.Red,
            };

            // fluid bottom
            canvas.DrawCircle(bottomFluidGlassHullCenter.X, bottomFluidGlassHullCenter.Y, 80f, fluidPaint);

            // draw marker
            SKPoint markerAreaStart = new SKPoint(bottomFluidGlassHullCenter.X + 100, bottomFluidGlassHullCenter.Y - 120);

            SKPoint markerAreaEnd = new SKPoint(markerAreaStart.X, fluidTopGlass.Top);

            int minTemp = -30;
            int maxTemp = 50;
            int step = 10;
            int tempRange = Math.Abs(minTemp) + Math.Abs(maxTemp);
            int markerCount = tempRange / step;

            float thermometerPixelHeight = markerAreaStart.Y - markerAreaEnd.Y;
            float pixelStep = thermometerPixelHeight / markerCount;

            float yMarkerStep = markerAreaStart.Y;

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
                SKRect marker = new SKRect();
                marker.Left = markerAreaStart.X;
                marker.Top = yMarkerStep + 2;
                marker.Bottom = yMarkerStep - 2;
                marker.Right = marker.Left + 100;
                canvas.DrawRect(marker, markerPaint);
                canvas.DrawText((minTemp + (i * step)).ToString(), marker.Right - 30, yMarkerStep - 10, markerPaint);
                yMarkerStep -= pixelStep;
            }

            // draw live

            float temperatureY = Temperature.Map(minTemp, maxTemp, markerAreaStart.Y, markerAreaEnd.Y);

            // fluid top
            SKRect fluidTop = fluidTopGlass;
            fluidTop.Top = temperatureY;
            fluidTop.Left += 20;
            fluidTop.Right -= 20;

            canvas.DrawRect(fluidTop, fluidPaint);

            // live indicator
            SKPaint indicatorPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.Black,
                TextSize = 40,
                TextAlign = SKTextAlign.Center,
            };

            SKRect indicator = new SKRect();
            indicator.Left = bottomFluidGlassHullCenter.X;
            indicator.Top = temperatureY + 1;
            indicator.Bottom = temperatureY - 1;
            indicator.Right = bottomFluidGlassHullCenter.X + 400;
            canvas.DrawRect(indicator, indicatorPaint);
            canvas.DrawCircle(bottomFluidGlassHullCenter.X, temperatureY, 10, indicatorPaint);
            canvas.DrawText(Temperature.ToString("0.00"), indicator.Right - 50, indicator.Top - 10, indicatorPaint);
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