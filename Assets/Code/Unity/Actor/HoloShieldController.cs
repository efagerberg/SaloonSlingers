using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class HoloShieldController : MonoBehaviour
    {
        [SerializeField]
        private TemporaryHitPoints temporaryHitPoints;
        [SerializeField]
        private GameObject shieldModel;
        [SerializeField]
        private SphereCollider sphereCollider;

        private void OnEnable()
        {
            temporaryHitPoints.Points.Increased += OnIncrease;
            temporaryHitPoints.Points.Decreased += OnDecreased;
        }

        private void OnDisable()
        {
            temporaryHitPoints.Points.Increased -= OnIncrease;
            temporaryHitPoints.Points.Decreased -= OnDecreased;
        }

        private void Awake()
        {
            SetShieldActive(temporaryHitPoints.Points.Value > 0);
        }

        private void OnIncrease(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.Before == 0) SetShieldActive(true);
        }

        private void OnDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After == 0) SetShieldActive(false);
        }

        private void SetShieldActive(bool active)
        {
            shieldModel.SetActive(active);
            sphereCollider.enabled = active;
        }

        private void LateUpdate()
        {
            // Looks better if the rotation is frozen
            // Eventually, there will be collision ripples to tell
            // the player where they hit an enemy or where they were hit.
            sphereCollider.transform.rotation = Quaternion.identity;
        }
    }
}
