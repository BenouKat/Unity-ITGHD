using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LANManager{

	
	//LAN
	public LANMode modeLANselected;
	public float numberOfPlayersConnected;
	
	//Score tournament
	public Dictionary<NetworkPlayer, double> scoreOfPlayers;
	
	//Point tournament
	public Dictionary<NetworkPlayer, int> pointsOfPlayers;
	
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
		scoreOfPlayers = new Dictionary<NetworkPlayer, double>();
		pointsOfPlayers = new Dictionary<NetworkPlayer, int>();
		numberOfPlayersConnected = 0;
		roundNumber = 0;
	}
	
	
	
	
	
}
