namespace OracleSqlBuilder {
	/// <summary>
	/// OracleSql Build class.
	/// </summary>
	public static class OracleSqlBuild {
		/// <summary>
		/// OracleSql Select builder.
		/// </summary>
		/// <param name="Database">The database.</param>
		/// <param name="Table">The table.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <returns>The instance of OracleSqlBuilderSelect.</returns>
		public static OracleSqlBuilderSelect Select(string Database, string Table, string TableAlias) {
			return new OracleSqlBuilderSelect(Database, Table, TableAlias);
		}

		/// <summary>
		/// OracleSql Select builder.
		/// </summary>
		/// <param name="Database">The database.</param>
		/// <param name="Table">The table.</param>
		/// <returns>The instance of OracleSqlBuilderSelect.</returns>
		public static OracleSqlBuilderSelect Select(string Database, string Table) {
			return new OracleSqlBuilderSelect(Database, Table);
		}

		/// <summary>
		/// OracleSql Select builder.
		/// </summary>
		/// <param name="Select">The instance of OracleSqlBuilderSelect</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <returns>The instance of OracleSqlBuilderSelect.</returns>
		public static OracleSqlBuilderSelect Select(OracleSqlBuilderSelect Select, string TableAlias) {
			return new OracleSqlBuilderSelect(Select, TableAlias);
		}

		/// <summary>
		/// OracleSql Select Count builder.
		/// </summary>
		/// <param name="Database">The database.</param>
		/// <param name="Table">The table.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <returns>The instance of OracleSqlBuilderSelectCount.</returns>
		public static OracleSqlBuilderSelectCount SelectCount(string Database, string Table, string TableAlias) {
			return new OracleSqlBuilderSelectCount(Database, Table, TableAlias);
		}

		/// <summary>
		/// OracleSql Select builder.
		/// </summary>
		/// <param name="Database">The database.</param>
		/// <param name="Table">The table.</param>
		/// <returns>The instance of OracleSqlBuilderSelectCount.</returns>
		public static OracleSqlBuilderSelectCount SelectCount(string Database, string Table) {
			return new OracleSqlBuilderSelectCount(Database, Table);
		}

		/// <summary>
		/// OracleSql Insert builder.
		/// </summary>
		/// <param name="Database">The database.</param>
		/// <param name="Table">The table.</param>
		/// <returns>The instance of OracleSqlBuilderInsert.</returns>
		public static OracleSqlBuilderInsert Insert(string Database, string Table) {
			return new OracleSqlBuilderInsert(Database, Table);
		}

		/// <summary>
		/// OracleSql Update builder.
		/// </summary>
		/// <param name="Database">The database.</param>
		/// <param name="Table">The table.</param>
		/// <returns>The instance of OracleSqlBuilderUpdate.</returns>
		public static OracleSqlBuilderUpdate Update(string Database, string Table) {
			return new OracleSqlBuilderUpdate(Database, Table);
		}

		/// <summary>
		/// OracleSql Delete builder.
		/// </summary>
		/// <param name="Database">The database.</param>
		/// <param name="Table">The table.</param>
		/// <returns>The instance of OracleSqlBuilderDelete.</returns>
		public static OracleSqlBuilderDelete Delete(string Database, string Table) {
			return new OracleSqlBuilderDelete(Database, Table);
		}
	}
}
