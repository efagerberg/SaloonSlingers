using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class TransitionCanvasGroupRegister : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup transitionCanvasGroup;

        void Awake()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SceneLoader.TransitionCanvasGroup = transitionCanvasGroup;
        }
    }
}
