using System;
namespace OWCE.Models
{
    public class IntBoardDetail : BaseBoardDetail
    {
        private int _value;
        public virtual int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public IntBoardDetail(string name) : base(name)
        {

        }
    }
}
