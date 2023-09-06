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
        private Renderer _renderer;

        private Transform currentTarget;
        private Health targetHealth;
        private Health health;
        private NavMeshAgent agent;
        private EnemyHandInteractableController currentHandController;
        private CardSpawner cardSpawner;
        private Vector3 spawnPosition;
        private HandInteractableSpawner handInteractableSpawner;
        private Color originalColor;
        private VisibilityDetector visibilityDetector;

        public void Reset()
        {
            health.Reset();
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            visibilityDetector = GetComponent<VisibilityDetector>();
            health = GetComponent<Health>();

            cardSpawner = GameObject.FindGameObjectWithTag("CardSpawner").GetComponent<CardSpawner>();
            handInteractableSpawner = GameObject.FindGameObjectWithTag("HandInteractableSpawner")
                                                .GetComponent<HandInteractableSpawner>();
            originalColor = _renderer.material.color;
        }

        private void Update()
        {
            if (currentTarget != null)
            {
                if (Vector3.Distance(currentTarget.position, transform.position) > visibilityDetector.SightDistance)
                {
                    ReturnHome();
                    return;
                }
                agent.SetDestination(currentTarget.position);
                FaceTarget();
                return;
            }

            foreach (var target in visibilityDetector.GetVisible(LayerMask.GetMask("Player")))
            {
                currentTarget = target.transform.GetComponent<XROrigin>().Camera.transform;
                targetHealth = currentTarget.GetComponent<Health>();
                agent.stoppingDistance = persueStoppingDistance;
                if (!IsInvoking(nameof(Attack)))
                    InvokeRepeating(nameof(Attack), 0.0f, 1.0f);
                Debug.DrawLine(transform.position, target.point, Color.blue, 1f);
            }
        }

        private void ReturnHome()
        {
            currentTarget = null;
            agent.SetDestination(spawnPosition);
            agent.stoppingDistance = 0f;
            CancelInvoke(nameof(Attack));
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
            if (targetHealth != null)
                targetHealth.Points.OnPointsChanged += HandleTargetHealthChanged;
            health.Points.OnPointsChanged += HandleHealthChanged;
        }

        private void OnDisable()
        {
            Deck.OnDeckEmpty -= DeckEmptyHandler;
            if (targetHealth != null)
                targetHealth.Points.OnPointsChanged -= HandleTargetHealthChanged;
            health.Points.OnPointsChanged -= HandleHealthChanged;
        }

        private void HandleTargetHealthChanged(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After == 0)
            {
                currentTarget = null;
                ReturnHome();
            }
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

            currentHandController.transform.SetParent(null, true);
            // Aim for more center mass
            Vector3 heightOffset = new(0, 0.25f, 0);
            Vector3 direction = (currentTarget.position - transform.position - heightOffset).normalized;
            currentHandController.Throw(direction * throwSpeed);
            ControllerSwapper swapper = currentHandController.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.PLAYER);
            currentHandController = null;
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
