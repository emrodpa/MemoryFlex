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

using Windows.UI.ApplicationSettings;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace winappMemoryFlexer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPageSettings : Page
    {
        Rect _windowBounds;
        double _settingsWidth = 346;
        Popup _settingsPopup;

        public BlankPageSettings()
        {
            this.InitializeComponent();

            _windowBounds = Window.Current.Bounds;

            Window.Current.SizeChanged += OnWindowSizeChanged;

            SettingsPane.GetForCurrentView().CommandsRequested += BlankPage_CommandsRequested;

        }  // of BlankPageSettings()

        void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _windowBounds = Window.Current.Bounds;

        }  // of OnWindowSizeChanged()

        void BlankPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            

        }  // of BlankPage_CommandsRequested()

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }

        }  // of OnWindowActivated()

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;

        }  // of OnPopupClosed()

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


        }  // of OnNavigatedTo()

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SettingsPane.Show();

        }  // of Button_Click_1()

        private void HandleSizeChange(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            _settingsWidth = Convert.ToInt32(rb.Content);

        }  // of HandleSizeChange()

    }  // of class BlankPageSettings

}  // of namespace winappMemoryFlexer
