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

        private Points hitPoints;
        private Rigidbody rb;
        private bool switchDirection = false;

        public void Reset()
        {
            hitPoints.Reset();
        }

        private void Awake()
        {
            hitPoints = GetComponent<Attributes>().Registry[AttributeType.Health];
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            hitPoints.Decreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            hitPoints.Decreased -= OnHitPointsDecreased;
        }

        private void OnHitPointsDecreased(IReadOnlyPoints sender, ValueChangeEvent<uint> e)
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
