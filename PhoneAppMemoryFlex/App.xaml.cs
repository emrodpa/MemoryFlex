using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneAppMemoryFlex.Resources;

using Windows.Storage;
using System.Collections.Generic;

using ErrorHandlingLib_Phone;
using PlayLib_Phone;

namespace PhoneAppMemoryFlex
{
    public partial class App : Application
    {
        private ErrorHandler oErrorHandler;

        public static List<SettingsForPlay> SettingsForPlayList;

        public static PlayLevels PlayLevel;

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            oErrorHandler = new ErrorHandler();
            oErrorHandler.ErrorOcurred += oErrorHandler_ErrorOcurred;

            ErrorItem l_Error = new ErrorItem("starting app", "app");
            oErrorHandler.Items.Add(l_Error);

            SettingsForPlayList = new List<SettingsForPlay>();
            /*
            // reading the Play Level from the settings store
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            bool hasContainer = localSettings.Containers.ContainsKey("playsettings");
            bool hasSetting = false;

            if (hasContainer)
            {
                hasSetting = localSettings.Containers["playsettings"].Values.ContainsKey("playlevel");
            }

            if (hasSetting)
                PlayLevel = (PlayLevels)Int32.Parse(localSettings.Containers["playsettings"].Values["playlevel"].ToString());
            else
                PlayLevel = PlayLevels.Beginner;
            */
        }  // of App()

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

            //Frame rootFrame = Window.Current.Content as Frame;
            ;

            //if (!RootFrame.Navigate(new Uri("BasicPageError.xaml", UriKind.Relative)))
            if (!RootFrame.Navigate("BasicPageError.xaml", l_FirstParams))
            {
                throw new Exception("Failed to create error page");
            }
            
        }  // of NavigateToErrorPage()

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }  // of Application_Launching()

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }  // of Application_Activated()

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }  // of Application_Deactivated()

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }  // of Application_Closing()

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }  // of RootFrame_NavigationFailed()

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }  // of Application_UnhandledException()

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }  // of InitializeLanguage()

        public static int GetGridWidth(double p_WindowBoundsWidth)
        {
            return (int)Math.Round(5.0 * p_WindowBoundsWidth / 8.0);

        }  // of GetGridWidth()

        public static int GetGridHeight(double p_WindowBoundsHeight)
        {
            return (int)Math.Round(1.0 * p_WindowBoundsHeight - 50);

        }  // of GetGridHeight()

    }  // of class App

}  // of namespace PhoneAppMemoryFlex