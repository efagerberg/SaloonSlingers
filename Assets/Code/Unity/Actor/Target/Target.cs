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
        private float raycastDistance = 0.1f;

        private Health health;
        private Rigidbody rb;
        private bool switchDirection = false;

        public void Reset()
        {
            health.Reset();
        }

        private void Awake()
        {
            health = GetComponent<Health>();
            rb = GetComponent<Rigidbody>();
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
