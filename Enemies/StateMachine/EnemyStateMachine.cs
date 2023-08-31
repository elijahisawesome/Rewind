using System;
using Godot;

public class EnemyStateMachine{
    EnemyBaseState currentState;
    EnemyBaseState previousState;
    public BaseEnemy enemy;

    public readonly AlertEnemyState alertState = new AlertEnemyState();
    
    public readonly IdleEnemyState idleState = new IdleEnemyState();
    
    public readonly SearchingEnemyState searchingState = new SearchingEnemyState();

    public EnemyStateMachine(BaseEnemy _enemy){
        enemy = _enemy;
        currentState = idleState;
        idleState.EnterState(this);
    }
    public void process(double delta){
        currentState.MaintainState(delta);
    }

    public void changeState(EnemyBaseState newState){
        previousState = currentState;
        previousState.ExitState(currentState);
        currentState = newState;
        currentState.EnterState(this);
    }
    
}
