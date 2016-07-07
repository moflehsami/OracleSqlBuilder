using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;

namespace OracleSqlBuilder {
	/// <summary>
	/// OracleSql Exec Query class.
	/// </summary>
	public class OracleSqlExecQuery {
		#region Public Methods
		/// <summary>
		/// Executes a query and returns a OracleSqlData instance.
		/// </summary>
		/// <param name="ConnectionString">The connection string.</param>
		/// <param name="Select">The instance of OracleSqlBuilderSelect.</param>
		/// <returns>The instance of OracleSqlData.</returns>
		public OracleSqlData Execute(string ConnectionString, OracleSqlBuilderSelect Select) {
			if (String.IsNullOrWhiteSpace(ConnectionString)) {
				throw new ArgumentNullException("Connection String argument should not be null.");
			}
			if (Select == null) {
				throw new ArgumentNullException("Select argument should not be null.");
			}
			Stopwatch watch = new Stopwatch();
			watch.Start();
			OracleSqlData data = new OracleSqlData();
			using (OracleConnection cn = new OracleConnection()) {
				cn.ConnectionString = ConnectionString;
				try {
					cn.Open();
					using (OracleCommand cmd = new OracleCommand()) {
						cmd.Connection = cn;
						cmd.CommandText = Select.ToString();
						cmd.CommandType = CommandType.Text;
						Select.PrintQuery();
						if (Select.GetParameters().Count > 0) {
							foreach (KeyValuePair<string, object> kvp in Select.GetParameters()) {
								cmd.Parameters.Add(new OracleParameter(kvp.Key, kvp.Value));
							}
							Select.PrintParameters();
						}
						using (OracleDataAdapter da = new OracleDataAdapter()) {
							da.SelectCommand = cmd;
							try {
								da.Fill(data.DataSetResult);
								data.DataTableResult = data.DataSetResult.Tables[0];
								data.DataViewResult = data.DataTableResult.AsDataView();
								data.RowCount = data.DataTableResult.Rows.Count;
								if (data.RowCount > 0) {
									foreach (DataRow dr in data.DataTableResult.Rows) {
										Dictionary<string, object> dicRow = new Dictionary<string, object>();
										foreach (DataColumn dc in data.DataTableResult.Columns) {
											dicRow.Add(dc.ColumnName, dr[dc.ColumnName]);
										}
										data.ListResults.Add(dicRow);
									}
								}
							} catch (OracleException ex) {
								Debug.WriteLine("An error occured while trying to execute the query.");
								Debug.WriteLine(String.Format("OracleSQL Error: {0} - {1}", ex.ErrorCode, ex.Message));
								Debug.WriteLine(ex);
							}
						}
					}
				} catch (OracleException ex) {
					Debug.WriteLine("Unable to connect to the specified OracleSQL host.");
					Debug.WriteLine(String.Format("OracleSQL Error: {0} - {1}", ex.ErrorCode, ex.Message));
					Debug.WriteLine(ex);
				} finally {
					if (cn != null && cn.State == ConnectionState.Open) {
						cn.Close();
					}
				}
			}
			watch.Stop();
			data.Duration = watch.Elapsed;
			this._PrintRowsFound(data.RowCount);
			this._PrintDuration(data.Duration);
			return data;
		}

		/// <summary>
		/// Executes a query and returns a OracleSqlData instance.
		/// </summary>
		/// <param name="Select">The instance of OracleSqlBuilderSelect.</param>
		/// <returns>The instance of OracleSqlData.</returns>
		public OracleSqlData Execute(OracleSqlBuilderSelect Select) {
			return this.Execute(OracleSqlConnectionString.Read(), Select);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Prints the number of found rows to the output window.
		/// </summary>
		/// <param name="RowCount">The number of rows to be printed out.</param>
		private void _PrintRowsFound(long RowCount) {
			if (!OracleSqlConfig.Debug) {
				return;
			}
			Debug.WriteLine(String.Format("Rows Found: {0:n0}.", RowCount));
		}

		/// <summary>
		/// Prints the duration to the output window.
		/// </summary>
		/// <param name="Duration">The duration to be printed out.</param>
		private void _PrintDuration(TimeSpan Duration) {
			if (!OracleSqlConfig.Debug) {
				return;
			}
			Debug.WriteLine(String.Format("Duration: {0:c}.", Duration));
		} 
		#endregion
	}
}
