using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace cachy.UWP
{

    sealed partial class App : Application
    {

        #region constructor / destructor

        public App()
        {
            Directory.ResolvePath = ResolvePath;
            DLoggerManager.PathDelimiter =  "\\";
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        #endregion

        #region private methods

        private static Boolean ResolvePath(String token,
            out String resolvedPath)
        {
            switch (token)
            {
                case "{AppData}":
                    {
                        String appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        resolvedPath = appData;
                        return (true);
                    }
                case "{LocalVaults}":
                    {
                        String appData = String.Empty;
                        if(ResolvePath("{AppData}", out appData))
                        {
                            if (!appData.EndsWith("\\")) appData += "\\";
                            resolvedPath = appData + "LocalVaults\\";
                            return (true);
                        }
                        else
                        {
                            resolvedPath = String.Empty;
                            return (false);
                        }
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("Path token '{0}' has not been handled by the logging host.", token));
                    }
            }
        }

        #endregion

        #region base class overrides

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Xamarin.Forms.Forms.Init(e);
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }
                Window.Current.Content = rootFrame;
            }
            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            Window.Current.Activate();
        }

        #endregion

        #region object events

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        #endregion

    }
}
