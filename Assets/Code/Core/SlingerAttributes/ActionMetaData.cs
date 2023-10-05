namespace SaloonSlingers.Core
{
    public class ActionMetaData
    {
        public float Duration { get; set; }
        public float CoolDown { get; set; }
        public float PointRecoveryPeriod { get; set; }

        public ActionMetaData(float duration, float coolDown, float pointRecoveryPeriod)
        {
            Duration = duration;
            CoolDown = coolDown;
            PointRecoveryPeriod = pointRecoveryPeriod;
        }
    }
}
