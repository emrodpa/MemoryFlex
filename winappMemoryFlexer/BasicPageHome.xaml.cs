using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.Storage;

using ErrorHandlingLib;
using PlayLib;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace winappMemoryFlexer
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class BasicPageHome : winappMemoryFlexer.Common.LayoutAwarePage
    {
        FirstParams<List<SettingsForPlay>> m_FirstParamsFromPreviousPage;
        FirstParams<FactorySequences> m_FirstParamsForNextPage;

        public BasicPageHome()
        {
            this.InitializeComponent();

        }  // of BasicPageHome()

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            PlayLevels m_PlayLevel;

            // reading the Play Level from the settings store
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            bool hasContainer = localSettings.Containers.ContainsKey("playsettings");
            bool hasSetting = false;

            if (hasContainer)
            {
                hasSetting = localSettings.Containers["playsettings"].Values.ContainsKey("playlevel");
            }

            if (hasSetting)
                m_PlayLevel = (PlayLevels)Int32.Parse(localSettings.Containers["playsettings"].Values["playlevel"].ToString());
            else
                m_PlayLevel = PlayLevels.Beginner;

            //await btnStart.WhenClickedAsync();
            m_FirstParamsFromPreviousPage = (FirstParams<List<SettingsForPlay>>)navigationParameter;

            await PrepareFirstParamsForNextPageAsync(m_PlayLevel);

            //tbkPlayingLevel.Text = string.Format("the current game level is at the \"{0}\" level of difficulty.",m_PlayLevel.ToString());
            //tbkHintPlayingLevel.Text = " hint: use the Settings Charm to change the level (you may need to restart the app)";
            //tbkPlayingLevel.Text = string.Format("hint: use the Settings Charm to change the level of difficulty of the game", m_PlayLevel.ToString());

            btnStart.IsEnabled = true;

            listviewPlayLevels.SelectedIndex = (int)m_PlayLevel;
            listviewPlayLevels.SelectionChanged += listviewPlayLevels_SelectionChanged;

        }  // of LoadState()

        private async Task PrepareFirstParamsForNextPageAsync(PlayLevels p_PlayLevel)
        {
            List<SettingsForPlay> l_SettingsForPlayList = m_FirstParamsFromPreviousPage.Param;

            SettingsForPlay l_SettingsForPlay = l_SettingsForPlayList.Find(p => p.PlayLevel.Equals(p_PlayLevel));

            StorageFolder l_ImageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("FigureImages");

            FactorySequences l_FacSequences = new FactorySequences(l_ImageFolder, l_SettingsForPlay);

            await l_FacSequences.PopulateFigureListAsync();

            m_FirstParamsForNextPage = new FirstParams<FactorySequences>(m_FirstParamsFromPreviousPage.oErrorHandler);
            m_FirstParamsForNextPage.Param = l_FacSequences;

            // set the UI data
            tbkNumberFiguresInSequence.Text = string.Format("{0}", l_SettingsForPlay.NumberFiguresInSequence);

            tbkSequenceDuration.Text = string.Format("{0} s", l_SettingsForPlay.IntervalBetweenFigures * l_SettingsForPlay.NumberFiguresInSequence /1000.0);

            tbkNumberFiguresTotal.Text = string.Format("{0}", l_SettingsForPlay.NumberOfRows * l_SettingsForPlay.NumberOfColumns);

            tbkMaxPLayChances.Text = string.Format("{0}", l_SettingsForPlay.oGoal.MaxPLayChances);

            tbkMaximumPlayTimeInSeconds.Text = string.Format("{0} s", l_SettingsForPlay.oGoal.MaxPlayingTime);
        
        }  // of PrepareFirstParamsForNextPageAsync()

        private async void listviewPlayLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int l_SelectedLevel = ((Windows.UI.Xaml.Controls.ListViewBase)(sender)).SelectedIndex;

            // Setting in a container
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Windows.Storage.ApplicationDataContainer container = localSettings.CreateContainer("playsettings", Windows.Storage.ApplicationDataCreateDisposition.Always);

            localSettings.Containers["playsettings"].Values["playlevel"] = l_SelectedLevel;

            await PrepareFirstParamsForNextPageAsync((PlayLevels)l_SelectedLevel);

        }  // of listviewPlayLevels_SelectionChanged()

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {

        }  // of SaveState()

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BasicPageDisplaySequence), m_FirstParamsForNextPage);

        }  // of btnStart_Click()

    }  // of class BasicPageHome

}  // of namespace winappMemoryFlexer
