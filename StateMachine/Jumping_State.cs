using Godot;
using System;

public class JumpingState : PlayerBaseState
{
    public override void EnterState(ref PlayerBaseState newState,StateMachine _stateMachine)
    {
        stateMachine = _stateMachine;
        //trigger animation;
        
        stateMachine.player.applyJumpForce();
        stateMachine.changeAnimation('J');
    }

    public override void ExitState(PlayerBaseState oldState)
    {
        //maybe do something

    }

    public override void MaintainState(double delta)
    {
        bool onGround = stateMachine.player.isOnGround();
        bool moving = !stateMachine.player.isStandingStill();
        stateMachine.player.applyGravity(delta);
        stateMachine.player.airMove(delta);
        if(onGround){
            if(moving){
                stateMachine.ChangeState(stateMachine.walkingState);
            }
            else{
                stateMachine.ChangeState(stateMachine.idleState);
            }
        }
    }
}