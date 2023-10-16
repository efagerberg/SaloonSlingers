using System.Collections;

using SaloonSlingers.Unity.Actor;

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SaloonSlingers.Unity
{
    public class HandAbsorber : MonoBehaviour
    {
        [SerializeField]
        private float absorbTime = 0.5f;

        private Coroutine absorbCoroutine;

        public void OnHover(HoverEnterEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<HandProjectile>(out var projectile)) return;
            
            projectile.Stack();
        }

        public void OnHoverExit(HoverExitEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<HandProjectile>(out var projectile)) return;

            projectile.Unstack();
        }

        public void OnSelectEnter(SelectEnterEventArgs args)
        {
            if (!args.interactableObject.transform.gameObject.TryGetComponent<HandProjectile>(out var projectile)) return;
            
            absorbCoroutine = StartCoroutine(Absorb(LevelManager.Instance.Player.GetComponent<TemporaryHitPoints>(), projectile));
        }

        public void OnSelectExit(SelectExitEventArgs _)
        {
            StopCoroutine(absorbCoroutine);
        }

        private IEnumerator Absorb(TemporaryHitPoints tempHitPoints, HandProjectile projectile)
        {
            projectile.Pause();
            yield return new WaitForSeconds(absorbTime);
            tempHitPoints.Points.Reset();
            tempHitPoints.Points.Increase(projectile.HandEvaluation.Score);
            projectile.Kill();
        }
    }
}
