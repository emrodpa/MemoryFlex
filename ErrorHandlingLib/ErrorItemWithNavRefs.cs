using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ErrorHandlingLib
{
    /// <summary>
    /// extends class ErrorItem with
    /// interface INavigationRef
    /// </summary>
    public partial class ErrorItemWithNavRefs : ErrorItem , INavigationRef
    {
        public Type CurrentPage { get; protected set; }

        public Type PreviousPage { get; protected set; }

        public ErrorItemWithNavRefs(string p_Description, string p_Location, DateTime p_TimeStamp, ErrorTypes p_Type, Type p_CurrentPage, Type p_PreviousPage)
            : base(p_Description, p_Location, p_TimeStamp, p_Type)
        {
            CurrentPage = p_CurrentPage;
            PreviousPage = p_PreviousPage;

        }  // of ErrorItemWithNavRefs()

        public ErrorItemWithNavRefs(string p_Description, string p_Location, ErrorTypes p_Type, Type p_CurrentPage, Type p_PreviousPage)
            : this(p_Description, p_Location, DateTime.Now, p_Type, p_CurrentPage, p_PreviousPage)
        {

        }  // of overloaded ErrorItemWithNavRefs()

    }  // of class ErrorItemWithNavRefs

}  // of namespace ErrorHandlingLib
