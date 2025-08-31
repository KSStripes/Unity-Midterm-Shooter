using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController _enemy;

    // Constructor
    public EnemyState(EnemyController enemy)
    {
        _enemy = enemy;
    }

    // Each state must implement these methods
    public abstract void OnStateEnter();
    public abstract void OnStateUpdate(); // whilst enemy is in this state
    public abstract void OnStateExit();
}
