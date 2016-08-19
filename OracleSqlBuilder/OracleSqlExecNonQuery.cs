using System;
using System.Data;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;

namespace OracleSqlBuilder {
    /// <summary>
    /// OracleSql Exec Non Query class.
    /// </summary>
    internal class OracleSqlExecNonQuery {
        #region Internal Methods
        /// <summary>
        /// Executes a non-query and returns a OracleSqlNonData instance.
        /// </summary>
        /// <param name="ConnectionString">The connection string.</param>
        /// <param name="Queries">The instance/s of OracleSqlBuilderNonQuery.</param>
        /// <returns>The instance of OracleSqlNonData.</returns>
        internal OracleSqlNonData Execute(string ConnectionString, params OracleSqlBuilderNonQuery[] Queries) {
            if (String.IsNullOrWhiteSpace(ConnectionString)) {
                throw new ArgumentNullException("Connection String argument should not be null.");
            }
            if (Queries == null) {
                throw new ArgumentNullException("Queries argument should not be null.");
            }
            if (Queries.Length == 0) {
                throw new ArgumentException("Queries argument should not be empty.");
            }
            var watch = new Stopwatch();
            watch.Start();
            var data = new OracleSqlNonData();
            using (var cn = new OracleConnection()) {
                cn.ConnectionString = ConnectionString;
                try {
                    cn.Open();
                    var tr = cn.BeginTransaction(IsolationLevel.ReadCommitted);
                    try {
                        double dblCounter = 0;
                        foreach (var Query in Queries) {
                            using (var cmd = new OracleCommand()) {
                                cmd.Connection = cn;
                                cmd.CommandText = Query.ToString();
                                cmd.CommandType = CommandType.Text;
                                Query.PrintQuery();
                                if (Query.GetParameters().Count > 0) {
                                    foreach (var kvp in Query.GetParameters()) {
                                        cmd.Parameters.Add(new OracleParameter(kvp.Key, kvp.Value));
                                    }
                                    Query.PrintParameters();
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
                        if (OracleSqlConfig.ThrowExceptions) {
                            throw;
                        }
                    } catch (Exception ex) {
                        tr.Rollback();
                        data.AffectedRows = 0;
                        Debug.WriteLine("An error occured while trying to execute the query.");
                        Debug.WriteLine(String.Format("Error: {0}", ex.Message));
                        Debug.WriteLine(ex);
                        if (OracleSqlConfig.ThrowExceptions) {
                            throw;
                        }
                    }
                } catch (OracleException ex) {
                    Debug.WriteLine("Unable to connect to the specified OracleSql host.");
                    Debug.WriteLine(String.Format("OracleSql Error: {0} - {1}", ex.ErrorCode, ex.Message));
                    Debug.WriteLine(ex);
                    if (OracleSqlConfig.ThrowExceptions) {
                        throw;
                    }
                } catch (Exception ex) {
                    Debug.WriteLine("Unable to connect to the specified OracleSql host.");
                    Debug.WriteLine(String.Format("Error: {0}", ex.Message));
                    Debug.WriteLine(ex);
                    if (OracleSqlConfig.ThrowExceptions) {
                        throw;
                    }
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
        /// <param name="Queries">The instance/s of OracleSqlBuilderNonQuery.</param>
        /// <returns>The instance of OracleSqlNonData.</returns>
        internal OracleSqlNonData Execute(params OracleSqlBuilderNonQuery[] Queries) {
            return this.Execute(OracleSqlConnectionString.Read("default"), Queries);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Prints the affected rows to the output window.
        /// </summary>
        /// <param name="AffectedRows">The affected rows to be printed out.</param>
        private void _PrintAffectedRows(long AffectedRows) {
            if (!OracleSqlConfig.Debug) {
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
