using System;

namespace SaloonSlingers.Core
{
    public delegate void HitPointsChangedHandler(Health sender, ValueChangeEvent<uint> e);
    public delegate void AliveChangedHandler(Health sender, ValueChangeEvent<bool> e);

    public class Health
    {
        public bool Alive { get; private set; }
        public uint HitPoints
        {
            get { return _hitPoints; }
            set
            {
                uint hitPointsBefore = _hitPoints;
                _hitPoints = Math.Clamp(value, 0, MaxHitPoints);
                if (hitPointsBefore != _hitPoints)
                    OnHitPointsChanged?.Invoke(this, new ValueChangeEvent<uint>(hitPointsBefore, _hitPoints));

                bool aliveBefore = Alive;
                Alive = _hitPoints > 0;
                if (aliveBefore != Alive)
                    OnDestroyedChanged?.Invoke(this, new ValueChangeEvent<bool>(aliveBefore, Alive));
            }
        }
        public uint MaxHitPoints { get; }
        public event HitPointsChangedHandler OnHitPointsChanged;
        public event AliveChangedHandler OnDestroyedChanged;

        public Health(uint initial)
        {
            MaxHitPoints = initial;
            HitPoints = initial;
            Alive = initial > 0;
        }

        private uint _hitPoints;
    }
}
