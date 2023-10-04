using System;

namespace SaloonSlingers.Core
{
    public delegate void PointsIncreasedHandler(Points sender, ValueChangeEvent<uint> e);
    public delegate void PointsDecreasedHandler(Points sender, ValueChangeEvent<uint> e);

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
                _value = Math.Clamp(value, 0, MaxValue);
                var e = new ValueChangeEvent<uint>(before, _value);
                if (e.Before == e.After) return;

                if (e.Before > e.After) PointsDecreased?.Invoke(this, e);
                else PointsIncreased?.Invoke(this, e);
            }
        }
        public uint MaxValue { get; }
        public event PointsIncreasedHandler PointsIncreased;
        public event PointsDecreasedHandler PointsDecreased;

        public Points(uint initial)
        {
            MaxValue = initial;
            _value = initial;
        }

        public Points(uint initial, uint max)
        {
            MaxValue = max;
            _value = initial;
        }

        private uint _value;
    }
}
