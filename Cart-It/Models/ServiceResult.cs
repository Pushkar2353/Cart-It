namespace Cart_It.Models
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; }

        // Constructor for success
        public ServiceResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = null;
        }

        // Constructor for success with data
        public ServiceResult(bool isSuccess, object data)
        {
            IsSuccess = isSuccess;
            Message = isSuccess ? "Operation was successful." : "Operation failed.";
            Data = data;
        }

        // Constructor for failure
        public ServiceResult(bool isSuccess, string message, object data = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }
    }
}
