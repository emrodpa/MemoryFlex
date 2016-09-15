
namespace ErrorHandlingLib
{
    public enum ErrorTypes { Error, Warning, Info, NoDataReturned}

    public delegate void DELErrorOcurrence(ErrorItem p_Error);

    public interface IErrorHandling
    {
        event DELErrorOcurrence ErrorOcurred;


    }  // of interface IErrorHandling

}  // of namespace ErrorHandlingLib
