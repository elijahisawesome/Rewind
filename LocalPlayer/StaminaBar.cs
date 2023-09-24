using Godot;
using System;
using System.Drawing;

public partial class StaminaBar : TextureProgressBar
{
	// Called when the node enters the scene tree for the first time.
	float depleteRate = 100f;
	int rotationRate=3;
	int pos  =0;
	float rechargeRate = 15f;
	float viewWidth;
	float viewHeight;
	double subValue;
	float horizPercent;
	public bool depleting = false;
	public bool burnout = false;
	MarginContainer parent;
	UISpawnable particleSpawner;
	public bool recharged(){
		return Value >= 100f;
	}
	public override void _Ready()
	{
		parent = GetParent<MarginContainer>();
		viewWidth = (float)ProjectSettings.GetSetting("display/window/size/viewport_width");
		viewHeight = (float)ProjectSettings.GetSetting("display/window/size/viewport_height");
		subValue = 100f;
		Value = 100f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public override void _PhysicsProcess(double delta){
		if(!depleting || burnout){
			subValue += delta*rechargeRate;
			if(subValue > 100){
				subValue = 100;
			}
			Value = subValue;
			if(recharged()){
				burnout = false;
			}
			else{
				if(burnout){
					particleSpawner.GetChild<GpuParticles2D>(0).ProcessMaterial.Set("emission_box_extents",new Vector3(Size.X*(float)Value/100/2,Position.Y-10f,1f));
					particleSpawner.setPos(new Vector2((float)(Size.X*subValue/100/2),Position.Y-10f));
					setRandomRotation();
				}	
			}
		}
		else{
			Value -= delta*depleteRate;
			subValue = Value;
			
		}
		if(subValue <= 0){
			burnout = true;
			string spawnablePath = "res://2DArt/HUDElements/UISpawnable.tscn";
			PackedScene spawnablePackedScene = GD.Load<PackedScene>(spawnablePath);
			var elem = spawnablePackedScene.Instantiate<UISpawnable>();
			particleSpawner = elem;
			
			//particleSpawner.GetChild<GpuParticles2D>(0).LocalCoords = true;
			//elem.Position = new Vector2 ((float)(viewWidth/2), Position.Y-100f); 
			parent.CallDeferred("add_child",elem);
		}
		
		
	}
	public void panicing(){
		depleting = !depleting;
	}
	void setRandomRotation(){
		pos++;
		pos = pos%rotationRate;
		if(pos ==0){
			PivotOffset = new Vector2(Size.X/2,Size.Y/2);
			var fac = new Random();
			var Fac = fac.NextSingle()*1f/2/(float)Math.PI - .5f/2/(float)Math.PI;
			Rotation = Fac/5f;
		}
	}


}
