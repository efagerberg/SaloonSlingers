using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public class ContinueButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(ToNextScene);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ToNextScene);
        }

        private void ToNextScene()
        {
            GameManager.Instance.SceneLoader.LoadScene("StartingScene");
        }
    }
}
