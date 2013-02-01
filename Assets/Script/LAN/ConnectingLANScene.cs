using UnityEngine;
using System.Collections;

public class ConnectingLANScene : MonoBehaviour {
	
	public GameObject cubeLoading;
	public GameObject cubePlayers;
	public GameObject cubePlayersPosition;
	public GameObject hiddenPart;

	public LANConnexionState stateScene;
	
	private NetworkScript nets;
	
	
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
	public Rect posName;
	
	//PLAYERPERFS SCENE
	public Rect posNumberSamePack;
	public Rect posNumberSongs;
	public Rect posChoiceDifficulty;
	public Rect posToggleDifficulty;
	public Rect posBackButton;
	
	//CONNEXION IN/OUT
	public Rect posInfoConnexion;
	
	/**
	 * TO DO :
	 * - Regarder la gestion des id players
	 * - Gérer les connexion entrante et sortante (envoi du pseudo, verification de l'id dans les profiles joueurs)
	 * - Gérer les connexions sortantes non-dernière
	 * - Envoi/Reception de profile (RPC byte[]) via bouton
	 * - Envoie du nom des packs via RPC string => Verification de disponibilité en live => Beaucoup plus simple.
	 * - Mode "ready".
	 * - Debut de partie.
	 */
	
	// Use this for initialization
	void Start () {
		TextManager.Instance.LoadTextFile();
		nets = GetComponent<NetworkScript>();
		stateScene = LANConnexionState.LOADING;
		
		networkStarted = false;
		actualColor = 0f;
		sens = 1f;
		time = 0f;
		
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
			cubePlayers.transform.GetChild(0).renderer.material.color = new Color(1f, 1f, 1f, 1f);
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
			break;
		case LANConnexionState.IDLE:
			if(LANManager.Instance.isCreator) OnGUIIP();
			break;
		default:
			break;
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
	}
	
	void OnGUIIP()
	{
		GUI.color = new Color(0f, 0f, 0f, 1f);
		GUI.Label(new Rect(posMyIp.x*Screen.width + 1, posMyIp.y*Screen.height + 1, posMyIp.width*Screen.width, posMyIp.height*Screen.height), "Share your IP :\n" + LANManager.Instance.actualIP + ":" + LANManager.Instance.actualPort, "big");
		GUI.color = new Color(1f, 1f, 1f, 1f);
		GUI.Label(new Rect(posMyIp.x*Screen.width, posMyIp.y*Screen.height, posMyIp.width*Screen.width, posMyIp.height*Screen.height), "Share your IP :\n" + LANManager.Instance.actualIP + ":" + LANManager.Instance.actualPort, "big");
	}
}