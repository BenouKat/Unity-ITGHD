using UnityEngine;
using System.Collections;

public class ConnectingLANScene : MonoBehaviour {
	
	public GameObject cubeLoading;

	public LANConnexionState stateScene;
	
	private NetworkScript nets;
	
	
	//GUI
	public GUISkin skin;
	public Rect connectingLabel;
		
		
	public float speedColorLoading;
	private float actualColor;
	private float sens;
	
	private float time;
	private bool networkStarted;
	
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
			//nets.StartNetwork();	
		}
	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		switch(stateScene)
		{
		case LANConnexionState.LOADING:
			OnGUILoading();
			break;
		default:
			break;
		}
		
	}
	
	void OnGUILoading()
	{
		GUI.Label(new Rect(connectingLabel.x*Screen.width, connectingLabel.y*Screen.height, connectingLabel.width*Screen.width, connectingLabel.height*Screen.height), LANManager.Instance.isCreator ? TextManager.Instance.texts["LAN"]["NETWORKInitialize"] : TextManager.Instance.texts["LAN"]["NETWORKConnecting"], "centered");
	}
}