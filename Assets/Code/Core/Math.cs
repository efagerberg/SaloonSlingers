namespace SaloonSlingers.Core
{
    public static class Math
    {
        public static float Sigmoid(float x)
        {
            return (float)(1.0f / (1.0f + System.Math.Exp(-x)));
        }
    }
}