using BehaviorDesigner.Runtime;

using SaloonSlingers.Unity.Actor;


public class SharedEnemy : SharedVariable<Enemy>
{
    public static implicit operator SharedEnemy(Enemy value)
    {
        return new SharedEnemy { mValue = value };
    }
}
