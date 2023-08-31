using Godot;

public abstract partial class BaseEnemy:CharacterBody3D{
    char baseState = 'I';


    public abstract bool CheckVision();

    public abstract void MoveToTarget(double delta);

    public void SwitchBaseState(char s){
        baseState = s;
    }

    public abstract void CheckDistanceFromTarget();

    public abstract void SwitchTarget();

    public abstract void MoveToLastKnownTargetPosition();

    public abstract void Attack();

    public abstract void Roam();

    public abstract void EvaluateTargets();

    public abstract bool EvaluateAttack();

}