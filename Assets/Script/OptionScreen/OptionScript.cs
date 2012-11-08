using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	public GameObject cubeIcon;
	public GameObject screen;
	public GameObject bgScreen;
	public GameObject[] menuItem;
	public GUISkin skin;
	
	
	private Vector3[] Rotate;
	public float speedScale;
	private float loadColor;
	public float speedColor;
	public float speedColorFade;
	
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
	
	//Audio
	public string generalVolume;
	
	//Video
	public bool enableBloom;
	public bool enableDOF;
	public int antialiasing;
	
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
	
	public Transform[] form1;
	public Transform[] form2;
	public Transform[] form3;
	
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
		
		tex = new Dictionary<string, Texture2D>();
		tex.Add("labelbg", (Texture2D) Resources.Load("GUIBarMini"));
		
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
		
		generalVolume = DataManager.Instance.generalVolume.ToString("0");
		
		enableBloom = DataManager.Instance.enableBloom;
		enableDOF = DataManager.Instance.enableDepthOfField;
		antialiasing = DataManager.Instance.antiAliasing;
		
		networkValue[ProfileDownloadType.NEVER] = "Ne jamais partager les profiles";
		networkValue[ProfileDownloadType.NOTMANY] = "Ne pas stocker plus de 10 profiles";
		networkValue[ProfileDownloadType.MANY] = "Ne pas stocker plus de 50 profiles";
		networkValue[ProfileDownloadType.ALL] = "Stocker tous les profiles";
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
				OnGUIOption();
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
			GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width + 1, Screen.height - pos2D.y + offsetLabelOption.y*Screen.height + 1, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), theSelected.name);
			GUI.color = new Color(theSelected.renderer.material.color.r, theSelected.renderer.material.color.g, theSelected.renderer.material.color.b, loadColor);
			GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width, Screen.height - pos2D.y + offsetLabelOption.y*Screen.height, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), theSelected.name);
		}
	}
	
	void OnGUIOptionFadeIn(){
		GUI.skin = skin;
		GUI.color = new Color(0f, 0f, 0f, 1 - alphaFadeIn);
		GUI.Label(new Rect(labelOption.x*Screen.width + 1, labelOption.y*Screen.height + 1, labelOption.width*Screen.width, labelOption.height*Screen.height), optionSelected);
		GUI.color = new Color(theSelected.renderer.material.color.r, theSelected.renderer.material.color.g, theSelected.renderer.material.color.b, 1 - alphaFadeIn);
		GUI.Label(new Rect(labelOption.x*Screen.width, labelOption.y*Screen.height, labelOption.width*Screen.width, labelOption.height*Screen.height), optionSelected);
	}
	
	
	void OnGUIOption(){
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
			if(GUI.Button(new Rect(posButtonCache.x*Screen.width, posButtonCache.y*Screen.height, posButtonCache.width*Screen.width, posButtonCache.height*Screen.height), textOption["GENERAL_RELOADCACHE"])){
				//Mettre un truc de confirmation
				LoadManager.Instance.SaveCache();
			}
				
			//Changer profile
			if(GUI.Button(new Rect(posButtonProfile.x*Screen.width, posButtonProfile.y*Screen.height, posButtonProfile.width*Screen.width, posButtonProfile.height*Screen.height), textOption["GENERAL_RELOADPROFILE"])){
				//Mettre un truc de confirmation
				PlayerPrefs.DeleteKey("idProfile");
				Application.LoadLevel("SplashScreen");
			}	
			break;
			
		//Network
			case "Network":
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["NETWORK_CHOICE"]);
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width + offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "rightArrow")){
				PDT++;
				if(PDT > 3) PDT = 0;
			}
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width - offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "leftArrow")){
				PDT--;
				if(PDT < 0) PDT = 3;
			}
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width, posLabelListChoice.y*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), networkValue[PDT]);
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
				GUI.Label(new Rect(labelMapping.x*Screen.width + supX*offsetXlabelMapping*Screen.width, labelMapping.y*Screen.height + supY*offsetYlabelMapping*Screen.width, labelMapping.width*Screen.width, labelMapping.height*Screen.height), giveLabelForIndex(i) + giveCodeForIndex(i).ToString();
			}
			
			GUI.Label(new Rect(posLabelOption.x*Screen.width, posLabelOption.y*Screen.height, posLabelOption.width*Screen.width, posLabelOption.height*Screen.height), textOption["MAPPING_CHOICE"]);
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width + offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "rightArrow")){
				choicePosition += 1;
				if(choicePosition > 2) choicePosition = 0;
			}
			
			if(GUI.Button(new Rect(posLabelListChoice.x*Screen.width - offsetBetweenLabel*Screen.width, posLabelListChoice.y*Screen.height, sizeArrowButton.x*Screen.width, sizeArrowButton.y*Screen.height), "", "leftArrow")){
				choicePosition -= 1;
				if(choicePosition < 0) choicePosition = 2;
			}
			
			GUI.Label(new Rect(posLabelListChoice.x*Screen.width, posLabelListChoice.y*Screen.height + offsetLabelYOption*2*Screen.height, posLabelListChoice.width*Screen.width, posLabelListChoice.height*Screen.height), choicePosition == 0 ? "KeyBoard" : choicePosition == 1 ? "Arcade Stick" : "DancePad");
			
			GUI.Label(new Rect(labelInfo.x*Screen.width, labelInfo.y*Screen.height, labelInfo.width*Screen.width, labelInfo.height*Screen.height), textOption["MAPPING_INFO"]);
			
			//Faire une anim ?
			
			break;
		}
		
		//Mettre un label d'aide
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Save")){
			//Accept
		}
		
		if(GUI.Button(new Rect(posButtonCancel.x*Screen.width, posButtonCancel.y*Screen.height, posButtonCancel.width*Screen.width, posButtonCancel.height*Screen.height), "Cancel")){
			//Cancel
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
			case StateOption.OPTION:
				UpdateScreenFadeIn();
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
			
			
			if(theSelected != null && Input.GetMouseButtonDown(0)){
				if(theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Stop();
				optionSelected = theSelected.name;
				optionMenuMode = StateOption.CUBEFADEIN;
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
		if(screen.transform.position.y <= 0.1f && bgScreen.renderer.material.color.a < 1f){
			screen.transform.position = new Vector3(0f, 0f, 0f);
			bgScreen.renderer.material.color = new Color(bgScreen.renderer.material.color.r*10f, bgScreen.renderer.material.color.g*10f, bgScreen.renderer.material.color.b*10f, 1f);
			optionMenuMode = StateOption.OPTION;
		
		}else{
			screen.transform.position = Vector3.Lerp(screen.transform.position, new Vector3(0f, 0f, 0f), speedFadeScreen*Time.deltaTime);
		}
		
		if(bgScreen.renderer.material.color.a >= 1f && lerpColorScreenFade < 1f){
			bgScreen.renderer.material.color = Color.Lerp(bgScreen.renderer.material.color , colorBasicScreen , lerpColorScreenFade);
			lerpColorScreenFade += speedColorScreenFade*Time.deltaTime;
		}
	}
	
	
	
	KeyCode giveCodeForIndex(int i){
		switch(i)
		{
			case 2:
			return DataManager.Instance.KeyCodeUp;
			break;
			case 1:
			return DataManager.Instance.KeyCodeDown;
			break;
			case 0:
			return DataManager.Instance.KeyCodeLeft;
			break;
			case 3:
			return DataManager.Instance.KeyCodeRight;
			break;
			case 6:
			return DataManager.Instance.SecondaryKeyCodeUp;
			break;
			case 5:
			return DataManager.Instance.SecondaryKeyCodeDown;
			break;
			case 4:
			return DataManager.Instance.SecondaryKeyCodeLeft;
			break;
			case 7:
			return DataManager.Instance.SecondaryKeyCodeRight;
			break;
		}
	}
	
	string giveLabelForIndex(int i){
		switch(i){
			case 0:
			return "Left : ";
			break;
			case 1:
			return "Down : ";
			break;
			case 2:
			return "Up : ";
			break;
			case 3:
			return "Right : ";
			break;
			case 4:
			return "Secondary Left : ";
			break;
			case 5:
			return "Secondary Down : ";
			break;
			case 6:
			return "Secondary Up : ";
			break;
			case 7:
			return "Secondary Right : ";
			break;
			
		}
	
	}
	
	
	
}
