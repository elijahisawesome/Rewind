using Godot;
using System;

public partial class himbo_base : Node3D
{
	// Called when the node enters the scene tree for the first time.
	AnimationPlayer pl;
	public override void _Ready()
	{
		pl = GetChild<AnimationPlayer>(1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/*
	public override void _Input(InputEvent iEvent){
		if(Input.IsActionPressed("ui_home")){
			pl.Play("Idle");
		}
		if(Input.IsActionPressed("ui_end")){
			pl.Play("Running");
		}
	}
	*/
	public void _on_animation_player_animation_finished(string animName){
		GD.Print(animName);
		if(animName == "Jumping"){
			pl.Play("Falling");
		}
	}  
	public void _on_animation_player_animation_finished(){
		if(pl.CurrentAnimation == "Jumping"){
			pl.Play("Falling");
		}
	}  
	public void swtichAnimation(Char inputChar){
		switch(inputChar){
			case 'I':
				pl.Play("Idle");
				break;
			case 'R':
				pl.Play("Walk");
				break;
			case 'J':
				if(pl.CurrentAnimation != "Falling"){
					pl.Play("Jumping");
				}
				break;
			case 'F':
				pl.Play("Falling");
				break;
			case 'E':

				break;
			case 'P':
				pl.Play("Walk");
				break;
			default:
				break;
		}
	}

}
