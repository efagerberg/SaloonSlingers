using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Target : MonoBehaviour, IActor
    {
        public event EventHandler Killed;
        [SerializeField]
        private float speed = 1.0f;
        [SerializeField]
        private float raycastDistance = 0.1f;

        private Core.Attribute hitPoints;
        private bool switchDirection = false;

        public void ResetActor()
        {
            hitPoints.Reset();
        }

        private void OnEnable()
        {
            if (hitPoints == null) return;

            hitPoints.Depleted += OnDeath;
        }

        private void OnDisable()
        {
            if (hitPoints == null) return;

            hitPoints.Depleted -= OnDeath;
        }

        private void Start()
        {
            hitPoints ??= GetComponent<Attributes>().Registry[AttributeType.Health];
            hitPoints.Depleted += OnDeath;
        }

        private void OnDeath(IReadOnlyAttribute sender, EventArgs e)
        {
            Killed?.Invoke(gameObject, EventArgs.Empty);
        }

        private void FixedUpdate()
        {
            if (switchDirection == false && Physics.Raycast(transform.position, transform.forward, out _, raycastDistance))
            {
                switchDirection = true;
            }
        }

        private void Update()
        {
            if (switchDirection)
            {
                transform.Rotate(Vector3.up, 180);
                switchDirection = false;
            }
            float velocity = speed * Time.deltaTime;
            transform.Translate(velocity * Vector3.forward);
        }
    }
}
