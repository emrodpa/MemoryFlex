using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Data.Xml;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.ApplicationSettings;

using ErrorHandlingLib;
using PlayLib;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace winappMemoryFlexer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private ErrorHandler oErrorHandler;

        private List<SettingsForPlay> m_SettingsForPlayList;

        public PlayLevels m_PlayLevel;

        Rect _windowBounds;
        double _settingsWidth = 346;
        Popup _settingsPopup;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            oErrorHandler = new ErrorHandler();
            oErrorHandler.ErrorOcurred += oErrorHandler_ErrorOcurred;

            ErrorItem l_Error = new ErrorItem("starting app", "app");
            oErrorHandler.Items.Add(l_Error);

            m_SettingsForPlayList = new List<SettingsForPlay>();

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

        }  // of App()

        void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _windowBounds = Window.Current.Bounds;

        }  // of OnWindowSizeChanged()

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }

        }  // of OnWindowActivated()

        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand cmd = new SettingsCommand("PlayLevel", "Play Levels", (x) =>
            {
                _settingsPopup = new Popup();
                _settingsPopup.Closed += OnPopupClosed;
                Window.Current.Activated += OnWindowActivated;
                _settingsPopup.IsLightDismissEnabled = true;
                _settingsPopup.Width = _settingsWidth;
                _settingsPopup.Height = _windowBounds.Height;

                ucSimpleSettingsNarrow mypane = new ucSimpleSettingsNarrow((int)m_PlayLevel);
                mypane.Width = _settingsWidth;
                mypane.Height = _windowBounds.Height;

                _settingsPopup.Child = mypane;
                _settingsPopup.SetValue(Canvas.LeftProperty, _windowBounds.Width - _settingsWidth);
                _settingsPopup.SetValue(Canvas.TopProperty, 0);
                _settingsPopup.IsOpen = true;
            });

            args.Request.ApplicationCommands.Add(cmd);

        }  // of App_CommandsRequested()

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;

            // reading the Play Level from the settings store
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            m_PlayLevel = (PlayLevels)Int32.Parse(localSettings.Containers["playsettings"].Values["playlevel"].ToString());

        }  // of OnPopupClosed()

        void oErrorHandler_ErrorOcurred(ErrorItem p_Error)
        {
            oErrorHandler.Items.Add(p_Error);

            NavigateToErrorPage(p_Error.FirstParamsKeyValueHolder);

        }  // of oErrorHandler_ErrorOcurred()

        void NavigateToErrorPage(Dictionary<string, string> p_FirstParamsKeyValueHolder)
        {
            FirstParams<Dictionary<string, string>> l_FirstParams = new FirstParams<Dictionary<string, string>>(oErrorHandler);

            if (p_FirstParamsKeyValueHolder != null && p_FirstParamsKeyValueHolder.Count > 0)
            {
                l_FirstParams.Param = p_FirstParamsKeyValueHolder;
            }
            else
            {
                Dictionary<string, string> l_DicForQueryString = new Dictionary<string, string>();
                l_DicForQueryString.Add("myname", "Emilio");
                l_FirstParams.Param = l_DicForQueryString;

            }  // of if/else

            Frame rootFrame = Window.Current.Content as Frame;

            if (!rootFrame.Navigate(typeof(BasicPageError), l_FirstParams))
            {
                throw new Exception("Failed to create error page");
            }

        }  // of NavigateToErrorPage()

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;

            _windowBounds = Window.Current.Bounds;

            Window.Current.SizeChanged += OnWindowSizeChanged;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                // get play settings from config file
                StorageFolder l_StorageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                StorageFile l_StorageFile = await l_StorageFolder.GetFileAsync("PlayConfigurations.xml");

                //int l_GridWidth = (int)Math.Round(5.0 * Window.Current.Bounds.Width / 8.0);
                int l_GridWidth = App.GetGridWidth(Window.Current.Bounds.Width);
                //int l_GridHeight = (int)Math.Round(Window.Current.Bounds.Height - 140);
                int l_GridHeight = App.GetGridHeight(Window.Current.Bounds.Height);

                m_SettingsForPlayList = await SettingsForPlay.GetListFromFileAsync(l_StorageFile, l_GridWidth, l_GridHeight);

                FirstParams<List<SettingsForPlay>> l_FirstParams = new FirstParams<List<SettingsForPlay>>(oErrorHandler);
                l_FirstParams.Param = m_SettingsForPlayList;

                if (!rootFrame.Navigate(typeof(BasicPageHome), l_FirstParams))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();

        }  // of OnLaunched()

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();

        }  // of OnSuspending()

        public static int GetGridWidth(double p_WindowBoundsWidth)
        {
            return (int)Math.Round( 5.0 * p_WindowBoundsWidth / 8.0 );

        }  // of GetGridWidth()

        public static int GetGridHeight(double p_WindowBoundsHeight)
        {
            return (int)Math.Round( 1.0 * p_WindowBoundsHeight - 140 );

        }  // of GetGridHeight()

    }  // of class App

}  // of namespace winappMemoryFlexer
