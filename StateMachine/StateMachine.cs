
public class StateMachine{
    public readonly FallingState fallingState;
    public readonly IdleState idleState;

    public readonly JumpingState jumpingState;
    public readonly WalkingState walkingState;
    public readonly DeathState deathState;

    public PlayerBaseState currentState;
    public PlayerBaseState previousState;
    public Player player;
    public StateMachine(Player _player){
        player = _player;
        fallingState=new FallingState();
        idleState = new IdleState();
        jumpingState = new JumpingState();
        walkingState = new WalkingState();
        deathState = new DeathState();

        currentState = idleState;
        idleState.EnterState(ref currentState, this);
    }

    public void ChangeState(PlayerBaseState newState){
        previousState = currentState;
        previousState.ExitState(currentState);
        currentState = newState;
        currentState.EnterState(ref previousState, this);
    }
    public void process(double delta){
        currentState.MaintainState(delta);
    }
    public void changeAnimation(char c){

    }
}