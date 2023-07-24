using System;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.CardEntities;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace SaloonSlingers.Unity.Slingers
{
    public delegate void EnemyDied(Enemy sender, EventArgs e);

    public class Enemy : MonoBehaviour
    {
        public Deck Deck { get; private set; }
        public event EnemyDied OnEnemyDied;

        [SerializeField]
        private float lookDistance = 10f;
        [SerializeField]
        private float lookSpeed = 5f;
        [SerializeField]
        private GameObject handInteractablePrefab;
        [SerializeField]
        private int interactablePoolSize = 5;
        [SerializeField]
        private Transform handAttachTransform;
        [SerializeField]
        private float persueStoppingDistance = 5f;
        [SerializeField]
        private float throwSpeed = 5f;

        private Transform playerTarget;
        private Transform currentTarget;
        private Health targetHealth;
        private Health health;
        private NavMeshAgent agent;
        private IObjectPool<GameObject> handInteractablePool;
        private EnemyHandInteractableController currentHandController;
        private Rigidbody rigidBody;
        private ICardSpawner cardSpawner;
        private Vector3 spawnPosition;

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerTarget = player.GetComponent<XROrigin>().Camera.transform;
            currentTarget = playerTarget;
            targetHealth = player.GetComponent<Health>();
            health = GetComponent<Health>();
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<CardSpawner>();
            handInteractablePool = new UnityEngine.Pool.ObjectPool<GameObject>(
                () =>
                {
                    GameObject go = Instantiate(handInteractablePrefab);
                    go.SetActive(false);
                    return go;
                },
                (GameObject go) =>
                {
                    go.SetActive(true);
                    HandProjectile projectile = go.GetComponent<HandProjectile>();
                    projectile.OnHandProjectileDied += DespawnHandProjectile;
                    ControllerSwapper swapper = go.GetComponent<ControllerSwapper>();
                    swapper.SetController(ControllerTypes.ENEMY);
                },
                (GameObject go) =>
                {
                    go.SetActive(false);
                    HandProjectile projectile = go.GetComponent<HandProjectile>();
                    projectile.OnHandProjectileDied -= DespawnHandProjectile;
                    go.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                },
                defaultCapacity: interactablePoolSize
            );
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            rigidBody = GetComponent<Rigidbody>();
            GameObject clone = handInteractablePool.Get();
            currentHandController = clone.GetComponent<EnemyHandInteractableController>();
            currentHandController.transform.SetParent(handAttachTransform, false);
        }

        private void Update()
        {
            if (currentTarget == null)
            {
                ReturnHome();
                return;
            }

            float distance = Vector3.Distance(currentTarget.position, transform.position);
            if (distance <= lookDistance)
            {
                FaceTarget();
                agent.SetDestination(currentTarget.position);
                agent.stoppingDistance = persueStoppingDistance;
                if (!IsInvoking(nameof(Attack)))
                    InvokeRepeating(nameof(Attack), 0.0f, 1.0f);
            }
            else ReturnHome();
        }

        private void ReturnHome()
        {
            agent.SetDestination(spawnPosition);
            agent.stoppingDistance = 0f;
            CancelInvoke(nameof(Attack));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookDistance);
        }

        private void FaceTarget()
        {
            Vector3 direction = (currentTarget.position - transform.position).normalized;
            Quaternion lookRotations = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotations, Time.deltaTime * lookSpeed);
        }

        private void OnEnable()
        {
            Deck = new Deck().Shuffle();
            Deck.OnDeckEmpty += DeckEmptyHandler;
            spawnPosition = transform.position;
            targetHealth.Points.OnPointsChanged += HandleTargetHealthChanged;
            health.Points.OnPointsChanged += HandleHealthChanged;
        }

        private void OnDisable()
        {
            Deck.OnDeckEmpty -= DeckEmptyHandler;
            targetHealth.Points.OnPointsChanged -= HandleTargetHealthChanged;
            health.Points.OnPointsChanged -= HandleHealthChanged;
        }

        private void HandleTargetHealthChanged(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After == 0) currentTarget = null;
            if (e.Before == 0 && e.After != 0) currentTarget = playerTarget;
        }

        private void HandleHealthChanged(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After == 0)
            {
                CancelInvoke(nameof(Attack));
                handInteractablePool.Clear();
                OnEnemyDied?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Attack()
        {
            if (!agent.hasPath) return;
            if (currentHandController.Cards.Count == 2)
            {
                currentHandController.transform.SetParent(null, true);
                Vector3 direction = (currentTarget.position - currentHandController.transform.position).normalized;
                currentHandController.Throw(direction * throwSpeed);
                ControllerSwapper swapper = currentHandController.GetComponent<ControllerSwapper>();
                swapper.SetController(ControllerTypes.PLAYER);

                GameObject clone = handInteractablePool.Get();
                currentHandController = clone.GetComponent<EnemyHandInteractableController>();
                currentHandController.transform.SetParent(handAttachTransform, false);
                return;
            }
            currentHandController.Draw(Deck, cardSpawner);
        }

        private void DespawnHandProjectile(HandProjectile sender, EventArgs _)
        {
            foreach (ICardGraphic c in sender.CardGraphics)
                cardSpawner.Despawn(c);
            handInteractablePool.Release(sender.gameObject);
        }

        private void DeckEmptyHandler(Deck sender, EventArgs e)
        {
            handInteractablePool.Clear();
            CancelInvoke(nameof(Attack));
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(currentHandController.transform.position, currentTarget.position);
        }
    }
}
