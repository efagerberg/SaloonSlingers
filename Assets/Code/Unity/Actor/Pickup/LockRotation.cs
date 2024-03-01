using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class LockRotation : MonoBehaviour
    {
        [SerializeField]
        private Vector3 lockRotation;

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Euler(lockRotation);
        }
    }
}