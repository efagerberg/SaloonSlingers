using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class RaycastRebound : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1.0f;
        [SerializeField]
        private float raycastDistance = 0.1f;

        private bool switchDirection = false;

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
