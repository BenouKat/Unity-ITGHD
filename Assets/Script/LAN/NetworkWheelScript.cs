using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class NetworkWheelScript : MonoBehaviour {
	
	public KeyValuePair<Difficulty, Dictionary<Difficulty, Song>> lastSongChecked;
	
	public bool isSearching;
	
	public bool isRefreshVote;
	
	public NetworkPlayer playerSearching;
	
	void Awake()
	{
		Network.SetSendingEnabled(0, true);
		Network.isMessageQueueRunning = true;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnApplicationQuit()
	{
		Network.Disconnect();
	}
	
	//server side
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log(player.externalIP + " : " + player.externalPort + " disconnected");
		
		var nameDisconnected = LANManager.Instance.players[player].name;
		
		GetComponent<ChatScript>().sendDirectMessage("Info", nameDisconnected + " has left the game");
		
		LANManager.Instance.players.Remove(player);
		
		if(LANManager.Instance.players.Count <= 1)
		{
			LANManager.Instance.rejectedByServer = true;
			LANManager.Instance.errorToDisplay = TextManager.Instance.texts["LAN"]["ALONE"];
			Network.Disconnect();
			Application.LoadLevel("LAN");
		}
	}
	
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if(!LANManager.Instance.rejectedByServer)
		{
			LANManager.Instance.rejectedByServer = !GetComponent<GeneralScriptLAN>().quitAsked;
			LANManager.Instance.errorToDisplay = TextManager.Instance.texts["LAN"]["DISCONNECTED"] + info.ToString();
			if(LANManager.Instance.rejectedByServer)
			{
				Application.LoadLevel("LAN");
			}
		}
		
	}
	
	//Only called by server from players
	[RPC]
	void getInfoOnPlayerConnected(NetworkPlayer player, string name, string id, int vict, string packname)
	{
		networkView.RPC("gameIsLaunched", player);
	}
	
	[RPC]
	public void sendStatut(NetworkPlayer player, int statut)
	{
		LANManager.Instance.players[player].statut = (LANStatut) statut;
	}
	
	[RPC]
	void notifyReadyToChoose()
	{
		GetComponent<GeneralScriptLAN>().pickSetted = true;	
	}
	
	//Only server
	public bool isPlayerStatutReady(LANStatut statutAsked)
	{
		var isok = true;
		for(int i=0; i < LANManager.Instance.players.Count; i++)
		{
			if(LANManager.Instance.players.ElementAt(i).Value.statut != statutAsked)
			{
				isok = false;
				break;
			}
		}
		
		return isok;
	}
	
	public void setPicker()
	{
		switch(LANManager.Instance.hostSystem)
		{
			case 0:
			
			LANManager.Instance.isPicker = true;
			break;
			
			
			case 1:
			
			if(LANManager.Instance.turn == 0)
			{
				LANManager.Instance.isPicker = true;
			}else
			{
				for(int i=1; i < LANManager.Instance.players.Count; i++)
				{
					networkView.RPC ("sendPickInfo", LANManager.Instance.players.ElementAt(i).Key, i == LANManager.Instance.turn);
				}
			}
			LANManager.Instance.turn++;
			if(LANManager.Instance.turn >= LANManager.Instance.players.Count)
			{
				LANManager.Instance.turn = 0;	
			}
			
			break;
			case 2:
			
			LANManager.Instance.turn = (int)(UnityEngine.Random.value*LANManager.Instance.players.Count);
			if(LANManager.Instance.turn >= LANManager.Instance.players.Count)
			{
				LANManager.Instance.turn = LANManager.Instance.players.Count - 1;	
			}
			if(LANManager.Instance.turn == 0)
			{
				LANManager.Instance.isPicker = true;
			}else
			{
				for(int i=1; i < LANManager.Instance.players.Count; i++)
				{
					networkView.RPC ("sendPickInfo", LANManager.Instance.players.ElementAt(i).Key, i == LANManager.Instance.turn);
				}
			}
			break;
		}
	}
	
	[RPC]
	void sendPickInfo(bool picker)
	{
		LANManager.Instance.isPicker = picker;
	}
	
	//Server only
	[RPC]
	public void callSong(NetworkPlayer player, string name, string subtitle, int step, int difficulty, int level)
	{
		if(!isSearching)
		{
			var theSIP = new SongInfoProfil(name, subtitle, step, (Difficulty)difficulty, level);
			if(lastSongChecked.Equals(default(KeyValuePair<Difficulty, Dictionary<Difficulty, Song>>)) || !lastSongChecked.Value[lastSongChecked.Key].sip.CompareId(theSIP))
			{
				for(int i=0; i < LANManager.Instance.players.Count; i++)
				{
					LANManager.Instance.players.ElementAt(i).Value.songChecked = 0;	
				}
				isSearching = true;
				networkView.RPC("setSearching", RPCMode.Others, true);
				networkView.RPC ("checkSong", RPCMode.Others, name, subtitle, step, difficulty, level);
				playerSearching = player;
				lastSongChecked = LoadManager.Instance.FindSong(theSIP);
				LANManager.Instance.players[Network.player].songChecked = lastSongChecked.Equals(default(KeyValuePair<Difficulty, Dictionary<Difficulty, Song>>)) ? 2 : 1;
				
			}else
			{
				isSearching = true;
				playerSearching = player;
			}
			
		}
	}
	
	//Client only
	[RPC]
	void checkSong(string name, string subtitle, int step, int difficulty, int level)
	{
		lastSongChecked = LoadManager.Instance.FindSong(new SongInfoProfil(name, subtitle, step, (Difficulty)difficulty, level));
		var getSong = !lastSongChecked.Equals(default(KeyValuePair<Difficulty, Dictionary<Difficulty, Song>>));
		networkView.RPC("getSongCheckResult", RPCMode.Server, Network.player, getSong);
	}
	
	[RPC]
	void setSearching(bool search)
	{
		isSearching = search;	
		if(!isSearching) setVoteMode(true);
	}
	
	[RPC]
	void setVoteMode(bool vote)
	{
		GetComponent<GeneralScriptLAN>().inVoteMode = vote;	
		GetComponent<GeneralScriptLAN>().releaseHiddenVote = false;	
	}
	
	[RPC]
	void releaseVote()
	{
		GetComponent<GeneralScriptLAN>().releaseHiddenVote = true;	
	}
	
	//Server only
	[RPC]
	void getSongCheckResult(NetworkPlayer player, bool result)
	{
		LANManager.Instance.players[player].songChecked = result ? 1 : 2;
	}
	
	//Server only
	//-1 - Not arrived
	//0 - Ok
	//1+ - Ko
	public void isSongAvailable()
	{
		var getKo = 0;
		for(int i=0; i<LANManager.Instance.players.Count; i++)
		{
			if(LANManager.Instance.players.ElementAt(i).Value.songChecked == 0)
			{
				return;	
			}
			if(LANManager.Instance.players.ElementAt(i).Value.songChecked == 2)
			{
				getKo++;	
			}
		}
		networkView.RPC("setSearching", RPCMode.Others, false);
		setSearching(false);
		if(playerSearching.ToString() == "0")
		{
			getResultSearch(getKo);
		}else
		{
			networkView.RPC ("getResultSearch", playerSearching, getKo);	
		}
		return;
	}
	
	//To Asked Players
	[RPC]
	void getResultSearch(int numberKo)
	{
		
		if(!LANManager.Instance.isPicker || numberKo > 0)
		{
			GetComponent<GeneralScriptLAN>().launchProposition = false;
			networkView.RPC("setVoteMode", RPCMode.All, false);
			if(numberKo == 0)
			{
				String textInfo = TextManager.Instance.texts["LAN"]["NETWORKWheelVoteSuccess"];
				if(!String.IsNullOrEmpty(lastSongChecked.Value[lastSongChecked.Key].subtitle))
				{
					textInfo = textInfo.Replace("SUBTTL", lastSongChecked.Value[lastSongChecked.Key].subtitle);
				}else
				{
					textInfo = textInfo.Replace("- SUBTTL", "");
				}
				textInfo = textInfo.Replace("TITLE", lastSongChecked.Value[lastSongChecked.Key].title).Replace("DIFFICULTY", lastSongChecked.Key.ToString()).Replace("LVL", lastSongChecked.Value[lastSongChecked.Key].level.ToString());
				GetComponent<ChatScript>().sendDirectMessage(ProfileManager.Instance.currentProfile.name, textInfo);
			}else
			{
				String textInfo = LANManager.Instance.isPicker ? TextManager.Instance.texts["LAN"]["NETWORKWheelRealVoteFailed"] : TextManager.Instance.texts["LAN"]["NETWORKWheelVoteFailed"];
				if(!String.IsNullOrEmpty(lastSongChecked.Value[lastSongChecked.Key].subtitle))
				{
					textInfo = textInfo.Replace("SUBTTL", lastSongChecked.Value[lastSongChecked.Key].subtitle);
				}else
				{
					textInfo = textInfo.Replace("- SUBTTL", "");
				}
				textInfo = textInfo.Replace("TITLE", lastSongChecked.Value[lastSongChecked.Key].title).Replace("NUMBERFAIL", numberKo.ToString());
				GetComponent<ChatScript>().sendDirectMessage(ProfileManager.Instance.currentProfile.name, textInfo);
			}
		}else
		{
			networkView.RPC ("releaseVote", RPCMode.All);
			GetComponent<GeneralScriptLAN>().launchProposition = false;
			
			String textInfo = TextManager.Instance.texts["LAN"]["NETWORKWheelRealVoteSuccess"];
			if(!String.IsNullOrEmpty(lastSongChecked.Value[lastSongChecked.Key].subtitle))
			{
				textInfo = textInfo.Replace("SUBTTL", lastSongChecked.Value[lastSongChecked.Key].subtitle);
			}else
			{
				textInfo = textInfo.Replace("- SUBTTL", "");
			}
			textInfo = textInfo.Replace("TITLE", lastSongChecked.Value[lastSongChecked.Key].title).Replace("DIFFICULTY", lastSongChecked.Key.ToString()).Replace("LVL", lastSongChecked.Value[lastSongChecked.Key].level.ToString());
			textInfo += "\n" + TextManager.Instance.texts["LAN"]["NETWORKWheelPickerVote"];
			GetComponent<ChatScript>().sendDirectMessage(ProfileManager.Instance.currentProfile.name, textInfo);
			if(LANManager.Instance.isCreator)
			{
				initVoteMode(Network.player);
			}else
			{
				networkView.RPC("initVoteMode", RPCMode.Server, Network.player);
			}
				
		}
	}
	
	//Only server
	[RPC]
	void initVoteMode(NetworkPlayer player)
	{
		for(int i=0; i < LANManager.Instance.players.Count; i++)
		{
			LANManager.Instance.players.ElementAt(i).Value.vote = 0;	
		}
		LANManager.Instance.players[player].vote = 1;
		isRefreshVote = true;
		goInVoteMode(true);
		networkView.RPC ("goInVoteMode", RPCMode.Others, true);
	}
	
	//Only client
	[RPC]
	void goInVoteMode(bool entering)
	{
		if(entering)
		{
			GetComponent<GeneralScriptLAN>().popinVoteMode();
		}else
		{
			GetComponent<GeneralScriptLAN>().popoutVoteMode();
		}
	}
	
	//Only server
	[RPC]
	public void getResultVote(NetworkPlayer player, int result)
	{
		LANManager.Instance.players[player].vote = result;
	}
	
	//Server only
	public void refreshVoteMode()
	{
		var getKo = 0;
		for(int i=0; i<LANManager.Instance.players.Count; i++)
		{
			if(LANManager.Instance.players.ElementAt(i).Value.vote == 0)
			{
				return;	
			}
			if(LANManager.Instance.players.ElementAt(i).Value.vote == 2)
			{
				getKo++;	
			}
		}
		isRefreshVote = false;
		networkView.RPC("setVoteMode", RPCMode.All, false);
		if(getKo > 0)
		{
			GetComponent<ChatScript>().sendDirectMessage("Info", TextManager.Instance.texts["LAN"]["VOTEFailFinal"]);
			goInVoteMode(false);
			networkView.RPC("goInVoteMode", RPCMode.Others, false);
		}else
		{
			for(int i=0; i<LANManager.Instance.players.Count; i++)
			{
				LANManager.Instance.players.ElementAt(i).Value.isReady = false;
			}
			goInOption();
			networkView.RPC ("goInOption", RPCMode.Others);
		}
		return;
	}
	
	//Only client
	[RPC]
	void goInOption()
	{
		GetComponent<GeneralScriptLAN>().popinOption();	
	}
	
	[RPC]
	public void getPlayerReady(NetworkPlayer player)
	{
		GetComponent<ChatScript>().sendDirectMessage("Info", LANManager.Instance.players[player].name + " is ready");
		LANManager.Instance.players[player].isReady = true;
	}
	
	//Server only
	public void refreshOptionMode()
	{
		for(int i=0; i<LANManager.Instance.players.Count; i++)
		{
			if(!LANManager.Instance.players.ElementAt(i).Value.isReady)
			{
				return;	
			}
		}
		goOnPlay();
		networkView.RPC("goOnPlay", RPCMode.Others);
	}
	
	
	//Passer ici les paramètres des top players
	[RPC]
	void goOnPlay()
	{
		GetComponent<GeneralScriptLAN>().play();
	}
	
	[RPC]
	public void getLeaderboard(string lb)
	{
		GetComponent<LaunchSongZoneLAN>().setLeaderboard(lb);
	}


}
