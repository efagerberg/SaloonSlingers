using System.Collections;
using System.Collections.Generic;
using System.Linq;

using SaloonSlingers.Core;

using UnityEngine;


namespace SaloonSlingers.Unity.Actor
{
    public struct EnemyData
    {
        public Attributes Attributes;
        public Outline Outline;
        public HandProjectileCurseTarget CurseTarget;
    }

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

        private const float UI_DISTANCE = 0.5f;

        public void CastPeer(VisibilityDetector detector, EnemyInfoDisplay display, Transform peererTransform)
        {

            IEnumerator coroutine = GetActionCoroutine(() => DoPeer(detector, display, peererTransform));
            if (coroutine == null) return;

            StartCoroutine(coroutine);
        }

        private IEnumerator DoPeer(VisibilityDetector detector, EnemyInfoDisplay display, Transform peererTransform)
        {
            int lastSeenId = -1;
            IDictionary<int, EnemyData> seen = new Dictionary<int, EnemyData>();

            float currentDuration = MetaData.Duration;
            while (currentDuration > 0)
            {
                var closest = (detector.GetVisible(LayerMask.GetMask("Enemy"), xRay: true)
                                       .FirstOrDefault());
                if (closest == null || !closest.TryGetComponent<Attributes>(out var currentAttributes))
                {
                    display.Hide();
                    yield return new WaitForSeconds(Interval);
                    currentDuration -= Interval;
                    continue;
                }

                int currentId = closest.GetInstanceID();
                if (!seen.TryGetValue(currentId, out var currentEnemyData))
                {
                    currentEnemyData = seen[currentId] = new EnemyData()
                    {
                        CurseTarget = currentAttributes.GetComponent<HandProjectileCurseTarget>(),
                        Attributes = currentAttributes,
                        Outline = currentAttributes.GetComponent<Outline>()
                    };
                }

                if (lastSeenId != -1 && lastSeenId != currentId)
                    seen[lastSeenId].Outline.enabled = false;
                currentEnemyData.Outline.enabled = true;
                lastSeenId = currentId;

                display.Show();
                HandProjectile projectile = null;
                float timeWaited = 0;
                while (timeWaited < Interval)
                {
                    if (projectile == null)
                        projectile = closest.GetComponentInChildren<HandProjectile>();
                    display.SetTarget(currentEnemyData, projectile);
                    var direction = (currentAttributes.transform.position - peererTransform.position);
                    var offset = new Vector3(0.1f, 0.1f, 0);
                    // Make sure the UI is in front of the player when close
                    var distance = Mathf.Min(UI_DISTANCE, direction.magnitude / 2f);
                    var basePosition = peererTransform.position + direction.normalized * distance;
                    display.transform.position = basePosition + offset;
                    yield return null;
                    timeWaited += Time.deltaTime;
                }
                currentDuration -= timeWaited;
            };

            display.Hide();
            foreach (var x in seen.Values)
                x.Outline.enabled = false;
            seen.Clear();
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
