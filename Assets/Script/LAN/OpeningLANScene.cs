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
		NONE
	}
	
	
	public GameObject ringJoin;
	public GameObject[] optionJoinCube;
	public GameObject ring;
	public GameObject[] optionCube;
	public Quaternion rotationBase;
	public float rotationSpeed;
	
	private GameObject ringSelected;
	
	public Transform cameraPos1;
	public Transform cameraPos2;
	
	private int optionJoinSelected;
	private int optionSelected;
	
	
	private FadeManager fm;
	
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
	private string roundValue;
	public Rect labelMaster;
	public Rect posBackMaster;
	public Rect posForwMaster;
	public Rect labelSongDiff;
	public Rect posBackSongDiff;
	public Rect posForwSongDiff;
	public Rect infoElimination;
	
	//Join entering
	public Rect posJoiningLabel;
	public Rect textFieldIP;
	public Rect posConnexionStatut;
	private string ipValue;
	
	private StateLAN stateLAN;
	
	//Join && Mode
	public float limitAlphaClign;
	public float speedAlphaCling;
	private float sens;
	private float alphaClign;
	private float alphaOption;
	public float speedAlphaOption;
	
	public float speedShining;
	public float speedPingPongRotation;
	public float maxDegreePingPong;
	
	public float speedTransitionTranslation;
	public float speedTransitionRotation;
	
	//Idem
	private float sensShininess;
	private float shininess;
	private GameObject previousGo;
	private Color baseMaterialColor;
	public Material selectedMaterial;
	private bool activeTransition;
	private bool activeTransitionBack;
	private float sensRotationCam;
	
	public Dictionary<string, Texture2D> tex;
	
	// Use this for initialization
	void Start () {
		TextManager.Instance.LoadTextFile();
		tex = new Dictionary<string, Texture2D>();
		tex.Add("join0", (Texture2D) Resources.Load("LANCreate"));
		tex.Add("join1", (Texture2D) Resources.Load("LANJoin"));
		tex.Add("option0", (Texture2D) Resources.Load("LANFFA"));
		tex.Add("option1", (Texture2D) Resources.Load("LANScoreTournament"));
		tex.Add("option2", (Texture2D) Resources.Load("LANPointTournament"));
		tex.Add("option3", (Texture2D) Resources.Load("LANElimination"));
		tex.Add("black", (Texture2D) Resources.Load("black"));
		
		sens = -1f;
		sensShininess = -1f;
		sensRotationCam = 1f;
		optionJoinSelected = -1;
		optionSelected = -1;
		alphaClign = 1f;
		stateLAN = StateLAN.JOINCHOOSE;
		ringSelected = ringJoin;
		alphaOption = 0f;
		roundValue = LANManager.Instance.roundNumber.ToString();
		fm = GetComponent<FadeManager>();
		ipValue = "";
		shininess = 0f;
		
		baseMaterialColor = selectedMaterial.color;
		rotationBase = ring.transform.rotation;
	}
	
	
	void OnGUI()
	{
		GUI.skin = skin;
		switch(stateLAN){
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
	
	void OnGUIChoose()
	{
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
			
			GUI.Label(new Rect(infoOption.x*Screen.width, infoOption.y*Screen.height, infoOption.width*Screen.width, infoOption.height*Screen.height), TextManager.Instance.texts["LAN"]["MENUJoinOption" + optionJoinSelected]);
		}
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Back") && !activeTransition && !activeTransitionBack){
			fm.FadeIn("mainmenu");
		}
		
		
	
	}
	
	void OnGUISelectMode()
	{
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
			
			GUI.Label(new Rect(infoOptionReverse.x*Screen.width, infoOptionReverse.y*Screen.height, infoOptionReverse.width*Screen.width, infoOptionReverse.height*Screen.height), TextManager.Instance.texts["LAN"]["MENUOption" + optionSelected]);
		
		}
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Back") && !activeTransition && !activeTransitionBack){
			activeTransitionBack = true;
		}
		
	}
	
	void OnGUIOptionChoose()
	{
		GUI.color = new Color(1f, 1f, 1f, 0.7f*alphaOption);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex["black"]);
		
		//Label Option Selected
		GUI.color = new Color(1f, 1f, 1f, 1f*alphaOption);
		GUI.DrawTexture(new Rect(posOptionSelected.x*Screen.width, posOptionSelected.y*Screen.height, posOptionSelected.width*Screen.width, posOptionSelected.height*Screen.height), tex["option" + optionSelected]);
		
		//Round
		GUI.Label(new Rect(labelRound.x*Screen.width, labelRound.y*Screen.height, labelRound.width*Screen.width, labelRound.height*Screen.height), TextManager.Instance.texts["LAN"]["OPTIONRound"]);
		
		roundValue = GUI.TextField(new Rect(textFieldRound.x*Screen.width, textFieldRound.y*Screen.height, textFieldRound.width*Screen.width, textFieldRound.height*Screen.height), roundValue.Trim(), 2);
		
		if(optionSelected == 3) //Special Elimination
		{
			GUI.Label(new Rect(infoRound.x*Screen.width, infoRound.y*Screen.height, infoRound.width*Screen.width, infoRound.height*Screen.height), TextManager.Instance.texts["LAN"]["INFOElimination"]);
		}
		
		//Host system
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
		
		GUI.Label(new Rect(labelMaster.x*Screen.width, labelMaster.y*Screen.height, labelMaster.width*Screen.width, labelMaster.height*Screen.height), TextManager.Instance.texts["LAN"]["OPTIONHost"+ LANManager.Instance.hostSystem]);
		
		
		//Song diff system
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
		
		GUI.Label(new Rect(labelSongDiff.x*Screen.width, labelSongDiff.y*Screen.height, labelSongDiff.width*Screen.width, labelSongDiff.height*Screen.height), TextManager.Instance.texts["LAN"]["OPTIONSongDiff"+ LANManager.Instance.songDiffSystem]);
					
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		if(GUI.Button(new Rect(posButtonConfirm.x*Screen.width, posButtonConfirm.y*Screen.height, posButtonConfirm.width*Screen.width, posButtonConfirm.height*Screen.height), "Confirm") && !activeTransition && !activeTransitionBack){
			fm.FadeIn("sdhfoisdfoisdf"); // !!!!
		}
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Back") && !activeTransition && !activeTransitionBack){
			stateLAN = StateLAN.MODECHOOSE;
		}
	}
	
	void OnGUIJoinEntering()
	{
		GUI.color = new Color(1f, 1f, 1f, 0.7f*alphaOption);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex["black"]);
		
		//Label Join
		GUI.color = new Color(1f, 1f, 1f, 1f*alphaOption);
		GUI.DrawTexture(new Rect(posOptionSelected.x*Screen.width, posOptionSelected.y*Screen.height, posOptionSelected.width*Screen.width, posOptionSelected.height*Screen.height), tex["join" + optionJoinSelected]);
		
		//Info join
		GUI.color = new Color(1f, 1f, 1f, 1f);
		GUI.Label(new Rect(posJoiningLabel.x*Screen.width, posJoiningLabel.y*Screen.height, posJoiningLabel.width*Screen.width, posJoiningLabel.height*Screen.height), TextManager.Instance.texts["LAN"]["INFOJoin"], "centered");
		Debug.Log(TextManager.Instance.texts["LAN"]["INFOJoin"]);
		ipValue = GUI.TextField(new Rect(textFieldIP.x*Screen.width, textFieldIP.y*Screen.height, textFieldIP.width*Screen.width, textFieldIP.height*Screen.height), ipValue.Trim(), 25);
					
		
		//Connexion statut
		GUI.Label(new Rect(posConnexionStatut.x*Screen.width, posConnexionStatut.y*Screen.height, posConnexionStatut.width*Screen.width, posConnexionStatut.height*Screen.height), "Connecting...", "centered"); //todo
		
		if(GUI.Button(new Rect(posButtonConfirm.x*Screen.width, posButtonConfirm.y*Screen.height, posButtonConfirm.width*Screen.width, posButtonConfirm.height*Screen.height), "Confirm") && !activeTransition && !activeTransitionBack){
			 // Joing room 
		}
		
		if(GUI.Button(new Rect(posButtonBack.x*Screen.width, posButtonBack.y*Screen.height, posButtonBack.width*Screen.width, posButtonBack.height*Screen.height), "Back") && !activeTransition && !activeTransitionBack){
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
				
		if(Physics.Raycast(ray, out hit))
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
						stateLAN = StateLAN.OPTIONCHOOSE;
					}
					
					
					
				}else
				{
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
			optionSelected = -1;
		}
		
		
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
		
		
		if(activeTransition)
		{
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraPos2.position, speedTransitionTranslation*Time.deltaTime);
			Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraPos2.rotation, speedTransitionRotation*Time.deltaTime);
			
			if(Vector3.Distance(Camera.main.transform.position, cameraPos2.position) <= 0.01f){
				Camera.main.transform.position = cameraPos2.position;
				Camera.main.transform.rotation = cameraPos2.rotation;
				activeTransition = false;
				stateLAN = StateLAN.MODECHOOSE;
				ringSelected = ring;
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
				ringSelected = ringJoin;
			}
		}
		
	}
	
}
