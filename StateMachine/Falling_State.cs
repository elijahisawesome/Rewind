using Godot;
using System;

public class FallingState : PlayerBaseState
{
    public override void EnterState(ref PlayerBaseState newState,StateMachine _stateMachine)
    {
        //trigger animation;
        stateMachine = _stateMachine;
        stateMachine.changeAnimation('F');

    }

    public override void ExitState(PlayerBaseState oldState)
    {
        //maybe do something

    }

    public override void MaintainState(double delta)
    {
        //do falling movement
        stateMachine.player.airMove(delta);
        if(stateMachine.player.isOnGround()){
            stateMachine.ChangeState(stateMachine.walkingState);
            return;
        }

        stateMachine.player.applyGravity(delta);
    }
}