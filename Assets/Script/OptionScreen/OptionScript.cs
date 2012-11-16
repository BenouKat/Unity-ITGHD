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
	public GUISkin skin;
	private FadeManager fm;
	
	
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
	public Rect posButtonOption;
	public Rect posButtonBack;
	public Rect posButtonCancel;
	public Rect posLabelListChoice;
	public Vector2 sizeArrowButton;
	public float offsetBetweenLabel;
	private string errorMessage;
	public Rect posLabelHelp;
	
	//Audio
	private string generalVolume;
	
	//Video
	private bool enableBloom;
	private bool enableDOF;
	private int antiAliasing;
	
	//KeyMapping
	//8 game objet + 8 label / button + un label général
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
	public Rect labelDialogMap;
	public float offsetDialogMap;
	public Rect labelInfo;
	public int choicePosition;
	
	public GameObject[] formObject;
	public Transform[] form1;
	public Transform[] form2;
	public Transform[] form3;
	public float speedMovementMapping;
	private bool inPosition;
	
	private float fadeColorMapping;
	public float speedColorMapping;
	
	private float alphaBlackMapping;
	public float speedFadeBlackMapping;
	private int indexInputSelected;
	private bool inInputEntryMode;
	
	//General
	public Rect posButtonCache;
	public Rect posButtonProfile;
	
	private string GOS;
	private string mouseSpeed;
	private bool quickMode;
	private bool padMode;
	private bool cacheMode;
	
	//Network
	private string[] networkValue;
	private ProfileDownloadType PDT;
	
	
	//textures
	public Dictionary<string, Texture2D> tex;
	
	//texts
	private Dictionary<string, string> textOption;
	// Use this for initialization
	void Start () {
		
		//test
		TextManager.Instance.LoadTextFile();
		
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
		PDT = DataManager.Instance.PDT;
		
		generalVolume = (DataManager.Instance.generalVolume * 100f).ToString("0");
		
		enableBloom = DataManager.Instance.enableBloom;
		enableDOF = DataManager.Instance.enableDepthOfField;
		antiAliasing = DataManager.Instance.antiAliasing;
		
		networkValue = new string[4];
		networkValue[(int)ProfileDownloadType.NEVER] = "Ne jamais partager les profiles";
		networkValue[(int)ProfileDownloadType.NOTMANY] = "Ne pas stocker plus de 10 profiles";
		networkValue[(int)ProfileDownloadType.MANY] = "Ne pas stocker plus de 50 profiles";
		networkValue[(int)ProfileDownloadType.ALL] = "Stocker tous les profiles";
		
		fadeOut = false;
		fm = gameObject.GetComponent<FadeManager>();
		inPosition = false;
		fadeColorMapping = 0f;
		alphaBlackMapping = 0f;
		inInputEntryMode = false;
		errorMessage = "";
		optionConfirm = "";
		confirmDialogActivated = false;
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
		if(theSelected != null){
			var pos2D = Camera.main.WorldToScreenPoint(theSelected.transform.position);
			GUI.color = new Color(1f, 1f, 1f, 0.5f*alphaFadeIn*loadColor);
			GUI.DrawTexture(new Rect(pos2D.x + (sizeLabelBG.x*Screen.width), Screen.height - pos2D.y + (sizeLabelBG.y*Screen.height), sizeLabelBG.width*Screen.width, sizeLabelBG.height*Screen.height), tex["labelbg"]);
			GUI.color = new Color(0f, 0f, 0f, 1f*alphaFadeIn*loadColor);
			GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width + 1, Screen.height - pos2D.y + offsetLabelOption.y*Screen.height + 1, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), theSelected.name, "TitleLabel");
			GUI.color = new Color(theSelected.renderer.material.color.r, theSelected.renderer.material.color.g, theSelected.renderer.material.color.b, loadColor);
			GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width, Screen.height - pos2D.y + offsetLabelOption.y*Screen.height, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), theSelected.name, "TitleLabel");
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
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1 + offsetLabelYOption*3*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_PADMODE"]);
			
			//cache
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1 + offsetLabelYOption*4*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["GENERAL_USECACHE"]);

			break;
			
		//Network
			case "Network":
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["NETWORK_CHOICE"]);
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width + 1, posLabelListChoice.y*Screen.height + 1, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), networkValue[(int)PDT]);
			break;
			
			
		//Audio
			case "Audio":
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["AUDIO_GENERAL"]);
			break;
			
		//Video
			case "Video":
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_BLOOM"]);

			
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1 + offsetLabelYOption*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_DOF"]);

			
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_AA"]);

			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width + 1, posLabelListChoice.y*Screen.height + 1 + offsetLabelYOption*2*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), "x" + antiAliasing);

			break;
			
			
		//Key Mapping
			case "Key Mapping":
			GUI.Label(new Rect(labelDialogMap.x*Screen.width + 1, labelDialogMap.y*Screen.height + 1, labelDialogMap.width*Screen.width, labelDialogMap.height*Screen.height), "Primary");
			GUI.Label(new Rect(labelDialogMap.x*Screen.width + 1, labelDialogMap.y*Screen.height + 1, labelDialogMap.width*Screen.width, labelDialogMap.height*Screen.height), "Secondary");
			for(int i=0; i<8; i++){
				var supX = i < 4 ? i : i - 4;
				var supY = i < 4 ? 0 : 1;
				GUI.Label(new Rect(labelMapping.x*Screen.width + 1 + supX*offsetXlabelMapping*Screen.width, labelMapping.y*Screen.height + 1 + supY*offsetYlabelMapping*Screen.width, labelMapping.width*Screen.width, labelMapping.height*Screen.height), giveLabelForIndex(i) + giveCodeForIndex(i).ToString());
			}
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width + 1, posLabelOption.y*Screen.height + 1, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["MAPPING_CHOICE"]);

			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width + 1, posLabelListChoice.y*Screen.height + 1 + offsetLabelYOption*2*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), choicePosition == 0 ? "KeyBoard" : choicePosition == 1 ? "Arcade Stick" : "DancePad");
			
			GUI.Label(new Rect(labelInfo.x*Screen.width + 1, labelInfo.y*Screen.height + 1, labelInfo.width*Screen.width, labelInfo.height*Screen.height), inInputEntryMode ? textOption["MAPPING_INPUT"] : textOption["MAPPING_INFO"]);
			
			//Faire une anim ?
			
			break;
		}
		
		if(!String.IsNullOrEmpty(errorMessage)){
			GUI.Label(new Rect(posLabelHelp.x*Screen.width + 1, posLabelHelp.y*Screen.height + 1, posLabelHelp.width*Screen.width + 1, posLabelHelp.height*Screen.height + 1), errorMessage);
		}
		
		if(!confirmDialogActivated)
		{
			GUI.Label(new Rect(LabelConfirmDialog.x*Screen.width, LabelConfirmDialog.y*Screen.height, LabelConfirmDialog.width*Screen.width, LabelConfirmDialog.height*Screen.height), textOption["CONFIRM_DIALOG"]);
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
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["NETWORK_CHOICE"]);
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width + offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "rightArrow")){
				PDT++;
				if((int)PDT > 3) PDT = (ProfileDownloadType)0;
			}
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width - offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "leftArrow")){
				PDT--;
				if((int)PDT < 0) PDT = (ProfileDownloadType)3;
			}
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width, posLabelListChoice.y*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), networkValue[(int)PDT]);
			//Faire une anim ?
			break;
			
			
		//Audio
			case "Audio":
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["AUDIO_GENERAL"]);
			generalVolume = GUI.TextField(new Rect(posTextFieldOption.x*Screen.width, posTextFieldOption.y*Screen.height, posTextFieldOption.width*Screen.width, posTextFieldOption.height*Screen.height), generalVolume, 5);
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
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["VIDEO_AA"]);
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width + offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height + offsetLabelYOption*2*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "rightArrow")){
				antiAliasing += 2;
				if(antiAliasing > 8) antiAliasing = 0;
			}
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width - offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height + offsetLabelYOption*2*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "leftArrow")){
				antiAliasing -= 2;
				if(antiAliasing < 0) antiAliasing = 8;
			}
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width, posLabelListChoice.y*Screen.height + offsetLabelYOption*2*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), "x" + antiAliasing);
			//Faire une anim ?
			break;
			
			
		//Key Mapping
			case "Key Mapping":
			GUI.Label(new Rect(labelDialogMap.x*Screen.width, labelDialogMap.y*Screen.height, labelDialogMap.width*Screen.width, labelDialogMap.height*Screen.height), "Primary");
			GUI.Label(new Rect(labelDialogMap.x*Screen.width, labelDialogMap.y*Screen.height, labelDialogMap.width*Screen.width, labelDialogMap.height*Screen.height), "Secondary");
			for(int i=0; i<8; i++){
				var supX = i < 4 ? i : i - 4;
				var supY = i < 4 ? 0 : 1;
				if(indexInputSelected == -1){
					GUI.color = new Color(1f, 1f, 1f, 1f);
				}else if(indexInputSelected != i){
					GUI.color = new Color(1 - 0.6f*fadeColorMapping, 1 - 0.6f*fadeColorMapping, 1 - 0.6f*fadeColorMapping, 1f);
				}
				GUI.Label(new Rect(labelMapping.x*Screen.width + supX*offsetXlabelMapping*Screen.width, labelMapping.y*Screen.height + supY*offsetYlabelMapping*Screen.width, labelMapping.width*Screen.width, labelMapping.height*Screen.height), giveLabelForIndex(i) + giveCodeForIndex(i).ToString());
				GUI.color = new Color(1f, 1f, 1f, 1f);
			}
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["MAPPING_CHOICE"]);
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width + offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "rightArrow") && !inInputEntryMode){
				choicePosition += 1;
				if(choicePosition > 2) choicePosition = 0;
				inPosition = false;
			}
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width - offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "leftArrow") && !inInputEntryMode){
				choicePosition -= 1;
				if(choicePosition < 0) choicePosition = 2;
				inPosition = false;
			}
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width, posLabelListChoice.y*Screen.height + offsetLabelYOption*2*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), choicePosition == 0 ? "KeyBoard" : choicePosition == 1 ? "Arcade Stick" : "DancePad");
			
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
	}
	
	void UpdateOptionSelect () {
		
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
			RaycastHit hit;
					
			if(Physics.Raycast(ray, out hit))
			{
				
				var theGo = hit.transform.gameObject;
				if(theGo != null){
					theSelected = theGo;
					if(!theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Play();
					theSelected.renderer.material.color = Color.Lerp(theSelected.renderer.material.color, theSelected.GetComponent<ColorGO>().myColor, loadColor);
					theSelected.transform.localScale = Vector3.Lerp(theSelected.transform.localScale, new Vector3(1.4f, 1.4f, 1.4f), Time.deltaTime/speedScale);
					if(loadColor < 1f) loadColor += Time.deltaTime*speedColor;
				}else if(theSelected != null){
					loadColor = 0f;
					if(theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Stop();
					theSelected = null;
				}
				
			}else if(theSelected != null){
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
			if(theSelected.name == "Network"){
				foreach(var obj in formObject){
					((GameObject)obj).renderer.enabled = true;
				}
			}
		}else{
			screen.transform.position = Vector3.Lerp(screen.transform.position, new Vector3(0f, 0f, 0f), speedFadeScreen*Time.deltaTime);
		}
		
		
	}
	
	void UpdateScreenFadeOut(){
		if(screen.transform.position.y > 19.9f && bgScreen.renderer.material.color.a <= 0f){
			screen.transform.position = new Vector3(0f, 20f, 0f);
			optionMenuMode = StateOption.CUBEFADEOUT;
			if(theSelected.name == "Network"){
				foreach(var obj in formObject){
					((GameObject)obj).renderer.enabled = true;
				}
			}
		}else{
			screen.transform.position = Vector3.Lerp(screen.transform.position, new Vector3(0f, 20f, 0f), speedFadeScreen*Time.deltaTime);
		}
		
		if(lerpColorScreenFade < 1f){
			lerpColorScreenFade += speedColorScreenFade*Time.deltaTime;
			bgScreen.renderer.material.color = Color.Lerp(bgScreen.renderer.material.color , new Color(colorBasicScreen.r, colorBasicScreen.g, colorBasicScreen.b, 0f) , lerpColorScreenFade);
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
	
		if(theSelected.name == "Network"){
			if(!inPosition){
				inPosition = true;
				for(int i=0; i< formObject.Length; i++){
					var targetedTransform = choicePosition == 0 ? form1[i] : choicePosition == 1 ? form2[i] : form3[i];
					formObject[i].transform.position = Vector3.Lerp(formObject[i].transform.position, targetedTransform.position, speedMovementMapping*Time.deltaTime);
					if(inPosition && Vector3.Distance(formObject[i].transform.position, targetedTransform.position) > 0.01f){
						inPosition = false;
					}
				}
			}
		
			if(!inInputEntryMode){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
				RaycastHit hit;
						
				if(Physics.Raycast(ray, out hit))
				{
					
					var theGo = hit.transform.gameObject;
					if(theGo != null && theGo.tag == "MainMenuItem"){
						theSelectedTouch = theGo;
						if(indexInputSelected == -1) indexInputSelected = System.Convert.ToInt32(theSelectedTouch.name.Replace("Key", ""));
						if(fadeColorMapping < 1f){
							foreach(var obj in formObject){
								((GameObject)obj).renderer.material.color = Color.Lerp(((GameObject)obj).renderer.material.color, new Color(0.2f, 0.2f, 0.2f, 1f), fadeColorMapping);
							}
							fadeColorMapping += speedColorMapping*Time.deltaTime;
						}
						
					}else if(theSelectedTouch != null){
						theSelectedTouch = null;
						indexInputSelected = -1;
						if(fadeColorMapping > 0f){
							foreach(var obj in formObject){
								((GameObject)obj).renderer.material.color = Color.Lerp(((GameObject)obj).renderer.material.color, new Color(0.2f, 0.2f, 0.2f, 1f), fadeColorMapping);
							}
							fadeColorMapping -= speedColorMapping*Time.deltaTime;
						}
					}
					
				}else if(theSelectedTouch != null){
					theSelectedTouch = null;
					indexInputSelected = -1;
					if(fadeColorMapping > 0f){
						foreach(var obj in formObject){
							((GameObject)obj).renderer.material.color = Color.Lerp(((GameObject)obj).renderer.material.color, new Color(0.2f, 0.2f, 0.2f, 1f), fadeColorMapping);
						}
						fadeColorMapping -= speedColorMapping*Time.deltaTime;
					}
				}
			
			
				if(theSelectedTouch != null && Input.GetMouseButtonDown(0)){
					inInputEntryMode = true;
				}
			}
			
			if(inInputEntryMode && alphaBlackMapping < 0.8f){
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
			DataManager.Instance.KeyCodeUp = k;
			break;
			case 1:
			DataManager.Instance.KeyCodeDown = k;
			break;
			case 0:
			DataManager.Instance.KeyCodeLeft = k;
			break;
			case 3:
			DataManager.Instance.KeyCodeRight = k;
			break;
			case 6:
			DataManager.Instance.SecondaryKeyCodeUp = k;
			break;
			case 5:
			DataManager.Instance.SecondaryKeyCodeDown = k;
			break;
			case 4:
			DataManager.Instance.SecondaryKeyCodeLeft = k;
			break;
			case 7:
			DataManager.Instance.SecondaryKeyCodeRight = k;
			break;
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
		DataManager.Instance.generalVolume = System.Convert.ToInt32(generalVolume)/100f;
		DataManager.Instance.enableBloom = enableBloom;
		DataManager.Instance.enableDepthOfField = enableDOF;
		DataManager.Instance.antiAliasing = antiAliasing;
		
		DataManager.Instance.PDT = PDT;
		DataManager.Instance.useTheCacheSystem = cacheMode;
		DataManager.Instance.dancepadMode = padMode;
		DataManager.Instance.quickMode = quickMode;
		DataManager.Instance.mouseMolSpeed = System.Convert.ToInt32(mouseSpeed);
		DataManager.Instance.userGOS = (float)System.Convert.ToDouble(GOS);
		
		AudioListener.volume = DataManager.Instance.generalVolume;
		QualitySettings.antiAliasing = DataManager.Instance.antiAliasing;
		
		ProfileManager.Instance.currentProfile.saveOptions();
	}
	
	void setByDataManager()
	{
		GOS = DataManager.Instance.userGOS.ToString("0.000");
		mouseSpeed = DataManager.Instance.mouseMolSpeed.ToString("0");
		quickMode = DataManager.Instance.quickMode;
		padMode = DataManager.Instance.dancepadMode;
		cacheMode = DataManager.Instance.useTheCacheSystem;
		PDT = DataManager.Instance.PDT;
		
		generalVolume = (DataManager.Instance.generalVolume * 100f).ToString("0");
		
		enableBloom = DataManager.Instance.enableBloom;
		enableDOF = DataManager.Instance.enableDepthOfField;
		antiAliasing = DataManager.Instance.antiAliasing;
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
		
		if(!System.Double.TryParse(generalVolume, out output)){
			errorMessage = "General Volume : not a valid entry";
			return false;
		}else{
			if(output < 0){
				errorMessage = "General Volume : Minimum value is 0";
				return false;
			}else if(output > 100){
				errorMessage = "General Volume : Maximum value is 100";
				return false;
			}
		}
		
		if(cacheMode && !File.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache" + "dataSong.cache")){
			errorMessage = "'Use the cache system' is set to 'yes' but the cache is not generated.\nGenerate the cache before confirm.";
			return false;
		}
		errorMessage = "";
		return true;
	}
	
	
}
