using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OracleSqlBuilder {
    /// <summary>
    /// OracleSql Builder Update class.
    /// </summary>
    public class OracleSqlBuilderUpdate : OracleSqlBuilderNonQuery {
        #region Private Property
        private Dictionary<string, object> _Updates { get; set; }
        private List<string> _Wheres { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the database and table for the insert statement.
        /// </summary>
        /// <param name="Database">The database of the query.</param>
        /// <param name="Table">The table of the query.</param>
        public OracleSqlBuilderUpdate(string Database, string Table) {
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
        /// <returns>The current instance of this class.</returns>
        public OracleSqlBuilderUpdate SetParameter(string Name, object Value) {
            this._SetParameter(Name, Value);
            return this;
        }

        /// <summary>
        /// Adds a pair of field and value for the UPDATE clause.
        /// </summary>
        /// <param name="Condition">The condition to check before adding the pair for the UPDATE clause.</param>
        /// <param name="Field">The field to be added.</param>
        /// <param name="Value">The value of the field.</param>
        /// <returns>The current instance of this class.</returns>
        public OracleSqlBuilderUpdate SetUpdate(bool Condition, string Field, object Value) {
            if (!Condition) {
                return this;
            }
            if (String.IsNullOrWhiteSpace(Field)) {
                throw new ArgumentException("Field argument should not be empty.");
            }
            if (!this._IsValidField(Field)) {
                throw new ArgumentException(String.Format("Field argument '{0}' is not a valid format.", Field));
            }
            if (Value == null || Value == DBNull.Value) {
                this._SetUpdate(Field, "NULL");
            } else if (Value is bool) {
                this._SetUpdate(Field, Convert.ToInt16(Value).ToString());
            } else if (this._IsValidExpression(Value.ToString())) {
                this._SetUpdate(Field, String.Format("'{0}'", Value));
            } else {
                string strParameterName = String.Format(":{0}", Regex.Replace(Field.Replace('-', '_'), @"(?<=\w)([A-Z])", delegate(Match m) {
                    return String.Format("{0}{1}", "_", m.Value);
                }));
                this._SetParameter(strParameterName, Value);
                this._SetUpdate(Field, strParameterName);
            }
            return this;
        }

        /// <summary>
        /// Adds a pair of field and value for the UPDATE clause.
        /// </summary>
        /// <param name="Field">The field to be added.</param>
        /// <param name="Value">The value of the field.</param>
        /// <returns>The current instance of this class.</returns>
        public OracleSqlBuilderUpdate SetUpdate(string Field, object Value) {
            return this.SetUpdate(true, Field, Value);
        }

        /// <summary>
        /// Adds a condition to the WHERE clause.
        /// </summary>
        /// <param name="Condition">The condition to check before adding to the WHERE clause.</param>
        /// <param name="ConditionStatement">The condition statement/s to be added.</param>
        /// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
        /// <returns>The current instance of this class.</returns>
        public OracleSqlBuilderUpdate SetWhere(bool Condition, string ConditionStatement, params object[] ParameterValues) {
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
        public OracleSqlBuilderUpdate SetWhere(string ConditionStatement, params object[] ParameterValues) {
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
            sb.AppendLine(String.Format("UPDATE {0}.{1}", this._EncloseBackTick(this._Database), this._EncloseBackTick(this._Table)));
            sb.AppendLine("SET");
            sb.AppendLine(String.Format("\t{0}", String.Join(",\n\t", this._Updates.Select(kv => String.Format("{0} = {1}", this._Name(kv.Key), kv.Value)))));
            if (this._Wheres.Count > 0) {
                sb.AppendLine(String.Format("WHERE\n\t({0})", String.Join(" AND ", this._Wheres)));
            }
            return sb.ToString();
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void _InitProperties() {
            this._Updates = new Dictionary<string, object>();
            this._Wheres = new List<string>();
        }

        /// <summary>
        /// Adds an .
        /// </summary>
        /// <param name="Field">The name of the parameter.</param>
        /// <param name="Value">The value of the parameter.</param>
        private void _SetUpdate(string Field, object Value) {
            if (String.IsNullOrWhiteSpace(Field)) {
                throw new ArgumentException("Field argument should not be empty.");
            }
            if (!this._IsValidField(Field)) {
                throw new ArgumentException("Field argument is not a valid format.");
            }
            if (this._Updates.ContainsKey(Field)) {
                this._Updates[Field] = Value;
            } else {
                this._Updates.Add(Field, Value);
            }
        }
        #endregion
    }
}
