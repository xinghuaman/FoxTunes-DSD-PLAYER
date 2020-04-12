﻿using FoxTunes.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FoxTunes
{
    public class BassReplayGainScannerMonitor : PopulatorBase, IBassReplayGainScannerMonitor
    {
        public static readonly TimeSpan INTERVAL = TimeSpan.FromSeconds(1);

        public BassReplayGainScannerMonitor(IBassReplayGainScanner scanner, bool reportProgress, CancellationToken cancellationToken) : base(reportProgress)
        {
            this.ScannerItems = new Dictionary<Guid, ScannerItem>();
            this.Scanner = scanner;
            this.CancellationToken = cancellationToken;
        }

        public Dictionary<Guid, ScannerItem> ScannerItems { get; private set; }

        public IBassReplayGainScanner Scanner { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public Task Scan()
        {
#if NET40
            var task = TaskEx.Run(() =>
#else
            var task = Task.Run(() =>
#endif
            {
                this.Scanner.Scan();
            });
#if NET40
            return TaskEx.WhenAll(task, this.Monitor(task));
#else
            return Task.WhenAll(task, this.Monitor(task));
#endif
        }

        protected virtual async Task Monitor(Task task)
        {
            await this.SetName("Converting files").ConfigureAwait(false);
            while (!task.IsCompleted)
            {
                if (this.CancellationToken.IsCancellationRequested)
                {
                    Logger.Write(this, LogLevel.Debug, "Requesting cancellation from scanner.");
                    this.Scanner.Cancel();
                    await this.SetName("Cancelling").ConfigureAwait(false);
                    break;
                }
                this.Scanner.Update();
                var position = 0;
                var count = 0;
                var builder = new StringBuilder();
                foreach (var scannerItem in this.Scanner.ScannerItems)
                {
                    this.UpdateStatus(scannerItem);
                    position += scannerItem.Progress;
                    count += ScannerItem.PROGRESS_COMPLETE;
                    if (scannerItem.Status == ScannerItemStatus.Processing)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(Path.GetFileName(scannerItem.FileName));
                    }
                }
                if (builder.Length > 0)
                {
                    await this.SetDescription(builder.ToString()).ConfigureAwait(false);
                }
                else
                {
                    await this.SetDescription("Waiting for scanner").ConfigureAwait(false);
                }
                await this.SetPosition(position).ConfigureAwait(false);
                await this.SetCount(count).ConfigureAwait(false);
#if NET40
                await TaskEx.Delay(INTERVAL).ConfigureAwait(false);
#else
                await Task.Delay(INTERVAL).ConfigureAwait(false);
#endif
            }
            while (!task.IsCompleted)
            {
                Logger.Write(this, LogLevel.Debug, "Waiting for scanner to complete.");
                this.Scanner.Update();
#if NET40
                await TaskEx.Delay(INTERVAL).ConfigureAwait(false);
#else
                await Task.Delay(INTERVAL).ConfigureAwait(false);
#endif
            }
        }

        protected virtual void UpdateStatus(ScannerItem scannerItem)
        {
            var currentScannerItem = default(ScannerItem);
            if (this.ScannerItems.TryGetValue(scannerItem.Id, out currentScannerItem) && currentScannerItem.Status != scannerItem.Status)
            {
                Logger.Write(this, LogLevel.Debug, "Scanner status changed for file \"{0}\": {1}", scannerItem.FileName, Enum.GetName(typeof(ScannerItemStatus), scannerItem.Status));
                this.OnStatusChanged(scannerItem);
            }
            this.ScannerItems[scannerItem.Id] = scannerItem;
        }

        protected virtual void OnStatusChanged(ScannerItem scannerItem)
        {
            if (this.StatusChanged == null)
            {
                return;
            }
            this.StatusChanged(this, new BassScannerMonitorEventArgs(scannerItem));
        }

        public event BassScannerMonitorEventHandler StatusChanged;
    }

    public delegate void BassScannerMonitorEventHandler(object sender, BassScannerMonitorEventArgs e);

    public class BassScannerMonitorEventArgs : EventArgs
    {
        public BassScannerMonitorEventArgs(ScannerItem scannerItem)
        {
            this.ScannerItem = scannerItem;
        }

        public ScannerItem ScannerItem { get; private set; }
    }
}
