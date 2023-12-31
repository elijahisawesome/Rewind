public class SearchingEnemyState : EnemyBaseState
{
    EnemyStateMachine stateMachine;
    BaseEnemy enemy;
    public override void EnterState(EnemyStateMachine e)
    {
        stateMachine = e;
        enemy = stateMachine.enemy;
        enemy.targetID = -1;
        enemy.ResetRoamCounter();
        enemy.switchStateChar('S');
        Godot.GD.Print("searching...");
    }

    public override void EnterState(ref PlayerBaseState newState, StateMachine _stateMachine)
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState(PlayerBaseState oldState)
    {
        
    }

    public override void MaintainState(double delta)
    {
        enemy.EvaluateTargets();
        bool visible = enemy.hasTarget();
        
        if(visible){
            stateMachine.changeState(stateMachine.alertState);
        }
        else if (enemy.HasArrivedAtLastPosition()){
            if(enemy.SearchTimerComplete()){
                if(enemy.DoneSearching()){
                    stateMachine.changeState(stateMachine.idleState);
                    return;
                }
                else{
                    if (enemy.SetRoamTarget()){
                        enemy.IterateRoamCounter();
                        enemy.StartTimer();
                    }
                }
                
            }
            else{
                enemy.LookAround(delta);
            }
            
        }
        else{
            enemy.MoveToRoamTarget(delta);
        }
    }
}