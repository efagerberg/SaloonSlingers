using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HandInitializer : MonoBehaviour
    {
        [SerializeField]
        private Transform leftDeckAttachTransform;
        [SerializeField]
        private Transform rightDeckAttachTransform;
        [SerializeField]
        private GameObject deckGraphicPrefab;
        [SerializeField]
        private Transform leftAbsorberAttachTransform;
        [SerializeField]
        private Transform rightAbsorberAttachTransform;
        [SerializeField]
        private GameObject absorberPrefab;
        [SerializeField]
        private Transform enemyPeerPanelAttachTransform;
        [SerializeField]
        private GameObject enemyPeerPanelPrefab;
        [SerializeField]
        private Transform leftShieldAttachTransform;
        [SerializeField]
        private Transform rightShieldAttachTransform;
        [SerializeField]
        private GameObject shieldPrefab;

        private Transform deckAttachTransform;
        private Transform absorberAttachTransform;
        private Transform shieldAttachTransform;
        private bool isPrimary = false;

        private void Start()
        {
            ActorHandedness handedness = GetComponentInParent<ActorHandedness>();
            switch (handedness.Current)
            {
                case Handedness.RIGHT:
                    deckAttachTransform = leftDeckAttachTransform;
                    absorberAttachTransform = leftAbsorberAttachTransform;
                    shieldAttachTransform = leftShieldAttachTransform;
                    break;
                case Handedness.LEFT:
                    deckAttachTransform = rightDeckAttachTransform;
                    absorberAttachTransform = rightAbsorberAttachTransform;
                    shieldAttachTransform = rightShieldAttachTransform;
                    break;
            }

            isPrimary = transform.parent.parent.name.Contains(
                handedness.Current.ToString(),
                System.StringComparison.CurrentCultureIgnoreCase
            );
            var instantiated = new List<GameObject>();
            if (!isPrimary)
            {
                GameObject deckGraphicGO = Instantiate(deckGraphicPrefab, deckAttachTransform);
                handedness.DeckGraphic = deckGraphicGO.GetComponent<DeckGraphic>();
                var absorberGO = Instantiate(absorberPrefab, absorberAttachTransform);
                var shieldGO = Instantiate(shieldPrefab, shieldAttachTransform);
                instantiated.Append(absorberGO);
                instantiated.Append(shieldGO);
                instantiated.Append(deckGraphicGO);
            }
            else
            {
                GameObject clone = Instantiate(enemyPeerPanelPrefab, enemyPeerPanelAttachTransform);
                handedness.EnemyPeerDisplay = clone.GetComponent<EnemyHandDisplay>();
                instantiated.Append(clone);
            }

            foreach (var instance in instantiated)
                instance.layer = LayerMask.NameToLayer("Player");
        }
    }
}
