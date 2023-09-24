using Godot;
using System;

public class PanicState : PlayerBaseState
{
    public override void EnterState(ref PlayerBaseState previousState,StateMachine _stateMachine)
    {
        //trigger animation;
        stateMachine =_stateMachine;
        stateMachine.player.panicToggle();
        stateMachine.changeAnimation('P');
    }

    public override void ExitState(PlayerBaseState newState)
    {
        //maybe do something

    }

    public override void MaintainState(double delta)
    {
        //do falling movement
        bool panicingSwitch = Input.IsActionJustPressed("Panic") || stateMachine.player.panicInBurnout();
        
        
        if(panicingSwitch){
            stateMachine.player.panicToggle();
            stateMachine.ChangeState(stateMachine.walkingState);
        }
        else if(stateMachine.player.isOnGround() && Input.IsActionJustPressed("Jump")){
            stateMachine.player.applyJumpForce();
        }
        else if(!stateMachine.player.isOnGround()){
            stateMachine.player.applyGravity(delta);
        }
        stateMachine.player.Panic(delta);
    }
}
