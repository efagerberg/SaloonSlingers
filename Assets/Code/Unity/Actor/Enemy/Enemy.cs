using System;
using System.Collections;

using SaloonSlingers.Core;

using UnityEngine;
using UnityEngine.AI;

namespace SaloonSlingers.Unity.Actor
{
    public class Enemy : MonoBehaviour, IActor
    {
        public Deck Deck { get; private set; }
        public event EventHandler Death;

        [SerializeField]
        private Transform handAttachTransform;
        [SerializeField]
        private float throwSpeed = 5f;

        private Transform currentTarget;

        private HitPoints targetHitPoints;
        private HitPoints hitPoints;
        private NavMeshAgent agent;
        private EnemyHandInteractableController currentHandController;
        private ISpawner<GameObject> cardSpawner;
        private Vector3 spawnPosition;
        private ISpawner<GameObject> handInteractableSpawner;
        private Color originalColor;
        private VisibilityDetector visibilityDetector;
        private GameManager gameManager;
        private DrawContext drawCtx;

        public void Reset()
        {
            hitPoints.Points.Reset();
            currentTarget = null;
            agent.stoppingDistance = 0f;
            currentHandController = null;
            CancelInvoke(nameof(Attack));
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            visibilityDetector = GetComponent<VisibilityDetector>();
            hitPoints = GetComponent<HitPoints>();

            cardSpawner = LevelManager.Instance.CardSpawner;
            handInteractableSpawner = LevelManager.Instance.HandInteractableSpawner;
            gameManager = GameManager.Instance;
        }

        private void Update()
        {
            //if (currentTarget != null)
            //{
            //    if (Vector3.Distance(currentTarget.position, transform.position) > visibilityDetector.SightDistance)
            //    {
            //        ReturnHome();
            //        return;
            //    }
            //    agent.SetDestination(currentTarget.position);
            //    FaceTarget();
            //    return;
            //}

            //if (!IsInvoking(nameof(LookForPlayer)))
            //    InvokeRepeating(nameof(LookForPlayer), visibilityIntervalSeconds, visibilityIntervalSeconds);
        }

        private void ReturnHome()
        {
            currentTarget = null;
            agent.SetDestination(spawnPosition);
            agent.stoppingDistance = 0f;
            CancelInvoke(nameof(Attack));
        }

        private void OnEnable()
        {
            Deck = new Deck().Shuffle();
            Deck.OnDeckEmpty += DeckEmptyHandler;
            spawnPosition = transform.position;
            if (targetHitPoints != null)
                targetHitPoints.Points.PointsDecreased += OnTargetHitPointsDecreased;
            hitPoints.Points.PointsDecreased += OnHitPointsDecreased;
        }

        private void OnDisable()
        {
            Deck.OnDeckEmpty -= DeckEmptyHandler;
            if (targetHitPoints != null)
                targetHitPoints.Points.PointsDecreased -= OnTargetHitPointsDecreased;
            hitPoints.Points.PointsDecreased -= OnHitPointsDecreased;
        }

        private void OnTargetHitPointsDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            if (e.After == 0)
            {
                currentTarget = null;
                ReturnHome();
            }
        }

        private void OnHitPointsDecreased(Points sender, ValueChangeEvent<uint> e)
        {
            //if (e.Before > e.After && e.After != 0) StartCoroutine(nameof(HitEffect));
            if (e.After == 0)
            {
                CancelInvoke(nameof(Attack));
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
            if (gameManager.Saloon.HouseGame.CanDraw(drawCtx))
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
