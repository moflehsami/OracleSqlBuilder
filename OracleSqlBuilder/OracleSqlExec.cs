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

		///// <summary>
		///// Executes a non-query and returns a OracleSqlNonData instance.
		///// </summary>
		///// <param name="ConnectionString">The connection string.</param>
		///// <param name="Inserts">The instance/s of OracleSqlBuilderInsert.</param>
		///// <returns>The instance of OracleSqlNonData.</returns>
		//public static OracleSqlNonData NonQuery(string ConnectionString, params OracleSqlBuilderInsert[] Inserts) {
		//	return new OracleSqlExecNonQuery().Execute(ConnectionString, Inserts);
		//}

		///// <summary>
		///// Executes a non-query and returns a OracleSqlNonData instance.
		///// </summary>
		///// <param name="Inserts">The instance/s of OracleSqlBuilderInsert.</param>
		///// <returns>The instance of OracleSqlNonData.</returns>
		//public static OracleSqlNonData NonQuery(params OracleSqlBuilderInsert[] Inserts) {
		//	return new OracleSqlExecNonQuery().Execute(Inserts);
		//}

		///// <summary>
		///// Executes a non-query and returns a OracleSqlNonData instance.
		///// </summary>
		///// <param name="ConnectionString">The connection string.</param>
		///// <param name="Updates">The instance/s of OracleSqlBuilderUpdate.</param>
		///// <returns>The instance of OracleSqlNonData.</returns>
		//public static OracleSqlNonData NonQuery(string ConnectionString, params OracleSqlBuilderUpdate[] Updates) {
		//	return new OracleSqlExecNonQuery().Execute(ConnectionString, Updates);
		//}

		///// <summary>
		///// Executes a non-query and returns a OracleSqlNonData instance.
		///// </summary>
		///// <param name="Updates">The instance/s of OracleSqlBuilderUpdate.</param>
		///// <returns>The instance of OracleSqlNonData.</returns>
		//public static OracleSqlNonData NonQuery(params OracleSqlBuilderUpdate[] Updates) {
		//	return new OracleSqlExecNonQuery().Execute(Updates);
		//}

		///// <summary>
		///// Executes a non-query and returns a OracleSqlNonData instance.
		///// </summary>
		///// <param name="ConnectionString">The connection string.</param>
		///// <param name="Deletes">The instance/s of OracleSqlBuilderDelete.</param>
		///// <returns>The instance of OracleSqlNonData.</returns>
		//public static OracleSqlNonData NonQuery(string ConnectionString, params OracleSqlBuilderDelete[] Deletes) {
		//	return new OracleSqlExecNonQuery().Execute(ConnectionString, Deletes);
		//}

		///// <summary>
		///// Executes a non-query and returns a OracleSqlNonData instance.
		///// </summary>
		///// <param name="Deletes">The instance/s of OracleSqlBuilderDelete.</param>
		///// <returns>The instance of OracleSqlNonData.</returns>
		//public static OracleSqlNonData NonQuery(params OracleSqlBuilderDelete[] Deletes) {
		//	return new OracleSqlExecNonQuery().Execute(Deletes);
		//}

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
