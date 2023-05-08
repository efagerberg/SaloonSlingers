namespace SaloonSlingers.Core
{
    public class ActionPoints : Points
    {
        public float Duration { get; set; }
        public float CoolDown { get; set; }
        public float PointRecoveryPeriod { get; set; }

        public ActionPoints(uint value, float duration, float coolDown, float pointRecoveryPeriod) : base(value)
        {
            Duration = duration;
            CoolDown = coolDown;
            PointRecoveryPeriod = pointRecoveryPeriod;
        }
    }
}
