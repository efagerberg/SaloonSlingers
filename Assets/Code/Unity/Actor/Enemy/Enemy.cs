using System;
using System.Collections;
using System.Linq;

using SaloonSlingers.Core;

using Unity.XR.CoreUtils;

using UnityEngine;
using UnityEngine.AI;

namespace SaloonSlingers.Unity.Actor
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
        private Renderer _renderer;
        [SerializeField]
        private float visibilityIntervalSeconds = 1f;

        private Transform _currentTarget;
        private Transform currentTarget
        {
            get => _currentTarget;
            set
            {
                if (value == default)
                    _renderer.material.color = originalColor;
                else
                    _renderer.material.color = Color.red;
                _currentTarget = value;
            }
        }
        private Health targetHealth;
        private Health health;
        private NavMeshAgent agent;
        private EnemyHandInteractableController currentHandController;
        private CardSpawner cardSpawner;
        private Vector3 spawnPosition;
        private HandInteractableSpawner handInteractableSpawner;
        private Color originalColor;
        private VisibilityDetector visibilityDetector;
        private HeistManager gameRulesManager;
        private DrawContext drawCtx;

        public void Reset()
        {
            health.Reset();
            currentTarget = null;
            agent.stoppingDistance = 0f;
            currentHandController = null;
            CancelInvoke(nameof(Attack));
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
            gameRulesManager = GameObject.FindGameObjectWithTag("GameRulesManager").GetComponent<HeistManager>();
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

            if (!IsInvoking(nameof(LookForPlayer)))
                InvokeRepeating(nameof(LookForPlayer), visibilityIntervalSeconds, visibilityIntervalSeconds);
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

        private void LookForPlayer()
        {
            var player = visibilityDetector.GetVisible(LayerMask.GetMask("Player")).FirstOrDefault();
            if (player == null) return;

            currentTarget = player.GetComponent<XROrigin>().Camera.transform;
            targetHealth = currentTarget.GetComponent<Health>();
            agent.stoppingDistance = persueStoppingDistance;
            CancelInvoke(nameof(LookForPlayer));
            if (!IsInvoking(nameof(Attack)))
                InvokeRepeating(nameof(Attack), 0.0f, 1.0f);
        }

        private void HandleHealthChanged(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.Before > e.After && e.After != 0) StartCoroutine(nameof(HitEffect));
            if (e.After == 0)
            {
                CancelInvoke(nameof(Attack));
                CancelInvoke(nameof(LookForPlayer));
                StopCoroutine(nameof(HitEffect));
                _renderer.material.color = originalColor;
                Death?.Invoke(gameObject, EventArgs.Empty);
            }
        }

        private void Attack()
        {
            if (currentTarget == null) return;
            if (currentHandController == null)
            {
                GameObject clone = SpawnInteractable();
                currentHandController = clone.GetComponent<EnemyHandInteractableController>();
                currentHandController.transform.SetParent(handAttachTransform, false);
            }
            HandProjectile projectile = currentHandController.GetComponent<HandProjectile>();
            drawCtx.Hand = projectile.Cards;
            drawCtx.Evaluation = projectile.HandEvaluation;
            drawCtx.Deck = Deck;
            if (gameRulesManager.Game.CanDraw(drawCtx))
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
                c.Kill();
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
