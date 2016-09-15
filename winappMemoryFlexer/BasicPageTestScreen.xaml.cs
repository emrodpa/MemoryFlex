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
    public sealed partial class BasicPageTestScreen : winappMemoryFlexer.Common.LayoutAwarePage
    {
        Play m_Play;

        FirstParams<Play> m_FirstParamsFromPreviousPage;
        //FirstParams<Play> m_FirstParamsForNextPage;

        DispatcherTimer m_DispatcherTimer;
        DispatcherTimer m_DispatcherTimerForShowSequence;

        public long m_TickCounter;

        public BasicPageTestScreen()
        {
            this.InitializeComponent();

            //Window.Current.SizeChanged += Current_SizeChanged;

            m_DispatcherTimer = new DispatcherTimer();
            m_DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            m_DispatcherTimer.Tick += m_DispatcherTimer_Tick;

            m_TickCounter = 0;

        }  // of BasicPageTestScreen()

        async void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (m_DispatcherTimer != null)
                m_DispatcherTimer.Stop();

            await NavigateToHomePageAsync();


        }  // of Current_SizeChanged()

        void m_DispatcherTimer_Tick(object sender, object e)
        {
            m_TickCounter++;
            tbkTimeLeft.Text = string.Format("{0} s", m_TickCounter);

            if (m_TickCounter >= m_Play.FacSequences.oSettingsForPlay.oGoal.MaxPlayingTime)
            {
                m_Play_PlayFinished(PlayFinishingTypes.TimeExpired, DateTime.Now, m_Play.NumberOfCorrectGuessess, m_Play.WrongGuesses.Count());
            
            }  // of if time is up

        }  // of m_DispatcherTimer_Tick()

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
            m_FirstParamsFromPreviousPage = (FirstParams<Play>)navigationParameter;

            m_Play = m_FirstParamsFromPreviousPage.Param;

            m_Play.GuessHappened += m_Play_GuessHappened;
            m_Play.PlayFinished += m_Play_PlayFinished;

            try
            {
                // prepare the grid of images

                int l_GridWidth = m_Play.FacSequences.oSettingsForPlay.GridWidth;
                int l_GridHeight = m_Play.FacSequences.oSettingsForPlay.GridHeight;

                // make the image grid like an square (as per the smaller rectangle size)
                int l_RowHeight = (m_Play.FacSequences.oSettingsForPlay.GridWidth < m_Play.FacSequences.oSettingsForPlay.GridHeight)
                    ? (int)Math.Round(1.0 * m_Play.FacSequences.oSettingsForPlay.GridWidth / m_Play.FacSequences.oSettingsForPlay.NumberOfRows)
                    : (int)Math.Round(1.0 * m_Play.FacSequences.oSettingsForPlay.GridHeight / m_Play.FacSequences.oSettingsForPlay.NumberOfRows);

                //m_Play.FacSequences.oSettingsForPlay.GridWidth = (int)Math.Round(Window.Current.Bounds.Width - 150);
                //m_Play.FacSequences.oSettingsForPlay.GridHeight = (int)Math.Round(3.0 * Window.Current.Bounds.Height / 5.0);

                SolidColorBrush l_BrushGrid = new SolidColorBrush(Windows.UI.Colors.Aqua);

                //gridImages.Background = l_BrushGrid;

                RowDefinition l_RowDefinition;
                for (int i = 0; i < m_Play.FacSequences.oSettingsForPlay.NumberOfRows; i++)
                {
                    l_RowDefinition = new RowDefinition();
                    l_RowDefinition.Height = new GridLength(l_RowHeight);
                    gridImages.RowDefinitions.Add(l_RowDefinition);
                    l_RowDefinition = null;

                }  // of loop over rows

                for (int j = 0; j < m_Play.FacSequences.oSettingsForPlay.NumberOfColumns; j++)
                {
                    gridImages.ColumnDefinitions.Add(new ColumnDefinition());

                }  // of loop over columns

                Image l_Img;
                BitmapImage l_BitmapImage;
                for (int l_RowPos = 0; l_RowPos < m_Play.FacSequences.oSettingsForPlay.NumberOfRows; l_RowPos++)
                {
                    for (int l_ColumnPos = 0; l_ColumnPos < m_Play.FacSequences.oSettingsForPlay.NumberOfColumns; l_ColumnPos++)
                    {
                        l_Img = new Image();
                        l_Img.Margin = new Thickness(5.0, 0.0, 5.0, 0.0);
                        l_BitmapImage = new BitmapImage(new Uri(this.BaseUri, string.Format("FigureImages/{0}", m_Play.FacSequences.FigureList[l_RowPos * m_Play.FacSequences.oSettingsForPlay.NumberOfColumns + l_ColumnPos].ImageFilename)));

                        //natural px width of image source
                        // don't need to set Height, system maintains aspect ratio, and calculates the other
                        // dimension, so long as one dimension measurement is provided

                        if (m_Play.FacSequences.oSettingsForPlay.DeltaWidth <= m_Play.FacSequences.oSettingsForPlay.DeltaHeight)
                            l_Img.Width = l_BitmapImage.DecodePixelWidth = m_Play.FacSequences.oSettingsForPlay.DeltaWidth - 5;
                        else
                            l_Img.Height = l_BitmapImage.DecodePixelHeight = m_Play.FacSequences.oSettingsForPlay.DeltaHeight - 5;

                        l_Img.Source = l_BitmapImage;

                        l_Img.Name = m_Play.FacSequences.FigureList[l_RowPos * m_Play.FacSequences.oSettingsForPlay.NumberOfColumns + l_ColumnPos].Id.ToString();

                        Grid.SetRow(l_Img, l_RowPos);
                        Grid.SetColumn(l_Img, l_ColumnPos);

                        m_Play.FacSequences.FigureList[l_RowPos * m_Play.FacSequences.oSettingsForPlay.NumberOfColumns + l_ColumnPos].RowPosition = l_RowPos;
                        m_Play.FacSequences.FigureList[l_RowPos * m_Play.FacSequences.oSettingsForPlay.NumberOfColumns + l_ColumnPos].ColumnPosition = l_ColumnPos;

                        l_Img.Tapped += l_Img_Tapped;

                        gridImages.Children.Add(l_Img);

                        l_Img = null;
                        l_BitmapImage = null;

                    }  // of loop over rows

                }  // of loop over rows


                tbkChancesLeft.Text = m_Play.FacSequences.oSettingsForPlay.oGoal.MaxPLayChances.ToString();
                //tbkTimeLeft.Text = string.Format("{0} s", m_Play.FacSequences.oSettingsForPlay.oGoal.MaxPlayingTime.ToString());

                tbkPlayStatus.Text = "Playing";
                m_DispatcherTimer.Start();            
            }
            catch(Exception ex)
            {
                m_FirstParamsFromPreviousPage.oErrorHandler.FireErrorOcurred(string.Format("Error when preparing grid: {0}", ex.Message), "LoadState in BasicPageTestScreen");
            }  // of try/catch

         }  // of LoadState()

        private async void m_Play_PlayFinished(PlayFinishingTypes p_PlayFinishingType, DateTime p_DateTime, int p_NumberOfRightGuesses, int p_NumberOfWrongGuesses)
        {
            if (m_DispatcherTimer != null)
                m_DispatcherTimer.Stop();

            m_Play.GuessHappened -= m_Play_GuessHappened;
            m_Play.PlayFinished -= m_Play_PlayFinished;

            btnStopPlaying.IsEnabled = false;
            btnStopPlaying.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            btnNewPlay.IsEnabled = true;
            btnNewPlay.Visibility = Windows.UI.Xaml.Visibility.Visible;
            tbkGuess.Visibility = Windows.UI.Xaml.Visibility.Visible;

            SolidColorBrush l_BrushRed = new SolidColorBrush(Windows.UI.Colors.Red);
            SolidColorBrush l_BrushYellow = new SolidColorBrush(Windows.UI.Colors.Yellow);
            SolidColorBrush l_BrushViolet = new SolidColorBrush(Windows.UI.Colors.Violet);

            switch (p_PlayFinishingType)
            {
                case PlayFinishingTypes.NoChancesLeft:
                    tbkPlayStatus.Text = "No Chances Left";
                    tbkPlayStatus.Foreground = l_BrushRed;
                    tbkGuess.Text = "Game Lost";
                    tbkGuess.Foreground = l_BrushRed;
                    break;

                case PlayFinishingTypes.StoppedByUser:
                    tbkPlayStatus.Text = "Game Stopped";
                    tbkPlayStatus.Foreground = l_BrushRed;
                    tbkGuess.Text = "";
                    tbkGuess.Foreground = l_BrushYellow;
                    break;

                case PlayFinishingTypes.Success:
                    tbkPlayStatus.Text = "Success";
                    tbkPlayStatus.Foreground = l_BrushViolet;
                    tbkGuess.Text = "Congratulations!";
                    tbkGuess.Foreground = l_BrushViolet;
                    break;

                case PlayFinishingTypes.TimeExpired:
                    tbkPlayStatus.Text = "Time is Up";
                    tbkPlayStatus.Foreground = l_BrushRed;
                    tbkGuess.Text = "Game Lost";
                    tbkGuess.Foreground = l_BrushRed;
                    break;

                default:
                    tbkPlayStatus.Text = "Finished";
                    tbkPlayStatus.Foreground = l_BrushRed;
                    tbkGuess.Text = "";
                    tbkGuess.Foreground = l_BrushRed;
                    break;
            
            }  // of switch

            await PlaySequenceAsync(m_Play.FacSequences.FigureList);

            btnPlaySequence.Visibility = Windows.UI.Xaml.Visibility.Visible;
            btnPlaySequence.IsEnabled = true;

        }  // of m_Play_PlayFinished()

        async void m_Play_GuessHappened(Figure p_Figure, int p_GuessCount, bool p_IsCorrect)
        {
            try       
            {
                Image l_Image = (Image)gridImages.FindName(p_Figure.Id.ToString());

                if (p_IsCorrect)
                {
                    SolidColorBrush l_BrushGold = new SolidColorBrush(Windows.UI.Colors.Gold);
                    tbkGuess.Foreground = l_BrushGold;
                    tbkGuess.Text = "Right Guess";
                    tbkGuess.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    // remove image from grid
                    gridImages.Children.Remove(l_Image);
                    // add border in position of the image
                    Border l_Border = new Border();
                    l_Border.BorderThickness = new Thickness(10, 10, 10, 10);
                    SolidColorBrush l_BrushCell = new SolidColorBrush(Windows.UI.Colors.Gold);
                    l_Border.BorderBrush = l_BrushCell;
                    Grid.SetRow(l_Border, p_Figure.RowPosition);
                    Grid.SetColumn(l_Border, p_Figure.ColumnPosition);
                    // add image as children to border
                    l_Border.Child = l_Image;
                    // add border as children of grid
                    gridImages.Children.Add(l_Border);

                    //tbkGuess.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    SolidColorBrush l_BrushRed = new SolidColorBrush(Windows.UI.Colors.Red);
                    tbkGuess.Foreground = l_BrushRed;
                    tbkGuess.Text = "Wrong Guess";
                    tbkGuess.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    int l_ChancesLeft = m_Play.FacSequences.oSettingsForPlay.oGoal.MaxPLayChances - p_GuessCount;

                    tbkChancesLeft.Text = l_ChancesLeft.ToString();

                    // remove image from grid
                    gridImages.Children.Remove(l_Image);
                    // add border in position of the image
                    Border l_Border = new Border();
                    l_Border.BorderThickness = new Thickness(10, 10, 10, 10);
                    SolidColorBrush l_BrushCell = new SolidColorBrush(Windows.UI.Colors.Red);
                    l_Border.BorderBrush = l_BrushCell;
                    Grid.SetRow(l_Border, p_Figure.RowPosition);
                    Grid.SetColumn(l_Border, p_Figure.ColumnPosition);
                    // add image as children to border
                    l_Border.Child = l_Image;
                    // add border as children of grid
                    gridImages.Children.Add(l_Border);

                    await Task.Delay(500);

                    // remove image as children of border
                    l_Border.Child = null;

                    // remove border from grid
                    gridImages.Children.Remove(l_Border);
                    // add image in position of the border
                    Grid.SetRow(l_Image, p_Figure.RowPosition);
                    Grid.SetColumn(l_Image, p_Figure.ColumnPosition);
                    // add image as children of grid
                    gridImages.Children.Add(l_Image);

                    //tbkGuess.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                    l_Image.Tapped += l_Img_Tapped;

                }  // of if/else is a correct guess
            
            }
            catch(Exception ex)
            {
                m_FirstParamsFromPreviousPage.oErrorHandler.FireErrorOcurred(string.Format("Error when setting border: {0}", ex.Message), "m_Play_GuessHappened in BasicPageTestScreen");

            }  // of try/catch

        }  // of m_Play_GuessHappened()

        void l_Img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                Image l_Img = (Image)sender;

                l_Img.Tapped -= l_Img_Tapped;

                Figure l_Figure = m_Play.FacSequences.FigureList.Find(f => f.Id.Equals(Guid.Parse(l_Img.Name)));

                if (l_Figure == null)
                    throw new Exception("this shouldn't happen");

                int l_RowPos = l_Figure.RowPosition;
                int l_ColumnPos = l_Figure.ColumnPosition;

                // find if tapped figure is same as next unmarked figure in sequence

                m_Play.MakeGuess(l_Figure.Id);            
            }
            catch(Exception ex)
            {
                m_FirstParamsFromPreviousPage.oErrorHandler.FireErrorOcurred(string.Format("Error: {0}", ex.Message), "l_Img_Tapped in BasicPageTestScreen");
            
            }  // of try/catch

        }  // of l_Img_Tapped()

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
            await NavigateToHomePageAsync();

            /*  this code goes back to the display sequence page
            if (m_DispatcherTimer != null)
                m_DispatcherTimer.Stop();

            Frame rootFrame = Window.Current.Content as Frame;

            // get play settings from config file
            StorageFolder l_StorageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile l_StorageFile = await l_StorageFolder.GetFileAsync("PlayConfigurations.xml");

            List<SettingsForPlay> l_SettingsForPlayList = await SettingsForPlay.GetListFromFileAsync(l_StorageFile);

            // reading the Play Level from the settings store
            PlayLevels l_PlayLevel;

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            bool hasContainer = localSettings.Containers.ContainsKey("playsettings");
            bool hasSetting = false;

            if (hasContainer)
            {
                hasSetting = localSettings.Containers["playsettings"].Values.ContainsKey("playlevel");
            }

            if (hasSetting)
                l_PlayLevel = (PlayLevels)Int32.Parse(localSettings.Containers["playsettings"].Values["playlevel"].ToString());
            else
                l_PlayLevel = PlayLevels.Beginner;

            StorageFolder l_ImageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("FigureImages");

            FactorySequences l_FacSequences = new FactorySequences(l_ImageFolder, l_SettingsForPlayList.Find(s=>s.PlayLevel.Equals(l_PlayLevel)));
            await l_FacSequences.PopulateFigureListAsync();

            FirstParams<FactorySequences> l_FirstParams = new FirstParams<FactorySequences>(m_FirstParamsFromPreviousPage.oErrorHandler);
            l_FirstParams.Param = l_FacSequences;

            if (!rootFrame.Navigate(typeof(BasicPageDisplaySequence), l_FirstParams))
            {
                throw new Exception("Failed to create initial page");
            }
            */
        }  // of GoBack()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_FigureList">all figures available in the play</param>
        /// <returns></returns>
        public async Task PlaySequenceAsync(List<Figure> p_FigureList)
        {
            Uri l_Uri;
            foreach (Guid id in m_Play.oSequence.FigureIDList)
            {
                l_Uri = new Uri(this.BaseUri, string.Format("FigureImages/{0}", p_FigureList.Find(f => f.Id.Equals(id)).ImageFilename));

                // start playing the sequence
                await PostAndWaitAsync(l_Uri, m_Play.oSequence.Interval);

            }  // of loop over figures in sequence

        }  // of PlaySequence()

        public async Task PostAndWaitAsync(Uri p_Uri, int p_Interval)
        {
            m_DispatcherTimerForShowSequence = new DispatcherTimer();
            m_DispatcherTimerForShowSequence.Interval = new TimeSpan(0, 0, 0, 0, p_Interval);

            var tcs = new TaskCompletionSource<object>();
            EventHandler<object> lambda = (s, e) => tcs.TrySetResult(null);

            try
            {
                m_DispatcherTimerForShowSequence.Tick += lambda;

                m_DispatcherTimerForShowSequence.Start();

                BitmapImage l_BitmapImage = new BitmapImage(p_Uri);
                imgHolder.Source = l_BitmapImage;

                await tcs.Task;
            }
            finally
            {
                m_DispatcherTimerForShowSequence.Stop();
                m_DispatcherTimerForShowSequence.Tick -= lambda;
            }  // of try/finally

        }  // of PostAndWaitAsync()

        private void btnStopPlaying_Click(object sender, RoutedEventArgs e)
        {
            if (m_DispatcherTimer != null)
                m_DispatcherTimer.Stop();

            m_Play_PlayFinished(PlayFinishingTypes.StoppedByUser, DateTime.Now, m_Play.NumberOfCorrectGuessess, m_Play.WrongGuesses.Count());

        }  // of btnStopPlaying_Click()

        private async void btnPlaySequence_Click(object sender, RoutedEventArgs e)
        {
            btnPlaySequence.IsEnabled = false;

            await PlaySequenceAsync(m_Play.FacSequences.FigureList);

            btnPlaySequence.IsEnabled = true;

        }  // of btnPlaySequence_Click()

        private async void btnNewPlay_Click(object sender, RoutedEventArgs e)
        {
            await NavigateToHomePageAsync();

        }  // of btnNewPlay_Click()

        protected async Task NavigateToHomePageAsync()
        {
            m_DispatcherTimer.Stop();

            // get play settings from config file
            StorageFolder l_StorageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile l_StorageFile = await l_StorageFolder.GetFileAsync("PlayConfigurations.xml");

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

        }  // of NavigateToHomePageAsync()

    }  // of class BasicPageTestScreen

}  // namespace winappMemoryFlexer
