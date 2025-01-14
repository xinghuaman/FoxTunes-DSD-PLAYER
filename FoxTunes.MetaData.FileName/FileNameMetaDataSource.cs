﻿using FoxTunes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FoxTunes
{
    public class FileNameMetaDataSource : BaseComponent, IMetaDataSource
    {
        private static readonly string PREFIX = typeof(FileNameMetaDataSource).Name;

        public static ArtworkType ArtworkTypes = ArtworkType.FrontCover;

        public FileNameMetaDataSource(IEnumerable<IFileNameMetaDataExtractor> extractors)
        {
            this.Extractors = extractors;
        }

        public IEnumerable<IFileNameMetaDataExtractor> Extractors { get; private set; }

        public IConfiguration Configuration { get; private set; }

        public BooleanConfigurationElement LooseImages { get; private set; }

        public BooleanConfigurationElement CopyImages { get; private set; }

        public IArtworkProvider ArtworkProvider { get; private set; }

        public override void InitializeComponent(ICore core)
        {
            this.Configuration = core.Components.Configuration;
            this.LooseImages = this.Configuration.GetElement<BooleanConfigurationElement>(
                MetaDataBehaviourConfiguration.SECTION,
                MetaDataBehaviourConfiguration.READ_LOOSE_IMAGES
            );
            this.CopyImages = this.Configuration.GetElement<BooleanConfigurationElement>(
                MetaDataBehaviourConfiguration.SECTION,
                MetaDataBehaviourConfiguration.COPY_IMAGES_ELEMENT
            );
            this.ArtworkProvider = core.Components.ArtworkProvider;
            base.InitializeComponent(core);
        }

        public IEnumerable<string> GetWarnings(string fileName)
        {
            return Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<MetaDataItem>> GetMetaData(string fileName)
        {
            var result = new List<MetaDataItem>();
            var metaData = default(IDictionary<string, string>);
            foreach (var extractor in this.Extractors)
            {
                if (!extractor.Extract(fileName, out metaData))
                {
                    continue;
                }
                foreach (var key in metaData.Keys)
                {
                    var name = default(string);
                    if (!CommonMetaData.Lookup.TryGetValue(key, out name))
                    {
                        name = key;
                    }
                    result.Add(this.GetMetaData(name, metaData[key]));
                }
                break;
            }
            if (this.LooseImages.Value)
            {
                foreach (var type in new[] { ArtworkType.FrontCover, ArtworkType.BackCover })
                {
                    if (!ArtworkTypes.HasFlag(type))
                    {
                        continue;
                    }
                    var value = this.ArtworkProvider.Find(fileName, type);
                    if (!string.IsNullOrEmpty(value) && File.Exists(value))
                    {
                        if (this.CopyImages.Value)
                        {
                            value = await this.ImportImage(value, value).ConfigureAwait(false);
                        }
                        result.Add(new MetaDataItem()
                        {
                            Name = Enum.GetName(typeof(ArtworkType), type),
                            Value = value,
                            Type = MetaDataItemType.Image
                        });
                    }
                }
            }
            return result;
        }

        public Task<IEnumerable<MetaDataItem>> GetMetaData(IFileAbstraction fileAbstraction)
        {
            throw new NotImplementedException();
        }

        protected virtual MetaDataItem GetMetaData(string name, string value)
        {
            return new MetaDataItem(name, MetaDataItemType.Tag)
            {
                Value = value
            };
        }

        private Task<string> ImportImage(string fileName, string id)
        {
            return FileMetaDataStore.IfNotExistsAsync(PREFIX, id, result => FileMetaDataStore.CopyAsync(PREFIX, id, fileName));
        }

        public Task SetMetaData(string fileName, IEnumerable<MetaDataItem> metaDataItems, Func<MetaDataItem, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
