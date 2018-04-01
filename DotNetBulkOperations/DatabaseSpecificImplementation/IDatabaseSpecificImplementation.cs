using System.Data;

namespace DotNetBulkOperations.DatabaseSpecificImplementation
{
    internal interface IDatabaseSpecificImplementation
    {
        void BulkInsert(IDbConnection dbConnection, DataTable dataTable, BulkOptions bulkOptions);
    }
}
