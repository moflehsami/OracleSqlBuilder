
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OracleSqlBuilder {
	/// <summary>
	/// OracleSql Builder Select class.
	/// </summary>
	public class OracleSqlBuilderSelect : OracleSqlBuilderQuery {
		#region Private Properties
		private string _From { get; set; }
		private bool _IsDistinct { get; set; }
		private List<string> _Fields { get; set; }
		private List<string> _Joins { get; set; }
		private List<string> _Wheres { get; set; }
		private List<string> _Groups { get; set; }
		private bool _IsWithRollUp { get; set; }
		private List<string> _Havings { get; set; }
		private List<string> _Orders { get; set; }
		private double _LimitRowCount { get; set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes the database, table, and table alias for the SELECT statement.
		/// </summary>
		/// <param name="Database">The database of the query.</param>
		/// <param name="Table">The table of the query.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		public OracleSqlBuilderSelect(string Database, string Table, string TableAlias) {
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
			if (!String.IsNullOrWhiteSpace(TableAlias)) {
				this._TableAlias = this._RemoveBackTick(TableAlias);
			} else {
				this._TableAlias = Table;
			}
			this._From = String.Format("{0}.{1}{2}", this._EncloseBackTick(Database), this._EncloseBackTick(Table), !String.IsNullOrWhiteSpace(TableAlias) ? String.Format(" AS {0}", this._EncloseBackTick(this._RemoveBackTick(TableAlias))) : null);
			this._InitProperties();
		}

		/// <summary>
		/// Initializes the database, and table for the SELECT statement.
		/// </summary>
		/// <param name="Database">The database of the query.</param>
		/// <param name="Table">The table of the query.</param>
		public OracleSqlBuilderSelect(string Database, string Table)
			: this(Database, Table, null) {
		}

		/// <summary>
		/// Initializes the query as a table, and table alias for the SELECT statement.
		/// </summary>
		/// <param name="Select">The OracleSqlBuilderSelect instance.</param>
		/// <param name="TableAlias">The alias of the query.</param>
		public OracleSqlBuilderSelect(OracleSqlBuilderSelect Select, string TableAlias) {
			if (Select == null) {
				throw new ArgumentException("Select argument should not be null.");
			}
			string strQuery = Select.ToString();
			if (String.IsNullOrWhiteSpace(strQuery)) {
				throw new ArgumentException("Select argument should not be empty.");
			}
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			this._Table = String.Format("(\n{0}\n)", this._AddTab(strQuery));
			this._TableAlias = this._RemoveBackTick(TableAlias);
			this._InitProperties();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Sets a virtual field to be used in the query.
		/// </summary>
		/// <param name="Name">The name of the virtual field.</param>
		/// <param name="Expression">The query expression.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetVirtualField(string Name, string Expression) {
			this._SetVirtualField(Name, Expression);
			return this;
		}

		/// <summary>
		/// Sets a parameter to be used in the query.
		/// </summary>
		/// <param name="Name">The name of the parameter.</param>
		/// <param name="Value">The value of the parameter.</param>
		public OracleSqlBuilderSelect SetParameter(string Name, string Value) {
			this._SetParameter(Name, Value);
			return this;
		}

		/// <summary>
		/// Sets the query to be distinct or not. By default it is set to false.
		/// </summary>
		/// <param name="Distinct">Is the query distinct?</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetDistinct(bool Distinct) {
			this._IsDistinct = Distinct;
			return this;
		}

		/// <summary>
		/// Adds an expression/column to the SELECT clause.
		/// </summary>
		/// <param name="Expression">The expression/column to be added.</param>
		/// <param name="Alias">The alias of the expression.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetSelect(string Expression, string Alias) {
			if (String.IsNullOrWhiteSpace(Expression)) {
				throw new ArgumentException("Expression argument should not be empty.");
			}
			if (!this._IsValidExpression(Expression)) {
				throw new ArgumentException(String.Format("Expression argument '{0}' is not a valid format.", Expression));
			}
			if (this._VirtualFields.ContainsKey(Expression) && String.IsNullOrWhiteSpace(Alias)) {
				Alias = Expression;
			}
			this._Fields.Add(String.Format("{0}{1}", this._Name(Expression), String.IsNullOrWhiteSpace(Alias) ? null : String.Format(" AS {0}", this._EncloseBackTick(this._RemoveBackTick(Alias)))));
			return this;
		}

		/// <summary>
		/// Adds an expression/column to the SELECT clause.
		/// </summary>
		/// <param name="Expression">The expression/column to be added.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetSelect(string Expression) {
			this.SetSelect(Expression, null);
			return this;
		}

		/// <summary>
		/// Adds an expression/column to the SELECT clause.
		/// </summary>
		/// <param name="Select">The OracleSqlBuilderSelect instance.</param>
		/// <param name="Alias">The alias of the expression.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetSelect(OracleSqlBuilderSelect Select, string Alias) {
			if (Select == null) {
				throw new ArgumentException("Select argument should not be null.");
			}
			string strQuery = Select.ToString();
			if (String.IsNullOrWhiteSpace(strQuery)) {
				throw new ArgumentException("Select argument should not be empty.");
			}
			if (String.IsNullOrWhiteSpace(Alias)) {
				throw new ArgumentException("Alias argument should not be empty.");
			}
			this._Fields.Add(String.Format("(\n{0}\n) AS {1}", this._AddTab(strQuery), this._EncloseBackTick(this._RemoveBackTick(Alias))));
			return this;
		}

		/// <summary>
		/// Adds a LEFT JOIN clause.
		/// </summary>
		/// <param name="Database">The database of the table to be joined.</param>
		/// <param name="Table">The table to be joined.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetLeftJoin(string Database, string Table, string TableAlias, string ConditionStatement) {
			if (String.IsNullOrWhiteSpace(Database)) {
				throw new ArgumentException("Database argument should not be empty.");
			}
			if (!this._IsValidField(Database)) {
				throw new ArgumentException(String.Format("Database argument '{0}' should only contain any word character (letter, number, underscore).", Database));
			}
			if (String.IsNullOrWhiteSpace(Table)) {
				throw new ArgumentException("Table argument should not be empty.");
			}
			if (!this._IsValidField(Table)) {
				throw new ArgumentException(String.Format("Table argument '{0}' should only contain any word character (letter, number, underscore).", Table));
			}
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("LEFT JOIN {0}.{1} AS {2}\n\tON ({3})", this._EncloseBackTick(Database), this._EncloseBackTick(Table), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a LEFT JOIN clause.
		/// </summary>
		/// <param name="Table">The table to be joined.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetLeftJoin(string Table, string TableAlias, string ConditionStatement) {
			if (String.IsNullOrWhiteSpace(Table)) {
				throw new ArgumentException("Table argument should not be empty.");
			}
			if (!this._IsValidField(Table)) {
				throw new ArgumentException(String.Format("Table argument '{0}' should only contain any word character (letter, number, underscore).", Table));
			}
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("LEFT JOIN {0}.{1} AS {2}\n\tON ({3})", this._EncloseBackTick(this._Database), this._EncloseBackTick(Table), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a LEFT JOIN clause.
		/// </summary>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetLeftJoin(string TableAlias, string ConditionStatement) {
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("LEFT JOIN {0}.{1} AS {2}\n\tON ({3})", this._EncloseBackTick(this._Database), this._EncloseBackTick(this._Table), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a LEFT JOIN clause.
		/// </summary>
		/// <param name="Select">The OracleSqlBuilderSelect instance.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetLeftJoin(OracleSqlBuilderSelect Select, string TableAlias, string ConditionStatement) {
			if (Select == null) {
				throw new ArgumentException("Select argument should not be null.");
			}
			string strQuery = Select.ToString();
			if (String.IsNullOrWhiteSpace(strQuery)) {
				throw new ArgumentException("Select argument should not be empty.");
			}
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("LEFT JOIN (\n{0}\n) AS {1}\n\tON ({2})", this._AddTab(strQuery), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a LEFT JOIN clause.
		/// </summary>
		/// <param name="Selects">The list of OracleSqlBuilderSelect instances.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetLeftJoin(List<OracleSqlBuilderSelect> Selects, string TableAlias, string ConditionStatement) {
			if (Selects == null) {
				throw new ArgumentException("Selects argument should not be null.");
			}
			List<string> lstQueries = new List<string>();
			foreach (var Select in Selects) {
				if (Select == null) {
					throw new ArgumentException("A member of Selects argument should not be null.");
				}
				if (String.IsNullOrWhiteSpace(Select.ToString())) {
					throw new ArgumentException("A member of Selects argument should not be empty.");
				}
				lstQueries.Add(Select.ToString());
			}
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("LEFT JOIN (\n{0}\n) AS {1}\n\tON ({2})", this._AddTab(String.Format("({0})", String.Join(") UNION (", lstQueries.ToArray()))), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a RIGHT JOIN clause.
		/// </summary>
		/// <param name="Database">The database of the table to be joined.</param>
		/// <param name="Table">The table to be joined.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetRightJoin(string Database, string Table, string TableAlias, string ConditionStatement) {
			if (String.IsNullOrWhiteSpace(Database)) {
				throw new ArgumentException("Database argument should not be empty.");
			}
			if (!this._IsValidField(Database)) {
				throw new ArgumentException(String.Format("Database argument '{0}' should only contain any word character (letter, number, underscore).", Database));
			}
			if (String.IsNullOrWhiteSpace(Table)) {
				throw new ArgumentException("Table argument should not be empty.");
			}
			if (!this._IsValidField(Table)) {
				throw new ArgumentException(String.Format("Table argument '{0}' should only contain any word character (letter, number, underscore).", Table));
			}
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("RIGHT JOIN {0}.{1} AS {2}\n\tON ({3})", this._EncloseBackTick(Database), this._EncloseBackTick(Table), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a RIGHT JOIN clause.
		/// </summary>
		/// <param name="Table">The table to be joined.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetRightJoin(string Table, string TableAlias, string ConditionStatement) {
			if (String.IsNullOrWhiteSpace(Table)) {
				throw new ArgumentException("Table argument should not be empty.");
			}
			if (!this._IsValidField(Table)) {
				throw new ArgumentException(String.Format("Table argument '{0}' should only contain any word character (letter, number, underscore).", Table));
			}
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("RIGHT JOIN {0}.{1} AS {2}\n\tON ({3})", this._EncloseBackTick(this._Database), this._EncloseBackTick(Table), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a RIGHT JOIN clause.
		/// </summary>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetRightJoin(string TableAlias, string ConditionStatement) {
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("RIGHT JOIN {0}.{1} AS {2}\n\tON ({3})", this._EncloseBackTick(this._Database), this._EncloseBackTick(this._Table), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a RIGHT JOIN clause.
		/// </summary>
		/// <param name="Select">The OracleSqlBuilderSelect instance.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetRightJoin(OracleSqlBuilderSelect Select, string TableAlias, string ConditionStatement) {
			if (Select == null) {
				throw new ArgumentException("Select argument should not be null.");
			}
			string strQuery = Select.ToString();
			if (String.IsNullOrWhiteSpace(strQuery)) {
				throw new ArgumentException("Select argument should not be empty.");
			}
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("RIGHT JOIN (\n{0}\n) AS {1}\n\tON ({2})", this._AddTab(strQuery), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a LEFT JOIN clause.
		/// </summary>
		/// <param name="Selects">The list of OracleSqlBuilderSelect instances.</param>
		/// <param name="TableAlias">The alias of the table.</param>
		/// <param name="ConditionStatement">The condition statment/s of the joined table.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetRightJoin(List<OracleSqlBuilderSelect> Selects, string TableAlias, string ConditionStatement) {
			if (Selects == null) {
				throw new ArgumentException("Selects argument should not be null.");
			}
			List<string> lstQueries = new List<string>();
			foreach (var Select in Selects) {
				if (Select == null) {
					throw new ArgumentException("A member of Selects argument should not be null.");
				}
				if (String.IsNullOrWhiteSpace(Select.ToString())) {
					throw new ArgumentException("A member of Selects argument should not be empty.");
				}
				lstQueries.Add(Select.ToString());
			}
			TableAlias = this._RemoveBackTick(TableAlias);
			if (String.IsNullOrWhiteSpace(TableAlias)) {
				throw new ArgumentException("TableAlias argument should not be empty.");
			}
			ConditionStatement = this._RemoveBackTick(ConditionStatement);
			if (String.IsNullOrWhiteSpace(ConditionStatement)) {
				throw new ArgumentException("Condition argument should not be empty.");
			}
			this._Joins.Add(String.Format("RIGHT JOIN (\n{0}\n) AS {1}\n\tON ({2})", this._AddTab(String.Format("({0})", String.Join(") UNION (", lstQueries.ToArray()))), this._EncloseBackTick(TableAlias), this._Name(ConditionStatement)));
			return this;
		}

		/// <summary>
		/// Adds a condition to the WHERE clause.
		/// </summary>
		/// <param name="Condition">The condition to check before adding to the WHERE clause.</param>
		/// <param name="ConditionStatement">The condition statement/s to be added.</param>
		/// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetWhere(bool Condition, string ConditionStatement, params object[] ParameterValues) {
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
		public OracleSqlBuilderSelect SetWhere(string ConditionStatement, params object[] ParameterValues) {
			return this.SetWhere(true, ConditionStatement, ParameterValues);
		}

		/// <summary>
		/// Adds an expression/s to the GROUP BY clause.
		/// </summary>
		/// <param name="Expressions">Additional expressions to be added.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetGroupBy(params string[] Expressions) {
			if (Expressions != null & Expressions.Length > 0) {
				foreach (string strExpression in Expressions) {
					if (String.IsNullOrWhiteSpace(strExpression)) {
						continue;
					}
					if (!this._IsValidExpression(strExpression)) {
						throw new ArgumentException(String.Format("Expression '{0}' is not a valid format.", strExpression));
					}
					this._Groups.Add(this._Name(strExpression));
				}
			}
			return this;
		}

		/// <summary>
		/// Adds a WITH ROLLUP clause to the GROUP BY clause.
		/// </summary>
		/// <param name="WithRollUp"></param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetWithRollUp(bool WithRollUp) {
			this._IsWithRollUp = WithRollUp;
			return this;
		}

		/// <summary>
		/// Adds a condition to the HAVING clause.
		/// </summary>
		/// <param name="Condition">The condition to check before adding to the HAVING clause.</param>
		/// <param name="ConditionStatement">The condition statement/s to be added.</param>
		/// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetHaving(bool Condition, string ConditionStatement, params object[] ParameterValues) {
			if (Condition) {
				ConditionStatement = this._RemoveBackTick(ConditionStatement);
				if (String.IsNullOrWhiteSpace(ConditionStatement)) {
					throw new ArgumentException("Condition argument should not be empty.");
				}
				List<string> lstParameters = new List<string>();
				if (ParameterValues != null && ParameterValues.Length > 0) {
					foreach (object objParameterValue in ParameterValues) {
						string strParameter = String.Format(":having_condition_{0}", this._Parameters.Count(kv => kv.Key.Contains(":having_condition")) + 1);
						this._SetParameter(strParameter, objParameterValue);
						lstParameters.Add(strParameter);
					}
				}
				this._Havings.Add(this._Name(String.Format(ConditionStatement, lstParameters.ToArray())));
			}
			return this;
		}

		/// <summary>
		/// Adds a condition to the HAVING clause.
		/// </summary>
		/// <param name="ConditionStatement">The condition statement/s to be added.</param>
		/// <param name="ParameterValues">The arguments to be passed for formatting a string.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetHaving(string ConditionStatement, params object[] ParameterValues) {
			return this.SetHaving(true, ConditionStatement, ParameterValues);
		}

		/// <summary>
		/// Adds an expression/s to the ORDER BY clause.
		/// </summary>
		/// <param name="Direction">The order direction of the expression/s.</param>
		/// <param name="Expressions">The expression to be ordered.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetOrderBy(OrderDirections Direction, params string[] Expressions) {
			if (Expressions != null & Expressions.Length > 0) {
				List<string> lstExpression = new List<string>();
				foreach (string strExpression in Expressions) {
					if (String.IsNullOrWhiteSpace(strExpression)) {
						continue;
					}
					if (!this._IsValidExpression(strExpression)) {
						throw new ArgumentException(String.Format("Expression '{0}' is not a valid format.", strExpression));
					}
					lstExpression.Add(this._Name(strExpression));
				}
				if (lstExpression.Count > 0) {
					this._Orders.Add(String.Format("{0} {1}", String.Join(", ", lstExpression.ToArray()), this._GetOrderDirection(Direction)));
				}
			}
			return this;
		}

		/// <summary>
		/// Adds an expression/s to the ORDER BY clause.
		/// </summary>
		/// <param name="Expressions">The expression to be ordered.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetOrderBy(params string[] Expressions) {
			this.SetOrderBy(OrderDirections.Asc, Expressions);
			return this;
		}

		/// <summary>
		/// Sets the LIMIT clause.
		/// </summary>
		/// <param name="RowCount">The row count limit.</param>
		/// <returns>The current instance of this class.</returns>
		public OracleSqlBuilderSelect SetLimit(double RowCount) {
			this._LimitRowCount = RowCount;
			return this;
		}
		#endregion

		#region Public Override Method
		/// <summary>
		/// Builds the query string.
		/// </summary>
		/// <returns>The query string.</returns>
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("SELECT");
			if (this._Fields.Count > 0) {
				sb.AppendLine(String.Format("\t{0}", String.Join(",\n\t", this._Fields)));
			} else {
				sb.AppendLine("\t*");
			}
			sb.AppendLine(String.Format("FROM {0}", this._From));
			if (this._Joins.Count > 0) {
				sb.AppendLine(String.Join("\n", this._Joins));
			}
			if (this._Wheres.Count > 0) {
				sb.AppendLine(String.Format("WHERE\n\t({0})", String.Join(" ", this._Wheres)));
			}
			if (this._LimitRowCount > 0) {
				if (this._Wheres.Count > 0) {
					sb.AppendLine(String.Format(" AND ROWNUM <= {0}", this._LimitRowCount));
				} else {
					sb.AppendLine(String.Format("WHERE\n\t(ROWNUM <= {0})", this._LimitRowCount));
				}
			}
			if (this._Groups.Count > 0) {
				string strGroupBy = String.Format("GROUP BY {0}", String.Join(", ", this._Groups));
				if (this._IsWithRollUp) {
					sb.Append(strGroupBy);
				} else {
					sb.AppendLine(strGroupBy);
				}
			}
			if (this._IsWithRollUp) {
				sb.AppendLine(" WITH ROLLUP");
			}
			if (this._Havings.Count > 0) {
				sb.AppendLine(String.Format("HAVING\n\t({0})", String.Join(" ", this._Havings)));
			}
			if (this._Orders.Count > 0) {
				sb.AppendLine(String.Format("ORDER BY {0}", String.Join(", ", this._Orders)));
			}
			return sb.ToString().Trim();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Initializes the properties.
		/// </summary>
		private void _InitProperties() {
			this._Fields = new List<string>();
			this._Joins = new List<string>();
			this._Wheres = new List<string>();
			this._Groups = new List<string>();
			this._Havings = new List<string>();
			this._Orders = new List<string>();
		}

		/// <summary>
		/// Gets the order direction in string format.
		/// </summary>
		/// <param name="Dir">The order direction.</param>
		/// <returns>The order direction in string format</returns>
		private string _GetOrderDirection(OrderDirections Dir) {
			switch (Dir) {
				case OrderDirections.Asc:
					return "ASC";
				case OrderDirections.Desc:
					return "DESC";
				default:
					return "ASC";
			}
		}
		#endregion
	}
}
