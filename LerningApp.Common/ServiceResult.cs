namespace LerningApp.Common;

public class ServiceResult
{
    public string? Message { get; set; }
    public string? Field { get; set; }
    public bool Result { get; set; }

    public static ServiceResult Success()
    {
        return new ServiceResult { Result = true};
    }

    public static ServiceResult Fail(string message, string? field = null)
    {
        return new ServiceResult { Result = false, Field = message, Message = field };
    }
    
}