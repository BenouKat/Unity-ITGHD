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
	
	//Glissement d'object select
	private Dictionary<string, GameObject> goToBack;
	private string forbiddenTouch;
	public float speedSlide;
	
	//audio
	public AudioSource audioSMainTitle;
	public AudioSource audioSMainMenu;
	
	//Clignotement pressstart
	public float speedPressStart;
	private float signPressStart;
	private float alphaPressStart;
	private Texture2D pressStart;
	public Rect posPressStart;
	
	//Fondu de choix
	public float speedChoice;
	private float timeSelect;
	
	//GUI text d'aide
	private Texture2D GUITextTexture;
	public Rect posGUIimage;
	public Rect posGUItext;
	public float speedGUIImage;
	private float alphaGUIImage;
	
	public GUISkin skin;
	
	private FadeManager fm;
	private bool iFade;
	
	public float timeFade;
	// Use this for initialization
	void Start () {
		
		//Load
		LoadManager.Instance.Loading();
		TextManager.Instance.LoadTextFile();
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
		alphaGUIImage = 0f;
		timeSelect = 0f;
		iFade = false;
	}
	
	void OnGUI(){
		if(time >= 6 && (this.updat == UpdateUp || this.updat == UpdateTang)){
			alphaPressStart += signPressStart*(Time.deltaTime/speedPressStart);
			if((alphaPressStart >= 1f && signPressStart == 1f) || (alphaPressStart <= 0f && signPressStart == -1f)){
				signPressStart *= -1f;	
			}
			GUI.color = new Color(1f, 1f, 1f, alphaPressStart);
			GUI.DrawTexture(new Rect(posPressStart.x*Screen.width, posPressStart.y*Screen.height, posPressStart.width*Screen.width, posPressStart.height*Screen.height), pressStart);
		}
		if(!String.IsNullOrEmpty(forbiddenTouch) && this.updat != UpdateSwitchMode){
			GUI.skin = skin;
			if(alphaGUIImage <= 1f){
				alphaGUIImage += Time.deltaTime/speedGUIImage;	
			}
			GUI.color = new Color(1f, 1f, 1f, alphaGUIImage);
			GUI.DrawTexture(new Rect(posGUIimage.x*Screen.width, posGUIimage.y*Screen.height, posGUIimage.width*Screen.width, posGUIimage.height*Screen.height), GUITextTexture);
			
			GUI.color = new Color(0f, 0f, 0f, alphaGUIImage);
			GUI.Label(new Rect(posGUItext.x*Screen.width +1f, posGUItext.y*Screen.height+1f, posGUItext.width*Screen.width, posGUItext.height*Screen.height), TextManager.Instance.texts["MainMenu"][forbiddenTouch]);
			
			GUI.color = new Color(1f, 1f, 1f, alphaGUIImage);
			GUI.Label(new Rect(posGUItext.x*Screen.width, posGUItext.y*Screen.height, posGUItext.width*Screen.width, posGUItext.height*Screen.height), TextManager.Instance.texts["MainMenu"][forbiddenTouch]);
		}else if(alphaGUIImage >= 0f){
			alphaGUIImage -= Time.deltaTime/speedGUIImage;	
		}
		
		if(timeSelect >= timeFade && !iFade){
			fm.FadeIn();
			iFade = true;
		}
	}
	
	//Wait avant de lancer le mainmenu
	void UpdateWait(){
		if(time >= 1f && !enterPushed){
			audioSMainTitle.Play();
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
			}
		}else{
			time += Time.deltaTime;	
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
			if(Camera.main.transform.position.x <= 0.001f){
				//Camera.main.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				Camera.main.transform.position = new Vector3(0f, -35f, -30f);
				inPlace = true;
			}else{
				Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), Time.deltaTime/speedFadeMenu);
				Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0f, -35f, -30f), Time.deltaTime/speedFadeMenu);
			}
		}
		if(Camera.main.transform.position.x <= 1f){
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
						Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(Vector3.Lerp(new Vector3(0f, 0f, 1f), (theGo.transform.position - Camera.main.transform.position), 0.002f)), 0.1f);
					}else{
						theGo.transform.parent.localPosition = Vector3.Lerp(theGo.transform.parent.localPosition, new Vector3(-28f, theGo.transform.parent.localPosition.y, theGo.transform.parent.localPosition.z), Time.deltaTime/speedSlide);
						if(!goToBack.ContainsKey(theGo.name)) goToBack.Add(theGo.name, theGo.transform.parent.gameObject);
						forbiddenTouch = theGo.name;
						Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(Vector3.Lerp(new Vector3(0f, 0f, 1f), (theGo.transform.position - Camera.main.transform.position), 0.002f)), 0.1f);
					}
					
					if(Input.GetMouseButtonDown(0)){
						this.updat = UpdateSwitchMode;
					}
				}else{
					forbiddenTouch = "";
					Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), 0.1f);
				}
			}else{
				forbiddenTouch = "";
				Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), 0.1f);
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
		}
	}
	
	
	//Update de la selection
	void UpdateSwitchMode(){
		goToBack[forbiddenTouch].transform.position = Vector3.Lerp(goToBack[forbiddenTouch].transform.position, new Vector3(goToBack[forbiddenTouch].transform.position.x, 50f, goToBack[forbiddenTouch].transform.position.z), Time.deltaTime/speedChoice);
		Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x, 60f, Camera.main.transform.position.z), Time.deltaTime/speedChoice);
		
		timeSelect += Time.deltaTime;
	}
}
