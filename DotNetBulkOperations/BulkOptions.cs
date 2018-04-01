
namespace DotNetBulkOperations
{
    /// <summary>
    /// Options for bulk operations.
    /// </summary>
    public class BulkOptions
    {
        /// <summary>
        /// Timeout in seconds.
        /// </summary>
        public int TimeoutInSeconds { get; set; }

        /// <summary>
        /// Default constructor. It will fill the options with default values.
        /// </summary>
        public BulkOptions()
        {
            this.TimeoutInSeconds = 1000;
        }
    }
}
