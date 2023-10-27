public class AttackingEnemyState : EnemyBaseState
{
    int countdown = 60;
    EnemyStateMachine eStateMachine;
    BaseEnemy enemy;
    public override void EnterState(EnemyStateMachine e)
    {
        eStateMachine = e;
        enemy = eStateMachine.enemy;
        enemy.switchStateChar('A');
        enemy.targetID = -1;
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
        countdown--;
        if(countdown <= 0){
            eStateMachine.changeState(eStateMachine.idleState);
        }  
    }

}