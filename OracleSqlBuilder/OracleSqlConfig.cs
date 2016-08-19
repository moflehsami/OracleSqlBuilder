namespace OracleSqlBuilder {
    /// <summary>
    /// OracleSql Config class.
    /// </summary>
    public static class OracleSqlConfig {
        #region Public Property
        /// <summary>
        /// Debug property. If true, printing out the statements and parameters are enabled.
        /// </summary>
        public static bool Debug { get; set; }

        /// <summary>
        /// Throw Exceptions property. If true, errors will be thrown out.
        /// </summary>
        public static bool ThrowExceptions { get; set; }

        /// <summary>
        /// Default Connection property. The name of the default connection.
        /// </summary>
        public static string DefaultConnection { get; set; }
        #endregion

        #region Static Constructor Method
        /// <summary>
        /// Static constructor.
        /// </summary>
        static OracleSqlConfig() {
            OracleSqlConfig._InitStaticProperties();
        }
        #endregion

        #region Private Static Method
        /// <summary>
        /// Initalizes the static properties.
        /// </summary>
        private static void _InitStaticProperties() {
            OracleSqlConfig.Debug = false;
            OracleSqlConfig.DefaultConnection = "default";
        }
        #endregion
    }
}
