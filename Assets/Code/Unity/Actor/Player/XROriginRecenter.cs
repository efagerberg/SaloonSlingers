using Unity.XR.CoreUtils;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class XROriginRecenter : MonoBehaviour
    {
        public Transform OrientationReference;
        public XROrigin Origin;

        public void Recenter()
        {
            Origin.MatchOriginUpCameraForward(OrientationReference.up, OrientationReference.forward);
            Origin.MoveCameraToWorldLocation(OrientationReference.position + new Vector3(0, Origin.CameraInOriginSpaceHeight, 0));
        }

        private void Start()
        {
            if (Origin == null) Origin = GetComponent<XROrigin>();
            Recenter();
        }
    }
}
