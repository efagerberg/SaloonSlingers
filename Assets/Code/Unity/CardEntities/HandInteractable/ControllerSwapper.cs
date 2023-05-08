using SaloonSlingers.Unity.CardEntities;

using UnityEngine;


namespace SaloonSlingers.Unity
{
    public enum ControllerTypes
    {
        PLAYER, ENEMY
    }

    [RequireComponent(typeof(PlayerHandInteractableController))]
    [RequireComponent(typeof(EnemyHandInteractableController))]
    public class ControllerSwapper : MonoBehaviour
    {
        [SerializeField]
        private PlayerHandInteractableController playerController;
        [SerializeField]
        private EnemyHandInteractableController enemyController;

        public void SetController(ControllerTypes type)
        {
            switch (type)
            {
                case ControllerTypes.PLAYER:
                    playerController.enabled = true;
                    enemyController.enabled = false;
                    gameObject.layer = LayerMask.NameToLayer("PlayerBody");
                    break;
                case ControllerTypes.ENEMY:
                    enemyController.enabled = true;
                    playerController.enabled = false;
                    gameObject.layer = LayerMask.NameToLayer("Enemy");
                    break;
            }
        }

        private void Awake()
        {
            playerController.enabled = false;
            enemyController.enabled = false;
        }
    }
}
