using System;

using SaloonSlingers.Core;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Target : MonoBehaviour, IActor
    {
        public event EventHandler Death;
        [SerializeField]
        private float speed = 1.0f;
        [SerializeField]
        private float moveDistance = 5.0f;

        private Health health;
        private bool movingRight = true;

        public void Reset()
        {
            health.Reset();
        }

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            health.Points.OnPointsChanged += HandleHealthChanged;
        }

        private void OnDisable()
        {
            health.Points.OnPointsChanged -= HandleHealthChanged;
        }

        private void HandleHealthChanged(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After == 0)
                Death?.Invoke(gameObject, EventArgs.Empty);
        }

        private void Update()
        {
            // Calculate translation based on speed and direction
            float translation = speed * Time.deltaTime * (movingRight ? 1 : -1);

            // Translate the object's position
            transform.Translate(translation, 0, 0);

            // Change direction at boundaries
            if (transform.position.x > moveDistance || transform.position.x < -moveDistance)
            {
                movingRight = !movingRight;
            }
        }
    }
}
