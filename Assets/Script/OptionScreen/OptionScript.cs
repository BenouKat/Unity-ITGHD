using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

public enum StateOption{
	OPTIONSELECTION,
	CUBEFADEIN,
	SCREENFADEIN,
	CUBEFADEOUT,
	SCREENFADEOUT,
	OPTION
}

public class OptionScript : MonoBehaviour {
	
	
	private GameObject theSelected;
	private GameObject theSelectedTouch;
	public GameObject cubeIcon;
	public GameObject screen;
	public GameObject bgScreen;
	public GameObject[] menuItem;
	public Camera camMapping;
	public GUISkin skin;
	private FadeManager fm;
	private bool alreadyFaded;
	private float timeBeforeFade;
	
	private Vector3[] Rotate;
	public float speedScale;
	private float loadColor;
	public float speedColor;
	public float speedColorFade;
	
	//Confirm Dialog
	public Rect LabelConfirmDialog;
	private bool confirmDialogActivated;
	private string optionConfirm;
	
	//Fade Option
	private float lerpColorScreenFade;
	public float speedFadeScreen;
	public float speedColorScreenFade;
	public Color colorBasicScreen = new Color(0f, 0.075f, 0.05f, 1f);
	//Fade Cube
	private float alphaFadeIn;
	public float speedFadeIn;
	
	//Labels
	public Rect sizeLabelBG;
	public Rect offsetLabelOption;
	public Rect labelOption;
	
	//states
	private StateOption optionMenuMode;
	private string optionSelected;
	private bool fadeOut;
	
	//Option variable
	public Rect posLabelOption;
	public float offsetLabelYOption;
	public Rect posTextFieldOption;
	public Rect posSliderOption;
	public Rect posValueSliderOption;
	public Rect posButtonOption;
	public Rect posButtonBack;
	public Rect posButtonCancel;
	public Rect posLabelListChoice;
	public Rect sizeArrowButton;
	public float offsetBetweenLabel;
	private string errorMessage;
	public Rect posLabelHelp;
	
	//Audio
	private float generalVolume;
	
	//Video
	private bool enableBloom;
	private bool enableDOF;
	private bool onlyOnGame;
	private int antiAliasing;
	
	//KeyMapping
	//8 game objet + 8 label / button + un label g�n�ral
	/*
	0 : Left 1
	1 : Down 1
	2 : Up 1
	3 : Right 1
	4 : Left 2
	5 : Down 2
	6 : Up 2
	7 : Right 2
	*/
	public Rect labelMapping;
	public float offsetYlabelMapping;
	public float offsetXlabelMapping;
	public Rect labelDialogMapPri;
	public Rect labelDialogMapSec;
	public Rect labelInfo;
	public int choicePosition;
	
	public GameObject[] formObject;
	public Transform[] form1;
	public Transform[] form2;
	public Transform[] form3;
	public Vector2[] posLabelPrim;
	public Vector2[] posLabelSec;
	public float speedMovementMapping;
	private bool inPosition;
	
	private float timeStillFade;
	private float fadeColorMapping;
	public float speedColorMapping;
	
	private float alphaBlackMapping;
	public float speedFadeBlackMapping;
	private int indexInputSelected;
	private bool inInputEntryMode;
	
	public GameObject KeyMapParentObject;
	
	//General
	public Rect posButtonCache;
	public Rect posButtonProfile;
	
	private string GOS;
	private string mouseSpeed;
	private bool quickMode;
	private bool padMode;
	private bool cacheMode;
	
	//Network
	private string portPref;
	
	
	//textures
	public Dictionary<string, Texture2D> tex;
	
	//texts
	private Dictionary<string, string> textOption;
	// Use this for initialization
	void Start () {
		
		if(!LoadManager.Instance.alreadyLoaded) TextManager.Instance.LoadTextFile();
		tex = new Dictionary<string, Texture2D>();
		tex.Add("labelbg", (Texture2D) Resources.Load("GUIBarMini"));
		tex.Add("black", (Texture2D) Resources.Load("black"));
		
		optionMenuMode = StateOption.OPTIONSELECTION;
		alphaFadeIn = 1f;
		lerpColorScreenFade = 0f;
		textOption = TextManager.Instance.texts["Option"];
		
		GOS = DataManager.Instance.userGOS.ToString("0.000");
		mouseSpeed = DataManager.Instance.mouseMolSpeed.ToString("0");
		quickMode = DataManager.Instance.quickMode;
		padMode = DataManager.Instance.dancepadMode;
		cacheMode = DataManager.Instance.useTheCacheSystem;
		portPref = LANManager.Instance.actualPort.ToString();
		
		generalVolume = DataManager.Instance.generalVolume * 100f;
		
		enableBloom = DataManager.Instance.enableBloom;
		enableDOF = DataManager.Instance.enableDepthOfField;
		antiAliasing = DataManager.Instance.antiAliasing;
		onlyOnGame = DataManager.Instance.onlyOnGame;
		
		fadeOut = false;
		fm = gameObject.GetComponent<FadeManager>();
		inPosition = false;
		fadeColorMapping = 0f;	
		alphaBlackMapping = 0f;
		inInputEntryMode = false;
		errorMessage = "";
		optionConfirm = "";
		confirmDialogActivated = false;
		timeStillFade = 0f;
		alreadyFaded = false;
		timeBeforeFade = 0f;
	}
	
	void OnGUI(){
		switch(optionMenuMode){
			case StateOption.OPTIONSELECTION:
				OnGUIOptionSelect();
				break;
			case StateOption.CUBEFADEIN:
				OnGUIOptionSelect();
				OnGUIOptionFadeIn();
				break;
			case StateOption.SCREENFADEIN:
				OnGUIOptionFadeIn();
				break;
			case StateOption.OPTION:
				OnGUIOptionFadeIn();
				OnGUIOptionBlackLabel();
				OnGUIOption();
				break;
			case StateOption.SCREENFADEOUT:
				OnGUIOptionFadeIn();
				break;
			case StateOption.CUBEFADEOUT:
				OnGUIOptionSelect();
				OnGUIOptionFadeIn();
				break;
		}
	}
	
	void OnGUIOptionSelect(){
		GUI.skin = skin;
		GUI.color = new Color(1f, 1f, 1f, alphaFadeIn);
		for(int i = 0; i<menuItem.Count(); i++)
		{
			var pos2D = Camera.main.WorldToScreenPoint(menuItem[i].transform.position);
			GUI.color = new Color(1f, 1f, 1f, 0.5f*alphaFadeIn);
			GUI.DrawTexture(new Rect(pos2D.x + (sizeLabelBG.x*Screen.width), Screen.height - pos2D.y + (sizeLabelBG.y*Screen.height), sizeLabelBG.width*Screen.width, sizeLabelBG.height*Screen.height), tex["labelbg"]);
			
			GUI.color = new Color(0f, 0f, 0f, 1f*alphaFadeIn);
			GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width + 1, Screen.height - pos2D.y + offsetLabelOption.y*Screen.height + 1, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), menuItem[i].name, "TitleLabel");
			
			if(theSelected != null && theSelected.name == menuItem[i].name)
			{
				GUI.color = new Color(1f,1f, 1f, 1f*alphaFadeIn*(1 - loadColor));
				GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width, Screen.height - pos2D.y + offsetLabelOption.y*Screen.height, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), menuItem[i].name, "TitleLabel");
			
				GUI.color = new Color(theSelected.renderer.material.color.r, theSelected.renderer.material.color.g, theSelected.renderer.material.color.b, loadColor);
				GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width, Screen.height - pos2D.y + offsetLabelOption.y*Screen.height, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), menuItem[i].name, "TitleLabel");
			}else
			{
				GUI.color = new Color(1f,1f, 1f, 1f*alphaFadeIn);
				GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width, Screen.height - pos2D.y + offsetLabelOption.y*Screen.height, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), menuItem[i].name, "TitleLabel");
			}
		}
	}
	
	void OnGUIOptionFadeIn(){
		GUI.skin = skin;
		GUI.color = new Color(0f, 0f, 0f, 1 - alphaFadeIn);
		GUI.Label(new Rect(labelOption.x*Screen.width + 1, labelOption.y*Screen.height + 1, labelOption.width*Screen.width, labelOption.height*Screen.height), optionSelected, "TitleLabel");
		GUI.color = new Color(theSelected.renderer.material.color.r, theSelected.renderer.material.color.g, theSelected.renderer.material.color.b, 1 - alphaFadeIn);
		GUI.Label(new Rect(labelOption.x*Screen.width, labelOption.y*Screen.height, labelOption.width*Screen.width, labelOption.height*Screen.height), optionSelected, "TitleLabel");
	}
	
	void OnGUIOptionBlackLabel(){
		GUI.color = new Color(0f, 0f, 0f, 1f);
		switch(theSelected.name)
		{
			
			case "General":
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_GOS"]);		
			//MouseSpeed
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1 + offsetLabelYOption*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_MOUSESPEED"]);
		
			//Quickmod
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1 + offsetLabelYOption*2*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_QUICKMODE"]);
			//Padmod
			/*
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1 + offsetLabelYOption*3*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_PADMODE"]);*/
			
			//cache
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1 + offsetLabelYOption*4*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_USECACHE"]);

			break;
			
		//Network
			case "Network":
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["NETWORK_PORT"]);
			
			//GUI.Label(new Rect(posLabelListChoice.x*Screen.width + 1, posLabelListChoice.y*Screen.height + 1, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), networkValue[(int)PDT], "centeredLabel");
			break;
			
			
		//Audio
			case "Audio":
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["AUDIO_GENERAL"]);
			GUI.Label(new Rect(posValueSliderOption.x*Screen.width + 1, posValueSliderOption.y*Screen.height + 1, posValueSliderOption.width*Screen.width, posValueSliderOption.height*Screen.height), generalVolume.ToString("0"));
			break;
			
		//Video
			case "Video":
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_BLOOM"]);

			
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1 + offsetLabelYOption*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_DOF"]);

			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height + offsetLabelYOption*2*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_OOG"]);
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + offsetLabelYOption*3*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_AA"]);

			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width + 1, posLabelListChoice.y*Screen.height + 1 + offsetLabelYOption*3*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), "x" + antiAliasing, "centeredLabel");

			break;
			
			
		//Key Mapping
			case "Key Mapping":
			GUI.Label(new Rect(labelDialogMapPri.x*Screen.width + 1, labelDialogMapPri.y*Screen.height + 1, labelDialogMapPri.width*Screen.width, labelDialogMapPri.height*Screen.height), "Primary");
			GUI.Label(new Rect(labelDialogMapSec.x*Screen.width + 1, labelDialogMapSec.y*Screen.height + 1, labelDialogMapSec.width*Screen.width, labelDialogMapSec.height*Screen.height), "Secondary");
			for(int i=0; i<8; i++){
				var supX = i < 4 ? 0 : 1;
				var supY = i < 4 ? i : i - 4;
				GUI.Label(new Rect(labelMapping.x*Screen.width + 1 + supX*offsetXlabelMapping*Screen.width, labelMapping.y*Screen.height + 1 + supY*offsetYlabelMapping*Screen.width, labelMapping.width*Screen.width, labelMapping.height*Screen.height), giveLabelForIndex(i) + giveCodeForIndex(i).ToString());
			}
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["MAPPING_CHOICE"]);

			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width + 1, posLabelListChoice.y*Screen.height + 1, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), choicePosition == 0 ? "KeyBoard" : choicePosition == 1 ? "Arcade Stick" : "DancePad", "centeredLabel");
			
			GUI.Label(new Rect(labelInfo.x*Screen.width + 1, labelInfo.y*Screen.height + 1, labelInfo.width*Screen.width, labelInfo.height*Screen.height), inInputEntryMode ? textOption["MAPPING_INPUT"] : textOption["MAPPING_INFO"]);
			
			//Faire une anim ?
			
			break;
		}
		
		if(!String.IsNullOrEmpty(errorMessage)){
			GUI.Label(new Rect(posLabelHelp.x*Screen.width + 1, posLabelHelp.y*Screen.height + 1, posLabelHelp.width*Screen.width + 1, posLabelHelp.height*Screen.height + 1), errorMessage);
		}
		
		if(confirmDialogActivated)
		{
			GUI.Label(new Rect(LabelConfirmDialog.x*Screen.width + 1, LabelConfirmDialog.y*Screen.height + 1, LabelConfirmDialog.width*Screen.width, LabelConfirmDialog.height*Screen.height), textOption["CONFIRM_DIALOG"]);
		}
	
	}
	
	
	void OnGUIOption(){
		GUI.color = new Color(1f, 1f, 1f, 1f);
		switch(theSelected.name)
		{
		//General
			case "General":
			//GOS
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_GOS"]);
			GOS = GUI.TextField(new Rect(posTextFieldOption.x*Screen.width, posTextFieldOption.y*Screen.height, posTextFieldOption.width*Screen.width, posTextFieldOption.height*Screen.height), GOS, 5);
		
			//MouseSpeed
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height + offsetLabelYOption*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_MOUSESPEED"]);
			mouseSpeed = GUI.TextField(new Rect(posTextFieldOption.x*Screen.width, posTextFieldOption.y*Screen.height + offsetLabelYOption*Screen.height, posTextFieldOption.width*Screen.width, posTextFieldOption.height*Screen.height), mouseSpeed, 1);
		
			//Quickmod
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height + offsetLabelYOption*2*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_QUICKMODE"]);
			if(quickMode){
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*2*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "GreenButton")){
					quickMode = false;
				}
			}else{
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*2*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "RedButton")){
					quickMode = true;
				}
			}
			
			//Padmod
			/*
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height + offsetLabelYOption*3*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_PADMODE"]);
			if(padMode){
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*3*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "GreenButton")){
					padMode = false;
				}
			}else{
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*3*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "RedButton")){
					padMode = true;
				}
			}
			*/
			
			//cache
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height + offsetLabelYOption*4*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_USECACHE"]);
			if(cacheMode){
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*4*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "GreenButton")){
					cacheMode = false;
				}
			}else{
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*4*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "RedButton")){
					cacheMode = true;
				}
			}
			
			//reload cache
			if(GUI.Button(new Rect(posButtonCache.x*Screen.width, posButtonCache.y*Screen.height, posButtonCache.width*Screen.width, posButtonCache.height*Screen.height), textOption["GENERAL_RELOADCACHE"], "MenuButton")){
				confirmDialogActivated = true;
				optionConfirm = "cache";
				
			}
				
			//Changer profile
			if(GUI.Button(new Rect(posButtonProfile.x*Screen.width, posButtonProfile.y*Screen.height, posButtonProfile.width*Screen.width, posButtonProfile.height*Screen.height), textOption["GENERAL_RELOADPROFILE"], "MenuButton")){
				confirmDialogActivated = true;
				optionConfirm = "profile";
				
			}	
			break;
			
		//Network
			case "Network":
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["NETWORK_PORT"]);
			
			/*
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width + offsetBetweenLabel*Screen.width + sizeArrowButton.x*Screen.width, posLabelListChoice.y*Screen.height + sizeArrowButton.y*Screen.height, sizeArrowButton.width*Screen.width, sizeArrowButton.height*Screen.height), "", "rightArrow")){
				PDT++;
				if((int)PDT > 3) PDT = (ProfileDownloadType)0;
			}
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width - offsetBetweenLabel*Screen.width + sizeArrowButton.x*Screen.width, posLabelListChoice.y*Screen.height + sizeArrowButton.y*Screen.height, sizeArrowButton.width*Screen.width, sizeArrowButton.height*Screen.height), "", "leftArrow")){
				PDT--;
				if((int)PDT < 0) PDT = (ProfileDownloadType)3;
			}
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width, posLabelListChoice.y*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), networkValue[(int)PDT], "centeredLabel");
			*/
			//Faire une anim ?
			
			portPref = GUI.TextField(new Rect(posTextFieldOption.x*Screen.width, posTextFieldOption.y*Screen.height, posTextFieldOption.width*Screen.width, posTextFieldOption.height*Screen.height), portPref, 5);
		
			break;
			
			
		//Audio
			case "Audio":
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["AUDIO_GENERAL"]);
			generalVolume = GUI.HorizontalSlider(new Rect(posSliderOption.x*Screen.width, posSliderOption.y*Screen.height, posSliderOption.width*Screen.width, posSliderOption.height*Screen.height), generalVolume, 0, 100);
			GUI.Label(new Rect(posValueSliderOption.x*Screen.width, posValueSliderOption.y*Screen.height, posValueSliderOption.width*Screen.width, posValueSliderOption.height*Screen.height), generalVolume.ToString("0"));
			break;
			
		//Video
			case "Video":
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_BLOOM"]);
			if(enableBloom){
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "GreenButton")){
					enableBloom = false;
				}
			}else{
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "RedButton")){
					enableBloom = true;
				}
			}
			
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height + offsetLabelYOption*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_DOF"]);
			if(enableDOF){
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "GreenButton")){
					enableDOF = false;
				}
			}else{
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "RedButton")){
					enableDOF = true;
				}
			}
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height + offsetLabelYOption*2*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_OOG"]);
			if(onlyOnGame){
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*2*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "GreenButton")){
					onlyOnGame = false;
				}
			}else{
				if(GUI.Button(new Rect(posButtonOption.x*Screen.width, posButtonOption.y*Screen.height + offsetLabelYOption*2*Screen.height, posButtonOption.width*Screen.width, posButtonOption.height*Screen.height), "", "RedButton")){
					onlyOnGame = true;
				}
			}
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height + offsetLabelYOption*3*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_AA"]);
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width + offsetBetweenLabel*Screen.width + sizeArrowButton.x*Screen.width, posLabelListChoice.y*Screen.height + offsetLabelYOption*3*Screen.height + sizeArrowButton.y*Screen.height, sizeArrowButton.width*Screen.width, sizeArrowButton.width*Screen.height), "", "rightArrow")){
				antiAliasing += 2;
				if(antiAliasing > 8) antiAliasing = 0;
				if(antiAliasing == 6) antiAliasing = 8;
			}
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width - offsetBetweenLabel*Screen.width + sizeArrowButton.x*Screen.width, posLabelListChoice.y*Screen.height + offsetLabelYOption*3*Screen.height + sizeArrowButton.y*Screen.height, sizeArrowButton.width*Screen.width, sizeArrowButton.width*Screen.height), "", "leftArrow")){
				antiAliasing -= 2;
				if(antiAliasing < 0) antiAliasing = 8;
				if(antiAliasing == 6) antiAliasing = 4;
			}
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width, posLabelListChoice.y*Screen.height + offsetLabelYOption*3*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), "x" + antiAliasing, "centeredLabel");
			//Faire une anim ?
			break;
			
			
		//Key Mapping
			case "Key Mapping":
			GUI.Label(new Rect(labelDialogMapPri.x*Screen.width, labelDialogMapPri.y*Screen.height, labelDialogMapPri.width*Screen.width, labelDialogMapPri.height*Screen.height), "Primary");
			GUI.Label(new Rect(labelDialogMapSec.x*Screen.width, labelDialogMapSec.y*Screen.height, labelDialogMapSec.width*Screen.width, labelDialogMapSec.height*Screen.height), "Secondary");
			for(int i=0; i<8; i++){
				var supX = i < 4 ? 0 : 1;
				var supY = i < 4 ? i : i - 4;
				if(indexInputSelected == -1){
					GUI.color = new Color(1f, 1f, 1f, 1f);
				}else if(indexInputSelected != i){
					GUI.color = formObject[i].renderer.material.color;
				}
				GUI.Label(new Rect(labelMapping.x*Screen.width + supX*offsetXlabelMapping*Screen.width, labelMapping.y*Screen.height + supY*offsetYlabelMapping*Screen.width, labelMapping.width*Screen.width, labelMapping.height*Screen.height), giveLabelForIndex(i) + giveCodeForIndex(i).ToString());
				GUI.color = new Color(1f, 1f, 1f, 1f);
			}
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["MAPPING_CHOICE"]);
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width + offsetBetweenLabel*Screen.width + sizeArrowButton.x*Screen.width, posLabelListChoice.y*Screen.height + sizeArrowButton.y*Screen.height, sizeArrowButton.width*Screen.width, sizeArrowButton.height*Screen.height), "", "rightArrow") && !inInputEntryMode){
				choicePosition += 1;
				if(choicePosition > 2) choicePosition = 0;
				inPosition = false;
			}
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width - offsetBetweenLabel*Screen.width + sizeArrowButton.x*Screen.width, posLabelListChoice.y*Screen.height + sizeArrowButton.y*Screen.height, sizeArrowButton.width*Screen.width, sizeArrowButton.height*Screen.height), "", "leftArrow") && !inInputEntryMode){
				choicePosition -= 1;
				if(choicePosition < 0) choicePosition = 2;
				inPosition = false;
			}
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width, posLabelListChoice.y*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), choicePosition == 0 ? "KeyBoard" : choicePosition == 1 ? "Arcade Stick" : "DancePad", "centeredLabel");
			
			GUI.color = new Color(1f, 1f, 1f, alphaBlackMapping);
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), tex["black"]);
			GUI.color = new Color(1f, 1f, 1f, 1f);
			
			GUI.Label(new Rect(labelInfo.x*Screen.width, labelInfo.y*Screen.height, labelInfo.width*Screen.width, labelInfo.height*Screen.height), inInputEntryMode ? textOption["MAPPING_INPUT"] : textOption["MAPPING_INFO"]);
			
			//Faire une anim ?
			
			break;
		}
		
		if(!String.IsNullOrEmpty(errorMessage)){
			GUI.color = new Color(1f, 0.1f, 0.1f, 1f);
			GUI.Label(new Rect(posLabelHelp.x*Screen.width, posLabelHelp.y*Screen.height, posLabelHelp.width*Screen.width, posLabelHelp.height*Screen.height), errorMessage);
			GUI.color = new Color(1f, 1f, 1f, 1f);
		}
		
		if(!inInputEntryMode){
			if(!confirmDialogActivated){
			
				if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Save")){
					if(verifValidData()){
						bgScreen.renderer.material.color = new Color(bgScreen.renderer.material.color.r*15f, bgScreen.renderer.material.color.g*15f, bgScreen.renderer.material.color.b*15f, 1f);
						lerpColorScreenFade = 0f;
						optionMenuMode = StateOption.SCREENFADEOUT;
						putInDataManager();
					}
				}
				
				if(GUI.Button(new Rect(posButtonCancel.x*Screen.width, posButtonCancel.y*Screen.height, posButtonCancel.width*Screen.width, posButtonCancel.height*Screen.height), "Cancel")){
					errorMessage = "";
					bgScreen.renderer.material.color = new Color(bgScreen.renderer.material.color.r*15f, bgScreen.renderer.material.color.g*15f, bgScreen.renderer.material.color.b*15f, 1f);
					lerpColorScreenFade = 0f;
					optionMenuMode = StateOption.SCREENFADEOUT;
					setByDataManager();
				}
			}else
			{
				GUI.Label(new Rect(LabelConfirmDialog.x*Screen.width, LabelConfirmDialog.y*Screen.height, LabelConfirmDialog.width*Screen.width, LabelConfirmDialog.height*Screen.height), textOption["CONFIRM_DIALOG"]);
		
				if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Confirm")){
					switch(optionConfirm)
					{
						case "cache":
						LoadManager.Instance.SaveCache();
						break;
						case "profile":
						PlayerPrefs.DeleteKey("idProfile");
						Application.LoadLevel("SplashScreen");
						break;
					}
					confirmDialogActivated = false;
				}
				
				if(GUI.Button(new Rect(posButtonCancel.x*Screen.width, posButtonCancel.y*Screen.height, posButtonCancel.width*Screen.width, posButtonCancel.height*Screen.height), "Cancel")){
					confirmDialogActivated = false;
				}
			}
		}else{
			if (Event.current.isKey)
			{
				saveCodeForIndex(indexInputSelected, Event.current.keyCode);
				inInputEntryMode = false;
				indexInputSelected = -1;
			}
		
		}
	}
	
	// Update is called once per frame
	
	void Update(){
		switch(optionMenuMode){
			case StateOption.OPTIONSELECTION:
				UpdateOptionSelect();
				break;
			case StateOption.CUBEFADEIN:
				UpdateCubeFadeIn();
				break;
			case StateOption.SCREENFADEIN:
				UpdateScreenFadeIn();
				break;
			case StateOption.SCREENFADEOUT:
				UpdateScreenFadeOut();
				break;
			case StateOption.CUBEFADEOUT:
				UpdateCubeFadeOut();
				break;
			case StateOption.OPTION:
				UpdateMapping();
				break;
		}
		
		if(!alreadyFaded && timeBeforeFade > 0.25f){
			GetComponent<FadeManager>().FadeOut();
			alreadyFaded = true;
		}else{
			timeBeforeFade += Time.deltaTime;
		}
	}
	
	void UpdateOptionSelect () {
		
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
			RaycastHit hit;
					
			if(Physics.Raycast(ray, out hit))
			{
				
				var theGo = hit.transform.gameObject;
				if(theGo != null && theGo.tag != "MenuItem"){
					theSelected = theGo;
					theSelected.transform.GetChild(0).particleSystem.Play();
					theSelected.renderer.material.color = Color.Lerp(theSelected.renderer.material.color, theSelected.GetComponent<ColorGO>().myColor, loadColor);
					theSelected.transform.localScale = Vector3.Lerp(theSelected.transform.localScale, new Vector3(1.4f, 1.4f, 1.4f), Time.deltaTime/speedScale);
					if(loadColor < 1f) loadColor += Time.deltaTime*speedColor;
				}else if(theSelected != null){
					loadColor = 0f;
					if(theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Stop();
					theSelected = null;
				}
				
			}else if(theSelected != null ){
				loadColor = 0f;
				if(theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Stop();
				theSelected = null;
			}
			
			var white = new Color(1f, 1f, 1f, 1f);
			for(int i = 0; i < menuItem.Length;i++){
				if(menuItem[i].renderer.material.color != white && menuItem[i] != theSelected){
					var r = menuItem[i].renderer.material.color.r < 1f ? menuItem[i].renderer.material.color.r + Time.deltaTime*speedColorFade : 1f;
					var g = menuItem[i].renderer.material.color.g < 1f ? menuItem[i].renderer.material.color.g + Time.deltaTime*speedColorFade : 1f;
					var b = menuItem[i].renderer.material.color.b < 1f ? menuItem[i].renderer.material.color.b + Time.deltaTime*speedColorFade : 1f;
					menuItem[i].renderer.material.color = new Color( r, g, b, 1f);
				}
				if(menuItem[i].transform.localScale.x > 1.21f){
					menuItem[i].transform.localScale = Vector3.Lerp(menuItem[i].transform.localScale, new Vector3(1.2f, 1.2f, 1.2f), Time.deltaTime/speedScale);
				}else if(menuItem[i].transform.localScale.x != 1.2f){
					menuItem[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);	
				}
				
			}
			
			
			if(theSelected != null && Input.GetMouseButtonDown(0) && !fadeOut){
				if(theSelected.name == "Back"){
					ProfileManager.Instance.SaveProfile();
					fadeOut = true;
					fm.FadeIn("mainmenu");
				}else{
				
					if(theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Stop();
					optionSelected = theSelected.name;
					optionMenuMode = StateOption.CUBEFADEIN;
				}
			}
		
	}
	
	void UpdateCubeFadeIn(){
		for(int i = 0; i < menuItem.Length;i++){
			alphaFadeIn -= speedFadeIn*Time.deltaTime;
			menuItem[i].renderer.material.color = new Color(menuItem[i].renderer.material.color.r, menuItem[i].renderer.material.color.g, menuItem[i].renderer.material.color.b, alphaFadeIn);
			cubeIcon.renderer.material.color = new Color(theSelected.renderer.material.color.r, theSelected.renderer.material.color.g, theSelected.renderer.material.color.b, 1 - alphaFadeIn);
			if(alphaFadeIn <= 0){
				optionMenuMode = StateOption.SCREENFADEIN;
			}
		}
	}
	
	void UpdateScreenFadeIn(){
		if(screen.transform.position.y <= 0.1f){
			screen.transform.position = new Vector3(0f, 0f, 0f);
			bgScreen.renderer.material.color = new Color(bgScreen.renderer.material.color.r*15f, bgScreen.renderer.material.color.g*15f, bgScreen.renderer.material.color.b*15f, 1f);
			optionMenuMode = StateOption.OPTION;
			if(theSelected.name == "Key Mapping"){
				KeyMapParentObject.SetActive(true);
			}
		}else{
			screen.transform.position = Vector3.Lerp(screen.transform.position, new Vector3(0f, 0f, 0f), speedFadeScreen*Time.deltaTime);
		}
		
		
	}
	
	void UpdateScreenFadeOut(){
		if(screen.transform.position.y > 19.9f){
			screen.transform.position = new Vector3(0f, 20f, 0f);
			optionMenuMode = StateOption.CUBEFADEOUT;
			
		}else if(bgScreen.renderer.material.color.a <= 0f){
			screen.transform.position = Vector3.Lerp(screen.transform.position, new Vector3(0f, 20f, 0f), speedFadeScreen*Time.deltaTime);
		}
		
		if(lerpColorScreenFade < 1f){
			lerpColorScreenFade += speedColorScreenFade*Time.deltaTime;
			bgScreen.renderer.material.color = Color.Lerp(bgScreen.renderer.material.color , new Color(colorBasicScreen.r, colorBasicScreen.g, colorBasicScreen.b, 0f) , lerpColorScreenFade);
		}
		
		if(theSelected.name == "Key Mapping" && KeyMapParentObject.activeInHierarchy){
			KeyMapParentObject.SetActive(false);
		}
	}
	
	
	void UpdateCubeFadeOut(){
		for(int i = 0; i < menuItem.Length;i++){
			alphaFadeIn += speedFadeIn*Time.deltaTime;
			menuItem[i].renderer.material.color = new Color(menuItem[i].renderer.material.color.r, menuItem[i].renderer.material.color.g, menuItem[i].renderer.material.color.b, alphaFadeIn);
			cubeIcon.renderer.material.color = new Color(theSelected.renderer.material.color.r, theSelected.renderer.material.color.g, theSelected.renderer.material.color.b, 1 - alphaFadeIn);
			if(alphaFadeIn >= 1){
				lerpColorScreenFade = 0f;
				optionMenuMode = StateOption.OPTIONSELECTION;
			}
		}
	}
	
	
	void UpdateMapping(){
	
		if(theSelected.name == "Key Mapping"){
			if(!inPosition){
				inPosition = true;
				for(int i=0; i< formObject.Length; i++){
					var targetedTransform = (choicePosition == 0 ? form1[i] : choicePosition == 1 ? form2[i] : form3[i]);
					formObject[i].transform.position = Vector3.Lerp(formObject[i].transform.position, targetedTransform.position, speedMovementMapping*Time.deltaTime);
					if(inPosition && Vector3.Distance(formObject[i].transform.position, targetedTransform.position) > 0.01f){
						inPosition = false;
					}
				}
				labelDialogMapPri.x = Mathf.Lerp (labelDialogMapPri.x, posLabelPrim[choicePosition].x, speedMovementMapping*Time.deltaTime);
				labelDialogMapPri.y = Mathf.Lerp (labelDialogMapPri.y, posLabelPrim[choicePosition].y, speedMovementMapping*Time.deltaTime);
				labelDialogMapSec.x = Mathf.Lerp (labelDialogMapSec.x, posLabelSec[choicePosition].x, speedMovementMapping*Time.deltaTime);
				labelDialogMapSec.y = Mathf.Lerp (labelDialogMapSec.y, posLabelSec[choicePosition].y, speedMovementMapping*Time.deltaTime);
			}
		
			if(!inInputEntryMode){
				Ray ray = camMapping.ScreenPointToRay(Input.mousePosition);	
				RaycastHit hit;
						
				if(Physics.Raycast(ray, out hit))
				{
					
					var theGo = hit.transform.gameObject;
					if(theGo != null && theGo.tag == "MenuItem"){
						if(indexInputSelected == -1 || (theSelectedTouch != null && theSelectedTouch.name != theGo.name)){
							fadeColorMapping = 0f;
						}
						theSelectedTouch = theGo;
						timeStillFade = 0f;
						indexInputSelected = System.Convert.ToInt32(theSelectedTouch.name);
						if(fadeColorMapping < 1f){
							for(int i=0; i<formObject.Length; i++){
								GameObject obj = formObject[i];
								fadeColorMapping += speedColorMapping*Time.deltaTime;
								if(obj.name != theSelectedTouch.name){
									obj.renderer.material.color = Color.Lerp(obj.renderer.material.color, new Color(0.2f, 0.2f, 0.2f, 1f), fadeColorMapping);
								}else{
									obj.renderer.material.color = Color.Lerp(obj.renderer.material.color, new Color(1f, 1f, 1f, 1f), fadeColorMapping);
								}
							}
							
						}
						
					}else if(timeStillFade > 0.1f){
						if(indexInputSelected != -1){
							fadeColorMapping = 0f;	
						}
						indexInputSelected = -1;
						if(fadeColorMapping < 1f){
							fadeColorMapping += speedColorMapping*Time.deltaTime;
							foreach(var obj in formObject){
								((GameObject)obj).renderer.material.color = Color.Lerp(((GameObject)obj).renderer.material.color, new Color(1f, 1f, 1f, 1f), fadeColorMapping);
							}
						}
					
					}else{
						timeStillFade += Time.deltaTime;
					}
					
				}else if(timeStillFade > 0.1f){
					if(indexInputSelected != -1){
						fadeColorMapping = 0f;	
					}
					indexInputSelected = -1;
					if(fadeColorMapping < 1f){
						fadeColorMapping += speedColorMapping*Time.deltaTime;
						foreach(var obj in formObject){
							((GameObject)obj).renderer.material.color = Color.Lerp(((GameObject)obj).renderer.material.color, new Color(1f, 1f, 1f, 1f), fadeColorMapping);
						}
					}
				}else{
					timeStillFade += Time.deltaTime;
				}
			
			
				if(indexInputSelected != -1 && Input.GetMouseButtonDown(0)){
					inInputEntryMode = true;
					fadeColorMapping = 0f;
				}
			}
			
			if(inInputEntryMode && alphaBlackMapping < 0.9f){
				alphaBlackMapping += speedFadeBlackMapping*Time.deltaTime;
			}else if(!inInputEntryMode && alphaBlackMapping > 0f){
				alphaBlackMapping -= speedFadeBlackMapping*Time.deltaTime;
			}
		
		}
		
		
		if(lerpColorScreenFade < 1f){
			bgScreen.renderer.material.color = Color.Lerp(bgScreen.renderer.material.color , colorBasicScreen , lerpColorScreenFade);
			lerpColorScreenFade += speedColorScreenFade*Time.deltaTime;
		}
	
	
	}
	
	
	
	KeyCode giveCodeForIndex(int i){
		switch(i)
		{
			case 2:
			return DataManager.Instance.KeyCodeUp;
			case 1:
			return DataManager.Instance.KeyCodeDown;
			case 0:
			return DataManager.Instance.KeyCodeLeft;
			case 3:
			return DataManager.Instance.KeyCodeRight;
			case 6:
			return DataManager.Instance.SecondaryKeyCodeUp;
			case 5:
			return DataManager.Instance.SecondaryKeyCodeDown;
			case 4:
			return DataManager.Instance.SecondaryKeyCodeLeft;
			case 7:
			return DataManager.Instance.SecondaryKeyCodeRight;
		}
		return KeyCode.None;
	}
	
	
	void saveCodeForIndex(int i, KeyCode k){
		switch(i)
		{
			case 2:
			resetCodeForPrimary(k, DataManager.Instance.KeyCodeUp);
			DataManager.Instance.KeyCodeUp = k;
			break;
			case 1:
			resetCodeForPrimary(k, DataManager.Instance.KeyCodeDown);
			DataManager.Instance.KeyCodeDown = k;
			break;
			case 0:
			resetCodeForPrimary(k, DataManager.Instance.KeyCodeLeft);
			DataManager.Instance.KeyCodeLeft = k;
			break;
			case 3:
			resetCodeForPrimary(k, DataManager.Instance.KeyCodeRight);
			DataManager.Instance.KeyCodeRight = k;
			break;
			case 6:
			resetCodeForSecondary(k, DataManager.Instance.SecondaryKeyCodeUp);
			DataManager.Instance.SecondaryKeyCodeUp = k;
			break;
			case 5:
			resetCodeForSecondary(k, DataManager.Instance.SecondaryKeyCodeDown);
			DataManager.Instance.SecondaryKeyCodeDown = k;
			break;
			case 4:
			resetCodeForSecondary(k, DataManager.Instance.SecondaryKeyCodeLeft);
			DataManager.Instance.SecondaryKeyCodeLeft = k;
			break;
			case 7:
			resetCodeForSecondary(k, DataManager.Instance.SecondaryKeyCodeRight);
			DataManager.Instance.SecondaryKeyCodeRight = k;
			break;
		}
		
		
	}
	
	void resetCodeForPrimary(KeyCode kc, KeyCode okc){
		if(kc == DataManager.Instance.KeyCodeUp){
			DataManager.Instance.KeyCodeUp = okc;
		}else if(kc == DataManager.Instance.KeyCodeDown){
			DataManager.Instance.KeyCodeDown = okc;
		}else if(kc == DataManager.Instance.KeyCodeLeft){
			DataManager.Instance.KeyCodeLeft = okc;
		}else if(kc == DataManager.Instance.KeyCodeRight){
			DataManager.Instance.KeyCodeRight = okc;
		}
	}
	
	void resetCodeForSecondary(KeyCode kc, KeyCode okc){
		if(kc == DataManager.Instance.SecondaryKeyCodeUp){
			DataManager.Instance.SecondaryKeyCodeUp = okc;
		}else if(kc == DataManager.Instance.SecondaryKeyCodeDown){
			DataManager.Instance.SecondaryKeyCodeDown = okc;
		}else if(kc == DataManager.Instance.SecondaryKeyCodeLeft){
			DataManager.Instance.SecondaryKeyCodeLeft = okc;
		}else if(kc == DataManager.Instance.SecondaryKeyCodeRight){
			DataManager.Instance.SecondaryKeyCodeRight = okc;
		}
	}
	
	string giveLabelForIndex(int i){
		switch(i){
			case 0:
			return "Left : ";
			case 1:
			return "Down : ";
			case 2:
			return "Up : ";
			case 3:
			return "Right : ";
			case 4:
			return "Secondary Left : ";
			case 5:
			return "Secondary Down : ";
			case 6:
			return "Secondary Up : ";
			case 7:
			return "Secondary Right : ";
		}
		return "";
	}
	
	
	void putInDataManager()
	{
		DataManager.Instance.generalVolume = generalVolume/100f;
		DataManager.Instance.enableBloom = enableBloom;
		DataManager.Instance.enableDepthOfField = enableDOF;
		DataManager.Instance.antiAliasing = antiAliasing;
		
		LANManager.Instance.actualPort = System.Convert.ToInt32(portPref);
		DataManager.Instance.useTheCacheSystem = cacheMode;
		DataManager.Instance.dancepadMode = padMode;
		DataManager.Instance.quickMode = quickMode;
		DataManager.Instance.mouseMolSpeed = System.Convert.ToInt32(mouseSpeed);
		DataManager.Instance.userGOS = (float)System.Convert.ToDouble(GOS);
		DataManager.Instance.onlyOnGame = onlyOnGame;
		
		AudioListener.volume = DataManager.Instance.generalVolume;
		QualitySettings.antiAliasing = DataManager.Instance.antiAliasing;
		
		ProfileManager.Instance.currentProfile.saveOptions();
		ProfileManager.Instance.SaveProfile();
		
		GameObject.Find("OptionManager").GetComponent<OptionManager>().reloadEffect();
	}
	
	void setByDataManager()
	{
		GOS = DataManager.Instance.userGOS.ToString("0.000");
		mouseSpeed = DataManager.Instance.mouseMolSpeed.ToString("0");
		quickMode = DataManager.Instance.quickMode;
		padMode = DataManager.Instance.dancepadMode;
		cacheMode = DataManager.Instance.useTheCacheSystem;
		portPref = LANManager.Instance.actualPort.ToString();
		
		generalVolume = DataManager.Instance.generalVolume * 100f;
		
		enableBloom = DataManager.Instance.enableBloom;
		enableDOF = DataManager.Instance.enableDepthOfField;
		antiAliasing = DataManager.Instance.antiAliasing;
		onlyOnGame = DataManager.Instance.onlyOnGame;
	}
	
	
	bool verifValidData(){
		double output = 0;
		var outputInt = 0;
		if(!System.Double.TryParse(GOS, out output)){
			errorMessage = "Global Offset : not a valid entry";
			return false;
		}
		if(!System.Int32.TryParse(mouseSpeed, out outputInt)){
			errorMessage = "Mouse speed : not a valid entry";
			return false;
		}else{
			 if(outputInt <= 0){
				errorMessage = "Mouse speed : Minimum value is 1";
				return false;
			}else if(outputInt > 5){
				errorMessage = "Mouse speed : Maximum value is 5";
				return false;
			}
		}
		
		if(!System.Int32.TryParse(portPref, out outputInt)){
			errorMessage = "Port : not a valid entry";	
			return false;
		}else{
			if(outputInt < 0)
			{
				errorMessage = "Network port is a positive integer";
				return false;
			}
		}
		
		if(cacheMode && Directory.GetFiles(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache").Where(c => c.Contains("dataSong")).Count() == 0){
			errorMessage = "'Use the cache system' is set to 'yes' but the cache is not generated.\nGenerate the cache before confirm.";
			return false;
		}
		errorMessage = "";
		return true;
	}
	
	
}
