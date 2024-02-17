using System;

namespace SaloonSlingers.Core
{
    public delegate void IncreasedHandler(IReadOnlyAttribute sender, ValueChangeEvent<uint> e);
    public delegate void DecreasedHandler(IReadOnlyAttribute sender, ValueChangeEvent<uint> e);
    public delegate void DepletedHandler(IReadOnlyAttribute sender, EventArgs e);

    public interface IReadOnlyAttribute
    {
        uint Value { get; }
        uint MaxValue { get; }
        uint InitialValue { get; }
        event IncreasedHandler Increased;
        event DecreasedHandler Decreased;
        event DepletedHandler Depleted;
        float AsPercent();
    }

    [Serializable]
    public class Attribute : IReadOnlyAttribute
    {
        public uint Value
        {
            get { return _value; }
            private set
            {
                uint before = _value;

                if (value == uint.MaxValue && before == 0) _value = 0;
                else _value = System.Math.Clamp(value, 0, MaxValue);

                var e = new ValueChangeEvent<uint>(before, _value);
                if (e.Before == e.After) return;

                if (e.Before > e.After)
                {
                    Decreased?.Invoke(this, e);
                    if (e.After == 0) Depleted?.Invoke(this, EventArgs.Empty);
                }
                else Increased?.Invoke(this, e);
            }
        }
        public uint MaxValue { get; }
        public uint InitialValue { get; private set; }
        public event IncreasedHandler Increased;
        public event DecreasedHandler Decreased;
        public event DepletedHandler Depleted;

        public void Reset()
        {
            Value = InitialValue;
        }

        public void Reset(uint newValue)
        {
            InitialValue = newValue;
            Value = newValue;
        }

        public Attribute(uint initial) : this(initial, initial)
        { }

        public Attribute(uint initial, uint max)
        {
            InitialValue = initial;
            MaxValue = max;
            _value = initial;
        }

        public void Decrement() => Decrease(1);

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
            if (Value == 0) return 0;

            return Value / (float)InitialValue;
        }

        public static implicit operator uint(Attribute p) => p.Value;

        private uint _value;
    }
}
