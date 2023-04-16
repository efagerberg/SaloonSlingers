using System;

namespace SaloonSlingers.Core
{
    public delegate void PointsChangedHandler(Points sender, ValueChangeEvent<uint> e);
    public class Points
    {
        public uint Value
        {
            get { return _value; }
            set
            {
                uint before = _value;
                _value = Math.Clamp(value, 0, MaxValue);
                if (before == _value) return;

                OnPointsChanged?.Invoke(
                    this,
                    new ValueChangeEvent<uint>(before, _value)
                );
            }
        }
        public uint MaxValue { get; }
        public event PointsChangedHandler OnPointsChanged;

        public Points(uint initial)
        {
            MaxValue = initial;
            Value = initial;
        }

        private uint _value;
    }
}
