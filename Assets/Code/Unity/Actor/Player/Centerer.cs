using Unity.XR.CoreUtils;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class Centerer : MonoBehaviour
    {
        public Transform OrientationReference;
        public XROrigin Origin;

        private void Start()
        {
            if (Origin == null) Origin = GetComponent<XROrigin>();
            Center();
        }

        public void Center()
        {
            Origin.MatchOriginUpCameraForward(OrientationReference.up, OrientationReference.forward);
            Origin.MoveCameraToWorldLocation(OrientationReference.position + new Vector3(0, Origin.CameraInOriginSpaceHeight, 0));
        }
    }
}
