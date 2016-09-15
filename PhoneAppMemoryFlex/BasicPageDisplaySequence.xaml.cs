using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System.Windows.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Phone.Media;
using System.Windows.Media;

using ErrorHandlingLib_Phone;
using PlayLib_Phone;

namespace PhoneAppMemoryFlex
{
    public partial class BasicPageDisplaySequence : PhoneApplicationPage
    {
        Play m_Play;

        FactorySequences m_FirstParamsFromPreviousPage;
        Play m_FirstParamsForNextPage;

        Sequence m_Sequence;

        DispatcherTimer m_DispatcherTimer;


        public BasicPageDisplaySequence()
        {
            InitializeComponent();

        }  // of BasicPageDisplaySequence()

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                m_FirstParamsFromPreviousPage = (FactorySequences)NavigationService.GetLastNavigationData();

                FactorySequences l_FacSequences = m_FirstParamsFromPreviousPage;

                m_Sequence = await l_FacSequences.CreateSequenceAsync();

                await PlaySequenceAsync(l_FacSequences.FigureList);

                m_Play = new Play(m_Sequence, l_FacSequences);

                m_FirstParamsForNextPage = m_Play;

                NavigationService.Navigate("/BasicPageTestScreen.xaml", m_FirstParamsForNextPage);
            }
            catch(Exception ex)
            {
                m_DispatcherTimer.Stop();

                NavigationService.Navigate(new Uri("/BasicPageHome.xaml", UriKind.Relative));
            
            }  // of try/catch

        }  // of PhoneApplicationPage_Loaded()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_FigureList">all figures available in the play</param>
        /// <returns></returns>
        public async Task PlaySequenceAsync(List<Figure> p_FigureList)
        {
            //Uri l_Uri;
            string l_ImagePath;
            foreach (Guid id in m_Sequence.FigureIDList)
            {
                //l_Uri = new Uri(, string.Format("FigureImages/{0}", p_FigureList.Find(f => f.Id.Equals(id)).ImageFilename));
                l_ImagePath = string.Format("FigureImages/{0}", p_FigureList.Find(f => f.Id.Equals(id)).ImageFilename);
                // start playing the sequence
                await PostAndWaitAsync(l_ImagePath, m_Sequence.Interval);

            }  // of loop over figures in sequence

        }  // of PlaySequence()

        // public async Task PostAndWaitAsync(Uri p_Uri, int p_Interval)
        public async Task PostAndWaitAsync(string p_ImagePath, int p_Interval)
        {
            m_DispatcherTimer = new DispatcherTimer();
            m_DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, p_Interval);

            var tcs = new TaskCompletionSource<object>();
            EventHandler lambda = (s, e) => tcs.TrySetResult(null);

            try
            {
                m_DispatcherTimer.Tick += lambda;

                m_DispatcherTimer.Start();

                // BitmapImage l_BitmapImage = new BitmapImage(p_Uri);
                ImageSourceConverter l_ImageSourceConverter = new ImageSourceConverter();
                imgHolder.Source = (ImageSource)l_ImageSourceConverter.ConvertFromString(p_ImagePath);

                await tcs.Task;
            }
            finally
            {
                m_DispatcherTimer.Stop();
                m_DispatcherTimer.Tick -= lambda;
            }  // of try/finally

        }  // of PostAndWaitAsync()

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_DispatcherTimer.Stop();

            NavigationService.Navigate(new Uri("/BasicPageHome.xaml", UriKind.Relative));

        }  // of PhoneApplicationPage_BackKeyPress()

    }  // of class BasicPageDisplaySequence

}  // of namespace PhoneAppMemoryFlex