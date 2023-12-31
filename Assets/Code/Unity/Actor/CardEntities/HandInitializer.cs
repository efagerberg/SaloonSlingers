using System.Collections.Generic;

using UnityEngine;

namespace SaloonSlingers.Unity.Actor
{
    public class HandInitializer : MonoBehaviour
    {
        [SerializeField]
        private Transform rightDeckAttachTransform;
        [SerializeField]
        private Transform leftDeckAttachTransform;
        [SerializeField]
        private GameObject deckGraphicPrefab;
        [SerializeField]
        private Transform rightAbsorberAttachTransform;
        [SerializeField]
        private Transform leftAbsorberAttachTransform;
        [SerializeField]
        private GameObject absorberPrefab;
        [SerializeField]
        private Transform rightEnemyPeerPanelAttachTransform;
        [SerializeField]
        private Transform leftEnemyPeerPanelAttachTransform;
        [SerializeField]
        private GameObject enemyPeerPanelPrefab;
        [SerializeField]
        private Transform rightShieldAttachTransform;
        [SerializeField]
        private Transform leftShieldAttachTransform;
        [SerializeField]
        private GameObject shieldPrefab;

        private Transform deckAttachTransform;
        private Transform absorberAttachTransform;
        private Transform shieldAttachTransform;
        private Transform enemyPeerPanelAttachTransform;
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
                    enemyPeerPanelAttachTransform = leftEnemyPeerPanelAttachTransform;
                    break;
                case Handedness.LEFT:
                    deckAttachTransform = rightDeckAttachTransform;
                    absorberAttachTransform = rightAbsorberAttachTransform;
                    shieldAttachTransform = rightShieldAttachTransform;
                    enemyPeerPanelAttachTransform = rightEnemyPeerPanelAttachTransform;
                    break;
            }

            isPrimary = transform.parent.parent.name.Contains(
                handedness.Current.ToString(),
                System.StringComparison.CurrentCultureIgnoreCase
            );
            List<GameObject> instantiated = new();
            if (!isPrimary)
            {
                var deckGraphicGO = Instantiate(deckGraphicPrefab, deckAttachTransform);
                var absorberGO = Instantiate(absorberPrefab, absorberAttachTransform);
                var shieldGO = Instantiate(shieldPrefab, shieldAttachTransform);
                var peerPanelGO = Instantiate(enemyPeerPanelPrefab, enemyPeerPanelAttachTransform);

                handedness.DeckGraphic = deckGraphicGO.GetComponent<DeckGraphic>();
                handedness.EnemyPeerDisplay = peerPanelGO.GetComponent<EnemyHandDisplay>();

                instantiated.Add(absorberGO);
                instantiated.Add(shieldGO);
                instantiated.Add(deckGraphicGO);
                instantiated.Add(peerPanelGO);
            }

            foreach (var instance in instantiated)
                instance.layer = LayerMask.NameToLayer("Player");
        }
    }
}
