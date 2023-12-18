namespace SaloonSlingers.Core
{
    public class ActionMetaData
    {
        public float Duration { get; set; }
        public float CoolDown { get; set; }
        public float RecoveryPeriod { get; set; }

        public ActionMetaData(float duration, float coolDown, float pointRecoveryPeriod)
        {
            Duration = duration;
            CoolDown = coolDown;
            RecoveryPeriod = pointRecoveryPeriod;
        }
    }
}
