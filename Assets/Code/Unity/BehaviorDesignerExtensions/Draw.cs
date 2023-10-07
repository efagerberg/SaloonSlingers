using System;

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.Unity
{
    public class SharedEnemyHandInteractableController : SharedVariable<EnemyHandInteractableController>
    {
        public static implicit operator SharedEnemyHandInteractableController(EnemyHandInteractableController value)
        {
            return new SharedEnemyHandInteractableController { mValue = value };
        }
    }

    public class Draw : BehaviorDesigner.Runtime.Tasks.Action
    {
        public Transform AttachTransform;
        public SharedEnemyHandInteractableController ReturnedObject;

        private readonly Deck deck = new Deck().Shuffle();
        private DrawContext drawCtx;

        public override TaskStatus OnUpdate()
        {
            DoDraw();
            return TaskStatus.Success;
        }

        private void DoDraw()
        {
            if (ReturnedObject.Value == null)
            {
                GameObject clone = SpawnInteractable();
                ReturnedObject.Value = clone.GetComponent<EnemyHandInteractableController>();
                ReturnedObject.Value.transform.SetParent(AttachTransform, false);
            }
            HandProjectile projectile = ReturnedObject.Value.GetComponent<HandProjectile>();
            drawCtx.Hand = projectile.Cards;
            drawCtx.Evaluation = projectile.HandEvaluation;
            drawCtx.Deck = deck;
            if (GameManager.Instance.Saloon.HouseGame.CanDraw(drawCtx))
            {
                ReturnedObject.Value.Draw(deck, LevelManager.Instance.CardSpawner.Spawn);
                return;
            }
        }

        private GameObject SpawnInteractable()
        {
            GameObject spawned = LevelManager.Instance.HandInteractableSpawner.Spawn();
            HandProjectile projectile = spawned.GetComponent<HandProjectile>();
            projectile.Death += DespawnHandProjectile;
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.ENEMY);
            return spawned;
        }

        private void DespawnHandProjectile(object sender, EventArgs _)
        {
            var instance = sender as GameObject;
            var projectile = instance.GetComponent<HandProjectile>();
            foreach (ICardGraphic c in projectile.CardGraphics)
                c.Kill();
            projectile.Death -= DespawnHandProjectile;
        }
    }
}
