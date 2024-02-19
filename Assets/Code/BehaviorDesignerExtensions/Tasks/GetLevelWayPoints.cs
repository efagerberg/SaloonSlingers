using System.Linq;

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity;

public class GetLevelWayPoints : Action
{
    public SharedGameObjectList WayPoints;

    public override TaskStatus OnUpdate()
    {
        var wayPoints = LevelManager.Instance.EnemyManager.EnemyWayPoints;
        if (wayPoints.Count == 0) return TaskStatus.Failure;
        WayPoints.Value = wayPoints.Select(sp => sp.gameObject).ToList();
        return TaskStatus.Success;
    }
}
