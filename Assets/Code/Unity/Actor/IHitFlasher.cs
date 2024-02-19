using System.Collections;

namespace SaloonSlingers.Unity.Actor
{
    public interface IHitFlasher
    {
        public IEnumerator Flash(float duration);
    }
}