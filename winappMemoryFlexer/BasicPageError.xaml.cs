using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;

using ErrorHandlingLib;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace winappMemoryFlexer
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class BasicPageError : winappMemoryFlexer.Common.LayoutAwarePage
    {
        FirstParams<Dictionary<string, string>> m_FirstParams;
        Type m_ReturnPage;

        public BasicPageError()
        {
            this.InitializeComponent();

            m_ReturnPage = typeof(BasicPageHome);

        }  // of BasicPageError()

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            m_FirstParams = (FirstParams<Dictionary<string, string>>)navigationParameter;

            if (m_FirstParams.oErrorHandler.Items.Count > 0)
            {
                ErrorItem l_ErrorItem = m_FirstParams.oErrorHandler.Items.Last();

                if (l_ErrorItem.PreviousPage != null)
                {
                    m_ReturnPage = l_ErrorItem.PreviousPage;
                }  // of if

                switch (l_ErrorItem.Type)
                {
                    case ErrorTypes.Error:
                        tbkErrorHeader.Text = "There was an error";
                        break;

                    case ErrorTypes.Warning:
                        tbkErrorHeader.Text = "Operation failed:";
                        break;

                    case ErrorTypes.Info:
                        tbkErrorHeader.Text = "Operation failed:";
                        break;

                    default:
                        break;

                }  // of switch

                tbkErrorMessage.Text = l_ErrorItem.Description;

                tbkErrorLocation.Text = string.Format("at {0} on {1}", l_ErrorItem.Location, l_ErrorItem.TimeStamp.ToString("MM/dd/yyyy hh:mm:ss"));
            }
            else
            {
                tbkErrorMessage.Text = "unknown error";
            }  // of if/else

        }  // of LoadState()

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }  // of SaveState()

        protected override void GoBack(object sender, RoutedEventArgs e)
        {

            // clean the page's concerned keys
            //m_FirstParams.Param.Clear();

            this.Frame.Navigate(m_ReturnPage, m_FirstParams);

        }  // of GoBack()

    }  // of class BasicPageError()

}  // of namespace winappMemoryFlexer
