using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager found");
        }
        else
        {
            instance = this;
        }
    }

    public void SetSceneCameraActive(bool _isActive)
    {
        if (sceneCamera == null)
            sceneCamera = GameObject.FindGameObjectWithTag("SceneCamera");

        sceneCamera.SetActive(_isActive);
    }
}