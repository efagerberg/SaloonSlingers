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
            private set
            {
                uint before = _value;

                if (value == uint.MaxValue && before == 0) _value = 0;
                else _value = Math.Clamp(value, 0, MaxValue);

                var e = new ValueChangeEvent<uint>(before, _value);
                if (e.Before == e.After) return;

                if (e.Before > e.After) PointsDecreased?.Invoke(this, e);
                else PointsIncreased?.Invoke(this, e);
            }
        }
        public uint MaxValue { get; }
        public uint InitialValue { get; }
        public event PointsIncreasedHandler PointsIncreased;
        public event PointsDecreasedHandler PointsDecreased;

        public void Reset()
        {
            Value = InitialValue;
        }

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

        public void Decrement()
        {
            Decrease(1);
        }

        public void Decrease(uint amount)
        {
            // Check for underflow
            if (Value >= amount)
                Value -= amount;
            else Value = 0;
        }


        public void Increment() => Value++;

        public void Increase(uint amount) => Value += amount;

        private uint _value;
    }
}
