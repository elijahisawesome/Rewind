using Godot;

public abstract partial class BaseEnemy:CharacterBody3D{
    char baseState = 'I';


    public abstract bool CheckVision();

    public abstract void MoveToTarget(double delta);

    public void SwitchBaseState(char s){
        baseState = s;
    }
    public abstract void SetTargetLastPosition();

    public abstract bool HasArrivedAtLastPosition();

    public abstract void CheckDistanceFromTarget();

    public abstract void SwitchTarget();

    public abstract void MoveToLastKnownTargetPosition(double delta);

    public abstract void Attack();

    public abstract void Roam();

    public abstract void MoveToRoamTarget(double delta);

    public abstract void SetRoamTarget();

    public abstract void EvaluateTargets();

    public abstract bool EvaluateAttack();

}