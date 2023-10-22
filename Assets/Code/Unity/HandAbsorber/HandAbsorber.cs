using System.Collections;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HandAbsorber : MonoBehaviour
    {
        [SerializeField]
        private float absorbTime = 0.5f;

        private Coroutine absorbCoroutine;

        public IEnumerator Absorb(TemporaryHitPoints tempHitPoints, HandProjectile projectile)
        {
            projectile.Pause();
            yield return new WaitForSeconds(absorbTime);
            tempHitPoints.Points.Reset(projectile.HandEvaluation.Score);
            projectile.Kill();
        }
    }
}
