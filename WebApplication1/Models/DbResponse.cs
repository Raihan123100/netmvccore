namespace WebApplication1.Models
{
    public class DbResponse
    {
        /// <summary>
        /// Status code indicating success or failure
        /// 0 = Success
        /// Non-zero = Error (with specific error codes)
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Descriptive message about the operation result
        /// </summary>
        public string message { get; set; }
    }
    }
