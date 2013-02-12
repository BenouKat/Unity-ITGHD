using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LANManager{

	
	//LAN
	public LANMode modeLANselected;
	
	public bool isCreator;
	
	public string actualIP;
	public int actualPort;
	
	public string IPRequest;
	public int portRequest;
	
	public float numberOfPlayersConnected;
	
	public string errorToDisplay;
	
	//For server
	public Dictionary<NetworkPlayer, CublastPlayer> players;
	
	//Elimination
	public int roundNumber;
	
	//0 - Host only
	//1 - Each player after each
	//2 - Random
	public int hostSystem;
	
	//0 - All Same
	//1 - All Different
	//2 - Select players
	public int songDiffSystem;
	
	//INSTANCE
	private static LANManager instance;
	
	public static LANManager Instance{
		get{
			if(instance == null){ 
				instance = new LANManager();
				instance.init();
			}
			return instance;
		}
	}
	
	
	public void init(){
		modeLANselected = LANMode.NONE;
		players = new Dictionary<NetworkPlayer, CublastPlayer>();
		numberOfPlayersConnected = 0;
		roundNumber = 0;
		isCreator = true;
		
		actualIP = "";
		actualPort = 0;
		
		IPRequest = "";
		portRequest = 0;
	}
	
	
	
	public static string errorToString(NetworkConnectionError nce)
	{
		switch(nce){
		case NetworkConnectionError.AlreadyConnectedToAnotherServer :
			return "You are already connected to another server.";
		case NetworkConnectionError.AlreadyConnectedToServer :
			return "You are already connected to the server.";
		case NetworkConnectionError.ConnectionFailed :
			return "The server initialization has failed.";
		case NetworkConnectionError.CreateSocketOrThreadFailure :
			return "The server can't initialize. Check your used sockets.";
		case NetworkConnectionError.EmptyConnectTarget :
			return "The ip address field is empty.";
		case NetworkConnectionError.IncorrectParameters :
			return "The ip address field is incorrect.";
		case NetworkConnectionError.InternalDirectConnectFailed :
			return "Same network already connected by NAT.";
		case NetworkConnectionError.TooManyConnectedPlayers :
			return "The room is full. Please try later.";
		case NetworkConnectionError.NoError :
			return "";
		default :
			return "Connexion problems. Please try again.";
		}
		
	}
	
	
	
	
	
}
