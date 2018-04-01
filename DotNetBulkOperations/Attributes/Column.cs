using System;

namespace DotNetBulkOperations.Attributes
{
    /// <summary>
    /// Attribute for defining a column in a entity to perform a bulk operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class Column : Attribute
    {
        /// <summary>
        /// Name of the column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column Attribute constructor.
        /// </summary>
        /// <param name="name"></param>
        public Column(string name)
        {
            this.Name = name;
        }
    }
}
