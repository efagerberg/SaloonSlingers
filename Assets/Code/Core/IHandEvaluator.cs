namespace SaloonSlingers.Core
{
    public interface IHandEvaluator
    {
        public float Evaluate(Card[] hand);
        public float GetMaxHandValue();
        public float GetMinHandValue();
    }
}
