using System;

namespace SaloonSlingers.Core
{
    public delegate void PointsIncreasedHandler(HitPoints sender, ValueChangeEvent<uint> e);
    public delegate void PointsDecreasedHandler(HitPoints sender, ValueChangeEvent<uint> e);

    [Serializable]
    public class HitPoints
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

                if (e.Before > e.After) Decreased?.Invoke(this, e);
                else PointsIncreased?.Invoke(this, e);
            }
        }
        public uint MaxValue { get; }
        public uint InitialValue { get; }
        public event PointsIncreasedHandler PointsIncreased;
        public event PointsDecreasedHandler Decreased;

        public void Reset()
        {
            Value = InitialValue;
        }

        public HitPoints() { }

        public HitPoints(uint initial) : this(initial, initial)
        { }

        public HitPoints(uint initial, uint max)
        {
            InitialValue = initial;
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
