using SaloonSlingers.Core;

using TMPro;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity
{
    public class SaloonHandout : MonoBehaviour
    {
        [SerializeField]
        private TextAsset saloonConifgAsset;
        [SerializeField]
        private TextMeshProUGUI description;
        [SerializeField]
        private TextMeshProUGUI title;
        [SerializeField]
        private TextMeshProUGUI houseGame;
        [SerializeField]
        private TextMeshProUGUI interestRisk;
        [SerializeField]
        private SceneLoader sceneLoader;

        private Saloon saloon;
        private XRBaseInteractable interactable;

        private void Awake()
        {
            saloon = LevelManager.Load(saloonConifgAsset.text);
            interestRisk.text = string.Format("{0}%", saloon.InterestRisk * 100.0f);
            title.text = saloon.Name;
            description.text = saloon.Description;
            houseGame.text = saloon.HouseGame.Name;

            interactable = GetComponent<XRBaseInteractable>();
        }

        private void OnEnable()
        {
            interactable.activated.AddListener(OnActivate);
        }

        private void OnDisable()
        {
            interactable.activated.RemoveListener(OnActivate);
        }

        private void OnActivate(ActivateEventArgs arg0)
        {
            GameManager.Instance.LoadSaloon(saloon);
        }
    }
}
