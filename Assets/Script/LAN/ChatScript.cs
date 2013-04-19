using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ChatScript : MonoBehaviour {
	
	private string dialog;
	private List<string> dialogListed;
	
	public int maxLineCount;
	
	private bool activateChat;
	
	private bool lockButton;
	
	//GUI 
	public GUISkin skin;
	public Rect posTextField;
	public Rect posTextArea;
	public Rect posButtonHideChat;
	public Vector2 decalChatWindow;
	private bool chatActive;
	private bool popinChatTrigger;
	private bool popoutChatTrigger;
	private float valueDecal;
	public float speedDecalChatWindow;
	
	public bool callSongList = false;
	
	private bool newMessage;
	private float time;
	public float speedclignNewMessage;
	
	private string tmpDialog;
	// Use this for initialization
	void Start () {
		dialog = "";
		tmpDialog = "";
		valueDecal = 1;
		chatActive = false;
		popinChatTrigger = false;
		popoutChatTrigger = false;
		lockButton = false;
		time = 0f;
		dialogListed = new List<string>();
	}
	
	// Update is called once per frame
	void Update () {
		if(popinChatTrigger)
		{
			popinChat();	
		}
		
		if(popoutChatTrigger)
		{
			popoutChat();	
		}
	}
	
	void OnGUI()
	{
		if(activateChat)
		{
			GUI.skin = skin;
			GUI.depth = 100;
	
			//Voir ici pour le customStyle
	
			GUI.TextArea(new Rect(posTextArea.x*Screen.width + (decalChatWindow.x*valueDecal)*Screen.width, 
				posTextArea.y*Screen.height + (decalChatWindow.y*valueDecal)*Screen.height, 
				posTextArea.width*Screen.width, posTextArea.height*Screen.height), dialog, 2000);
			
			tmpDialog = GUI.TextField(new Rect(posTextField.x*Screen.width + (decalChatWindow.x*valueDecal)*Screen.width, 
				posTextField.y*Screen.height + (decalChatWindow.y*valueDecal)*Screen.height, 
				posTextField.width*Screen.width, posTextField.height*Screen.height), tmpDialog, 160);
			
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Return && !String.IsNullOrEmpty(tmpDialog))
			{
				networkView.RPC("addLine", RPCMode.All, ProfileManager.Instance.currentProfile.name, tmpDialog);
				tmpDialog = "";
			}
			
			if(newMessage)
			{
				GUI.color = new Color(1f, 1f, 1f - Mathf.PingPong(time, 0.75f), 1f);
				time += Time.deltaTime*speedclignNewMessage;
			}else
			{
				GUI.color = new Color(1f, 1f, 1f, 1f);	
			}
			
			if(GUI.Button(new Rect(posButtonHideChat.x*Screen.width, posButtonHideChat.y*Screen.height, posButtonHideChat.width*Screen.width, posButtonHideChat.height*Screen.height), chatActive ? "Hide" : "Chat") && !lockButton)
			{
				if(chatActive)
				{
					popoutChatTrigger = true;
					if(callSongList)
					{
						if(!GetComponent<GeneralScriptLAN>().getZoneOption().activeModule)
						{
							GetComponent<SongZoneLAN>().onPopin();
						}
					}
				}else{
					popinChatTrigger = true;
					if(callSongList)
					{
						if(!GetComponent<GeneralScriptLAN>().getZoneOption().activeModule)
						{
							GetComponent<SongZoneLAN>().onPopout();
						}
					}
				}
			}
		}
	}
	
	
	
	public string getDialog()
	{
		return dialog;
	}
	
	public void sendDirectMessage(string name, string text)
	{
		networkView.RPC("addLine", RPCMode.All, name, text);
	}
		
	[RPC]
	public void addLine(string name, string text)
	{

		dialog += name + " : " + text + "\n";
		dialogListed.Add(name + " : " + text + "\n");
		if(dialogListed.Count > maxLineCount)
		{
			dialog = dialog.Remove(0, dialogListed.ElementAt(0).Length);
			dialogListed.RemoveAt(0);
		}
		newMessage = !chatActive;
		
	}
	
	public void cleanText()
	{
		dialog = "";	
		dialogListed.Clear();
	}
	
	public void popoutChat()
	{
		if(valueDecal < 0.99f){
			valueDecal = Mathf.Lerp(valueDecal, 1f, speedDecalChatWindow*Time.deltaTime);	
		}else{
			valueDecal = 1f;	
			popoutChatTrigger = false;
			chatActive = false;
		}
		
	}
	
	public void popinChat()
	{
		newMessage = false;
		if(valueDecal > 0.01f){
			valueDecal = Mathf.Lerp(valueDecal, 0f, speedDecalChatWindow*Time.deltaTime);	
		}else{
			valueDecal = 0f;	
			popinChatTrigger = false;
			chatActive = true;
		}
	}
	
	public void forcePopinChat()
	{
		popoutChatTrigger = false;
		chatActive = false;
		popinChatTrigger = true;
	}
	
	public void forcePopoutChat()
	{
		popinChatTrigger = false;
		chatActive = true;
		popoutChatTrigger = true;
	}
	
	public void activeChat(bool activate)
	{
		activateChat = activate;
	}
	
	public bool isActive()
	{
		return chatActive;	
	}
	
	public void setLockButton(bool lockbut)
	{
		lockButton = lockbut;	
	}
}
