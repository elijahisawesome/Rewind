using Godot;
using System;
using System.Collections.Generic;

public partial class AgroSystemTestProbe : TextEdit
{
	List<Test_Enemy.baseTargets> targets;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		targets =  GetParent().GetParent().GetParent<Test_Enemy>().targets;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		targets =  GetParent().GetParent().GetParent<Test_Enemy>().targets;
		String str = "";
		try{
			foreach(var tgt in targets){
				
			str += "target: " +tgt.target.ToString();
			str += '\n';
			str += "heat: " + tgt.heat.ToString();
			str += '\n';
			str += '\n';
		}
		Text = str;
		}
		catch(Exception e){

		}

	}
}
