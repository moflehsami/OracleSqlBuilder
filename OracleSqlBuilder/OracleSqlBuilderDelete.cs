using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OracleSqlBuilder {
	/// <summary>
	/// OracleSql Builder Delete class.
	/// </summary>
	public class OracleSqlBuilderDelete : OracleSqlBuilderNonQuery {
		#region Private Property
		private List<string> _Wheres { get; set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes the database and table for the insert statement.
		/// </summary>
		/// <param name="Database">The database of the query.</param>
		/// <param name="Table">The table of the query.</param>
		public OracleSqlBuilderDelete(string Database, string Table) {
			if (String.IsNullOrWhiteSpace(Database)) {
				throw new ArgumentException("Database argument should not be empty.");
			}
			if (!this._IsValidField(Database)) {
				throw new ArgumentException(String.Format("Database argument '{0}' should only contain any word character (letter, number, underscore).", Database));
			}
			this._Database = Database;
			if (String.IsNullOrWhiteSpace(Table)) {
				throw new ArgumentException("Table argument should not be empty.");
			}
			if (!this._IsValidField(Table)) {
				throw new ArgumentException(String.Format("Table argument '{0}' should only contain any word character (letter, number, underscore).", Table));
			}
			this._Table = Table;
			this._InitProperties();
		}
		#endregion

		#region Public Method
		/// <summary>
		/// Sets a parameter to be used in the query.
		/// </summary>
		/// <param name="Name">The name of the parameter.</param>
		/// <param name="Value">The value of the parameter.</param>
		public OracleSqlBuilderDelete SetParameter(string Name, string Value) {
			this._SetParameter(Name, Value);
			return this;
		}

		/// <summary>
		/// Adds a condition to the WHERE clause.
		/// </summary>
		/// <param name="Condition">The condition to check before adding to the WHERE clause.</param>
		/// <param name="ConditionStatement">The condition statement/s to be added.</param>
		/// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderDelete SetWhere(bool Condition, string ConditionStatement, params object[] ParameterValues) {
			if (Condition) {
				ConditionStatement = this._RemoveBackTick(ConditionStatement);
				if (String.IsNullOrWhiteSpace(ConditionStatement)) {
					throw new ArgumentException("ConditionStatement argument should not be empty.");
				}
				List<string> lstParameters = new List<string>();
				if (ParameterValues != null && ParameterValues.Length > 0) {
					foreach (object objParameterValue in ParameterValues) {
						string strParameter = String.Format(":where_condition_{0}", this._Parameters.Count(kv => kv.Key.Contains(":where_condition")) + 1);
						this._SetParameter(strParameter, objParameterValue);
						lstParameters.Add(strParameter);
					}
				}
				this._Wheres.Add(this._Name(String.Format(ConditionStatement, lstParameters.ToArray())));
			}
			return this;
		}

		/// <summary>
		/// Adds a condition to the WHERE clause.
		/// </summary>
		/// <param name="ConditionStatement">The condition statement/s to be added.</param>
		/// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderDelete SetWhere(string ConditionStatement, params object[] ParameterValues) {
			return this.SetWhere(true, ConditionStatement, ParameterValues);
		}
		#endregion

		#region Public Override Method
		/// <summary>
		/// Builds the query.
		/// </summary>
		/// <returns>The query.</returns>
		/// <returns>The current instance of this class.</returns>
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(String.Format("DELETE FROM {0}.{1}", this._EncloseBackTick(this._Database), this._EncloseBackTick(this._Table)));
			if (this._Wheres.Count > 0) {
				sb.AppendLine(String.Format("WHERE\n\t({0})", String.Join(" ", this._Wheres)));
			}
			return sb.ToString();
		}
		#endregion

		#region Private Method
		/// <summary>
		/// Initializes the properties.
		/// </summary>
		private void _InitProperties() {
			this._Wheres = new List<string>();
		}
		#endregion
	}
}
