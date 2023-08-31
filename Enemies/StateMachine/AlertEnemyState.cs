using Godot;

public class AlertEnemyState : EnemyBaseState
{   
    EnemyStateMachine stateMachine;
    BaseEnemy enemy;
    public override void EnterState(EnemyStateMachine e)
    {
        stateMachine = e;
        enemy = stateMachine.enemy;
        enemy.SwitchBaseState('A');
        GD.Print("Alert!");
    }

    public override void EnterState(ref PlayerBaseState newState, StateMachine _stateMachine)
    {

    }

    public override void ExitState(PlayerBaseState oldState)
    {

    }

    public override void MaintainState(double delta)
    {
        enemy.MoveToTarget(delta);
        if(Input.IsActionJustPressed("ResetAI")){
            stateMachine.changeState(stateMachine.idleState);
        }
    }
}