using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.ApplicationSettings;


// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace winappMemoryFlexer
{
    public sealed partial class ucSimpleSettingsNarrow : UserControl
    {
        public int SelectedLevel { get; set; }

        public ucSimpleSettingsNarrow()
        {
            this.InitializeComponent();

        }  // of ucSimpleSettingsNarrow()

        public ucSimpleSettingsNarrow(int p_SelectedLevel)
            : this()
        {
            SelectedLevel = p_SelectedLevel;

        }  // of ucSimpleSettingsNarrow()

        private void MySettingsBackClicked(object sender, RoutedEventArgs e)
        {
            if (this.Parent.GetType() == typeof(Popup))
            {
                ((Popup)this.Parent).IsOpen = false;
            }

            SettingsPane.Show();

        }  // of MySettingsBackClicked()

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            listviewPlayLevels.SelectedIndex = SelectedLevel;
            listviewPlayLevels.SelectionChanged += listviewPlayLevels_SelectionChanged;

        }  // of UserControl_Loaded()

        private void listviewPlayLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                SelectedLevel = ((Windows.UI.Xaml.Controls.ListViewBase)(sender)).SelectedIndex;

                // Setting in a container
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

                Windows.Storage.ApplicationDataContainer container = localSettings.CreateContainer("playsettings", Windows.Storage.ApplicationDataCreateDisposition.Always);

                localSettings.Containers["playsettings"].Values["playlevel"] = SelectedLevel;

        }  // of listviewPlayLevels_SelectionChanged()


    }  // of class ucSimpleSettingsNarrow

}  // of namespace winappMemoryFlexer
