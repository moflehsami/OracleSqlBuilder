using System;

namespace OracleSqlBuilder {
	/// <summary>
	/// OracleSql Non Data class.
	/// </summary>
	public class OracleSqlNonData {
		#region Public Properties
		/// <summary>
		/// The number of affected rows.
		/// </summary>
		public double AffectedRows {
			get;
			internal set;
		}

		/// <summary>
		/// The duration of the query.
		/// </summary>
		public TimeSpan Duration {
			get;
			internal set;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor method.
		/// </summary>
		public OracleSqlNonData() {
			this._InitProperties();
		}
		#endregion

		#region Private Method
		/// <summary>
		/// Initializes the properties.
		/// </summary>
		private void _InitProperties() {
			this.AffectedRows = 0;
		}
		#endregion
	}
}
