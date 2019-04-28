﻿using FoxTunes.Interfaces;
using System.Diagnostics;
using System.IO;

namespace FoxTunes
{
    public class ProcessReader
    {
        protected static ILogger Logger
        {
            get
            {
                return LogManager.Logger;
            }
        }

        const int BUFFER_SIZE = 10240;

        public ProcessReader(Process process)
        {
            this.Process = process;
        }

        public Process Process { get; }

        public void CopyTo(ProcessWriter writer)
        {
            Logger.Write(this.GetType(), LogLevel.Debug, "Begin reading data from process {0} with {1} byte buffer.", this.Process.Id, BUFFER_SIZE);
            var length = default(int);
            var buffer = new byte[BUFFER_SIZE];
            while (!this.Process.HasExited)
            {
                while ((length = this.Process.StandardOutput.BaseStream.Read(buffer, 0, BUFFER_SIZE)) > 0)
                {
                    writer.Write(buffer, length);
                }
            }
            Logger.Write(this.GetType(), LogLevel.Debug, "Finished reading data from process {0}, closing process input.", this.Process.Id);
            writer.Close();
        }

        public void CopyTo(Stream stream)
        {
            Logger.Write(this.GetType(), LogLevel.Debug, "Begin reading data from process {0}.", this.Process.Id);
            this.Process.StandardOutput.BaseStream.CopyTo(stream);
            Logger.Write(this.GetType(), LogLevel.Debug, "Finished reading data from channel {0}.", this.Process.Id);
        }
    }
}
