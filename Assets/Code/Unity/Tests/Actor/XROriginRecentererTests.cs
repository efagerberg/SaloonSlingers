using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Unity.Actor;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

public class XROriginRecentererTests
{
    [Test]
    public void CentersXROrigin_WhenRecenterCall()
    {
        XROriginRecenter subject = BuildSubject(out Camera camera, out XROrigin origin, out GameObject referenceGO);
        subject.OrientationReference = referenceGO.transform;
        subject.Origin = origin;
        subject.Recenter();

        AssertCentered(camera, origin, referenceGO);
    }

    [UnityTest]
    public IEnumerator CentersXROrigin_AsPartOfInitialization()
    {
        XROriginRecenter subject = BuildSubject(out Camera camera, out XROrigin origin, out GameObject referenceGO);
        subject.runInEditMode = true;
        subject.OrientationReference = referenceGO.transform;
        yield return null;

        AssertCentered(camera, origin, referenceGO);
    }

    private static XROriginRecenter BuildSubject(out Camera camera, out XROrigin origin, out GameObject referenceGO)
    {
        var subjectGO = new GameObject();
        camera = subjectGO.AddComponent<Camera>();
        origin = subjectGO.AddComponent<XROrigin>();
        origin.Camera = camera;
        referenceGO = new GameObject();
        referenceGO.transform.SetPositionAndRotation(
            new Vector3(100, 2, -20),
            Quaternion.Euler(123, 321, 123)
        );
        subjectGO.transform.SetPositionAndRotation(
            new Vector3(1, 2, 3),
            Quaternion.Euler(90, 1, 123)
        );
        return subjectGO.AddComponent<XROriginRecenter>();
    }

    private static void AssertCentered(Camera camera, XROrigin origin, GameObject referenceGO)
    {
        var comparer = new Vector3EqualityComparer(10e-6f);
        Assert.That(origin.transform.up, Is.EqualTo(referenceGO.transform.up).Using(comparer));
        Assert.That(camera.transform.forward, Is.EqualTo(referenceGO.transform.forward).Using(comparer));
        Assert.That(camera.transform.position.y, Is.EqualTo(referenceGO.transform.position.y + origin.CameraInOriginSpaceHeight));
    }
}
