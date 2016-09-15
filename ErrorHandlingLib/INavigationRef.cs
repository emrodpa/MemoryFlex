using System;

namespace ErrorHandlingLib
{
    public interface INavigationRef
    {
        Type CurrentPage { get; }

        Type PreviousPage { get; }

    }  // of interface INavigationRef

}  // of namespace ErrorHandlingLib
