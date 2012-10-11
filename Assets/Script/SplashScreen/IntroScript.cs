using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class IntroScript : MonoBehaviour {

	enum LOGIN_STATUT{
		VALID,
		CHOOSE,
		PASSWORD,
		INVALID,
		NONE
	}
	
	enum INTRO_STATE{
		QUESTION1,
		QUESTION2,
		USERNAME,
		PASSWORD,
		RETYPEPASSWORD,
		FINISH,
		NONE
	}
	//GUI
	public Rect labelConnecting;
	public Rect ButtonProfiles;
	public Rect nextButton;
	public Rect prevButton;
	public Rect labelInfo;
	public Rect ChooseButton;
	public Rect CreateNewButton;
	public Rect BackButton;
	public Rect TextAreaPassword;
	
	
	//Cubes qui s'envolent
	public List<GameObject> EnvolCube;
	public List<float> angleStart;
	public List<float> speedCube;
	
	//Speech
	public Rect labelSpeech;
	public Rect labelAnswer;
	public Rect TextFieldCreate;
	private string speechToDisplay;
	private int posSpeech;
	public float speedSpeech;
	private int answer1selected;
	private bool answer1valid;
	private int answer2selected;
	private bool answer2valid;
	private bool wrongPassword;
	private INTRO_STATE stateIntro;
	private float alphaAnswer;
	public float speedAlphaAnswer;
	private string username;
	private string newpassword;
	private string newpasswordagain;
	private bool inputFired;
	private bool inputPassTextFired;
	private bool specialInputFired;
	
	//Speech Question
	public Vector3 LookRotation;
	public float speedRotationCamQues;
	public List<GameObject> cubesQuestion;
	public float speedMonteCube;
	private Color theBumpColor;
	public float speedBump;
	public float limitBump;
	public GameObject redBloc;
	public float speedRedBloc;
	public ParticleSystem psExplode;
	public float speedAlphaBloc;
	public Camera camOrtho;
	public GameObject positionner;
	
	
	
	//Finish
	public ParticleSystem psExplosionFinal;
	public GameObject bigCube;
	public float speedScaleCube;
	public float speedAlphaCamZoom;
	public float speedAlphaCamZoomY;
	public float speedZoomProgress;
	public float limitblank;
	private Texture2D blank;
	private float alphaBlank;
	public float speedAlphaBlank;
	private bool enterPushedForFinish;
	
	public float offset;
	public float speedLetters;
	
	
	//blui
	public GameObject cubeBlui;
	public float speedRotation;
	public float speedTranslation;
	public float speedRotationRalenti;
	public float speedTranslationRalenti;
	public float speedRotationCamera;
	public float speedRotationCameraAfterRalenti;
	public float limitZRalenti;
	public float limitZRespeed;
	public float limitZReboost;
	private bool okforSpeech;
	
	public GUISkin skin;
	private string labelToDisplay;
	private int posLength;
	//private float time;
	
	
	//choosing profile
	private int startNumber;
	private int profileSelected;
	private string infoToDisplay;
	private int posLengthInfo;
	public float speedLettersInfo;
	private string password;
	
	
	private LOGIN_STATUT ls;
	
	void Start(){
		TextManager.Instance.LoadTextFile();
		labelToDisplay = TextManager.Instance.texts["SplashScreen"]["LOADING"];
		posLength = 0;
		blank = (Texture2D) Resources.Load("blank");
	//	time = 0;
		ls = LOGIN_STATUT.NONE;
		StartCoroutine(TextLogin());
		startNumber = 0;
		profileSelected = -1;
		password = "";
		okforSpeech = false;
		wrongPassword = false;
		stateIntro = INTRO_STATE.NONE;
		answer1selected = -1;
		answer2selected = -1;
		alphaAnswer = 0f;
		inputFired = false;
		username = "";
		newpassword = "";
		newpasswordagain = "";
		speechToDisplay = "";
		alphaBlank = 0f;
		enterPushedForFinish = false;
	}
	
	
	void OnGUI(){
		GUI.skin = skin;
		
		
		
		if(ls != LOGIN_STATUT.VALID && ls != LOGIN_STATUT.INVALID) GUI.Label(new Rect(labelConnecting.x*Screen.width, labelConnecting.y*Screen.height, labelConnecting.width*Screen.width, labelConnecting.height*Screen.height), labelToDisplay.Substring(0, posLength), "CenteredLabel");
	
		if(ls == LOGIN_STATUT.PASSWORD){
			
			GUI.SetNextControlName("pass");
			password = GUI.PasswordField(new Rect(TextAreaPassword.x*Screen.width, TextAreaPassword.y*Screen.height, TextAreaPassword.width*Screen.width, TextAreaPassword.height*Screen.height), password, '*');
			
			GUI.FocusControl("pass");
			
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Return){
				if(password == ProfileManager.Instance.profiles[profileSelected].password){
					ls = LOGIN_STATUT.NONE;
					ProfileManager.Instance.setCurrentProfile(ProfileManager.Instance.profiles[profileSelected]);
					labelToDisplay = TextManager.Instance.texts["SplashScreen"]["CONNECTING_SUCCEED"].Replace("USER_NAME", ProfileManager.Instance.currentProfile.name);
					posLength = 0;	
					StartCoroutine(TextDisplay(LOGIN_STATUT.VALID));
					stateIntro = INTRO_STATE.FINISH;
				}else{
					labelToDisplay = TextManager.Instance.texts["SplashScreen"]["CONNECTING_PASSWORDFAIL"];
					posLength = 0;	
					StartCoroutine(TextDisplay(LOGIN_STATUT.NONE));
					password = "";
				}
			}
			
			if(GUI.Button(new Rect(BackButton.x*Screen.width, BackButton.y*Screen.height, BackButton.width*Screen.width, BackButton.height*Screen.height), "< back", "ButtonSimple")){
				ls = LOGIN_STATUT.NONE;
				labelToDisplay = TextManager.Instance.texts["SplashScreen"]["CONNECTING_CHOOSE"];
				posLength = 0;
				StartCoroutine(TextDisplay(LOGIN_STATUT.CHOOSE));
			}
		}
		
		
		if(ls == LOGIN_STATUT.CHOOSE){
			for(int i=startNumber; i<startNumber+4 && i<ProfileManager.Instance.profiles.Count;i++){
				if(GUI.Button(new Rect(ButtonProfiles.x*Screen.width, (ButtonProfiles.y + offset*(i-startNumber))*Screen.height, ButtonProfiles.width*Screen.width, ButtonProfiles.height*Screen.height), ProfileManager.Instance.profiles[i].name, profileSelected == i ? "ButtonSimpleSelected" : "ButtonSimple")){
					profileSelected = i;
					infoToDisplay = "Name : " + ProfileManager.Instance.profiles[i].name + "\n\n" +
					"Number of song played : " + ProfileManager.Instance.profiles[i].scoreOnSong.Count + "\n\n" +
					"Game time : " + "00:00h" + "\n\n\n\n" +
					"Story mode progression : " + "0%" + "\n\n" + 
					"Achievement : " + "0/400" + "\n\n";
					StopAllCoroutines();
					StartCoroutine(TextInfo());
				}
				
			}
			if((startNumber+4) < ProfileManager.Instance.profiles.Count){
				if(GUI.Button(new Rect(nextButton.x*Screen.width, nextButton.y*Screen.height, nextButton.width*Screen.width, nextButton.height*Screen.height), "next >", "ButtonSimple")){
					startNumber += 4;
				}
			}
			if(startNumber > 0){
				if(GUI.Button(new Rect(prevButton.x*Screen.width, prevButton.y*Screen.height, prevButton.width*Screen.width, prevButton.height*Screen.height), "< prev", "ButtonSimple")){
					startNumber -= 4;
				}
			}
			
			if(GUI.Button(new Rect(CreateNewButton.x*Screen.width, CreateNewButton.y*Screen.height, CreateNewButton.width*Screen.width, CreateNewButton.height*Screen.height), "NEW PROFILE", "ButtonSimple")){
				labelToDisplay = "";
				posLength = 0;
				ls = LOGIN_STATUT.INVALID;
			}
			
			if(profileSelected != -1){
				GUI.Label(new Rect(labelInfo.x*Screen.width, labelInfo.y*Screen.height, labelInfo.width*Screen.width, labelInfo.height*Screen.height), infoToDisplay.Substring(0, posLengthInfo));
			
				if(GUI.Button(new Rect(ChooseButton.x*Screen.width, ChooseButton.y*Screen.height, ChooseButton.width*Screen.width, ChooseButton.height*Screen.height), "VALID", "ButtonSimple")){
					ls = LOGIN_STATUT.NONE;
					posLength = 0;
					labelToDisplay = TextManager.Instance.texts["SplashScreen"]["CONNECTING_PASSWORD"] + " : ";
					StartCoroutine(TextDisplay(LOGIN_STATUT.PASSWORD));
				}
			}
		}
		
		if(ls == LOGIN_STATUT.INVALID && okforSpeech){
			GUI.Label(new Rect(labelSpeech.x*Screen.width, labelSpeech.y*Screen.height, labelSpeech.width*Screen.width, labelSpeech.height*Screen.height), speechToDisplay.Substring(0, posSpeech) , "CenteredBrightLabel");
		
			if(stateIntro == INTRO_STATE.QUESTION1){
				if(answer1selected != -1){
					GUI.color = new Color(1f, 1f, 1f, alphaAnswer);
					GUI.Label(new Rect(labelAnswer.x*Screen.width, labelAnswer.y*Screen.height, labelAnswer.width*Screen.width, labelAnswer.height*Screen.height), 
						TextManager.Instance.texts["SplashScreen"]["QUESTION_1_ANSWER_" + answer1selected], "CenteredBrightLabel");
				}
			}else if(stateIntro == INTRO_STATE.QUESTION2){
				if(answer2selected != -1){
					GUI.color = new Color(1f, 1f, 1f, alphaAnswer);
					GUI.Label(new Rect(labelAnswer.x*Screen.width, labelAnswer.y*Screen.height, labelAnswer.width*Screen.width, labelAnswer.height*Screen.height), 
						TextManager.Instance.texts["SplashScreen"]["QUESTION_2_ANSWER_" + answer2selected], "CenteredBrightLabel");
				}
			}else if(stateIntro == INTRO_STATE.USERNAME){
				if(Event.current.isKey && Event.current.keyCode == KeyCode.Return && !String.IsNullOrEmpty(username)){
					stateIntro = INTRO_STATE.NONE;
					specialInputFired = true;
				}else{
					GUI.SetNextControlName("user");
					username = GUI.TextField(new Rect(TextFieldCreate.x*Screen.width, TextFieldCreate.y*Screen.height, TextFieldCreate.width*Screen.width, TextFieldCreate.y*Screen.height), username, "CenteredBrightLabel");
					GUI.FocusControl("user");	
				}
				
			}else if(stateIntro == INTRO_STATE.PASSWORD){
				if(Event.current.isKey && Event.current.keyCode == KeyCode.Return && !String.IsNullOrEmpty(newpassword)){
					stateIntro = INTRO_STATE.NONE;
					specialInputFired = true;
				}else{
					GUI.SetNextControlName("newpassword");
					newpassword = GUI.PasswordField(new Rect(TextFieldCreate.x*Screen.width, TextFieldCreate.y*Screen.height, TextFieldCreate.width*Screen.width, TextFieldCreate.y*Screen.height), newpassword, '*', "CenteredBrightLabel");
					GUI.FocusControl("newpassword");
				}
				
				
			}else if(stateIntro == INTRO_STATE.RETYPEPASSWORD){
				if(Event.current.isKey && Event.current.keyCode == KeyCode.Return && !String.IsNullOrEmpty(newpasswordagain)){
					if(newpassword != newpasswordagain){
						wrongPassword = true;
					}
					stateIntro = INTRO_STATE.NONE;
					specialInputFired = true;
				}else{
					GUI.SetNextControlName("newpasswordagain");
					newpasswordagain = GUI.PasswordField(new Rect(TextFieldCreate.x*Screen.width, TextFieldCreate.y*Screen.height, TextFieldCreate.width*Screen.width, TextFieldCreate.y*Screen.height), newpasswordagain, '*', "CenteredBrightLabel");
					GUI.FocusControl("newpasswordagain");
				}
				
				
			}
			
			
		}	
			
			
		GUI.color = new Color(1f, 1f, 1f, alphaBlank);
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), blank);
	
	}
	
	
	void Update(){
		if((ls == LOGIN_STATUT.VALID || ls == LOGIN_STATUT.INVALID) && Camera.main.transform.eulerAngles.y > 0 && stateIntro != INTRO_STATE.QUESTION1 && stateIntro != INTRO_STATE.QUESTION2){
			if(cubeBlui.transform.position.z < limitZRalenti){
				cubeBlui.transform.Rotate(new Vector3(0f, 0f , Time.deltaTime*speedRotation));
				cubeBlui.transform.position = new Vector3(cubeBlui.transform.position.x, cubeBlui.transform.position.y, cubeBlui.transform.position.z + Time.deltaTime*speedTranslation);
				Camera.main.transform.Rotate(new Vector3(0f, Time.deltaTime*speedRotationCamera, 0f));
			}else if(cubeBlui.transform.position.z < limitZRespeed){
				cubeBlui.transform.Rotate(new Vector3(0f, 0f , Time.deltaTime*speedRotationRalenti));
				cubeBlui.transform.position = new Vector3(cubeBlui.transform.position.x, cubeBlui.transform.position.y, cubeBlui.transform.position.z + Time.deltaTime*speedTranslationRalenti);
				Camera.main.transform.Rotate(new Vector3(0f, Time.deltaTime*speedRotationCamera, 0f));
			}else{
				var distBase = Vector3.Distance(new Vector3(0f, 0f, limitZRespeed), new Vector3(0f, 0f, limitZReboost));
				var distActual = Vector3.Distance(new Vector3(0f, 0f, cubeBlui.transform.position.z), new Vector3(0f, 0f, limitZRespeed));
				var percent = distActual/distBase;
				if(percent > 1f) percent = 1f;
				var speedRotat = Mathf.Lerp(speedRotationRalenti, speedRotation, percent);
				var speedTranslat = Mathf.Lerp(speedTranslationRalenti, speedTranslation, percent);
				var speedmovecam = Mathf.Lerp(speedRotationCamera, speedRotationCameraAfterRalenti, percent);
				cubeBlui.transform.Rotate(new Vector3(0f , 0f , Time.deltaTime*speedRotat));
				cubeBlui.transform.position = new Vector3(cubeBlui.transform.position.x, cubeBlui.transform.position.y, cubeBlui.transform.position.z + Time.deltaTime*speedTranslat);
				Camera.main.transform.Rotate(new Vector3(0f, Time.deltaTime*speedmovecam*(Camera.main.transform.eulerAngles.y > 30f ? 1f : Camera.main.transform.eulerAngles.y/35f), 0f));
				if(Camera.main.transform.eulerAngles.y < 5f && !okforSpeech){
					okforSpeech = true;
					if(stateIntro != INTRO_STATE.FINISH){
						
						StartCoroutine(SpeechDisplay());
						
					}else{
						
						StartCoroutine(finishDisplay(true));
						
					}
				}
			}
		}
		
		if(EnvolCube.Count > 0 && Camera.main.transform.eulerAngles.y < 45f){
			for(int i=0;i<EnvolCube.Count;i++){
				if(angleStart.ElementAt(i) > Camera.main.transform.eulerAngles.y){
					if(EnvolCube.ElementAt(i).active == false){
						EnvolCube.ElementAt(i).SetActiveRecursively(true);
					}
					EnvolCube.ElementAt(i).transform.position += EnvolCube.ElementAt(i).transform.forward*Time.deltaTime*speedCube.ElementAt(i);
					if(EnvolCube.ElementAt(i).transform.position.z > 0){
						EnvolCube.ElementAt(i).transform.GetChild(0).particleSystem.Stop();
						EnvolCube.ElementAt(i).active = false;
						EnvolCube.Remove(EnvolCube.ElementAt(i));
						angleStart.Remove(angleStart.ElementAt(i));
						speedCube.Remove(speedCube.ElementAt(i));
						i--;
					}
				}
			}
		}
		
		
		if(stateIntro == INTRO_STATE.QUESTION1){

			if(!answer1valid){

				if(Mathf.Abs(Camera.main.transform.eulerAngles.y - positionner.transform.eulerAngles.y) < 1f){
			
					if(Mathf.Abs(cubesQuestion[0].transform.position.y - camOrtho.transform.position.y) < 0.1f){
			
						Ray ray = camOrtho.ScreenPointToRay(Input.mousePosition);	
						RaycastHit hit;
					
						//Faire un raycast
						if(Physics.Raycast(ray, out hit))
						{

							var theGo = hit.transform.gameObject;
							if(theGo != null && theGo.tag == "QuestionObject"){
				
								//Faire clignoter le bloc dans le raycast et mettre à jour le answerselected
								answer1selected = System.Convert.ToInt32(theGo.name);
								if(alphaAnswer < 1f) alphaAnswer += Time.deltaTime*speedAlphaAnswer;
								if(Input.GetMouseButtonDown(0)){
									answer1valid = true;
									redBloc.transform.position = new Vector3(theGo.transform.position.x, theGo.transform.position.y - 5f, theGo.transform.position.z);
									StartCoroutine(RedBlocQuestion1());
								}
							}else{
								Debug.Log(theGo == null);
								if(theGo != null) Debug.Log(theGo.name);
								if(alphaAnswer > 0f) alphaAnswer -= Time.deltaTime*speedAlphaAnswer;
								//answer1selected = -1;	
							}
						}else{

							if(alphaAnswer > 0f) alphaAnswer -= Time.deltaTime*speedAlphaAnswer;
							//answer1selected = -1;	
						}
						
						if(Mathf.Abs(cubesQuestion[0].transform.position.y - camOrtho.transform.position.y) < 0.001f){
							foreach(var cq in cubesQuestion){
								//Faire venir les blocs
								cq.transform.position = Vector3.Lerp(cq.transform.position, new Vector3(cq.transform.position.x, camOrtho.transform.position.y, cq.transform.position.z), Time.deltaTime/speedMonteCube);
							}	
						}
					}else{
						foreach(var cq in cubesQuestion){
							//Faire venir les blocs
							cq.transform.position = Vector3.Lerp(cq.transform.position, new Vector3(cq.transform.position.x, camOrtho.transform.position.y, cq.transform.position.z), Time.deltaTime/speedMonteCube);
						}
					}
					
					if(Mathf.Abs(Camera.main.transform.eulerAngles.y - positionner.transform.eulerAngles.y) < 0.001f){
						Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, positionner.transform.eulerAngles, Time.deltaTime/speedRotationCamQues);
					}
					
				}else{
					Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, positionner.transform.eulerAngles, Time.deltaTime/speedRotationCamQues);
				}
				
			}
			
			
			
			
			foreach(var cb in cubesQuestion){
				if(System.Convert.ToInt32(cb.name) == answer1selected || cb.renderer.material.color.r > limitBump){
					cb.renderer.material.color = new Color(theBumpColor.r, theBumpColor.g, theBumpColor.b, cb.renderer.material.color.a);
				}else{
					cb.renderer.material.color = new Color(limitBump, limitBump, limitBump, cb.renderer.material.color.a);
				}
			}
			
			if(theBumpColor.r < limitBump){
				theBumpColor = new Color(1f, 1f, 1f, 1f);
			}else{
				theBumpColor = new Color(theBumpColor.r - Time.deltaTime/speedBump, theBumpColor.g - Time.deltaTime/speedBump, theBumpColor.b - Time.deltaTime/speedBump, 1f);	
			}
			
		}else if(stateIntro == INTRO_STATE.QUESTION2){
			if(!answer2valid){
				if(Mathf.Abs(Camera.main.transform.eulerAngles.y - positionner.transform.eulerAngles.y) < 1f){
					if(Mathf.Abs(cubesQuestion[0].transform.position.y - camOrtho.transform.position.y) < 0.1f){
						Ray ray = camOrtho.ScreenPointToRay(Input.mousePosition);	
						RaycastHit hit;
					
						//Faire un raycast
						if(Physics.Raycast(ray, out hit))
						{
							var theGo = hit.transform.gameObject;
							if(theGo != null && theGo.tag == "QuestionObject"){
								//Faire clignoter le bloc dans le raycast et mettre à jour le answerselected
								answer2selected = System.Convert.ToInt32(theGo.name);
								if(alphaAnswer < 1f) alphaAnswer += Time.deltaTime*speedAlphaAnswer;
								if(Input.GetMouseButtonDown(0)){
									answer2valid = true;
									redBloc.transform.position = new Vector3(theGo.transform.position.x, theGo.transform.position.y - 5f, theGo.transform.position.z);
									StartCoroutine(RedBlocQuestion2());
								}
							}else{
								if(alphaAnswer > 0f) alphaAnswer -= Time.deltaTime*speedAlphaAnswer;
								//answer2selected = -1;	
							}
						}else{
							if(alphaAnswer > 0f) alphaAnswer -= Time.deltaTime*speedAlphaAnswer;
							//answer2selected = -1;	
						}
						
						if(Mathf.Abs(cubesQuestion[0].transform.position.y - camOrtho.transform.position.y) < 0.001f){
							foreach(var cq in cubesQuestion){
								//Faire venir les blocs
								cq.transform.position = Vector3.Lerp(cq.transform.position, new Vector3(cq.transform.position.x, camOrtho.transform.position.y, cq.transform.position.z), Time.deltaTime/speedMonteCube);
							}	
						}
					}else{
						foreach(var cq in cubesQuestion){
							//Faire venir les blocs
							cq.transform.position = Vector3.Lerp(cq.transform.position, new Vector3(cq.transform.position.x, camOrtho.transform.position.y, cq.transform.position.z), Time.deltaTime/speedMonteCube);
						}
					}
					
					if(Mathf.Abs(Camera.main.transform.eulerAngles.y - positionner.transform.eulerAngles.y) < 0.001f){
						Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, positionner.transform.eulerAngles, Time.deltaTime/speedRotationCamQues);
					}
					
				}else{
					Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, positionner.transform.eulerAngles, Time.deltaTime/speedRotationCamQues);
				}
				
			}
			
			
			
			
			foreach(var cb in cubesQuestion){
				if(System.Convert.ToInt32(cb.name) == answer2selected || cb.renderer.material.color.r > limitBump){
					cb.renderer.material.color = new Color(theBumpColor.r, theBumpColor.g, theBumpColor.b, cb.renderer.material.color.a);
				}else{
					cb.renderer.material.color = new Color(limitBump, limitBump, limitBump, cb.renderer.material.color.a);
				}
			}
			
			if(theBumpColor.r < limitBump){
				theBumpColor = new Color(1f, 1f, 1f, 1f);
			}else{
				theBumpColor = new Color(theBumpColor.r - Time.deltaTime/speedBump, theBumpColor.g - Time.deltaTime/speedBump, theBumpColor.b - Time.deltaTime/speedBump, 1f);	
			}
			
		}
		
		
		if(Input.GetKeyDown(KeyCode.Return)){
			inputFired = true;	
			inputPassTextFired = true;
			if(stateIntro == INTRO_STATE.FINISH) enterPushedForFinish = true;
		}else{
			inputFired = false;	
		}
		
		if(enterPushedForFinish){
			if(alphaBlank < 1f){
				alphaBlank += Time.deltaTime*speedAlphaBlank;
			}else{
				Application.LoadLevel("MainMenu");
			}
			
		}
	
	}
					
	public int returnResultOfQuestion(){
		switch(answer2selected){
		case 1:
			switch(answer1selected){
			case 1:
				return 1;
			case 2:
				return 1;
			case 3:
				return 2;
			case 4:
				return 3;
			}
			return -1;
		case 2:
			switch(answer1selected){
			case 1:
				return 1;
			case 2:
				return 2;
			case 3:
				return 2;
			case 4:
				return 3;
			}
			return -1;
		case 3:
			switch(answer1selected){
			case 1:
				return 2;
			case 2:
				return 3;
			case 3:
				return 3;
			case 4:
				return 4;
			}
			return -1;
		case 4:
			return 4;
		}
		return -1;
	}	
	
	
	IEnumerator RedBlocQuestion1(){
		while(redBloc.transform.position.y < cubesQuestion[answer1selected - 1].transform.position.y){
			redBloc.transform.position += new Vector3(0f, Time.deltaTime*speedRedBloc, 0f); 
			yield return new WaitForEndOfFrame();
		}
		redBloc.transform.position = new Vector3(0f, camOrtho.transform.position.y-5f, 0f);
		psExplode.transform.position = cubesQuestion[answer1selected - 1].transform.position;
		psExplode.transform.position -= new Vector3(0f, 0f, 1f);
		psExplode.Play();
		while(cubesQuestion[0].renderer.material.color.a > 0){
			foreach(var cb in cubesQuestion){
				cb.renderer.material.color = new Color(cb.renderer.material.color.r, cb.renderer.material.color.g, cb.renderer.material.color.b, cb.renderer.material.color.a - Time.deltaTime*speedAlphaBloc);
			}
			alphaAnswer -= Time.deltaTime*speedAlphaBloc;
			yield return new WaitForEndOfFrame();	
		}
		
		foreach(var cb in cubesQuestion){
			cb.transform.position = new Vector3(cb.transform.position.x, camOrtho.transform.position.y -5f, cb.transform.position.z);
			cb.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		}
		
		while(Camera.main.transform.eulerAngles.y > 0.1f && stateIntro != INTRO_STATE.QUESTION2){
			Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), Time.deltaTime/speedRotationCamQues);
			yield return new WaitForEndOfFrame();	
		}
		
		stateIntro = INTRO_STATE.NONE;
	}
	
	IEnumerator RedBlocQuestion2(){
		while(redBloc.transform.position.y < cubesQuestion[answer2selected - 1].transform.position.y){
			redBloc.transform.position += new Vector3(0f, Time.deltaTime*speedRedBloc, 0f);
			yield return new WaitForEndOfFrame();
		}
		redBloc.transform.position = new Vector3(0f, camOrtho.transform.position.y-5f, 0f);
		psExplode.transform.position = cubesQuestion[answer2selected - 1].transform.position;
		psExplode.transform.position -= new Vector3(0f, 0f, 1f);
		psExplode.Play();
		while(cubesQuestion[0].renderer.material.color.a > 0){
			foreach(var cb in cubesQuestion){
				cb.renderer.material.color = new Color(cb.renderer.material.color.r, cb.renderer.material.color.g, cb.renderer.material.color.b, cb.renderer.material.color.a - Time.deltaTime*speedAlphaBloc);
			}
			alphaAnswer -= Time.deltaTime*speedAlphaBloc;
			yield return new WaitForEndOfFrame();	
		}
		
		foreach(var cb in cubesQuestion){
			cb.transform.position = new Vector3(cb.transform.position.x, camOrtho.transform.position.y-5f, cb.transform.position.z);
			cb.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		}
		
		while(Camera.main.transform.eulerAngles.y > 0.1f){
			Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), Time.deltaTime/speedRotationCamQues);
			yield return new WaitForEndOfFrame();	
		}
		
		stateIntro = INTRO_STATE.NONE;
	}
	
	IEnumerator TextInfo(){
		posLengthInfo = 0;
		while(posLengthInfo < infoToDisplay.Length){
			posLengthInfo++;
			yield return new WaitForSeconds(speedLettersInfo);
		}
	}
	
	IEnumerator TextDisplay(LOGIN_STATUT lst){
		while(posLength < labelToDisplay.Length){
			posLength++;
			yield return new WaitForSeconds(speedLetters);
		}
		if(lst != LOGIN_STATUT.NONE){
			if(lst == LOGIN_STATUT.INVALID || lst == LOGIN_STATUT.VALID) yield return new WaitForSeconds(1f);
			ls = lst;	
		}
	}
	
	
	IEnumerator finishDisplay(bool waiting){
		
		if(waiting) yield return new WaitForSeconds(1.5f);
		psExplosionFinal.Play();
		
		bigCube.transform.localScale = new Vector3(30f, 30f, 30f);
		while(bigCube.transform.localScale.x > 25f){
			bigCube.transform.localScale -= new Vector3(Time.deltaTime*speedScaleCube, Time.deltaTime*speedScaleCube, Time.deltaTime*speedScaleCube);
			yield return new WaitForEndOfFrame();
		}
		
		psExplosionFinal.Play();
		
		bigCube.transform.localScale = new Vector3(30f, 30f, 30f);
		while(bigCube.transform.localScale.x > 20f){
			bigCube.transform.localScale -= new Vector3(Time.deltaTime*speedScaleCube, Time.deltaTime*speedScaleCube, Time.deltaTime*speedScaleCube);
			yield return new WaitForEndOfFrame();
		}
		
		yield return new WaitForSeconds(2f);
		
		
		while(alphaBlank < 1f){
			Camera.main.transform.position -= new Vector3(0f, Camera.main.transform.position.y <= 0 ? 0 : Time.deltaTime*speedAlphaCamZoomY, -Time.deltaTime*speedAlphaCamZoom);
			speedAlphaCamZoom += Time.deltaTime*speedZoomProgress;
			if(Camera.main.transform.position.z > limitblank){
				alphaBlank += Time.deltaTime*speedAlphaBlank;
			}	
			yield return new WaitForEndOfFrame();
		}
		
		
		if(!enterPushedForFinish) Application.LoadLevel("MainMenu");
		
	}
	
	
	
	IEnumerator SpeechDisplay(){
		for(int i=0;i<=4;i++){
			speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_" + i];
			posSpeech = 0;
			while(posSpeech < speechToDisplay.Length || !inputFired){
				if(posSpeech < speechToDisplay.Length){
					posSpeech++;
					if(inputPassTextFired && posSpeech > 1){
						posSpeech = speechToDisplay.Length;
					}
					inputPassTextFired = false;
					yield return new WaitForSeconds(speedSpeech);
				}else{
					inputPassTextFired = false;
					yield return new WaitForEndOfFrame();
					
				}
			}
		}
		
		speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_5"];
		posSpeech = 0;
		while(posSpeech < speechToDisplay.Length){
			posSpeech++;
			if(inputPassTextFired && posSpeech > 1){
				posSpeech = speechToDisplay.Length;
			}
			inputPassTextFired = false;
			yield return new WaitForSeconds(speedSpeech);
		}
		stateIntro = INTRO_STATE.QUESTION1;
		
		while(!answer1valid){
			yield return new WaitForEndOfFrame();	
		}
		
		for(int i=6;i<=9;i++){
			speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_"+ i];
			posSpeech = 0;
			while(posSpeech < speechToDisplay.Length || !inputFired){
				if(posSpeech < speechToDisplay.Length){
					posSpeech++;
					if(inputPassTextFired && posSpeech > 1){
						posSpeech = speechToDisplay.Length;
					}
					inputPassTextFired = false;
					yield return new WaitForSeconds(speedSpeech);
				}else{
					inputPassTextFired = false;
					yield return new WaitForEndOfFrame();
				}
			}
		}
		
		speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_10"];
		posSpeech = 0;
		while(posSpeech < speechToDisplay.Length){
			posSpeech++;
			if(inputPassTextFired && posSpeech > 1){
				posSpeech = speechToDisplay.Length;
			}
			inputPassTextFired = false;
			yield return new WaitForSeconds(speedSpeech);
		}
		
		stateIntro = INTRO_STATE.QUESTION2;
		while(!answer2valid){
			yield return new WaitForEndOfFrame();	
		}
		
		for(int i=11;i<=12;i++){
			speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_" + i];
			posSpeech = 0;
			while(posSpeech < speechToDisplay.Length || !inputFired){
				if(posSpeech < speechToDisplay.Length){
					posSpeech++;
					if(inputPassTextFired && posSpeech > 1){
						posSpeech = speechToDisplay.Length;
					}
					inputPassTextFired = false;
					yield return new WaitForSeconds(speedSpeech);
				}else{
					inputPassTextFired = false;
					yield return new WaitForEndOfFrame();
				}
			}
		}
		
		for(int i=1;i<=3;i++){
			speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_RESULT_" + returnResultOfQuestion() + "_" + i];
			posSpeech = 0;
			while(posSpeech < speechToDisplay.Length || !inputFired){
				if(posSpeech < speechToDisplay.Length){
					posSpeech++;
					if(inputPassTextFired && posSpeech > 1){
						posSpeech = speechToDisplay.Length;
					}
					inputPassTextFired = false;
					yield return new WaitForSeconds(speedSpeech);
				}else{
					inputPassTextFired = false;
					yield return new WaitForEndOfFrame();
				}
			}
		}
		
		for(int i=13;i<=15;i++){
			yield return new WaitForEndOfFrame();
			if(wrongPassword){
				speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_PASSWORDWRONG"];
				newpassword = "";
				newpasswordagain = "";
				wrongPassword = false;
			}else{
				speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_" + i];
			}
			
			posSpeech = 0;
			var notChanged = false;
			specialInputFired = false;
			while(posSpeech < speechToDisplay.Length || !specialInputFired){
				if(posSpeech < speechToDisplay.Length){
					posSpeech++;
					yield return new WaitForSeconds(speedSpeech);
				}else{
					if(!notChanged){
						if(i == 13) stateIntro = INTRO_STATE.USERNAME;
						if(i == 14) stateIntro = INTRO_STATE.PASSWORD;
						if(i == 15) stateIntro = INTRO_STATE.RETYPEPASSWORD;	
						notChanged = true;
					}
					yield return new WaitForEndOfFrame();
				}
			}
			
			if(wrongPassword){
				i = 13;
			}
		}
		
		var newuser = new Profile(username, newpassword);
		ProfileManager.Instance.profiles.Add(newuser);
		ProfileManager.Instance.setCurrentProfile(ProfileManager.Instance.profiles.Last());
		ProfileManager.Instance.SaveProfile();
		
		for(int i=16;i<=20;i++){
			speechToDisplay = TextManager.Instance.texts["SplashScreen"]["SPEECH_" + i];
			if(i == 20) speechToDisplay = speechToDisplay.Replace("USER_NAME", username);
			posSpeech = 0;
			while(posSpeech < speechToDisplay.Length || !inputFired){
				if(posSpeech < speechToDisplay.Length){
					posSpeech++;
					if(inputPassTextFired && posSpeech > 1){
						posSpeech = speechToDisplay.Length;
					}
					inputPassTextFired = false;
					yield return new WaitForSeconds(speedSpeech);
					
				}else{
					inputPassTextFired = false;
					yield return new WaitForEndOfFrame();
				}
			}
		}
		
		speechToDisplay = "";
		posSpeech = 0;
		stateIntro = INTRO_STATE.FINISH;
		StartCoroutine(finishDisplay(false));
	}
	
	IEnumerator TextLogin(){
		yield return new WaitForFixedUpdate();
		while(posLength < labelToDisplay.Length){
			posLength++;
			yield return new WaitForSeconds(speedLetters);
		}
		LoadManager.Instance.Loading();
		labelToDisplay = TextManager.Instance.texts["SplashScreen"]["CONNECTING"];
		posLength = 0;
		while(posLength < labelToDisplay.Length){
			posLength++;
			yield return new WaitForSeconds(speedLetters);
		}
		yield return new WaitForSeconds(1f);
		ProfileManager.Instance.LoadProfiles();
		var verif = ProfileManager.Instance.verifyCurrentProfile();
		if(verif){
			labelToDisplay = TextManager.Instance.texts["SplashScreen"]["CONNECTING_SUCCEED"].Replace("USER_NAME", ProfileManager.Instance.currentProfile.name);
			posLength = 0;
			while(posLength < labelToDisplay.Length){
				posLength++;
				yield return new WaitForSeconds(speedLetters);
				
			}
			yield return new WaitForSeconds(1f);
			ls = LOGIN_STATUT.VALID;
			stateIntro = INTRO_STATE.FINISH;
		}else if(ProfileManager.Instance.profiles.Count > 0){
			labelToDisplay = TextManager.Instance.texts["SplashScreen"]["CONNECTING_CHOOSE"];
			posLength = 0;
			while(posLength < labelToDisplay.Length){
				posLength++;
				yield return new WaitForSeconds(speedLetters);
			}
			ls = LOGIN_STATUT.CHOOSE;
		}else{
			labelToDisplay = TextManager.Instance.texts["SplashScreen"]["CONNECTING_FAIL"];
			posLength = 0;
			while(posLength < labelToDisplay.Length){
				posLength++;
				yield return new WaitForSeconds(speedLetters);
				
			}
			yield return new WaitForSeconds(1f);
			labelToDisplay = "";
			posLength = 0;
			ls = LOGIN_STATUT.INVALID;
		}
		
		
	
	}
	
	
	
}