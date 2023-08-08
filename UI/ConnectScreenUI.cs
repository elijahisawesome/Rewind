using Godot;
using System;

using System.Net.Sockets;
using System.Net;


public partial class ConnectScreenUI : Node
{
	Button button;
	TextEdit tEdit;
	MultiplayerManager MPM;
	public bool connected=false;
	bool hosting = false;

	UdpClient test1;
	UdpClient test2;
	public override void _Ready()
	{
		button = GetNode<Button>("SubViewportContainer/HBoxContainer/VBoxContainer/Button");
		tEdit = GetNode<TextEdit>("SubViewportContainer/HBoxContainer/VBoxContainer/TextEdit");
		MPM = GetParent().GetParent().GetNode<MultiplayerManager>("MultiplayerManager");
		
		/*
		byte[] sendBytesTest = System.Text.Encoding.ASCII.GetBytes("Is anybody there?");
		byte[] bytes = new byte[500];
		test1 = new UdpClient();
		test2 = new UdpClient();

		//test1.Connect("127.0.0.1", 11000);
		IPEndPoint RemoteIpEndPoint = new  IPEndPoint(System.Net.IPAddress.Loopback, 11001);

		test2.Client.Bind(RemoteIpEndPoint);

		//test2.Connect("127.0.0.1", 11001);
		IPEndPoint remoteIpEndPoint = new  IPEndPoint(System.Net.IPAddress.Loopback, 11000);
		test1.Client.Bind(remoteIpEndPoint);

		test1.Client.SendTo(sendBytesTest, RemoteIpEndPoint);
		int len = test2.Client.Receive( bytes);
		foreach(var bytee in bytes)
		{
			GD.Print(bytee);
		}
		GD.Print(bytes);
		*/
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
