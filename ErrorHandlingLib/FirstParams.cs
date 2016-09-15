using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingLib
{
    public partial class FirstParams<T>
    {
        public ErrorHandler oErrorHandler { get; set; }

        public T Param { get; set; }

        public FirstParams(ErrorHandler p_ErrorHandler)
        {
            oErrorHandler = p_ErrorHandler;

        }  // of FirstParams()

    }  // of class FirstParams

}  // of namespace ErrorHandlingLib
