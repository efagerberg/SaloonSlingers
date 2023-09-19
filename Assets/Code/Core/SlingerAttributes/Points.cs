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
                // Value could be negative technically, but the actual _value variable needs to be
                // positive.
                _value = (uint)Math.Clamp((int)value, 0, MaxValue);
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
            _value = initial;
        }

        private uint _value;
    }
}
