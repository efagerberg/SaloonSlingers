using UnityEngine;


namespace SaloonSlingers.Unity.Actor
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
                    break;
                case ControllerTypes.ENEMY:
                    enemyController.enabled = true;
                    playerController.enabled = false;
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
