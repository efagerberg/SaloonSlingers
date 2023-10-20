using System;

namespace SaloonSlingers.Core
{
    public delegate void PointsIncreasedHandler(Points sender, ValueChangeEvent<uint> e);
    public delegate void PointsDecreasedHandler(Points sender, ValueChangeEvent<uint> e);

    [Serializable]
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

                if (e.Before > e.After) Decreased?.Invoke(this, e);
                else Increased?.Invoke(this, e);
            }
        }
        public uint MaxValue { get; }
        public uint InitialValue { get; private set; }
        public event PointsIncreasedHandler Increased;
        public event PointsDecreasedHandler Decreased;

        public void Reset()
        {
            Value = InitialValue;
        }

        public void Reset(uint newValue)
        {
            InitialValue = newValue;
            Value = newValue;
        }

        public Points(uint initial) : this(initial, initial)
        { }

        public Points(uint initial, uint max)
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

        public float AsPercent()
        {
            if (_value == 0) return 0;

            return _value / (float)InitialValue;
        }

        private uint _value;
    }
}
