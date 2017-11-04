﻿using FoxTunes.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace FoxTunes
{
    public class AddLibraryHierarchyNodeToPlaylistTask : PlaylistTaskBase
    {
        public const string ID = "4E0DD392-1138-4DA8-84C2-69B27D1E34EA";

        public AddLibraryHierarchyNodeToPlaylistTask(int sequence, LibraryHierarchyNode libraryHierarchyNode) : base(ID, sequence)
        {
            this.LibraryHierarchyNode = libraryHierarchyNode;
        }

        public LibraryHierarchyNode LibraryHierarchyNode { get; private set; }

        public IPlaybackManager PlaybackManager { get; private set; }

        public override void InitializeComponent(ICore core)
        {
            this.PlaybackManager = core.Managers.Playback;
            base.InitializeComponent(core);
        }

        protected override Task OnRun()
        {
            using (var databaseContext = this.DataManager.CreateWriteContext())
            {
                using (var transaction = databaseContext.Connection.BeginTransaction())
                {
                    this.AddPlaylistItems(databaseContext, transaction);
                    this.ShiftItems(databaseContext, transaction);
                    this.AddOrUpdateMetaDataFromLibrary(databaseContext, transaction);
                    this.SequenceItems(databaseContext, transaction);
                    this.SetPlaylistItemsStatus(databaseContext, transaction);
                    transaction.Commit();
                }
            }
            this.SignalEmitter.Send(new Signal(this, CommonSignals.PlaylistUpdated));
            return Task.CompletedTask;
        }

        private void AddPlaylistItems(IDatabaseContext databaseContext, IDbTransaction transaction)
        {
            var parameters = default(IDbParameterCollection);
            using (var command = databaseContext.Connection.CreateCommand(this.Database.CoreSQL.AddLibraryHierarchyNodeToPlaylist, new[] { "libraryHierarchyItemId", "sequence", "status" }, out parameters))
            {
                command.Transaction = transaction;
                parameters["libraryHierarchyItemId"] = this.LibraryHierarchyNode.Id;
                parameters["sequence"] = this.Sequence;
                parameters["status"] = PlaylistItemStatus.Import;
                this.Offset = command.ExecuteNonQuery();
            }
        }
    }
}