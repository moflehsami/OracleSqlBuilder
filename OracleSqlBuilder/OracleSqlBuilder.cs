using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OracleSqlBuilder {
	/// <summary>
	/// OracleSql Builder class.
	/// </summary>
	public abstract class OracleSqlBuilder {
		#region Protected Properties
		/// <summary>
		/// Reserved Keywords property.
		/// </summary>
		protected List<string> _ReservedKeywords { get; private set; }

		/// <summary>
		/// Virtual Fields property.
		/// </summary>
		protected Dictionary<string, string> _VirtualFields { get; private set; }

		/// <summary>
		/// Database property.
		/// </summary>
		protected string _Database { get; set; }

		/// <summary>
		/// Table property.
		/// </summary>
		protected string _Table { get; set; }

		/// <summary>
		/// Table Alias property.
		/// </summary>
		protected string _TableAlias { get; set; }

		/// <summary>
		/// Parameters property.
		/// </summary>
		protected Dictionary<string, object> _Parameters { get; private set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public OracleSqlBuilder() {
			this._InitProperties();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets the parameters.
		/// </summary>
		/// <returns>The parameters.</returns>
		public Dictionary<string, object> GetParameters() {
			return this._Parameters;
		}

		/// <summary>
		/// Prints the parameters to the output window.
		/// </summary>
		public void PrintParameters() {
			if (!OracleSqlConfig.Debug) {
				return;
			}
			Debug.WriteLine(null);
			Debug.WriteLine("--------------------------------------------------");
			if (this._Parameters == null || this._Parameters.Count == 0) {
				Debug.WriteLine("No parameters available.");
				return;
			}
			Debug.WriteLine("OracleSQL Parameters:");
			int intCounter = 1;
			foreach (KeyValuePair<string, object> kvp in this._Parameters) {
				Debug.WriteLine(String.Format("Parameter {0}:", intCounter));
				Debug.WriteLine(String.Format("Type:\t{0}", kvp.Value != null ? kvp.Value.GetType() : null));
				Debug.WriteLine(String.Format("Name:\t{0}", kvp.Key));
				Debug.WriteLine(String.Format("Value:\t{0}", kvp.Value));
				Debug.WriteLine(null);
				intCounter++;
			}
		}

		/// <summary>
		/// Merges the parameters with other parameters.
		/// </summary>
		/// <param name="Parameters">The parameters to be merged.</param>
		public void MergeParameters(params Dictionary<string, object>[] Parameters) {
			if (Parameters == null || Parameters.Length == 0) {
				return;
			}
			foreach (Dictionary<string, object> Parameter in Parameters) {
				foreach (KeyValuePair<string, object> kvp in Parameter) {
					if (this._Parameters.ContainsKey(kvp.Key)) {
						this._Parameters[kvp.Key] = kvp.Value;
					} else {
						this._Parameters.Add(kvp.Key, kvp.Value);
					}
				}
			}
		}
		#endregion

		#region Public Virtual Method
		/// <summary>
		/// Prints the query to the output window.
		/// </summary>
		public virtual void PrintQuery() {
			if (!OracleSqlConfig.Debug) {
				return;
			}
			Debug.WriteLine(null);
			Debug.WriteLine("==================================================");
			Debug.WriteLine("OracleSQL Query:");
			Debug.WriteLine(this);
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Sets a virtual field to be used in the query.
		/// </summary>
		/// <param name="Name">The name of the virtual field.</param>
		/// <param name="Expression">The query expression.</param>
		protected void _SetVirtualField(string Name, string Expression) {
			if (String.IsNullOrWhiteSpace(Name)) {
				throw new ArgumentException("Name argument should not be empty.");
			}
			if (Regex.IsMatch(Name, @"\W")) {
				throw new ArgumentException("Name argument should only contain any word character (letter, number, underscore).");
			}
			if (String.IsNullOrWhiteSpace(Expression)) {
				throw new ArgumentException("Expression argument should not be empty.");
			}
			if (this._VirtualFields.ContainsKey(Name)) {
				this._VirtualFields[Name] = Expression;
			} else {
				this._VirtualFields.Add(Name, Expression);
			}
		}

		/// <summary>
		/// Sets a parameter to be used in the query.
		/// </summary>
		/// <param name="Name">The name of the parameter.</param>
		/// <param name="Value">The value of the parameter.</param>
		protected void _SetParameter(string Name, object Value) {
			if (String.IsNullOrWhiteSpace(Name)) {
				throw new ArgumentException("Name argument should not be empty.");
			}
			if (!Regex.IsMatch(Name, @"^\:[\w]+$")) {
				throw new ArgumentException("Name argument should only contain ':' and any word character (letter, number, underscore) after.");
			}
			if (this._Parameters.ContainsKey(Name)) {
				this._Parameters[Name] = Value;
			} else {
				this._Parameters.Add(Name, Value);
			}
		}

		/// <summary>
		/// Checks if an expression is valid.
		/// </summary>
		/// <param name="Expression">The expression to be checked.</param>
		/// <returns>True if valid. Otherwise, false.</returns>
		protected bool _IsValidExpression(string Expression) {
			return Regex.IsMatch(Expression, @"^(?:[\w]+\.)?[\w]+$");
		}

		/// <summary>
		/// Checks if a field is valid.
		/// </summary>
		/// <param name="Field">The field to be checked.</param>
		/// <returns>True if valid. Otherwise, false.</returns>
		protected bool _IsValidField(string Field) {
			return Regex.IsMatch(Field, @"\w");
		}

		/// <summary>
		/// Checks if a value is numeric.
		/// </summary>
		/// <param name="Value">The value to be checked.</param>
		/// <returns>True if numeric. Otherwise, false.</returns>
		protected bool _IsNumeric(object Value) {
			if (Value is string) {
				return Regex.IsMatch(Value.ToString(), @"^(?:\+|\-)?(?:0|[1-9][\d]*)(?:\.[\d]+)?$");
			} else {
				return Value is sbyte || Value is byte || Value is short || Value is ushort || Value is int || Value is uint || Value is long || Value is ulong || Value is float || Value is double || Value is decimal;
			}
		}

		/// <summary>
		/// Formats the given value to a safe backticked value.
		/// </summary>
		/// <param name="Value">The value to be formatted.</param>
		/// <returns>The safe backticked value.</returns>
		protected string _Name(string Value) {
			string strValue = Value.Trim();
			if (!String.IsNullOrWhiteSpace(strValue)) {
				if (strValue == "*") {
					return strValue;
				}
				if (this._IsNumeric(strValue)) {
					return strValue;
				}
				if (this._ReservedKeywords.Contains(strValue)) {
					return strValue.ToUpper();
				}
				if (!this._ReservedKeywords.Contains(strValue) && Regex.IsMatch(strValue, @"^[\w]+(?:\.[^ \*]*)*$", RegexOptions.IgnoreCase)) {
					// with string
					if (!strValue.Contains(".")) {
						if (this._VirtualFields.ContainsKey(strValue)) {
							return this._VirtualFields[strValue];
						} else {
							return String.Format("{0}.{1}", this._EncloseBackTick(!String.IsNullOrWhiteSpace(this._TableAlias) ? this._TableAlias : this._Table), this._EncloseBackTick(strValue));
						}
					}
					// with string.string
					else {
						string[] strDataArray = strValue.Split('.');
						return this._EncloseBackTick(String.Join("\".\"", strDataArray));
					}
				}
				// with string.*
				if (Regex.IsMatch(strValue, @"^[\w]+\.\*$", RegexOptions.IgnoreCase)) {
					return String.Format("\"{0}", strValue.Replace(".*", "\".*"));
				}
				// with functions and all others
				if (Regex.IsMatch(strValue, @"\@?(?:\'[^\']+\'|(?:(?:\w\.)?\w)+(?:\s*\()?)", RegexOptions.IgnoreCase)) {
					MatchEvaluator evaluator = new MatchEvaluator(this._NameMatched);
					return Regex.Replace(strValue, @"\@?(?:\'[^\']+\'|(?:(?:\w\.)?\w)+(?:\s*\()?)", evaluator);
				}
				//// with functions
				//if (Regex.IsMatch(strValue, @"^([\w]+)\((.*)\)$", RegexOptions.IgnoreCase)) {
				//	string[] strMatches = Regex.Split(strValue, @"^([\w]+)\((.*)\)$", RegexOptions.IgnoreCase);
				//	return String.Format("{0}({1})", strMatches[1], this._Name(strMatches[2]));
				//}
				//// with alias
				//if (Regex.IsMatch(strValue, @"^([\w]+(\.[\w]+|\(.*\))*)\s+AS\s+([\w]+)$", RegexOptions.IgnoreCase)) {
				//	string[] strMatches = Regex.Split(strValue, @"^([\w]+(\.[\w]+|\(.*\))*)\s+AS\s+([\w]+)$", RegexOptions.IgnoreCase);
				//	return String.Format("{0} AS {1}", this._Name(strMatches[1]), this._Name(strMatches[2]));
				//}
				//if (Regex.IsMatch(strValue, @"((?<![\\])[\'])((?:.(?!(?<![\\])\1))*.?)\1|[\:\']?[\w]+(?:\.[\w]*)*[\(\']?", RegexOptions.IgnoreCase)) {
				//	MatchEvaluator evaluator = new MatchEvaluator(this._NameMatched);
				//	return Regex.Replace(strValue, @"((?<![\\])[\'])((?:.(?!(?<![\\])\1))*.?)\1|[\:\']?[\w]+(?:\.[\w]*)*[\(\']?", evaluator);
				//}
				//if (Regex.IsMatch(strValue, @"^[\w\s]*[\w]+", RegexOptions.IgnoreCase)) {
				//	return this._EncloseBackTick(strValue);
				//}
			}
			return strValue;
		}

		/// <summary>
		/// Encloses the value with backticks.
		/// </summary>
		/// <param name="Value">The value to be formatted.</param>
		/// <returns>The backticked value.</returns>
		protected string _EncloseBackTick(string Value) {
			if (String.IsNullOrWhiteSpace(Value)) {
				return Value;
			}
			return String.Format("\"{0}\"", Value).Trim();
		}

		/// <summary>
		/// Removes backticks from a value.
		/// </summary>
		/// <param name="Value">The value to be formatted.</param>
		/// <returns>The backtick-free value.</returns>
		protected string _RemoveBackTick(string Value) {
			if (String.IsNullOrWhiteSpace(Value)) {
				return Value;
			}
			return Value.Replace("\"", "").Trim();
		}

		/// <summary>
		/// Adds a tab character on each line of the value.
		/// </summary>
		/// <param name="Value">The value to be formatted.</param>
		/// <returns>A formatted value.</returns>
		protected string _AddTab(string Value) {
			StringBuilder sb = new StringBuilder();
			using (StringReader reader = new StringReader(Value)) {
				string strLine;
				while ((strLine = reader.ReadLine()) != null) {
					sb.AppendLine(String.Format("\t{0}", strLine));
				}
			}
			return sb.ToString().Trim();
		}
		#endregion

		#region Private Method
		/// <summary>
		/// Initializes the properties.
		/// </summary>
		private void _InitProperties() {
			this._ReservedKeywords = new List<string>() { "ABORT", "ACCEPT", "ACCESS", "ADD", "ADMIN", "AFTER", "ALL", "ALLOCATE", "ALTER", "ANALYZE", "AND", "ANY", "ARCHIVE", "ARCHIVELOG", "ARRAY", "ARRAYLEN", "AS", "ASC", "ASSERT", "ASSIGN", "AT", "AUDIT", "AUTHORIZATION", "AVG", "BACKUP", "BASE_TABLE", "BECOME", "BEFORE", "BEGIN", "BETWEEN", "BINARY_INTEGER", "BLOCK", "BODY", "BOOLEAN", "BY", "CACHE", "CANCEL", "CASCADE", "CASE", "CHANGE", "CHAR", "CHAR_BASE", "CHARACTER", "CHECK", "CHECKPOINT", "CLOSE", "CLUSTER", "CLUSTERS", "COBOL", "COLAUTH", "COLUMN", "COLUMNS", "COMMENT", "COMMIT", "COMPILE", "COMPRESS", "CONNECT", "CONSTANT", "CONSTRAINT", "CONSTRAINTS", "CONTENTS", "CONTINUE", "CONTROLFILE", "COUNT", "CRASH", "CREATE", "CURRENT", "CURRVAL", "CURSOR", "CYCLE", "DATA_BASE", "DATABASE", "DATAFILE", "DATE", "DAY", "DBA", "DEBUGOFF", "DEBUGON", "DEC", "DECIMAL", "DECLARE", "DEFAULT", "DEFINITION", "DELAY", "DELETE", "DELTA", "DESC", "DIGITS", "DISABLE", "DISMOUNT", "DISPOSE", "DISTINCT", "DO", "DOUBLE", "DROP", "DUMP", "EACH", "ELSE", "ELSIF", "ENABLE", "END", "ENTRY", "ESCAPE", "EVENTS", "EXCEPT", "EXCEPTION", "EXCEPTION_INIT", "EXCEPTIONS", "EXCLUSIVE", "EXEC", "EXECUTE", "EXISTS", "EXIT", "EXPLAIN", "EXTENT", "EXTERNALLY", "FALSE", "FETCH", "FILE", "FLOAT", "FLUSH", "FOR", "FORCE", "FOREIGN", "FORM", "FORTRAN", "FOUND", "FREELIST", "FREELISTS", "FROM", "FUNCTION", "GENERIC", "GO", "GOTO", "GRANT", "GROUP", "GROUPS", "HAVING", "HOUR", "IDENTIFIED", "IF", "IMMEDIATE", "IN", "INCLUDING", "INCREMENT", "INDEX", "INDEXES", "INDICATOR", "INITIAL", "INITRANS", "INSERT", "INSTANCE", "INT", "INTEGER", "INTERSECT", "INTERVAL", "INTO", "IS", "KEY", "LANGUAGE", "LAYER", "LEVEL", "LIKE", "LIMITED", "LINK", "LISTS", "LOCK", "LOGFILE", "LONG", "LOOP", "MANAGE", "MANUAL", "MAX", "MAXDATAFILES", "MAXEXTENTS", "MAXINSTANCES", "MAXLOGFILES", "MAXLOGHISTORY", "MAXLOGMEMBERS", "MAXTRANS", "MAXVALUE", "MIN", "MINEXTENTS", "MINUS", "MINUTE", "MINVALUE", "MLSLABEL", "MOD", "MODE", "MODIFY", "MODULE", "MONTH", "MOUNT", "NATURAL", "NEW", "NEXT", "NEXTVAL", "NOARCHIVELOG", "NOAUDIT", "NOCACHE", "NOCOMPRESS", "NOCYCLE", "NOMAXVALUE", "NOMINVALUE", "NONE", "NOORDER", "NORESETLOGS", "NORMAL", "NOSORT", "NOT", "NOTFOUND", "NOWAIT", "NULL", "NUMBER", "NUMBER_BASE", "NUMERIC", "OF", "OFF", "OFFLINE", "OLD", "ON", "ONLINE", "ONLY", "OPEN", "OPTIMAL", "OPTION", "OR", "ORDER", "OTHERS", "OUT", "OWN", "PACKAGE", "PARALLEL", "PARTITION", "PCTFREE", "PCTINCREASE", "PCTUSED", "PLAN", "PLI", "POSITIVE", "PRAGMA", "PRECISION", "PRIMARY", "PRIOR", "PRIVATE", "PRIVILEGES", "PROCEDURE", "PROFILE", "PUBLIC", "QUOTA", "RAISE", "RANGE", "RAW", "READ", "REAL", "RECORD", "RECOVER", "REFERENCES", "REFERENCING", "RELEASE", "REMR", "RENAME", "RESETLOGS", "RESOURCE", "RESTRICTED", "RETURN", "REUSE", "REVERSE", "REVOKE", "ROLE", "ROLES", "ROLLBACK", "ROW", "ROWID", "ROWLABEL", "ROWNUM", "ROWS", "ROWTYPE", "RUN", "SAVEPOINT", "SCHEMA", "SCN", "SECOND", "SECTION", "SEGMENT", "SELECT", "SEPARATE", "SEQUENCE", "SESSION", "SET", "SHARE", "SHARED", "SIZE", "SMALLINT", "SNAPSHOT", "SOME", "SORT", "SPACE", "SQL", "SQLBUF", "SQLCODE", "SQLERRM", "SQLERROR", "SQLSTATE", "START", "STATEMENT", "STATEMENT_ID", "STATISTICS", "STDDEV", "STOP", "STORAGE", "SUBTYPE", "SUCCESSFUL", "SUM", "SWITCH", "SYNONYM", "SYSDATE", "SYSTEM", "TABAUTH", "TABLE", "TABLES", "TABLESPACE", "TASK", "TEMPORARY", "TERMINATE", "THEN", "THREAD", "TIME", "TO", "TRACING", "TRANSACTION", "TRIGGER", "TRIGGERS", "TRUE", "TRUNCATE", "TYPE", "UID", "UNDER", "UNION", "UNIQUE", "UNLIMITED", "UNTIL", "UPDATE", "USE", "USER", "USING", "VALIDATE", "VALUES", "VARCHAR", "VARCHAR2", "VARIANCE", "VIEW", "VIEWS", "WHEN", "WHENEVER", "WHERE", "WHILE", "WITH", "WORK", "WRITE", "XOR", "YEAR" };
			this._VirtualFields = new Dictionary<string, string>();
			this._Parameters = new Dictionary<string, object>();
		}

		/// <summary>
		/// Formats the matched value.
		/// </summary>
		/// <param name="MatchedValue">The matched value to be formatted.</param>
		/// <returns>The formatted match value.</returns>
		private string _NameMatched(Match MatchedValue) {
			string strValue = MatchedValue.Value;
			if (!strValue.Contains("(") && !strValue.Contains(":") && !strValue.Contains("'") && !this._ReservedKeywords.Contains(strValue)) {
				if (Regex.IsMatch(strValue.Substring(strValue.Length - 1, 1), @"\W", RegexOptions.IgnoreCase)) {
					return String.Format("{0}{1}", this._Name(strValue.Substring(0, strValue.Length - 1)), strValue.Substring(strValue.Length - 1, 1));
				}
				return this._Name(strValue);
			}
			return strValue;
		}

		///// <summary>
		///// Formats the matched value.
		///// </summary>
		///// <param name="MatchedValue">The matched value to be formatted.</param>
		///// <returns>The formatted match value.</returns>
		//private string _NameMatched(Match MatchedValue) {
		//	if (!MatchedValue.Value.Contains("(") && !MatchedValue.Value.Contains(":") && !this._ReservedKeywords.Contains(MatchedValue.Value)) {
		//		if (Regex.IsMatch(MatchedValue.Value, @"^[\w]+(?:\.[^ \*]*)*$")) {
		//			return this._Name(MatchedValue.Value);
		//		}
		//	}
		//	return MatchedValue.Value;
		//}
		#endregion
	}
}
