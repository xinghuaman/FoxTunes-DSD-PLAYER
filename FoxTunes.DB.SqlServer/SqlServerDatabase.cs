﻿using FoxDb;
using FoxDb.Interfaces;
using FoxTunes.Interfaces;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;

namespace FoxTunes
{
    public class SqlServerDatabase : Database
    {
        const string CONNECTION_STRING = "FoxTunes";

        public static readonly string FileName = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        public SqlServerDatabase()
            : base(GetProvider())
        {

        }

        private static Lazy<string> _ConnectionString = new Lazy<string>(() =>
        {
            var connectionString = ConfigurationManager.ConnectionStrings[CONNECTION_STRING];
            if (connectionString != null)
            {
                return connectionString.ConnectionString;
            }
            CreateConnectionString();
            var userInterface = ComponentRegistry.Instance.GetComponent<IUserInterface>();
            if (userInterface != null)
            {
                userInterface.Warn(string.Format("No connection string found.\nEdit {0} to resolve the create one.", FileName.GetName()));
            }
            throw new InvalidOperationException("No connection string specified.");
        });

        private static void CreateConnectionString()
        {
            if (!File.Exists(FileName))
            {
                Logger.Write(typeof(SqlServerDatabase), LogLevel.Warn, "Config file \"{0}\" does not exist, cannot add connection string.", FileName.GetName());
                return;
            }
            var document = XDocument.Load(FileName);
            var connectionStrings = document.Root.Element("connectionStrings");
            if (connectionStrings == null)
            {
                connectionStrings = new XElement("connectionStrings");
                document.Root.AddFirst(connectionStrings);
            }
            if (connectionStrings.ToString().Contains(CONNECTION_STRING))
            {
                return;
            }
            connectionStrings.Add(new XComment("Uncomment the next element to use the default connection string."));
            connectionStrings.Add(new XComment(new XElement("add", new XAttribute("name", CONNECTION_STRING), new XAttribute("connectionString", "Data Source=localhost;Integrated Security=true;Initial Catalog=FoxTunes")).ToString()));
            document.Save(FileName);
        }

        public static string ConnectionString
        {
            get
            {
                return _ConnectionString.Value;
            }
        }

        public override IsolationLevel PreferredIsolationLevel
        {
            get
            {
                return IsolationLevel.ReadUncommitted;
            }
        }

        protected override IDatabaseQueries CreateQueries()
        {
            var queries = new SqlServerDatabaseQueries(this);
            queries.InitializeComponent(this.Core);
            return queries;
        }

        private static IProvider GetProvider()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);
            return new SqlServer2012Provider(builder);
        }
    }
}
