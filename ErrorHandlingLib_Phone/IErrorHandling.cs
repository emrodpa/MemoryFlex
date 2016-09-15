using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingLib_Phone
{
    public enum ErrorTypes { Error, Warning, Info, NoDataReturned }

    public delegate void DELErrorOcurrence(ErrorItem p_Error);

    public interface IErrorHandling
    {
        event DELErrorOcurrence ErrorOcurred;


    }  // of interface IErrorHandling

}  // of namespace ErrorHandlingLib_Phone
