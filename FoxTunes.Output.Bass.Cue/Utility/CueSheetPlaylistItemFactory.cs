﻿using FoxTunes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoxTunes
{
    public class CueSheetPlaylistItemFactory : PopulatorBase
    {
        private static IDictionary<string, string> FILE_NAMES = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "PERFORMER", CommonMetaData.Artist },
            { "DATE", CommonMetaData.Year },
            { "TITLE", CommonMetaData.Album }
        };

        private static IDictionary<string, string> TRACK_NAMES = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "PERFORMER", CommonMetaData.Artist },
            { "DATE", CommonMetaData.Year }
        };

        public CueSheetPlaylistItemFactory(bool reportProgress) : base(reportProgress)
        {
            this.DataFileNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, string> DataFileNames { get; private set; }

        public IOutput Output { get; private set; }

        public IMetaDataSourceFactory MetaDataSourceFactory { get; private set; }

        public IFileSystemBrowser FileSystemBrowser { get; private set; }

        public override void InitializeComponent(ICore core)
        {
            this.Output = core.Components.Output;
            this.MetaDataSourceFactory = core.Factories.MetaDataSource;
            this.FileSystemBrowser = core.Components.FileSystemBrowser;
            base.InitializeComponent(core);
        }

        public async Task<PlaylistItem[]> Create(CueSheet cueSheet)
        {
            if (this.ReportProgress)
            {
                this.Name = Strings.CueSheetPlaylistItemFactory_Name;
            }

            var playlistItems = new List<PlaylistItem>();
            var directoryName = Path.GetDirectoryName(cueSheet.FileName);
            var metaDataSource = this.MetaDataSourceFactory.Create();
            for (var a = 0; a < cueSheet.Files.Length; a++)
            {
                var file = cueSheet.Files[a];
                var target = this.Resolve(directoryName, file.Path);
                var metaData = default(IEnumerable<MetaDataItem>);
                try
                {
                    metaData = (
                        await metaDataSource.GetMetaData(target).ConfigureAwait(false)
                    ).ToArray();
                }
                catch (Exception e)
                {
                    metaData = Enumerable.Empty<MetaDataItem>();
                    Logger.Write(this, LogLevel.Debug, "Failed to read meta data from file \"{0}\": {1}", target, e.Message);
                }
                for (var b = 0; b < file.Tracks.Length; b++)
                {
                    var fileName = default(string);
                    if (b + 1 < file.Tracks.Length)
                    {
                        fileName = BassCueStreamAdvisor.CreateUrl(
                            target,
                            file.Tracks[b].Index.Time,
                            CueSheet.GetTrackLength(file.Tracks[b], file.Tracks[b + 1])
                        );
                    }
                    else
                    {
                        fileName = BassCueStreamAdvisor.CreateUrl(
                            target,
                            file.Tracks[b].Index.Time
                        );
                    }
                    if (this.ReportProgress)
                    {
                        this.Description = Path.GetFileName(fileName);
                    }
                    var playlistItem = new PlaylistItem()
                    {
                        DirectoryName = directoryName,
                        FileName = fileName
                    };
                    playlistItem.MetaDatas = this.GetMetaData(cueSheet, file.Tracks[b], metaData);
                    playlistItems.Add(playlistItem);
                }
            }
            return playlistItems.ToArray();
        }

        protected virtual MetaDataItem[] GetMetaData(CueSheet cueSheet, CueSheetTrack cueSheetTrack, IEnumerable<MetaDataItem> fileMetaData)
        {
            var metaDataItems = new Dictionary<string, MetaDataItem>(StringComparer.OrdinalIgnoreCase);
            if (fileMetaData != null)
            {
                foreach (var metaDataItem in fileMetaData)
                {
                    metaDataItems[metaDataItem.Name] = metaDataItem;
                }
            }
            foreach (var tag in cueSheet.Tags)
            {
                var name = default(string);
                if (!FILE_NAMES.TryGetValue(tag.Name, out name))
                {
                    if (!CommonMetaData.Lookup.TryGetValue(tag.Name, out name))
                    {
                        name = tag.Name;
                    }
                }
                metaDataItems[name] = new MetaDataItem(name, MetaDataItemType.Tag)
                {
                    Value = tag.Value
                };
            }
            foreach (var tag in cueSheetTrack.Tags)
            {
                var name = default(string);
                if (!TRACK_NAMES.TryGetValue(tag.Name, out name))
                {
                    if (!CommonMetaData.Lookup.TryGetValue(tag.Name, out name))
                    {
                        name = tag.Name;
                    }
                }
                metaDataItems[name] = new MetaDataItem(name, MetaDataItemType.Tag)
                {
                    Value = tag.Value
                };
            }
            metaDataItems[CommonMetaData.Track] = new MetaDataItem(CommonMetaData.Track, MetaDataItemType.Tag)
            {
                Value = cueSheetTrack.Number
            };
            //TODO: We could create the CommonProperties.Duration for all but the last track in each file. 
            //TODO: Without understanding the file format we can't determine the length of the last track.
            //TODO: Just don't provide any duration for now.
            metaDataItems.Remove(CommonProperties.Duration);
            return metaDataItems.Values.ToArray();
        }

        protected virtual string Resolve(string directoryName, string name)
        {
            return this.DataFileNames.GetOrAdd(
                Path.Combine(directoryName, name),
                () =>
                {
                    var fileName = Path.Combine(directoryName, name);
                    if (File.Exists(fileName))
                    {
                        return fileName;
                    }
                    Logger.Write(this, LogLevel.Warn, "Cue sheet references non existant file \"{0}\", attempting to resolve it...", fileName);
                    return this.Resolve(directoryName, Directory.GetFiles(directoryName), Path.GetFileNameWithoutExtension(name));
                }
            );
        }

        protected virtual string Resolve(string directoryName, string[] fileNames, string name)
        {
            foreach (var fileName in fileNames)
            {
                if (!Path.GetFileNameWithoutExtension(fileName).Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                if (!this.Output.IsSupported(fileName))
                {
                    continue;
                }
                Logger.Write(this, LogLevel.Warn, "Located a suitable file \"{0}\".", fileName);
                return fileName;
            }
            fileNames = fileNames.Where(fileName => this.Output.IsSupported(fileName)).ToArray();
            if (fileNames.Length == 1)
            {
                var fileName = fileNames[0];
                Logger.Write(this, LogLevel.Warn, "Only \"{0}\" is supported for playback.", fileName);
                return fileName;
            }
            if (this.FileSystemBrowser != null)
            {
                return this.Browse(directoryName, name);
            }
            throw new InvalidOperationException(string.Format("Cue sheet references non existant name \"{0}\".", name));
        }

        protected virtual string Browse(string directoryName, string name)
        {
            var options = new BrowseOptions(
                string.Format(Strings.CueSheetPlaylistItemFactory_Prompt, name),
                directoryName,
                Enumerable.Empty<BrowseFilter>(),
                BrowseFlags.File
            );
            var result = this.FileSystemBrowser.Browse(options);
            if (result.Success)
            {
                return result.Paths.FirstOrDefault();
            }
            throw new InvalidOperationException(string.Format("Cue sheet references non existant name \"{0}\", manual resolution was cancelled.", name));
        }
    }
}
