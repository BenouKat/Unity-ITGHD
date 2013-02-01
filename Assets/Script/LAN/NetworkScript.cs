using UnityEngine;
using System.Collections;
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
		cls.stateScene = LANConnexionState.INITIALIZESCENE;
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		
		Debug.Log(player.externalIP + " : " + player.externalPort + " connected");

		//Recupération du joueur et association dans LANManager
		//Instanciation du game Object de connexion
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		
	}
	
	void OnConnectedToServer()
	{
		LANManager.Instance.actualIP = Network.player.ipAddress;
		LANManager.Instance.actualPort = Network.player.port;
		
		cls.stateScene = LANConnexionState.INITIALIZESCENE;
		
		//Recupération de tous les joueurs joueur et association dans LANManager
		//Instanciation du game Object de connexion
		
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		
	}
	
	void OnFailedToConnect(NetworkConnectionError nce)
	{
		
	}
}
