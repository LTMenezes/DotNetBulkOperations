using System;

namespace DotNetBulkOperations.Attributes
{
    /// <summary>
    /// Attribute for defining a table in a entity to perform a bulk operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class Table : Attribute
    {
        /// <summary>
        /// Name of the table.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Table attribute constructor.
        /// </summary>
        /// <param name="name"></param>
        public Table(string name)
        {
            this.Name = name;
        }
    }
}
