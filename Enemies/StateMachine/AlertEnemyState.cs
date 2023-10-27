

using Godot;

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
        enemy.EvaluateTargets();
        bool visible = enemy.hasTarget();
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
                stateMachine.changeState(stateMachine.searchingState);
                enemy.SetTargetLastPosition();
            }
            else{
                
            }
        }
       
        if(Godot.Input.IsActionJustPressed("ResetAI")){
            stateMachine.changeState(stateMachine.idleState);
        }
    }
}