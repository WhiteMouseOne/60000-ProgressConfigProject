namespace Progress.Model.Dto.Login
{
    public class ApiResponseData
    {
        public int code { get; set; } = 200;
        public object? data { get; set; }
        public string message { get; set; } = "";
    }
}
