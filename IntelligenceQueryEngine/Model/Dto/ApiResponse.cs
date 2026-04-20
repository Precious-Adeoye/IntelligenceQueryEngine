namespace IntelligenceQueryEngine.Model.Dto
{
    public class ApiResponse<T>
    {
        public string Status { get; set; } = "success";
        public int Page { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public T? Data { get; set; }
    }

    public class ErrorResponse
    {
        public string Status { get; set; } = "error";
        public string Message { get; set; } = string.Empty;
    }
}
