using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class MainMenu : MonoBehaviour {
	
	private delegate void updateMethod();
	private updateMethod updat;
	
	//Montée de caméra
	public float speedUp;
	private float trueSpeedUp;
	public float limiteslowUp;
	public float limitesUp;
	public float addUp;
	
	//Tangue de la caméra
	public float speedTang;
	private float trueSpeedTang;
	public float limiteslowTangRight;
	public float limiteslowTangLeft;
	public float addTang;
	public float addStart;
	private float sign;
	
	//Première fois
	private bool firststart = true;
	
	//Temps surlequel on peut appuyer sur start
	private float time;
	private bool enterPushed;
	
	//Fondu de caméra vers le menu
	public float speedFadeMenu;
	private bool inPlace;
	private float timeBack;
	public float timeBeforeBack;
	
	//Glissement d'object select
	private Dictionary<string, GameObject> goToBack;
	private string forbiddenTouch;
	private string forbiddenTouchGUI;
	public float speedSlide;
	
	//audio
	public AudioSource audioSMainTitle;
	public AudioSource audioSMainMenu;
	public float speedAudioUp;
	
	//Clignotement pressstart
	public float speedPressStart;
	private float signPressStart;
	private float alphaPressStart;
	private Texture2D pressStart;
	public Rect posPressStart;
	
	//Fondu de choix
	public float speedChoice;
	private float timeSelect;
	public float speedFadeMenuRapide;
	
	
	//Fondu de sous choix free
	public Vector3 positionCameraSousChoixFree;
	public Vector3 rotationCameraSousChoixFree;
	public float speedMoveSousChoixFree;
	private bool sousChoixEnPlace;
	public GameObject packFree;
	
	
	//Sous Choix
	private string selection;
	public float speedTransistionSousChoix;
	
	//GUI text d'aide
	private Texture2D GUITextTexture;
	private Texture2D Black;
	public Rect posGUIimage;
	public Rect posGUItext;
	public float speedGUIImage;
	private float alphaGUIImage;
	private bool DisplayLabel;
	
	
	//Message d'erreur
	private bool error;
	private string listerror = "Story Mods Option Trophy Learn LAN Double";
	public Rect posError;
	public float offsetNumberBuild;
	public float speedFadeError;
	private float alphaError;
	
	public GUISkin skin;
	
	private FadeManager fm;
	private bool iFade;
	
	public float timeFade;
	// Use this for initialization
	void Start () {
		
		//Load
		if(!LoadManager.Instance.alreadyLoaded) TextManager.Instance.LoadTextFile();
		//if(!LoadManager.Instance.alreadyLoaded) LoadManager.Instance.Loading();
		//TextManager.Instance.LoadTextFile();
		fm = gameObject.GetComponent<FadeManager>();
		
		
		this.updat = UpdateWait;
		sign = -1f;
		Camera.main.transform.eulerAngles = new Vector3(66f, 32.5f, 0f);
		trueSpeedUp = speedUp;
		trueSpeedTang = 200;
		time = 0f;
		enterPushed = false;
		goToBack = new Dictionary<string, GameObject>();
		inPlace = false;
		signPressStart = 1f;
		alphaPressStart = 0f;
		pressStart = (Texture2D) Resources.Load("PressStart");
		GUITextTexture = (Texture2D) Resources.Load("GUIBar");
		Black = (Texture2D) Resources.Load("black");
		alphaGUIImage = 0f;
		timeSelect = 0f;
		iFade = false;
		selection = "";
		sousChoixEnPlace = false;
		DisplayLabel = false;
		error = false;
		alphaError = 0f;
		timeBack = 0f;
		forbiddenTouchGUI = "";
	}
	
	void OnGUI(){
		GUI.skin = skin;
		
		if(time >= 6 && (this.updat == UpdateUp || this.updat == UpdateTang)){
			alphaPressStart += signPressStart*(Time.deltaTime/speedPressStart);
			if((alphaPressStart >= 1f && signPressStart == 1f) || (alphaPressStart <= 0f && signPressStart == -1f)){
				signPressStart *= -1f;	
			}
			GUI.color = new Color(1f, 1f, 1f, alphaPressStart);
			GUI.DrawTexture(new Rect(posPressStart.x*Screen.width, posPressStart.y*Screen.height, posPressStart.width*Screen.width, posPressStart.height*Screen.height), pressStart);
		}
		if(!String.IsNullOrEmpty(forbiddenTouch) && (this.updat != UpdateSwitchMode || DisplayLabel) && !error){
			forbiddenTouchGUI = forbiddenTouch;
			if(alphaGUIImage <= 1f){
				alphaGUIImage += Time.deltaTime/speedGUIImage;	
			}
			GUI.color = new Color(1f, 1f, 1f, alphaGUIImage);
			GUI.DrawTexture(new Rect(posGUIimage.x*Screen.width, posGUIimage.y*Screen.height, posGUIimage.width*Screen.width, posGUIimage.height*Screen.height), GUITextTexture);
			
			GUI.color = new Color(0f, 0f, 0f, alphaGUIImage);
			GUI.Label(new Rect(posGUItext.x*Screen.width +1f, posGUItext.y*Screen.height+1f, posGUItext.width*Screen.width, posGUItext.height*Screen.height), TextManager.Instance.texts["MainMenu"][forbiddenTouchGUI]);
			
			GUI.color = new Color(1f, 1f, 1f, alphaGUIImage);
			GUI.Label(new Rect(posGUItext.x*Screen.width, posGUItext.y*Screen.height, posGUItext.width*Screen.width, posGUItext.height*Screen.height), TextManager.Instance.texts["MainMenu"][forbiddenTouchGUI]);
		}else if(alphaGUIImage > 0f || !String.IsNullOrEmpty(forbiddenTouchGUI)){
			
			GUI.color = new Color(1f, 1f, 1f, alphaGUIImage);
			GUI.DrawTexture(new Rect(posGUIimage.x*Screen.width, posGUIimage.y*Screen.height, posGUIimage.width*Screen.width, posGUIimage.height*Screen.height), GUITextTexture);
			
			GUI.color = new Color(0f, 0f, 0f, alphaGUIImage);
			GUI.Label(new Rect(posGUItext.x*Screen.width +1f, posGUItext.y*Screen.height+1f, posGUItext.width*Screen.width, posGUItext.height*Screen.height), TextManager.Instance.texts["MainMenu"][forbiddenTouchGUI]);
			
			GUI.color = new Color(1f, 1f, 1f, alphaGUIImage);
			GUI.Label(new Rect(posGUItext.x*Screen.width, posGUItext.y*Screen.height, posGUItext.width*Screen.width, posGUItext.height*Screen.height), TextManager.Instance.texts["MainMenu"][forbiddenTouchGUI]);
			alphaGUIImage -= Time.deltaTime/speedGUIImage;	
			if(alphaGUIImage <= 0) forbiddenTouchGUI = "";
		}
		
		if(error){
			GUI.color = new Color(1f, 1f, 1f, 0.7f*alphaError);
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Black);
			GUI.color = new Color(1f, 1f, 1f, 1f*alphaError);
			GUI.Label(new Rect(posError.x*Screen.width, posError.y*Screen.height, posError.width*Screen.width, posError.height*Screen.height), 
				TextManager.Instance.texts["ErrorMainMenuMessage"][forbiddenTouch], "errorMessage");
			GUI.Label(new Rect(posError.x*Screen.width, (posError.y + offsetNumberBuild)*Screen.height, posError.width*Screen.width, posError.height*Screen.height), 
				TextManager.Instance.texts["Config"]["NumberBuild"], "errorMessage");
		}
		
		
	}
	
	//Wait avant de lancer le mainmenu
	void UpdateWait(){
		if(time >= 1f && !enterPushed){
			if(!DataManager.Instance.alreadyPressStart) audioSMainTitle.Play();
			this.updat = UpdateUp;
		}else{
			time += Time.deltaTime;	
		}
	}
	
	// Update is called once per frame
	void Update () {
		this.updat();
		if(time >= 6f && !enterPushed){
			if(Input.GetKeyDown(KeyCode.Return)){
				audioSMainMenu.Play();
				this.updat = UpdateMainMenu;
				enterPushed = true;
				DataManager.Instance.alreadyPressStart = true;
			}
		}else{
			time += Time.deltaTime;	
		}
		
		if(DataManager.Instance.alreadyPressStart && !enterPushed){
			audioSMainMenu.time = UnityEngine.Random.value*(audioSMainMenu.time/2);
			audioSMainMenu.volume = 0;
			audioSMainMenu.Play();
			this.updat = UpdateMainMenu;
			enterPushed = true;
		}
		
		if(timeSelect >= timeFade && !iFade){
			if(selection == "Quit"){
				Application.Quit();
			}else{
				fm.FadeIn(selection);
				iFade = true;
			}
			
		}else if(enterPushed && timeSelect == 0f && audioSMainMenu.volume < 1){
			audioSMainMenu.volume += speedAudioUp*Time.deltaTime;
		}
		
		
	}
	
	//Montée de caméra
	void UpdateUp () {
		
		
		
		
		Camera.main.transform.Rotate(new Vector3(-51f*(Time.deltaTime/trueSpeedUp), 0f, 0f));
		
		if(Camera.main.transform.eulerAngles.x <= limitesUp){
			Camera.main.transform.eulerAngles = new Vector3(15f, 32.5f, 0f);
			updat = UpdateTang;
		}
		
		if(Camera.main.transform.eulerAngles.x <= limiteslowUp){
			trueSpeedUp += Time.deltaTime*addUp;
		}
	}
	
	
	//tangue de caméra
	void UpdateTang () {
		if(firststart){
			trueSpeedTang -= Time.deltaTime*addStart;
			if(trueSpeedTang <= speedTang){
				firststart = false;	
			}
		}
		
		Camera.main.transform.Rotate(new Vector3(0f, sign*51f*(Time.deltaTime/trueSpeedTang), 0f));
		
		if(Camera.main.transform.eulerAngles.y <= 15 && sign == -1f ||
			Camera.main.transform.eulerAngles.y >= 45 && sign == 1f ){
			sign *= -1f;
		}
		
		if(Camera.main.transform.eulerAngles.y <= limiteslowTangLeft){
			trueSpeedTang -= sign*Time.deltaTime*addTang;
		}else if(Camera.main.transform.eulerAngles.y >= limiteslowTangRight){
			trueSpeedTang += sign*Time.deltaTime*addTang;
		}else if(!firststart){
				trueSpeedTang = speedTang;
		}
	}
	
	//Update du main menu
	void UpdateMainMenu(){
		
		if(audioSMainTitle.isPlaying){
			if( audioSMainTitle.volume <= 0){
				audioSMainTitle.Stop();
				audioSMainTitle.volume = 100f;
			}else{
				audioSMainTitle.volume -= Time.deltaTime;
			}
		}
		
		if(!inPlace){
			if(Camera.main.transform.position.x <= 0.01f){
				//Camera.main.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				Camera.main.transform.position = new Vector3(0f, -35f, -30f);
				inPlace = true;
			}else{
				Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), Time.deltaTime/speedFadeMenu);
				Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0f, -35f, -30f), Time.deltaTime/speedFadeMenu);
			}
		}
		if(Camera.main.transform.position.x <= 1f){
			if(!error){
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
				RaycastHit hit;
				//Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(ray.direction), 0.1f);
				
				if(Physics.Raycast(ray, out hit))
				{
					
					var theGo = hit.transform.gameObject;
					if(theGo != null && theGo.tag == "MenuItem"){
						
						if(theGo.name.Contains("Cube")){
							theGo.transform.localPosition = Vector3.Lerp(theGo.transform.localPosition, new Vector3(-28f, theGo.transform.localPosition.y, theGo.transform.localPosition.z), Time.deltaTime/speedSlide);
							if(!goToBack.ContainsKey(theGo.transform.GetChild(0).name)) goToBack.Add(theGo.transform.GetChild(0).name, theGo);
							forbiddenTouch = theGo.transform.GetChild(0).name;
							if(timeBack >= timeBeforeBack) Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(Vector3.Lerp(new Vector3(0f, 0f, 1f), (theGo.transform.position - Camera.main.transform.position), 0.002f)), 0.1f);
						}else{
							theGo.transform.parent.localPosition = Vector3.Lerp(theGo.transform.parent.localPosition, new Vector3(-28f, theGo.transform.parent.localPosition.y, theGo.transform.parent.localPosition.z), Time.deltaTime/speedSlide);
							if(!goToBack.ContainsKey(theGo.name)) goToBack.Add(theGo.name, theGo.transform.parent.gameObject);
							forbiddenTouch = theGo.name;
							if(timeBack >= timeBeforeBack) Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(Vector3.Lerp(new Vector3(0f, 0f, 1f), (theGo.transform.position - Camera.main.transform.position), 0.002f)), 0.1f);
						}
						
						if(Input.GetMouseButtonDown(0)){
							if(listerror.Contains(forbiddenTouch)){
								error = true;	
							}else{
								sousChoixEnPlace = false;
								selection = forbiddenTouch;
								if(selection == "Free"){
									packFree.SetActiveRecursively(true);
								}
								this.updat = UpdateSwitchMode;
							}
							
						}
					}else{
						timeBack = 0f;
						forbiddenTouch = "";
						Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), 0.1f);
					}
				}else{
					timeBack = 0f;
					forbiddenTouch = "";
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), 0.1f);
				}	
				if(timeBack < timeBeforeBack){
					timeBack += Time.deltaTime;	
				}
				var toDelete = new List<string>();
				foreach(var el in goToBack){
					if(el.Key != forbiddenTouch){
						el.Value.transform.localPosition = Vector3.Lerp(el.Value.transform.localPosition, new Vector3(-20f, el.Value.transform.localPosition.y, el.Value.transform.localPosition.z), Time.deltaTime/speedSlide);
						if(el.Value.transform.localPosition.x >= -20.01f){
							el.Value.transform.localPosition = new Vector3(-20f, el.Value.transform.localPosition.y, el.Value.transform.localPosition.z);
							toDelete.Add(el.Key);
						}
					}
				}
				foreach(var del in toDelete){
					goToBack.Remove(del);	
				}
				
				if(alphaError > 0) alphaError -= Time.deltaTime/speedFadeError;
			}else{
				
				if(alphaError < 1) alphaError += Time.deltaTime/speedFadeError;
				if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)){
					error = false;	
				}
			}
		}
	}
	
	
	
	void UpdateSousMenu(){
		if(!error){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit))
			{
				
				var theGo = hit.transform.gameObject;
				if(theGo != null && theGo.tag == "SousMenuItem"){
					if(theGo.name.Contains("Cube")){
						theGo.transform.localScale = Vector3.Lerp(theGo.transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), Time.deltaTime/speedSlide);
						if(!goToBack.ContainsKey(theGo.transform.GetChild(0).name)) goToBack.Add(theGo.transform.GetChild(0).name, theGo);
						forbiddenTouch = theGo.transform.GetChild(0).name;
						
					}else{
						theGo.transform.parent.localScale = Vector3.Lerp(theGo.transform.parent.localScale, new Vector3(1.5f, 1.5f, 1.5f), Time.deltaTime/speedSlide);
						if(!goToBack.ContainsKey(theGo.name)) goToBack.Add(theGo.name, theGo.transform.parent.gameObject);
						forbiddenTouch = theGo.name;
						
					}
					
					if(Input.GetMouseButtonDown(0)){
						if(listerror.Contains(forbiddenTouch)){
							error = true;	
						}else{
							selection = forbiddenTouch;
						}
						
					}
				}else{
					forbiddenTouch = "";
				}
			}else{
				forbiddenTouch = "";
			}	
			var toDelete = new List<string>();
			foreach(var el in goToBack){
				if(el.Key != forbiddenTouch){
					el.Value.transform.localScale = Vector3.Lerp(el.Value.transform.localScale, new Vector3(1f, 1f, 1f), Time.deltaTime/speedSlide);
					if(el.Value.transform.localScale.x <= 1.01f){
						el.Value.transform.localScale = new Vector3(1f, 1f, 1f);
						toDelete.Add(el.Key);
					}
				}
			}
			foreach(var del in toDelete){
				goToBack.Remove(del);	
			}
			if(alphaError > 0) alphaError -= Time.deltaTime/speedFadeError;
		}else{
			
			if(alphaError < 1) alphaError += Time.deltaTime/speedFadeError;
			if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)){
				error = false;	
			}
		}
	}
	
	
	//Update de la selection
	void UpdateSwitchMode(){
		
		if(selection == "Free" ){
			if(!sousChoixEnPlace){
				if(Mathf.Abs(Camera.main.transform.position.x - positionCameraSousChoixFree.x) > 0.01f){
					Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, positionCameraSousChoixFree, Time.deltaTime/speedMoveSousChoixFree);
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.Euler(rotationCameraSousChoixFree), Time.deltaTime/speedMoveSousChoixFree);
					if(Mathf.Abs(Camera.main.transform.position.x - positionCameraSousChoixFree.x) < 15f) packFree.transform.localPosition = Vector3.Lerp(packFree.transform.localPosition, new Vector3(0f, 0f, 0f), Time.deltaTime/speedFadeMenuRapide);
				}else{
					Camera.main.transform.position = positionCameraSousChoixFree;
					Camera.main.transform.eulerAngles = rotationCameraSousChoixFree;
					sousChoixEnPlace = true;
					goToBack[forbiddenTouch].transform.localPosition = new Vector3(-20f, goToBack[forbiddenTouch].transform.localPosition.y, goToBack[forbiddenTouch].transform.localPosition.z);
					goToBack.Clear();
					forbiddenTouch = "";
					DisplayLabel = true;
				}	
			}else{
				UpdateSousMenu();
			}
			
		}else if(selection == "Option"){
			goToBack[selection].transform.position = Vector3.Lerp(goToBack[selection].transform.position, new Vector3(goToBack[selection].transform.position.x, 50f, goToBack[selection].transform.position.z), Time.deltaTime/speedChoice);
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x, 60f, Camera.main.transform.position.z), Time.deltaTime/speedChoice);
			audioSMainMenu.volume = 1f - (float)timeSelect/(float)timeFade;
			timeSelect += Time.deltaTime;
			
		}else if(selection == "Back"){
			
			if(Mathf.Abs(Camera.main.transform.position.x) <= 0.001f){
				Camera.main.transform.position = new Vector3(0f, -35f, -30f);
				Camera.main.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f));
				packFree.transform.localPosition = new Vector3(-40f, 0f, 0f);
				if(selection == "Back") selection = "";
				sousChoixEnPlace = false;
				packFree.SetActiveRecursively(false);
				this.updat = UpdateMainMenu;
				DisplayLabel = false;
				
				
				
			}else{
				Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), Time.deltaTime/speedFadeMenuRapide);
				Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0f, -35f, -30f), Time.deltaTime/speedFadeMenuRapide);
				packFree.transform.localPosition = Vector3.Lerp(packFree.transform.localPosition, new Vector3(-40f, 0f, 0f), Time.deltaTime/speedFadeMenuRapide);
				
				if(Mathf.Abs(Camera.main.transform.position.x) <= 1f){
					UpdateMainMenu();
					//DisplayLabel = true;
				}else{
					forbiddenTouch = "";
					if(goToBack.Count > 0) goToBack.Clear();
				}
			}
			
		}else if(selection == "Solo" || selection == "LAN" || selection == "Double"){
			for(int i=0; i<packFree.transform.GetChildCount();i++){
				if(packFree.transform.GetChild(i).GetChild(0).name != selection){
					packFree.transform.GetChild(i).transform.position -= new Vector3(0f, Time.deltaTime/speedTransistionSousChoix, 0f);
				}
			}
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x, 60f, Camera.main.transform.position.z), Time.deltaTime/speedChoice);
			audioSMainMenu.volume = 1f - (float)timeSelect/(float)timeFade;
			timeSelect += Time.deltaTime;
		}else if(!String.IsNullOrEmpty(selection)){
			goToBack[selection].transform.position = Vector3.Lerp(goToBack[selection].transform.position, new Vector3(goToBack[selection].transform.position.x, 50f, goToBack[selection].transform.position.z), Time.deltaTime/speedChoice);
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x, 60f, Camera.main.transform.position.z), Time.deltaTime/speedChoice);
			
			audioSMainMenu.volume = 1f - (float)timeSelect/(float)timeFade;
			timeSelect += Time.deltaTime;
		}
		
	}
}