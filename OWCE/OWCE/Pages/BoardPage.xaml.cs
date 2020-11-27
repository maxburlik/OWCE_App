using System;
using System.Collections.Generic;
using Plugin.BLE;
using Xamarin.Essentials;
using Xamarin.Forms;
using RestSharp;
using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.EventArgs;

namespace OWCE
{
    public partial class BoardPage : ContentPage
    {
        public OWBoard Board { get; internal set; }

        public string SpeedHeader
        {
            get
            {
                var unit = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric) ? "kmph" : "mph";
                return $"Speed ({unit})";
            }
        }

        public BoardPage(OWBoard board)
        {
            Board = board;

            Task.Run(async () =>
            {
                await board.SubscribeToBLE();
            });

            BindingContext = this;

            InitializeComponent();

            //CrossBluetoothLE.Current.Adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
            //CrossBluetoothLE.Current.Adapter.DeviceConnectionLost += Adapter_DeviceConnectionLost;

            NavigationPage.SetHasBackButton(this, false);
        }

        //private void Adapter_DeviceDisconnected(object sender, DeviceEventArgs e)
        //{
        //    // TODO: Impl
        //}

        //private void Adapter_DeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        //{
        //    // TODO: Impl
        //}

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (GaugeAbsolueLayout.WidthRequest != width)
            {
                GaugeAbsolueLayout.WidthRequest = width;
                GaugeAbsolueLayout.HeightRequest = width;
                GaugeAbsolueLayout.MinimumWidthRequest = width;
                GaugeAbsolueLayout.MinimumHeightRequest = width;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            DisconnectAndPop();
            return false;
        }

        bool speedReportingEnabled_ = false;
        CancellationTokenSource speedReportingCts;

        // Cancel speech if a cancellation token exists & hasn't been already requested.
        public void CancelSpeech()
        {
            if (speedReportingCts?.IsCancellationRequested ?? true)
                return;

            speedReportingCts.Cancel();
        }

        async void SpeedReporting_Clicked(object sender, System.EventArgs e)
        {
            if (!speedReportingEnabled_)
            {
                speedReportingEnabled_ = true;
                System.Diagnostics.Debug.WriteLine("ACTIVATED Speed Reporting");
                string voiceMessage = "Activated speed reporting.";
                CancelSpeech();
                speedReportingCts = new CancellationTokenSource();
                Task speechTask = TextToSpeech.SpeakAsync(voiceMessage, speedReportingCts.Token);

                await Task.WhenAny(Task.FromResult(0), speechTask); // To avoid runaway task warning

                Board.SpeedChanged += Board_SpeedChanged;
            }
        }

        int lastReportedSpeed = 0;
        DateTime nextHighSpeedTime = DateTime.Now;
        DateTime nextSlowSpeedTime = DateTime.Now;

        const int TimeInSecsIncrementSlow = 10;
        const int TimeInSecsIncrementFast = 2;
        const int MinSpeedKm = 5;
        const int MinSpeedMi = 3;

        private void Board_SpeedChanged(object sender, OWBoard.SpeedChangedEventArgs e)
        {
            var currentTime = DateTime.Now;
            int newSpeed = (int)e.speedValue;
            bool isMetric = Preferences.Get("metric_display", System.Globalization.RegionInfo.CurrentRegion.IsMetric);
            string voiceMessage;

            if ((isMetric && newSpeed > MinSpeedKm)
                || (!isMetric && newSpeed > MinSpeedMi))
            {
                string unit = isMetric ? "kph" : "mph";

                if (currentTime > nextSlowSpeedTime)
                {
                    lastReportedSpeed = newSpeed;
                    nextSlowSpeedTime = currentTime + TimeSpan.FromSeconds(TimeInSecsIncrementSlow);
                    nextHighSpeedTime = currentTime + TimeSpan.FromSeconds(TimeInSecsIncrementFast);

                    System.Diagnostics.Debug.WriteLine($"New slow speed {newSpeed}");
                    voiceMessage = $"Baseline {newSpeed} {unit}";
                    CancelSpeech();
                    speedReportingCts = new CancellationTokenSource();
                    TextToSpeech.SpeakAsync(voiceMessage, speedReportingCts.Token);
                }
                else if (currentTime > nextHighSpeedTime)
                {
                    if (newSpeed > lastReportedSpeed)
                    {
                        lastReportedSpeed = newSpeed;
                        nextSlowSpeedTime = currentTime + TimeSpan.FromSeconds(TimeInSecsIncrementSlow);
                        nextHighSpeedTime = currentTime + TimeSpan.FromSeconds(TimeInSecsIncrementFast);

                        System.Diagnostics.Debug.WriteLine($"New fast speed {newSpeed}");
                        voiceMessage = $"Faster {newSpeed} {unit}";
                        CancelSpeech();
                        speedReportingCts = new CancellationTokenSource();
                        TextToSpeech.SpeakAsync(voiceMessage, speedReportingCts.Token);
                    }
                }
            }
        }
        async void Disconnect_Clicked(object sender, System.EventArgs e)
        {
            await DisconnectAndPop();
        }

        private async Task DisconnectAndPop()
        {
            if (speedReportingEnabled_)
            {
                Board.SpeedChanged -= Board_SpeedChanged;
                System.Diagnostics.Debug.WriteLine("DEACTIVATED Speed Reporting");
                CancelSpeech();
                speedReportingCts = new CancellationTokenSource();
                string voiceMessage = "Deactivated speed reporting.";
                TextToSpeech.SpeakAsync(voiceMessage, speedReportingCts.Token);
            }

            await Board.Disconnect();
            await Navigation.PopAsync();
        }

        private bool _isLogging = false;

        /*
        private async void LogData_Clicked(object sender, System.EventArgs e)
        {
            if (_isLogging)
            {
                LogDataButton.Text = "Start Logging Data";
                _isLogging = false;
                string zip = await Board.StopLogging();
                Hud.Dismiss();
                Hud.Show("Uploading");
                var client = new RestClient("https://owce.app");

                var request = new RestRequest("/upload_log.php", Method.POST);
                request.AddParameter("serial", Board.SerialNumber);
                request.AddParameter("ride_start", DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                try
                {
                    var response = await client.ExecuteTaskAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                        HttpWebRequest httpRequest = WebRequest.Create(response.Content) as HttpWebRequest;
                        httpRequest.Method = "PUT";
                        using (Stream dataStream = httpRequest.GetRequestStream())
                        {
                            var buffer = new byte[8000];
                            using (FileStream fileStream = new FileStream(zip, FileMode.Open, FileAccess.Read))
                            {
                                int bytesRead = 0;
                                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    dataStream.Write(buffer, 0, bytesRead);
                                }
                            }
                        }
                        HttpWebResponse uploadResponse = httpRequest.GetResponse() as HttpWebResponse;


                        Hud.Dismiss();
                        if (uploadResponse.StatusCode == HttpStatusCode.OK)
                        {
                            await DisplayAlert("Success", "Log file sucessfully uploaded.", "Ok");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Could not upload log at this time.", "Ok");
                        }
                    }
                    else
                    {
                        Hud.Dismiss();
                        await DisplayAlert("Error", "Could not upload log at this time.", "Ok");
                    }
                }
                catch (Exception err)
                {
                    Hud.Dismiss();
                    await DisplayAlert("Error", err.Message, "Ok");
                    // Log
                }
            }
            else
            {
                LogDataButton.Text = "Stop Logging Data";
                _isLogging = true;
                await Board.StartLogging();

            }

        }
        */

    }
}
