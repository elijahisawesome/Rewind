using Godot;
using System;

using System.Net.Sockets;
using System.Net;


public partial class ConnectScreenUI : Node
{
	Button button;
	LineEdit lEdit;
	MultiplayerManager MPM;
	public bool connected=false;
	bool hosting = false;

	UdpClient test1;
	UdpClient test2;
	public override void _Ready()
	{
		button = GetNode<Button>("VBoxContainer/HBoxContainer/Button");
		lEdit = GetNode<LineEdit>("VBoxContainer/HBoxContainer2/LineEdit");
		MPM = GetParent().GetParent().GetNode<MultiplayerManager>("MultiplayerManager");
		
	}

	public override void _Process(double delta)
	{
	}
	public void ConnectAttempt(){
		MPM.setHostAddress(lEdit.Text);
		MPM.connectClient(lEdit.Text);
	}
	public bool isConnected(){
		return connected;
	}
	public bool isHosting(){
		return hosting;
	}
	private void _on_button_pressed(){
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
