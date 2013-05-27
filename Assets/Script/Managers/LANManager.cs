using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LANManager{

	
	//LAN
	public LANMode modeLANselected;
	
	public bool isCreator;
	
	public LANStatut statut;
	
	public bool isPicker;
	
	public string actualIP;
	public int actualPort;
	
	public string IPRequest;
	public int portRequest;
	
	public float numberOfPlayersConnected;
	
	public string errorToDisplay;
	public bool rejectedByServer;
	
	//For server
	public Dictionary<NetworkPlayer, CublastPlayer> players;
	public Dictionary<NetworkPlayer, PlayerState> dataPlayerSave;
	
	//Elimination
	public int roundNumber;
	
	//0 - Host only
	//1 - Each player after each
	//2 - Random
	public int hostSystem;
	
	public int turn;
	
	//0 - All Same
	//1 - All Different
	//2 - Select players
	public int songDiffSystem;
	
	public bool getTheRightToChange;
	
	
	public bool dataArrived;
	
	public int actualRound;
	
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
	
	public string convertDifficultyModeToString(int diffmode)
	{
		switch(diffmode)
		{
		case 0:
			return "Same as master";
		case 1:
			return "Player decision";
		case 2:
			return "Some players decision";
		}
		return "null";
	}
	
	public string convertHostSystemToString(int hostSystem)
	{
		switch(hostSystem)
		{
		case 0:
			return "Master only";
		case 1:
			return "One by one";
		case 2:
			return "Random";
		}
		return "null";
		
	}
	
	
	public void init(){
		rejectedByServer = false;
		modeLANselected = LANMode.NONE;
		getTheRightToChange = false;
		isPicker = false;
		turn = 0;
		players = new Dictionary<NetworkPlayer, CublastPlayer>();
		numberOfPlayersConnected = 0;
		roundNumber = 5;
		isCreator = true;
		
		actualIP = "";
		actualPort = 0;
		
		actualRound = 0;
		
		IPRequest = "";
		portRequest = 0;
		
		dataArrived = false;
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
	
	
	public string returnPackAvailableText()
	{
		string packText = "";
		Dictionary<string, string> banned = new Dictionary<string, string>();
		List<List<string>> listPack = new List<List<string>>();
		for(int i=0; i < players.Count; i++)
		{
			listPack.Add(players.ElementAt(i).Value.packName.Split(';').ToList());
		}
		
		
		//Pour toute les listes
		for(int i=0; i < listPack.Count; i++)
		{
			//pour toutes les autres listes
			
			for(int j=i; j < listPack.Count; j++)
			{
				if(i != j)
				{
					var foundOne = false;
					for(int k=0; k < listPack.ElementAt(i).Count() && !foundOne; k++)
					{
						//Si l'élément de la liste comparé contient l'élement en cours de la liste en cours
						if(listPack.ElementAt(j).Contains(listPack.ElementAt(i).ElementAt(k)))
						{
							foundOne = true;
							
						}
					}
					
					//S'il n'a rien trouvé
					if(!foundOne)
					{
						var firstbanned = players.ElementAt(i).Value.name;
						var secondbanned = players.ElementAt(j).Value.name;
						
						if(!(banned.ContainsKey(secondbanned) && banned[secondbanned].Contains(firstbanned)))
						{
							if(banned.ContainsKey(firstbanned))
							{
								banned[firstbanned] += ";" + secondbanned;	
							}else
							{
								banned.Add(firstbanned, secondbanned);
							}
						}
					}
				}
			}
		}
		
		if(banned.Count == 0)
		{
			packText += TextManager.Instance.texts["LAN"]["NETWORKPackSuccess"];	
		}else
		{
			foreach(var banPlayer in banned)
			{
				packText += TextManager.Instance.texts["LAN"]["NETWORKPackFail"];
				packText = packText.Replace("FIRST_NAME", banPlayer.Key);
				packText = packText.Replace("OTHER_NAMES", banPlayer.Value.Replace(";", ", "));
				packText += "\n";
			}
			
			packText += TextManager.Instance.texts["LAN"]["NETWORKPackFailInfo"] + "\n";
		}
		
		return packText;
	}
	
	
	public List<CublastPlayer> getLeaderboard()
	{
		var playerSorted = players.OrderByDescending(c => c.Value.getScoreNumeric());
		var leaderboard = new List<CublastPlayer>();
		for(int i=0; i < playerSorted.Count(); i++)
		{
			leaderboard.Add(playerSorted.ElementAt(i).Value);
		}
		return leaderboard;
	}
	
	public string parseLeaderboardToString(List<CublastPlayer> lb)
	{
		var stringParse = "";
		for(int i=0; i<lb.Count; i++)
		{
			stringParse += lb.ElementAt(i).name + ";" + lb.ElementAt(i).idFile + ";" + lb.ElementAt(i).getScore();
			if(i != lb.Count - 1)
			{
				stringParse += ":";
			}
		}
		
		return stringParse;
	}
	
	
	public int convertPlaceToPoint(int place)
	{
		switch(place)
		case 0:
			return 10;
		case 1:
			return 8;
		case 2:
			return 6;
		case 3 :
			return 4;
		case 4 :
			return 3;
		case 5 :
			return 2;
		case 6 : 
			return 1;
		case 7 :
			return 0;
	}

}
