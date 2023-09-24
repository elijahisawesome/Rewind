using Godot;
using System;

public partial class PlayerStatusFace : TextureRect
{
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		string Path = "res://2DArt/HUDElements/Faces/disconnected-icon.png";
		applyFace(Path);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
	public void setFace(char c){
		string Path;
		switch(c){
			case 'E':
				Path = "res://2DArt/HUDElements/Faces/dead-icon.png";
				break;
			case 'P':
				Path = "res://2DArt/HUDElements/Faces/default-icon.png";
				break;
			default:
				Path = "res://2DArt/HUDElements/Faces/bonus-icon.png";
				break;
		}
		applyFace(Path);
	}
	void applyFace(string s){
		Texture = GD.Load<Texture2D>(s);
	}
}
