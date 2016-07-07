using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;

namespace OracleSqlBuilder {
	/// <summary>
	/// OracleSql Exec Non Query class.
	/// </summary>
	public class OracleSqlExecNonQuery {
		#region Public Methods
		/// <summary>
		/// Executes a non-query and returns a OracleSqlNonData instance.
		/// </summary>
		/// <param name="ConnectionString">The connection string.</param>
		/// <param name="Inserts">The instance/s of OracleSqlBuilderInsert.</param>
		/// <returns>The instance of OracleSqlNonData.</returns>
		public OracleSqlNonData Execute(string ConnectionString, params OracleSqlBuilderInsert[] Inserts) {
			if (String.IsNullOrWhiteSpace(ConnectionString)) {
				throw new ArgumentNullException("Connection String argument should not be null.");
			}
			if (Inserts == null) {
				throw new ArgumentNullException("Inserts argument should not be null.");
			}
			if (Inserts.Length == 0) {
				throw new ArgumentException("Inserts argument should not be empty.");
			}
			Stopwatch watch = new Stopwatch();
			watch.Start();
			OracleSqlNonData data = new OracleSqlNonData();
			using (OracleConnection cn = new OracleConnection()) {
				cn.ConnectionString = ConnectionString;
				try {
					cn.Open();
					OracleTransaction tr = cn.BeginTransaction(IsolationLevel.ReadCommitted);
					try {
						double dblCounter = 0;
						foreach (OracleSqlBuilderInsert Insert in Inserts) {
							using (OracleCommand cmd = new OracleCommand()) {
								cmd.Connection = cn;
								cmd.CommandText = Insert.ToString();
								cmd.CommandType = CommandType.Text;
								Insert.PrintQuery();
								if (Insert.GetParameters().Count > 0) {
									foreach (KeyValuePair<string, object> kvp in Insert.GetParameters()) {
										cmd.Parameters.Add(new OracleParameter(kvp.Key, kvp.Value));
									}
									Insert.PrintParameters();
								}
								data.AffectedRows += cmd.ExecuteNonQuery();
							}
							dblCounter++;
						}
						tr.Commit();
					} catch (OracleException ex) {
						tr.Rollback();
						data.AffectedRows = 0;
						Debug.WriteLine("An error occured while trying to execute the query.");
						Debug.WriteLine(String.Format("OracleSql Error: {0} - {1}", ex.ErrorCode, ex.Message));
						Debug.WriteLine(ex);
					}
				} catch (OracleException ex) {
					Debug.WriteLine("Unable to connect to the specified OracleSql host.");
					Debug.WriteLine(String.Format("OracleSql Error: {0} - {1}", ex.ErrorCode, ex.Message));
					Debug.WriteLine(ex);
				} finally {
					if (cn != null && cn.State == ConnectionState.Open) {
						cn.Close();
					}
				}
			}
			watch.Stop();
			data.Duration = watch.Elapsed;
			this._PrintAffectedRows(data.AffectedRows);
			this._PrintDuration(data.Duration);
			return data;
		}

		/// <summary>
		/// Executes a non-query and returns a OracleSqlNonData instance.
		/// </summary>
		/// <param name="Inserts">The instance/s of OracleSqlBuilderInsert.</param>
		/// <returns>The instance of OracleSqlNonData.</returns>
		public OracleSqlNonData Execute(params OracleSqlBuilderInsert[] Inserts) {
			return this.Execute(OracleSqlConnectionString.Read("default"), Inserts);
		}

		/// <summary>
		/// Executes a non-query and returns a OracleSqlData instance.
		/// </summary>
		/// <param name="ConnectionString">The connection string.</param>
		/// <param name="Updates">The instance/s of OracleSqlBuilderUpdate.</param>
		/// <returns>The instance of OracleSqlNonData.</returns>
		public OracleSqlNonData Execute(string ConnectionString, params OracleSqlBuilderUpdate[] Updates) {
			if (String.IsNullOrWhiteSpace(ConnectionString)) {
				throw new ArgumentNullException("Connection String argument should not be null.");
			}
			if (Updates == null) {
				throw new ArgumentNullException("Updates argument should not be null.");
			}
			if (Updates.Length == 0) {
				throw new ArgumentException("Updates argument should not be empty.");
			}
			Stopwatch watch = new Stopwatch();
			watch.Start();
			OracleSqlNonData data = new OracleSqlNonData();
			using (OracleConnection cn = new OracleConnection()) {
				cn.ConnectionString = ConnectionString;
				try {
					cn.Open();
					OracleTransaction tr = cn.BeginTransaction(IsolationLevel.ReadCommitted);
					try {
						double dblCounter = 0;
						foreach (OracleSqlBuilderUpdate Update in Updates) {
							using (OracleCommand cmd = new OracleCommand()) {
								cmd.Connection = cn;
								cmd.CommandText = Update.ToString();
								cmd.CommandType = CommandType.Text;
								Update.PrintQuery();
								if (Update.GetParameters().Count > 0) {
									foreach (KeyValuePair<string, object> kvp in Update.GetParameters()) {
										cmd.Parameters.Add(new OracleParameter(kvp.Key, kvp.Value));
									}
									Update.PrintParameters();
								}
								data.AffectedRows += cmd.ExecuteNonQuery();
							}
							dblCounter++;
						}
						tr.Commit();
					} catch (OracleException ex) {
						tr.Rollback();
						data.AffectedRows = 0;
						Debug.WriteLine("An error occured while trying to execute the query.");
						Debug.WriteLine(String.Format("OracleSql Error: {0} - {1}", ex.ErrorCode, ex.Message));
						Debug.WriteLine(ex);
					}
				} catch (OracleException ex) {
					Debug.WriteLine("Unable to connect to the specified OracleSql host.");
					Debug.WriteLine(String.Format("OracleSql Error: {0} - {1}", ex.ErrorCode, ex.Message));
					Debug.WriteLine(ex);
				} finally {
					if (cn != null && cn.State == ConnectionState.Open) {
						cn.Close();
					}
				}
			}
			watch.Stop();
			data.Duration = watch.Elapsed;
			this._PrintAffectedRows(data.AffectedRows);
			this._PrintDuration(data.Duration);
			return data;
		}

		/// <summary>
		/// Executes a non-query and returns a OracleSqlNonData instance.
		/// </summary>
		/// <param name="Updates">The instance/s of OracleSqlBuilderUpdate.</param>
		/// <returns>The instance of OracleSqlNonData.</returns>
		public OracleSqlNonData Execute(params OracleSqlBuilderUpdate[] Updates) {
			return this.Execute(OracleSqlConnectionString.Read("default"), Updates);
		}

		/// <summary>
		/// Executes a non-query and returns a OracleSqlData instance.
		/// </summary>
		/// <param name="ConnectionString">The connection string.</param>
		/// <param name="Deletes">The instance/s of OracleSqlBuilderDelete.</param>
		/// <returns>The instance of OracleSqlNonData.</returns>
		public OracleSqlNonData Execute(string ConnectionString, params OracleSqlBuilderDelete[] Deletes) {
			if (String.IsNullOrWhiteSpace(ConnectionString)) {
				throw new ArgumentNullException("Connection String argument should not be null.");
			}
			if (Deletes == null) {
				throw new ArgumentNullException("Deletes argument should not be null.");
			}
			if (Deletes.Length == 0) {
				throw new ArgumentException("Deletes argument should not be empty.");
			}
			Stopwatch watch = new Stopwatch();
			watch.Start();
			OracleSqlNonData data = new OracleSqlNonData();
			using (OracleConnection cn = new OracleConnection()) {
				cn.ConnectionString = ConnectionString;
				try {
					cn.Open();
					OracleTransaction tr = cn.BeginTransaction(IsolationLevel.ReadCommitted);
					try {
						double dblCounter = 0;
						foreach (OracleSqlBuilderDelete Delete in Deletes) {
							using (OracleCommand cmd = new OracleCommand()) {
								cmd.Connection = cn;
								cmd.CommandText = Delete.ToString();
								cmd.CommandType = CommandType.Text;
								Delete.PrintQuery();
								if (Delete.GetParameters().Count > 0) {
									foreach (KeyValuePair<string, object> kvp in Delete.GetParameters()) {
										cmd.Parameters.Add(new OracleParameter(kvp.Key, kvp.Value));
									}
									Delete.PrintParameters();
								}
								data.AffectedRows += cmd.ExecuteNonQuery();
							}
							dblCounter++;
						}
						tr.Commit();
					} catch (OracleException ex) {
						tr.Rollback();
						data.AffectedRows = 0;
						Debug.WriteLine("An error occured while trying to execute the query.");
						Debug.WriteLine(String.Format("OracleSql Error: {0} - {1}", ex.ErrorCode, ex.Message));
						Debug.WriteLine(ex);
					}
				} catch (OracleException ex) {
					Debug.WriteLine("Unable to connect to the specified OracleSql host.");
					Debug.WriteLine(String.Format("OracleSql Error: {0} - {1}", ex.ErrorCode, ex.Message));
					Debug.WriteLine(ex);
				} finally {
					if (cn != null && cn.State == ConnectionState.Open) {
						cn.Close();
					}
				}
			}
			watch.Stop();
			data.Duration = watch.Elapsed;
			this._PrintAffectedRows(data.AffectedRows);
			this._PrintDuration(data.Duration);
			return data;
		}

		/// <summary>
		/// Executes a non-query and returns a OracleSqlNonData instance.
		/// </summary>
		/// <param name="Deletes">The instance/s of OracleSqlBuilderDelete.</param>
		/// <returns>The instance of OracleSqlNonData.</returns>
		public OracleSqlNonData Execute(params OracleSqlBuilderDelete[] Deletes) {
			return this.Execute(OracleSqlConnectionString.Read("default"), Deletes);
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Prints the affected rows to the output window.
		/// </summary>
		/// <param name="AffectedRows">The affected rows to be printed out.</param>
		private void _PrintAffectedRows(long AffectedRows) {
			if (!MySqlConfig.Debug) {
				return;
			}
			Debug.WriteLine(String.Format("Affected Rows: {0:n0}.", AffectedRows));
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
