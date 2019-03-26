namespace OrdersAPI.Wrapper
{
    public class ProviderResponseWrapper
    {
        public string ResponseMessage { get; set; }
        public HTTPResponseCodes ResponseHTMLType { get; set; }
    }
    
    public enum HTTPResponseCodes
    {
        HTTP_OK_RESPONSE = 200,
        HTTP_BAD_REQUEST = 400,
        HTTP_NOT_FOUND = 404,
        HTTP_SERVER_FAILURE_RESPONSE = 500,
    };
}
