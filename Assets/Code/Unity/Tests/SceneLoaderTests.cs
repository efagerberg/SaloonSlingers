using System.Collections;

using NUnit.Framework;

using SaloonSlingers.Unity;
using SaloonSlingers.Unity.Tests;

using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SceneLoaderTests
{
    [UnityTest]
    public IEnumerator WhenSceneAlreadyActive_DoesNotReloadScene()
    {
        var currentScene = SceneManager.GetActiveScene();
        var sceneLoader = TestUtils.CreateComponent<SceneLoader>();
        sceneLoader.LoadScene(currentScene.name);

        int tries = 10;
        while (tries-- > 0)
        {
            yield return null;
            Assert.That(SceneManager.GetActiveScene().name, Is.EqualTo(currentScene.name));
        }
    }
}
