﻿using FoxTunes.Interfaces;
using System;
using System.Windows;
using System.Windows.Threading;

namespace FoxTunes.ViewModel
{
    public class PlaybackState : ViewModelBase
    {
        private static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromMilliseconds(500);

        public static readonly IPlaybackManager PlaybackManager = ComponentRegistry.Instance.GetComponent<IPlaybackManager>();

        public static readonly DispatcherTimer Timer;

        static PlaybackState()
        {
            Timer = new DispatcherTimer(DispatcherPriority.Background);
            Timer.Interval = UPDATE_INTERVAL;
            Timer.Start();
        }

        public static readonly DependencyProperty PlaylistItemProperty = DependencyProperty.Register(
            "PlaylistItem",
            typeof(PlaylistItem),
            typeof(PlaybackState),
            new PropertyMetadata(new PropertyChangedCallback(OnPlaylistItemChanged))
        );

        public static PlaylistItem GetPlaylistItem(PlaybackState source)
        {
            return (PlaylistItem)source.GetValue(PlaylistItemProperty);
        }

        public static void SetPlaylistItem(PlaybackState source, PlaylistItem value)
        {
            source.SetValue(PlaylistItemProperty, value);
        }

        public static void OnPlaylistItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var playbackState = sender as PlaybackState;
            if (playbackState == null)
            {
                return;
            }
            playbackState.OnPlaylistItemChanged();
        }

        public PlaybackState()
        {
            if (Timer != null)
            {
                Timer.Tick += this.OnTick;
            }
        }

        public PlaylistItem PlaylistItem
        {
            get
            {
                return GetPlaylistItem(this);
            }
            set
            {
                SetPlaylistItem(this, value);
            }
        }

        protected virtual void OnPlaylistItemChanged()
        {
            if (this.PlaylistItemChanged != null)
            {
                this.PlaylistItemChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("PlaylistItem");
        }

        public event EventHandler PlaylistItemChanged;

        private bool _IsPlaying { get; set; }

        public bool IsPlaying
        {
            get
            {
                return this._IsPlaying;
            }
            set
            {
                this._IsPlaying = value;
                this.OnIsPlayingChanged();
            }
        }

        protected virtual void OnIsPlayingChanged()
        {
            if (this.IsPlayingChanged != null)
            {
                this.IsPlayingChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("IsPlaying");
        }

        public event EventHandler IsPlayingChanged;

        private bool _IsPaused { get; set; }

        public bool IsPaused
        {
            get
            {
                return this._IsPaused;
            }
            set
            {
                this._IsPaused = value;
                this.OnIsPausedChanged();
            }
        }

        protected virtual void OnIsPausedChanged()
        {
            if (this.IsPausedChanged != null)
            {
                this.IsPausedChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("IsPaused");
        }

        public event EventHandler IsPausedChanged;

        public string Caption
        {
            get
            {
                if (this.IsPaused)
                {
                    return ";";
                }
                if (this.IsPlaying)
                {
                    return "4";
                }
                return string.Empty;
            }
        }

        protected virtual void OnCaptionChanged()
        {
            if (this.CaptionChanged != null)
            {
                this.CaptionChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("Caption");
        }

        public event EventHandler CaptionChanged;

        protected virtual void OnTick(object sender, EventArgs e)
        {
            var isPlaying = default(bool);
            var isPaused = default(bool);
            if (PlaybackManager != null && this.PlaylistItem != null)
            {
                var currentStream = PlaybackManager.CurrentStream;
                if (currentStream != null)
                {
                    isPlaying = this.PlaylistItem.Id == currentStream.Id && string.Equals(this.PlaylistItem.FileName, currentStream.FileName, StringComparison.OrdinalIgnoreCase);
                    if (isPlaying)
                    {
                        isPaused = currentStream.IsPaused;
                    }
                }
            }
            var refresh = default(bool);
            if (this.IsPlaying != isPlaying)
            {
                this.IsPlaying = isPlaying;
                refresh = true;
            }
            if (this.IsPaused != isPaused)
            {
                this.IsPaused = isPaused;
                refresh = true;
            }
            if (refresh)
            {
                this.OnCaptionChanged();
            }
        }

        protected override void OnDisposing()
        {
            if (Timer != null)
            {
                Timer.Tick -= this.OnTick;
            }
            base.OnDisposing();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new PlaybackState();
        }
    }
}
