using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OracleSqlBuilder {
	/// <summary>
	/// OracleSql Builder Insert class.
	/// </summary>
	public class OracleSqlBuilderInsert : OracleSqlBuilderNonQuery {
		#region Private Property
		private Dictionary<string, object> _Inserts { get; set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes the database and table for the insert statement.
		/// </summary>
		/// <param name="Database">The database of the query.</param>
		/// <param name="Table">The table of the query.</param>
		public OracleSqlBuilderInsert(string Database, string Table) {
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
		/// Adds a pair of field and value for the INSERT clause.
		/// </summary>
		/// <param name="Condition">The condition to check before adding the pair for the INSERT clause.</param>
		/// <param name="Field">The field to be added.</param>
		/// <param name="Value">The value of the field.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderInsert SetInsert(bool Condition, string Field, object Value) {
			if (!Condition) {
				return this;
			}
			if (String.IsNullOrWhiteSpace(Field)) {
				throw new ArgumentException("Field argument should not be empty.");
			}
			if (!this._IsValidField(Field)) {
				throw new ArgumentException("Field argument is not a valid format.");
			}
			if (Value == null || Value == DBNull.Value) {
				this._SetInsert(Field, "NULL");
			} else if (Value is bool) {
				this._SetInsert(Field, Convert.ToInt16(Value).ToString());
			} else if (this._IsNumeric(Value)) {
				this._SetInsert(Field, Value);
			} else if (this._IsValidExpression(Value.ToString())) {
				this._SetInsert(Field, String.Format("'{0}'", Value));
			} else {
				string strParameterName = String.Format(":{0}", Regex.Replace(Regex.Replace(Field, @"\W", "_"), @"(?<=\w)([A-Z])", delegate (Match m) {
					return String.Format("{0}{1}", "_", m.Value);
				}));
				this._SetParameter(strParameterName, Value);
				this._SetInsert(Field, strParameterName);
			}
			return this;
		}

		/// <summary>
		/// Adds a pair of field and value for the INSERT clause.
		/// </summary>
		/// <param name="Field">The field to be added.</param>
		/// <param name="Value">The value of the field.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderInsert SetInsert(string Field, object Value) {
			return this.SetInsert(true, Field, Value);
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
			sb.AppendLine(String.Format("INSERT INTO {0}.{1}", this._EncloseBackTick(this._Database), this._EncloseBackTick(this._Table)));
			sb.AppendLine(String.Format("\t({0})", String.Join(",\n\t", this._Inserts.Keys.Select(x => this._Name(x)).ToArray())));
			sb.AppendLine(String.Format("VALUES\n\t({0})", String.Join(",\n\t", this._Inserts.Values.ToArray())));
			return sb.ToString();
		}
		#endregion

		#region Private Method
		/// <summary>
		/// Initializes the properties.
		/// </summary>
		private void _InitProperties() {
			this._Inserts = new Dictionary<string, object>();
		}

		/// <summary>
		/// Adds an .
		/// </summary>
		/// <param name="Field">The name of the parameter.</param>
		/// <param name="Value">The value of the parameter.</param>
		private void _SetInsert(string Field, object Value) {
			if (String.IsNullOrWhiteSpace(Field)) {
				throw new ArgumentException("Field argument should not be empty.");
			}
			if (!this._IsValidField(Field)) {
				throw new ArgumentException("Field argument is not a valid format.");
			}
			if (this._Inserts.ContainsKey(Field)) {
				this._Inserts[Field] = Value;
			} else {
				this._Inserts.Add(Field, Value);
			}
		}
		#endregion
	}
}
