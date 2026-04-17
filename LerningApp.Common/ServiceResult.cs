namespace LerningApp.Common;
using static LerningApp.Common.Enums;
public class ServiceResult
{
    public string? Message { get; set; }
    public string? Field { get; set; }
    public bool Result { get; set; }
    public ServiceErrorType? ErrorType { get; set; }

    public static ServiceResult Success()
    {
        return new ServiceResult { Result = true};
    }

    public static ServiceResult Fail(string message, ServiceErrorType errorType, string? field = null)
    {
        return new ServiceResult { Result = false, Field = field, ErrorType = errorType, Message = message };
    }
    
}