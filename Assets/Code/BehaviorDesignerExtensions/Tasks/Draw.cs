using System;

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Core;
using SaloonSlingers.Unity;
using SaloonSlingers.Unity.Actor;

using UnityEngine;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class Draw : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedTransform AttachTransform;
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
            if (ReturnedObject.Value == null && AttachTransform.Value != null)
            {
                GameObject clone = SpawnInteractable();
                ReturnedObject.Value = clone.GetComponent<EnemyHandInteractableController>();
                ReturnedObject.Value.transform.SetParent(AttachTransform.Value, false);
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
            ControllerSwapper swapper = spawned.GetComponent<ControllerSwapper>();
            swapper.SetController(ControllerTypes.ENEMY);
            return spawned;
        }
    }
}
