using UnityEngine;
using System.Collections;

public class NetworkScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void StartNetwork()
	{
		
		
		if(LANManager.Instance.isCreator)
		{
			Network.InitializeServer(8, 25000, !Network.HavePublicAddress());
		}else
		{
			Network.Connect(LANManager.Instance.IPRequest, LANManager.Instance.portRequest); //TODO
		}
	}
	
	void OnServerInitialized()
	{
		LANManager.Instance.actualIP = Network.player.ipAddress;
		LANManager.Instance.actualPort = Network.player.port;
		Network.maxConnections = 8;
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		//Envoi/Recupération du profil
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
		
		//Envoi/Recupération du profil
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
