namespace SaloonSlingers.Core
{
    public delegate void DashPointsChangedHandler(DashConfig sender, ValueChangeEvent<uint> e);

    public class DashConfig
    {
        public Points DashPoints { get; }
        public float Speed { get; set; }
        public float Duration { get; set; }
        public float CoolDown { get; set; }
        public float PointRecoveryPeriod { get; set; }

        public DashConfig(uint dashPoints, float speed, float duration, float coolDown, float pointRecoveryPeriod)
        {
            DashPoints = new Points(dashPoints);
            Speed = speed;
            Duration = duration;
            CoolDown = coolDown;
            PointRecoveryPeriod = pointRecoveryPeriod;
        }
    }
}
