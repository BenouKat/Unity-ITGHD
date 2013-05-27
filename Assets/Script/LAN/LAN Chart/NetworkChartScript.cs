using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class NetworkChartScript : MonoBehaviour {
	
	private InGameScript igs;
	
	//START
	public bool readyToPlay;
	public bool readyFinish;
	public bool startVerifyFinish;
	public float timeSendReady;
	private float timeReady;
	
	
	//GAME
	private Dictionary<NetworkPlayer, PlayerState> dataPlayer;
	private int[] positions;
	private float[] scores;
	private float[] lives;
	private int[] comboType;
	private string infoToSend;
	private string[] infoCutted;
	private string[] infoPlayer;
	private bool alreadyInitialized;
	
	private float oldScore;
	private float oldLife;
	private int oldCT;
	private int oldPosition;
	
	private int positionInTab;
	
	
	private bool updateRequired;
	
	public float refreshTime;
	private float time;
	
	
	//GUI
	public GameObject[] networkPlayersObject;
	private UILabel[] NPnames;
	private UILabel[] NPscores;
	private UILabel[] NPRank;
	private UISprite[] NPHUD;
	private GameObject[] NPStreak;
	public UILabel rankPlayer;
	public UILabel positionScore;
	public UILabel bestPlayer;
	
	private bool isDifferenceFirst;
	private float yourScore;
	private float firstScore;
	private float secondScore;
	public Color notFirstColor;
	
	
	public Color[] colorStreak;
	public Color[] colorRank;
	public Color[] colorLife;
	
	//POOL
	private PlayerState poolPS;
	private int poolIndex;
	private int poolIndexSec;
	private int pooldecalIndex;
	
	private List<NetworkPlayer> playersDisconnected;
	
	void Awake()
	{
		Network.SetLevelPrefix(9);
		Network.SetSendingEnabled(0, true);
		Network.isMessageQueueRunning = true;	
		playersDisconnected = new List<NetworkPlayer>();
	}
	
	
	// Use this for initialization
	void Start () {
		timeReady = timeSendReady;
		time = 0f;
		readyToPlay = false;
		oldPosition = 0;
		alreadyInitialized = false;
		readyFinish = false;
		startVerifyFinish = false;
		igs = GetComponent<InGameScript>();
		LANManager.Instance.statut = LANStatut.GAME;
		if(LANManager.Instance.isCreator)
		{
			LANManager.Instance.players[Network.player].statut = LANStatut.GAME;	
			
			dataPlayer = new Dictionary<NetworkPlayer, PlayerState>();
			
			dataPlayer.Add(Network.player, new PlayerState());	
			
			dataPlayer[Network.player].name = LANManager.Instance.players[Network.player].name;
			
			positions = new int[LANManager.Instance.players.Count];
			scores = new float[LANManager.Instance.players.Count];
			lives = new float[LANManager.Instance.players.Count];
			comboType = new int[LANManager.Instance.players.Count];
			alreadyInitialized = true;
		}
		
		NPnames = new UILabel[networkPlayersObject.Count()];
		NPscores = new UILabel[networkPlayersObject.Count()];
		NPRank = new UILabel[networkPlayersObject.Count()];
		NPHUD = new UISprite[networkPlayersObject.Count()];
		NPStreak = new GameObject[networkPlayersObject.Count()];
		
		for(poolIndex=0; poolIndex<networkPlayersObject.Count(); poolIndex++)
		{
			NPnames[poolIndex] = networkPlayersObject[poolIndex].transform.FindChild("Name").GetComponent<UILabel>();
			NPscores[poolIndex] = networkPlayersObject[poolIndex].transform.FindChild("Score").GetComponent<UILabel>();
			NPRank[poolIndex] = networkPlayersObject[poolIndex].transform.FindChild("Rank").GetComponent<UILabel>();
			NPStreak[poolIndex] = networkPlayersObject[poolIndex].transform.FindChild("Streak").gameObject;
			NPHUD[poolIndex] = networkPlayersObject[poolIndex].transform.FindChild("HUD").GetComponent<UISprite>();
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
					timeReady = 0f;
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
				time = 0f;
			}else{
				time += Time.deltaTime;
			}
		}
		
		if(LANManager.Instance.isCreator && startVerifyFinish)
		{
			var everyoneValid = true;
			for(int i=0; i<dataPlayer.Count; i++)
			{
				if(!dataPlayer.ElementAt(i).Value.hasFinished)
				{
					everyoneValid = false;	
				}
			}
			
			if(everyoneValid)
			{
				startVerifyFinish = false;
				networkView.RPC ("sendFinish", RPCMode.Others);
				sendFinish();
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
		
		playersDisconnected.Add(player);
		//LANManager.Instance.players.Remove(player);
	}
	
	public void cleanPlayerDisconnected()
	{
		for(int i=0; i<playersDisconnected.Count; i++)
		{
			LANManager.Instance.players.Remove(playersDisconnected.ElementAt(i));	
			dataPlayer.Remove(playersDisconnected.ElementAt(i));
		}
		playersDisconnected.Clear();
	}
	
	public void saveData()
	{
		LANManager.Instance.dataPlayerSave = dataPlayer;	
	}
	
	//Only called by server from players
	[RPC]
	void getInfoOnPlayerConnected(NetworkPlayer player, string name, string id, int vict, string packname)
	{
		networkView.RPC("gameIsLaunched", player);
	}
	
	//Server
	[RPC]
	void sendStatus(NetworkPlayer player, int statut)
	{
		LANManager.Instance.players[player].statut = (LANStatut)statut;
		
		if(!dataPlayer.ContainsKey(player)){
			dataPlayer.Add(player, new PlayerState());
			dataPlayer[player].name = LANManager.Instance.players[player].name;
		}
		
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

			getReadyToPlay();
			networkView.RPC ("getReadyToPlay", RPCMode.Others);
				
			
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
				getInfo(Network.player, score, life, ct);
			}else{
				networkView.RPC("getInfo", RPCMode.Server, Network.player, score, life, ct);
			}
			oldScore = score;
			oldLife = life;
			oldCT = ct;
		}
	}
	
	
	//server
	[RPC]
	public void getInfo(NetworkPlayer player, float score, float life, int ct)
	{
		updateRequired = true;
		dataPlayer[player].fillPlayerState(score, life, ct);
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
		poolIndex=0;
		foreach(KeyValuePair<NetworkPlayer, PlayerState> el in dataPlayer.OrderByDescending(c => c.Value.score))
		{
			dataPlayer[el.Key].position = poolIndex+1;
			poolIndex++;
		}
		
		infoToSend = "";
		for(poolIndex=0; poolIndex<LANManager.Instance.players.Count; poolIndex++)
		{
			if(!String.IsNullOrEmpty(infoToSend)) infoToSend += "!";
			poolPS = dataPlayer[LANManager.Instance.players.ElementAt(poolIndex).Key];
			infoToSend += poolPS.position + ";" + poolPS.score + ";" + poolPS.life + ";" + poolPS.comboType;
		}
		updateRequired = false;
		networkView.RPC ("refreshInfo", RPCMode.Others, infoToSend);
		refreshInfo(infoToSend);
	}
	
	//client and server side
	[RPC]
	public void refreshInfo(string info)
	{
		infoCutted = info.Split('!');
		for(poolIndex=0; poolIndex<infoCutted.Count(); poolIndex++)
		{
			infoPlayer = infoCutted[poolIndex].Split(';');
			positions[poolIndex] = System.Convert.ToInt32(infoPlayer[0]);
			scores[poolIndex] = (float)System.Convert.ToDouble(infoPlayer[1]);
			lives[poolIndex] = (float)System.Convert.ToDouble(infoPlayer[2]);
			comboType[poolIndex] = System.Convert.ToInt32(infoPlayer[3]);
		}
		
		
		isDifferenceFirst = false;
		for(poolIndex=0; poolIndex<positions.Count(); poolIndex++)
		{
			if(poolIndex == positionInTab)
			{
				if(!rankPlayer.enabled && scores[positionInTab] >= 0.1f)
				{
					rankPlayer.enabled = true;
				}
				makeRankColor(rankPlayer, positions[positionInTab]);
				if(oldPosition != positions[positionInTab])
				{
					if(positions[positionInTab] == 1)
					{
						isDifferenceFirst = false;
						bestPlayer.text = "You are the leader !";
						
					}else
					{
						isDifferenceFirst = true;
					}
					yourScore = scores[positionInTab];
				}
			}else{
				
				pooldecalIndex = poolIndex > positionInTab ? poolIndex - 1 : poolIndex;
				if(!NPRank[pooldecalIndex].enabled && scores[poolIndex] >= 0.1f)
				{
					NPRank[pooldecalIndex].enabled = true;
				}
				makeRankColor(NPRank[pooldecalIndex], positions[poolIndex]);
				makeStreakColor(NPStreak[pooldecalIndex], comboType[poolIndex]);
				makeLifeColor(NPHUD[pooldecalIndex], lives[poolIndex]);
				NPscores[pooldecalIndex].text = scores[poolIndex].ToString("00.00") + "%";
				if(positions[poolIndex] == 1)
				{
					firstScore = scores[poolIndex];	
					bestPlayer.text = "Best : " + NPnames[pooldecalIndex].text;
				}
				if(positions[poolIndex] == 2)
				{
					secondScore = scores[poolIndex];
				}
			}
		}
		
		if(isDifferenceFirst)
		{
			positionScore.text = (firstScore - yourScore).ToString("0.00") + "%";
			positionScore.effectColor = notFirstColor;
		}else{
			positionScore.text = (yourScore - secondScore).ToString("0.00") + "%";
			positionScore.effectColor = igs.beatthebeatColor;
		}
	}
	
	
	//Server
	public void sendPrimaryInfo()
	{
		infoToSend = "";
		for(poolIndex=0; poolIndex<LANManager.Instance.players.Count; poolIndex++)
		{
			if(!String.IsNullOrEmpty(infoToSend)) infoToSend += ";";
			if(dataPlayer.ContainsKey(LANManager.Instance.players.ElementAt(poolIndex).Key))
			{
				
				poolPS = dataPlayer[LANManager.Instance.players.ElementAt(poolIndex).Key];
				infoToSend += poolPS.name;
			}else{
				infoToSend += "";
			}
			
			
		}
		
		for(poolIndex=0; poolIndex<LANManager.Instance.players.Count; poolIndex++)
		{
			if(LANManager.Instance.players.ElementAt(poolIndex).Key.ToString() == "0")
			{
				refreshPrimaryInfo(infoToSend, poolIndex, LANManager.Instance.players.Count);
			}else{
				networkView.RPC ("refreshPrimaryInfo", LANManager.Instance.players.ElementAt(poolIndex).Key, infoToSend, poolIndex, LANManager.Instance.players.Count);
			}	
		}
		
		
		updateRequired = false;
		
		
	}
	
	//client and server side
	[RPC]
	public void refreshPrimaryInfo(string name, int sentPositionInTabs, int numberPlayer)
	{
		if(!alreadyInitialized)
		{
			positions = new int[numberPlayer];
			scores = new float[numberPlayer];
			lives = new float[numberPlayer];
			comboType = new int[numberPlayer];
			alreadyInitialized = true;
		}
		
		positionInTab = sentPositionInTabs;
		infoCutted = name.Split(';');
		for(poolIndexSec=0; poolIndexSec<infoCutted.Count(); poolIndexSec++)
		{
			if(poolIndexSec != positionInTab)
			{
				pooldecalIndex = poolIndexSec > positionInTab ? poolIndexSec - 1 : poolIndexSec;
				NPnames[pooldecalIndex].text = infoCutted[poolIndexSec];
				if(!String.IsNullOrEmpty(NPnames[pooldecalIndex].text) && !NPnames[pooldecalIndex].enabled)
				{
					NPnames[pooldecalIndex].enabled = true;
					NPscores[pooldecalIndex].enabled = true;
					NPscores[pooldecalIndex].text = "00.00%";
					NPHUD[pooldecalIndex].enabled = true;
					NPStreak[pooldecalIndex].active = true;
				}
			}
		}
	}
	
	
	public void makeRankColor(UILabel rankToColor, int thePos)
	{
		if(rankToColor.text != thePos.ToString())
		{
			rankToColor.text = thePos.ToString();
			rankToColor.effectColor = colorRank[thePos - 1];
		}
	}
	
	public void makeStreakColor(GameObject streakToColor, int theCT)
	{
		if(streakToColor.renderer.material.GetColor("_TintColor") != colorStreak[theCT])
		{
			streakToColor.renderer.material.SetColor("_TintColor", colorStreak[theCT]);
		}
	}
	
	public void makeLifeColor(UISprite hudToColor, float theLife)
	{
		if(theLife >= 100)
		{
			if(hudToColor.color != colorLife[0]) hudToColor.color = colorLife[0];
		}else if(theLife <= 0f)
		{
			if(hudToColor.color != colorLife[3]) hudToColor.color = colorLife[3];
		}else if(theLife <= 25f)
		{
			if(hudToColor.color != colorLife[2]) hudToColor.color = colorLife[2];
		}else if(hudToColor.color != colorLife[1])
		{
			hudToColor.color = colorLife[1];
		}
	}
	
	[RPC]
	public void hasFinished(NetworkPlayer player, string dataStat)
	{
		dataPlayer[player].statistics = dataStat;
		dataPlayer[player].hasFinished = true;	
	}
	
	[RPC]
	void sendFinish()
	{
		readyFinish = true;	
	}
}
