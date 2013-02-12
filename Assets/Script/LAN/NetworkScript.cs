using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class NetworkScript : MonoBehaviour {
	
	private ConnectingLANScene cls;
	
	// Use this for initialization
	void Start () {
		cls = GetComponent<ConnectingLANScene>();
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	
	public void StartNetwork()
	{
		if(LANManager.Instance.isCreator)
		{
			var nce = Network.InitializeServer(8, LANManager.Instance.actualPort, false);
			LANManager.Instance.errorToDisplay = LANManager.errorToString(nce);
			if(!String.IsNullOrEmpty(LANManager.Instance.errorToDisplay)){
				cls.stateScene = LANConnexionState.FAIL;
			}
		}else
		{
			Network.Connect(LANManager.Instance.IPRequest, LANManager.Instance.portRequest); //TODO
		}
	}
	
	void OnServerInitialized()
	{
		if(Network.player.externalIP.Contains("UNASSIGNED"))
		{
			LANManager.Instance.actualIP = Network.player.ipAddress;
		}else{
			LANManager.Instance.actualIP = Network.player.externalIP;
		}
		
		Network.maxConnections = 8;
		Debug.Log("Initialized ! " + LANManager.Instance.actualIP + " : " + LANManager.Instance.actualPort);
		
		LANManager.Instance.players.Add(Network.player, new CublastPlayer(ProfileManager.Instance.currentProfile.name));
		cls.addHitNet(Network.player);
		cls.stateScene = LANConnexionState.INITIALIZESCENE;
	}
	
	//server side
	void OnPlayerConnected(NetworkPlayer player)
	{
		
		Debug.Log(player.externalIP + " : " + player.externalPort + " connected");
		
		if(!LANManager.Instance.players.ContainsKey(player))
		{
			LANManager.Instance.players.Add(player, new CublastPlayer("Waiting..."));
		}
		
		cls.addHitNet(player);
	}
	
	//server side
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log(player.externalIP + " : " + player.externalPort + " disconnected");
		
		var nameDisconnected = LANManager.Instance.players[player].name;
		
		LANManager.Instance.players.Remove(player);
		
		cls.removeHitNet(player);
		
		sendNameOfAllPlayers(nameDisconnected);
	}
	
	
	//client side
	void OnConnectedToServer()
	{
		LANManager.Instance.actualIP = Network.player.ipAddress;
		LANManager.Instance.actualPort = Network.player.port;
		
		networkView.RPC("getNameOnPlayerConnected", RPCMode.Server, Network.player, ProfileManager.Instance.currentProfile.name);
		
		
		cls.stateScene = LANConnexionState.INITIALIZESCENE;
		
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		
	}
	
	void OnFailedToConnect(NetworkConnectionError nce)
	{
		
	}
	
	//Only called by server from players
	void getNameOnPlayerConnected(NetworkPlayer player, string name)
	{
		if(!LANManager.Instance.players.ContainsKey(player))
		{
			LANManager.Instance.players.Add(player, new CublastPlayer(name));
		}else
		{
			LANManager.Instance.players[player].name = name;
		}
		
		sendNameOfAllPlayers(name);
	}
	
	//Only called by client from server
	void sendNameOfAllPlayers(string nameEvent)
	{
		var players = new string[LANManager.Instance.players.Count];
		for(int i = 0; i < LANManager.Instance.players.Count; i++)
		{
			players[i] = LANManager.Instance.players.ElementAt(i).Value.name;	
		}
		
		networkView.RPC("getNameOfAllPlayers", RPCMode.Others, players, nameEvent);	
	}
	
	
	void getNameOfAllPlayers(string[] players, string nameEvent)
	{
		cls.hitNetClient.Clear();
		
		for(int i = 0; i < players.Length; i++)
		{
			cls.associateCubeToString(players[i]);	
		}
		
		cls.checkForVerificationConnected(nameEvent);
	}
}
