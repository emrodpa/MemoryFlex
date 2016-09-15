using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using System.Threading.Tasks;
using Windows.Storage;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

using ErrorHandlingLib_Phone;
using PlayLib_Phone;

namespace PhoneAppMemoryFlex
{
    public partial class BasicPageHome : PhoneApplicationPage
    {
        SettingsForPlay m_SettingsForPlay;

        FactorySequences m_FirstParamsForNextPage;

        //This is a variable for the help popup.
        private Popup m_NotificationPopup = new Popup();

        public BasicPageHome()
        {
            this.InitializeComponent();

        }  // of BasicPageHome()

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppSettings l_AppSettings = new AppSettings();

            PlayLevels l_PlayLevel = (PlayLevels)l_AppSettings.GetValueOrDefault<int>("PlayLevel", 0);

            // to force the page (landscape) orientation to the popup
            if (!LayoutRoot.Children.Contains(m_NotificationPopup))
                LayoutRoot.Children.Add(m_NotificationPopup);

            int l_GridWidth = App.GetGridWidth(this.ActualWidth);
            int l_GridHeight = App.GetGridHeight(this.ActualHeight);

            App.SettingsForPlayList = SettingsForPlay.GetListFromFileAsync("PlayConfigurations.xml", l_GridWidth, l_GridHeight);

            m_SettingsForPlay = App.SettingsForPlayList.Find(s => s.PlayLevel.Equals(l_PlayLevel));

            await PrepareFirstParamsForNextPageAsync(m_SettingsForPlay.PlayLevel);

            btnStart.IsEnabled = true;

            // no needed when using two-way binding
            //listviewPlayLevels.SelectedIndex = (int)m_SettingsForPlay.PlayLevel;
            listviewPlayLevels.SelectionChanged += listviewPlayLevels_SelectionChanged;

        }    // of PhoneApplicationPage_Loaded()

        private async Task PrepareFirstParamsForNextPageAsync(PlayLevels p_PlayLevel)
        {
            StorageFolder l_ImageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("FigureImages");

            FactorySequences l_FacSequences = new FactorySequences(l_ImageFolder, m_SettingsForPlay);

            // set the UI data
            tbkNumberFiguresInSequence.Text = string.Format("{0}", m_SettingsForPlay.NumberFiguresInSequence);

            tbkSequenceDuration.Text = string.Format("{0} s", m_SettingsForPlay.IntervalBetweenFigures * m_SettingsForPlay.NumberFiguresInSequence / 1000.0);

            tbkNumberFiguresTotal.Text = string.Format("{0}", m_SettingsForPlay.NumberOfRows * m_SettingsForPlay.NumberOfColumns);

            tbkMaxPLayChances.Text = string.Format("{0}", m_SettingsForPlay.oGoal.MaxPLayChances);

            tbkMaximumPlayTimeInSeconds.Text = string.Format("{0} s", m_SettingsForPlay.oGoal.MaxPlayingTime);

            await l_FacSequences.PopulateFigureListAsync();

            m_FirstParamsForNextPage = l_FacSequences;

        }  // of PrepareFirstParamsForNextPageAsync()

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate("/BasicPageDisplaySequence.xaml", m_FirstParamsForNextPage);

        }  // of btnStart_Click()

        private async void listviewPlayLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnStart.IsEnabled = false;

            int l_SelectedLevel = ((System.Windows.Controls.ListBox)(sender)).SelectedIndex;

            m_SettingsForPlay = App.SettingsForPlayList.Find(s => s.PlayLevel.Equals((PlayLevels)l_SelectedLevel));


            await PrepareFirstParamsForNextPageAsync(m_SettingsForPlay.PlayLevel);

            btnStart.IsEnabled = true;

        }  // of listviewPlayLevels_SelectionChanged()

        /// <summary>
        /// Click event handler for the help button.
        ///This will create a popup/message box for help and add content to the popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayNotificationPopup(string p_Text)
        {
            String l_Text = p_Text;

            //Stack panel with a black background.
            StackPanel panelHelp = new StackPanel();
            panelHelp.Background = new SolidColorBrush(Colors.Black);
            panelHelp.Width = 400;
            panelHelp.Height = 200;
            panelHelp.Orientation = System.Windows.Controls.Orientation.Vertical;

            //Create a white border.
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(7.0);

            //Create a close button to exit popup.
            Button close = new Button();
            close.Content = "Close";
            close.Width = 120;
            close.Margin = new Thickness(5.0);
            close.Click += new RoutedEventHandler(close_Click);
            close.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

            //Create helper text
            TextBlock textblockHelp = new TextBlock();
            textblockHelp.FontSize = 24;
            textblockHelp.Foreground = new SolidColorBrush(Colors.White);
            textblockHelp.TextWrapping = TextWrapping.Wrap;
            textblockHelp.Text = l_Text;
            textblockHelp.Margin = new Thickness(5.0);
            textblockHelp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            //Add controls to stack panel
            panelHelp.Children.Add(textblockHelp);
            panelHelp.Children.Add(close);
            border.Child = panelHelp;

            // Set the Child property of Popup to the border 
            // that contains a stackpanel, textblock and button.
            m_NotificationPopup.Child = border;

            // Set where the popup will show up on the screen.   
            m_NotificationPopup.VerticalOffset = 100;
            m_NotificationPopup.HorizontalOffset = 85;

            // Open the popup.
            m_NotificationPopup.IsOpen = true;

        }  // of DisplayNotificationPopup()

        /// <summary>
        /// Click event handler for the close button on the help popup.
        /// Closes the poupup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void close_Click(object sender, RoutedEventArgs e)
        {
            m_NotificationPopup.IsOpen = false;

        }  // of close_Click()

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }  // of while

        }  // of PhoneApplicationPage_BackKeyPress()

    }  // of class BasicPageHome

}  // of namespace PhoneAppMemoryFlex