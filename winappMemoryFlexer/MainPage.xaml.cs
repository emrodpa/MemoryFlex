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

using Windows.Storage;

using PlayLib;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace winappMemoryFlexer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }  // of MainPage()

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            StorageFolder l_ImageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("FigureImages");

            SettingsForPlay l_oSettingsForPlay = new SettingsForPlay(PlayLevels.Beginner, 3, 1000, 3, 5000,5,3,660,800);

            FactorySequences l_FacSequences = new FactorySequences(l_ImageFolder, l_oSettingsForPlay);

            await l_FacSequences.PopulateFigureListAsync();

            Sequence l_Sequence1 = await l_FacSequences.CreateSequenceAsync();
            Sequence l_Sequence2 = await l_FacSequences.CreateSequenceAsync();
            Sequence l_Sequence3 = await l_FacSequences.CreateSequenceAsync();


        }  // of OnNavigatedTo()

    }  // of class MainPage

}  // of namespace winappMemoryFlexer
