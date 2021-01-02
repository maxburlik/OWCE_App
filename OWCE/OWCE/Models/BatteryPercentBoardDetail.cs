using System;
namespace OWCE.Models
{
    public class BatteryPercentBoardDetail : IntBoardDetail
    {
        public BatteryPercentBoardDetail(string name) : base(name)
        {
        }

        public override int Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (base.Value != value)
                {
                    base.Value = value;
                    var changedArgs = new BatteryPercentChangedEventArgs();
                    changedArgs.batteryPercentValue = value;
                    OnBatteryPercentChanged(changedArgs);
                }
            }
        }

        public class BatteryPercentChangedEventArgs : EventArgs
        {
            public int batteryPercentValue;
        }

        public delegate void BatteryPercentChangedEventHandler(object sender, BatteryPercentChangedEventArgs e);

        public event BatteryPercentChangedEventHandler BatteryPercentChanged;

        private void OnBatteryPercentChanged(BatteryPercentChangedEventArgs e)
        {
            if (BatteryPercentChanged != null)
            {
                BatteryPercentChanged(this, e);
            }
        }
    }
}
