using System.Collections;
using System.Linq;

using SaloonSlingers.Core;

using UnityEngine;


namespace SaloonSlingers.Unity.Actor
{
    public class Peerable : ActionPerformer
    {
        [field: SerializeField]
        public float Interval { get; set; } = 0.2f;

        [SerializeField]
        private uint startingPeers = 3;
        [SerializeField]
        private float startingCooldown = 3;
        [SerializeField]
        private float startingDuration = 5f;
        [SerializeField]
        private float startingRecoveryPeriod = 1f;

        private const float X_OFFSET = 0.2f;
        private const float Y_OFFSET = 0.3f;
        private const float UI_DISTANCE = 1f;

        public void CastPeer(VisibilityDetector detector, EnemyHandDisplay display, Transform peererTransform)
        {

            IEnumerator coroutine = GetActionCoroutine(() => DoPeer(detector, display, peererTransform));
            if (coroutine == null) return;

            StartCoroutine(coroutine);
        }

        private IEnumerator DoPeer(VisibilityDetector detector, EnemyHandDisplay display, Transform peererTransform)
        {
            Enemy lastEnemy = null;
            Outline lastOutline = null;

            Outline currentOutline = null;
            Enemy currentEnemy = null;
            float currentDuration = MetaData.Duration;
            while (currentDuration > 0)
            {
                var closest = (detector.GetVisible(LayerMask.GetMask("Enemy"), xRay: true)
                                       .FirstOrDefault());

                if (closest == null ||
                    !closest.TryGetComponent<Enemy>(out currentEnemy))
                {
                    if (lastEnemy == null)
                    {
                        display.Hide();
                        display.SetProjectiles(null, null);
                    }
                    if (lastOutline != null) lastOutline.enabled = false;
                }
                else
                {
                    display.Show();
                    var projectile = currentEnemy.GetComponentInChildren<HandProjectile>();
                    var target = currentEnemy.GetComponent<HandProjectileCurseTarget>();
                    display.SetProjectiles(projectile, target.Cursed);

                    if (currentEnemy != null && currentEnemy != lastEnemy)
                    {
                        currentOutline = currentEnemy.GetComponent<Outline>();
                        currentOutline.enabled = true;
                    }

                }

                if (lastOutline != null && currentOutline != lastOutline) lastOutline.enabled = false;
                lastEnemy = currentEnemy;
                lastOutline = currentOutline;
                var intervalDuration = 0f;
                while (intervalDuration < Interval)
                {
                    if (currentEnemy)
                    {
                        var directionToEnemy = currentEnemy.transform.position - peererTransform.position;
                        var desiredPosition = peererTransform.position + directionToEnemy.normalized * UI_DISTANCE + new Vector3(X_OFFSET, Y_OFFSET, 0);
                        display.transform.position = desiredPosition;
                    }
                    intervalDuration += Time.deltaTime;
                    yield return null;
                }
                currentDuration -= intervalDuration;
            };

            display.Hide();
            if (lastEnemy != null) lastOutline.enabled = false;
        }

        private void Start()
        {
            if (IsInitialized) return;

            Attribute points = new(startingPeers);
            ActionMetaData metaData = new()
            {
                Duration = startingDuration,
                Cooldown = startingCooldown,
                RecoveryPeriod = startingRecoveryPeriod
            };
            Initialize(points, metaData);
        }
    }
}
