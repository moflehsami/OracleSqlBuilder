using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OracleSqlBuilder {
    /// <summary>
    /// OracleSql Connection String class.
    /// </summary>
    public class OracleSqlConnectionString {
        #region Private Static Property
        private static Dictionary<string, string> _Connections { get; set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// Maximum life time (in seconds) of the connection.
        /// </summary>
        public uint ConnectionLifetime { get; set; }

        /// <summary>
        /// Maximum time (in seconds) to wait for a free connection from the pool.
        /// </summary>
        public uint ConnectionTimeout { get; set; }

        /// <summary>
        /// Returns an implicit database connection if set to true.
        /// </summary>
        public bool ContextConnection { get; set; }

        /// <summary>
        /// Oracle Net Services Name, Connect Descriptor, or an easy connect naming that identifies the database
        /// to which to connect.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// Administrative privileges: SYSDBA or SYSOPER.
        /// </summary>
        public DBAPrivileges DBAPrivilege { get; set; }

        /// <summary>
        /// Number of connections that are closed when an excessive amount of established connections are unused.
        /// </summary>
        public uint DecrPoolSize { get; set; }

        /// <summary>
        /// Controls the enlistment behavior and capabilities of a connection in context of COM+ transactions or
        /// System.Transactions.
        /// </summary>
        public bool Enlist { get; set; }

        /// <summary>
        /// Enables ODP.NET connection pool to proactively remove connections from the pool when an Oracle RAC
        /// service, service member, or node goes down. Works with RAC, Data Guard, or a single database instance.
        /// </summary>
        public bool HAEvents { get; set; }

        /// <summary>
        /// Enables ODP.NET connection pool to balance work requests across Oracle RAC instances based on the load
        /// balancing advisory and service goal.
        /// </summary>
        public bool LoadBalancing { get; set; }

        /// <summary>
        /// Number of new connections to be created when all connections in the pool are in use.
        /// </summary>
        public uint IncrPoolSize { get; set; }

        /// <summary>
        /// Maximum number of connections in a pool.
        /// </summary>
        public uint MaxPoolSize { get; set; }

        /// <summary>
        /// Caches metadata information.
        /// </summary>
        public bool MetadataPooling { get; set; }

        /// <summary>
        /// Minimum number of connections in a pool.
        /// </summary>
        public uint MinPoolSize { get; set; }

        /// <summary>
        /// Password for the user specified by User Id.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Retrieval of the password in the connection string.
        /// </summary>
        public bool PersistSecurityInfo { get; set; }

        /// <summary>
        /// Connection pooling.
        /// </summary>
        public bool Pooling { get; set; }

        /// <summary>
        /// Indicates whether or not a transaction is local or distributed throughout its lifetime.
        /// </summary>
        public PromotableTransactions PromotableTransaction { get; set; }

        /// <summary>
        /// User name of the proxy user.
        /// </summary>
        public string ProxyUserId { get; set; }

        /// <summary>
        /// Password of the proxy user.
        /// </summary>
        public string ProxyPassword { get; set; }

        /// <summary>
        /// Statement cache purged when the connection goes back to the pool.
        /// </summary>
        public bool StatementCachePurge { get; set; }

        /// <summary>
        /// Statement cache enabled and cache size, that is, the maximum number of statements that can be cached.
        /// </summary>
        public uint StatementCacheSize { get; set; }

        /// <summary>
        /// Oracle user name.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Validation of connections coming from the pool.
        /// </summary>
        public bool ValidateConnection { get; set; }
        #endregion

        #region Constructor Method
        /// <summary>
        /// Constructor.
        /// </summary>
        public OracleSqlConnectionString() {
            this._InitProperties();
        }
        #endregion

        #region Static Constructor Method
        /// <summary>
        /// Static constructor.
        /// </summary>
        static OracleSqlConnectionString() {
            OracleSqlConnectionString._InitStaticProperties();
        }
        #endregion

        #region Public Override Method
        /// <summary>
        /// Generates the connection string.
        /// </summary>
        /// <returns>The connection string.</returns>
        public override string ToString() {
            List<string> lstConnections = new List<string>();
            if (this.ConnectionLifetime != 0) {
                lstConnections.Add(String.Format("Connection Lifetime={0}", this.ConnectionLifetime));
            }
            if (this.ConnectionTimeout != 15) {
                lstConnections.Add(String.Format("Connection Timeout={0}", this.ConnectionTimeout));
            }
            if (this.ContextConnection) {
                lstConnections.Add("Connection Timeout=true");
            }
            if (!String.IsNullOrWhiteSpace(this.DataSource)) {
                lstConnections.Add(String.Format("Data Source={0}", this.DataSource));
            }
            if (this.DBAPrivilege != DBAPrivileges.None) {
                switch (this.DBAPrivilege) {
                    case DBAPrivileges.SYSDBA:
                        lstConnections.Add("DBA Privilege=SYSDBA");
                        break;
                    case DBAPrivileges.SYSOPER:
                        lstConnections.Add("DBA Privilege=SYSOPER");
                        break;
                }
            }
            if (this.DecrPoolSize > 1) {
                lstConnections.Add(String.Format("Decr Pool Size={0}", this.DecrPoolSize));
            }
            if (!this.Enlist) {
                lstConnections.Add("Enlist=false");
            }
            if (this.HAEvents) {
                lstConnections.Add("HA Events=true");
            }
            if (this.LoadBalancing) {
                lstConnections.Add("Load Balancing=true");
            }
            if (this.IncrPoolSize > 5) {
                lstConnections.Add(String.Format("Incr Pool Size={0}", this.IncrPoolSize));
            }
            if (this.MaxPoolSize > this.IncrPoolSize && this.MaxPoolSize != 100) {
                lstConnections.Add(String.Format("Max Pool Size={0}", this.MaxPoolSize));
            }
            if (!this.MetadataPooling) {
                lstConnections.Add("Metadata Pooling=false");
            }
            if (this.MinPoolSize > 1) {
                lstConnections.Add(String.Format("Min Pool Size={0}", this.MinPoolSize));
            }
            if (!String.IsNullOrWhiteSpace(this.Password)) {
                lstConnections.Add(String.Format("Password={0}", this.Password));
            }
            if (this.PersistSecurityInfo) {
                lstConnections.Add("Persist Security Info=true");
            }
            if (!this.Pooling) {
                lstConnections.Add("Pooling=false");
            }
            if (this.PromotableTransaction != PromotableTransactions.Promotable) {
                switch (this.PromotableTransaction) {
                    case PromotableTransactions.Local:
                        lstConnections.Add("Promotable Transaction=local");
                        break;
                }
            }
            if (!String.IsNullOrWhiteSpace(this.ProxyUserId)) {
                lstConnections.Add(String.Format("Proxy User Id={0}", this.ProxyUserId));
            }
            if (!String.IsNullOrWhiteSpace(this.ProxyPassword)) {
                lstConnections.Add(String.Format("Proxy Password={0}", this.ProxyPassword));
            }
            if (this.StatementCachePurge) {
                lstConnections.Add("Statement Cache Purge=true");
            }
            if (this.StatementCacheSize > 10) {
                lstConnections.Add(String.Format("Statement Cache Size={0}", this.StatementCacheSize));
            }
            if (!String.IsNullOrWhiteSpace(this.UserId)) {
                lstConnections.Add(String.Format("User Id={0}", this.UserId));
            }
            if (this.ValidateConnection) {
                lstConnections.Add("Validate Connection=true");
            }
            return String.Join(";", lstConnections.ToArray());
        }
        #endregion

        #region Public Static Method
        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="Name">The name of the connection.</param>
        /// <param name="Connection">The OracleSqlConnectionString instance.</param>
        public static void Set(string Name, OracleSqlConnectionString Connection) {
            if (String.IsNullOrWhiteSpace(Name)) {
                throw new ArgumentException("Name argument should not be empty.");
            }
            if (Regex.IsMatch(Name, @"\W")) {
                throw new ArgumentException("Name argument should only contain any word character (letter, number, underscore).");
            }
            OracleSqlConnectionString._SetConnection(Name, Connection.ToString());
        }

        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="Connection">The OracleSqlConnectionString instance.</param>
        public static void Set(OracleSqlConnectionString Connection) {
            OracleSqlConnectionString.Set(OracleSqlConfig.DefaultConnection, Connection);
        }

        /// <summary>
        /// Reads the connection by name.
        /// </summary>
        /// <param name="Name">The name of the connection.</param>
        /// <returns>The connection.</returns>
        public static string Read(string Name) {
            if (String.IsNullOrWhiteSpace(Name)) {
                throw new ArgumentException("Name argument should not be empty.");
            }
            if (Regex.IsMatch(Name, @"\W")) {
                throw new ArgumentException("Name argument should only contain any word character (letter, number, underscore).");
            }
            if (!OracleSqlConnectionString._Connections.ContainsKey(Name)) {
                throw new KeyNotFoundException(String.Format("The Name '{0}' does not exist.", Name));
            }
            return OracleSqlConnectionString._Connections[Name];
        }

        /// <summary>
        /// Reads the connection by name.
        /// </summary>
        /// <returns>The connection.</returns>
        public static string Read() {
            return OracleSqlConnectionString.Read(OracleSqlConfig.DefaultConnection);
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void _InitProperties() {
            this.ConnectionLifetime = 0;
            this.ConnectionTimeout = 15;
            this.ContextConnection = false;
            this.DataSource = null;
            this.DBAPrivilege = DBAPrivileges.None;
            this.DecrPoolSize = 1;
            this.Enlist = true;
            this.HAEvents = false;
            this.LoadBalancing = false;
            this.IncrPoolSize = 5;
            this.MaxPoolSize = 100;
            this.MetadataPooling = true;
            this.MinPoolSize = 1;
            this.Password = null;
            this.PersistSecurityInfo = false;
            this.Pooling = true;
            this.PromotableTransaction = PromotableTransactions.Local;
            this.ProxyUserId = null;
            this.ProxyPassword = null;
            this.StatementCachePurge = false;
            this.StatementCacheSize = 10;
            this.UserId = null;
            this.ValidateConnection = false;
        }

        /// <summary>
        /// Sets the connection.
        /// </summary>
        /// <param name="Name">The name of the connection.</param>
        /// <param name="Connection">The connection string.</param>
        private static void _SetConnection(string Name, string Connection) {
            if (String.IsNullOrWhiteSpace(Name)) {
                throw new ArgumentException("Name argument should not be empty.");
            }
            if (Regex.IsMatch(Name, @"\W")) {
                throw new ArgumentException("Name argument should only contain any word character (letter, number, underscore).");
            }
            if (String.IsNullOrWhiteSpace(Connection)) {
                throw new ArgumentException("Connection argument should not be empty.");
            }
            if (OracleSqlConnectionString._Connections.ContainsKey(Name)) {
                OracleSqlConnectionString._Connections[Name] = Connection;
            } else {
                OracleSqlConnectionString._Connections.Add(Name, Connection);
            }
        }
        #endregion

        #region Private Static Method
        /// <summary>
        /// Initializes the static properties.
        /// </summary>
        private static void _InitStaticProperties() {
            OracleSqlConnectionString._Connections = new Dictionary<string, string>();
        }
        #endregion
    }
}
