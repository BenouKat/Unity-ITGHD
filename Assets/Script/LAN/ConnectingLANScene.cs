using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ConnectingLANScene : MonoBehaviour {
	
	public GameObject cubeLoading;
	public GameObject cubePlayers;
	public GameObject cubePlayersPosition;
	public GameObject cubePlayerPositionFinal;
	public GameObject hiddenPart;
	
	//for server
	public Dictionary<GameObject, NetworkPlayer> hitNet;
	
	//for clients
	public Dictionary<GameObject, CublastPlayer> hitNetClient;
	private int previousLenghtConnected;
	
	//for everyone
	public List<GameObject> profileAlreadyGetted;
	public LANConnexionState stateScene;
	
	private NetworkScript nets;
	
	private Dictionary<string, Texture2D> tex;
	
	private FadeManager fm;
	
	//GUI
	public GUISkin skin;
	public Rect posMyIp;
	
	//CONNECTING SCENE	
	public Rect connectingLabel;
	public float speedColorLoading;
	private float actualColor;
	private float sens;
	
	private float time;
	private bool networkStarted;
	
	
	//INIT SCENE
	public float speedTranslationPlayercubes;
	public float speedTranslationCubes;
	
	
	//IDLE SCENE
	public Rect posMode;
	public Rect posRound;
	public Rect posSongSelection;
	public Rect posDifficultyChosen;
	public Rect posName;
	public Rect posVictory;
	public Rect posDifficulty;
	public Rect posButtonDifficulty;
	public Rect posGetProfile;
	public Rect posButtonAskPack;
	public Rect posReadyButton;
	
	
	private float alphaDescription;
	public float speedAlphaDescription;
	private GameObject playerSelected;
	
	private bool somethingSelected;
	private bool locked;
	private bool isReady;
	public Rect posQuitButton;
	public Rect posLaunchGameButton;
	
	//ENTERING SCENE
	public float startSpeedRotation;
	public float timeSlowRotation;
	public float timeEnd;
	private float countForTimeEnd;
	public float speedMoveHalo;
	public ParticleSystem halo;
	public ParticleSystem cubeMid;
	public Rect finalModePos;
	
	// Use this for initialization
	void Start () {
		
		nets = GetComponent<NetworkScript>();
		stateScene = LANConnexionState.LOADING;
	
		hitNet = new Dictionary<GameObject, NetworkPlayer>();

		hitNetClient = new Dictionary<GameObject, CublastPlayer>();
		previousLenghtConnected = 0;
		
		
		networkStarted = false;
		actualColor = 0f;
		sens = 1f;
		time = 0f;
		alphaDescription = 0f;
		
		locked = false;
		somethingSelected = false;
		isReady = false;
		countForTimeEnd = 0f;
		tex = new Dictionary<string, Texture2D>();
		tex.Add("mode0", (Texture2D) Resources.Load("LANFFA"));
		tex.Add("mode1", (Texture2D) Resources.Load("LANScoreTournament"));
		tex.Add("mode2", (Texture2D) Resources.Load("LANPointTournament"));
		tex.Add("mode3", (Texture2D) Resources.Load("LANElimination"));
		tex.Add("black", (Texture2D) Resources.Load("black"));
		
		if(LANManager.Instance.isCreator)
		{
			profileAlreadyGetted.Add(cubePlayers.transform.GetChild(0).gameObject);
		}
		
		fm = GetComponent<FadeManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
		switch(stateScene)
		{
		case LANConnexionState.LOADING:
			UpdateLoading();
			break;
		case LANConnexionState.INITIALIZESCENE:
			UpdateInitScene();
			break;
		case LANConnexionState.IDLE:
			UpdateIdleScene();
			break;
		case LANConnexionState.FAIL:
			break;
		case LANConnexionState.ANIMENTERING:
			UpdateEnteringScene();
			break;
		default:
			break;
		}
	}

	void UpdateLoading()
	{
		
		if(actualColor >= 1f && sens > 0f)
		{
			sens = -1f;
		}
		else if(actualColor <= 0.3f && sens < 0f)
		{
			sens = 1f;
		}
		
		actualColor += speedColorLoading*Time.deltaTime*sens;
		
		cubeLoading.renderer.material.color = new Color(actualColor, actualColor, actualColor, 1f);
		
		time += Time.deltaTime;
		
		if(time >= 1f && !networkStarted)
		{
			networkStarted = true;
			nets.StartNetwork();
		}
	}
	
	
	void UpdateInitScene()
	{
		if(actualColor > 0)
		{
			actualColor -= speedColorLoading*Time.deltaTime;
			cubeLoading.renderer.material.color = new Color(actualColor, actualColor, actualColor, 1f);
		}else if(cubeLoading.active)
		{
			cubeLoading.active = false;	
			hiddenPart.SetActiveRecursively(true);
			for(int i = 0; i < hiddenPart.transform.childCount; i++)
			{
				hiddenPart.transform.GetChild(i).gameObject.active = true;
			}
			if(LANManager.Instance.isCreator)
			{
				cubePlayers.transform.GetChild(0).renderer.material.color = new Color(1f, 1f, 1f, 1f);
			}
			
		}
		
		if(hiddenPart.active)
		{
			
			cubePlayers.transform.position = Vector3.Lerp(cubePlayers.transform.position, cubePlayersPosition.transform.position, speedTranslationCubes*Time.deltaTime);
			for(int i = 0; i < cubePlayers.transform.GetChildCount(); i ++)
			{
				cubePlayers.transform.GetChild(i).localPosition = Vector3.Lerp(cubePlayers.transform.GetChild(i).localPosition, cubePlayersPosition.transform.GetChild(i).localPosition, speedTranslationPlayercubes*Time.deltaTime);	
			}
			
			if((Vector3.Distance(cubePlayers.transform.position, cubePlayersPosition.transform.position) <= 0.01f) && 
				(Vector3.Distance(cubePlayers.transform.GetChild(0).localPosition, cubePlayersPosition.transform.GetChild(0).localPosition) <= 0.01f))
			{
				cubePlayers.transform.position = Vector3.Lerp(cubePlayers.transform.position, cubePlayersPosition.transform.position, speedTranslationCubes*Time.deltaTime);
				for(int i = 0; i < cubePlayers.transform.GetChildCount(); i ++)
				{
					cubePlayers.transform.GetChild(i).localPosition = Vector3.Lerp(cubePlayers.transform.GetChild(i).localPosition, cubePlayersPosition.transform.GetChild(i).localPosition, speedTranslationPlayercubes*Time.deltaTime);	
				}	
				
				stateScene = LANConnexionState.IDLE;
			}
			
			
		}
		
		
	}
	
	void UpdateIdleScene()
	{
		
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
		RaycastHit hit;
		
		if(!locked)
		{
			
			if(Physics.Raycast(ray, out hit))
			{
				var theGO = hit.transform.gameObject;
				
				if(theGO != null && theGO.tag == "MenuItem" && (hitNet.Keys.Contains(theGO) || hitNetClient.Keys.Contains(theGO)))
				{
					if(playerSelected != null) playerSelected.renderer.material.color = idleColor(playerSelected);
					playerSelected = theGO;
					playerSelected.renderer.material.color = new Color(1f, 1f, 0.5f, 1f);
					somethingSelected = true;
					
					if(Input.GetMouseButtonDown(0))
					{
						locked = true;
						playerSelected.renderer.material.color = new Color(1f, 0.9f, 0f, 1f);
					}
				}else
				{
					if(playerSelected != null) playerSelected.renderer.material.color = idleColor(playerSelected);
					somethingSelected = false;	
				}
				
			}else{
				if(playerSelected != null) playerSelected.renderer.material.color = idleColor(playerSelected);
				somethingSelected = false;	
			}
		}
		
		if(Input.GetMouseButtonDown(1))
		{
			locked = false;
		}
		
		
		if(somethingSelected && alphaDescription < 1f)
		{
			alphaDescription += speedAlphaDescription*Time.deltaTime;	
		}else if(!somethingSelected && alphaDescription > 0f){
			alphaDescription -= speedAlphaDescription*Time.deltaTime;
		}
	}
	
	void UpdateEnteringScene()
	{
		
		if(countForTimeEnd >= timeEnd)
		{
			LANManager.Instance.statut = LANStatut.SELECTSONG;
			Application.LoadLevel("LANWheel");
		}
		
		for(int i=0; i < cubePlayerPositionFinal.transform.GetChildCount(); i++)
		{
			cubePlayers.transform.GetChild(i).position = Vector3.Lerp(cubePlayers.transform.GetChild(i).position, cubePlayerPositionFinal.transform.GetChild(i).position, speedTranslationCubes*Time.deltaTime);
		}
		cubePlayerPositionFinal.transform.Rotate(new Vector3(startSpeedRotation*Time.deltaTime, 0f, 0f));
		startSpeedRotation = Mathf.Lerp(startSpeedRotation, 0, timeSlowRotation*Time.deltaTime);
		
		halo.transform.Translate(-speedMoveHalo*Time.deltaTime, 0f, 0f);
		
		countForTimeEnd += Time.deltaTime;
	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		switch(stateScene)
		{
		case LANConnexionState.LOADING:
			OnGUILoading();
			break;
		case LANConnexionState.FAIL:
			OnGUIFail();
			break;
		case LANConnexionState.INITIALIZESCENE:
			OnGUIInitScene();
			if(LANManager.Instance.isCreator) OnGUIIP();
			OnGUIInfoParty();
			break;
		case LANConnexionState.IDLE:
			if(LANManager.Instance.isCreator) OnGUIIP();
			OnGUIInfoParty();
			OnGUIIdle();
			break;
		case LANConnexionState.ANIMENTERING:
			OnGUIAnimEntering();
			break;
		default:
			break;
		}
		
	}
	
	void OnGUIAnimEntering()
	{
		
		if(countForTimeEnd >= time - 4f)
		{
			GUI.color = new Color(1f, 1f, 1f, countForTimeEnd <= timeEnd - 3f ? 1f - (timeEnd - 3f - countForTimeEnd) : 1f);
			GUI.DrawTexture(new Rect(finalModePos.x*Screen.width, finalModePos.y*Screen.height, finalModePos.width*Screen.width, finalModePos.height*Screen.height), tex["mode" + (int)LANManager.Instance.modeLANselected]);
		}
		if(countForTimeEnd >= timeEnd - 1.5f)
		{
			GUI.color = new Color(1f, 1f, 1f, countForTimeEnd <= timeEnd - 0.5f ? 1f - (timeEnd - 0.5f - countForTimeEnd) : 1f);
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), tex["black"]);
		}
	}
	
	void OnGUILoading()
	{
		GUI.Label(new Rect(connectingLabel.x*Screen.width, connectingLabel.y*Screen.height, connectingLabel.width*Screen.width, connectingLabel.height*Screen.height), LANManager.Instance.isCreator ? TextManager.Instance.texts["LAN"]["NETWORKInitialize"] : TextManager.Instance.texts["LAN"]["NETWORKConnecting"], "centered");
	}
	
	void OnGUIInitScene()
	{
		GUI.Label(new Rect(connectingLabel.x*Screen.width, connectingLabel.y*Screen.height, connectingLabel.width*Screen.width, connectingLabel.height*Screen.height), LANManager.Instance.isCreator ? TextManager.Instance.texts["LAN"]["NETWORKInitSuccess"] : TextManager.Instance.texts["LAN"]["NETWORKSuccess"], "centered");
	}
	
	void OnGUIFail()
	{
		GUI.Label(new Rect(connectingLabel.x*Screen.width, connectingLabel.y*Screen.height, connectingLabel.width*Screen.width, connectingLabel.height*Screen.height), LANManager.Instance.isCreator ? TextManager.Instance.texts["LAN"]["NETWORKInitFail"] + "\n" + LANManager.Instance.errorToDisplay : TextManager.Instance.texts["LAN"]["NETWORKFail"] + "\n" + LANManager.Instance.errorToDisplay, "centered");
	
		if(GUI.Button(new Rect(posQuitButton.x*Screen.width, posQuitButton.y*Screen.height, posQuitButton.width*Screen.width, posQuitButton.height*Screen.height), "Quit")){
			fm.FadeIn("LAN");	
		}
	
	}
	
	void OnGUIInfoParty()
	{
		if(LANManager.Instance.dataArrived)
		{
			GUI.DrawTexture(new Rect(posMode.x*Screen.width, posMode.y*Screen.height, posMode.width*Screen.width, posMode.height*Screen.height), tex["mode" + (int)LANManager.Instance.modeLANselected]);
			if(LANManager.Instance.modeLANselected > LANMode.FFA)
			{
				GUI.Label(new Rect(posRound.x*Screen.width, posRound.y*Screen.height, posRound.width*Screen.width, posRound.height*Screen.height), "Rounds : " + LANManager.Instance.roundNumber);
			}
		
			GUI.Label(new Rect(posSongSelection.x*Screen.width, posSongSelection.y*Screen.height, posSongSelection.width*Screen.width, posSongSelection.height*Screen.height), "Selection : " + LANManager.Instance.convertHostSystemToString(LANManager.Instance.hostSystem));
		
			GUI.Label(new Rect(posDifficultyChosen.x*Screen.width, posDifficultyChosen.y*Screen.height, posDifficultyChosen.width*Screen.width, posDifficultyChosen.height*Screen.height), "Song difficulty : " + LANManager.Instance.convertDifficultyModeToString(LANManager.Instance.songDiffSystem));
		}
	}
	
	void OnGUIIP()
	{
		if(LANManager.Instance.isCreator)
		{
			var localText = LANManager.Instance.actualIP.Contains("192.168") ? "\n(Local address)" : "";
			GUI.color = new Color(0f, 0f, 0f, 1f);
			GUI.Label(new Rect(posMyIp.x*Screen.width + 1, posMyIp.y*Screen.height + 1, posMyIp.width*Screen.width, posMyIp.height*Screen.height), "Share your IP :\n" + LANManager.Instance.actualIP + ":" + LANManager.Instance.actualPort + localText, "big");
			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.Label(new Rect(posMyIp.x*Screen.width, posMyIp.y*Screen.height, posMyIp.width*Screen.width, posMyIp.height*Screen.height), "Share your IP :\n" + LANManager.Instance.actualIP + ":" + LANManager.Instance.actualPort + localText, "big");
		}
	}
	
	void OnGUIIdle()
	{
		if(playerSelected != null)
		{
			GUI.color = new Color(1f, 1f, 1f, alphaDescription);
			if(LANManager.Instance.isCreator)
			{	
				var player = LANManager.Instance.players[hitNet[playerSelected]];
				GUI.Label(new Rect(posName.x*Screen.width, posName.y*Screen.height, posName.width*Screen.width, posName.height*Screen.height), player.name);
				GUI.Label(new Rect(posVictory.x*Screen.width, posVictory.y*Screen.height, posVictory.width*Screen.width, posVictory.height*Screen.height), "Vict : " + player.victoryOnline);
				GUI.Label(new Rect(posDifficulty.x*Screen.width, posDifficulty.y*Screen.height, posDifficulty.width*Screen.width, posDifficulty.height*Screen.height), "Difficulty selection : " + LANManager.Instance.convertDifficultyModeToString(player.difficultyMode));	
				
				if(LANManager.Instance.songDiffSystem == 2)
				{
					if(GUI.Button(new Rect(posButtonDifficulty.x*Screen.width, posButtonDifficulty.y*Screen.height, posButtonDifficulty.width*Screen.width, posButtonDifficulty.height*Screen.height), "Switch") && locked){	
						LANManager.Instance.players[hitNet[playerSelected]].difficultyMode = (LANManager.Instance.players[hitNet[playerSelected]].difficultyMode == 0 ? 1 : 0);
						networkView.RPC("changeRightDifficulty", hitNet[playerSelected], LANManager.Instance.players[hitNet[playerSelected]].difficultyMode == 1);
					}
				}
				
				if(!profileAlreadyGetted.Contains(playerSelected))
				{
					if(GUI.Button(new Rect(posGetProfile.x*Screen.width, posGetProfile.y*Screen.height, posGetProfile.width*Screen.width, posGetProfile.height*Screen.height), "Get Profile"))
					{
						var pl = LANManager.Instance.players[hitNet[playerSelected]];
						Debug.Log("Send : " + pl.name + " : " + pl.idFile + " : " + Network.player.ToString());
						nets.notifyPlayerForProfile(pl.name, pl.idFile, Network.player);
						profileAlreadyGetted.Add(playerSelected);
					}
				}
			}else{
				var player = hitNetClient[playerSelected];
				GUI.Label(new Rect(posName.x*Screen.width, posName.y*Screen.height, posName.width*Screen.width, posName.height*Screen.height), player.name);
				GUI.Label(new Rect(posVictory.x*Screen.width, posVictory.y*Screen.height, posVictory.width*Screen.width, posVictory.height*Screen.height), "Vict : " + player.victoryOnline);
				
				if(!profileAlreadyGetted.Contains(playerSelected))
				{
					if(GUI.Button(new Rect(posGetProfile.x*Screen.width, posGetProfile.y*Screen.height, posGetProfile.width*Screen.width, posGetProfile.height*Screen.height), "Get Profile"))
					{
						var pl = hitNetClient[playerSelected];
						Debug.Log("Send : " + pl.name + " : " + pl.idFile + " : " + Network.player.ToString());
						nets.callProfileToServer(pl.name, pl.idFile, Network.player);
						
						profileAlreadyGetted.Add(playerSelected);
					}
				}
			
			}
		}
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		if(LANManager.Instance.isCreator)
		{
			
			
			if(GUI.Button(new Rect(posButtonAskPack.x*Screen.width, posButtonAskPack.y*Screen.height, 
				posButtonAskPack.width*Screen.width, posButtonAskPack.height*Screen.height), "Check pack")){
				GetComponent<ChatScript>().sendDirectMessage("Info", LANManager.Instance.returnPackAvailableText());
			}
			
			if(LANManager.Instance.players.Count > 1)
			{
				if(GUI.Button(new Rect(posLaunchGameButton.x*Screen.width, posLaunchGameButton.y*Screen.height, 
					posLaunchGameButton.width*Screen.width, posLaunchGameButton.height*Screen.height), "Start"))
				{
					var someoneNotReady = false;	
					for(int i = 0; i < LANManager.Instance.players.Count; i++)
					{
						if(!LANManager.Instance.players.ElementAt(i).Value.isReady)
						{
							var cubeSelec = hitNet.FirstOrDefault(c => c.Value == LANManager.Instance.players.ElementAt(i).Key).Key;
							cubeSelec.transform.Find("3 - Unready").gameObject.active = true;
							cubeSelec.transform.Find("3 - Unready").particleSystem.Play();
							GetComponent<ChatScript>().sendDirectMessage("Info", LANManager.Instance.players.ElementAt(i).Value.name + " is not ready");
							someoneNotReady = true;
						}
					}
					if(!someoneNotReady)
					{
						//launch	
						networkView.RPC("launchGame", RPCMode.All);
					}
				}
			}
		}
		
		if(!isReady)
		{
			if(GUI.Button(new Rect(posReadyButton.x*Screen.width, posReadyButton.y*Screen.height, posReadyButton.width*Screen.width, posReadyButton.height*Screen.height), "Ready")){

				nets.callPlayerReady(ProfileManager.Instance.currentProfile.name, ProfileManager.Instance.currentProfile.idFile);	
				
				isReady = true;
			}
		}
		
		if(GUI.Button(new Rect(posQuitButton.x*Screen.width, posQuitButton.y*Screen.height, posQuitButton.width*Screen.width, posQuitButton.height*Screen.height), "Quit")){
			Network.Disconnect();
			LANManager.Instance.players.Clear();
			fm.FadeIn("LAN");	
		}
		
		
	}
	
	
	//Only server
	public bool addHitNet(NetworkPlayer player)
	{
		if(hitNet.Count < 8)
		{
			for(var i = 0; i < cubePlayers.transform.childCount; i++)
			{
				if(!hitNet.ContainsKey(cubePlayers.transform.GetChild(i).gameObject))
				{
					hitNet.Add(cubePlayers.transform.GetChild(i).gameObject, player);
					
					cubePlayers.transform.GetChild(i).Find("0 - Connected").gameObject.active = true;
					cubePlayers.transform.GetChild(i).Find("0 - Connected").particleSystem.Play();
					cubePlayers.transform.GetChild(i).gameObject.renderer.material.color = new Color(1f, 1f, 1f, 1f);
					return true;
				}
			}
		}
		return false;
	}
	
	//Only server
	public void removeHitNet(NetworkPlayer player)
	{
		var cube = hitNet.First(c => c.Value == player).Key;
		cube.transform.Find("1 - Disconnected").gameObject.active = true;
		cube.transform.Find("1 - Disconnected").particleSystem.Play();
		cube.renderer.material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		if(playerSelected == cube)
		{
			playerSelected = null;	
		}
		hitNet.Remove(cube);
	}
	
	//Only client
	public void associateCubeToString(string name, string id, bool ready, int vict)
	{
		for(int i = 0; i < cubePlayers.transform.childCount; i++)
		{
			
			if(!hitNetClient.ContainsKey(cubePlayers.transform.GetChild(i).gameObject))
			{
				if(name == ProfileManager.Instance.currentProfile.name && id == ProfileManager.Instance.currentProfile.idFile)
				{
					profileAlreadyGetted.Add(cubePlayers.transform.GetChild(i).gameObject);
				}
				hitNetClient.Add(cubePlayers.transform.GetChild(i).gameObject, new CublastPlayer(name, vict, ready, id));	
				cubePlayers.transform.GetChild(i).gameObject.renderer.material.color = ready ? new Color(0.5f, 0.8f, 1f, 1f) : new Color(1f, 1f, 1f, 1f);
				break;
			}
		}
		
	}
	
	//Only client
	public void checkForVerificationConnected(string nameEvent, string id, Dictionary<GameObject, CublastPlayer> oldList)
	{
		if(previousLenghtConnected == 0)
		{
			previousLenghtConnected = hitNetClient.Count;	
			
		}else if (previousLenghtConnected != hitNetClient.Count)
		{
			if(previousLenghtConnected < hitNetClient.Count)
			{
				for(int i=0; i < hitNetClient.Count; i++)
				{
					if(hitNetClient.ElementAt(i).Value.name == nameEvent && hitNetClient.ElementAt(i).Value.idFile == id)
					{
						
						hitNetClient.ElementAt(i).Key.transform.Find("0 - Connected").gameObject.active = true;
						hitNetClient.ElementAt(i).Key.transform.Find("0 - Connected").particleSystem.Play();
						hitNetClient.ElementAt(i).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f);
						break;
					}
					
				}
			}else if(previousLenghtConnected > hitNetClient.Count)
			{
				for(int i=0; i < oldList.Count; i++)
				{
					if(oldList.ElementAt(i).Value.name == nameEvent && oldList.ElementAt(i).Value.idFile == id)
					{
						if(profileAlreadyGetted.Contains(oldList.ElementAt(i).Key))
						{
							profileAlreadyGetted.Remove(oldList.ElementAt(i).Key);
						}
						oldList.ElementAt(i).Key.transform.Find("1 - Disconnected").gameObject.active = true;
						oldList.ElementAt(i).Key.transform.Find("1 - Disconnected").particleSystem.Play();
						oldList.ElementAt(i).Key.renderer.material.color = new Color(0.5f, 0.5f, 0.5f, 1f);
						break;
					}
				}
			}
			oldList.Clear();
			previousLenghtConnected = hitNetClient.Count;
		}
		
		if(playerSelected != null && !hitNetClient.ContainsKey(playerSelected))
		{
			playerSelected = null;
		}
	}
	
	public void checkPlayerReady(string name, string id)
	{
		if(LANManager.Instance.isCreator)
		{
			var key = LANManager.Instance.players.FirstOrDefault(c => c.Value.name == name && c.Value.idFile == id).Key;
			var realkey = hitNet.FirstOrDefault(c => c.Value == key).Key;
			realkey.transform.Find("2 - Ready").gameObject.active = true;
			realkey.transform.Find("2 - Ready").particleSystem.Play();
			realkey.renderer.material.color = new Color(0.5f, 0.8f, 1f, 1f);
			LANManager.Instance.players[key].isReady = true;
		}else{
			var key = hitNetClient.FirstOrDefault(c => c.Value.name == name && c.Value.idFile == id).Key;
			key.transform.Find("2 - Ready").gameObject.active = true;
			key.transform.Find("2 - Ready").particleSystem.Play();
			key.renderer.material.color = new Color(0.5f, 0.8f, 1f, 1f);
			hitNetClient[key].isReady = true;
		}
		
		
	}
	
	
	public Color idleColor(GameObject go)
	{
		if(LANManager.Instance.isCreator)
		{
			if(LANManager.Instance.players[hitNet[go]].isReady)
			{
				return new Color(0.5f, 0.8f, 1f, 1f);	
			}
			return new Color(1f, 1f, 1f, 1f);
		}else
		{
			if(hitNetClient[go].isReady)
			{
				return new Color(0.5f, 0.8f, 1f, 1f);	
			}
			return new Color(1f, 1f, 1f, 1f);
		}
	}
}