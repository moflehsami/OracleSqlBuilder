namespace OracleSqlBuilder {
    /// <summary>
    /// OracleSql Exec class.
    /// </summary>
    public static class OracleSqlExec {
        /// <summary>
        /// Executes a query and returns a OracleSqlData instance.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        /// <param name="Select">The instance of OracleSqlBuilderSelect.</param>
        /// <returns>The instance of OracleSqlData.</returns>
        public static OracleSqlData Query(string ConnectionString, OracleSqlBuilderSelect Select) {
            return new OracleSqlExecQuery().Execute(ConnectionString, Select);
        }

        /// <summary>
        /// Executes a query and returns a OracleSqlData instance.
        /// </summary>
        /// <param name="Select">The instance of OracleSqlBuilderSelect.</param>
        /// <returns>The instance of OracleSqlData.</returns>
        public static OracleSqlData Query(OracleSqlBuilderSelect Select) {
            return new OracleSqlExecQuery().Execute(Select);
        }

        /// <summary>
        /// Executes a query and returns a OracleSqlData instance.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        /// <param name="SelectCount">The instance of OracleSqlBuilderSelectCount.</param>
        /// <returns>The instance of OracleSqlData.</returns>
        public static OracleSqlData Query(string ConnectionString, OracleSqlBuilderSelectCount SelectCount) {
            return new OracleSqlExecQuery().Execute(ConnectionString, SelectCount);
        }

        /// <summary>
        /// Executes a query and returns a OracleSqlData instance.
        /// </summary>
        /// <param name="SelectCount">The instance of OracleSqlBuilderSelectCount.</param>
        /// <returns>The instance of OracleSqlData.</returns>
        public static OracleSqlData Query(OracleSqlBuilderSelectCount SelectCount) {
            return new OracleSqlExecQuery().Execute(SelectCount);
        }

        /// <summary>
        /// Executes a non-query and returns a OracleSqlNonData instance.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        /// <param name="Queries">The instance/s of OracleSqlBuilderNonQuery.</param>
        /// <returns>The instance of OracleSqlNonData.</returns>
        public static OracleSqlNonData NonQuery(string ConnectionString, params OracleSqlBuilderNonQuery[] Queries) {
            return new OracleSqlExecNonQuery().Execute(ConnectionString, Queries);
        }

        /// <summary>
        /// Executes a non-query and returns a OracleSqlNonData instance.
        /// </summary>
        /// <param name="Queries">The instance/s of OracleSqlBuilderNonQuery.</param>
        /// <returns>The instance of OracleSqlNonData.</returns>
        public static OracleSqlNonData NonQuery(params OracleSqlBuilderNonQuery[] Queries) {
            return new OracleSqlExecNonQuery().Execute(Queries);
        }
    }
}
