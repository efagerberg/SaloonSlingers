using System.Linq;

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

using SaloonSlingers.Unity;

public class GetLevelWayPoints : Action
{
    public SharedGameObjectList WayPoints;

    public override TaskStatus OnUpdate()
    {
        if (LevelManager.Instance.EnemySpawner.SpawnPoints.Count == 0)
            return TaskStatus.Failure;
        WayPoints.Value = LevelManager.Instance.EnemySpawner.SpawnPoints.Select(sp => sp.gameObject).ToList();
        return TaskStatus.Success;
    }
}
