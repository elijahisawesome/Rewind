

public class AlertEnemyState : EnemyBaseState
{   
    EnemyStateMachine stateMachine;
    BaseEnemy enemy;
    public override void EnterState(EnemyStateMachine e)
    {
        stateMachine = e;
        enemy = stateMachine.enemy;
        enemy.switchStateChar('A');
        enemy.SetTargetLastPosition();
        Godot.GD.Print("Alert!");
    }

    public override void EnterState(ref PlayerBaseState newState, StateMachine _stateMachine)
    {

    }

    public override void ExitState(PlayerBaseState oldState)
    {

    }

    public override void MaintainState(double delta)
    {
        bool visible = enemy.CheckVision();
        bool Arrived = enemy.HasArrivedAtLastPosition();
        if(!Arrived){
            if(visible){
                enemy.MoveToTarget(delta);
                enemy.SetTargetLastPosition();
            }
            else{
                enemy.MoveToLastKnownTargetPosition(delta);
            }
        }
        else{
            if(visible){
                enemy.Attack();
                enemy.SetTargetLastPosition();
            }
            else{
                stateMachine.changeState(stateMachine.searchingState);
            }
        }
       
        if(Godot.Input.IsActionJustPressed("ResetAI")){
            stateMachine.changeState(stateMachine.idleState);
        }
    }
}