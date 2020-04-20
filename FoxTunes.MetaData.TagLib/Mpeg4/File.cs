﻿using FoxTunes.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TagLib.Mpeg4;

namespace FoxTunes.Mpeg4
{
    public class File : global::TagLib.Mpeg4.File, IMetaDataSource
    {
        protected static ILogger Logger
        {
            get
            {
                return LogManager.Logger;
            }
        }

        public File(string path) : base(path)
        {

        }

        public IEnumerable<string> GetWarnings(string fileName)
        {
            return Enumerable.Empty<string>();
        }

        public Task<IEnumerable<MetaDataItem>> GetMetaData(string fileName)
        {
            var box = this.GetXtraBox();
            var metaDatas = new List<MetaDataItem>();
            if (box != null && box.Data != null)
            {
                var parser = new XtraBoxParser(box.Data.Data);
                var metaDataItems = parser.Tags.Where(
                    tag => XtraTag.CanImport(tag)
                ).Select(
                    tag => tag.ToMetaDataItem()
                ).ToArray();
                metaDatas.AddRange(metaDataItems);
            }
#if NET40
            return TaskEx.FromResult<IEnumerable<MetaDataItem>>(metaDatas);
#else
            return Task.FromResult<IEnumerable<MetaDataItem>>(metaDatas);
#endif
        }

        public Task SetMetaData(string fileName, IEnumerable<MetaDataItem> metaDataItems, Func<MetaDataItem, bool> predicate)
        {
            var box = this.GetXtraBox();
            var tags = metaDataItems.Where(
                metaDataItem => XtraTag.CanExport(metaDataItem)
            ).Select(
                metaDataItem => XtraTag.FromMetaDataItem(metaDataItem)
            ).ToArray();
            if (box == null)
            {
                this.AddTags(tags);
            }
            else if (tags.Any())
            {
                this.UpdateTags(box, tags);
            }
            else
            {
                this.RemoveXtraBox();
            }
#if NET40
            return TaskEx.FromResult(false);
#else
            return Task.CompletedTask;
#endif
        }

        protected virtual void AddTags(IEnumerable<XtraTag> tags)
        {
            var box = this.CreateXtraBox();
            var formatter = new XtraBoxFormatter(tags);
            box.Data = formatter.Data;
        }

        protected virtual void UpdateTags(Box box, IEnumerable<XtraTag> tags)
        {
            var parser = new XtraBoxParser(box.Data.Data);
            var currentTags = parser.Tags;
            this.UpdateTags(box, currentTags, tags);
        }

        protected virtual void UpdateTags(Box box, IEnumerable<XtraTag> currentTags, IEnumerable<XtraTag> updatedTags)
        {
            var tags = new Dictionary<string, XtraTag>(StringComparer.OrdinalIgnoreCase);
            foreach (var tag in currentTags.Concat(updatedTags))
            {
                tags[tag.Name] = tag;
            }
            var formatter = new XtraBoxFormatter(tags.Values);
            box.Data = formatter.Data;
        }

        protected virtual Box GetXtraBox()
        {
            foreach (var box in this.UdtaBoxes.OfType<IsoUserDataBox>())
            {
                foreach (var child in box.Children)
                {
                    if (child.BoxType == XtraBox.Xtra)
                    {
                        return child;
                    }
                }
            }
            return null;
        }

        protected virtual Box CreateXtraBox()
        {
            foreach (var box in this.UdtaBoxes.OfType<IsoUserDataBox>())
            {
                var child = new XtraBox();
                box.AddChild(child);
                return child;
            }
            return null;
        }

        protected virtual void RemoveXtraBox()
        {
            foreach (var box in this.UdtaBoxes.OfType<IsoUserDataBox>())
            {
                box.RemoveChild(XtraBox.Xtra);
            }
        }

        #region IBaseComponent

        public void InitializeComponent(ICore core)
        {
            //Nothing to do.
        }

        protected virtual void OnPropertyChanging(string propertyName)
        {
            if (this.PropertyChanging == null)
            {
                return;
            }
            this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
        }

        [field: NonSerialized]
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual Task OnError(string message)
        {
            return this.OnError(new Exception(message));
        }

        protected virtual Task OnError(Exception exception)
        {
            return this.OnError(exception.Message, exception);
        }

        protected virtual Task OnError(string message, Exception exception)
        {
            return this.OnError(this, new ComponentErrorEventArgs(message, exception));
        }

        protected virtual Task OnError(object sender, ComponentErrorEventArgs e)
        {
            Logger.Write(this, LogLevel.Error, e.Message);
            if (this.Error == null)
            {
#if NET40
                return TaskEx.FromResult(false);
#else
                return Task.CompletedTask;
#endif
            }
            return this.Error(sender, e);
        }

        [field: NonSerialized]
        public event ComponentErrorEventHandler Error;

        #endregion
    }
}
