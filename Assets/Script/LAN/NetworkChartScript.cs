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
	public Dictionary<NetworkPlayer, PlayerState> dataPlayer;
	public int[] positions;
	public float[] scores;
	public float[] lives;
	public int[] comboType;
	public bool[] hasFailed;
	
	private PlayerState poolPS;
	private int poolIndex;
	
	void Awake()
	{
		Network.SetSendingEnabled(0, true);
		Network.isMessageQueueRunning = true;	
	}
	
	
	// Use this for initialization
	void Start () {
		timeReady = 0f;
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
		
	}
	
	
	//server
	[RPC]
	public void getInfo(NetworkPlayer player, float score, float life, int ct, bool failed)
	{
			
	}
	
	//server
	public void sendInfo()
	{
		//a revoir
		dataPlayer = dataPlayer.OrderByDescending(c => c.Value.score).ToDictionary(k => k.Key, v => v.Value);
		for(poolIndex=0; poolIndex<dataPlayer.Count; poolIndex++)
		{
			dataPlayer.ElementAt(poolIndex).Value.position = poolIndex+1;
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
		networkView.RPC ("refreshInfo", RPCMode.Others, positions, scores, lives, comboType, hasFailed);
		refreshInfo(positions, scores, lives, comboType, hasFailed);
	}
	
	//client and server side
	[RPC]
	public void refreshInfo(int[] pos, float[] score, float[] life, int[] ct, bool[] failed)
	{
		//Remplir les HUD
	}
}
