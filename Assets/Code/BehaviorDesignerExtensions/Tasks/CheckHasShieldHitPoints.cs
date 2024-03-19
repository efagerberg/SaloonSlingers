﻿using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Core;
using SaloonSlingers.Unity.Actor;

namespace SaloonSlingers.BehaviorDesignerExtensions
{
    public class CheckHasShieldHitPoints : Conditional
    {
        public IReadOnlyAttribute HitPoints { get; set; }

        public override void OnStart()
        {
            if (HitPoints == null)
                HitPoints = GetComponent<Enemy>().ShieldHitPoints;
        }

        public override TaskStatus OnUpdate()
        {
            return HitPoints.Value > 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
