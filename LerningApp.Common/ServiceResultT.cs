namespace LerningApp.Common;
using static LerningApp.Common.Enums;

public class ServiceResultT<T> : ServiceResult
{
    public T? Data { get; set; }
    
    public static ServiceResultT<T> Success(T data)
    {
        return new ServiceResultT<T> { Result = true, Data = data };
    }

    public static ServiceResultT<T> Fail(string message, ServiceErrorType errorType, string? field = null)
    {
        return new ServiceResultT<T> { Result = false, ErrorType = errorType, Field = field,Message = message};
    }
}