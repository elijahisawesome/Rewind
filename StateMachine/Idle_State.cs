using Godot;
using System;

public class IdleState : PlayerBaseState
{
    public override void EnterState(ref PlayerBaseState newState,StateMachine _stateMachine)
    {
        stateMachine = _stateMachine;
        //trigger animation;
        stateMachine.changeAnimation('I');
    }

    public override void ExitState(PlayerBaseState oldState)
    {
        //maybe do something

    }

    public override void MaintainState(double delta)
    {
        //do falling movement
        Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        bool jumping = Input.IsActionJustPressed("ui_accept");
        bool walking = false;
        for(int x = 0; x<=inputDir.Length(); x++){
            if(inputDir[x] != 0){
                walking = true;
                break;
            }
        }
        if(!stateMachine.player.isOnGround()){
            stateMachine.ChangeState(stateMachine.fallingState);
        }
        if(walking){
            //transition to walkingstate
            stateMachine.ChangeState(stateMachine.walkingState);
        }
        if(jumping){
            //transition to jumpingstate
            stateMachine.ChangeState(stateMachine.jumpingState);
        }
    }
}