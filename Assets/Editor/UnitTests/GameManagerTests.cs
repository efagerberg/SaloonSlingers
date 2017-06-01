using UnityEngine;

using NUnit.Framework;

public class GameManagerTests
{
    GameObject container;
    GameObject fakeCam;

    GameManager gm;

    [SetUp]
    public void Init()
    {
        container = new GameObject();
        fakeCam = new GameObject();
        fakeCam.tag = "SceneCamera";
        gm = container.AddComponent<GameManager>();
    }

    [TearDown]
    public void Dispose()
    {
        container = null;
    }

    [Test]
    public void SetSceneCameraActive_SetsCameraActive()
    {
        gm.SetSceneCameraActive(false);
        Assert.False(fakeCam.activeSelf);
    }
}
