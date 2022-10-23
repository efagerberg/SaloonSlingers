using System;

namespace SaloonSlingers.Core
{
    public delegate void DashPointsChangedHandler(Dash sender, ValueChangeEvent<uint> e);

    public class Dash
    {
        public uint DashPoints {
            get { return _dashPoints; }
            set {
                uint dashPointsBefore = _dashPoints;
                _dashPoints = Math.Clamp(value, 0, MaxDashPoints);
                if (dashPointsBefore != _dashPoints)
                    OnDashPointsChanged?.Invoke(this, new ValueChangeEvent<uint>(dashPointsBefore, _dashPoints));
            }
        }
        public float Speed { get; set; }
        public float Duration { get; set; }
        public float CoolDown { get; set; }
        public float PointRecoveryPeriod { get; set; }
        public uint MaxDashPoints { get; }
        public event DashPointsChangedHandler OnDashPointsChanged;

        public Dash(uint dashPoints, float speed, float duration, float coolDown, float pointRecoveryPeriod)
        {
            DashPoints = dashPoints;
            MaxDashPoints = dashPoints;
            Speed = speed;
            Duration = duration;
            CoolDown = coolDown;
            PointRecoveryPeriod = pointRecoveryPeriod;
        }

        private uint _dashPoints;
    }
}
