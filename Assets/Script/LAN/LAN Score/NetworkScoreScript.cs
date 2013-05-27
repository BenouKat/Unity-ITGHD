using UnityEngine;
using System.Collections;

public class NetworkScoreScript : MonoBehaviour {
	
	public GameObject panelGlobalGO;
	public GameObject panelIndividualGO;
	public GameObject panelMatchGO;
	
	private UIPanel panelGlobal;
	private UIPanel panelIndividual;
	private UIPanel panelMatch;
	
	private GameObject[] playerGlobal;
	private GameObject[] playerIndividual;
	private GameObject[] playerMatch;
	
	private bool readyToQuit;
	private float time;
	
	public CameraMove cam;
	public GUISkin skin;
	public Rect thequitButton;
	
	
	public FadeManager fm;
	void Awake()
	{
		Network.SetLevelPrefix(10);
		Network.SetSendingEnabled(0, true);
		Network.isMessageQueueRunning = true;	
	}
	
	// Use this for initialization
	void Start () {
		if(LANManager.Instance.isCreator)
		{
			for(int i=0; i<LANManager.Instance.players.Count; i++)
			{
				LANManager.Instance.players.ElementAt(i).isReady = false;
			}
			sendStatus(Network.player, LANStatut.RESULT);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!readyToQuit && time >= 0.3f)
		{
			
			networkView.RPC("sendStatus", RPCMode.Server, Network.player, (int)LANStatut.RESULT);
			time = 0f;
		}
		time += Time.deltaTime;
	}
	
	void OnGUI()
	{
		if(cam)
		{
			GUI.skin = skin;
			if(GUI.Button(new Rect(thequitButton.x*Screen.width, thequitButton.y*Screen.height, thequitButton.width*Screen.width, thequitButton.height*Screen.height), "Ready"))
			{
				if(LANManager.Instance.isCreator)
				{
					isAskedForQuit(Network.Player);
				}else{
					networkView.RPC("isAskedForQuit", RPCMode.Server, Network.player);
				}
			}
		}
	}
	
	void isAskedForQuit(NetworkPlayer player)
	{
		LANManager.Instance.players[player].isReady = true;
		
		for(int i=0; i<LANManager.Instance.players.Count; i++)
		{
			if(!LANManager.Instance.players.ElementAt(i).isReady)
			{
				return;
			}
		}
		
		networkView.RPC("quitScene", RPC.Others);
		quitScene();
		
	}
	
	void quitScene()
	{
		Network.SetSendingEnabled(0, false);
		Network.isMessageQueueRunning = false;
		LANManager.Instance.statut = LANStatut.SONGWHEEL;
		Network.SetLevelPrefix(8);
		Application.LoadLevel("LANWheel");
	}
	
	void OnApplicationQuit()
	{
		Network.Disconnect();
	}
	
	[RPC]
	void getInfoOnPlayerConnected(NetworkPlayer player, string name, string id, int vict, string packname)
	{
		networkView.RPC("gameIsLaunched", player);
	}
	
	[RPC]
	void sendStatus(NetworkPlayer player, int statut)
	{
		LANManager.Instance.players[player].statut = (LANStatut)statut;
		
		for(int i=0; i<LANManager.Instance.players.Count; i++)
		{
			if(LANManager.Instance.players.ElementAt(i).statut != LANStatut.RESULT){
				return;
			}
		}
			
			
		
		var stringData = "";
		var stringLeaderboard = "";
		for(int i=0; i<LANManager.Instance.dataPlayerSave.Count; i++)
		{
			if(i > 0) stringData += ":";
			var el = LANManager.Instance.dataPlayerSave.ElementAt(i).Value;
			stringData += el.name + ";" + el.score + ";" + (el.life <= 0f ? "1" : "0") + ";" + el.comboType + ";" + el.statistics;
		}
		
		var listPlayersTemp = dataString.Split(':');
		var listDataPlayerTemp = new Dictionary<NetworkPlayer, PlayerState>();
		
		for(int i=0; i<listPlayersTemp.Lenght; i++)
		{
			var thestat = listPlayersTemp[i].Split(';');
			listDataPlayerTemp.Add(LANManager.Instance.dataPlayerSave.ElementAt(i).Key, new PlayerState(thestat[0], (float)System.Convert.ToDouble(thestat[1]), thestat[2] == "1", System.Convert.ToInt32(thestat[3]), System.Convert.ToInt32(thestat[4]), System.Convert.ToInt32(thestat[5]), System.Convert.ToInt32(thestat[6]), System.Convert.ToInt32(thestat[7]), System.Convert.ToInt32(thestat[8]), System.Convert.ToInt32(thestat[9]), (float)System.Convert.ToDouble(thestat[10]), System.Convert.ToInt32(thestat[11]), System.Convert.ToInt32(thestat[12])));
		}
		listDataPlayerTemp = listDataPlayerTemp.OrderBy(c => c.Value.hasFailed).ThenByDescending(c => c.Value.score).ToDictionary(c.Key, d.Value);
		
		for(int i=0; i<listDataPlayerTemp.Count; i++)
		{
			if( i > 0) stringLeaderboard += ":";
			
			switch(LANManager.Instance.modeLANselected){
			
			case LANMode.FFA:
				LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].points += (i == 0 ? 1 : 0);
				stringLeaderboard += LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].name + ";" + LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].points;
				break;
				
			case LANMode.SCORETOURN:
				LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].scores += listDataPlayerTemp.ElementAt(i).Value.score;
				stringLeaderboard += LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].name + ";" + LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].score;
				break;
				
			case LANMode.POINTTOURN:
				LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].points += LANManager.Instance.convertPlaceToPoint(i);
				stringLeaderboard += LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].name + ";" + LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].points;
				break;
				
			case LANMode.ELIMINATION:
				LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].scores += listDataPlayerTemp.ElementAt(i).Value.score;
				stringLeaderboard += LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].name + ";" + LANManager.Instance.players[listDataPlayerTemp.ElementAt(i).Key].score;
				break;
			}
			
			
		}
		

		networkView.RPC("dataTreatment", RPCMode.Others, stringData, stringLeaderboard);
		dataTreatment(stringData, stringLeaderboard);
		
		
	}
	
	[RPC]
	void dataTreatment(string dataString, string stringLeaderboard)
	{
		fm.FadeOut();
	
		readyToQuit = true;
		var listPlayers = dataString.Split(':');
		var listDataPlayer = new List<PlayerState>();
		
		for(int i=0; i<listPlayers.Lenght; i++)
		{
			var thestat = listPlayers[i].Split(';');
			listDataPlayer.Add(new PlayerState(thestat[0], (float)System.Convert.ToDouble(thestat[1]), thestat[2] == "1", System.Convert.ToInt32(thestat[3]), System.Convert.ToInt32(thestat[4]), System.Convert.ToInt32(thestat[5]), System.Convert.ToInt32(thestat[6]), System.Convert.ToInt32(thestat[7]), System.Convert.ToInt32(thestat[8]), System.Convert.ToInt32(thestat[9]), (float)System.Convert.ToDouble(thestat[10]), System.Convert.ToInt32(thestat[11]), System.Convert.ToInt32(thestat[12])));
		}
		listDataPlayer = listDataPlayer.OrderBy(c => c.hasFailed).ThenByDescending(c => c.score).ToList();
		
		for(int i=0; i<8; i++)
		{
			
			if(i >= dataString.Lenght)
			{
				panelGlobalGO.transform.FindChild("Player" + (i + 1)).gameObject.setActiveRecursively(false);
				panelIndividualGO.transform.FindChild("Player" + (i + 1)).gameObject.setActiveRecursively(false);
			}else{
				panelGlobalGO.transform.FindChild("Player" + (i + 1)).gameObject.setActiveRecursively(true);
				panelIndividualGO.transform.FindChild("Player" + (i + 1)).gameObject.setActiveRecursively(true);
				var el = listDataPlayer.ElementAt(i);
				var tchild = panelGlobalGO.transform.FindChild("Player" + (i + 1));
				//Go
				tchild.FindChild("Name").GetComponent<UILabel>().text = el.name;
				tchild.FindChild("Score").GetComponent<UILabel>().text = el.score;
				if(el.hasFailed)
				{
					tchild.FindChild("Score").GetComponent<UILabel>().effectColor = new Color(1f, 0.2f, 0.2f, 1f);
				}
				
				//indiv
				tchild = panelIndividualGO.transform.FindChild("Player" + (i + 1));
				var stringScore = giveNoteOfScore(el.score);
				tchild.FindChild("Name").GetComponent<UILabel>().text = el.name;
				
				if(el.score >= 96f)
				{
					if(el.score >= 98f)
					{
						tchild.FindChild("SILVER").gameObject.SetActiveRecursively(true);
					}else if(el.score >= 99f){
						tchild.FindChild("GOLD").gameObject.SetActiveRecursively(true);
					}else if(el.score >= 100f)
					{
						tchild.FindChild("QUAD").gameObject.SetActiveRecursively(true);
					}else{
						tchild.FindChild("BRONZE").gameObject.SetActiveRecursively(true);
					}
				}else{
					tchild.FindChild("Note").GetComponent<UISprite>().spriteName = stringScore.Split(';')[1];
				}
				
				if(stringScore.Split(';')[0] == "-")
				{
					tchild.FindChild("Lucky").gameObject.active = true;
				}
				else if(stringScore.Split(';')[0] == "+")
				{
					tchild.FindChild("Superb").gameObject.active = true;
				}
				
				tchild.FindChild("Result").GetComponent<UILabel>().text = "Result : [FCOLOR]" + el.fcount + "[FFFFFF]/[ECOLOR]" + el.excount + "[FFFFFF]/[GCOLOR]" + el.gcount +  "[FFFFFF]/[DCOLOR]" + el.dcount + "[FFFFFF]/[WCOLOR]" + el.wcount + "[FFFFFF]/[MCOLOR]" + el.mcount;
				
				tchild.FindChild("Mistake").GetComponent<UILabel>().text = "First Mistake : " + (el.firstmistake < 0f ? "None" : (el.firstmistake.ToString("0.0") + "%"));
				tchild.FindChild("Combo").GetComponent<UILabel>().text = "Max Combo : " + el.maxCombo;
				tchild.FindChild("Prec").GetComponent<UILabel>().text = "Av. Prec. : " + el.average + "ms";
				
				if(comboType < 3)
				{
					tchild.FindChild("ComboType").GetComponent<UISprite>().spriteName = (score >= 100f ? "Perfect" : 
																							(el.comboType == 0 ? "FFC" :
																								el.ComboType == 1 ? "FEC" :
																									el.ComboType == 2 ? "FC"))
				}
			}
		}
		
		//Leaderboard
		var splitLeaderBoard = stringLeaderboard.Split(':');
		for(int i=0; i<8; i++)
		{
			if(i >= splitLeaderBoard.Lenght)
			{
				panelMatchGO.transform.FindChild("Player" + (i + 1)).gameObject.setActiveRecursively(false);
			}else{
				panelMatchGO.transform.FindChild("Player" + (i + 1)).gameObject.setActiveRecursively(true);
				var childLB = splitLeaderBoard.Split(';');
				var tchild = panelMatchGO.transform.FindChild("Player" + (i + 1));
				
				tchild.FindChild("Name").GetComponent<UILabel>().text = childLB[0];
				tchild.FindChild("Score").GetComponent<UILabel>().text = childLB[1];
			}
		}
		
		
		
	}
	
}
