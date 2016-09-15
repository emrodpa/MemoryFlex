using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Data.Xml;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.ApplicationSettings;

using ErrorHandlingLib;
using PlayLib;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace winappMemoryFlexer
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class BasicPageDisplaySequence : winappMemoryFlexer.Common.LayoutAwarePage
    {
        Play m_Play;

        FirstParams<FactorySequences> m_FirstParamsFromPreviousPage;
        FirstParams<Play> m_FirstParamsForNextPage;

        Sequence m_Sequence;

        DispatcherTimer m_DispatcherTimer;

        public BasicPageDisplaySequence()
        {
            this.InitializeComponent();

        }  // of BasicPageDisplaySequence()

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            try
            {
                m_FirstParamsFromPreviousPage = (FirstParams<FactorySequences>)navigationParameter;

                FactorySequences l_FacSequences = m_FirstParamsFromPreviousPage.Param;

                m_Sequence = await l_FacSequences.CreateSequenceAsync();

                await PlaySequenceAsync(l_FacSequences.FigureList);

                m_Play = new Play(m_Sequence, l_FacSequences);

                m_FirstParamsForNextPage = new FirstParams<Play>(m_FirstParamsFromPreviousPage.oErrorHandler);
                m_FirstParamsForNextPage.Param = m_Play;

                this.Frame.Navigate(typeof(BasicPageTestScreen), m_FirstParamsForNextPage);            
            }
            catch(Exception ex)
            {
                m_FirstParamsFromPreviousPage.oErrorHandler.FireErrorOcurred(ex.Message, "LoadState");
            }  // of try/catch

        }  // of LoadState()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_FigureList">all figures available in the play</param>
        /// <returns></returns>
        public async Task PlaySequenceAsync(List<Figure> p_FigureList)
        {
            Uri l_Uri;
            foreach(Guid id in m_Sequence.FigureIDList)
            {
                l_Uri = new Uri(this.BaseUri, string.Format("FigureImages/{0}", p_FigureList.Find(f=>f.Id.Equals(id)).ImageFilename));

                // start playing the sequence
                await PostAndWaitAsync(l_Uri, m_Sequence.Interval);

            }  // of loop over figures in sequence

        }  // of PlaySequence()

        public async Task PostAndWaitAsync(Uri p_Uri, int p_Interval)
        {
            m_DispatcherTimer = new DispatcherTimer();
            m_DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, p_Interval);

            var tcs = new TaskCompletionSource<object>();
            EventHandler<object> lambda = (s, e) => tcs.TrySetResult(null);

            try
            {
                m_DispatcherTimer.Tick += lambda;

                m_DispatcherTimer.Start();

                BitmapImage l_BitmapImage = new BitmapImage(p_Uri);
                imgHolder.Source = l_BitmapImage;

                await tcs.Task;
            }
            finally
            {
                m_DispatcherTimer.Stop();
                m_DispatcherTimer.Tick -= lambda;
            }  // of try/finally
        
        }  // of PostAndWaitAsync()

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {


        }  // of SaveState()

        private async void GoBack(object sender, RoutedEventArgs e)
        {
            m_DispatcherTimer.Stop();

            // get play settings from config file
            StorageFolder l_StorageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile l_StorageFile = await l_StorageFolder.GetFileAsync("PlayConfigurations.xml");

            /*
            int l_GridWidth = (int)Math.Round(Window.Current.Bounds.Width - 150);
            int l_GridHeight = (int)Math.Round(3.0 * Window.Current.Bounds.Height / 5.0);
            */

            //int l_GridWidth = (int)Math.Round(5.0 * Window.Current.Bounds.Width / 8.0);
            int l_GridWidth = App.GetGridWidth(Window.Current.Bounds.Width);
            //int l_GridHeight = (int)Math.Round(Window.Current.Bounds.Height - 140);
            int l_GridHeight = App.GetGridHeight(Window.Current.Bounds.Height);

            List<SettingsForPlay> l_SettingsForPlayList = await SettingsForPlay.GetListFromFileAsync(l_StorageFile, l_GridWidth, l_GridHeight);

            FirstParams<List<SettingsForPlay>> l_FirstParams = new FirstParams<List<SettingsForPlay>>(m_FirstParamsFromPreviousPage.oErrorHandler);
            l_FirstParams.Param = l_SettingsForPlayList;

            Frame rootFrame = Window.Current.Content as Frame;

            if (!rootFrame.Navigate(typeof(BasicPageHome), l_FirstParams))
                {
                    throw new Exception("Failed to create initial page");
                }

        }  // of GoBack()

    }  // of class BasicPageDisplaySequence

}  // of namespace winappMemoryFlexer
