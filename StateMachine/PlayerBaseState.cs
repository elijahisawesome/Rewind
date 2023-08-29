using Godot;

public abstract class PlayerBaseState{
    protected StateMachine stateMachine;

    public abstract void EnterState(ref PlayerBaseState newState, StateMachine _stateMachine);
    public abstract void ExitState(PlayerBaseState oldState);
    public abstract void MaintainState(double delta);

}