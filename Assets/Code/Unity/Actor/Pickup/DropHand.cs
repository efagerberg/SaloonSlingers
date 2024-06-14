using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class DropHand : MonoBehaviour
    {
        public void Drop()
        {
            var projectiles = GetComponentsInChildren<HandProjectile>();
            foreach (var projectile in projectiles)
            {
                projectile.Throw();
                projectile.transform.parent = null;
                var swapper = projectile.GetComponent<ControllerSwapper>();
                projectile.gameObject.layer = LayerMask.NameToLayer("UnassignedInteractable");
                swapper.SetController(ControllerTypes.PLAYER);
            }
        }
    }
}
