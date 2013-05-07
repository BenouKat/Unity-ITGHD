using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NetworkChartScript : MonoBehaviour {
	
	public InGameScript igs;
	
	//START
	public bool readyToPlay;
	public float timeSendReady;
	private float timeReady;
	
	
	//GAME
	private Dictionary<NetworkPlayer, PlayerState> dataPlayer;
	private int[] positions;
	private float[] scores;
	private float[] lives;
	private int[] comboType;
	private bool[] hasFailed;
	
	private float oldScore;
	private float oldLife;
	private int oldCT;
	
	private bool updateRequired;
	
	public float refreshTime;
	private float time;
	
	
	//GUI
	
	
	//POOL
	private PlayerState poolPS;
	private int poolIndex;
	private string[] poolString;
	
	void Awake()
	{
		Network.SetSendingEnabled(0, true);
		Network.isMessageQueueRunning = true;	
	}
	
	
	// Use this for initialization
	void Start () {
		timeReady = 0f;
		time = 0f;
		if(LANManager.Instance.isCreator)
		{
			LANManager.Instance.players[Network.player].statut = LANStatut.GAME;	
			
			dataPlayer = new Dictionary<NetworkPlayer, PlayerState>();
			
			dataPlayer.Add(Network.player, new PlayerState());	
			
			positions = new int[LANManager.Instance.players.Count];
			scores = new double[LANManager.Instance.players.Count];
			lives = new float[LANManager.Instance.players.Count];
			comboType = new int[LANManager.Instance.players.Count];
			hasFailed = new bool[LANManager.Instance.players.Count];
			poolString = new string[LANManager.Instance.players.Count];
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!readyToPlay)
		{
			if(!LANManager.Instance.isCreator){
				if(timeReady >= timeSendReady)
				{
					networkView.RPC("sendStatus", RPCMode.Server, Network.player, (int)LANManager.Instance.statut);
				}else{
					timeReady += Time.deltaTime;
				}
			}
		}else{
			if(time >= refreshTime)
			{
				askInfo();
				if(LANManager.Instance.isCreator)
				{
					if(updateRequired)
					{
						sendInfo();
					}
				}
			}else{
				time += Time.deltaTime;
			}
		}
	}
	
	void OnApplicationQuit()
	{
		Network.Disconnect();
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log(player.externalIP + " : " + player.externalPort + " disconnected");
		
		LANManager.Instance.players.Remove(player);
	}
	
	//Only called by server from players
	[RPC]
	void getInfoOnPlayerConnected(NetworkPlayer player, string name, string id, int vict, string packname)
	{
		networkView.RPC("gameIsLaunched", player);
	}
	
	
	[RPC]
	void sendStatus(NetworkPlayer player, int statut)
	{
		LANManager.Instance.players[player].statut = (LANStatut)statut;
		
		if(!dataPlayer.ContainsKey(player)) dataPlayer.Add(player, new PlayerState());
		
		sendPrimaryInfo();
		
		if(!readyToPlay)
		{
			for(poolIndex=0; poolIndex<LANManager.Instance.players.Count; poolIndex++)
			{
				if(LANManager.Instance.players.ElementAt(poolIndex).Value.statut != LANStatut.GAME)
				{
					return;	
				}
			}
			for(poolIndex=0; poolIndex<LANManager.Instance.players.Count; poolIndex++)
			{
				if(LANManager.Instance.players.ElementAt(poolIndex).Key == Network.player)
				{
					readyToPlay = true;	
				}else{
					networkView.RPC ("getReadyToPlay", RPCMode.Others);
				}
			}
		}
	}
	
	[RPC]
	void getReadyToPlay()
	{
		readyToPlay = true;	
	}
	
	//client
	[RPC]
	public void askInfo()
	{
		igs.askingForInfo();
	}
	
	public void objectToSend(float score, float life, int ct, bool failed)
	{
		if(score != oldScore || life != oldLife || ct != oldCT)
		{
			if(LANManager.Instance.isCreator)
			{
				getInfo(Network.player, score, life, ct, failed);
			}else{
				networkView.RPC("getInfo", RPC.Server, Network.player, score, life, ct, failed);
			}
			oldScore = score;
			life = oldLife;
			oldCT = ct;
		}
	}
	
	
	//server
	[RPC]
	public void getInfo(NetworkPlayer player, float score, float life, int ct, bool failed)
	{
		updateRequired = true;
		dataPlayer[player].fillPlayerState(score, life, ct, failed);
	}
	
	//server
	public void sendInfo()
	{
		//a revoir
		/*dataPlayer = dataPlayer.OrderByDescending(c => c.Value.score).ToDictionary(k => k.Key, v => v.Value);
		for(poolIndex=0; poolIndex<dataPlayer.Count; poolIndex++)
		{
			dataPlayer.ElementAt(poolIndex).Value.position = poolIndex+1;
		}*/
		
		foreach(KeyValuePair<NetworkPlayer, PlayerState> el in dataPlayer.OrderByDescending(c => c.Value.score))
		{
			dataPlayer[el.Key].position = poolIndex+1;
		}
		
		for(poolIndex=0; poolIndex<LANManager.Instance.players.Count; poolIndex++)
		{
			poolPS = dataPlayer[LANManager.Instance.players.ElementAt(poolIndex).Key];
			positions[poolIndex] = poolPS.position;
			scores[poolIndex] = poolPS.score;
			lives[poolIndex] = poolPS.life;
			comboType[poolIndex] = poolPS.comboType;
			hasFailed[poolIndex] = poolPS.hasFailed;
		}
		updateRequired = false;
		networkView.RPC ("refreshInfo", RPCMode.Others, positions, scores, lives, comboType, hasFailed);
		refreshInfo(positions, scores, lives, comboType, hasFailed);
	}
	
	public void sendPrimaryInfo()
	{
	
		for(poolIndex=0; poolIndex<LANManager.Instance.players.Count; poolIndex++)
		{
			if(dataPlayer.containsKey(LANManager.Instance.players.ElementAt(poolIndex).Key))
			{
				poolPS = dataPlayer[LANManager.Instance.players.ElementAt(poolIndex).Key];
				poolString[i] = poolPS.name;
			}else{
				poolString[i] = "";
			}
		}
		updateRequired = false;
		networkView.RPC ("refreshInfo", RPCMode.Others, positions, scores, lives, comboType, hasFailed);
		refreshInfo(positions, scores, lives, comboType, hasFailed);
	}
	
	//client and server side
	[RPC]
	public void refreshInfo(int[] pos, float[] score, float[] life, int[] ct, bool[] failed)
	{
		//Remplir les HUD info
	}
	
	//client and server side
	[RPC]
	public void refreshPrimaryInfo(string[] name)
	{
		//Remplir les HUD des noms
	}
}
