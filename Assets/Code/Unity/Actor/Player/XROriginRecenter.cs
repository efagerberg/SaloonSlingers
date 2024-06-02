using Unity.XR.CoreUtils;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class XROriginRecenter : MonoBehaviour
    {
        public Transform OrientationReference;
        public XROrigin Origin;

        private bool initialRecenteringPerformed = false;

        public void Recenter()
        {
            Origin.MatchOriginUpCameraForward(OrientationReference.up, OrientationReference.forward);
            Origin.MoveCameraToWorldLocation(OrientationReference.position + new Vector3(0, Origin.CameraInOriginSpaceHeight, 0));
        }

        private void Awake()
        {
            if (Origin == null) Origin = GetComponent<XROrigin>();
        }

        private void OnEnable()
        {
            Recenter();
        }
    }
}
