using System;
using System.Collections;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.CardEntities;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.AI;

namespace SaloonSlingers.Unity.Slingers
{
    public class Enemy : MonoBehaviour, IActor
    {
        public Deck Deck { get; private set; }
        public event EventHandler Death;

        [SerializeField]
        private float lookDistance = 10f;
        [SerializeField]
        private float lookSpeed = 5f;
        [SerializeField]
        private Transform handAttachTransform;
        [SerializeField]
        private float persueStoppingDistance = 5f;
        [SerializeField]
        private float throwSpeed = 5f;
        [SerializeField]
        private int handSizeBeforeAttack = 5;
        [SerializeField]
        private float lineOfSightSphereCastRaidus = 0.25f;
        [SerializeField]
        private Renderer _renderer;

        private Transform playerTarget;
        private Transform currentTarget;
        private Health targetHealth;
        private Health health;
        private NavMeshAgent agent;
        private EnemyHandInteractableController currentHandController;
        private Rigidbody rigidBody;
        private CardSpawner cardSpawner;
        private Vector3 spawnPosition;
        private HandInteractableSpawner handInteractableSpawner;
        private Color originalColor;

        public void Reset()
        {
            var health = GetComponent<Health>();
            health.Reset();
        }

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerTarget = player.GetComponent<XROrigin>().Camera.transform;
            currentTarget = playerTarget;
            targetHealth = player.GetComponent<Health>();
            health = GetComponent<Health>();
            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<CardSpawner>();
            handInteractableSpawner = GameObject.FindGameObjectWithTag("HandInteractableSpawner")
                                                .GetComponent<HandInteractableSpawner>();
            originalColor = _renderer.material.color;
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            rigidBody = GetComponent<Rigidbody>();
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

        private IEnumerator HitEffect()
        {
            _renderer.material.color = Color.yellow;
            yield return new WaitForSeconds(2);
            _renderer.material.color = originalColor;
        }

        private void HandleHealthChanged(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.Before > e.After && e.After != 0) StartCoroutine(nameof(HitEffect));
            if (e.After == 0)
            {
                CancelInvoke(nameof(Attack));
                StopCoroutine(nameof(HitEffect));
                _renderer.material.color = originalColor;
                Death?.Invoke(gameObject, EventArgs.Empty);
            }
        }

        private void Attack()
        {
            if (!agent.hasPath) return;
            if (currentHandController == null)
            {
                GameObject clone = SpawnInteractable();
                currentHandController = clone.GetComponent<EnemyHandInteractableController>();
                currentHandController.transform.SetParent(handAttachTransform, false);
            }
            if (currentHandController.Cards.Count < handSizeBeforeAttack)
            {
                currentHandController.Draw(Deck, cardSpawner.Spawn);
                return;
            }

            if (Physics.SphereCast(transform.position,
                           lineOfSightSphereCastRaidus,
                           transform.forward,
                           out RaycastHit hit,
                           lookDistance,
                           LayerMask.GetMask("Player", "Envionrment")))
            {
                Debug.DrawLine(transform.position, hit.point, Color.blue, 1f);
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment")) return; ;

                currentHandController.transform.SetParent(null, true);
                // Aim for more center mass
                Vector3 heightOffset = new(0, 0.25f, 0);
                Vector3 direction = (currentTarget.position - heightOffset - transform.position).normalized;
                currentHandController.Throw(direction * throwSpeed);
                ControllerSwapper swapper = currentHandController.GetComponent<ControllerSwapper>();
                swapper.SetController(ControllerTypes.PLAYER);
                currentHandController = null;
            }
        }

        private void DespawnHandProjectile(object sender, EventArgs _)
        {
            var instance = sender as GameObject;
            var projectile = instance.GetComponent<HandProjectile>();
            foreach (ICardGraphic c in projectile.CardGraphics)
                c.Die();
            projectile.Death -= DespawnHandProjectile;
        }

        private void DeckEmptyHandler(Deck sender, EventArgs e)
        {
            CancelInvoke(nameof(Attack));
        }
        private GameObject SpawnInteractable()
        {
            GameObject spawned = handInteractableSpawner.Spawn();
            HandProjectile projectile = spawned.GetComponent<HandProjectile>();
            projectile.Death += DespawnHandProjectile;
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.ENEMY);
            return spawned;
        }
    }
}
