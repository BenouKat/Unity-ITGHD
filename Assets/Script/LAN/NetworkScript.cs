using UnityEngine;
using System.Collections;

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
			Network.InitializeServer(8, LANManager.Instance.actualPort, false);
		}else
		{
			Network.Connect(LANManager.Instance.IPRequest, LANManager.Instance.portRequest); //TODO
		}
	}
	
	void OnServerInitialized()
	{
		LANManager.Instance.actualIP = Network.player.externalIP;
		Network.maxConnections = 8;
		Debug.Log("Initialized ! " + LANManager.Instance.actualIP + " : " + LANManager.Instance.actualPort);
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		
		Debug.Log(player.externalIP + " : " + player.externalPort + " connected");
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
