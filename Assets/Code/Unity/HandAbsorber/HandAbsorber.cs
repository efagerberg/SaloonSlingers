using System.Collections;

using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HandAbsorber : MonoBehaviour
    {
        [SerializeField]
        private float absorbTime = 0.5f;

        public IEnumerator Absorb(HitPoints hitPoints, HandProjectile projectile)
        {
            projectile.Pause();
            yield return new WaitForSeconds(absorbTime);
            hitPoints.Points.Reset(projectile.HandEvaluation.Score);
            projectile.Kill();
        }
    }
}
