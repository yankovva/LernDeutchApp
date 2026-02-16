namespace LerningApp.Common;

public class ServiceResultT<T> : ServiceResult
{
    public T? Data { get; set; }
    
    public static ServiceResultT<T> Success(T data)
    {
        return new ServiceResultT<T> { Result = true, Data = data };
    }

    public static ServiceResultT<T> Fail(string message)
    {
        return new ServiceResultT<T> { Result = false, Message = message};
    }
}