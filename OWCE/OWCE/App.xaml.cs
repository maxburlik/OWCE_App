using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Xamarin.Essentials;
using OWCE.DependencyInterfaces;
using OWCE.Pages;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OWCE
{
    public partial class App : Application
    {
        public static new App Current => Application.Current as App;
        public IOWBLE OWBLE { get; private set; }

        public bool MetricDisplay
        {
            get; set;
        }

        public bool SpeedDemon
        {
            get; set;
        }

        public bool SpeedReporting
        {
            get; set;
        }

        public int SpeedReportingBaselineTimeout
        {
            get; set;
        }

        public int SpeedReportingMinimum
        {
            get; set;
        }

        public bool BatteryPercentReporting
        {
            get; set;
        }

        public bool BatteryPercentInferredBasedOnVoltage
        {
            get; set;
        }

        public bool WheelslipReporting
        {
            get; set;
        }

        public App()
        {
            MetricDisplay = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);
            SpeedDemon = Preferences.Get("speed_demon", false);
            SpeedReporting = Preferences.Get("speed_reporting", true);
            SpeedReportingBaselineTimeout = Preferences.Get("speedreporting_baseline_timeout", 15);
            SpeedReportingMinimum = Preferences.Get("speedreporting_minimum", System.Globalization.RegionInfo.CurrentRegion.IsMetric ? 5 : 3);
            BatteryPercentReporting = Preferences.Get("batterypercent_reporting", true);
            BatteryPercentInferredBasedOnVoltage = Preferences.Get("batterypercent_inferred_voltage", false);
            WheelslipReporting = Preferences.Get("wheelslip_reporting", false);

            OWBLE = DependencyService.Get<IOWBLE>();

            if (String.IsNullOrEmpty(AppConstants.SyncfusionLicense) == false)
            {
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(AppConstants.SyncfusionLicense);
            }
            InitializeComponent();

            //MainPage = new NavigationPage(new BoardDetailsPage(new MockOWBoard()));
            //return;

            MainPage = new MainMasterDetailPage();
        }

        void ProceedToApp()
        {
            /*
            // This method works great for iOS, but on Android it flashes the screen which is annoying.

            var newPage = new NavigationPage(new BoardListPage());
            await Navigation.PushModalAsync(newPage);
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Android will crash without first popping modal.
                await Navigation.PopModalAsync(false);
            }
            ((App)Application.Current).MainPage = newPage;


            //await Navigation.PushAsync(new BoardListPage());
            //Navigation.RemovePage(this);
            */
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            AppCenter.Start($"android={AppConstants.AppCenterAndroid};ios={AppConstants.AppCenteriOS}", typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
