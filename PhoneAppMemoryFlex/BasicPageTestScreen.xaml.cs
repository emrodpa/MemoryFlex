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
    public partial class BasicPageTestScreen : PhoneApplicationPage
    {
        Play m_Play;

        Play m_FirstParamsFromPreviousPage;
        //FirstParams<Play> m_FirstParamsForNextPage;

        DispatcherTimer m_DispatcherTimer;
        DispatcherTimer m_DispatcherTimerForShowSequence;

        public long m_TickCounter;

        public BasicPageTestScreen()
        {
            InitializeComponent();

            m_DispatcherTimer = new DispatcherTimer();
            m_DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            m_DispatcherTimer.Tick += m_DispatcherTimer_Tick;

            m_TickCounter = 0;

        }  // of BasicPageTestScreen()

        void m_DispatcherTimer_Tick(object sender, object e)
        {
            m_TickCounter++;
            tbkTimeLeft.Text = string.Format("{0} s", m_TickCounter);

            if (m_TickCounter >= m_Play.FacSequences.oSettingsForPlay.oGoal.MaxPlayingTime)
            {
                m_Play_PlayFinished(PlayFinishingTypes.TimeExpired, DateTime.Now, m_Play.NumberOfCorrectGuessess, m_Play.WrongGuesses.Count());

            }  // of if time is up

        }  // of m_DispatcherTimer_Tick()

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                m_FirstParamsFromPreviousPage = (Play)NavigationService.GetLastNavigationData(); ;

                m_Play = m_FirstParamsFromPreviousPage;

                m_Play.GuessHappened += m_Play_GuessHappened;
                m_Play.PlayFinished += m_Play_PlayFinished;

                // prepare the grid of images

                int l_GridWidth = m_Play.FacSequences.oSettingsForPlay.GridWidth;
                int l_GridHeight = m_Play.FacSequences.oSettingsForPlay.GridHeight;

                // make the image grid like an square (as per the smaller rectangle size)
                int l_RowHeight = (m_Play.FacSequences.oSettingsForPlay.GridWidth < m_Play.FacSequences.oSettingsForPlay.GridHeight)
                    ? (int)Math.Round(1.0 * m_Play.FacSequences.oSettingsForPlay.GridWidth / m_Play.FacSequences.oSettingsForPlay.NumberOfRows)
                    : (int)Math.Round(1.0 * m_Play.FacSequences.oSettingsForPlay.GridHeight / m_Play.FacSequences.oSettingsForPlay.NumberOfRows);

                //m_Play.FacSequences.oSettingsForPlay.GridWidth = (int)Math.Round(Window.Current.Bounds.Width - 150);
                //m_Play.FacSequences.oSettingsForPlay.GridHeight = (int)Math.Round(3.0 * Window.Current.Bounds.Height / 5.0);

                SolidColorBrush l_BrushGrid = new SolidColorBrush(System.Windows.Media.Colors.Cyan);

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

                ImageSourceConverter l_ImageSourceConverter = new ImageSourceConverter();
                Image l_Img;
                string l_ImagePath;
                ImageSource l_ImageSource;
                for (int l_RowPos = 0; l_RowPos < m_Play.FacSequences.oSettingsForPlay.NumberOfRows; l_RowPos++)
                {
                    for (int l_ColumnPos = 0; l_ColumnPos < m_Play.FacSequences.oSettingsForPlay.NumberOfColumns; l_ColumnPos++)
                    {
                        l_Img = new Image();
                        l_Img.Margin = new Thickness(5.0, 0.0, 5.0, 0.0);
                        l_ImagePath = string.Format("FigureImages/{0}", m_Play.FacSequences.FigureList[l_RowPos * m_Play.FacSequences.oSettingsForPlay.NumberOfColumns + l_ColumnPos].ImageFilename);
                        l_ImageSource = (ImageSource)l_ImageSourceConverter.ConvertFromString(l_ImagePath);

                        /*
                        //natural px width of image source
                        // don't need to set Height, system maintains aspect ratio, and calculates the other
                        // dimension, so long as one dimension measurement is provided

                        if (m_Play.FacSequences.oSettingsForPlay.DeltaWidth <= m_Play.FacSequences.oSettingsForPlay.DeltaHeight)
                            l_Img.Width = l_ImageSource.DecodePixelWidth = m_Play.FacSequences.oSettingsForPlay.DeltaWidth - 5;
                        else
                            l_Img.Height = l_ImageSource.DecodePixelHeight = m_Play.FacSequences.oSettingsForPlay.DeltaHeight - 5;
                        /*/

                        l_Img.Source = l_ImageSource;

                        l_Img.Name = m_Play.FacSequences.FigureList[l_RowPos * m_Play.FacSequences.oSettingsForPlay.NumberOfColumns + l_ColumnPos].Id.ToString();

                        Grid.SetRow(l_Img, l_RowPos);
                        Grid.SetColumn(l_Img, l_ColumnPos);

                        m_Play.FacSequences.FigureList[l_RowPos * m_Play.FacSequences.oSettingsForPlay.NumberOfColumns + l_ColumnPos].RowPosition = l_RowPos;
                        m_Play.FacSequences.FigureList[l_RowPos * m_Play.FacSequences.oSettingsForPlay.NumberOfColumns + l_ColumnPos].ColumnPosition = l_ColumnPos;

                        l_Img.Tap += l_Img_Tapped;

                        gridImages.Children.Add(l_Img);

                        l_Img = null;
                        l_ImageSource = null;
                        l_ImagePath = string.Empty;

                    }  // of loop over rows

                }  // of loop over rows


                tbkChancesLeft.Text = m_Play.FacSequences.oSettingsForPlay.oGoal.MaxPLayChances.ToString();
                //tbkTimeLeft.Text = string.Format("{0} s", m_Play.FacSequences.oSettingsForPlay.oGoal.MaxPlayingTime.ToString());

                tbkPlayStatus.Text = "Playing";
                m_DispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                //throw ex;
                //m_FirstParamsFromPreviousPage.oErrorHandler.FireErrorOcurred(string.Format("Error when preparing grid: {0}", ex.Message), "LoadState in BasicPageTestScreen");

                if (m_DispatcherTimer != null && m_DispatcherTimer.IsEnabled)
                    m_DispatcherTimer.Stop();

                NavigationService.Navigate(new Uri("/BasicPageHome.xaml", UriKind.Relative));

            }  // of try/catch

        }  // of PhoneApplicationPage_Loaded()

        private async void m_Play_PlayFinished(PlayFinishingTypes p_PlayFinishingType, DateTime p_DateTime, int p_NumberOfRightGuesses, int p_NumberOfWrongGuesses)
        {
            if (m_DispatcherTimer != null)
            {
                m_DispatcherTimer.Stop();
                m_DispatcherTimer.Tick -= m_DispatcherTimer_Tick;
            }  // of if

            m_Play.GuessHappened -= m_Play_GuessHappened;
            m_Play.PlayFinished -= m_Play_PlayFinished;

            btnStopPlaying.IsEnabled = false;
            btnStopPlaying.Visibility = System.Windows.Visibility.Collapsed;
            btnNewPlay.IsEnabled = true;
            btnNewPlay.Visibility = System.Windows.Visibility.Visible;
            tbkGuess.Visibility = System.Windows.Visibility.Visible;

            SolidColorBrush l_BrushRed = new SolidColorBrush(System.Windows.Media.Colors.Red);
            SolidColorBrush l_BrushYellow = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
            SolidColorBrush l_BrushViolet = new SolidColorBrush(System.Windows.Media.Colors.Purple);

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

            btnPlaySequence.Visibility = System.Windows.Visibility.Visible;
            btnPlaySequence.IsEnabled = true;

        }  // of m_Play_PlayFinished()

        async void m_Play_GuessHappened(Figure p_Figure, int p_GuessCount, bool p_IsCorrect)
        {
            try
            {
                Image l_Image = (Image)gridImages.FindName(p_Figure.Id.ToString());

                if (p_IsCorrect)
                {
                    SolidColorBrush l_BrushGold = new SolidColorBrush(System.Windows.Media.Colors.Orange);
                    tbkGuess.Foreground = l_BrushGold;
                    tbkGuess.Text = "Right Guess";
                    tbkGuess.Visibility = System.Windows.Visibility.Visible;

                    // remove image from grid
                    gridImages.Children.Remove(l_Image);
                    // add border in position of the image
                    Border l_Border = new Border();
                    l_Border.BorderThickness = new Thickness(10, 10, 10, 10);
                    SolidColorBrush l_BrushCell = new SolidColorBrush(System.Windows.Media.Colors.Orange);
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
                    SolidColorBrush l_BrushRed = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    tbkGuess.Foreground = l_BrushRed;
                    tbkGuess.Text = "Wrong Guess";
                    tbkGuess.Visibility = System.Windows.Visibility.Visible;

                    int l_ChancesLeft = m_Play.FacSequences.oSettingsForPlay.oGoal.MaxPLayChances - p_GuessCount;

                    tbkChancesLeft.Text = l_ChancesLeft.ToString();

                    // remove image from grid
                    gridImages.Children.Remove(l_Image);
                    // add border in position of the image
                    Border l_Border = new Border();
                    l_Border.BorderThickness = new Thickness(10, 10, 10, 10);
                    SolidColorBrush l_BrushCell = new SolidColorBrush(System.Windows.Media.Colors.Red);
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

                    l_Image.Tap += l_Img_Tapped;

                }  // of if/else is a correct guess

            }
            catch (Exception ex)
            {
                throw ex;
                // m_FirstParamsFromPreviousPage.oErrorHandler.FireErrorOcurred(string.Format("Error when setting border: {0}", ex.Message), "m_Play_GuessHappened in BasicPageTestScreen");

            }  // of try/catch

        }  // of m_Play_GuessHappened()

        void l_Img_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                Image l_Img = (Image)sender;

                l_Img.Tap -= l_Img_Tapped;

                Figure l_Figure = m_Play.FacSequences.FigureList.Find(f => f.Id.Equals(Guid.Parse(l_Img.Name)));

                if (l_Figure == null)
                    throw new Exception("this shouldn't happen");

                int l_RowPos = l_Figure.RowPosition;
                int l_ColumnPos = l_Figure.ColumnPosition;

                // find if tapped figure is same as next unmarked figure in sequence

                m_Play.MakeGuess(l_Figure.Id);
            }
            catch (Exception ex)
            {
                throw ex;
               // m_FirstParamsFromPreviousPage.oErrorHandler.FireErrorOcurred(string.Format("Error: {0}", ex.Message), "l_Img_Tapped in BasicPageTestScreen");

            }  // of try/catch

        }  // of l_Img_Tapped()

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_FigureList">all figures available in the play</param>
        /// <returns></returns>
        public async Task PlaySequenceAsync(List<Figure> p_FigureList)
        {
            string l_ImagePath;
            foreach (Guid id in m_Play.oSequence.FigureIDList)
            {
                l_ImagePath = string.Format("FigureImages/{0}", p_FigureList.Find(f => f.Id.Equals(id)).ImageFilename);
                await PostAndWaitAsync(l_ImagePath, m_Play.oSequence.Interval);

            }  // of loop over figures in sequence

        }  // of PlaySequence()

        public async Task PostAndWaitAsync(string p_ImagePath, int p_Interval)
        {
            m_DispatcherTimerForShowSequence = new DispatcherTimer();
            m_DispatcherTimerForShowSequence.Interval = new TimeSpan(0, 0, 0, 0, p_Interval);

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

            int l_GridWidth = App.GetGridWidth(this.ActualWidth);
            int l_GridHeight = App.GetGridHeight(this.ActualHeight);

            List<SettingsForPlay> l_SettingsForPlayList = SettingsForPlay.GetListFromFileAsync("PlayConfigurations.xml", l_GridWidth, l_GridHeight);

            NavigationService.Navigate("/BasicPageHome.xaml", l_SettingsForPlayList);

        }  // of NavigateToHomePageAsync()

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_DispatcherTimer.Stop();

            NavigationService.Navigate(new Uri("/BasicPageHome.xaml", UriKind.Relative));

        }  // of PhoneApplicationPage_BackKeyPress()

    }  // of class BasicPageTestScreen

}  // of namespace PhoneAppMemoryFlex