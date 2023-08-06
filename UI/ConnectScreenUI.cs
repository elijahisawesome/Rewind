using Godot;
using System;

public partial class ConnectScreenUI : Node
{
	Button button;
	TextEdit tEdit;
	MultiplayerManager MPM;
	public bool connected=false;
	bool hosting = false;
	public override void _Ready()
	{
		button = GetNode<Button>("SubViewportContainer/HBoxContainer/VBoxContainer/Button");
		tEdit = GetNode<TextEdit>("SubViewportContainer/HBoxContainer/VBoxContainer/TextEdit");
		MPM = GetParent().GetParent().GetNode<MultiplayerManager>("MultiplayerManager");
	}

	public override void _Process(double delta)
	{
	}
	public void ConnectAttempt(){
		MPM.setHostAddress(tEdit.Text);
		MPM.connectClient(tEdit.Text);
	}
	public bool isConnected(){
		return connected;
	}
	public bool isHosting(){
		return hosting;
	}
	private void _on_button_pressed(){
		GD.Print(connected);
		ConnectAttempt();
		//TODO Make this actually check if connected
		connected = true;
		this.QueueFree();
	}
	private void _on_host_button_pressed(){
		GD.Print("Hosting");
		MPM.setHosting();
		hosting = true;
		this.QueueFree();
	}
}
