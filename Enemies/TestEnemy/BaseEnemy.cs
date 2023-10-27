using Godot;

public abstract partial class BaseEnemy:CharacterBody3D{
    char stateChar = 'I';
    public int id;
    public int targetID;
    public abstract bool hasTarget();
    public abstract void setRotation(string str);
    public abstract bool CheckVision();
    public abstract void MoveToTarget(double delta);
    public abstract void currentServerPos(enemyMovePacket v);

    public void switchStateChar(char s){
        stateChar = s;
    }
    public char getStateChar(){
        return stateChar;
    }
    public abstract void SetTargetLastPosition();
    public abstract void setTargets(enemyMovePacket emp);

    public abstract bool HasArrivedAtLastPosition();

    public abstract void CheckDistanceFromTarget();

    public abstract void SwitchTarget();

    public abstract void MoveToLastKnownTargetPosition(double delta);

    public abstract void IterateRoamCounter();

    public abstract bool SearchTimerComplete();
    public abstract void ResetRoamCounter();
    public abstract bool DoneSearching();

    public abstract void LookAround(double delta);

    public abstract void Attack();

    public abstract void Roam();

    public abstract void MoveToRoamTarget(double delta);

    public abstract bool SetRoamTarget();

    public abstract void EvaluateTargets();

    public abstract bool EvaluateAttack();

    public abstract void StartTimer();

}