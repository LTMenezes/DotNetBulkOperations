using System.Data;
using System.Data.SqlClient;

namespace DotNetBulkOperations.DatabaseSpecificImplementation
{
    internal class SqlServerBulkImplementation : IDatabaseSpecificImplementation
    {
        public void BulkInsert(IDbConnection dbConnection, DataTable dataTable, BulkOptions bulkOptions)
        {
            SqlConnection conn = (SqlConnection)dbConnection;

            SqlBulkCopyOptions options = SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.UseInternalTransaction;
            SqlTransaction transaction = null;

            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, options, transaction);

            sqlBulkCopy.DestinationTableName = dataTable.TableName;
            sqlBulkCopy.BulkCopyTimeout = bulkOptions.TimeoutInSeconds;

            foreach(DataColumn column in dataTable.Columns)
            {
                sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }

            sqlBulkCopy.WriteToServer(dataTable);
        }
    }
}
