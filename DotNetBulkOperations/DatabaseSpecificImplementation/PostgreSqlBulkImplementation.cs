using Npgsql;
using System.Data;
using System.Text;

namespace DotNetBulkOperations.DatabaseSpecificImplementation
{
    internal class PostgreSqlBulkImplementation : IDatabaseSpecificImplementation
    {
        public void BulkInsert(IDbConnection dbConnection, DataTable dataTable, BulkOptions bulkOptions)
        {
            NpgsqlConnection connection = (NpgsqlConnection)dbConnection;

            using (NpgsqlBinaryImporter writer = connection.BeginBinaryImport(BuildCopyFromCommand(dataTable)))
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    writer.WriteRow(dataRow.ItemArray);
                }
            }
        }

        private string BuildCopyFromCommand(DataTable dataTable)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"COPY {dataTable.TableName} (");

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if (i != 0)
                {
                    stringBuilder.Append(",");
                }

                stringBuilder.Append(dataTable.Columns[i].ColumnName);
            }

            stringBuilder.Append(") FROM STDIN (FORMAT BINARY)");

            return stringBuilder.ToString();
        }
    }
}
