public class IdleEnemyState : EnemyBaseState
{
    EnemyStateMachine eStateMachine;
    BaseEnemy enemy;
    public override void EnterState(EnemyStateMachine e)
    {
        eStateMachine = e;
        enemy = eStateMachine.enemy;
        enemy.switchStateChar('I');
        Godot.GD.Print("Idle");
    }

    public override void EnterState(ref PlayerBaseState newState, StateMachine _stateMachine)
    {
        throw new System.NotImplementedException();
    }


    public override void ExitState(PlayerBaseState oldState)
    {
        //throw new System.NotImplementedException();
    }

    public override void MaintainState(double delta)
    {
       if( eStateMachine.enemy.CheckVision()){
            eStateMachine.enemy.SwitchTarget();
            eStateMachine.changeState(eStateMachine.alertState);
            return;
       }

    }

}