namespace lonefire.Models.UtilModels
{
    public class SystemErrorModel
    {
        public int StatusCode { get; set; }

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}