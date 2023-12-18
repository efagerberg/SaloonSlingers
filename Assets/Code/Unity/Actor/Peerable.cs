using System.Collections;
using System.Linq;

using UnityEngine;


namespace SaloonSlingers.Unity.Actor
{
    public class Peerable : ActionPerformer
    {
        public float Interval { get; set; } = 0.2f;

        [SerializeField]
        private uint startingPeers = 3;
        [SerializeField]
        private float startingCooldown = 3;
        [SerializeField]
        private float startingDuration = 5f;
        [SerializeField]
        private float startingRecoveryPeriod = 1f;

        public void CastPeer(VisibilityDetector detector, EnemyHandDisplay display)
        {

            IEnumerator coroutine = GetActionCoroutine(() => DoPeer(detector, display));
            if (coroutine == null) return;

            StartCoroutine(coroutine);
        }

        private IEnumerator DoPeer(VisibilityDetector detector, EnemyHandDisplay display)
        {
            Enemy lastEnemy = null;
            Outline lastOutline = null;
            Enemy currentEnemy = null;
            Outline currentOutline = null;
            float currentDuration = MetaData.Duration;
            var intervalWait = new WaitForSeconds(Interval);

            while (currentDuration > 0)
            {
                var closest = detector.GetVisible(LayerMask.GetMask("Enemy"), xRay: true)
                                                .FirstOrDefault();

                if (closest == null)
                {
                    display.SetProjectile(null);
                    if (lastOutline != null) lastOutline.enabled = false;
                }
                else
                {
                    currentEnemy = closest.GetComponentInParent<Enemy>();
                    var projectile = currentEnemy.GetComponentInChildren<HandProjectile>();
                    display.SetProjectile(projectile);

                    if (currentEnemy != null && currentEnemy != lastEnemy)
                    {
                        currentOutline = currentEnemy.GetComponent<Outline>();
                        currentOutline.enabled = true;
                    }
                }
                display.Show();

                if (lastOutline != null && currentOutline != lastOutline) lastOutline.enabled = false;
                lastEnemy = currentEnemy;
                lastOutline = currentOutline;
                yield return intervalWait;
                currentDuration -= Interval;
            };
            display.SetProjectile(null);
            display.Hide();
            if (lastEnemy != null) lastOutline.enabled = false;
        }

        private void Start()
        {
            Points = new(startingPeers);
            MetaData = new(startingDuration, startingCooldown, startingRecoveryPeriod);
        }
    }
}
