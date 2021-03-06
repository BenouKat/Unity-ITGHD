using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OpeningLANScene : MonoBehaviour {
	
	public enum StateLAN
	{
		JOINCHOOSE,
		MODECHOOSE,
		OPTIONCHOOSE,
		JOINENTERING,
		ERROR,
		NONE
	}
	
	public AudioSource audioS;
	
	public GameObject ringJoin;
	public GameObject[] optionJoinCube;
	public GameObject ring;
	public GameObject[] optionCube;
	public Quaternion rotationBase;
	public float rotationSpeed;
	
	
	public ParticleSystem psFlash;
	public GameObject cache;
	
	public Transform cameraPos1;
	public Transform cameraPos2;
	
	private int optionJoinSelected;
	private int optionSelected;
	private int finalSelected;
	
	private FadeManager fm;
	private float timeFade;
	private bool alreadyFaded;
	
	public GUISkin skin;
	
	//Join && Mode
	public Rect posTitleOption;
	public Rect posTitleOptionReverse;
	public float ratioNotSelected;
	public Rect posBack;
	public Rect posForw;
	public Rect posButtonConfirm;
	public Rect posButtonBack;
	public Rect infoOption;
	public Rect infoOptionReverse;
	
	//Select Option
	public Rect posOptionSelected;
	public Rect labelRound;
	public Rect infoRound;
	public Rect textFieldRound;
	public Rect labelInfoMaster;
	public Rect labelMaster;
	public Rect posBackMaster;
	public Rect posForwMaster;
	public Rect labelInfoSongDiff;
	public Rect labelSongDiff;
	public Rect posBackSongDiff;
	public Rect posForwSongDiff;
	public Rect errorInfo;
	public Rect cacheInfo;
	
	//Join entering
	public Rect posJoiningLabel;
	public Rect textFieldIP;
	private string ipValue;
	
	private StateLAN stateLAN;
	
	//Join && Mode
	public float limitAlphaClign;
	public float speedAlphaCling;
	private float sens;
	private float alphaClign;
	private float alphaOption;
	private float alphaTitle;
	private float alphaDisappearTitle;
	public float speedAlphaOption;
	public float speedAlphaTitle;
	public float speedAlphaDisappearTitle;
	
	public float speedShining;
	public float speedPingPongRotation;
	public float maxDegreePingPong;
	
	public float speedTransitionTranslation;
	public float speedTransitionRotation;
	
	public bool error;
	
	//Idem
	private float sensShininess;
	private float shininess;
	private GameObject previousGo;
	private Color baseMaterialColor;
	public Material selectedMaterial;
	private bool activeTransition;
	private bool activeTransitionBack;
	private float sensRotationCam;
	private bool activeRoomTransition;
	public Rect ErrorPos;
	public Rect ErrorButtonPos;
	
	public Dictionary<string, Texture2D> tex;
	
	void TestShort()
	{
		TextManager.Instance.LoadTextFile();
	}
	
	// Use this for initialization
	void Start () {
		//TestShort();
		tex = new Dictionary<string, Texture2D>();
		tex.Add("join0", (Texture2D) Resources.Load("LANCreate"));
		tex.Add("join1", (Texture2D) Resources.Load("LANJoin"));
		tex.Add("option0", (Texture2D) Resources.Load("LANFFA"));
		tex.Add("option1", (Texture2D) Resources.Load("LANScoreTournament"));
		tex.Add("option2", (Texture2D) Resources.Load("LANPointTournament"));
		tex.Add("option3", (Texture2D) Resources.Load("LANElimination"));
		tex.Add("black", (Texture2D) Resources.Load("black"));
		tex.Add("cache", (Texture2D) Resources.Load("CacheNameWheel"));
		
		sens = -1f;
		sensShininess = -1f;
		sensRotationCam = 1f;
		optionJoinSelected = -1;
		optionSelected = -1;
		finalSelected = -1;
		alphaClign = 1f;
		stateLAN = LANManager.Instance.rejectedByServer ? StateLAN.ERROR : StateLAN.JOINCHOOSE;
		LANManager.Instance.init();
		alphaOption = 0f;
		alphaTitle = 0f;
		alphaDisappearTitle = 1f;
		fm = GetComponent<FadeManager>();
		ipValue = "";
		shininess = 0f;
		error = false;
		timeFade = 0f;
		alreadyFaded = false;
		
		baseMaterialColor = selectedMaterial.color;
		rotationBase = ring.transform.rotation;
		
		audioS.Play();
		StartCoroutine(soundVolume(true));
	}
	
	IEnumerator soundVolume(bool up)
	{
		if(up)
		{
			while(audioS.volume < 1f)
			{
				audioS.volume += Time.deltaTime;	
				yield return 0;
			}
		}else{
			while(audioS.volume > 0f)
			{
				audioS.volume -= Time.deltaTime;	
				yield return 0;
			}
		}
		
	}
	
	
	void OnGUI()
	{
		GUI.skin = skin;
		GUI.depth = -1;
		switch(stateLAN){
		case StateLAN.ERROR:
			OnGUIError();
			break;
		case StateLAN.JOINCHOOSE:
			OnGUIChoose();
			break;
		case StateLAN.MODECHOOSE:
			OnGUISelectMode();
			break;
		case StateLAN.OPTIONCHOOSE:
			OnGUIOptionChoose();
			break;
		case StateLAN.JOINENTERING:
			OnGUIJoinEntering();
			break;
		}
	}
	
	void OnGUIError()
	{
		GUI.color = new Color(1f, 1f, 1f, 0.8f);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex["black"]);	
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		GUI.Label(new Rect(ErrorPos.x*Screen.width, ErrorPos.y*Screen.height, ErrorPos.width*Screen.width, ErrorPos.height*Screen.height), LANManager.Instance.errorToDisplay);
		if(GUI.Button(new Rect(ErrorButtonPos.x*Screen.width, ErrorButtonPos.y*Screen.height, ErrorButtonPos.width*Screen.width, ErrorButtonPos.height*Screen.height), "Close"))
		{
			LANManager.Instance.errorToDisplay = "";
			LANManager.Instance.rejectedByServer = false;
			stateLAN = StateLAN.JOINCHOOSE;
		}
	}
	
	void OnGUIChoose()
	{
		GUI.color = new Color(1f, 1f, 1f, 0.7f*alphaOption);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex["black"]);	
		
		if(!activeTransition && !activeTransitionBack && optionJoinSelected != -1){
			for(int i=0; i<optionJoinCube.Length; i++)
			{
				if(i == optionJoinSelected)
				{
					GUI.color = new Color(1f, 1f, 1f, alphaClign);
					GUI.DrawTexture(new Rect((posTitleOption.x*Screen.width), (posTitleOption.y*Screen.height), posTitleOption.width*Screen.width, posTitleOption.height*Screen.height), tex["join" + i]);
				}
				
			}
			
			
			
			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.DrawTexture(new Rect(cacheInfo.x*Screen.width, cacheInfo.y*Screen.height, cacheInfo.width*Screen.width, cacheInfo.height*Screen.height), tex["cache"]);
			GUI.Label(new Rect(infoOption.x*Screen.width, infoOption.y*Screen.height, infoOption.width*Screen.width, infoOption.height*Screen.height), TextManager.Instance.texts["LAN"]["MENUJoinOption" + optionJoinSelected]);
		}
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Back") && !activeTransition && !activeTransitionBack){
			fm.FadeIn("mainmenu");
		}
		
		
	
	}
	
	void OnGUISelectMode()
	{
		GUI.color = new Color(1f, 1f, 1f, 0.7f*alphaOption);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex["black"]);
		
		if(!activeTransition && !activeTransitionBack && optionSelected != -1){
			for(int i=0; i<optionCube.Length; i++)
			{
				if(i == optionSelected)
				{
					GUI.color = new Color(1f, 1f, 1f, alphaClign);
					GUI.DrawTexture(new Rect((posTitleOptionReverse.x*Screen.width), (posTitleOptionReverse.y*Screen.height), posTitleOptionReverse.width*Screen.width, posTitleOptionReverse.height*Screen.height), tex["option" + i]);
				}
				
			}
			
			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.DrawTexture(new Rect(cacheInfo.x*Screen.width, cacheInfo.y*Screen.height, cacheInfo.width*Screen.width, cacheInfo.height*Screen.height), tex["cache"]);
			GUI.Label(new Rect(infoOptionReverse.x*Screen.width, infoOptionReverse.y*Screen.height, infoOptionReverse.width*Screen.width, infoOptionReverse.height*Screen.height), TextManager.Instance.texts["LAN"]["MENUOption" + optionSelected]);
		
		}
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Back") && !activeTransition && !activeTransitionBack){
			activeTransitionBack = true;
		}
		
	}
	
	void OnGUIOptionChoose()
	{
		if(!cache.activeInHierarchy)
		{
			GUI.color = new Color(1f, 1f, 1f, (0.7f*alphaOption) + alphaTitle);
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex["black"]);
		}
		
		
		//Label Option Selected
		GUI.color = new Color(1f, 1f, 1f, (1f*alphaOption) + (alphaTitle*alphaDisappearTitle));
		GUI.DrawTexture(new Rect(posOptionSelected.x*Screen.width, posOptionSelected.y*Screen.height, posOptionSelected.width*Screen.width, posOptionSelected.height*Screen.height), tex["option" + finalSelected]);
		
		//Error
		if(error)
		{
			GUI.color = new Color(1f, 0.2f, 0.2f, 1f - alphaTitle);
			GUI.Label(new Rect(errorInfo.x*Screen.width, errorInfo.y*Screen.height, errorInfo.width*Screen.width, errorInfo.height*Screen.height), TextManager.Instance.texts["LAN"]["ERRORMode"]);
		}
		
		//Round
		GUI.color = new Color(1f, 1f, 1f, 1f - alphaTitle);
		/*if(finalSelected != 0) //Not in ffa
		{
			GUI.Label(new Rect(labelRound.x*Screen.width, labelRound.y*Screen.height, labelRound.width*Screen.width, labelRound.height*Screen.height), TextManager.Instance.texts["LAN"]["OPTIONRound"]);
			
			roundValue = GUI.TextField(new Rect(textFieldRound.x*Screen.width, textFieldRound.y*Screen.height, textFieldRound.width*Screen.width, textFieldRound.height*Screen.height), roundValue.Trim(), 2);
		}
		
		if(finalSelected == 3) //Special Elimination
		{
			GUI.Label(new Rect(infoRound.x*Screen.width, infoRound.y*Screen.height, infoRound.width*Screen.width, infoRound.height*Screen.height), TextManager.Instance.texts["LAN"]["INFOElimination"]);
		}*/
		
		//Host system
		GUI.Label(new Rect(labelInfoMaster.x*Screen.width, labelInfoMaster.y*Screen.height, labelInfoMaster.width*Screen.width, labelInfoMaster.height*Screen.height), TextManager.Instance.texts["LAN"]["OPTIONHost"]);
		if(GUI.Button(new Rect(posBackMaster.x*Screen.width, posBackMaster.y*Screen.height, posBackMaster.width*Screen.width, posBackMaster.height*Screen.height), "", "buttonBack")){
			if(LANManager.Instance.hostSystem > 0)
			{
				LANManager.Instance.hostSystem--;
			}else
			{
				LANManager.Instance.hostSystem = 2;
			}
		}
			
		if(GUI.Button(new Rect(posForwMaster.x*Screen.width, posForwMaster.y*Screen.height, posForwMaster.width*Screen.width, posForwMaster.height*Screen.height), "", "buttonForw")){
			if(LANManager.Instance.hostSystem < 2)
			{
				LANManager.Instance.hostSystem++;
			}else
			{
				LANManager.Instance.hostSystem = 0;
			}
		}
		
		GUI.Label(new Rect(labelMaster.x*Screen.width, labelMaster.y*Screen.height, labelMaster.width*Screen.width, labelMaster.height*Screen.height), TextManager.Instance.texts["LAN"]["OPTIONHost"+ LANManager.Instance.hostSystem], "centered");
		
		
		//Song diff system
		if(finalSelected != 0) //Not in ffa
		{
			GUI.Label(new Rect(labelInfoSongDiff.x*Screen.width, labelInfoSongDiff.y*Screen.height, labelInfoSongDiff.width*Screen.width, labelInfoSongDiff.height*Screen.height), TextManager.Instance.texts["LAN"]["OPTIONSongDiff"]);
			if(GUI.Button(new Rect(posBackSongDiff.x*Screen.width, posBackSongDiff.y*Screen.height, posBackSongDiff.width*Screen.width, posBackSongDiff.height*Screen.height), "", "buttonBack")){
				if(LANManager.Instance.songDiffSystem > 0)
				{
					LANManager.Instance.songDiffSystem--;
				}else
				{
					LANManager.Instance.songDiffSystem = 2;
				}
			}
				
			if(GUI.Button(new Rect(posForwSongDiff.x*Screen.width, posForwSongDiff.y*Screen.height, posForwSongDiff.width*Screen.width, posForwSongDiff.height*Screen.height), "", "buttonForw")){
				if(LANManager.Instance.songDiffSystem < 2)
				{
					LANManager.Instance.songDiffSystem++;
				}else
				{
					LANManager.Instance.songDiffSystem = 0;
				}
			}
			
			GUI.Label(new Rect(labelSongDiff.x*Screen.width, labelSongDiff.y*Screen.height, labelSongDiff.width*Screen.width, labelSongDiff.height*Screen.height), TextManager.Instance.texts["LAN"]["OPTIONSongDiff"+ LANManager.Instance.songDiffSystem], "centered");
		}	
		GUI.color = new Color(1f, 1f, 1f, 1f - alphaTitle);
		if(GUI.Button(new Rect(posButtonConfirm.x*Screen.width, posButtonConfirm.y*Screen.height, posButtonConfirm.width*Screen.width, posButtonConfirm.height*Screen.height), "Confirm") && !activeTransition && !activeTransitionBack && !activeRoomTransition && isRoundNumberValid()){
			activeRoomTransition = true;
			StartCoroutine(roomTransition());
		}
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Back") && !activeTransition && !activeTransitionBack && !activeRoomTransition){
			stateLAN = StateLAN.MODECHOOSE;
		}
	}
	
	void OnGUIJoinEntering()
	{
		if(!cache.activeInHierarchy)
		{
			GUI.color = new Color(1f, 1f, 1f, (0.7f*alphaOption) + alphaTitle);
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex["black"]);
		}
		
		//Label Join
		GUI.color = new Color(1f, 1f, 1f, (1f*alphaOption) + (alphaTitle*alphaDisappearTitle));
		GUI.DrawTexture(new Rect(posOptionSelected.x*Screen.width, posOptionSelected.y*Screen.height, posOptionSelected.width*Screen.width, posOptionSelected.height*Screen.height), tex["join" + finalSelected]);
		
		//Error
		if(error)
		{
			GUI.color = new Color(1f, 0.2f, 0.2f, 1f - alphaTitle);
			GUI.Label(new Rect(errorInfo.x*Screen.width, errorInfo.y*Screen.height, errorInfo.width*Screen.width, errorInfo.height*Screen.height), TextManager.Instance.texts["LAN"]["ERRORJoin"]);
		}
		
		//Info join
		GUI.color = new Color(1f, 1f, 1f, 1f - alphaTitle);
		GUI.Label(new Rect(posJoiningLabel.x*Screen.width, posJoiningLabel.y*Screen.height, posJoiningLabel.width*Screen.width, posJoiningLabel.height*Screen.height), TextManager.Instance.texts["LAN"]["INFOJoin"], "centered");
		
		ipValue = GUI.TextField(new Rect(textFieldIP.x*Screen.width, textFieldIP.y*Screen.height, textFieldIP.width*Screen.width, textFieldIP.height*Screen.height), ipValue.Trim(), 25);
					
		if(GUI.Button(new Rect(posButtonConfirm.x*Screen.width, posButtonConfirm.y*Screen.height, posButtonConfirm.width*Screen.width, posButtonConfirm.height*Screen.height), "Confirm") && !activeTransition && !activeTransitionBack && !activeRoomTransition && isEntryJoinValid()){
			activeRoomTransition = true;
			StartCoroutine(roomTransition()); 
		}
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Back") && !activeTransition && !activeTransitionBack && !activeRoomTransition){
			stateLAN = StateLAN.JOINCHOOSE;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Clign
		if(sens <= 0 && alphaClign < limitAlphaClign || sens >= 0 && alphaClign > 1f)
		{
			sens *= -1f;
		}
		alphaClign += sens*speedAlphaCling;
		
		
		Camera.main.transform.Rotate(0f, 0f, speedPingPongRotation*Time.deltaTime*sensRotationCam);
		if(Camera.main.transform.eulerAngles.z >= 2 && sensRotationCam == 1f)
		{
			sensRotationCam = -1f;
		}else if(Camera.main.transform.eulerAngles.z <= 0f && sensRotationCam == -1f)
		{
			sensRotationCam = 1f;	
		}
		
		//Raycast
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
		RaycastHit hit;
				
		if(Physics.Raycast(ray, out hit) && stateLAN != StateLAN.ERROR)
		{
			var theGo = hit.transform.gameObject;
			if(theGo != null && theGo.tag == "MenuItem")
			{
				if(previousGo != null && previousGo != theGo)
				{
					previousGo.renderer.material.color = baseMaterialColor;
					shininess = 0f;
				}
				previousGo = theGo;
				if(stateLAN == StateLAN.JOINCHOOSE)
				{
					optionJoinSelected = System.Convert.ToInt32(theGo.name);	
					shininess += sensShininess*speedShining*Time.deltaTime;
					theGo.renderer.material.color = new Color(baseMaterialColor.r + ((1f-baseMaterialColor.r)*shininess),
						baseMaterialColor.g + ((1f-baseMaterialColor.g)*shininess), 
						baseMaterialColor.b + ((1f-baseMaterialColor.b)*shininess), 1f);
					
					if(Input.GetMouseButtonDown(0))
					{
						finalSelected = optionJoinSelected;
						switch(optionJoinSelected)
						{
							case 0:
								activeTransition = true;
								break;
							case 1:
								stateLAN = StateLAN.JOINENTERING;
								break;
						}
					}
					
					
				}else if(stateLAN == StateLAN.MODECHOOSE){
					optionSelected = System.Convert.ToInt32(theGo.name);	
					
					shininess += sensShininess*speedShining*Time.deltaTime;
					theGo.renderer.material.color = new Color(baseMaterialColor.r + ((1f-baseMaterialColor.r)*shininess),
						baseMaterialColor.g + ((1f-baseMaterialColor.g)*shininess), 
						baseMaterialColor.b + ((1f-baseMaterialColor.b)*shininess), 1f);
					
					if(Input.GetMouseButtonDown(0))
					{
						finalSelected = optionSelected;
						stateLAN = StateLAN.OPTIONCHOOSE;
					}
					
					
					
				}else
				{
					optionJoinSelected = -1;
					optionSelected = -1;
				}
				
				
				if(shininess <= 0f)
				{
					shininess = 0f;
					sensShininess = 1f;
				}else if(shininess >= 1f)
				{
					shininess = 1f;
					sensShininess = -1f;
				}
				
				
			}else
			{
				if(previousGo != null)
				{
					previousGo.renderer.material.color = baseMaterialColor;
					previousGo = null;
					shininess = 0f;
				}
				optionJoinSelected = -1;
				optionSelected = -1;
			}
		}else
		{	
			if(previousGo != null)
			{
				previousGo.renderer.material.color = baseMaterialColor;
				previousGo = null;
				shininess = 0f;
			}
			optionJoinSelected = -1;
			optionSelected = -1;
		}
		
		if(!activeRoomTransition)
		{
			if(stateLAN >= StateLAN.OPTIONCHOOSE && alphaOption < 1f)
			{
				alphaOption += Time.deltaTime*speedAlphaOption;
				if(alphaOption > 1f)
				{
					alphaOption = 1f;
				}
			}else if(stateLAN < StateLAN.OPTIONCHOOSE && alphaOption > 0f)
			{
				alphaOption -= Time.deltaTime*speedAlphaOption;
				if(alphaOption < 0f)
				{
					alphaOption = 0f;
				}
			}
		}
		
		
		if(activeTransition)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPos2.position, speedTransitionTranslation*Time.deltaTime);
			Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraPos2.rotation, speedTransitionRotation*Time.deltaTime);
			
			if(Vector3.Distance(Camera.main.transform.position, cameraPos2.position) <= 0.01f){
				Camera.main.transform.position = cameraPos2.position;
				Camera.main.transform.rotation = cameraPos2.rotation;
				activeTransition = false;
				stateLAN = StateLAN.MODECHOOSE;
			}
		}
		
		if(activeTransitionBack)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPos1.position, speedTransitionTranslation*Time.deltaTime);
			Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraPos1.rotation, speedTransitionRotation*Time.deltaTime);
			
			if(Vector3.Distance(Camera.main.transform.position, cameraPos1.position) <= 0.01f){
				Camera.main.transform.position = cameraPos1.position;
				Camera.main.transform.rotation = cameraPos1.rotation;
				activeTransitionBack = false;
				stateLAN = StateLAN.JOINCHOOSE;
			}
		}
		
		
		if(!alreadyFaded && timeFade > 0.25f){
			GetComponent<FadeManager>().FadeOut();
			alreadyFaded = true;
		}else{
			timeFade += Time.deltaTime;
		}
		
	}
	
	public IEnumerator roomTransition()
	{
		psFlash.gameObject.SetActive(true);
		cache.SetActive(true);
		StartCoroutine(soundVolume(false));
		while(alphaTitle < 1f)
		{
			alphaTitle += speedAlphaTitle*Time.deltaTime;
			alphaOption -= speedAlphaTitle*Time.deltaTime;
			cache.renderer.material.color = new Color(0f, 0f, 0f, (0.7f*alphaOption) + alphaTitle);
			yield return new WaitForEndOfFrame();	
		}
		
		yield return new WaitForSeconds(1.5f);
		
		while(alphaDisappearTitle > 0f)
		{
			alphaDisappearTitle -= speedAlphaDisappearTitle*Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		
		if(stateLAN == StateLAN.JOINENTERING)
		{
			LANManager.Instance.isCreator = false;
			LANManager.Instance.IPRequest = ipValue.Split(':')[0];
			LANManager.Instance.portRequest = System.Convert.ToInt32(ipValue.Split(':')[1]);
		}else
		{
			LANManager.Instance.isCreator = true;
			//LANManager.Instance.roundNumber = System.Convert.ToInt32(roundValue);
			LANManager.Instance.modeLANselected = (LANMode)finalSelected;
			if(finalSelected == 0) //ffa
			{
				LANManager.Instance.songDiffSystem = 1;
			}
		}
		
		Application.LoadLevel("LANSelection");
	}
	
	
	public bool isEntryJoinValid()
	{
		var result = 0;
		if(ipValue.Split(':')[0].Length < 8 && ipValue.Split(':')[0].Count(c => c == '.') != 3)
		{
			error = true;
			return false;
		}else if(!System.Int32.TryParse(ipValue.Split(':')[1], out result)){
			error = true;
			return false;
		}
		error = false;
		return true;
	}
	
	public bool isRoundNumberValid()
	{
		return true;
		/*
		var result = 0;
		if(finalSelected == 0) //ffa
		{
			error = false;
			return true;
		}
		if(System.Int32.TryParse(roundValue, out result))
		{
			error = !(result > 0 && result < 100);
			return !error;
		}
		error = true;
		return false;
		*/
	}
	
}
