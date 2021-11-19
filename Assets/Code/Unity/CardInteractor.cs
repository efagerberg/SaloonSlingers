
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GambitSimulator.Unity
{
    public class CardInteractor : XRDirectInteractor
    {
        [SerializeField]
        private CardSpawner cardSpawner;

        public override void GetValidTargets(List<XRBaseInteractable> targets)
        {
            base.GetValidTargets(targets);
            if (isSelectActive && targets.Count == 0 && cardSpawner.TrySpawnCard(transform.position, out CardInteractable interactable))
                targets.Add(interactable);
        }
    }
}
