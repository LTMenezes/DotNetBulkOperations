using DotNetBulkOperations.Attributes;
using DotNetBulkOperations.DatabaseSpecificImplementation;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace DotNetBulkOperations
{
    /// <summary>
    /// Bulk Operations.
    /// </summary>
    public static class BulkOperations
    {
        /// <summary>
        /// Bulk insert method for bulking a IEnumerable of entities.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="dbConnection"></param>
        /// <param name="entities"></param>
        /// <param name="bulkOptions">Optional bulk options.</param>
        public static void BulkInsert<T>(this IDbConnection dbConnection, IEnumerable<T> entities, BulkOptions bulkOptions = null) where T : class
        {
            if(dbConnection == null || entities == null)
            {
                throw new ArgumentNullException("Invalid null arguments");
            }

            Contract.EndContractBlock();

            if(bulkOptions == null)
            {
                bulkOptions = new BulkOptions();
            }

            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }

            DataTable dataTable = BuildDataTableForEntities(entities);

            CallDatabaseSpecificOperation(dbConnection, BulkOperationsEnum.BulkInsert, dataTable, bulkOptions);
        }

        internal static Type GetPropertyCorrespondingDataType(PropertyInfo propertyInfo)
        {
            Type typeOfColumn = propertyInfo.PropertyType;

            Type nullableUnderlyingType = Nullable.GetUnderlyingType(typeOfColumn);

            typeOfColumn = nullableUnderlyingType == null ? typeOfColumn : nullableUnderlyingType;

            return typeOfColumn;
        }

        private static void CallDatabaseSpecificOperation(IDbConnection dbConnection, BulkOperationsEnum bulkOperationsEnum, DataTable dataTable, BulkOptions bulkOptions)
        {
            IDatabaseSpecificImplementation implementation = GetDatabaseSpecificImplementation(dbConnection);

            switch (bulkOperationsEnum)
            {
                case BulkOperationsEnum.BulkInsert:
                    implementation.BulkInsert(dbConnection, dataTable, bulkOptions);
                    return;

                default:
                    throw new Exception("Unknown database.");
            }
        }

        private static IDatabaseSpecificImplementation GetDatabaseSpecificImplementation(IDbConnection dbConnection)
        {
            switch (dbConnection)
            {
                case SqlConnection sqlConnection:
                    return new SqlServerBulkImplementation();

                case NpgsqlConnection npgsqlConnection:
                    return new PostgreSqlBulkImplementation();

                default:
                    throw new ArgumentException("Unknown database.");
            }
        }

        private static DataTable BuildDataTableForEntities<T>(IEnumerable<T> entities) where T : class
        {
            IEnumerator<Table> tableAttributeEnumerator = typeof(T).GetCustomAttributes<Table>(false).GetEnumerator();

            if (tableAttributeEnumerator.MoveNext() == false)
            {
                throw new ArgumentException("Missing table attribute.");
            }

            Table tableAttribute = tableAttributeEnumerator.Current;

            DataTable dataTable = new DataTable();
            dataTable.BeginLoadData();
            dataTable.TableName = tableAttribute.Name;

            IDictionary<string, string> columnAttributeToPropertyMapping = new Dictionary<string, string>();

            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                IEnumerable<Column> columnPropertyEnumerable = propertyInfo.GetCustomAttributes<Column>();
                IEnumerator<Column> columnPropertyEnumerator = columnPropertyEnumerable?.GetEnumerator();

                if (columnPropertyEnumerator != null && columnPropertyEnumerator.MoveNext())
                {
                    Column column = columnPropertyEnumerator.Current;

                    DataColumn dataColumn = new DataColumn(column.Name, GetPropertyCorrespondingDataType(propertyInfo));
                    dataTable.Columns.Add(dataColumn);

                    columnAttributeToPropertyMapping.Add(column.Name, propertyInfo.Name);
                }
            }

            foreach (T entity in entities)
            {
                dataTable.Rows.Add(BuildDataRow<T>(dataTable, entity, columnAttributeToPropertyMapping));
            }

            dataTable.EndLoadData();
            dataTable.AcceptChanges();

            return dataTable;
        }

        private static DataRow BuildDataRow<T>(DataTable dataTable, T entity, IDictionary<string, string> columnAttributeToPropertyMapping) where T : class
        {
            DataColumnCollection dcCollection = dataTable.Columns;
            DataRow dataRow = dataTable.NewRow();
            dataRow.BeginEdit();

            foreach(DataColumn column in dcCollection)
            {
                string propertyName = columnAttributeToPropertyMapping[column.ColumnName];

                object propertyValue = entity.GetType().GetProperty(propertyName).GetValue(entity);

                if(propertyValue == null)
                {
                    dataRow.IsNull(column);
                }
                else
                {
                    dataRow[column.ColumnName] = propertyValue;
                }
            }

            dataRow.EndEdit();
            dataRow.AcceptChanges();

            return dataRow;
        }
    }
}
