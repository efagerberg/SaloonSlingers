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
    public void CentersXROrigin_OnReference()
    {
        var subjectGO = new GameObject();
        var camera = subjectGO.AddComponent<Camera>();
        var origin = subjectGO.AddComponent<XROrigin>();
        origin.Camera = camera;
        var referenceGO = new GameObject();
        referenceGO.transform.SetPositionAndRotation(
            new Vector3(100, 2, -20),
            Quaternion.Euler(123, 321, 123)
        );
        subjectGO.transform.SetPositionAndRotation(
            new Vector3(1, 2, 3),
            Quaternion.Euler(90, 1, 123)
        );
        var subject = subjectGO.AddComponent<XROriginRecenter>();
        subject.OrientationReference = referenceGO.transform;
        subject.Origin = origin;
        subject.Recenter();

        var comparer = new Vector3EqualityComparer(10e-6f);
        Assert.That(origin.transform.up, Is.EqualTo(referenceGO.transform.up).Using(comparer));
        Assert.That(camera.transform.forward, Is.EqualTo(referenceGO.transform.forward).Using(comparer));
        Assert.That(camera.transform.position.y, Is.EqualTo(referenceGO.transform.position.y + origin.CameraInOriginSpaceHeight));
    }

    [RequiresPlayMode]
    [UnityTest]
    public IEnumerator Start_CentersXROrigin_OnReference()
    {
        var subjectGO = new GameObject();
        var camera = subjectGO.AddComponent<Camera>();
        var origin = subjectGO.AddComponent<XROrigin>();
        origin.Camera = camera;
        var referenceGO = new GameObject();
        referenceGO.transform.SetPositionAndRotation(
            new Vector3(100, 2, -20),
            Quaternion.Euler(123, 321, 123)
        );
        subjectGO.transform.SetPositionAndRotation(
            new Vector3(1, 2, 3),
            Quaternion.Euler(90, 1, 123)
        );
        var subject = subjectGO.AddComponent<XROriginRecenter>();
        subject.OrientationReference = referenceGO.transform;
        yield return null;

        var comparer = new Vector3EqualityComparer(10e-6f);
        Assert.That(origin.transform.up, Is.EqualTo(referenceGO.transform.up).Using(comparer));
        Assert.That(camera.transform.forward, Is.EqualTo(referenceGO.transform.forward).Using(comparer));
        Assert.That(camera.transform.position.y, Is.EqualTo(referenceGO.transform.position.y + origin.CameraInOriginSpaceHeight));
    }
}
