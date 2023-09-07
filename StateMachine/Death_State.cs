using Godot;
using System;

public class DeathState : PlayerBaseState
{
    public override void EnterState(ref PlayerBaseState newState,StateMachine _stateMachine)
    {
        //trigger animation;
        stateMachine = _stateMachine;
        stateMachine.changeAnimation('E');

    }

    public override void ExitState(PlayerBaseState oldState)
    {
        //maybe do something

    }

    public override void MaintainState(double delta)
    {
        stateMachine.player.NoclipMove(delta);
        if(Input.IsActionJustPressed("Enter")){
            stateMachine.player.respawn();
        }
        
    }
}