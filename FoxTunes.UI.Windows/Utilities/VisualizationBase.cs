﻿using FoxTunes.Interfaces;
using System;
using System.Timers;

namespace FoxTunes
{
    public abstract class VisualizationBase : RendererBase
    {
        public readonly object SyncRoot = new object();

        public global::System.Timers.Timer Timer;

        public VisualizationBase()
        {
            this.Timer = new global::System.Timers.Timer();
            this.Timer.AutoReset = false;
            this.Timer.Elapsed += this.OnElapsed;
        }

        public bool Enabled { get; private set; }

        public int UpdateInterval
        {
            get
            {
                return Convert.ToInt32(this.Timer.Interval);
            }
            protected set
            {
                this.Timer.Interval = value;
            }
        }

        public override void InitializeComponent(ICore core)
        {
            PlaybackStateNotifier.Notify += this.OnNotify;
            base.InitializeComponent(core);
        }

        protected virtual void OnNotify(object sender, EventArgs e)
        {
            if (PlaybackStateNotifier.IsPlaying && !this.Enabled)
            {
                Logger.Write(this, LogLevel.Debug, "Playback was started, starting renderer.");
                this.Start();
            }
            else if (!PlaybackStateNotifier.IsPlaying && this.Enabled)
            {
                Logger.Write(this, LogLevel.Debug, "Playback was stopped, stopping renderer.");
                this.Stop();
            }
        }

        public void Start()
        {
            lock (this.SyncRoot)
            {
                if (this.Timer != null)
                {
                    this.Timer.Start();
                    this.Enabled = true;
                }
            }
        }

        public void Stop()
        {
            lock (this.SyncRoot)
            {
                if (this.Timer != null)
                {
                    this.Timer.Stop();
                    this.Enabled = false;
                }
            }
            if (!PlaybackStateNotifier.IsPlaying && !PlaybackStateNotifier.IsPaused)
            {
                var task = this.Clear();
            }
        }

        protected abstract void OnElapsed(object sender, ElapsedEventArgs e);

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
            base.OnDisposing();
        }
    }
}