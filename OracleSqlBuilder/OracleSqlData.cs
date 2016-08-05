using System;
using System.Collections.Generic;
using System.Data;

namespace OracleSqlBuilder {
    /// <summary>
    /// OracleSql Data class.
    /// </summary>
    public class OracleSqlData {
        #region Private Property
        private int _RecordIndex { get; set; }
        #endregion

        #region Internal Property
        /// <summary>
        /// The list of results.
        /// </summary>
        internal List<Dictionary<string, object>> ListResults { get; set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// Results as DataSet.
        /// </summary>
        public DataSet DataSetResult { get; internal set; }

        /// <summary>
        /// Results as DataTable.
        /// </summary>
        public DataTable DataTableResult { get; internal set; }

        /// <summary>
        /// Results as DataView.
        /// </summary>
        public DataView DataViewResult { get; internal set; }

        /// <summary>
        /// The number of rows/records.
        /// </summary>
        public long RowCount { get; internal set; }

        /// <summary>
        /// The duration of the query.
        /// </summary>
        public TimeSpan Duration { get; internal set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor method.
        /// </summary>
        public OracleSqlData() {
            this._InitProperties();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Checks if previous record exists.
        /// </summary>
        /// <returns>True if next record exists. Otherwise, false.</returns>
        public bool HasPrevious() {
            return this._RecordIndex > 0;
        }

        /// <summary>
        /// Moves the index to the previous record.
        /// </summary>
        public void Previous() {
            if (this.RowCount == 0) {
                return;
            }
            if (!this.HasPrevious()) {
                throw new IndexOutOfRangeException("The record index is currently at the first record.");
            }
            this._RecordIndex--;
        }

        /// <summary>
        /// Checks if next record exists.
        /// </summary>
        /// <returns>True if next record exists. Otherwise, false.</returns>
        public bool HasNext() {
            return this._RecordIndex < this.RowCount;
        }

        /// <summary>
        /// Moves the index to the next record.
        /// </summary>
        public void Next() {
            if (this.RowCount == 0) {
                return;
            }
            if (!this.HasNext()) {
                throw new IndexOutOfRangeException("The record index is currently at the last record.");
            }
            this._RecordIndex++;
        }

        /// <summary>
        /// Gets the first record of the result.
        /// </summary>
        /// <returns>The first record.</returns>
        public Dictionary<string, object> FirstRecord() {
            if (this.RowCount == 0) {
                return null;
            }
            return this.ListResults[0];
        }

        /// <summary>
        /// Gets the first record value by key of the result.
        /// </summary>
        /// <param name="Key">The key of the record.</param>
        /// <returns>The value of the record based on its key.</returns>
        public object FirstRecord(string Key) {
            if (this.RowCount == 0) {
                return null;
            }
            if (!this.ListResults[0].ContainsKey(Key)) {
                return null;
            }
            return this.FirstRecord()[Key];
        }

        /// <summary>
        /// Gets the last record of the result.
        /// </summary>
        /// <returns>The last record.</returns>
        public Dictionary<string, object> LastRecord() {
            if (this.RowCount == 0) {
                return null;
            }
            return this.ListResults[(int)this.RowCount - 1];
        }

        /// <summary>
        /// Gets the last record value by key of the result.
        /// </summary>
        /// <param name="Key">The key of the record.</param>
        /// <returns>The value of the record based on its key.</returns>
        public object LastRecord(string Key) {
            if (this.RowCount == 0) {
                return null;
            }
            if (!this.ListResults[(int)this.RowCount - 1].ContainsKey(Key)) {
                return null;
            }
            return this.ListResults[(int)this.RowCount - 1];
        }

        /// <summary>
        /// Gets the current record of the result.
        /// </summary>
        /// <returns>The current record.</returns>
        public Dictionary<string, object> Record() {
            if (this.RowCount == 0) {
                return null;
            }
            return this.ListResults[this._RecordIndex];
        }

        /// <summary>
        /// Gets a specific record value by key of the result.
        /// </summary>
        /// <param name="Key">The key of the record.</param>
        /// <returns>The value of the record based on its key.</returns>
        public object Record(string Key) {
            if (this.RowCount == 0) {
                return null;
            }
            if (!this.ListResults[this._RecordIndex].ContainsKey(Key)) {
                return null;
            }
            return this.ListResults[this._RecordIndex][Key];
        }

        /// <summary>
        /// Gets a specific record of the result.
        /// </summary>
        /// <param name="RecordIndex">The record index of the result to be retrieved.</param>
        /// <returns>The record.</returns>
        public Dictionary<string, object> Record(int RecordIndex) {
            if (this.RowCount == 0) {
                return null;
            }
            if (RecordIndex < 0) {
                throw new ArgumentOutOfRangeException("Record Index argument should be a positive number including zero '0'.");
            }
            if (RecordIndex >= this.RowCount) {
                throw new ArgumentOutOfRangeException("Record Index argument is out of range.");
            }
            return this.ListResults[RecordIndex];
        }

        /// <summary>
        /// Gets a specific record value by key of the result.
        /// </summary>
        /// <param name="RecordIndex">The record index.</param>
        /// <param name="Key">The key of the record.</param>
        /// <returns>The value of the record based on record index and its key.</returns>
        public object Record(int RecordIndex, string Key) {
            if (this.RowCount == 0) {
                return null;
            }
            if (RecordIndex < 0) {
                throw new ArgumentOutOfRangeException("Record Index argument should be a positive number including zero '0'.");
            }
            if (RecordIndex >= this.RowCount) {
                throw new ArgumentOutOfRangeException("Record Index argument is out of range.");
            }
            if (!this.ListResults[RecordIndex].ContainsKey(Key)) {
                return null;
            }
            return this.ListResults[RecordIndex][Key];
        }

        /// <summary>
        /// Gets the list of records.
        /// </summary>
        /// <returns>The list of records.</returns>
        public List<Dictionary<string, object>> Records() {
            if (this.RowCount == 0) {
                return null;
            }
            return this.ListResults;
        }

        /// <summary>
        /// Checks if there are records.
        /// </summary>
        /// <returns>True if there are records. Otherwise, false.</returns>
        public bool HasRecords() {
            return this.ListResults != null && this.RowCount > 0;
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Initializes the properties.
        /// </summary>
        private void _InitProperties() {
            this._RecordIndex = 0;
            this.ListResults = new List<Dictionary<string, object>>();
            this.DataSetResult = new DataSet();
            this.DataTableResult = new DataTable();
            this.DataViewResult = new DataView();
            this.RowCount = 0;
        }
        #endregion
    }
}
