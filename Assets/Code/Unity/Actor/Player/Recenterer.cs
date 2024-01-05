using Unity.XR.CoreUtils;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Recenterer : MonoBehaviour
    {
        [SerializeField]
        private Transform orientationReference;
        [SerializeField]
        private XROrigin origin;

        private void Awake()
        {
            if (origin == null) origin = GetComponent<XROrigin>();
        }

        private void Start()
        {
            Recenter();
        }

        public void Recenter()
        {
            origin.MatchOriginUpCameraForward(orientationReference.up, orientationReference.forward);
            origin.MoveCameraToWorldLocation(orientationReference.position + new Vector3(0, origin.CameraInOriginSpaceHeight, 0));
        }
    }
}
