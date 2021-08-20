﻿using FoxTunes.Interfaces;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace FoxTunes
{
    public class OscilloscopeRenderer : VisualizationBase
    {
        public OscilloscopeRendererData RendererData { get; private set; }

        public SelectionConfigurationElement Mode { get; private set; }

        public IntegerConfigurationElement Duration { get; private set; }

        public BooleanConfigurationElement Smooth { get; private set; }

        public IntegerConfigurationElement SmoothingFactor { get; private set; }

        public override void InitializeComponent(ICore core)
        {
            base.InitializeComponent(core);
            this.Mode = this.Configuration.GetElement<SelectionConfigurationElement>(
                OscilloscopeBehaviourConfiguration.SECTION,
                OscilloscopeBehaviourConfiguration.MODE_ELEMENT
            );
            this.Duration = this.Configuration.GetElement<IntegerConfigurationElement>(
                OscilloscopeBehaviourConfiguration.SECTION,
                OscilloscopeBehaviourConfiguration.DURATION_ELEMENT
            );
            this.Smooth = this.Configuration.GetElement<BooleanConfigurationElement>(
               VisualizationBehaviourConfiguration.SECTION,
               VisualizationBehaviourConfiguration.SMOOTH_ELEMENT
            );
            this.SmoothingFactor = this.Configuration.GetElement<IntegerConfigurationElement>(
               VisualizationBehaviourConfiguration.SECTION,
               VisualizationBehaviourConfiguration.SMOOTH_FACTOR_ELEMENT
            );
            this.Configuration.GetElement<IntegerConfigurationElement>(
               VisualizationBehaviourConfiguration.SECTION,
               VisualizationBehaviourConfiguration.INTERVAL_ELEMENT
            ).ConnectValue(value => this.UpdateInterval = value);
            this.Mode.ValueChanged += this.OnValueChanged;
            this.Duration.ValueChanged += this.OnValueChanged;
            this.Smooth.ValueChanged += this.OnValueChanged;
            this.SmoothingFactor.ValueChanged += this.OnValueChanged;
            var task = this.CreateBitmap();
        }

        protected virtual void OnValueChanged(object sender, EventArgs e)
        {
            var task = this.RefreshBitmap();
        }

        protected override void CreateViewBox()
        {
            this.RendererData = Create(
                this,
                this.Bitmap.PixelWidth,
                this.Bitmap.PixelHeight,
                OscilloscopeBehaviourConfiguration.GetDuration(this.Duration.Value),
                OscilloscopeBehaviourConfiguration.GetMode(this.Mode.Value)
            );
            this.Viewbox = new Rect(0, 0, this.Bitmap.PixelWidth, this.Bitmap.PixelHeight);
        }

        protected virtual Task RefreshBitmap()
        {
            return Windows.Invoke(() =>
            {
                this.RendererData = Create(
                    this,
                    this.Bitmap.PixelWidth,
                    this.Bitmap.PixelHeight,
                    OscilloscopeBehaviourConfiguration.GetDuration(this.Duration.Value),
                    OscilloscopeBehaviourConfiguration.GetMode(this.Mode.Value)
                );
            });
        }

        protected virtual async Task Render()
        {
            var bitmap = default(WriteableBitmap);
            var success = default(bool);
            var info = default(BitmapHelper.RenderInfo);

            await Windows.Invoke(() =>
            {
                bitmap = this.Bitmap;
                success = bitmap.TryLock(LockTimeout);
                if (!success)
                {
                    return;
                }
                info = BitmapHelper.CreateRenderInfo(bitmap, this.Color);
            }).ConfigureAwait(false);

            if (!success)
            {
                //Failed to establish lock.
                this.Start();
                return;
            }

            Render(info, this.RendererData);

            await Windows.Invoke(() =>
            {
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                bitmap.Unlock();
            }).ConfigureAwait(false);

            this.Start();
        }

        protected override void OnElapsed(object sender, ElapsedEventArgs e)
        {
            var data = this.RendererData;
            if (data == null)
            {
                this.Start();
                return;
            }
            try
            {
                if (data.LastUpdated < DateTime.UtcNow - data.Duration)
                {
                    if (!data.Update())
                    {
                        data.Clear();
                    }
                    UpdateValues(data);
                    data.LastUpdated = DateTime.UtcNow;
                }

                switch (data.Mode)
                {
                    default:
                    case OscilloscopeRendererMode.Mono:
                        if (this.Smooth.Value)
                        {
                            UpdateElementsSmoothMono(data.Values, data.Peaks, data.Elements, data.Width, data.Height, this.SmoothingFactor.Value);
                        }
                        else
                        {
                            UpdateElementsFastMono(data.Values, data.Peaks, data.Elements, data.Width, data.Height);
                        }
                        break;
                    case OscilloscopeRendererMode.Seperate:
                        if (this.Smooth.Value)
                        {
                            UpdateElementsSmoothSeperate(data.Values, data.Peaks, data.Elements, data.Width, data.Height, data.Channels, this.SmoothingFactor.Value);
                        }
                        else
                        {
                            UpdateElementsFastSeperate(data.Values, data.Peaks, data.Elements, data.Width, data.Height, data.Channels);
                        }
                        break;
                }

                var task = this.Render();
            }
            catch (Exception exception)
            {
                Logger.Write(this.GetType(), LogLevel.Warn, "Failed to update spectrum data, disabling: {0}", exception.Message);
            }
        }

        private static void UpdateElementsFastMono(OscilloscopeRendererDataValue[,] values, float[] peaks, Int32Point[,] elements, int width, int height)
        {
            height = height - 1;
            var center = height / 2;
            var range = width / 50;
            var peak = peaks[0];
            if (values.Length == 0 || peak == 0.0f)
            {
                return;
            }
            for (var x = 0; x < width; x++)
            {
                var y = default(int);
                var value = GetValue(values, 0, x, range, width);
                if (value < 0)
                {
                    y = center + Convert.ToInt32((Math.Abs(value) / peak) * (height / 2));
                }
                else if (value > 0)
                {
                    y = center - Convert.ToInt32((Math.Abs(value) / peak) * (height / 2));
                }
                else
                {
                    y = center;
                }
                elements[0, x].X = x;
                elements[0, x].Y = y;
            }
        }

        private static void UpdateElementsFastSeperate(OscilloscopeRendererDataValue[,] values, float[] peaks, Int32Point[,] elements, int width, int height, int channels)
        {
            if (values.Length == 0 || channels == 0)
            {
                return;
            }
            height = height / channels;
            var range = width / 50;
            for (var channel = 0; channel < channels; channel++)
            {
                var center = (height * channel) + (height / 2);
                var peak = peaks[channel];
                if (peak == 0.0f)
                {
                    continue;
                }
                for (var x = 0; x < width; x++)
                {
                    var y = default(int);
                    var value = GetValue(values, channel, x, range, width);
                    if (value < 0)
                    {
                        y = center + Convert.ToInt32((Math.Abs(value) / peak) * (height / 2));
                    }
                    else if (value > 0)
                    {
                        y = center - Convert.ToInt32((Math.Abs(value) / peak) * (height / 2));
                    }
                    else
                    {
                        y = center;
                    }
                    elements[channel, x].X = x;
                    elements[channel, x].Y = y;
                }
            }
        }

        private static void UpdateElementsSmoothMono(OscilloscopeRendererDataValue[,] values, float[] peaks, Int32Point[,] elements, int width, int height, int smoothing)
        {
            if (values.Length == 0 || peaks[0] == 0.0f)
            {
                return;
            }
            height = height / 2;
            var center = height;
            var range = width / 50;
            var peak = peaks[0];
            for (var x = 0; x < width; x++)
            {
                var y = default(int);
                var value = GetValue(values, 0, x, range, width);
                if (value < 0)
                {
                    y = center + Convert.ToInt32((Math.Abs(value) / peak) * height);
                }
                else if (value > 0)
                {
                    y = center - Convert.ToInt32((Math.Abs(value) / peak) * height);
                }
                else
                {
                    y = center;
                }
                elements[0, x].X = x;
                Animate(ref elements[0, x].Y, y, center - height, center + height, smoothing);
            }
        }

        private static void UpdateElementsSmoothSeperate(OscilloscopeRendererDataValue[,] values, float[] peaks, Int32Point[,] elements, int width, int height, int channels, int smoothing)
        {
            height = (height / channels) / 2;
            var range = width / 50;
            if (values.Length == 0)
            {
                return;
            }
            for (var channel = 0; channel < channels; channel++)
            {
                var center = ((height * 2) * channel) + height;
                var peak = peaks[channel];
                if (peak == 0.0f)
                {
                    continue;
                }
                for (var x = 0; x < width; x++)
                {
                    var y = default(int);
                    var value = GetValue(values, channel, x, range, width);
                    if (value < 0)
                    {
                        y = center + Convert.ToInt32((Math.Abs(value) / peak) * height);
                    }
                    else if (value > 0)
                    {
                        y = center - Convert.ToInt32((Math.Abs(value) / peak) * height);
                    }
                    else
                    {
                        y = center;
                    }
                    elements[channel, x].X = x;
                    Animate(ref elements[channel, x].Y, y, center - height, center + height, smoothing);
                }
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new OscilloscopeRenderer();
        }

        protected override void OnDisposing()
        {
            PlaybackStateNotifier.Notify -= this.OnNotify;
            lock (this.SyncRoot)
            {
                if (this.Timer != null)
                {
                    this.Timer.Elapsed -= this.OnElapsed;
                    this.Timer.Dispose();
                    this.Timer = null;
                }
            }
            if (this.Smooth != null)
            {
                this.Smooth.ValueChanged -= this.OnValueChanged;
            }
            if (this.SmoothingFactor != null)
            {
                this.SmoothingFactor.ValueChanged -= this.OnValueChanged;
            }
        }

        private static void Render(BitmapHelper.RenderInfo info, OscilloscopeRendererData data)
        {
            BitmapHelper.Clear(info);

            if (data.Elements != null)
            {
                switch (data.Mode)
                {
                    default:
                    case OscilloscopeRendererMode.Mono:
                        RenderMono(info, data.Elements, data.Width);
                        break;
                    case OscilloscopeRendererMode.Seperate:
                        RenderSeperate(info, data.Elements, data.Channels, data.Width);
                        break;
                }
            }
        }

        private static void RenderMono(BitmapHelper.RenderInfo info, Int32Point[,] elements, int width)
        {
            for (var x = 0; x < width - 1; x++)
            {
                var point1 = elements[0, x];
                var point2 = elements[0, x + 1];
                BitmapHelper.DrawLine(
                    info,
                    point1.X,
                    point1.Y,
                    point2.X,
                    point2.Y
                );
            }
        }

        private static void RenderSeperate(BitmapHelper.RenderInfo info, Int32Point[,] elements, int channels, int width)
        {
            for (var channel = 0; channel < channels; channel++)
            {
                for (var x = 0; x < width - 1; x++)
                {
                    var point1 = elements[channel, x];
                    var point2 = elements[channel, x + 1];
                    BitmapHelper.DrawLine(
                        info,
                        point1.X,
                        point1.Y,
                        point2.X,
                        point2.Y
                    );
                }
            }
        }

        private static void UpdateValues(OscilloscopeRendererData data)
        {
            switch (data.Format)
            {
                case OutputStreamFormat.Short:
                    UpdateValues(data.Samples16, data.Samples32, data.SampleCount);
                    break;
            }
            switch (data.Mode)
            {
                default:
                case OscilloscopeRendererMode.Mono:
                    UpdateValuesMono(data.Samples32, data.Values, data.Peaks, data.Width, data.SampleCount);
                    break;
                case OscilloscopeRendererMode.Seperate:
                    UpdateValuesSeperate(data.Samples, data.Values, data.Peaks, data.Channels, data.Width, data.SampleCount);
                    break;
            }
        }

        private static void UpdateValues(short[] samples16, float[] samples32, int count)
        {
            for (var a = 0; a < count; a++)
            {
                samples32[a] = samples16[a] / short.MaxValue;
            }
        }

        private static void UpdateValuesMono(float[] samples, OscilloscopeRendererDataValue[,] values, float[] peaks, int width, int count)
        {
            if (count == 0 || width == 0)
            {
                return;
            }
            var samplesPerValue = count / width;
            peaks[0] = 0.1f;
            if (samplesPerValue > 0)
            {
                for (int a = 0, x = 0; a < count && x < width; a += samplesPerValue, x++)
                {
                    values[0, x].Min = 0;
                    values[0, x].Max = 0;
                    for (var b = 0; b < samplesPerValue; b++)
                    {
                        var sample = samples[a + b];
                        values[0, x].Min = Math.Min(sample, values[0, x].Min);
                        values[0, x].Max = Math.Max(sample, values[0, x].Max);
                    }
                    peaks[0] = Math.Max(Math.Max(Math.Abs(values[0, x].Min), Math.Abs(values[0, x].Max)), peaks[0]);
                }
            }
            else
            {
                var valuesPerSample = (float)width / count;
                for (var x = 0; x < width; x++)
                {
                    var sample = samples[Convert.ToInt32(x / valuesPerSample)];
                    values[0, x].Min = Math.Min(sample, 0);
                    values[0, x].Max = Math.Max(sample, 0);
                    peaks[0] = Math.Max(Math.Max(Math.Abs(values[0, x].Min), Math.Abs(values[0, x].Max)), peaks[0]);
                }
            }
        }

        private static void UpdateValuesSeperate(float[,] samples, OscilloscopeRendererDataValue[,] values, float[] peaks, int channels, int width, int count)
        {
            if (channels == 0 || width == 0 || count == 0)
            {
                return;
            }
            var samplesPerValue = (count / channels) / width;
            Array.Clear(peaks, 0, peaks.Length);
            if (samplesPerValue > 0)
            {
                for (int a = 0, x = 0; a < count && x < width; a += samplesPerValue, x++)
                {
                    for (var channel = 0; channel < channels; channel++)
                    {
                        values[channel, x].Min = 0;
                        values[channel, x].Max = 0;
                        for (var b = 0; b < samplesPerValue; b++)
                        {
                            var sample = samples[channel, a + b];
                            values[channel, x].Min = Math.Min(sample, values[channel, x].Min);
                            values[channel, x].Max = Math.Max(sample, values[channel, x].Max);
                        }
                        peaks[channel] = Math.Max(Math.Max(Math.Abs(values[channel, x].Min), Math.Abs(values[channel, x].Max)), peaks[channel]);
                    }
                }
            }
            else
            {
                var valuesPerSample = (float)width / count;
                for (var x = 0; x < width; x++)
                {
                    for (var channel = 0; channel < channels; channel++)
                    {
                        var sample = samples[channel, Convert.ToInt32(x / valuesPerSample)];
                        values[channel, x].Min = Math.Min(sample, 0);
                        values[channel, x].Max = Math.Max(sample, 0);
                        peaks[channel] = Math.Max(Math.Max(Math.Abs(values[channel, x].Min), Math.Abs(values[channel, x].Max)), peaks[channel]);
                    }
                }
            }
        }

        private static float GetValue(OscilloscopeRendererDataValue[,] values, int channel, int index, int range, int width)
        {
            var start = default(int);
            var end = default(int);
            if (index > range)
            {
                start = index - range;
            }
            else
            {
                start = 0;
            }
            if (index + range < width)
            {
                end = index + range;
            }
            else
            {
                end = width - 1;
            }
            var value = default(float);
            for (var a = start; a < end; a++)
            {
                value += values[channel, a].Min + values[channel, a].Max;
            }
            value /= (end - start);
            return value;
        }

        public static OscilloscopeRendererData Create(OscilloscopeRenderer renderer, int width, int height, TimeSpan duration, OscilloscopeRendererMode mode)
        {
            var data = new OscilloscopeRendererData()
            {
                Renderer = renderer,
                Width = width,
                Height = height,
                Duration = duration,
                Mode = mode
            };
            return data;
        }

        public class OscilloscopeRendererData
        {
            public OscilloscopeRenderer Renderer;

            public int Width;

            public int Height;

            public int Rate;

            public int Channels;

            public OutputStreamFormat Format;

            public short[] Samples16;

            public float[] Samples32;

            public float[,] Samples;

            public int SampleCount;

            public OscilloscopeRendererDataValue[,] Values;

            public float[] Peaks;

            public Int32Point[,] Elements;

            public DateTime LastUpdated;

            public TimeSpan Duration;

            public OscilloscopeRendererMode Mode;

            public bool Update()
            {
                var rate = default(int);
                var channels = default(int);
                var format = default(OutputStreamFormat);
                if (!this.Renderer.Output.GetFormat(out rate, out channels, out format))
                {
                    return false;
                }
                this.Update(rate, channels, format);
                switch (this.Format)
                {
                    case OutputStreamFormat.Short:
                        this.SampleCount = this.Renderer.Output.GetData(this.Samples16) / sizeof(short);
                        break;
                    case OutputStreamFormat.Float:
                        this.SampleCount = this.Renderer.Output.GetData(this.Samples32) / sizeof(float);
                        break;
                }
                if (this.Rate > 0 && this.Channels > 0 && this.SampleCount > 0)
                {
                    switch (this.Mode)
                    {
                        case OscilloscopeRendererMode.Mono:
                            //Nothing to do.
                            break;
                        case OscilloscopeRendererMode.Seperate:
                            for (int a = 0, b = 0; a < this.SampleCount; a += this.Channels, b++)
                            {
                                for (var channel = 0; channel < this.Channels; channel++)
                                {
                                    this.Samples[channel, b] = this.Samples32[a + channel];
                                }
                            }
                            this.SampleCount /= this.Channels;
                            break;
                    }
                    return true;
                }
                return false;
            }

            private void Update(int rate, int channels, OutputStreamFormat format)
            {
                if (this.Rate == rate && this.Channels == channels && this.Format == format)
                {
                    return;
                }

                this.Rate = rate;
                this.Channels = channels;
                this.Format = format;

                if (format == OutputStreamFormat.Short)
                {
                    this.Samples16 = this.Renderer.Output.GetBuffer<short>(TimeSpan.FromMilliseconds(this.Renderer.UpdateInterval));
                    this.Samples32 = new float[this.Samples16.Length];
                }
                else if (format == OutputStreamFormat.Float)
                {
                    this.Samples32 = this.Renderer.Output.GetBuffer<float>(TimeSpan.FromMilliseconds(this.Renderer.UpdateInterval));
                }

                //TODO: Only realloc if required.
                switch (this.Mode)
                {
                    default:
                    case OscilloscopeRendererMode.Mono:
                        this.Values = new OscilloscopeRendererDataValue[1, this.Width];
                        this.Elements = CreateElements(1, this.Width, this.Height);
                        this.Peaks = new float[1];
                        break;
                    case OscilloscopeRendererMode.Seperate:
                        this.Samples = new float[this.Channels, this.Samples32.Length];
                        this.Values = new OscilloscopeRendererDataValue[this.Channels, this.Width];
                        this.Elements = CreateElements(this.Channels, this.Width, this.Height);
                        this.Peaks = new float[this.Channels];
                        break;
                }
            }

            private Int32Point[,] CreateElements(int channels, int width, int height)
            {
                height = height / channels;
                var elements = new Int32Point[channels, width];
                for (var channel = 0; channel < channels; channel++)
                {
                    var center = (height * channel) + (height / 2);
                    for (var x = 0; x < width; x++)
                    {
                        elements[channel, x].X = x;
                        elements[channel, x].Y = center;
                    }
                }
                return elements;
            }

            public void Clear()
            {
                if (this.Samples16 != null)
                {
                    Array.Clear(this.Samples16, 0, this.Samples16.Length);
                }
                if (this.Samples32 != null)
                {
                    Array.Clear(this.Samples32, 0, this.Samples32.Length);
                }
                this.SampleCount = 0;
            }
        }

        public struct OscilloscopeRendererDataValue
        {
            public float Min;

            public float Max;
        }
    }

    public enum OscilloscopeRendererMode : byte
    {
        None,
        Mono,
        Seperate
    }
}