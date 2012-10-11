using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class InGameScript : MonoBehaviour {
	
	#region Declarations
	//Game object song scene
	private GameObject arrow;
	public List<GameObject> typeArrow;
	public GameObject freeze;
	public GameObject mines;
	public Camera MainCamera;
	private Transform TMainCamera;
	public GameObject Background;
	public GameObject CameraBackground;
	private GameObject arrowLeft;
	private GameObject arrowRight;
	private GameObject arrowDown;
	private GameObject arrowUp;
	private Transform[] arrowTarget;
	public Material[] matSkinModel;
	private Material matArrowModel;
	public GameObject lifeBar;
	public GameObject progressBar;
	public GameObject progressBarEmpty;
	public GameObject slow;
	public GameObject fast;
	private Material matProgressBar;
	private Material matProgressBarFull;
	public Camera particleComboCam;
	
	private LifeBar theLifeBar;
	
	private Song thesong;
	
	//USED FOR UPDATE FUNCTION
	private double timebpm; //Temps joué sur la totalité, non live, non remise à 0
	private float timechart; //Temps joué sur le bpm actuel en live avec remise à 0
	private float timestop; //Temps utilisé pour le freeze
	private float totaltimestop; //Temps total à être stoppé
	private double timetotalchart; //Temps joué sur la totalité en live (timebpm + timechart)
	private float changeBPM; //Position en y depuis un changement de bpm (nouvelle reference)
	
	private int nextSwitchBPM; //Prochain changement de bpm
	private int nextSwitchStop;
	
	private double actualBPM; //Bpm actuel
	
	private double actualstop;
	private float speedmod; //speedmod 
	public float speedmodRate = 2f; //speedmod ajustation 
	
	//Temps pour le lachement de freeze
	public float unfrozed = 0.350f;
	
	private KeyCode KeyCodeUp;
	private KeyCode KeyCodeDown;
	private KeyCode KeyCodeLeft;
	private KeyCode KeyCodeRight;
	private KeyCode SecondaryKeyCodeUp;
	private KeyCode SecondaryKeyCodeDown;
	private KeyCode SecondaryKeyCodeLeft;
	private KeyCode SecondaryKeyCodeRight;
	
	//DISPLAY PREC
	
	//Temps pour l'affichage des scores
	public float limitDisplayScore = 2f;
	private float timeDisplayScore = 0f;
	public float timeClignotementFantastic = 0.05f;
	public float baseZoom = 20f;
	public float vitesseZoom = 0.1f;
	private bool sensFantastic;
	private float alpha;
	private float zoom;
	private Precision scoreToDisplay;
	public Rect posScore = new Rect(0.38f,0.3f,0.25f,0.25f);
	private Dictionary<Precision, int> countScore;
	
	//SLOWFAST
	public float ClignSpeed;
	public float stateSpeed; //-1 : fast, 0 : rien, 1 : slow
	private bool changeColorSlow = false;
	private bool changeColorFast = false;
	
	//LIFE
	private float life;
	private Dictionary<string, float> lifeBase;
	
	
	//SCORE
	private float score;
	private float scoreInverse;
	private float targetScoreInverse;
	private float fantasticValue;
	private Dictionary<string, float> scoreBase;
	private Dictionary<string, int> scoreCount;
	public Rect posPercent = new Rect(0.39f, 0f, 0.45f, 0.05f);
	
	//PassToDataManager
	private List<double> precAverage;
	private Dictionary<double, int> timeCombo;
	private Dictionary<double, double> lifeGraph;
	public double firstEx;
	public double firstGreat;
	public double firstMisteak;
	
	//FPS
	private long _count;
	private long fps;
	private float _timer;
	
	//CHART
	private List<Arrow> arrowLeftList;
	private List<Arrow> arrowRightList;
	private List<Arrow> arrowDownList;
	private List<Arrow> arrowUpList;
	private List<Arrow> mineLeftList;
	private List<Arrow> mineRightList;
	private List<Arrow> mineDownList;
	private List<Arrow> mineUpList;
	//Dico des arrow prises en freeze
	private Dictionary<Arrow, float> arrowFrozen;
	
	//PARTICLE SYSTEM
	
	private Dictionary<string, ParticleSystem> precLeft;
	private Dictionary<string, ParticleSystem> precRight;
	private Dictionary<string, ParticleSystem> precUp;
	private Dictionary<string, ParticleSystem> precDown;
	private Dictionary<string, ParticleSystem> clearcombo;
	
	
	
	//GUI
	private Dictionary<string, Texture2D> TextureBase;
	private float wd;
	private float hg;
	private float hgt;
	public float ecart;
	public float ecartCombo;
	private int[] displaying; //score decoup
	private int[] thetab; //combo decoup
	public GUISkin skin;
	public Rect infoSong;
	public Rect SpeedModTextAera;
	public Rect SpeedModText;
	public Rect SpeedModTextInfo;
	private string speedmodstring;
	private bool speedmodok;
	
	//DISPLAY
	private Color bumpColor;
	public float bumpfadeSpeed = 0.5f;
	public bool[] displayValue;
	
	//START
	private bool firstUpdate;
	private float oneSecond;
	private float startTheSong; //Time pour démarrer la chanson
	private bool scenechartfaded;
	
	//BUMP
	private int nextBump;
	private List<double> Bumps;
	public float speedBumps;
	
	
	//Input Bump
	public float speedBumpInput;
	private float scaleBase;
	public float percentScaleInput;
	
	//PROGRESSBAR
	private double firstArrow;
	private double lastArrow;
	
	//COMBO
	private int combo;
	public Rect posCombo = new Rect(0.40f, 0.5f, 0.45f, 0.05f);
	public Rect posdispCombo = new Rect(0.40f, 0.5f, 0.45f, 0.05f);
	private ComboType ct;
	public float speedCombofade;
	private float colorCombo;
	private int signCombo = -1;
	private bool alreadytaged = true;
	private int comboMisses;
	private float alphaCombo;
	public float speedAlphaCombo;
	
	//FAIL OR CLEAR
	private bool fail;
	private bool dead;
	private bool clear;
	private bool fullCombo;
	private bool fullExCombo;
	private bool fullFantCombo;
	private bool perfect;
	private bool isFullComboRace;
	private bool isFullExComboRace;
	private int typeOfDeath; // 0 : Immediatly, 1 : After 30 misses, 2 : Never
	public float timeFailAppear;
	private float timeFailDisappear;
	public float timeClearDisappear;
	private float zoomfail;
	public float speedzoom;
	public float speedziwp;
	private float zwip;
	public Rect posFail;
	public Rect posClear;
	public Rect posRetry;
	public Rect posGiveUp;
	private bool appearFailok;
	private bool disappearFailok;
	public float speedAlphaFailFade;
	private float failalpha;
	private float buttonfailalpha;
	public float speedbuttonfailalpha;
	private bool cacheFailed;
	public float speedFadeAudio;
	private float passalpha;
	public Rect fullComboPos;
	private bool deadAndRetry;
	private bool deadAndGiveUp;
	private float timeGiveUp;
	
	//SONG
	private AudioClip songLoaded;
	//DEBUG
	//private int iwashere;
	#endregion
	
	
	//Start
	void Start () {
		
		//Data from option
		speedmod = DataManager.Instance.speedmodSelected*speedmodRate;
		DataManager.Instance.LoadScoreJudge(DataManager.Instance.scoreJudgeSelected);
		DataManager.Instance.LoadHitJudge(DataManager.Instance.hitJudgeSelected);
		DataManager.Instance.LoadLifeJudge(DataManager.Instance.lifeJudgeSelected);
		isFullComboRace = DataManager.Instance.raceSelected == 9;
		isFullExComboRace = DataManager.Instance.raceSelected == 10;
		targetScoreInverse = DataManager.Instance.giveTargetScoreOfRace(DataManager.Instance.raceSelected);
		typeOfDeath = DataManager.Instance.deathSelected;
		KeyCodeDown = DataManager.Instance.KeyCodeDown;
		KeyCodeUp = DataManager.Instance.KeyCodeUp;
		KeyCodeLeft = DataManager.Instance.KeyCodeLeft;
		KeyCodeRight = DataManager.Instance.KeyCodeRight;
		SecondaryKeyCodeDown = DataManager.Instance.SecondaryKeyCodeDown;
		SecondaryKeyCodeUp = DataManager.Instance.SecondaryKeyCodeUp;
		SecondaryKeyCodeLeft = DataManager.Instance.SecondaryKeyCodeLeft;
		SecondaryKeyCodeRight = DataManager.Instance.SecondaryKeyCodeRight;
		speedmodstring = DataManager.Instance.speedmodSelected.ToString("0.00");
		speedmodok = true;
		var rand = (int)(UnityEngine.Random.value*DataManager.Instance.skyboxList.Count);
		if(rand == DataManager.Instance.skyboxList.Count){
			rand--;	
		}
		
		//Arrows
		for(int i=0;i<4;i++){
			if(i != DataManager.Instance.skinSelected){
				var go = GameObject.Find("ArrowSkin" + i);
				Destroy(go);
				var go2 = GameObject.Find("ArrowModelSkin" + i);
				Destroy(go2);
			}
		}
		arrow = typeArrow.ElementAt(DataManager.Instance.skinSelected);
		var modelskin = GameObject.Find("ArrowModelSkin" + DataManager.Instance.skinSelected);
		modelskin.SetActiveRecursively(false);
		arrowLeft = (GameObject) Instantiate(modelskin, new Vector3(0f, 0f, 2f), modelskin.transform.rotation);
		if(DataManager.Instance.skinSelected == 3){
			arrowLeft.transform.FindChild("RotationCenter").Rotate(0f, 0f, 90f);
		}else if(DataManager.Instance.skinSelected != 0){
			arrowLeft.transform.Rotate(0f, 0f, 90f);
			arrowLeft.transform.FindChild("ParticulePrec").Rotate(0f, 0f, -90f);
			arrowLeft.transform.FindChild("ParticulePrec").localPosition = new Vector3(-1f, 0f, 0f);
		}
		
		arrowLeft.transform.parent = MainCamera.gameObject.transform;
		arrowRight = (GameObject) Instantiate(modelskin, new Vector3(6f, 0f, 2f), modelskin.transform.rotation);
		if(DataManager.Instance.skinSelected == 3){
			arrowRight.transform.FindChild("RotationCenter").Rotate(0f, 0f, -90f);
		}else if(DataManager.Instance.skinSelected == 1){
			arrowRight.transform.Rotate(0f, 0f, -90f);
			arrowRight.transform.FindChild("ParticulePrec").Rotate(0f, 0f, 90f);
			arrowRight.transform.FindChild("ParticulePrec").localPosition = new Vector3(1f, 0f, 0f);
		}
		arrowRight.transform.parent = MainCamera.gameObject.transform;
		arrowDown = (GameObject) Instantiate(modelskin, new Vector3(2f, 0f, 2f), modelskin.transform.rotation);
		if(DataManager.Instance.skinSelected == 3){
			arrowDown.transform.FindChild("RotationCenter").Rotate(0f, 0f, 180f);
		}else if(DataManager.Instance.skinSelected == 1){
			arrowDown.transform.Rotate(0f, 0f, 180f);
			arrowDown.transform.FindChild("ParticulePrec").Rotate(0f, 0f, -180f);
			arrowDown.transform.FindChild("ParticulePrec").localPosition = new Vector3(0f, 1f, 0f);
		}
		arrowDown.transform.parent = MainCamera.gameObject.transform;
		arrowUp = (GameObject) Instantiate(modelskin, new Vector3(4f, 0f, 2f), modelskin.transform.rotation);
		arrowUp.transform.parent = MainCamera.gameObject.transform;
		
		arrowTarget = new Transform[4];
		arrowTarget[0] = arrowLeft.transform;
		arrowTarget[1] = arrowDown.transform;
		arrowTarget[2] = arrowUp.transform;
		arrowTarget[3] = arrowRight.transform;
		scaleBase = arrowTarget[0].localScale.x;
		matArrowModel = matSkinModel[DataManager.Instance.skinSelected];

		
		
		RenderSettings.skybox = DataManager.Instance.skyboxList.ElementAt(rand);
		DataManager.Instance.skyboxIndexSelected = rand;
		displayValue = DataManager.Instance.displaySelected;
		
		firstArrow = -10f;
		lastArrow = -10f;
		thesong = DataManager.Instance.songSelected;
		songLoaded = thesong.GetAudioClip();
		audio.loop = false;
		createTheChart(thesong);
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 0;
		nextSwitchBPM = 1;
		nextSwitchStop = 0;
		actualBPM = thesong.bpms.First().Value;
		actualstop = (double)0;
		changeBPM = 0;
		
		_count = 0L;
		
		timebpm = (double)0;
		timechart = 0f;//-(float)thesong.offset;
		timetotalchart = (double)0;
		
		
		arrowFrozen = new Dictionary<Arrow, float>();
		
		precRight = new Dictionary<string, ParticleSystem>();
		precLeft = new Dictionary<string, ParticleSystem>();
		precUp = new Dictionary<string, ParticleSystem>();
		precDown = new Dictionary<string, ParticleSystem>();
		clearcombo = new Dictionary<string, ParticleSystem>();
		
		//Prepare the scene
		foreach(var el in Enum.GetValues(typeof(PrecParticle))){
			precLeft.Add( el.ToString(), (ParticleSystem) arrowLeft.transform.FindChild("ParticulePrec").gameObject.transform.FindChild(el.ToString()).particleSystem );
			precDown.Add( el.ToString(), (ParticleSystem) arrowDown.transform.FindChild("ParticulePrec").gameObject.transform.FindChild(el.ToString()).particleSystem );
			precRight.Add( el.ToString(), (ParticleSystem) arrowRight.transform.FindChild("ParticulePrec").gameObject.transform.FindChild(el.ToString()).particleSystem );
			precUp.Add( el.ToString(), (ParticleSystem) arrowUp.transform.FindChild("ParticulePrec").gameObject.transform.FindChild(el.ToString()).particleSystem );
		}
		for(int i=0; i< particleComboCam.transform.GetChildCount(); i++){
			clearcombo.Add(particleComboCam.transform.GetChild(i).name, particleComboCam.transform.GetChild(i).particleSystem);
		}
		
		precAverage = new List<double>();
		timeCombo = new Dictionary<double, int>();
		lifeGraph = new Dictionary<double, double>();
		
		TMainCamera = MainCamera.transform;
		MoveCameraBefore();
		
		//Textures
		TextureBase = new Dictionary<string, Texture2D>();
		TextureBase.Add("FANTASTIC", (Texture2D) Resources.Load("Fantastic"));
		TextureBase.Add("EXCELLENT", (Texture2D) Resources.Load("Excellent"));
		TextureBase.Add("GREAT", (Texture2D) Resources.Load("Great"));
		TextureBase.Add("DECENT", (Texture2D) Resources.Load("Decent"));
		TextureBase.Add("WAYOFF", (Texture2D) Resources.Load("Wayoff"));
		TextureBase.Add("MISS", (Texture2D) Resources.Load("Miss"));
		for(int i=0; i<10; i++){
			TextureBase.Add("S" + i, (Texture2D) Resources.Load("Numbers/S" + i));
			TextureBase.Add("C" + i, (Texture2D) Resources.Load("Numbers/C" + i));
		}
		
		TextureBase.Add("PERCENT", (Texture2D) Resources.Load("Numbers/Percent"));
		TextureBase.Add("DOT", (Texture2D) Resources.Load("Numbers/Dot"));
		TextureBase.Add("COMBODISPLAY", (Texture2D) Resources.Load("DisplayCombo"));
		TextureBase.Add("BLACK", (Texture2D) Resources.Load("black"));
		TextureBase.Add("FAIL", (Texture2D) Resources.Load("Fail"));
		TextureBase.Add("CLEAR", (Texture2D) Resources.Load("Clear"));
		TextureBase.Add("FC", (Texture2D) Resources.Load("FC"));
		TextureBase.Add("FEC", (Texture2D) Resources.Load("FEC"));
		TextureBase.Add("FFC", (Texture2D) Resources.Load("FFC"));
		TextureBase.Add("PERFECT", (Texture2D) Resources.Load("Perfect"));
		
		//stuff
		scoreToDisplay = Precision.NONE;
		timeDisplayScore = Mathf.Infinity;
		sensFantastic = true;
		alpha = 1f;
		scenechartfaded = false;
		
		
		
		//init score and lifebase
		scoreBase = new Dictionary<string, float>();
		scoreCount = new Dictionary<string, int>();
		lifeBase = new Dictionary<string, float>();
		fantasticValue = 100f/(thesong.numberOfStepsWithoutJumps + thesong.numberOfJumps + thesong.numberOfFreezes + thesong.numberOfRolls);
		foreach(Precision el in Enum.GetValues(typeof(Precision))){
			if(el != Precision.NONE){
				scoreBase.Add(el.ToString(), fantasticValue*DataManager.Instance.ScoreWeightValues[el.ToString()]);
				lifeBase.Add(el.ToString(), DataManager.Instance.LifeWeightValues[el.ToString()]);
				
			}
		}
		
		foreach(ScoreCount el2 in Enum.GetValues(typeof(ScoreCount))){
			if(el2 != ScoreCount.NONE){
				scoreCount.Add(el2.ToString(), 0);
			}
		}
		
		
		
		life = 50f;
		lifeGraph.Add(0, life);
		score = 0f;
		scoreInverse = 100f;
		firstEx = -1;
		firstGreat = -1;
		firstMisteak = -1;
		
		theLifeBar = lifeBar.GetComponent<LifeBar>();
		
		
		//var bps = thesong.getBPS(actualBPM);
		//changeBPM -= (float)(bps*thesong.offset)*speedmod;
		firstUpdate = true;
		oneSecond = 0f;
		startTheSong = (float)thesong.offset + DataManager.Instance.globalOffsetSeconds;
		
		
		//bump
		nextBump = 0;
		
		//combo
		ct = ComboType.FULLFANTASTIC;
		matProgressBar = progressBarEmpty.renderer.material;
		matProgressBarFull = progressBar.renderer.material;
		colorCombo = 1f;
		comboMisses = 0;
		alphaCombo = 0.5f;
		//GUI
		/*wd = posPercent.width*128;
		hg = posPercent.height*1024;
		hgt = posPercent.height*254;*/
		//ecart = 92f;
		
		displaying = scoreDecoupe();
		thetab = comboDecoupe();
		
		
		//Fail and clear
		fail = false;
		clear = false;
		fullCombo = false;
		fullExCombo = false;
		fullFantCombo = false;
		perfect = false;
		dead = false;
		zwip = 0;
		appearFailok = false;
		disappearFailok = false;
		zoomfail = 0f;
		failalpha = 0f;
		passalpha = 0f;
		cacheFailed = true;
		timeGiveUp = 0f;
		
		
		//Transformation display
		if(displayValue[4]){ //No judge
			limitDisplayScore = -1f;
		}
		
		if(displayValue[5]){ //No background
			RenderSettings.skybox = null;
			Background.GetComponent<MoveBackground>().enabled = false;
			Background.SetActiveRecursively(false);
			
		}
		
		if(displayValue[6]){ //No target
			arrowLeft.renderer.enabled = false;
			arrowDown.renderer.enabled = false;
			arrowUp.renderer.enabled = false;
			arrowRight.renderer.enabled = false;
		}
		
		//No score : inside the code
		
		//No UI
		if(displayValue[8]){
			for(int i=0;i<lifeBar.transform.childCount; i++){
				lifeBar.transform.GetChild(i).renderer.enabled = false;	
			}
			progressBar.renderer.enabled = false;
			progressBarEmpty.renderer.enabled = false;
			slow.renderer.enabled = false;
			fast.renderer.enabled = false;
		}
	}
	
	
	
	//only for FPS
	void OnGUI(){
		
		GUI.skin = skin;
		
		//fake stuff
		GUI.Label(new Rect(0.9f*Screen.width, 0.05f*Screen.height, 200f, 200f), fps.ToString());		
		//end fake stuff
		GUI.color = new Color(0f, 0f, 0f, 1f);
		GUI.Label(new Rect(infoSong.x*Screen.width + 1, infoSong.y*Screen.height + 1, infoSong.width*Screen.width, infoSong.height*Screen.height), thesong.title + "\n" + thesong.artist + "\n" + thesong.stepartist);
		GUI.color = new Color(1f, 1f, 1f, 1f);
		GUI.Label(new Rect(infoSong.x*Screen.width, infoSong.y*Screen.height, infoSong.width*Screen.width, infoSong.height*Screen.height), thesong.title + "\n" + thesong.artist + "\n" + thesong.stepartist);
		
		if(timeDisplayScore < limitDisplayScore && !clear){

			GUI.color = new Color(1f, 1f, 1f, alpha);
			GUI.DrawTexture(new Rect(posScore.x*Screen.width - zoom, posScore.y*Screen.height, posScore.width*Screen.width + zoom*2, posScore.height*Screen.height), TextureBase[scoreToDisplay.ToString()]); 
		}
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		if(!displayValue[7]){
				
			for(int i=0;i<5;i++){
				if((i == 3 && displaying[3] == 0 && displaying[4] == 0) || (i == 4 && displaying[4] == 0)) break;
				GUI.DrawTexture(new Rect((posPercent.x + ecart*(4-i))*Screen.width, posPercent.y*Screen.height,  posPercent.width*Screen.width,  posPercent.height*Screen.height), TextureBase["S" + displaying[i]]);
				
			}
			GUI.DrawTexture(new Rect((posPercent.x + ((ecart*2)+(ecart/2f)))*Screen.width, posPercent.y*Screen.height,  posPercent.width*Screen.width, posPercent.height*Screen.height), TextureBase["DOT"]);
			GUI.DrawTexture(new Rect((posPercent.x + ecart*5)*Screen.width + posPercent.width, posPercent.y*Screen.height, posPercent.width*Screen.width, posPercent.height*Screen.height), TextureBase["PERCENT"]);
		}
		
		if(!displayValue[8]){
			if(combo >= 5f){
				//var czoom = zoom/4f;
				var col = matProgressBar.color;
				GUI.color = new Color(col.r , col.g, col.b, alphaCombo);
				for(int i=0; i<thetab.Length; i++){
					GUI.DrawTexture(new Rect((posCombo.x + ((ecartCombo*(thetab.Length-(i+1))/2f) -ecartCombo*((float)i/2f)))*Screen.width, 
					posCombo.y*Screen.height, posCombo.width*Screen.width, posCombo.height*Screen.height), TextureBase["C" + thetab[i]]);
				}
			}
		}
		
		
		
		if(dead){
			if(oneSecond > timeFailAppear){
				
				GUI.color = new Color(1f, 1f, 1f, failalpha);
				GUI.DrawTexture(new Rect(0f,0f, Screen.width*1.2f, Screen.height*1.2f), TextureBase["BLACK"]);
				var ratiow = (float)posFail.width/(float)Mathf.Max (posFail.width, posFail.height);
				var ratioh = (float)posFail.height/(float)Mathf.Max (posFail.width, posFail.height);
				GUI.color = new Color(1f, 1f, 1f, failalpha*alpha);
				if(!cacheFailed) GUI.DrawTexture(new Rect((posFail.x - zwip - ratiow*zoomfail/2f)*Screen.width, (posFail.y + zwip - ratioh*zoomfail/2f)*Screen.height ,
					(posFail.width + zwip*2 + ratiow*zoomfail)*Screen.width, (posFail.height - zwip*2 + ratioh*zoomfail)*Screen.height), TextureBase["FAIL"]);
				if(failalpha >= 1f){
					GUI.color = new Color(1f, 1f, 1f, buttonfailalpha);
					if(GUI.Button(new Rect(posRetry.x*Screen.width, posRetry.y*Screen.height, posRetry.width*Screen.width, posRetry.height*Screen.height), "Retry") && !deadAndRetry && !deadAndGiveUp && buttonfailalpha > 0.5f && speedmodok){
							deadAndRetry = true;
					}
					
					if(GUI.Button(new Rect(posGiveUp.x*Screen.width, posGiveUp.y*Screen.height, posGiveUp.width*Screen.width, posGiveUp.height*Screen.height), "Give up") && !deadAndRetry && !deadAndGiveUp && buttonfailalpha > 0.5f){
							deadAndGiveUp = true;
					}
					
					GUI.color = new Color(0.7f, 0.7f, 0.7f, buttonfailalpha);
					GUI.Label(new Rect(SpeedModText.x*Screen.width, SpeedModText.y*Screen.height, SpeedModText.width*Screen.width, SpeedModText.height*Screen.height), TextManager.Instance.texts["Util"]["SPEEDMOD_TEXT"]);
					speedmodstring = GUI.TextArea (new Rect(SpeedModTextAera.x*Screen.width, SpeedModTextAera.y*Screen.height, SpeedModTextAera.width*Screen.width, SpeedModTextAera.height*Screen.height), speedmodstring.Trim(), 5);
								
					if(!String.IsNullOrEmpty(speedmodstring)){
						double result;
						if(System.Double.TryParse(speedmodstring, out result)){
							if(result >= (double)0.25 && result <= (double)15){
								speedmodSelected = (float)result;
								speedmodok = true;
								var bpmdisplaying = thesong.bpmToDisplay;
								if(bpmdisplaying.Contains("->")){
									bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[0])*speedmodSelected).ToString("0") + "->" + (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[1])*speedmodSelected).ToString("0");
								}else{
									bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying)*speedmodSelected).ToString("0");
								}
								GUI.Label(new Rect((SpeedModTextInfo.x + offsetSpeedRateX)*Screen.width, SpeedModTextInfo.y*Screen.height, SpeedModTextInfo.width*Screen.width, SpeedModTextInfo.height*Screen.height), "Speedmod : x" + speedmodSelected.ToString("0.00") + " (" + bpmdisplaying + " BPM)");
							}else{
								GUI.color = new Color(0.7f, 0.2f, 0.2f, 1f);
								GUI.Label(new Rect((SpeedModTextInfo.x + offsetSpeedRateX)*Screen.width, SpeedModTextInfo.y*Screen.height, SpeedModTextInfo.width*Screen.width, SpeedModTextInfo.height*Screen.height), "Speedmod must be between x0.25 and x15");
								GUI.color = new Color(1f, 1f, 1f, 1f);
								speedmodok = false;
							}
						}else{
							GUI.color = new Color(0.7f, 0.2f, 0.2f, 1f);
							GUI.Label(new Rect((SpeedModTextInfo.x + offsetSpeedRateX)*Screen.width, SpeedModTextInfo.y*Screen.height, SpeedModTextInfo.width*Screen.width, SpeedModTextInfo.height*Screen.height), "Speedmod is not a valid value");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							speedmodok = false;
						}
					}else{
						GUI.color = new Color(0.7f, 0.2f, 0.2f, 1f);
						GUI.Label(new Rect((SpeedModTextInfo.x + offsetSpeedRateX)*Screen.width, SpeedModTextInfo.y*Screen.height, SpeedModTextInfo.width*Screen.width, SpeedModTextInfo.height*Screen.height), "Empty value");
						GUI.color = new Color(1f, 1f, 1f, 1f);
						speedmodok = false;
					}
				}
			}
			
		}
				
		if(clear){
			//if(oneSecond > timeFailAppear){
				var ratiow = (float)posClear.width/(float)Mathf.Max (posClear.width, posClear.height);
				var ratioh = (float)posClear.height/(float)Mathf.Max (posClear.width, posClear.height);
				if(fullCombo || fullExCombo || fullFantCombo || perfect){
					GUI.color = new Color(1f, 1f, 1f, passalpha*alpha);
					GUI.DrawTexture(new Rect(fullComboPos.x*Screen.width,fullComboPos.y*Screen.width, fullComboPos.width*Screen.width, fullComboPos.height*Screen.height), TextureBase[perfect ? "PERFECT" : (fullFantCombo ? "FFC" : (fullExCombo ? "FEC" : "FC"))]);
				}
				//GUI.DrawTexture(new Rect(0f,0f, Screen.width*1.2f, Screen.height*1.2f), TextureBase["BLACK"]);
				
				GUI.color = new Color(1f, 1f, 1f, 1f);
				if(appearFailok) GUI.DrawTexture(new Rect((posClear.x - zwip - ratiow*zoomfail/2f)*Screen.width, (posClear.y + zwip - ratioh*zoomfail/2f)*Screen.height ,
					(posClear.width + zwip*2 + ratiow*zoomfail)*Screen.width, (posClear.height - zwip*2 + ratioh*zoomfail)*Screen.height), TextureBase["CLEAR"]);	
				GUI.color = new Color(1f, 1f, 1f, failalpha);
				GUI.DrawTexture(new Rect(0f,0f, Screen.width*1.2f, Screen.height*1.2f), TextureBase["BLACK"]);
			//}
			
		}
		
	}
	
	
	
	
	// Update is called once per frame
	void Update () {
		
		
		//FPS
		_count++;
		_timer += Time.deltaTime;
		if(_timer >= 1f){
			fps = _count;
			_count = 0L;
			_timer = 0;
		}
		
		if((oneSecond >= 1.5f && !dead) || clear){
			//timetotal for this frame
			
			timetotalchart = timebpm + timechart + totaltimestop;
			//var totalchartbegin = timetotalchart;
			
			//iwashere = 0;
			
			//Move Arrows
			//VersionLibre
			MoveArrows();
				
			//Verify inputs
			VerifyValidFrozenArrow();
			VerifyKeysInput();
			VerifyKeysOutput();
			
			//Changement bpm et stops
			VerifyBPMnSTOP();
			
			
			
			//Progress time chart
			if(actualstop != 0){
				
					
					
					if(timestop >= actualstop){
						timechart += timestop - (float)actualstop;
						totaltimestop += (float)actualstop - timestop;
						timetotalchart = timebpm + timechart + totaltimestop;
						actualstop = (double)0;
						timestop = 0f;
						timechart += Time.deltaTime;
						//iwashere = 1;
					}else{
						totaltimestop += Time.deltaTime;
						timestop += Time.deltaTime;
						//iwashere = 2;
					}

			}else{
				timechart += Time.deltaTime;
				//iwashere = 3;
				//Debug.Log(timechart);
			}
			
			
			
			//GUI part, because GUI is so slow :(
			//Utile ?
			RefreshGUIPart();
			
			//Start song
			timetotalchart = timebpm + timechart + totaltimestop;
			
			
			if(firstUpdate){
				if(startTheSong <= 0f){
					audio.PlayOneShot(songLoaded);
					timechart += startTheSong; 
					timetotalchart = timebpm + timechart + totaltimestop;
					firstUpdate = false;
				}else{
					startTheSong -= Time.deltaTime;	
				}
			}
			
			BumpsBPM();
			
			//Fail/Clear part
			
			if(thesong.duration < timetotalchart && !fail && !clear){
				if(life > 0f){
					clear = true;
				}else{
					fail = true;
				}
				if(scoreCount["DECENT"] == 0 && scoreCount["WAYOFF"] == 0 && scoreCount["MISS"] == 0){
					if(score >= 100f || scoreInverse == 100f) perfect = true;
					if(scoreCount["EXCELLENT"] == 0 && scoreCount["GREAT"] == 0) fullFantCombo = true;
					if(scoreCount["GREAT"] == 0) fullExCombo = true;
					fullCombo = true;
				}
				oneSecond = 0f;
				failalpha = 0f;
				buttonfailalpha = 0f;
				passalpha = 1f;
				alpha = 1f;
				sensFantastic = true;
			}
			
			if(fail){
				if((typeOfDeath != 2 && (typeOfDeath == 0 || comboMisses >= 30)) || thesong.duration < timetotalchart || timeGiveUp >= 1f){
					dead = true;
					audio.Stop ();
					Background.GetComponent<MoveBackground>().enabled = false;
					CameraBackground.GetComponent<MoveCameraBackground>().enabled = false;
					matProgressBarFull.color = new Color(0.5f, 0.5f, 0.5f, 1f);
					TMainCamera.GetComponent<GrayscaleEffect>().enabled = true;
					oneSecond = 0f;
					alpha = 1f;
					sensFantastic = true;
					
				}
			}
			
			if(clear){
				oneSecond += Time.deltaTime;
				if(audio.volume > 0) audio.volume -= Time.deltaTime/speedFadeAudio;
					
				if(!appearFailok){
					StartCoroutine(swipTexture(false, posClear.height, 0f));
					var contains = perfect ? "Perf" : (fullFantCombo ? "FFC" : (fullExCombo ? "FEC" : ( fullCombo ? "FBC" : "noPS")) );
					if(!contains.Contains("noPS")){
						particleComboCam.gameObject.active = true;
						foreach(var part in clearcombo.Where(c => c.Key.Contains(contains))){
							part.Value.gameObject.active = true;
							part.Value.Play();
						}
					}
					
					appearFailok = true;
					
				}
				if(oneSecond > timeClearDisappear - 1){
					passalpha -= Time.deltaTime;
				}
				if(oneSecond > timeClearDisappear){
					if(failalpha >= 1){
							SendDataToDatamanager();
							Application.LoadLevel("ScoreScene");
					} 
					failalpha += Time.deltaTime/speedAlphaFailFade;
				}
				ClignCombo();
			}
			
			//timetotalchart = timebpm + timechart + totaltimestop;
			//if((timetotalchart - totalchartbegin) < 0.001f)Debug.Log("Progression : " + (timetotalchart - totalchartbegin) + " / dt : " + Time.deltaTime + " / washere : " + iwashere + " / time : " + timetotalchart);
			
		}else{
			oneSecond += Time.deltaTime;
			if(!dead) MoveCameraBefore();
			if(dead){
				if(oneSecond > timeFailAppear && !disappearFailok){
					//zoomfail += Time.deltaTime/speedzoom;
					if(failalpha < 1) failalpha += Time.deltaTime/speedAlphaFailFade;
					if(failalpha >= 1 && buttonfailalpha < 1) buttonfailalpha += Time.deltaTime/speedbuttonfailalpha;
					ClignFailed();
				}
				if(!appearFailok && oneSecond > timeFailAppear + 1){
					StartCoroutine(swipTexture(false, posFail.height, 0f));
					appearFailok = true;
					cacheFailed = false;
				}
				if(!disappearFailok && (deadAndRetry || deadAndGiveUp)){
					StartCoroutine(swipTexture(true, posFail.height, 0.5f));
					disappearFailok = true;
					timeFailDisappear = oneSecond;
				}
				if(disappearFailok && oneSecond < (timeFailDisappear + 1f) && buttonfailalpha > 0){
					buttonfailalpha -= Time.deltaTime/speedbuttonfailalpha;
				}
				if(disappearFailok && oneSecond > (timeFailDisappear + 1f) ){
					
					SendDataToDatamanager();
					if(deadAndRetry){
						Application.LoadLevel("ChartScene");
					}else if(deadAndGiveUp){
						Application.LoadLevel("ScoreScene");
					}
					
					
				}
			}else if(oneSecond >= 0.5f && !scenechartfaded){
				GetComponent<FadeManager>().FadeOut();
				scenechartfaded = true;
			}
			
		}
		
	}
	
	
	void BumpsBPM(){
		if(nextBump < Bumps.Count && Bumps[nextBump] <= timetotalchart){
			matArrowModel.color = new Color(1f, 1f, 1f, 1f);
			nextBump++;
		}	
		
		for(int i=0; i<4; i++){
			if(arrowTarget[i].transform.localScale.x < scaleBase){
				arrowTarget[i].transform.localScale += new Vector3(Time.deltaTime*speedBumpInput, Time.deltaTime*speedBumpInput, Time.deltaTime*speedBumpInput);
			}
		}
		
	}
	
	
	void FixedUpdate(){
		
		/*if(oneSecond >= 1f && !dead){
			MoveArrows();
		}*/
		
		if(matArrowModel.color.r > 0.5f){
			var m = 0.5f*Time.deltaTime/speedBumps;
			matArrowModel.color -= new Color(m,m,m, 0f);
		}
		if(timetotalchart >= firstArrow && timetotalchart < lastArrow){
			var div = (float)((timetotalchart - firstArrow)/(lastArrow - firstArrow));
			progressBar.transform.localPosition = new Vector3(progressBar.transform.localPosition.x, - (10f - 10f*div), 8f);
			progressBar.transform.localScale = new Vector3(progressBar.transform.localScale.x, 10f*div, 1f);
		}
		
		
		if(stateSpeed > 0){
			slow.renderer.material.color = new Color(1f, 0f, 0f, 0.5f);
			stateSpeed = 0f;
			changeColorSlow = true;
		}else if(stateSpeed < 0){
			fast.renderer.material.color = new Color(1f, 0f, 0f, 0.5f);
			stateSpeed = 0f;
			changeColorFast = true;
		}
		
		if(changeColorFast){
			var div = Time.deltaTime/ClignSpeed;
			var col = fast.renderer.material.color.r - div;
			var colp = fast.renderer.material.color.g + div;
			fast.renderer.material.color = new Color(col, colp, colp, 0.5f);
			if(col <= 0.5f) changeColorFast = false;
		}else if(changeColorSlow){
			var div = Time.deltaTime/ClignSpeed;
			var col = slow.renderer.material.color.r - div;
			var colp = slow.renderer.material.color.g + div;
			slow.renderer.material.color = new Color(col, colp, colp, 0.5f);
			if(col <= 0.5f) changeColorSlow = false;
		}
		
		
		if(ct < ComboType.FULLCOMBO && combo > 100){
			switch (ct){
			case ComboType.FULLEXCELLENT:
				
				if((colorCombo <= 0.3f && signCombo == -1) || (colorCombo >= 1f && signCombo == 1) ){ signCombo *= -1; }
				colorCombo += signCombo*Time.deltaTime/speedCombofade;
				
				matProgressBar.color = new Color(1f, 1f, colorCombo, 1f);
				alreadytaged = false;
				break;
			case ComboType.FULLFANTASTIC:
				if((colorCombo <= 0.3f && signCombo == -1) || (colorCombo >= 1f && signCombo == 1) ){ signCombo *= -1; }
				colorCombo += signCombo*Time.deltaTime/speedCombofade;
				matProgressBar.color = new Color(colorCombo, 1f, 1f, 1f);
				alreadytaged = false;
				break;
			}
		}else if(!alreadytaged){
			matProgressBar.color = new Color(1f, 1f, 1f, 1f);
			alreadytaged = true;
		}
	}
	
	
	
	//For moving arrows or do some displaying things
	
	#region Defilement chart
	
	void MoveArrows(){
	
		var bps = thesong.getBPS(actualBPM);
		var move = -((float)(bps*timechart))*speedmod +  changeBPM;
		TMainCamera.position = new Vector3(3f, move - 5, -10f);
		/*arrowLeft.transform.position = new Vector3(0f, -((float)(bps*timechart))*speedmod  + changeBPM, 2f);
		arrowRight.transform.position = new Vector3(6f, -((float)(bps*timechart))*speedmod + changeBPM, 2f);
		arrowDown.transform.position = new Vector3(2f, -((float)(bps*timechart))*speedmod + changeBPM, 2f);
		arrowUp.transform.position = new Vector3(4f, -((float)(bps*timechart))*speedmod + changeBPM, 2f);*/
		
		foreach(var el in arrowFrozen.Keys){
			var pos = el.goArrow.transform.position;
			el.goArrow.transform.position = new Vector3(pos.x, move, pos.z);
			pos = el.goArrow.transform.position;
			var div = ((el.posEnding.y - pos.y)/2f);
			el.goFreeze.transform.position = new Vector3(el.goFreeze.transform.position.x, (pos.y + div) , el.goFreeze.transform.position.z);
			el.goFreeze.transform.localScale = new Vector3(1f, -div, 0.1f);
			el.goFreeze.transform.GetChild(0).transform.localScale = new Vector3((el.posEnding.y - pos.y)/(el.posEnding.y - el.posBegining.y), 1f, 0.1f);
			el.changeColorFreeze(arrowFrozen[el], unfrozed);
		}
	}
	
	
	void MoveCameraBefore(){
	
		var bps = thesong.getBPS(actualBPM);
		var move = -((float)(bps*(-(1.5 - oneSecond - startTheSong)))*speedmod);
		TMainCamera.position = new Vector3(3f, move - 5, -10f);
	}
	
	
	void VerifyBPMnSTOP(){
		//BPM change verify
		if(nextSwitchBPM < thesong.bpms.Count && (thesong.bpms.ElementAt(nextSwitchBPM).Key <= timetotalchart)){
			
			//iwashere = 1;
			var bps = thesong.getBPS(actualBPM);
			changeBPM += -((float)(bps*(timechart - (float)(timetotalchart - thesong.bpms.ElementAt(nextSwitchBPM).Key))))*speedmod;
			timebpm += (double)timechart - (timetotalchart - thesong.bpms.ElementAt(nextSwitchBPM).Key);
			timechart = (float)(timetotalchart - thesong.bpms.ElementAt(nextSwitchBPM).Key);
			actualBPM = thesong.bpms.ElementAt(nextSwitchBPM).Value;
			nextSwitchBPM++;
		}
		
		
		//Stop verif
		if(nextSwitchStop < thesong.stops.Count && (thesong.stops.ElementAt(nextSwitchStop).Key <= timetotalchart)){
			//iwashere = 2;
			timetotalchart = timebpm + timechart + totaltimestop;
			timechart -= (float)timetotalchart - (float)thesong.stops.ElementAt(nextSwitchStop).Key;
			timestop += (float)timetotalchart - (float)thesong.stops.ElementAt(nextSwitchStop).Key;
			totaltimestop += (float)timetotalchart - (float)thesong.stops.ElementAt(nextSwitchStop).Key;
			timetotalchart = timebpm + timechart + totaltimestop;
			actualstop = thesong.stops.ElementAt(nextSwitchStop).Value;
			nextSwitchStop++;
		}
		
	}
	
	#endregion
	
	#region GUI
	void RefreshGUIPart(){
		if(scoreToDisplay == Precision.FANTASTIC){
			if(sensFantastic){
				alpha -= 0.2f*Time.deltaTime/timeClignotementFantastic;
				sensFantastic = alpha > 0.8f;
			}else{
				alpha += 0.2f*Time.deltaTime/timeClignotementFantastic;
				sensFantastic = alpha >= 1f;
			}
		}else{
			alpha = 1f;
		}
		
		if(zoom > 0 && scoreToDisplay != Precision.DECENT && scoreToDisplay != Precision.WAYOFF){
			zoom -= baseZoom*Time.deltaTime/vitesseZoom;
		}else{
			zoom = 0;
		}
		
		if(alphaCombo > 0.5){
			alphaCombo -= Time.deltaTime/speedAlphaCombo;	
		}
		
		timeDisplayScore += Time.deltaTime;
		
	}
	
	void ClignFailed(){
		if(sensFantastic){
			alpha -= 0.1f*Time.deltaTime/timeClignotementFantastic;
			sensFantastic = alpha > 0.9f;
		}else{
			alpha += 0.1f*Time.deltaTime/timeClignotementFantastic;
			sensFantastic = alpha >= 1f;
		}
		
		
	}
	
	void ClignCombo(){
		if(sensFantastic){
			alpha -= 0.2f*Time.deltaTime/timeClignotementFantastic;
			sensFantastic = alpha > 0.8f;
		}else{
			alpha += 0.2f*Time.deltaTime/timeClignotementFantastic;
			sensFantastic = alpha >= 1f;
		}
		
		
	}
	#endregion
	
	#region Inputs verify
	//Valid or deny the frozen arrow
	void VerifyValidFrozenArrow(){
		if(arrowFrozen.Any()){
			var KeyToRemove = new List<Arrow>();
			for(int i=0; i<arrowFrozen.Count;i++){
				var el = arrowFrozen.ElementAt(i);
				arrowFrozen[el.Key] += Time.deltaTime;
				
				if(el.Key.timeEndingFreeze <= timetotalchart){
					switch(el.Key.arrowPos){
					case ArrowPosition.LEFT:
						StartParticleFreezeLeft(false);
						break;
					case ArrowPosition.DOWN:
						StartParticleFreezeDown(false);
						break;
					case ArrowPosition.UP:
						StartParticleFreezeUp(false);
						break;
					case ArrowPosition.RIGHT:
						StartParticleFreezeRight(false);
						break;
					}
					GainScoreAndLife("FREEZE");
					scoreCount[el.Key.arrowType == ArrowType.FREEZE ? "FREEZE" : "ROLL"] += 1;
					DestroyImmediate(el.Key.goArrow);
					DestroyImmediate(el.Key.goFreeze);
					
					KeyToRemove.Add(el.Key);
				}
				
				
				if(el.Value >= unfrozed && !KeyToRemove.Contains(el.Key)){
					el.Key.goArrow.GetComponent<ArrowScript>().missed = true;
					switch(el.Key.arrowPos){
					case ArrowPosition.LEFT:
						StartParticleFreezeLeft(false);
						break;
					case ArrowPosition.DOWN:
						StartParticleFreezeDown(false);
						break;
					case ArrowPosition.UP:
						StartParticleFreezeUp(false);
						break;
					case ArrowPosition.RIGHT:
						StartParticleFreezeRight(false);
						break;
					}
					GainScoreAndLife("UNFREEZE");
					KeyToRemove.Add(el.Key);
				}
				
				
			}
			
			
			
			foreach(var k in KeyToRemove){
				arrowFrozen.Remove(k);
			}
			
		}
	}
	
	
	
	//Verify keys Input at this frame
	
	//Revoir la façon de gérer les mines...
	void VerifyKeysInput(){
		
		if((Input.GetKeyDown(KeyCodeLeft) || Input.GetKeyDown(SecondaryKeyCodeLeft) ) && (arrowLeftList.Any())){
			arrowTarget[0].localScale = arrowTarget[0].localScale*percentScaleInput;
			var ar = findNextLeftArrow();
			double realprec = ar.time - (timetotalchart); //Retirer ou non le Time.deltaTime ?
			double prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				
				if(ar.imJump){
					ar.alreadyValid = true;
					ar.goArrow.GetComponent<ArrowScript>().valid = true;
					
					if(!ar.neighboors.Any(c => c.alreadyValid == false)){
						var left = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
						var down = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
						var up = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
						var right = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
						if(left != null){
							if(left.arrowType == ArrowType.NORMAL || left.arrowType == ArrowType.MINE){
								DestroyImmediate(left.goArrow);
								
							}else{
								arrowFrozen.Add(left,0f);
								left.displayFrozenBar();
								StartParticleFreezeLeft(true);
								
							}
							
							arrowLeftList.Remove(left);
							StartParticleLeft(timeToPrec(prec));
						}
						if(down != null){
							if(down.arrowType == ArrowType.NORMAL || down.arrowType == ArrowType.MINE){
								DestroyImmediate(down.goArrow);
								
							}else{
								arrowFrozen.Add(down,0f);
								down.displayFrozenBar();
								StartParticleFreezeDown(true);
								
							}
							
							arrowDownList.Remove(down);
							StartParticleDown(timeToPrec(prec));
						}
						if(up != null){
							if(up.arrowType == ArrowType.NORMAL || up.arrowType == ArrowType.MINE){
								DestroyImmediate(up.goArrow);
								
							}else{
								arrowFrozen.Add(up,0f);
								up.displayFrozenBar();
								StartParticleFreezeUp(true);
								
							}
							arrowUpList.Remove(up);
							StartParticleUp(timeToPrec(prec));
						}
						if(right != null){
							if(right.arrowType == ArrowType.NORMAL || right.arrowType == ArrowType.MINE){
								DestroyImmediate(right.goArrow);
								
							}else{
								arrowFrozen.Add(right,0f);
								right.displayFrozenBar();
								StartParticleFreezeRight(true);
								
							}
							arrowRightList.Remove(right);
							StartParticleRight(timeToPrec(prec));
						}
						precAverage.Add(realprec);
						var ttp = timeToPrec(prec);
						GainScoreAndLife(ttp.ToString());
						scoreCount[ttp.ToString()] += 1;
						scoreCount[ar.neighboors.Count > 2 ? "HANDS" : "JUMPS"] += 1;
						displayPrec(prec);
						GainCombo(ar.neighboors.Count, ttp);
					}
				}else{
					if(ar.arrowType == ArrowType.NORMAL || ar.arrowType == ArrowType.MINE){
						DestroyImmediate(ar.goArrow);
						
					}else{
						ar.goArrow.GetComponent<ArrowScript>().valid = true;
						arrowFrozen.Add(ar,0f);
						ar.displayFrozenBar();
						StartParticleFreezeLeft(true);
						
					}
					precAverage.Add(realprec);
					var ttp = timeToPrec(prec);
					scoreCount[ttp.ToString()] += 1 ;
					GainCombo(1, ttp);
					arrowLeftList.Remove(ar);
					StartParticleLeft(ttp);
					GainScoreAndLife(ttp.ToString());
					displayPrec(prec);
				}
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
					if(ar.imJump){
					
						ar.alreadyValid = true;
						ar.goArrow.GetComponent<ArrowScript>().missed = true;
						if(!ar.neighboors.Any(c => c.alreadyValid == false)){
							var left = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
							var down = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
							var up = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
							var right = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
							if(left != null){
								
								arrowLeftList.Remove(left);
								StartParticleLeft(timeToPrec(prec));
							}
							if(down != null){
								
								
								arrowDownList.Remove(down);
								StartParticleDown(timeToPrec(prec));
							}
							if(up != null){
								
								arrowUpList.Remove(up);
								StartParticleUp(timeToPrec(prec));
							}
							if(right != null){
								
								arrowRightList.Remove(right);
								StartParticleRight(timeToPrec(prec));
							}
							precAverage.Add(realprec);
							var ttp = timeToPrec(prec);
							scoreCount[ttp.ToString()] += 1;
							GainScoreAndLife(ttp.ToString());
							displayPrec(prec);
						}
					}else{
						precAverage.Add(realprec);
						var ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						ar.goArrow.GetComponent<ArrowScript>().missed = true;
						arrowLeftList.Remove(ar);
						StartParticleLeft(ttp);
						displayPrec(prec);
						GainScoreAndLife(ttp.ToString());
					}
					ComboStop(false);
			}
			
			if(prec > precToTime(Precision.FANTASTIC) && prec < precToTime(Precision.WAYOFF)){
				stateSpeed = -1f*Mathf.Sign((float)(ar.time - timetotalchart));
			}else{
				stateSpeed = 0f;	
			}
			
		}
		
		
		if((Input.GetKeyDown(KeyCodeDown)  || Input.GetKeyDown(SecondaryKeyCodeDown))  && (arrowDownList.Any())){
			arrowTarget[1].localScale = arrowTarget[1].localScale*percentScaleInput;
			var ar = findNextDownArrow();
			
			double realprec = ar.time - (timetotalchart);
			double prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(ar.imJump){
					ar.alreadyValid = true;
					ar.goArrow.GetComponent<ArrowScript>().valid = true;
					
					if(!ar.neighboors.Any(c => c.alreadyValid == false)){
						var left = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
						var down = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
						var up = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
						var right = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
						if(left != null){
							if(left.arrowType == ArrowType.NORMAL || left.arrowType == ArrowType.MINE){
								DestroyImmediate(left.goArrow);
								
							}else{
								arrowFrozen.Add(left,0f);
								left.displayFrozenBar();
								StartParticleFreezeLeft(true);
								
							}
							
							arrowLeftList.Remove(left);
							StartParticleLeft(timeToPrec(prec));
						}
						if(down != null){
							if(down.arrowType == ArrowType.NORMAL || down.arrowType == ArrowType.MINE){
								DestroyImmediate(down.goArrow);
								
							}else{
								arrowFrozen.Add(down,0f);
								down.displayFrozenBar();
								StartParticleFreezeDown(true);
								
							}
							
							arrowDownList.Remove(down);
							StartParticleDown(timeToPrec(prec));
						}
						if(up != null){
							if(up.arrowType == ArrowType.NORMAL || up.arrowType == ArrowType.MINE){
								DestroyImmediate(up.goArrow);
								
							}else{
								arrowFrozen.Add(up,0f);
								up.displayFrozenBar();
								StartParticleFreezeUp(true);
								
							}
							arrowUpList.Remove(up);
							StartParticleUp(timeToPrec(prec));
						}
						if(right != null){
							if(right.arrowType == ArrowType.NORMAL || right.arrowType == ArrowType.MINE){
								DestroyImmediate(right.goArrow);
								
							}else{
								arrowFrozen.Add(right,0f);
								right.displayFrozenBar();
								StartParticleFreezeRight(true);
								
							}
							arrowRightList.Remove(right);
							StartParticleRight(timeToPrec(prec));
						}
						precAverage.Add(realprec);
						var ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						GainCombo(ar.neighboors.Count, ttp);
						scoreCount[ar.neighboors.Count > 2 ? "HANDS" : "JUMPS"] += 1;
						GainScoreAndLife(ttp.ToString());
						displayPrec(prec);
					}
				}else{
					if(ar.arrowType == ArrowType.NORMAL || ar.arrowType == ArrowType.MINE){
						DestroyImmediate(ar.goArrow);
						
					}else{
						ar.goArrow.GetComponent<ArrowScript>().valid = true;
						arrowFrozen.Add(ar,0f);
						ar.displayFrozenBar();
						StartParticleFreezeDown(true);
						
					}
					precAverage.Add(realprec);
					var ttp = timeToPrec(prec);
					scoreCount[ttp.ToString()] += 1;
					GainCombo(1, ttp);
					arrowDownList.Remove(ar);
					StartParticleDown(ttp);
					GainScoreAndLife(ttp.ToString());
					displayPrec(prec);
				}
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
					if(ar.imJump){
					
						ar.alreadyValid = true;
						ar.goArrow.GetComponent<ArrowScript>().missed = true;
						if(!ar.neighboors.Any(c => c.alreadyValid == false)){
							var left = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
							var down = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
							var up = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
							var right = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
							if(left != null){
								
								arrowLeftList.Remove(left);
								StartParticleLeft(timeToPrec(prec));
							}
							if(down != null){
								
								
								arrowDownList.Remove(down);
								StartParticleDown(timeToPrec(prec));
							}
							if(up != null){
								
								arrowUpList.Remove(up);
								StartParticleUp(timeToPrec(prec));
							}
							if(right != null){
								
								arrowRightList.Remove(right);
								StartParticleRight(timeToPrec(prec));
							}
							precAverage.Add(realprec);
							var ttp = timeToPrec(prec);
							scoreCount[ttp.ToString()] += 1;
							GainScoreAndLife(ttp.ToString());
							displayPrec(prec);
						}
					}else{
						precAverage.Add(realprec);
						var ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						ar.goArrow.GetComponent<ArrowScript>().missed = true;
						arrowDownList.Remove(ar);
						StartParticleDown(ttp);
						displayPrec(prec);
						GainScoreAndLife(ttp.ToString());
					}
					ComboStop(false);
			}
			
			if(prec > precToTime(Precision.FANTASTIC) && prec < precToTime(Precision.WAYOFF)){
				stateSpeed = -1f*Mathf.Sign((float)(ar.time - timetotalchart));
			}else{
				stateSpeed = 0f;	
			}
		}
		
		
		if((Input.GetKeyDown(KeyCodeUp) || Input.GetKeyDown(SecondaryKeyCodeUp)) && (arrowUpList.Any())){
			arrowTarget[2].localScale = arrowTarget[2].localScale*percentScaleInput;
			var ar = findNextUpArrow();
			double realprec = ar.time - (timetotalchart);
			double prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(ar.imJump){
					ar.alreadyValid = true;
					ar.goArrow.GetComponent<ArrowScript>().valid = true;
					
					if(!ar.neighboors.Any(c => c.alreadyValid == false)){
						var left = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
						var down = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
						var up = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
						var right = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
						if(left != null){
							if(left.arrowType == ArrowType.NORMAL || left.arrowType == ArrowType.MINE){
								DestroyImmediate(left.goArrow);
								
							}else{
								arrowFrozen.Add(left,0f);
								left.displayFrozenBar();
								StartParticleFreezeLeft(true);
								
							}
							
							arrowLeftList.Remove(left);
							StartParticleLeft(timeToPrec(prec));
						}
						if(down != null){
							if(down.arrowType == ArrowType.NORMAL || down.arrowType == ArrowType.MINE){
								DestroyImmediate(down.goArrow);
								
							}else{
								arrowFrozen.Add(down,0f);
								down.displayFrozenBar();
								StartParticleFreezeDown(true);
								
							}
							
							arrowDownList.Remove(down);
							StartParticleDown(timeToPrec(prec));
						}
						if(up != null){
							if(up.arrowType == ArrowType.NORMAL || up.arrowType == ArrowType.MINE){
								DestroyImmediate(up.goArrow);
								
							}else{
								arrowFrozen.Add(up,0f);
								up.displayFrozenBar();
								StartParticleFreezeUp(true);
								
							}
							arrowUpList.Remove(up);
							StartParticleUp(timeToPrec(prec));
						}
						if(right != null){
							if(right.arrowType == ArrowType.NORMAL || right.arrowType == ArrowType.MINE){
								DestroyImmediate(right.goArrow);
								
							}else{
								arrowFrozen.Add(right,0f);
								right.displayFrozenBar();
								StartParticleFreezeRight(true);
								
							}
							arrowRightList.Remove(right);
							StartParticleRight(timeToPrec(prec));
						}
						precAverage.Add(realprec);
						var ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						GainCombo(ar.neighboors.Count, ttp);
						scoreCount[ar.neighboors.Count > 2 ? "HANDS" : "JUMPS"] += 1;
						GainScoreAndLife(ttp.ToString());
						displayPrec(prec);
					}
				}else{
					if(ar.arrowType == ArrowType.NORMAL || ar.arrowType == ArrowType.MINE){
						DestroyImmediate(ar.goArrow);
						
					}else{
						ar.goArrow.GetComponent<ArrowScript>().valid = true;
						arrowFrozen.Add(ar,0f);
						ar.displayFrozenBar();
						StartParticleFreezeUp(true);
						
					}
					precAverage.Add(realprec);
					var ttp = timeToPrec(prec);
					scoreCount[ttp.ToString()] += 1;
					GainCombo(1, ttp);
					arrowUpList.Remove(ar);
					StartParticleUp(ttp);
					GainScoreAndLife(ttp.ToString());
					displayPrec(prec);
				}
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
					if(ar.imJump){
					
						ar.alreadyValid = true;
						ar.goArrow.GetComponent<ArrowScript>().missed = true;
						if(!ar.neighboors.Any(c => c.alreadyValid == false)){
							var left = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
							var down = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
							var up = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
							var right = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
							if(left != null){
								
								arrowLeftList.Remove(left);
								StartParticleLeft(timeToPrec(prec));
							}
							if(down != null){
								
								
								arrowDownList.Remove(down);
								StartParticleDown(timeToPrec(prec));
							}
							if(up != null){
								
								arrowUpList.Remove(up);
								StartParticleUp(timeToPrec(prec));
							}
							if(right != null){
								
								arrowRightList.Remove(right);
								StartParticleRight(timeToPrec(prec));
							}
							precAverage.Add(realprec);
							var ttp = timeToPrec(prec);
							scoreCount[ttp.ToString()] += 1;
							GainScoreAndLife(ttp.ToString());
							displayPrec(prec);
						}
					}else{
						precAverage.Add(realprec);
						var ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						ar.goArrow.GetComponent<ArrowScript>().missed = true;
						arrowUpList.Remove(ar);
						StartParticleUp(ttp);
						displayPrec(prec);
						GainScoreAndLife(ttp.ToString());
					}
					ComboStop(false);
			}
			
			if(prec > precToTime(Precision.FANTASTIC) && prec < precToTime(Precision.WAYOFF)){
				stateSpeed = -1f*Mathf.Sign((float)(ar.time - timetotalchart));
			}else{
				stateSpeed = 0f;	
			}
		
		}
		
		
		if((Input.GetKeyDown(KeyCodeRight) || Input.GetKeyDown(SecondaryKeyCodeRight)) && (arrowRightList.Any())){
			arrowTarget[3].localScale = arrowTarget[3].localScale*percentScaleInput;
			var ar = findNextRightArrow();
			double realprec = ar.time - (timetotalchart);
			double prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(ar.imJump){
					ar.alreadyValid = true;
					ar.goArrow.GetComponent<ArrowScript>().valid = true;
					
					if(!ar.neighboors.Any(c => c.alreadyValid == false)){
						var left = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
						var down = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
						var up = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
						var right = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
						if(left != null){
							if(left.arrowType == ArrowType.NORMAL || left.arrowType == ArrowType.MINE){
								DestroyImmediate(left.goArrow);
								
							}else{
								arrowFrozen.Add(left,0f);
								left.displayFrozenBar();
								StartParticleFreezeLeft(true);
								
							}
							
							arrowLeftList.Remove(left);
							StartParticleLeft(timeToPrec(prec));
						}
						if(down != null){
							if(down.arrowType == ArrowType.NORMAL || down.arrowType == ArrowType.MINE){
								DestroyImmediate(down.goArrow);
								
							}else{
								arrowFrozen.Add(down,0f);
								down.displayFrozenBar();
								StartParticleFreezeDown(true);
								
							}
							
							arrowDownList.Remove(down);
							StartParticleDown(timeToPrec(prec));
						}
						if(up != null){
							if(up.arrowType == ArrowType.NORMAL || up.arrowType == ArrowType.MINE){
								DestroyImmediate(up.goArrow);
								
							}else{
								arrowFrozen.Add(up,0f);
								up.displayFrozenBar();
								StartParticleFreezeUp(true);
								
							}
							arrowUpList.Remove(up);
							StartParticleUp(timeToPrec(prec));
						}
						if(right != null){
							if(right.arrowType == ArrowType.NORMAL || right.arrowType == ArrowType.MINE){
								DestroyImmediate(right.goArrow);
								
							}else{
								arrowFrozen.Add(right,0f);
								right.displayFrozenBar();
								StartParticleFreezeRight(true);
								
							}
							arrowRightList.Remove(right);
							StartParticleRight(timeToPrec(prec));
						}
						precAverage.Add(realprec);
						var ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						GainCombo(ar.neighboors.Count, ttp);
						scoreCount[ar.neighboors.Count > 2 ? "HANDS" : "JUMPS"] += 1;
						GainScoreAndLife(ttp.ToString());
						displayPrec(prec);
					}
				}else{
					if(ar.arrowType == ArrowType.NORMAL || ar.arrowType == ArrowType.MINE){
						DestroyImmediate(ar.goArrow);
						
					}else{
						ar.goArrow.GetComponent<ArrowScript>().valid = true;
						arrowFrozen.Add(ar,0f);
						ar.displayFrozenBar();
						StartParticleFreezeRight(true);
						
					}
					precAverage.Add(realprec);
					var ttp = timeToPrec(prec);
					scoreCount[ttp.ToString()] += 1;
					GainCombo(1, ttp);
					arrowRightList.Remove(ar);
					StartParticleRight(ttp);
					GainScoreAndLife(ttp.ToString());
					displayPrec(prec);
				}
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
					if(ar.imJump){
					
						ar.alreadyValid = true;
						ar.goArrow.GetComponent<ArrowScript>().missed = true;
						if(!ar.neighboors.Any(c => c.alreadyValid == false)){
							var left = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
							var down = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
							var up = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
							var right = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
							if(left != null){
								
								arrowLeftList.Remove(left);
								StartParticleLeft(timeToPrec(prec));
							}
							if(down != null){
								
								
								arrowDownList.Remove(down);
								StartParticleDown(timeToPrec(prec));
							}
							if(up != null){
								
								arrowUpList.Remove(up);
								StartParticleUp(timeToPrec(prec));
							}
							if(right != null){
								
								arrowRightList.Remove(right);
								StartParticleRight(timeToPrec(prec));
							}
							precAverage.Add(realprec);
							var ttp = timeToPrec(prec);
							scoreCount[ttp.ToString()] += 1;
							GainScoreAndLife(ttp.ToString());
							displayPrec(prec);
						}
					}else{
						precAverage.Add(realprec);
						var ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						ar.goArrow.GetComponent<ArrowScript>().missed = true;
						arrowRightList.Remove(ar);
						StartParticleRight(ttp);
						displayPrec(prec);
						GainScoreAndLife(ttp.ToString());
					}
					ComboStop(false);
			}
			
			if(prec > precToTime(Precision.FANTASTIC) && prec < precToTime(Precision.WAYOFF)){
				stateSpeed = -1f*Mathf.Sign((float)(ar.time - timetotalchart));
			}else{
				stateSpeed = 0f;	
			}
		}
		
		
		if(Input.GetKey(KeyCode.Escape)){
			timeGiveUp += Time.deltaTime;
			if(timeGiveUp >= 1f){
				fail = true;
			}
		}
		
		if(Input.GetKeyUp(KeyCode.Escape)){
			timeGiveUp = 0f;
		}
	}
	
	
	//Verify Key for all frames
	void VerifyKeysOutput(){
		if(arrowFrozen.Any()){
			if(Input.GetKey(KeyCodeLeft) || Input.GetKey(SecondaryKeyCodeLeft)){
				if(Input.GetKeyDown(KeyCodeLeft) || Input.GetKeyDown(SecondaryKeyCodeLeft)){
					if(arrowFrozen.Any(c => c.Key.arrowPos == ArrowPosition.LEFT && c.Key.arrowType == ArrowType.ROLL))
					{
						var froz = arrowFrozen.First(c => c.Key.arrowPos == ArrowPosition.LEFT);
						arrowFrozen[froz.Key] = 0f;
					}
				}
				if(arrowFrozen.Any(c => c.Key.arrowPos == ArrowPosition.LEFT && c.Key.arrowType == ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.arrowPos == ArrowPosition.LEFT);
					arrowFrozen[ar.Key] = 0f;
				}
			}
			
			
			if(Input.GetKey(KeyCodeDown) || Input.GetKey(SecondaryKeyCodeDown)){
				if(Input.GetKeyDown(KeyCodeDown) || Input.GetKeyDown(SecondaryKeyCodeDown)){
					if(arrowFrozen.Any(c => c.Key.arrowPos == ArrowPosition.DOWN && c.Key.arrowType == ArrowType.ROLL))
					{
						var froz = arrowFrozen.First(c => c.Key.arrowPos == ArrowPosition.DOWN);
						arrowFrozen[froz.Key] = 0f;
					}
				}
				if(arrowFrozen.Any(c => c.Key.arrowPos == ArrowPosition.DOWN && c.Key.arrowType == ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.arrowPos == ArrowPosition.DOWN);
					arrowFrozen[ar.Key] = 0f;
				}
			}
			
			
			if(Input.GetKey(KeyCodeUp) || Input.GetKey(SecondaryKeyCodeUp)){
				if(Input.GetKeyDown(KeyCodeUp) || Input.GetKeyDown(SecondaryKeyCodeUp)){
					if(arrowFrozen.Any(c => c.Key.arrowPos == ArrowPosition.UP && c.Key.arrowType == ArrowType.ROLL))
					{
						var froz = arrowFrozen.First(c => c.Key.arrowPos == ArrowPosition.UP);
						arrowFrozen[froz.Key] = 0f;
					}
				}
				if(arrowFrozen.Any(c => c.Key.arrowPos == ArrowPosition.UP && c.Key.arrowType == ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.arrowPos == ArrowPosition.UP);
					arrowFrozen[ar.Key] = 0f;
				}
			}
			
			
			if(Input.GetKey(KeyCodeRight) || Input.GetKey(SecondaryKeyCodeRight)){
				if(Input.GetKeyDown(KeyCodeRight) || Input.GetKeyDown(SecondaryKeyCodeRight)){
					if(arrowFrozen.Any(c => c.Key.arrowPos == ArrowPosition.RIGHT && c.Key.arrowType == ArrowType.ROLL))
					{
						var froz = arrowFrozen.First(c => c.Key.arrowPos == ArrowPosition.RIGHT);
						arrowFrozen[froz.Key] = 0f;
					}
				}
				if(arrowFrozen.Any(c => c.Key.arrowPos == ArrowPosition.RIGHT && c.Key.arrowType == ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.arrowPos == ArrowPosition.RIGHT);
					arrowFrozen[ar.Key] = 0f;
				}
			}
			
		}
		
	}
	
	#endregion
	
	#region Particules	
	void StartParticleLeft(Precision prec){
		var displayPrec = prec.ToString();
		if(prec < Precision.DECENT && combo >= 100) displayPrec += "C";
		var ps = precLeft[displayPrec];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
	}
	
	void StartParticleDown(Precision prec){
		var displayPrec = prec.ToString();
		if(prec < Precision.DECENT && combo >= 100) displayPrec += "C";
		var ps = precDown[displayPrec];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
	}
	
	void StartParticleUp(Precision prec){
		var displayPrec = prec.ToString();
		if(prec < Precision.DECENT && combo >= 100) displayPrec += "C";
		var ps = precUp[displayPrec];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
	}
	
	void StartParticleRight(Precision prec){
		var displayPrec = prec.ToString();
		if(prec < Precision.DECENT && combo >= 100) displayPrec += "C";
		var ps = precRight[displayPrec];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
	}
	
	void StartParticleFreezeLeft(bool state){
		var ps = precLeft["FREEZE"];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
		ps.time = 1f;
		ps.loop = state;
		
	}
	
	void StartParticleFreezeRight(bool state){
		var ps = precRight["FREEZE"];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
		ps.time = 1f;
		ps.loop = state;
		
	}
	
	void StartParticleFreezeDown(bool state){
		var ps = precDown["FREEZE"];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
		ps.time = 1f;
		ps.loop = state;
		
	}
	
	void StartParticleFreezeUp(bool state){
		var ps = precUp["FREEZE"];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
		ps.time = 1f;
		ps.loop = state;
		
	}
	
	public void StartParticleMineLeft(){
		var ps = precLeft["MINE"];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
		
	}
	
	public void StartParticleMineRight(){
		var ps = precRight["MINE"];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
		
	}
	
	public void StartParticleMineDown(){
		var ps = precDown["MINE"];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
		
	}
	
	public void StartParticleMineUp(){
		var ps = precUp["MINE"];
		if(!ps.gameObject.active) ps.gameObject.active = true;
		ps.Play();
		
	}
	
	#endregion
	
	#region util
	public void GainScoreAndLife(string s){
		if(!fail){
			if(lifeBase[s] <= 0 || combo >= DataManager.Instance.regenComboAfterMiss){
				life += lifeBase[s];
				if(life > 100f){
					life = 100f;	
				}else if(life < 0f){
					life = 0f;	
				}
				if(!lifeGraph.ContainsKey(timetotalchart)) lifeGraph.Add(timetotalchart, life);
				theLifeBar.ChangeBar(life);
			}
			score += scoreBase[s];
			scoreInverse -= (fantasticValue-scoreBase[s]);
			//Debug.Log ("si : " + scoreInverse);
			if(score > 100f){
				score = 100f;	
			}else if(score < 0f){
				score = 0f;	
			}
			displaying = scoreDecoupe();
		}
		
		if(life <= 0f || scoreInverse < targetScoreInverse){
			//Debug.Log ("Fail at life");
			fail = true;
		}
	}
	
	public void AddMineToScoreCombo(){
		scoreCount["MINE"] += 1;	
	}
	
	public void AddMissToScoreCombo(){
		scoreCount["MISS"] += 1;	
	}
	
	public void GainCombo(int c, Precision prec){
		combo+= c;
		alphaCombo = 1f;
		if(ct != ComboType.NONE && prec != Precision.FANTASTIC){
			if(ct == ComboType.FULLFANTASTIC){
				switch(prec){
				case Precision.EXCELLENT:
					ct = ComboType.FULLEXCELLENT;
					firstEx = timetotalchart;
					break;
				case Precision.GREAT:
					ct = ComboType.FULLCOMBO; 
					firstEx = timetotalchart;
					firstGreat = timetotalchart;
					break;
				default:
					firstEx = timetotalchart;
					firstGreat = timetotalchart;
					firstMisteak = timetotalchart;
					ct = ComboType.NONE; 
					break;
				}
			}else if(ct == ComboType.FULLEXCELLENT && prec > Precision.EXCELLENT){
				if(	prec == Precision.GREAT){ 
					firstGreat = timetotalchart;
					ct = ComboType.FULLCOMBO; 
				}
				else { 
					firstGreat = timetotalchart;
					firstMisteak = timetotalchart;
					ct = ComboType.NONE; 
				}
			}else if(ct == ComboType.FULLCOMBO && prec > Precision.GREAT){
				firstMisteak = timetotalchart;
				ct = ComboType.NONE;
			}
		}
		thetab = comboDecoupe();
		if(isFullExComboRace && ct == ComboType.FULLCOMBO){
			//Debug.Log("Fail at FEC race");
			fail = true;
		}
	}
	
	int[] scoreDecoupe(){
		var outtab = new int[5];
		var thescorestring = score.ToString("000.00").Replace(".","");
		for(int i=0;i<5;i++){
			outtab[i] = System.Convert.ToInt32(""+thescorestring[4-i]);
		}
		
		/*
		outtab[1] = System.Convert.ToInt32(""+thescorestring[3]);
		outtab[2] = System.Convert.ToInt32(""+thescorestring[2]);
		outtab[3] = System.Convert.ToInt32(""+thescorestring[1]);
		outtab[4] = System.Convert.ToInt32(""+thescorestring[0]);*/
		
		return outtab;
		
		//return new Rect(roolInt("0." + thescorestringentier[0]), roolInt("0." + thescorestringentier[1]), roolInt("0." + thescorestringdecim[0]), roolInt("0." + thescorestringdecim[1]));
	}
	
	
	int[] comboDecoupe(){
		var thecombo = combo.ToString();
		var outtab = new int[thecombo.Length];
		for(int i=0;i<thecombo.Length;i++){
			outtab[i] = System.Convert.ToInt32(""+thecombo[thecombo.Length-1-i]);
		}
		return outtab;
	}
	/*
	float roolInt(string i){
		var ic = (float)Convert.ToDouble(i);
		if(ic == 0.9f){
			return 0f;	
		}else{
			return ic+0.1f;		
		}
	}
	*/
	public void ComboStop(bool miss){
		
		if(!timeCombo.ContainsKey(timetotalchart)) timeCombo.Add(timetotalchart, combo);
		combo = 0;	
		if(ct != ComboType.NONE){
			switch(ct){
			case ComboType.FULLFANTASTIC:
				firstEx = timetotalchart;
				firstGreat = timetotalchart;
				firstMisteak = timetotalchart;
				break;
			case ComboType.FULLEXCELLENT:
				firstGreat = timetotalchart;
				firstMisteak = timetotalchart;
				break;
			case ComboType.FULLCOMBO:
				firstMisteak = timetotalchart;
				break;
			}
				
			ct = ComboType.NONE;
		}
		
		thetab = comboDecoupe();
		if(isFullComboRace || isFullExComboRace){
			//Debug.Log ("Fail at combo race");
			fail = true;
		}
		if(miss){
			comboMisses++;
		}else{
			comboMisses = 0;
		}
	}
	
	
	void SendDataToDatamanager(){
		DataManager.Instance.scoreEarned = score;
		DataManager.Instance.precAverage = precAverage;
		timeCombo.Add(timetotalchart, combo);
		DataManager.Instance.timeCombo = timeCombo;
		DataManager.Instance.lifeGraph = lifeGraph;
		DataManager.Instance.firstEx = firstEx;
		DataManager.Instance.firstGreat = firstGreat;
		DataManager.Instance.firstMisteak = firstMisteak;
		DataManager.Instance.perfect = perfect;
		DataManager.Instance.fullFantCombo = fullFantCombo;
		DataManager.Instance.fullExCombo = fullExCombo;
		DataManager.Instance.fullCombo = fullCombo;
		DataManager.Instance.scoreCount = scoreCount;
		DataManager.Instance.fail = fail;
		DataManager.Instance.firstArrow = firstArrow;
	}
	
	IEnumerator swipTexture(bool reverse, float height, float wait){
		
		if(!reverse){
			zwip = height/2f;
			while(zwip > 0f){
				zwip -= height/2f*Time.deltaTime/speedziwp;
				yield return new WaitForFixedUpdate();
			}
			zwip = 0f;
		}else{
			yield return new WaitForSeconds(wait);
			zwip = 0f;
			while(zwip < height/2f){
				zwip += height/2f*Time.deltaTime/speedziwp;
				yield return new WaitForFixedUpdate();
			}
			zwip = height/2f;
			cacheFailed = true;
			//Lancer la scène de score
		}
	}
	#endregion
	
	#region Precision
	double precToTime(Precision prec){
		if(prec <= Precision.WAYOFF){
			return DataManager.Instance.PrecisionValues[prec];	
		}
		return 0;
		/*switch(prec){
		case Precision.FANTASTIC:
			return 0.0215;
		case Precision.EXCELLENT:
			return 0.043;
		case Precision.GREAT:
			return 0.102;
		case Precision.DECENT:
			return 0.135;
		case Precision.WAYOFF:
			return 0.180;
		default:
			return 0;
		}*/
	}
	
	
	public void displayPrec(double prec){
		timeDisplayScore = 0f;
		zoom = baseZoom;
		scoreToDisplay = timeToPrec(prec);
	}
	
	Precision timeToPrec(double prec){
	
		var theprec = DataManager.Instance.PrecisionValues.Where(c => (prec <= c.Value));
		if(theprec.Count() > 0) return theprec.First().Key;
		return Precision.MISS;
		
		/*
		if(prec <= 0.0215){
			return Precision.FANTASTIC;
		}else if(prec <= 0.043){
			return Precision.EXCELLENT;
		}else if(prec <= 0.102){
			return Precision.GREAT;
		}else if(prec <= 0.135){
			return Precision.DECENT;
		}else if(prec <= 0.180){
			return Precision.WAYOFF;
		}
		return Precision.MISS;*/
		
	}
	
	
	#endregion
	
	#region Arrow and time
	
	#region oldCode
	//Tester un simple first or default
	/*public Arrow findNextUpArrow(){

		return arrowUpList.FirstOrDefault(s => s.time == arrowUpList.Min(c => c.time));
			
	}
	
	public Arrow findNextDownArrow(){

		return arrowDownList.FirstOrDefault(s => s.time == arrowDownList.Min(c => c.time));
			
	}
	
	public Arrow findNextLeftArrow(){

		return arrowLeftList.FirstOrDefault(s => s.time == arrowLeftList.Min(c => c.time));
			
	}
	
	public Arrow findNextRightArrow(){

		return arrowRightList.FirstOrDefault(s => s.time == arrowRightList.Min(c => c.time));
			
	}
	
	public Arrow findNextUpMine(){

		return mineUpList.FirstOrDefault(s => s.time == mineUpList.Min(c => c.time));
			
	}
	
	public Arrow findNextDownMine(){

		return mineDownList.FirstOrDefault(s => s.time == mineDownList.Min(c => c.time));
			
	}
	
	public Arrow findNextLeftMine(){

		return mineLeftList.FirstOrDefault(s => s.time == mineLeftList.Min(c => c.time));
			
	}
	
	public Arrow findNextRightMine(){

		return mineRightList.FirstOrDefault(s => s.time == mineRightList.Min(c => c.time));
			
	}*/
	#endregion
	
	public Arrow findNextUpArrow(){

		return arrowUpList.FirstOrDefault();
			
	}
	
	public Arrow findNextDownArrow(){

		return arrowDownList.FirstOrDefault();
			
	}
	
	public Arrow findNextLeftArrow(){

		return arrowLeftList.FirstOrDefault();
			
	}
	
	public Arrow findNextRightArrow(){

		return arrowRightList.FirstOrDefault();
			
	}
	
	public Arrow findNextUpMine(){

		return mineUpList.FirstOrDefault();
			
	}
	
	public Arrow findNextDownMine(){

		return mineDownList.FirstOrDefault();
			
	}
	
	public Arrow findNextLeftMine(){

		return mineLeftList.FirstOrDefault();
			
	}
	
	public Arrow findNextRightMine(){

		return mineRightList.FirstOrDefault();
			
	}
	//Remove key from arrow list
	public void removeArrowFromList(Arrow ar, string state){
		
		switch(state){
			case "left":
				arrowLeftList.Remove(ar);
				break;
			case "down":
				arrowDownList.Remove(ar);
				break;
			case "up":
				arrowUpList.Remove(ar);
				break;
			case "right":
				arrowRightList.Remove(ar);
				break;
				
		}
	}
	
	public void removeMineFromList(Arrow ar, string state){
		
		switch(state){
			case "left":
				mineLeftList.Remove(ar);
				break;
			case "down":
				mineDownList.Remove(ar);
				break;
			case "up":
				mineUpList.Remove(ar);
				break;
			case "right":
				mineRightList.Remove(ar);
				break;
				
		}
	}
	
	
	public double getTotalTimeChart(){
		return timetotalchart;	
	}
	
	#endregion
	
	#region Chart creation
	//Create the chart
	void createTheChart(Song s){
		
		var ypos = 0f;
		arrowLeftList = new List<Arrow>();
		arrowUpList = new List<Arrow>();
		arrowDownList = new List<Arrow>();
		arrowRightList = new List<Arrow>();
		mineLeftList = new List<Arrow>();
		mineUpList = new List<Arrow>();
		mineDownList = new List<Arrow>();
		mineRightList = new List<Arrow>();
		
		var theBPMCounter = 1;
		var theSTOPCounter = 0;
		double mesurecount = 0;
		double prevmesure = 0;
		double timecounter = 0;
		double timeBPM = 0;
		double timestop = 0;
		double timetotal = 0;
		float prec = 0.001f;
		var ArrowFreezed = new Arrow[4];
		Bumps = new List<double>();
		foreach(var mesure in s.stepchart){
			
			for(int beat=0;beat<mesure.Count;beat++){
			
			
				var bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
				if(theBPMCounter < s.bpms.Count && theSTOPCounter < s.stops.Count){
					while((theBPMCounter < s.bpms.Count && theSTOPCounter < s.stops.Count) 
						&& (s.mesureBPMS.ElementAt(theBPMCounter) < mesurecount - prec || s.mesureSTOPS.ElementAt(theSTOPCounter) < mesurecount - prec)){
					
						if(s.mesureBPMS.ElementAt(theBPMCounter) < s.mesureSTOPS.ElementAt(theSTOPCounter)){
							timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							
							timeBPM += timecounter;
							timecounter = 0;
							prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
							theBPMCounter++;
							bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
							//Debug.Log("And bpm change before / bpm");
						}else if(s.mesureBPMS.ElementAt(theBPMCounter) > s.mesureSTOPS.ElementAt(theSTOPCounter)){
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And stop change before");
						}else{
							timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							timeBPM += timecounter;
							timecounter = 0;
							prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
							theBPMCounter++;
							bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
							
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And bpm change before");
							//Debug.Log("And stop change before");
						}
						
					}
				}else if(theBPMCounter < s.bpms.Count){
					while((theBPMCounter < s.bpms.Count) && s.mesureBPMS.ElementAt(theBPMCounter) < mesurecount - prec){
						
						timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							
						timeBPM += timecounter;
						timecounter = 0;
						prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
						theBPMCounter++;
						bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
					
					}
				}else if(theSTOPCounter < s.stops.Count){
					while((theSTOPCounter < s.stops.Count) && s.mesureSTOPS.ElementAt(theSTOPCounter) < mesurecount - prec){
						
						timestop += s.stops.ElementAt(theSTOPCounter).Value;
						theSTOPCounter++;
					
					}
				}
				
				
				timecounter += (mesurecount - prevmesure)/bps;
				
				
				timetotal = timecounter + timeBPM + timestop;
				if((beat)%(mesure.Count/4) == 0) Bumps.Add(timetotal);
				
				var previousnote = mesure.ElementAt(beat).Trim();
				
				
				//Traitement
				if(displayValue[0]){ //No mine
					previousnote = previousnote.Replace('M', '0');
				}
				if(displayValue[1]){ //No jump
					if(previousnote.Where( c => c == '1' || c == '2' || c == '4').Count() == 2){
						var done = false;
						for(int i=0; i<4; i++){
							if(previousnote.ElementAt(i) == '1' || 	previousnote.ElementAt(i) == '2' || previousnote.ElementAt(i) == '4'){
								if(done){
									previousnote = previousnote.Remove(i, 1);
									previousnote = previousnote.Insert(i,"0");
								}else{
									done = true;	
								}
							}
						}
					}
				}
				if(displayValue[2]){ //No hands
					if(previousnote.Where( c => c == '1' || c == '2' || c == '4').Count() >= 3){
						var done = false;
						for(int i=0; i<4; i++){
							if(previousnote.ElementAt(i) == '1' || 	previousnote.ElementAt(i) == '2' || previousnote.ElementAt(i) == '4'){
								if(done){
									previousnote = previousnote.Remove(i, 1);
									previousnote = previousnote.Insert(i,"0");
								}else{
									done = true;	
								}
							}
						}
					}
				}
				
				if(displayValue[3]){ //No freeze
					previousnote = previousnote.Replace('2', '1');
					previousnote = previousnote.Replace('4', '1');
					previousnote = previousnote.Replace('3', '0');
				}
				
				char[] note = previousnote.ToCharArray();
				
				var listNeighboors = new List<Arrow>();
				var barr = false;
				for(int i =0;i<4; i++){
					if(note[i] == '1'){
						
						barr = true;
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						if(DataManager.Instance.skinSelected == 1 || DataManager.Instance.skinSelected == 3){
							for(int j=0; j<goArrow.transform.childCount;j++){
								if(goArrow.transform.GetChild(j).name.Contains("Deco")){
									goArrow.transform.GetChild(j).renderer.material.color = chooseColor(beat + 1, mesure.Count);	
								}
							}
						}else{
							goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						}
						var theArrow = new Arrow(goArrow, ArrowType.NORMAL, timetotal);
						
						switch(i){
						case 0:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, 90f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, 90f);
							}
							theArrow.arrowPos = ArrowPosition.LEFT;
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, 180f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, 180f);
							}
							theArrow.arrowPos = ArrowPosition.DOWN;
							arrowDownList.Add(theArrow);
							break;
						case 2:
							theArrow.arrowPos = ArrowPosition.UP;
							arrowUpList.Add(theArrow);
							break;
						case 3:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, -90f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, -90f);
							}
							theArrow.arrowPos = ArrowPosition.RIGHT;
							arrowRightList.Add(theArrow);
							break;
						}
						listNeighboors.Add(theArrow);
						goArrow.SetActiveRecursively(false);
						gameObject.GetComponent<ManageGameObject>().Add(timetotal, goArrow);
						//barrow = true;
					}else if(note[i] == '2'){
						barr = true;
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						if(DataManager.Instance.skinSelected == 1 || DataManager.Instance.skinSelected == 3){
							for(int j=0; j<goArrow.transform.childCount;j++){
								if(goArrow.transform.GetChild(j).name.Contains("Deco")){
									goArrow.transform.GetChild(j).renderer.material.color = chooseColor(beat + 1, mesure.Count);	
								}
							}
						}else{
							goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						}
						var theArrow = new Arrow(goArrow, ArrowType.FREEZE, timetotal);
						ArrowFreezed[i] = theArrow;
						switch(i){
						case 0:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, 90f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, 90f);
							}
							theArrow.arrowPos = ArrowPosition.LEFT;
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, 180f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, 180f);
							}
							theArrow.arrowPos = ArrowPosition.DOWN;
							arrowDownList.Add(theArrow);
							break;
						case 2:
							theArrow.arrowPos = ArrowPosition.UP;
							arrowUpList.Add(theArrow);
							break;
						case 3:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, -90f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, -90f);
							}
							theArrow.arrowPos = ArrowPosition.RIGHT;
							arrowRightList.Add(theArrow);
							break;
						}
						listNeighboors.Add(theArrow);
						goArrow.SetActiveRecursively(false);
						GetComponent<ManageGameObject>().Add(timetotal, goArrow);
					}else if(note[i] == '3'){
						barr = true;
						var theArrow = ArrowFreezed[i];
						if(ArrowFreezed[i] != null){
							var goFreeze = (GameObject) Instantiate(freeze, new Vector3(i*2, (theArrow.goArrow.transform.position.y + ((-ypos - theArrow.goArrow.transform.position.y)/2f)) , 1f), freeze.transform.rotation);
							goFreeze.transform.localScale = new Vector3(1f, -((-ypos - theArrow.goArrow.transform.position.y)/2f), 0.1f);
							if(DataManager.Instance.skinSelected == 1 || DataManager.Instance.skinSelected == 3){
							for(int j=0; j<theArrow.goArrow.transform.childCount;j++){
								if(theArrow.goArrow.transform.GetChild(j).name.Contains("Deco")){
									goFreeze.transform.GetChild(0).renderer.material.color = theArrow.goArrow.transform.GetChild(j).renderer.material.color;
									j = theArrow.goArrow.transform.childCount;
								}
							}
							}else{
								goFreeze.transform.GetChild(0).renderer.material.color = theArrow.goArrow.renderer.material.color;
							}
							theArrow.setArrowFreeze(timetotal, new Vector3(i*2,-ypos, 0f), goFreeze, null);
						}
						
						ArrowFreezed[i] = null;
					
					}else if(note[i] == '4'){
						barr = true;
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						if(DataManager.Instance.skinSelected == 1 || DataManager.Instance.skinSelected == 3){
							for(int j=0; j<goArrow.transform.childCount;j++){
								if(goArrow.transform.GetChild(j).name.Contains("Deco")){
									goArrow.transform.GetChild(j).renderer.material.color = chooseColor(beat + 1, mesure.Count);	
								}
							}
						}else{
							goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						}
						var theArrow = new Arrow(goArrow, ArrowType.ROLL, timetotal);
						ArrowFreezed[i] = theArrow;
						switch(i){
						case 0:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, 90f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, 90f);
							}
							theArrow.arrowPos = ArrowPosition.LEFT;
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, 180f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, 180f);
							}
							theArrow.arrowPos = ArrowPosition.DOWN;
							arrowDownList.Add(theArrow);
							break;
						case 2:
							theArrow.arrowPos = ArrowPosition.UP;
							arrowUpList.Add(theArrow);
							break;
						case 3:
							if(DataManager.Instance.skinSelected == 1){
								theArrow.goArrow.transform.Rotate(0f, 0f, -90f);
							}else if(DataManager.Instance.skinSelected == 3){
								theArrow.goArrow.transform.FindChild("RotationCenter").Rotate(0f, 0f, -90f);
							}
							theArrow.arrowPos = ArrowPosition.RIGHT;
							arrowRightList.Add(theArrow);
							break;
						}
						listNeighboors.Add(theArrow);
						goArrow.SetActiveRecursively(false);
						GetComponent<ManageGameObject>().Add(timetotal, goArrow);
					}else if(note[i] == 'M'){
						var goArrow = (GameObject) Instantiate(mines, new Vector3(i*2, -ypos, 0f), mines.transform.rotation);
						var theArrow = new Arrow(goArrow, ArrowType.MINE, timetotal);
						switch(i){
						case 0:
							theArrow.arrowPos = ArrowPosition.LEFT;
							mineLeftList.Add(theArrow);
							break;
						case 1:
							theArrow.arrowPos = ArrowPosition.DOWN;
							mineDownList.Add(theArrow);
							break;
						case 2:
							theArrow.arrowPos = ArrowPosition.UP;
							mineUpList.Add(theArrow);
							break;
						case 3:
							theArrow.arrowPos = ArrowPosition.RIGHT;
							mineRightList.Add(theArrow);
							break;
						}
						
					
					}
					
				}
				
				if(barr){
					if(firstArrow <= 0f) firstArrow = timetotal;
					lastArrow = timetotal;	
				}
				
				if(listNeighboors.Count > 1){
					foreach(var el in listNeighboors){
						el.neighboors = listNeighboors;
						el.imJump = true;
					}
				}
	
				if(theBPMCounter < s.bpms.Count){
					if(Mathf.Abs((float)(s.mesureBPMS.ElementAt(theBPMCounter) - mesurecount)) < prec){
							timeBPM += timecounter;
							timecounter = 0;
							theBPMCounter++;
							//Debug.Log("And bpm change");
					}
				}
				
				if(theSTOPCounter < s.stops.Count){
					if(Mathf.Abs((float)(s.mesureSTOPS.ElementAt(theSTOPCounter) - mesurecount)) < prec){
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And stop");
					}
				}
				prevmesure = mesurecount;
				mesurecount += (4f/(float)mesure.Count);
				
				
				ypos += (4f/(float)mesure.Count)*speedmod;
				
			}
		}
		
		//Sort
		arrowUpList = arrowUpList.OrderBy(c => c.time).ToList();
		arrowDownList = arrowDownList.OrderBy(c => c.time).ToList();
		arrowRightList = arrowRightList.OrderBy(c => c.time).ToList();
		arrowLeftList = arrowLeftList.OrderBy(c => c.time).ToList();
		
		mineUpList = mineUpList.OrderBy(c => c.time).ToList();
		mineDownList = mineDownList.OrderBy(c => c.time).ToList();
		mineRightList = mineRightList.OrderBy(c => c.time).ToList();
		mineLeftList = mineLeftList.OrderBy(c => c.time).ToList();
		
		GetComponent<ManageGameObject>().DoTheStartSort();
	}
	
	
	
	Color chooseColor(int posmesure, int mesuretot){
		
		
		switch(mesuretot){
		case 4:	
			return new Color(1f, 0f, 0f, 1f); //rouge
		
			
		case 8:
			if(posmesure%2 == 0){
				return new Color(0f, 0f, 1f, 1f); // bleu
			}
			return new Color(1f, 0f, 0f, 1f); //rouge
			
			
		case 12:
			if((posmesure-1)%3 == 0){
				return new Color(1f, 0f, 0f, 1f); //rouge
			}
			return new Color(1f, 0f, 1f, 1f); //violet
			
		case 16:
			if((posmesure-1)%4 == 0){
				return new Color(1f, 0f, 0f, 1f); //rouge
			}else if((posmesure+1)%4 == 0){
				return new Color(0f, 0f, 1f, 1f); //bleu
			}
			return new Color(0f, 1f, 0f, 1f); //vert
		
			
		case 24:
			if((posmesure+2)%6 == 0){
				return new Color(0f, 0f, 1f, 1f); //bleu
			}else if((posmesure-1)%6 == 0){
				return new Color(1f, 0f, 0f, 1f); //rouge
			}
			return new Color(1f, 0f, 1f, 1f); //violet
			
			
		case 32:
			if((posmesure-1)%8 == 0){
				return new Color(1f, 0f, 0f, 1f); //rouge
			}else if((posmesure+3)%8 == 0){
				return new Color(0f, 0f, 1f, 1f); //bleu
			}else if((posmesure+1)%4 == 0){
				return new Color(0f, 1f, 0f, 1f); //vert
			}
			return new Color(1f, 0.8f, 0f, 1f); //jaune
			
		case 48:
			if((posmesure+2)%6 == 0){
				return new Color(0f, 1f, 0f, 1f); //vert
			}else if((posmesure-1)%12 == 0){
				return new Color(1f, 0f, 0f, 1f); //rouge
			}else if((posmesure+5)%12 == 0){
				return new Color(0f, 0f, 1f, 1f); //bleu
			}
			return new Color(1f, 0f, 1f, 1f); // violet
		
			
		case 64:
			if((posmesure-1)%16 == 0){
				return new Color(1f, 0f, 0f, 1f); //rouge
			}else if((posmesure+7)%16 == 0){
				return new Color(0f, 0f, 1f, 1f); //bleu
			}else if((posmesure+3)%8 == 0){
				return new Color(0f, 1f, 0f, 1f); //vert
			}else if((posmesure+1)%4 == 0){
				return new Color(1f, 0.8f, 0f, 1f); //jaune
			}
			return new Color(0f, 1f, 0.8f, 1f); //turquoise
			
		case 192:
			if((posmesure-1)%48 == 0){
				return new Color(1f, 0f, 0f, 1f); //rouge
			}else if((posmesure+13)%48 == 0){
				return new Color(0f, 0f, 1f, 1f); //bleu
			}else if((posmesure+11)%24 == 0){
				return new Color(0f, 1f, 0f, 1f); //vert
			}else if((posmesure-1)%4 == 0){
				return new Color(1f, 0f, 1f, 1f); //violet
			}else if((posmesure+5)%12 == 0){
				return new Color(1f, 0.8f, 0f, 1f); //jaune
			}
			return new Color(0f, 1f, 0.8f, 1f); //turquoise
			
		default:
			return new Color(1f, 1f, 1f, 1f);
			
		}
	
		
	}
	
	
	
	#endregion
}
