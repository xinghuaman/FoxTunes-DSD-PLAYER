﻿using FoxDb;
using FoxTunes.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoxTunes
{
    [ComponentDependency(Slot = ComponentSlots.Database)]
    public class PlaylistManager : StandardManager, IPlaylistManager
    {
        public const string CLEAR_PLAYLIST = "ZZZZ";

        public PlaylistManager()
        {
            this._SelectedItems = new ConcurrentDictionary<Playlist, PlaylistItem[]>();
        }

        private volatile bool IsNavigating = false;

        public ICore Core { get; private set; }

        public IPlaylistBrowser PlaylistBrowser { get; private set; }

        public IDatabaseFactory DatabaseFactory { get; private set; }

        public IPlaybackManager PlaybackManager { get; private set; }

        public ISignalEmitter SignalEmitter { get; private set; }

        public override void InitializeComponent(ICore core)
        {
            this.Core = core;
            this.PlaylistBrowser = core.Components.PlaylistBrowser;
            this.DatabaseFactory = core.Factories.Database;
            this.PlaybackManager = core.Managers.Playback;
            this.PlaybackManager.CurrentStreamChanged += this.OnCurrentStreamChanged;
            this.SignalEmitter = core.Components.SignalEmitter;
            this.SignalEmitter.Signal += this.OnSignal;
            this.Refresh();
            base.InitializeComponent(core);
        }

        protected virtual Task OnSignal(object sender, ISignal signal)
        {
            switch (signal.Name)
            {
                case CommonSignals.PlaylistUpdated:
                    var playlists = signal.State as IEnumerable<Playlist>;
                    if (playlists != null)
                    {
                        this.RefreshSelectedItems(playlists);
                        if (this.SelectedPlaylist == null || playlists.Contains(this.SelectedPlaylist))
                        {
                            this.RefreshSelectedPlaylist();
                        }
                        if (this.CurrentPlaylist != null && playlists.Contains(this.CurrentPlaylist))
                        {
                            this.RefreshCurrentPlaylist();
                        }
                        if (this.CurrentItem != null && playlists.Any(playlist => playlist.Id == this.CurrentItem.Playlist_Id))
                        {
                            this.RefreshCurrentItem();
                        }
                    }
                    else
                    {
                        this.Refresh();
                    }
                    break;
            }
#if NET40
            return TaskEx.FromResult(false);
#else
            return Task.CompletedTask;
#endif
        }

        public void Refresh()
        {
            this.RefreshSelectedItems();
            this.RefreshSelectedPlaylist();
            this.RefreshCurrentPlaylist();
            this.RefreshCurrentItem();
        }

        protected virtual void RefreshSelectedItems()
        {
            this._SelectedItems.Clear();
            this.OnSelectedItemsChanged();
        }

        protected virtual void RefreshSelectedItems(IEnumerable<Playlist> playlists)
        {
            foreach (var playlist in playlists)
            {
                this._SelectedItems.TryRemove(playlist);
            }
            this.OnSelectedItemsChanged();
        }

        protected virtual void RefreshSelectedPlaylist()
        {
            if (this.SelectedPlaylist != null)
            {
                this.SelectedPlaylist = this.PlaylistBrowser.GetPlaylists().FirstOrDefault(playlist => playlist.Id == this.SelectedPlaylist.Id);
                if (this.SelectedPlaylist != null)
                {
                    Logger.Write(this, LogLevel.Debug, "Refreshed selected playlist: {0} => {1}", this.SelectedPlaylist.Id, this.SelectedPlaylist.Name);
                }
                else
                {
                    Logger.Write(this, LogLevel.Debug, "Failed to refresh selected playlist, it was removed or disabled.");
                }
            }
            if (this.SelectedPlaylist == null)
            {
                this.SelectedPlaylist = this.PlaylistBrowser.GetPlaylists().FirstOrDefault();
                if (this.SelectedPlaylist == null)
                {
                    Logger.Write(this, LogLevel.Warn, "Failed to select a playlist, perhaps none are enabled?");
                }
                else
                {
                    Logger.Write(this, LogLevel.Debug, "Selected first playlist: {0} => {1}", this.SelectedPlaylist.Id, this.SelectedPlaylist.Name);
                }
            }
        }

        protected virtual void RefreshCurrentPlaylist()
        {
            if (this.CurrentPlaylist != null)
            {
                this.CurrentPlaylist = this.PlaylistBrowser.GetPlaylists().FirstOrDefault(playlist => playlist.Id == this.CurrentPlaylist.Id);
                if (this.CurrentPlaylist != null)
                {
                    Logger.Write(this, LogLevel.Debug, "Refreshed current playlist: {0} => {1}", this.CurrentPlaylist.Id, this.CurrentPlaylist.Name);
                }
                else
                {
                    Logger.Write(this, LogLevel.Debug, "Failed to refresh current playlist, it was removed or disabled.");
                }
            }
        }

        protected virtual void RefreshCurrentItem()
        {
            if (this.CurrentItem != null)
            {
                var playlist = this.PlaylistBrowser.GetPlaylist(this.CurrentItem);
                if (playlist != null)
                {
                    this.CurrentItem = this.PlaylistBrowser.GetItems(playlist).FirstOrDefault(
                        playlistItem => playlistItem.Id == this.CurrentItem.Id && string.Equals(this.CurrentItem.FileName, playlistItem.FileName, StringComparison.OrdinalIgnoreCase)
                    );
                }
            }
            if (this.CurrentItem == null)
            {
                Logger.Write(this, LogLevel.Warn, "Failed to refresh current item.");
            }
        }

        protected virtual void OnCurrentStreamChanged(object sender, EventArgs e)
        {
            Logger.Write(this, LogLevel.Debug, "Playback manager output stream changed, updating current playlist item.");
            if (this.PlaybackManager.CurrentStream == null)
            {
                this.CurrentItem = null;
                Logger.Write(this, LogLevel.Debug, "Playback manager output stream is empty. Cleared current playlist item.");
            }
            else if (this.PlaybackManager.CurrentStream.PlaylistItem != this.CurrentItem)
            {
                this.CurrentItem = this.PlaybackManager.CurrentStream.PlaylistItem;
                Logger.Write(this, LogLevel.Debug, "Updated current playlist item: {0} => {1}", this.CurrentItem.Id, this.CurrentItem.FileName);
            }
        }

        public async Task Add(Playlist playlist)
        {
            using (var task = new AddPlaylistTask(playlist))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Add(Playlist playlist, LibraryHierarchyNode libraryHierarchyNode)
        {
            using (var task = new AddPlaylistTask(playlist, libraryHierarchyNode))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Remove(Playlist playlist)
        {
            using (var task = new RemovePlaylistTask(playlist))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Add(Playlist playlist, IEnumerable<string> paths, bool clear)
        {
            Logger.Write(this, LogLevel.Debug, "Adding paths to playlist.");
            var index = await this.PlaylistBrowser.GetInsertIndex(this.SelectedPlaylist).ConfigureAwait(false);
            await this.Insert(playlist, index, paths, clear).ConfigureAwait(false);
        }

        public async Task Insert(Playlist playlist, int index, IEnumerable<string> paths, bool clear)
        {
            Logger.Write(this, LogLevel.Debug, "Inserting paths into playlist at index: {0}", index);
            using (var task = new AddPathsToPlaylistTask(playlist, index, paths, clear))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Add(Playlist playlist, LibraryHierarchyNode libraryHierarchyNode, bool clear)
        {
            Logger.Write(this, LogLevel.Debug, "Adding library node to playlist.");
            var index = await this.PlaylistBrowser.GetInsertIndex(this.SelectedPlaylist).ConfigureAwait(false);
            await this.Insert(playlist, index, libraryHierarchyNode, clear).ConfigureAwait(false);
        }

        public async Task Insert(Playlist playlist, int index, LibraryHierarchyNode libraryHierarchyNode, bool clear)
        {
            Logger.Write(this, LogLevel.Debug, "Inserting library node into playlist at index: {0}", index);
            using (var task = new AddLibraryHierarchyNodeToPlaylistTask(playlist, index, libraryHierarchyNode, clear))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Add(Playlist playlist, IEnumerable<LibraryHierarchyNode> libraryHierarchyNodes, bool clear)
        {
            Logger.Write(this, LogLevel.Debug, "Adding library nodes to playlist.");
            var index = await this.PlaylistBrowser.GetInsertIndex(this.SelectedPlaylist).ConfigureAwait(false);
            await this.Insert(playlist, index, libraryHierarchyNodes, clear).ConfigureAwait(false);
        }

        public async Task Insert(Playlist playlist, int index, IEnumerable<LibraryHierarchyNode> libraryHierarchyNodes, bool clear)
        {
            Logger.Write(this, LogLevel.Debug, "Inserting library nodes into playlist at index: {0}", index);
            using (var task = new AddLibraryHierarchyNodesToPlaylistTask(playlist, index, libraryHierarchyNodes, clear))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Move(Playlist playlist, IEnumerable<PlaylistItem> playlistItems)
        {
            Logger.Write(this, LogLevel.Debug, "Re-ordering playlist items.");
            var index = await this.PlaylistBrowser.GetInsertIndex(this.SelectedPlaylist).ConfigureAwait(false);
            await this.Move(playlist, index, playlistItems).ConfigureAwait(false);
        }

        public async Task Move(Playlist playlist, int index, IEnumerable<PlaylistItem> playlistItems)
        {
            Logger.Write(this, LogLevel.Debug, "Re-ordering playlist items at index: {0}", index);
            using (var task = new MovePlaylistItemsTask(playlist, index, playlistItems))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Remove(Playlist playlist, IEnumerable<PlaylistItem> playlistItems)
        {
            using (var task = new RemoveItemsFromPlaylistTask(playlist, playlistItems))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Crop(Playlist playlist, IEnumerable<PlaylistItem> playlistItems)
        {
            using (var database = this.DatabaseFactory.Create())
            {
                using (var transaction = database.BeginTransaction(database.PreferredIsolationLevel))
                {
                    var queryable = database.AsQueryable<PlaylistItem>(transaction);
                    var query = queryable.Where(
                        playlistItem => playlistItem.Playlist_Id == playlist.Id
                    ).Except(playlistItems);
                    //TODO: Warning: Buffering a potentially large sequence.
                    playlistItems = query.ToArray();
                }
            }
            using (var task = new RemoveItemsFromPlaylistTask(playlist, playlistItems))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task Next()
        {
            Logger.Write(this, LogLevel.Debug, "Navigating to next playlist item.");
            if (this.IsNavigating)
            {
                Logger.Write(this, LogLevel.Debug, "Already navigating, ignoring request.");
                return;
            }
            try
            {
                this.IsNavigating = true;
                var playlistItem = await this.PlaylistBrowser.GetNextItem(this.CurrentPlaylist).ConfigureAwait(false);
                if (playlistItem == null)
                {
                    return;
                }
                Logger.Write(this, LogLevel.Debug, "Playing playlist item: {0} => {1}", playlistItem.Id, playlistItem.FileName);
                await this.Play(playlistItem).ConfigureAwait(false);
            }
            finally
            {
                this.IsNavigating = false;
            }
        }

        public async Task Previous()
        {
            Logger.Write(this, LogLevel.Debug, "Navigating to previous playlist item.");
            if (this.IsNavigating)
            {
                Logger.Write(this, LogLevel.Debug, "Already navigating, ignoring request.");
                return;
            }
            try
            {
                this.IsNavigating = true;
                var playlistItem = await this.PlaylistBrowser.GetPreviousItem(this.CurrentPlaylist).ConfigureAwait(false);
                if (playlistItem == null)
                {
                    return;
                }
                Logger.Write(this, LogLevel.Debug, "Playing playlist item: {0} => {1}", playlistItem.Id, playlistItem.FileName);
                await this.Play(playlistItem).ConfigureAwait(false);
            }
            finally
            {
                this.IsNavigating = false;
            }
        }

        public async Task Play(PlaylistItem playlistItem)
        {
            var outputStream = this.PlaybackManager.CurrentStream;
            if (outputStream != null && outputStream.PlaylistItem == playlistItem)
            {
                outputStream.Position = 0;
                return;
            }
            var exception = default(Exception);
            try
            {
                await this.PlaybackManager.Load(playlistItem, true).ConfigureAwait(false);
                return;
            }
            catch (Exception e)
            {
                exception = e;
            }
            await this.OnError(exception).ConfigureAwait(false);
        }

        public async Task Play(Playlist playlist, string fileName)
        {
            var playlistItem = await this.PlaylistBrowser.GetItem(playlist, fileName).ConfigureAwait(false);
            if (playlistItem == null)
            {
                return;
            }
            await this.Play(playlistItem).ConfigureAwait(false);
        }

        public async Task Play(Playlist playlist, int sequence)
        {
            var playlistItem = await this.PlaylistBrowser.GetItem(playlist, sequence).ConfigureAwait(false);
            if (playlistItem == null)
            {
                return;
            }
            await this.Play(playlistItem).ConfigureAwait(false);
        }

        public async Task Clear(Playlist playlist)
        {
            using (var task = new ClearPlaylistTask(playlist))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        private Playlist _SelectedPlaylist { get; set; }

        public Playlist SelectedPlaylist
        {
            get
            {
                return this._SelectedPlaylist;
            }
            set
            {
                if (object.ReferenceEquals(this._SelectedPlaylist, value))
                {
                    return;
                }
                this._SelectedPlaylist = value;
                this.OnSelectedPlaylistChanged();
            }
        }

        protected virtual void OnSelectedPlaylistChanged()
        {
            if (this.SelectedPlaylistChanged != null)
            {
                this.SelectedPlaylistChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("SelectedPlaylist");
        }

        public event EventHandler SelectedPlaylistChanged;

        private Playlist _CurrentPlaylist { get; set; }

        public Playlist CurrentPlaylist
        {
            get
            {
                return this._CurrentPlaylist;
            }
            protected set
            {
                if (object.ReferenceEquals(this._CurrentPlaylist, value))
                {
                    return;
                }
                this._CurrentPlaylist = value;
                this.OnCurrentPlaylistChanged();
            }
        }

        protected virtual void OnCurrentPlaylistChanged()
        {
            if (this.CurrentPlaylistChanged != null)
            {
                this.CurrentPlaylistChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("CurrentPlaylist");
        }

        public event EventHandler CurrentPlaylistChanged;

        private PlaylistItem _CurrentItem { get; set; }

        public PlaylistItem CurrentItem
        {
            get
            {
                return this._CurrentItem;
            }
            protected set
            {
                if (object.ReferenceEquals(this._CurrentItem, value))
                {
                    return;
                }
                this._CurrentItem = value;
                this.OnCurrentItemChanged();
            }
        }

        protected virtual void OnCurrentItemChanged()
        {
            if (this.CurrentItem == null)
            {
                this.CurrentPlaylist = null;
            }
            else
            {
                this.CurrentPlaylist = this.PlaylistBrowser.GetPlaylist(this.CurrentItem);
            }
            if (this.CurrentItemChanged != null)
            {
                this.CurrentItemChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("CurrentItem");
        }

        public event EventHandler CurrentItemChanged;

        private ConcurrentDictionary<Playlist, PlaylistItem[]> _SelectedItems { get; set; }

        public PlaylistItem[] SelectedItems
        {
            get
            {
                var playlistItems = default(PlaylistItem[]);
                if (this.SelectedPlaylist == null || !this._SelectedItems.TryGetValue(this.SelectedPlaylist, out playlistItems))
                {
                    return new PlaylistItem[] { };
                }
                return playlistItems;
            }
            set
            {
                if (this.SelectedPlaylist == null || object.Equals(this.SelectedPlaylist, value))
                {
                    return;
                }
                this._SelectedItems[this.SelectedPlaylist] = value;
                this.OnSelectedItemsChanged();
            }
        }

        protected virtual void OnSelectedItemsChanged()
        {
            if (this.SelectedItemsChanged != null)
            {
                this.SelectedItemsChanged(this, EventArgs.Empty);
            }
            this.OnPropertyChanged("SelectedItems");
        }

        public event EventHandler SelectedItemsChanged;

        public async Task SetRating(IEnumerable<PlaylistItem> playlistItems, byte rating)
        {
            using (var task = new UpdatePlaylistRatingTask(playlistItems, rating))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        public async Task IncrementPlayCount(PlaylistItem playlistItem)
        {
            using (var task = new UpdatePlaylistPlayCountTask(playlistItem))
            {
                task.InitializeComponent(this.Core);
                await this.OnBackgroundTask(task).ConfigureAwait(false);
                await task.Run().ConfigureAwait(false);
            }
        }

        protected virtual Task OnBackgroundTask(IBackgroundTask backgroundTask)
        {
            if (this.BackgroundTask == null)
            {
#if NET40
                return TaskEx.FromResult(false);
#else
                return Task.CompletedTask;
#endif
            }
            var e = new BackgroundTaskEventArgs(backgroundTask);
            this.BackgroundTask(this, e);
            return e.Complete();
        }

        public event BackgroundTaskEventHandler BackgroundTask;

        public IEnumerable<IInvocationComponent> Invocations
        {
            get
            {
                yield return new InvocationComponent(InvocationComponent.CATEGORY_PLAYLIST, CLEAR_PLAYLIST, "Clear", attributes: InvocationComponent.ATTRIBUTE_SEPARATOR);
            }
        }

        public Task InvokeAsync(IInvocationComponent component)
        {
            switch (component.Id)
            {
                case CLEAR_PLAYLIST:
                    return this.Clear(this.SelectedPlaylist);
            }
#if NET40
            return TaskEx.FromResult(false);
#else
            return Task.CompletedTask;
#endif
        }

        public bool CanHandle(string path)
        {
            return Directory.Exists(path) || this.PlaybackManager.IsSupported(path);
        }

        public Task Handle(IEnumerable<string> paths)
        {
            return this.Add(this.SelectedPlaylist, paths, false);
        }

        public void InitializeDatabase(IDatabaseComponent database, DatabaseInitializeType type)
        {
            if (!type.HasFlag(DatabaseInitializeType.Playlist))
            {
                return;
            }
            var scriptingRuntime = ComponentRegistry.Instance.GetComponent<IScriptingRuntime>();
            if (scriptingRuntime == null)
            {
                return;
            }
            using (var transaction = database.BeginTransaction())
            {
                {
                    var set = database.Set<Playlist>(transaction);
                    set.Clear();
                    set.Add(new Playlist() { Name = "Default", Type = PlaylistType.None, Enabled = true });
                }
                {
                    var set = database.Set<PlaylistColumn>(transaction);
                    set.Clear();
                    set.Add(new PlaylistColumn() { Name = "Artist / album", Type = PlaylistColumnType.Script, Sequence = 1, Script = scriptingRuntime.CoreScripts.Artist_Album, Width = PlaylistColumn.WIDTH_LARGE, Enabled = true });
                    set.Add(new PlaylistColumn() { Name = "Track no", Type = PlaylistColumnType.Script, Sequence = 2, Script = scriptingRuntime.CoreScripts.Track, Enabled = true });
                    set.Add(new PlaylistColumn() { Name = "Title / track artist", Type = PlaylistColumnType.Script, Sequence = 3, Script = scriptingRuntime.CoreScripts.Title_Performer, Width = PlaylistColumn.WIDTH_LARGE, Enabled = true });
                    set.Add(new PlaylistColumn() { Name = "Duration", Type = PlaylistColumnType.Script, Sequence = 4, Script = scriptingRuntime.CoreScripts.Duration, Enabled = true });
                    set.Add(new PlaylistColumn() { Name = "Codec", Type = PlaylistColumnType.Script, Sequence = 5, Script = scriptingRuntime.CoreScripts.Codec, Enabled = true });
                    set.Add(new PlaylistColumn() { Name = "BPM", Type = PlaylistColumnType.Script, Sequence = 6, Script = scriptingRuntime.CoreScripts.BPM, Enabled = false });
                    set.Add(new PlaylistColumn() { Name = "Album gain", Type = PlaylistColumnType.Script, Sequence = 7, Script = scriptingRuntime.CoreScripts.ReplayGainAlbumGain, Enabled = false });
                    set.Add(new PlaylistColumn() { Name = "Album peak", Type = PlaylistColumnType.Script, Sequence = 8, Script = scriptingRuntime.CoreScripts.ReplayGainAlbumPeak, Enabled = false });
                    set.Add(new PlaylistColumn() { Name = "Track gain", Type = PlaylistColumnType.Script, Sequence = 9, Script = scriptingRuntime.CoreScripts.ReplayGainTrackGain, Enabled = false });
                    set.Add(new PlaylistColumn() { Name = "Track peak", Type = PlaylistColumnType.Script, Sequence = 10, Script = scriptingRuntime.CoreScripts.ReplayGainTrackPeak, Enabled = false });
                    set.Add(new PlaylistColumn() { Name = "Play count", Type = PlaylistColumnType.Script, Sequence = 11, Script = scriptingRuntime.CoreScripts.PlayCount, Enabled = false });
                    set.Add(new PlaylistColumn() { Name = "Last played", Type = PlaylistColumnType.Script, Sequence = 12, Script = scriptingRuntime.CoreScripts.LastPlayed, Enabled = false });
                }
                transaction.Commit();
            }
        }
    }
}
