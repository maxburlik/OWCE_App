using System;
namespace OWCE.Models
{
    public class SpeedBoardDetail : FloatBoardDetail
    {
        public SpeedBoardDetail(string name) : base(name)
        {
        }

        public override float Value
        {
            get { return base.Value; }
            set
            {
                if (!base.Value.AlmostEqualTo(value))
                {
                    base.Value = value;
                    var speedArgs = new SpeedChangedEventArgs();
                    speedArgs.speedValue = value;
                    OnSpeedChanged(speedArgs);
                }
            }
        }

        public class SpeedChangedEventArgs : EventArgs
        {
            public float speedValue;
        }

        public delegate void SpeedChangedEventHandler(object sender, SpeedChangedEventArgs e);

        public event SpeedChangedEventHandler SpeedChanged;

        private void OnSpeedChanged(SpeedChangedEventArgs e)
        {
            if (SpeedChanged != null)
            {
                SpeedChanged(this, e);
            }
        }
    }
}
