using UnityEngine;
using UnityEngine.UI;

namespace SaloonSlingers.Unity
{
    public class TransitionImageRegister : MonoBehaviour
    {
        [SerializeField]
        private Image transitionImage;

        void Awake()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SceneLoader.TransitionImage = transitionImage;
        }
    }
}
