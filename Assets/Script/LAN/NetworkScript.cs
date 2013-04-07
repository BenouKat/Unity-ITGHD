using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class NetworkScript : MonoBehaviour {
	
	private ConnectingLANScene cls;
	
	public string ipAsked;
	public int portAsked;
	public bool server;
	
	void Test()
	{
		TextManager.Instance.LoadTextFile();
		ProfileManager.Instance.CreateTestProfile();
		LANManager.Instance.modeLANselected = LANMode.SCORETOURN;
		LANManager.Instance.hostSystem = 1;
		LANManager.Instance.songDiffSystem = 2;
		LANManager.Instance.roundNumber = 10;
		if(server)
		{
			LANManager.Instance.isCreator = true;	
			LANManager.Instance.actualPort = portAsked;
		}else
		{
			LANManager.Instance.isCreator = false;
			LANManager.Instance.portRequest = portAsked;
		}
		LANManager.Instance.IPRequest = ipAsked;
	}
	
	void TestShort()
	{
		ProfileManager.Instance.CreateTestProfile();
		LoadManager.Instance.Loading();
		
	}
	// Use this for initialization
	void Start () {
		TestShort();
		cls = GetComponent<ConnectingLANScene>();
		GetComponent<ChatScript>().activeChat(false);
		LANManager.Instance.statut = LANStatut.ROOM;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnApplicationQuit()
	{
		Network.Disconnect();
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
			Network.Connect(LANManager.Instance.IPRequest, LANManager.Instance.portRequest);
		}
	}
	
	void OnServerInitialized()
	{		
		
		if(Network.player.externalIP.Contains("UNASSIGNED"))
		{
			if(Network.player.ipAddress == "0.0.0.0")
			{
				LANManager.Instance.actualIP = "localhost";	
			}else
			{
				LANManager.Instance.actualIP = Network.player.ipAddress;
			}
		}else{
			LANManager.Instance.actualIP = Network.player.externalIP;
		}
		
		Network.maxConnections = 8;
		Debug.Log("Initialized ! " + LANManager.Instance.actualIP + " : " + LANManager.Instance.actualPort);
		
		LANManager.Instance.players.Add(Network.player, new CublastPlayer(ProfileManager.Instance.currentProfile.name, ProfileManager.Instance.currentProfile.idFile));
		LANManager.Instance.players[Network.player].packName = LoadManager.Instance.getAllPackName();
		cls.addHitNet(Network.player);
		GetComponent<ChatScript>().activeChat(true);
		LANManager.Instance.dataArrived = true;
		LANManager.Instance.getTheRightToChange = LANManager.Instance.songDiffSystem == 1;
		cls.stateScene = LANConnexionState.INITIALIZESCENE;
	}
	
	//server side
	void OnPlayerConnected(NetworkPlayer player)
	{
		
		Debug.Log(player.externalIP + " : " + player.externalPort + " connected");
		
		if(!LANManager.Instance.players.ContainsKey(player))
		{
			LANManager.Instance.players.Add(player, new CublastPlayer("Waiting...", ""));
		}
		
		networkView.RPC("sendInfoPartyToPlayer", RPCMode.Others, (int)LANManager.Instance.modeLANselected, LANManager.Instance.roundNumber, LANManager.Instance.hostSystem, LANManager.Instance.songDiffSystem);
		networkView.RPC("changeRightDifficulty", player, LANManager.Instance.songDiffSystem == 1);
		cls.addHitNet(player);
	}
	
	//server side
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log(player.externalIP + " : " + player.externalPort + " disconnected");
		
		var nameDisconnected = LANManager.Instance.players[player].name;
		var idDisconnected = LANManager.Instance.players[player].idFile;
		
		GetComponent<ChatScript>().sendDirectMessage("Info", nameDisconnected + " has left the node");
		
		LANManager.Instance.players.Remove(player);
		
		cls.removeHitNet(player);
		
		sendNameOfAllPlayers(nameDisconnected, idDisconnected);
	}
	
	
	//client side
	void OnConnectedToServer()
	{
		LANManager.Instance.actualIP = Network.player.ipAddress;
		LANManager.Instance.actualPort = Network.player.port;
		
		networkView.RPC("getInfoOnPlayerConnected", RPCMode.Server, Network.player, ProfileManager.Instance.currentProfile.name, ProfileManager.Instance.currentProfile.idFile, ProfileManager.Instance.currentProfile.victoryOnline, LoadManager.Instance.getAllPackName());
		
		GetComponent<ChatScript>().activeChat(true);
		GetComponent<ChatScript>().sendDirectMessage("Info", ProfileManager.Instance.currentProfile.name + " has joined the node");
		
		cls.stateScene = LANConnexionState.INITIALIZESCENE;
		
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		GetComponent<ChatScript>().activeChat(false);
		LANManager.Instance.errorToDisplay = info.ToString();
		cls.stateScene = LANConnexionState.FAIL;
	}
	
	void OnFailedToConnect(NetworkConnectionError nce)
	{
		LANManager.Instance.errorToDisplay = nce.ToString();
		cls.stateScene = LANConnexionState.FAIL;
	}
	
	//Only called by server from players
	[RPC]
	void getInfoOnPlayerConnected(NetworkPlayer player, string name, string id, int vict, string packname)
	{
		if(!LANManager.Instance.players.ContainsKey(player))
		{
			LANManager.Instance.players.Add(player, new CublastPlayer(name, id));
			LANManager.Instance.players[player].packName = packname;
			LANManager.Instance.players[player].victoryOnline = vict;
		}else
		{
			LANManager.Instance.players[player].name = name;
			LANManager.Instance.players[player].idFile = id;
			LANManager.Instance.players[player].packName = packname;
			LANManager.Instance.players[player].victoryOnline = vict;
		}
		
		
		sendNameOfAllPlayers(name, id);
	}
	
	[RPC]
	void sendInfoPartyToPlayer(int LANModeSelec, int roundNumber, int hostSystem, int songDiffSystem)
	{
		LANManager.Instance.modeLANselected = (LANMode)LANModeSelec;
		LANManager.Instance.roundNumber = roundNumber;
		LANManager.Instance.hostSystem = hostSystem;
		LANManager.Instance.songDiffSystem = songDiffSystem;
		LANManager.Instance.dataArrived = true;
	}
	
	//Only called by client from server
	[RPC]
	void sendNameOfAllPlayers(string nameEvent, string id)
	{
		string players = "";
		string playersID = "";
		string playersReady = "";
		string playersVict = "";
		for(int i = 0; i < LANManager.Instance.players.Count; i++)
		{
			players += LANManager.Instance.players.ElementAt(i).Value.name;
			playersID += LANManager.Instance.players.ElementAt(i).Value.idFile;
			playersReady += LANManager.Instance.players.ElementAt(i).Value.isReady ? "1" : "0";
			playersVict += LANManager.Instance.players.ElementAt(i).Value.victoryOnline;
			if(i < LANManager.Instance.players.Count - 1)
			{
				players += ";";	
				playersID += ";";
				playersReady += ";";
				playersVict += ";";
			}
		}
		
		networkView.RPC("getNameOfAllPlayers", RPCMode.Others, players, playersID, playersReady, playersVict, nameEvent, id);	
	}
	
	[RPC]
	void getNameOfAllPlayers(string players, string playersID, string playersReady, string playersVict, string nameEvent, string idEvent)
	{
		var oldList = new Dictionary<GameObject, CublastPlayer>();
		for(int i = 0; i < cls.hitNetClient.Count; i++){
			oldList.Add(cls.hitNetClient.ElementAt(i).Key, cls.hitNetClient.ElementAt(i).Value);	
		}
		cls.hitNetClient.Clear();
		string[] playerGetted = players.Split(';');
		string[] playersIDGetted = playersID.Split(';');
		string[] playersReadyGetted = playersReady.Split(';');
		string[] playersVictGetted = playersVict.Split(';');
		
		for(int i = 0; i < playerGetted.Length; i++)
		{
			cls.associateCubeToString(playerGetted[i], playersIDGetted[i], System.Convert.ToInt32(playersReadyGetted[i]) == 1, System.Convert.ToInt32(playersVictGetted[i]));	
		}
		
		cls.checkForVerificationConnected(nameEvent, idEvent, oldList);
	}
	
	public void callPlayerReady(string name, string id)
	{
		networkView.RPC ("onPlayerReady", RPCMode.All, name, id);	
	}
	
	[RPC]
	void onPlayerReady(string name, string id)
	{
		cls.checkPlayerReady(name, id);	
	}
	
	//Client side
	public void callProfileToServer(string name, string id, NetworkPlayer playerAsked)
	{
		networkView.RPC ("notifyPlayerForProfile", RPCMode.Server, name, id, playerAsked);
	}
	
	//Server side
	[RPC]
	public void notifyPlayerForProfile(string name, string id, NetworkPlayer playerAsked)
	{
		var playerCalled = LANManager.Instance.players.FirstOrDefault(c => c.Value.name == name && c.Value.idFile == id);
		if(!playerCalled.Equals(default(KeyValuePair<NetworkPlayer,CublastPlayer>)))
		{
			if(playerCalled.Key.ToString() == "0") //Server
			{
				networkView.RPC("getPlayerProfile", playerAsked, ProfileManager.Instance.getProfileStream());	
			}else
			{
				networkView.RPC ("sendProfile", playerCalled.Key, playerAsked);	
			}
			
		}
	}
	
	//Client called side
	[RPC]
	void sendProfile(NetworkPlayer playerAsked)
	{
		networkView.RPC("getProfile", RPCMode.Server, ProfileManager.Instance.getProfileStream(), playerAsked);	
	}
	
	//Server side
	[RPC]
	void getProfile(byte[] profile, NetworkPlayer playerAsked)
	{
		if(playerAsked.ToString() == "0")
		{
			var name = ProfileManager.Instance.saveProfileStream(profile);	
			GetComponent<ChatScript>().sendDirectMessage("Info", ProfileManager.Instance.currentProfile.name + " " + name + " profile");
		}else{
			networkView.RPC ("getPlayerProfile", playerAsked, profile);
		}
		
	}
	
	//Client asked side
	[RPC]
	void getPlayerProfile(byte[] profile)
	{
		var name = ProfileManager.Instance.saveProfileStream(profile);	
		GetComponent<ChatScript>().sendDirectMessage("Info", ProfileManager.Instance.currentProfile.name + " " + name + " profile");
	}
	
	[RPC]
	void launchGame()
	{
		//mettre une anim avant
		//TEMPORAIRE
		LANManager.Instance.statut = LANStatut.SELECTSONG;
		Application.LoadLevel("LANWheel");
	}
	
	[RPC]
	void gameIsLaunched()
	{
		LANManager.Instance.errorToDisplay = "This server is actually in game. Please retry later.";
		cls.stateScene = LANConnexionState.FAIL;
	}
	
	[RPC]
	void changeRightDifficulty(bool rights)
	{
		LANManager.Instance.getTheRightToChange = rights;
	}
}
