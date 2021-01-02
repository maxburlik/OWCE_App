namespace OWCE
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Xamarin.Essentials;

    public class WheelslipReporting : TTSReporting
    {
        private readonly TimeSpan GapTimeReport = TimeSpan.FromSeconds(3);

        private bool _running = false;
        private DateTime nextReportTime = DateTime.Now;
        private Random _random = new Random();
        private OWBoard _board;

        public WheelslipReporting(OWBoard board, TextToSpeechProvider ttsProvider) : base(ttsProvider)
        {
            _board = board;
            TextToSpeechPriority = 2;

            Debug.WriteLine($"Wheelslip Reporting created for board {board.Name}");
        }

        public override void Start()
        {
            if (_running)
            {
                return;
            }

            System.Diagnostics.Debug.WriteLine("ACTIVATED Wheelslip detector");

            _board.Speed.SpeedChanged += Board_SpeedChanged;
        }

        public override void Stop()
        {
            if (!_running)
            {
                return;
            }

            _board.Speed.SpeedChanged -= Board_SpeedChanged;

            _running = false;

            System.Diagnostics.Debug.WriteLine("DEACTIVATED Wheelslip detector");
        }

        private void Board_SpeedChanged(object sender, EventArgs e)
        {
            //  TODO: Anomaly detection
            var currentTime = DateTime.Now;

            if (currentTime > nextReportTime)
            {
                nextReportTime = currentTime + GapTimeReport;
                OnWheelslip();
            }
        }

        private void OnWheelslip()
        {
            string text = GetNextMessage();

            var locales = TextToSpeech.GetLocalesAsync().GetAwaiter().GetResult();
            //Locale locale = locales.FirstOrDefault(x => x.Name.Contains("UK")); // Just for fun. TODO: Remove
            Locale locale = locales.FirstOrDefault(x => x.Name.Contains("Russian")); // Just for fun. TODO: Remove
            var speechOptions = new SpeechOptions() { Locale = locale };

            SpeakMessage(text, speechOptions);
        }

        private object messageListLock = new object();
        private int index = -1;
        private string GetNextMessage()
        {
            lock (messageListLock)
            {
                if (index < 0 || index == (wheelslipMessages.Count - 1))
                {
                    wheelslipMessages = wheelslipMessages.OrderBy(x => _random.Next()).ToList();
                    index = 0;
                }
                else
                {
                    ++index;
                }

                return wheelslipMessages[index];
            }
        }

        private List<string> wheelslipMessages = new List<string>()
        {
            "Don't fall!",
            "Wheelslip detected.",
            "Best of luck.",
            "Wow, that was fast.",
            "Going warp speed.",
            "Hold on to your clavicles.",
            "Are you ok?",
            "Just run it off.",
            "Thrilling!",
            "Do you need a band aid?",
            "Nice drift!",
            "The ground is coming up pretty fast.",
            "I'm not your mother.",
            "Is this all wheel drive?",
            "Have you set your emergency contacts?",
            "Who put that there?",
            "That was fun.",
            "Is traction control even on?",
            "Where did that ground come from?",
            "You're expecting too much from this unicycle.",
            "Watch where you're driving!"
        };
    }
}
