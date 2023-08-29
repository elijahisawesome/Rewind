using Godot;
using System;

public class WalkingState : PlayerBaseState
{
    public override void EnterState(ref PlayerBaseState previousState,StateMachine _stateMachine)
    {
        //trigger animation;
        stateMachine =_stateMachine;

        stateMachine.changeAnimation('R');
    }

    public override void ExitState(PlayerBaseState newState)
    {
        //maybe do something

    }

    public override void MaintainState(double delta)
    {
        //do falling movement
        bool jumping = Input.IsActionJustPressed("ui_accept");
        stateMachine.player.walk(delta);
        
        if(jumping){
            stateMachine.ChangeState(stateMachine.jumpingState);
        }
        else if(!stateMachine.player.isOnGround()){
            stateMachine.ChangeState(stateMachine.fallingState);
        }
        else if(stateMachine.player.isStandingStill()){
            stateMachine.ChangeState(stateMachine.idleState);
        }
    }
}