﻿using FoxTunes.Interfaces;
using System.Threading.Tasks;

namespace FoxTunes
{
    public abstract class LibraryTaskBase : BackgroundTask
    {
        protected LibraryTaskBase(string id)
            : base(id)
        {
        }

        public IDataManager DataManager { get; private set; }

        public ISignalEmitter SignalEmitter { get; private set; }

        public override void InitializeComponent(ICore core)
        {
            this.DataManager = core.Managers.Data;
            this.SignalEmitter = core.Components.SignalEmitter;
            base.InitializeComponent(core);
        }
    }
}