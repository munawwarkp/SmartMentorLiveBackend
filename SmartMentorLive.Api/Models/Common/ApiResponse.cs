
namespace SmartMentorLive.Api.Contracts.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public string? TraceId { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message)
            => new ApiResponse<T> { Success = true, Message = message };
        public static ApiResponse<T> FailResponse(string message, List<string>? errors)
            => new ApiResponse<T> { Success = false, Message = message, Errors = errors };
    }
}
