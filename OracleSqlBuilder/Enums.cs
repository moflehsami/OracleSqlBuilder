namespace OracleSqlBuilder {
	/// <summary>
	/// DBA Privileges enum.
	/// </summary>
	public enum DBAPrivileges {
		/// <summary>
		/// No DBA privilege.
		/// </summary>
		None,
		/// <summary>
		/// SYSDBA privilege.
		/// </summary>
		SYSDBA,
		/// <summary>
		/// SYSOPER privilege.
		/// </summary>
		SYSOPER
	}

	/// <summary>
	/// Promotable Transactions enum.
	/// </summary>
	public enum PromotableTransactions {
		/// <summary>
		/// Promotable transaction.
		/// </summary>
		Promotable,
		/// <summary>
		/// Local transaction.
		/// </summary>
		Local
	}

	/// <summary>
	/// Order Directions enum.
	/// </summary>
	public enum OrderDirections {
		/// <summary>
		/// Asc value.
		/// </summary>
		Asc,
		/// <summary>
		/// Desc value.
		/// </summary>
		Desc
	}
}
