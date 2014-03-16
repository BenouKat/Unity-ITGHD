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
	public GameObject roll;
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
	public GameObject timeBar;
	public Camera particleComboCam;
	
	private LifeBar theLifeBar;
	private TimeBar theTimeBar;
	
	private Song thesong;
	
	//USED FOR UPDATE FUNCTION
	private double timebpm; //Temps joué sur la totalité, non live, non remise à 0
	private float timechart; //Temps joué sur le bpm actuel en live avec remise à 0
	private float timestop; //Temps utilisé pour le freeze
	private float totaltimestop; //Temps total à être stoppé
	private double timetotalchart; //Temps joué sur la totalité en live (timebpm + timechart)
	private float changeBPM; //Position en y depuis un changement de bpm (nouvelle reference)
	private double keybpms;
	private double valuebpms;
	private double keystops;
	private double valuestops;
	private int countBPM;
	private int countSTOP;
	
	private int nextSwitchBPM; //Prochain changement de bpm
	private int nextSwitchStop;
	
	private double actualBPM; //Bpm actuel
	
	private double actualstop;
	private float speedmod; //speedmod 
	public float speedmodRate = 2f; //speedmod ajustation 
	private float rateSelected;
	
	private double bpsForActualFrame;
	private float moveForActualFrame;
	private Vector3 posMoveArrow;
	private float divMoveArrow;
	
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
	
	
	private float zoom;
	private Precision scoreToDisplay;
	
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
	
	//PassToDataManager
	private List<double> precAverage;
	private List<double> timeOfCombo;
	private List<int> numberCombo;
	private Dictionary<double, int> timeCombo;
	private List<double> timeOfLife;
	private List<double> numberLife;
	private Dictionary<double, double> lifeGraph;
	public double firstEx;
	public double firstGreat;
	public double firstMisteak;
	
	//CHART
	private List<Arrow> arrowLeftList;
	private List<Arrow> arrowRightList;
	private List<Arrow> arrowDownList;
	private List<Arrow> arrowUpList;
	private List<Arrow> mineLeftList;
	private List<Arrow> mineRightList;
	private List<Arrow> mineDownList;
	private List<Arrow> mineUpList;
	private List<Arrow> keyToRemove;
	private List<Arrow> trash;
	private Arrow[] keyToCheck;
	//Dico des arrow prises en freeze
	private Dictionary<Arrow, float> arrowFrozen;
	private double prec;
	private double realprec;
	private Arrow arrowSelected;
	private Precision ttp;
	
	//PARTICLE SYSTEM
	
	private Dictionary<string, ParticleSystem> precLeft;
	private Dictionary<string, ParticleSystem> precRight;
	private Dictionary<string, ParticleSystem> precUp;
	private Dictionary<string, ParticleSystem> precDown;
	private Dictionary<string, ParticleSystem> clearcombo;
	
	
	
	//GUI
	
	private string speedmodstring;
	private float speedmodSelected;
	private bool speedmodok;
	private string infoToDisplay;
	private Color white = new Color(1f, 1f, 1f, 1f);
	private KeyValuePair<Precision, double> precFounded;
	
	
	//NEW GUI
	public UICamera cameraUI;
	public UILabel comboLabel;
	public Color dangerColor;
	public Color dangerOutlineColor;
	public Color FFCColor;
	public Color FFCOutlineColor;
	public Color FPCColor;
	public Color FPCOutlineColor;
	public Color maxColor;
	public Color maxOutlineColor;
	public Color normalColor;
	public Color normalOutlineColor;
	public UISprite HUDScore;
	public UISprite HUDCombo;
	public UILabel scoreLabel;
	public UILabel scoreBeatLabel;
	public Color normalbeatColor;
	public Color idlebeatColor;
	public Color beatthepersonalColor;
	public Color beatthebeatColor;
	private double persoRecordScore;
	private double recordScore;
	private string recordName;
	public UISprite judgeSprite;
	public Vector3[] baseScaleJudgeSprite;
	private Vector3 actualBaseScaleJudge;
	public float coefficientScaleJudge;
	public float zoomPower;
	public float speedZoomDecrease;
	private float actualZoom;
	public UISprite clearFail;
	private Vector3 normalScaleClearFail;
	private float scaleProgressClearFail;
	public float speedScaleClearFail;
	public UISprite blackSprite;
	private float alphaBlackSprite;
	public float speedAlphaBlackSprite;
	public float limitDisplayScore = 2f;
	private float timeDisplayScore = 0f;
	public float speedClignotementFantastic = 20f;
	public float alphaFantasticLimit = 0.8f;
	public float speedClignotementFail = 20f;
	public float alphaFailLimit = 0.9f;
	private bool nojudge;
	public float TEMPO_FailAppear;
	public float TEMPO_BlackAppear;
	public float TEMPO_ButtonAppear;
	public UILabel LabelDescriptionSong;
	public UILabel LabelDescriptionMaster;
	public UISprite SpriteLevelText;
	public Vector3[] baseScaleLevelSprite;
	public float coefficientScaleLevel;
	public UILabel LabelLevelNumber;
	public GameObject buttonRetry;
	private UISprite bkgrRetry;
	private UILabel lblRetry;
	public GameObject buttonGiveUp;
	private UISprite bkgrGiveUp;
	private UILabel lblGiveUp;
	public GameObject inputSpeedmod;
	private UISlicedSprite bkgrInput;
	private UILabel lblInput;
	private UIInput theUIInput;
	public UILabel labelSpeedmod;
	private float alphaButtonDie;
	public float speedAlphaButtonDie;
	public UISprite FullComboSprite;
	public Vector3[] baseScaleFCSprite;
	public float coefficientScaleFCSprite;
	public UISprite fastSprite;
	public UISprite slowSprite;
	public float speedFastSlowAlpha;
	
	
	//DISPLAY
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
	private ComboType ct;
	private int comboMisses;
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
	public float timeClearDisappear;
	private bool appearClearok;
	public float speedFadeAudio;
	private bool deadAndRetry;
	private bool deadAndGiveUp;
	
	//SOUND
	private AudioClip songLoaded;
	public AudioClip failedSound;
	public AudioClip comboSound;
	public AudioSource secondAudioSource;
	public AudioSource mainAudioSource;
	
	//POOL
	private Color poolColor = new Color(0f, 0f, 0f, 0f);
	private Vector3 poolVector = new Vector3(0f, 0f, 0f);
	private Vector2 poolVector2 = new Vector2(0f, 0f);
	private float poolFloat = 0f;
	private KeyValuePair<Arrow, float> poolArrow;
	private ParticleSystem poolPS;
	
	//NETWORK
	private bool inNetworkMode;
	private NetworkChartScript ncs;
	
	//DEBUG
	//private int iwashere;
	#endregion
	
	
	//Start
	void Start () {
		
		//Data from option
		speedmod = DataManager.Instance.speedmodSelected*speedmodRate;
		rateSelected = DataManager.Instance.rateSelected;
		DataManager.Instance.LoadScoreJudge(DataManager.Instance.scoreJudgeSelected);
		DataManager.Instance.LoadHitJudge(DataManager.Instance.hitJudgeSelected);
		DataManager.Instance.LoadLifeJudge(DataManager.Instance.lifeJudgeSelected);
		isFullComboRace = DataManager.Instance.raceSelected == 9;
		isFullExComboRace = DataManager.Instance.raceSelected == 10;
		targetScoreInverse = DataManager.Instance.giveTargetScoreOfRace(DataManager.Instance.raceSelected);
		typeOfDeath = DataManager.Instance.deathSelected;
		persoRecordScore = DataManager.Instance.personalScore;
		recordScore = DataManager.Instance.recordScore;
		recordName = DataManager.Instance.recordName;
		KeyCodeDown = DataManager.Instance.KeyCodeDown;
		KeyCodeUp = DataManager.Instance.KeyCodeUp;
		KeyCodeLeft = DataManager.Instance.KeyCodeLeft;
		KeyCodeRight = DataManager.Instance.KeyCodeRight;
		SecondaryKeyCodeDown = DataManager.Instance.SecondaryKeyCodeDown;
		SecondaryKeyCodeDown = DataManager.Instance.SecondaryKeyCodeDown;
		SecondaryKeyCodeUp = DataManager.Instance.SecondaryKeyCodeUp;
		SecondaryKeyCodeLeft = DataManager.Instance.SecondaryKeyCodeLeft;
		SecondaryKeyCodeRight = DataManager.Instance.SecondaryKeyCodeRight;
		speedmodSelected = DataManager.Instance.speedmodSelected;
		speedmodstring = DataManager.Instance.speedmodSelected.ToString("0.00");
		speedmodok = true;
		var rand = (int)(UnityEngine.Random.value*DataManager.Instance.skyboxList.Count);
		if(rand == DataManager.Instance.skyboxList.Count){
			rand--;	
		}
		inNetworkMode = (Application.loadedLevelName == "LANChartScene");
		if(inNetworkMode)
		{
			Debug.Log("NETWORK MODE");
			ncs = GetComponent<NetworkChartScript>();
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
		modelskin.SetActive(false);
		
		arrowLeft = (GameObject) Instantiate(modelskin, new Vector3(0f, 0f, 2f), modelskin.transform.rotation);
		if(DataManager.Instance.skinSelected == 3){
			arrowLeft.transform.FindChild("RotationCenter").Rotate(0f, 0f, 90f);
		}else if(DataManager.Instance.skinSelected != 0){
			arrowLeft.transform.Rotate(0f, 0f, 90f);
			arrowLeft.transform.FindChild("ParticulePrec").Rotate(0f, 0f, -90f);
			arrowLeft.transform.FindChild("ParticulePrec").localPosition = new Vector3(-1f, 0f, 0f);
		}
		arrowLeft.SetActive(true);
		arrowLeft.transform.parent = MainCamera.gameObject.transform;
		
		arrowRight = (GameObject) Instantiate(modelskin, new Vector3(6f, 0f, 2f), modelskin.transform.rotation);
		if(DataManager.Instance.skinSelected == 3){
			arrowRight.transform.FindChild("RotationCenter").Rotate(0f, 0f, -90f);
		}else if(DataManager.Instance.skinSelected == 1){
			arrowRight.transform.Rotate(0f, 0f, -90f);
			arrowRight.transform.FindChild("ParticulePrec").Rotate(0f, 0f, 90f);
			arrowRight.transform.FindChild("ParticulePrec").localPosition = new Vector3(1f, 0f, 0f);
		}
		arrowRight.SetActive(true);
		arrowRight.transform.parent = MainCamera.gameObject.transform;
		
		arrowDown = (GameObject) Instantiate(modelskin, new Vector3(2f, 0f, 2f), modelskin.transform.rotation);
		if(DataManager.Instance.skinSelected == 3){
			arrowDown.transform.FindChild("RotationCenter").Rotate(0f, 0f, 180f);
		}else if(DataManager.Instance.skinSelected == 1){
			arrowDown.transform.Rotate(0f, 0f, 180f);
			arrowDown.transform.FindChild("ParticulePrec").Rotate(0f, 0f, -180f);
			arrowDown.transform.FindChild("ParticulePrec").localPosition = new Vector3(0f, 1f, 0f);
		}
		arrowDown.SetActive(true);
		arrowDown.transform.parent = MainCamera.gameObject.transform;
		
		arrowUp = (GameObject) Instantiate(modelskin, new Vector3(4f, 0f, 2f), modelskin.transform.rotation);
		arrowUp.SetActive(true);
		arrowUp.transform.parent = MainCamera.gameObject.transform;
		
		arrowTarget = new Transform[4];
		arrowTarget[0] = arrowLeft.transform;
		arrowTarget[1] = arrowDown.transform;
		arrowTarget[2] = arrowUp.transform;
		arrowTarget[3] = arrowRight.transform;
		scaleBase = arrowTarget[0].localScale.x;
		matArrowModel = matSkinModel[DataManager.Instance.skinSelected];
		
		
		RenderSettings.skybox = DataManager.Instance.skyboxList.ElementAt(rand);
		RenderSettings.skybox.SetColor("_Tint", fillColor(0.35f, 0.35f, 0.35f, 0.5f));
		DataManager.Instance.skyboxIndexSelected = rand;
		displayValue = DataManager.Instance.displaySelected;
		
		firstArrow = -10f;
		lastArrow = -10f;
		thesong = (rateSelected == 0f ? DataManager.Instance.songSelected : DataManager.Instance.songSelected.getRatedSong(rateSelected));
		songLoaded = thesong.GetAudioClip();
		mainAudioSource.loop = false;
		createTheChart(thesong);
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 0;
		nextSwitchBPM = 1;
		nextSwitchStop = 0;
		actualBPM = thesong.bpms.First().Value;
		actualstop = (double)0;
		changeBPM = 0;
		
		timebpm = (double)0;
		timechart = 0f;//-(float)thesong.offset;
		timetotalchart = (double)0;
		
		countBPM = thesong.bpms.Count;
		countSTOP = thesong.stops.Count;
		
		
		arrowFrozen = new Dictionary<Arrow, float>();
		keyToRemove = new List<Arrow>();
		keyToCheck = new Arrow[4];
		for(int i=0; i<4; i++) keyToCheck[i] = null;
		
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
		for(int i=0; i< particleComboCam.transform.childCount; i++){
			clearcombo.Add(particleComboCam.transform.GetChild(i).name, particleComboCam.transform.GetChild(i).particleSystem);
		}
		
		precAverage = new List<double>();
		timeCombo = new Dictionary<double, int>();
		timeOfCombo = new List<double>();
		numberCombo = new List<int>();
		
		lifeGraph = new Dictionary<double, double>();
		timeOfLife = new List<double>();
		numberLife = new List<double>();
		
		TMainCamera = MainCamera.transform;
		MoveCameraBefore();
		
		
		//stuff
		scoreToDisplay = Precision.NONE;
		timeDisplayScore = Mathf.Infinity;
		scenechartfaded = false;
		
		
		
		//init score and lifebase
		scoreBase = new Dictionary<string, float>();
		scoreCount = new Dictionary<string, int>();
		lifeBase = new Dictionary<string, float>();
		fantasticValue = 100f/(thesong.numberOfStepsAbsolute + thesong.numberOfFreezes + thesong.numberOfRolls);
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
		//lifeGraph.Add(0, life);
		timeOfLife.Add(0);
		numberLife.Add(life);
		score = 0f;
		scoreInverse = 100f;
		firstEx = -1;
		firstGreat = -1;
		firstMisteak = -1;
		
		theLifeBar = lifeBar.GetComponent<LifeBar>();
		theTimeBar = timeBar.GetComponent<TimeBar>();
		
		//TimeBar
		
		theTimeBar.setDuration((float)firstArrow, (float)lastArrow);
		
		//var bps = thesong.getBPS(actualBPM);
		//changeBPM -= (float)(bps*thesong.offset)*speedmod;
		firstUpdate = true;
		oneSecond = 0f;
		
		startTheSong = (float)thesong.offset + DataManager.Instance.globalOffsetSeconds + DataManager.Instance.userGOS;
		
		//bump
		nextBump = 0;
		
		//combo
		ct = ComboType.FULLFANTASTIC;
		comboMisses = 0;
		
		scoreRefresh();
		comboRefresh();
		
		
		//Fail and clear
		fail = false;
		clear = false;
		fullCombo = false;
		fullExCombo = false;
		fullFantCombo = false;
		perfect = false;
		dead = false;
		appearClearok = false;
		
		
		//GUI
		var title = thesong.title;
		if(title.Length > 30) title = title.Remove(30, title.Length - 30) + "...";
		var artist = thesong.artist;
		if(artist.Length > 30) artist = artist.Remove(30, artist.Length - 30) + "...";
		var stepartist = thesong.stepartist;
		if(stepartist.Length > 30) stepartist = stepartist.Remove(30, stepartist.Length - 30) + "...";
		infoToDisplay = title + "\n" + artist + "\n"  + stepartist + "\n\n";
		
		LabelDescriptionSong.text = infoToDisplay;
		
		infoToDisplay = "";
		var mystat = ProfileManager.Instance.FindTheSongStat(thesong.sip);
		if(mystat != null)
		{
			if(mystat.fail)
			{
				infoToDisplay += "Never cleared\n";
			}else{
				infoToDisplay += "Personnal best : " + mystat.score.ToString("0.00") + "%\n";
			}
		}else{
			infoToDisplay += "First try\n";
		}
		
		if(!inNetworkMode){
			if(recordScore != -1)
			{
				LabelDescriptionMaster.text = "Best : " + recordName + " [" + recordScore.ToString("0.00") + "%]";
			}else
			{
				LabelDescriptionMaster.text = "Best : --";	
			}
			
		}
		
		LabelLevelNumber.text = thesong.level.ToString();
		LabelLevelNumber.color = DataManager.Instance.diffColor[(int)thesong.difficulty];
		
		
		
		
		
		SpriteLevelText.spriteName = Enum.GetName(typeof(Difficulty), (thesong.difficulty)).ToLower();
		SpriteLevelText.transform.localScale = baseScaleLevelSprite[(int)thesong.difficulty]*coefficientScaleLevel;
		
		if(!inNetworkMode){
			if(recordScore != -1)
			{
				scoreBeatLabel.text = persoRecordScore == -1 ? recordScore.ToString("00.00") + "%" : persoRecordScore.ToString("00.00") + "%";	
				scoreBeatLabel.effectColor = normalbeatColor;
			}else{
				scoreBeatLabel.text = "--%";
				scoreBeatLabel.effectColor = idlebeatColor;
			}
		}
		
		judgeSprite.enabled = false;
		
		
		
		clearFail.enabled = false;
		blackSprite.enabled = false;
		
		scaleProgressClearFail = 0f;
		alphaBlackSprite = 0f;
		
		bkgrGiveUp = buttonGiveUp.transform.FindChild("Background").GetComponent<UISprite>();
		lblGiveUp = buttonGiveUp.transform.FindChild("Label").GetComponent<UILabel>();
		
		bkgrRetry = buttonRetry.transform.FindChild("Background").GetComponent<UISprite>();
		lblRetry = buttonRetry.transform.FindChild("Label").GetComponent<UILabel>();
		
		bkgrInput = inputSpeedmod.transform.FindChild("Background").GetComponent<UISlicedSprite>();
		lblInput = inputSpeedmod.transform.FindChild("Label").GetComponent<UILabel>();
		theUIInput = inputSpeedmod.GetComponent<UIInput>();
		
		theUIInput.text = speedmodSelected.ToString("0.00");
		
		var bpmdisplaying = thesong.bpmToDisplay;
		if(bpmdisplaying.Contains("->")){
			bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[0])*speedmodSelected*(1f + (rateSelected/100f))).ToString("0") + "->" + (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[1])*speedmodSelected*(1f + (rateSelected/100f))).ToString("0");
		}else{
			bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying)*speedmodSelected*(1f + (rateSelected/100f))).ToString("0");
		}
		labelSpeedmod.text = "Speedmod : x" + speedmodSelected.ToString("0.00") + " (" + bpmdisplaying + " BPM)";

		
		Screen.showCursor = false;
		
		
		
		//Transformation display
		if(displayValue[4]){ //No judge
			limitDisplayScore = -1f;
			fastSprite.enabled = false;
			slowSprite.enabled = false;
		}
		
		if(displayValue[5]){ //No background
			RenderSettings.skybox = null;
			Background.GetComponent<MoveBackground>().enabled = false;
			Background.SetActive(false);
			
		}
		
		if(displayValue[6]){ //No target
			if(arrowLeft.transform.childCount > 1)
			{
				for(int i=0; i<arrowLeft.transform.childCount; i++)
				{
					if(arrowLeft.transform.GetChild(i).name != "ParticulePrec") 
						arrowLeft.transform.GetChild(i).renderer.enabled = false;
				}
				
				for(int i=0; i<arrowDown.transform.childCount; i++)
				{
					if(arrowDown.transform.GetChild(i).name != "ParticulePrec") 
						arrowDown.transform.GetChild(i).renderer.enabled = false;
				}
				
				for(int i=0; i<arrowUp.transform.childCount; i++)
				{
					if(arrowUp.transform.GetChild(i).name != "ParticulePrec") 
						arrowUp.transform.GetChild(i).renderer.enabled = false;
				}
				
				for(int i=0; i<arrowRight.transform.childCount; i++)
				{
					if(arrowRight.transform.GetChild(i).name != "ParticulePrec") 
						arrowRight.transform.GetChild(i).renderer.enabled = false;
				}
			}else{
				arrowLeft.renderer.enabled = false;
				arrowDown.renderer.enabled = false;
				arrowUp.renderer.enabled = false;
				arrowRight.renderer.enabled = false;
			}
			
		}
		
		//No score
		if(displayValue[7])
		{
			scoreLabel.enabled = false;
			scoreBeatLabel.enabled = false;
			HUDScore.enabled = false;
			LabelDescriptionMaster.enabled = false;
		}
		
		//No UI
		if(displayValue[8]){
			//Set renderer recursively disable
			for(int i=0;i<lifeBar.transform.childCount; i++){
				if(lifeBar.transform.GetChild(i).childCount > 0){
					for(int j=0;j<lifeBar.transform.GetChild(i).childCount; j++){
						if(lifeBar.transform.GetChild(i).GetChild(j).childCount > 0){
							for(int k=0;k<lifeBar.transform.GetChild(i).GetChild(j).childCount; k++){
								lifeBar.transform.GetChild(i).GetChild(j).GetChild(k).renderer.enabled = false;	
							}
						}else{
							lifeBar.transform.GetChild(i).GetChild(j).renderer.enabled = false;	
						}
					}
				}else{
					lifeBar.transform.GetChild(i).renderer.enabled = false;	
				}
				
			}
			for(int i=0;i<timeBar.transform.childCount; i++){
				timeBar.transform.GetChild(i).renderer.enabled = false;	
			}
			theTimeBar.disableTime();
			theLifeBar.disableLifeBar();
			comboLabel.enabled = false;
			HUDCombo.enabled = false;
		}
		
		
		
		GC.Collect();
	}
	
	
	void OnApplicationQuit()
	{
		if(!displayValue[5]) RenderSettings.skybox.SetColor("_Tint", fillColor(0.5f, 0.5f, 0.5f, 0.5f));
		DataManager.Instance.removeRatedSong();
	}
	
	
	// Update is called once per frame
	void Update () {
		
		if((!dead && oneSecond >= 1.5f) || clear){
			//timetotal for this frame
			
			timetotalchart = timebpm + timechart + totaltimestop;

			
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
				
					}else{
						totaltimestop += Time.deltaTime;
						timestop += Time.deltaTime;
					
					}

			}else{
				timechart += Time.deltaTime;

			}
			
			//GUI part
			RefreshGUIPart();
			
			//Start song
			timetotalchart = timebpm + timechart + totaltimestop;
			
			
			if(firstUpdate){
				if(startTheSong <= 0f){
					mainAudioSource.PlayOneShot(songLoaded);
					timechart += startTheSong; 
					timetotalchart = timebpm + timechart + totaltimestop;
					firstUpdate = false;
					MoveArrows();
				}else{
					startTheSong -= Time.deltaTime;	
				}
				
			}
			
			BumpsBPM();
			
			//Fail/Clear part
			if(thesong.duration < timetotalchart && (!(fail) || inNetworkMode) && !clear){
				if(!arrowLeftList.Any() && !arrowRightList.Any() && !arrowUpList.Any() && !arrowDownList.Any())
				{
					if(!inNetworkMode)
					{
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
							secondAudioSource.PlayOneShot(comboSound); 
						}
						oneSecond = 0f;
					}else{
						numberCombo.Add(combo);
						fail = false;
						clear = true;
						if(scoreCount["DECENT"] == 0 && scoreCount["WAYOFF"] == 0 && scoreCount["MISS"] == 0){
							if(score >= 100f || scoreInverse == 100f) perfect = true;
							if(scoreCount["EXCELLENT"] == 0 && scoreCount["GREAT"] == 0) fullFantCombo = true;
							if(scoreCount["GREAT"] == 0) fullExCombo = true;
							fullCombo = true;
							secondAudioSource.PlayOneShot(comboSound); 
						}
						oneSecond = 0f;
						var theaverage = 0f;
						for(int i = 0; i<precAverage.Count; i++)
						{
							theaverage += Mathf.Abs((float)precAverage[i]);
						}
						theaverage = precAverage.Count == 0 ? 0f : (theaverage/(float)precAverage.Count);
						if(LANManager.Instance.isCreator)
						{
							ncs.startVerifyFinish = true;
							ncs.hasFinished(Network.player, 
								scoreCount["FANTASTIC"] + ";" +
								scoreCount["EXCELLENT"] + ";" +
								scoreCount["GREAT"] + ";" +
								scoreCount["DECENT"] + ";" +
								scoreCount["WAYOFF"] + ";" +
								scoreCount["MISS"] + ";" +
								(firstMisteak/thesong.duration) + ";" +
								(int)theaverage + ";" +
								numberCombo.Max().ToString()
							);	
						}else{
							networkView.RPC("hasFinished", RPCMode.Server, Network.player, 
								scoreCount["FANTASTIC"] + ";" +
								scoreCount["EXCELLENT"] + ";" +
								scoreCount["GREAT"] + ";" +
								scoreCount["DECENT"] + ";" +
								scoreCount["WAYOFF"] + ";" +
								scoreCount["MISS"] + ";" +
								(firstMisteak/thesong.duration) + ";" +
								(int)theaverage + ";" +
								numberCombo.Max().ToString()
							);
						}
					}
				}
			}
			
			if(fail && !inNetworkMode){
				if((typeOfDeath != 2 && (typeOfDeath == 0 || comboMisses >= 30)) || thesong.duration < timetotalchart){
					dead = true;
					
					clearFail.enabled = true;
					clearFail.spriteName = "Fail";
					blackSprite.enabled = true;
					normalScaleClearFail = clearFail.transform.localScale;
					clearFail.transform.localScale = fillVector(clearFail.transform.localScale.x*10f, clearFail.transform.localScale.y/10f, clearFail.transform.localScale.z);
					clearFail.color = fillColor (clearFail.color.r, clearFail.color.g, clearFail.color.b, 0f);
					buttonRetry.SetActive(true);
					buttonGiveUp.SetActive(true);
					inputSpeedmod.SetActive(true);
					labelSpeedmod.enabled = true;
					judgeSprite.enabled = false;
					fastSprite.enabled = false;
					slowSprite.enabled = false;
					cameraUI.enabled = true;
					Screen.showCursor = true;
					
					mainAudioSource.Stop ();
					mainAudioSource.volume = 1f;
					mainAudioSource.PlayOneShot(failedSound);
					secondAudioSource.volume = 0f;
					secondAudioSource.loop = true;
					secondAudioSource.Play();
					Background.GetComponent<MoveBackground>().enabled = false;
					CameraBackground.GetComponent<MoveCameraBackground>().enabled = false;
					TMainCamera.GetComponent<GrayscaleEffect>().enabled = true;
					oneSecond = 0f;
					GetComponent<DebugOffset>().enabled = true;
				}
			}else if(clear){
				if(!inNetworkMode || (inNetworkMode && ncs.readyFinish)) oneSecond += Time.deltaTime;
				if(mainAudioSource.volume > 0) mainAudioSource.volume -= Time.deltaTime/speedFadeAudio;
					
				if(!appearClearok){
					
					//FC / FEC / FFC
					var contains = perfect ? "Perfect" : (fullFantCombo ? "FFC" : (fullExCombo ? "FEC" : ( fullCombo ? "FBC" : "noPS")) );
					if(!contains.Contains("noPS")){
						FullComboSprite.enabled = true;
						FullComboSprite.spriteName = (contains == "FBC" ? "FC" : contains);
						FullComboSprite.transform.localScale = baseScaleFCSprite[perfect ? 3 : (fullFantCombo ? 2 : (fullExCombo ? 1 : 0))]*coefficientScaleFCSprite; 
						//particleComboCam.gameObject.SetActive(true);
						foreach(var part in clearcombo.Where(c => c.Key.Contains(contains))){
							part.Value.gameObject.SetActive(true);
							part.Value.Play();
						}
					}
					
					clearFail.enabled = true;
					if(inNetworkMode && life <= 0f) clearFail.spriteName = "Fail";
					blackSprite.enabled = true;
					normalScaleClearFail = clearFail.transform.localScale;
					clearFail.transform.localScale = fillVector(clearFail.transform.localScale.x*10f, clearFail.transform.localScale.y/10f, clearFail.transform.localScale.z);
					clearFail.color = fillColor (clearFail.color.r, clearFail.color.g, clearFail.color.b, 0f);
					appearClearok = true;
					
				}else if(scaleProgressClearFail < 1f){
					scaleProgressClearFail += speedScaleClearFail*Time.deltaTime;
					clearFail.transform.localScale = Vector3.Lerp(clearFail.transform.localScale, normalScaleClearFail, scaleProgressClearFail > 1f ? 1f : scaleProgressClearFail);
					clearFail.color = fillColor (clearFail.color.r, clearFail.color.g, clearFail.color.b, scaleProgressClearFail*4f > 1f ? 1f : scaleProgressClearFail*4f);
				
				}
				
				if(oneSecond > timeClearDisappear){
					if(alphaBlackSprite >= 1f)
					{
							SendDataToDatamanager();
							if(!displayValue[5]) RenderSettings.skybox.SetColor("_Tint", fillColor(0.5f, 0.5f, 0.5f, 0.5f));
							Screen.showCursor = true;
							if(!inNetworkMode)
							{
								Application.LoadLevel("ScoreScene");
							}else{
								Network.SetSendingEnabled(0, false);
								Network.isMessageQueueRunning = false;
								LANManager.Instance.statut = LANStatut.RESULT;
								Network.SetLevelPrefix(10);
								ncs.cleanPlayerDisconnected();
								ncs.saveData();
								Application.LoadLevel("LANScoreScene");
							}
					}
					alphaBlackSprite += speedAlphaBlackSprite*Time.deltaTime;
					blackSprite.color = fillColor(blackSprite.color.r, blackSprite.color.g, blackSprite.color.b, alphaBlackSprite);
					clearFail.color = fillColor (clearFail.color.r, clearFail.color.g, clearFail.color.b, 1f - alphaBlackSprite);
				}
			}
			
			
			
		}else{
			if(!inNetworkMode || (inNetworkMode && ncs.readyToPlay)) oneSecond += Time.deltaTime;
			if(!dead) MoveCameraBefore();
			if(dead){
				//Etape 1 : Faire apparaitre le fail
				if(scaleProgressClearFail < 1f){ 
					if(oneSecond >= TEMPO_FailAppear)
					{
						scaleProgressClearFail += speedScaleClearFail*Time.deltaTime;
						clearFail.transform.localScale = Vector3.Lerp(clearFail.transform.localScale, normalScaleClearFail, scaleProgressClearFail > 1f ? 1f : scaleProgressClearFail);
						clearFail.color = fillColor (clearFail.color.r, clearFail.color.g, clearFail.color.b, scaleProgressClearFail*4f > 1f ? 1f : scaleProgressClearFail*4f);
					}
				}
				//Etape 2 : Faire apparaitre le fondu noir
				else if(alphaBlackSprite < 1f)
				{
					if(oneSecond >= TEMPO_BlackAppear)
					{
						alphaBlackSprite += speedAlphaBlackSprite*Time.deltaTime;
						blackSprite.color = fillColor(blackSprite.color.r, blackSprite.color.g, blackSprite.color.b, alphaBlackSprite);
					}
				//Etape 3 : Faire apparaitre les boutons et le texte
				}else if(!deadAndGiveUp && !deadAndRetry && alphaButtonDie < 1f){
					speedmodok = false;
					alphaButtonDie += speedAlphaButtonDie*Time.deltaTime;
					bkgrRetry.color = fillColor(bkgrRetry.color.r, bkgrRetry.color.g, bkgrRetry.color.b, alphaButtonDie);
					lblRetry.color = fillColor(lblRetry.color.r, lblRetry.color.g, lblRetry.color.b, alphaButtonDie);
					bkgrGiveUp.color = fillColor(bkgrGiveUp.color.r, bkgrGiveUp.color.g, bkgrGiveUp.color.b, alphaButtonDie);
					lblGiveUp.color = fillColor(lblGiveUp.color.r, lblGiveUp.color.g, lblGiveUp.color.b, alphaButtonDie);
					bkgrInput.color = fillColor(bkgrInput.color.r, bkgrInput.color.g, bkgrInput.color.b, alphaButtonDie);
					lblInput.color = fillColor(lblInput.color.r, lblInput.color.g, lblInput.color.b, alphaButtonDie);
					labelSpeedmod.color = fillColor(labelSpeedmod.color.r, labelSpeedmod.color.g, labelSpeedmod.color.b, alphaButtonDie);
					theUIInput.activeColor = fillColor (1f, 1f, 1f, alphaButtonDie);
				}else if(!deadAndGiveUp && !deadAndRetry){
					
					speedmodstring = theUIInput.text;
					
					if(!String.IsNullOrEmpty(speedmodstring)){
						double result;
						if(System.Double.TryParse(speedmodstring, out result)){
							if(result >= (double)0.25 && result <= (double)15){
								speedmodSelected = (float)result;
								speedmodok = true;
								var bpmdisplaying = thesong.bpmToDisplay;
								if(bpmdisplaying.Contains("->")){
									bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[0])*speedmodSelected*(1f + (rateSelected/100f))).ToString("0") + "->" + (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[1])*speedmodSelected*(1f + (rateSelected/100f))).ToString("0");
								}else{
									bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying)*speedmodSelected*(1f + (rateSelected/100f))).ToString("0");
								}
								labelSpeedmod.color = new Color(1f, 1f, 1f, 1f);
								labelSpeedmod.text = "Speedmod : x" + speedmodSelected.ToString("0.00") + " (" + bpmdisplaying + " BPM)";
							}else{
								labelSpeedmod.color = new Color(0.7f, 0.2f, 0.2f, 1f);
								labelSpeedmod.text = "Speedmod must be between x0.25 and x15";
								speedmodok = false;
							}
						}else{
							labelSpeedmod.color = new Color(0.7f, 0.2f, 0.2f, 1f);
							labelSpeedmod.text = "Speedmod is not a valid value";
							speedmodok = false;
						}
					}else{
						labelSpeedmod.color = new Color(0.7f, 0.2f, 0.2f, 1f);
						labelSpeedmod.text = "Empty value";
						speedmodok = false;
					}
				//Etape 5 : Faire disparaitre le fail et le reste en fondu
				}else if(alphaButtonDie > 0f)
				{
					alphaButtonDie -= speedAlphaButtonDie*Time.deltaTime;
					clearFail.color = fillColor (1f, 1f, 1f, alphaButtonDie);
					bkgrRetry.color = fillColor(bkgrRetry.color.r, bkgrRetry.color.g, bkgrRetry.color.b, alphaButtonDie);
					lblRetry.color = fillColor(lblRetry.color.r, lblRetry.color.g, lblRetry.color.b, alphaButtonDie);
					bkgrGiveUp.color = fillColor(bkgrGiveUp.color.r, bkgrGiveUp.color.g, bkgrGiveUp.color.b, alphaButtonDie);
					lblGiveUp.color = fillColor(lblGiveUp.color.r, lblGiveUp.color.g, lblGiveUp.color.b, alphaButtonDie);
					bkgrInput.color = fillColor(bkgrInput.color.r, bkgrInput.color.g, bkgrInput.color.b, alphaButtonDie);
					lblInput.color = fillColor(lblInput.color.r, lblInput.color.g, lblInput.color.b, alphaButtonDie);
					labelSpeedmod.color = fillColor(labelSpeedmod.color.r, labelSpeedmod.color.g, labelSpeedmod.color.b, alphaButtonDie);
					theUIInput.activeColor = fillColor (1f, 1f, 1f, alphaButtonDie);
				}
				//Etape 6 : Changement
				else{
					SendDataToDatamanager();
					if(deadAndRetry){
						DataManager.Instance.speedmodSelected = speedmodSelected;
						Application.LoadLevel("ChartScene");
					}else if(deadAndGiveUp){
						if(!displayValue[5]) RenderSettings.skybox.SetColor("_Tint", fillColor(0.5f, 0.5f, 0.5f, 0.5f));
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
			matArrowModel.color = white;
			nextBump++;
		}	
		
		for(int i=0; i<4; i++){
			if(arrowTarget[i].transform.localScale.x < scaleBase){
				arrowTarget[i].transform.localScale += fillVector(Time.deltaTime*speedBumpInput, Time.deltaTime*speedBumpInput, Time.deltaTime*speedBumpInput);
			}
		}
		
	}
	
	
	void FixedUpdate(){
		
		/*if(oneSecond >= 1f && !dead){
			MoveArrows();
		}*/
		
		if(matArrowModel.color.r > 0.5f){
			poolFloat = 0.5f*Time.deltaTime/speedBumps;
			matArrowModel.color -= fillColor(poolFloat,poolFloat,poolFloat, 0f);
		}
		
		if(dead && scaleProgressClearFail >= 1f)
		{
			judgeSprite.color = fillColor(judgeSprite.color.r, judgeSprite.color.g, judgeSprite.color.b, alphaFailLimit + Mathf.PingPong(Time.time*speedClignotementFail, 1f - alphaFailLimit));
		}
		theTimeBar.updateTimeBar((float)timetotalchart);
		
		
		
	}
	
	
	
	//For moving arrows or do some displaying things
	
	#region Defilement chart
	
	void MoveArrows(){
	
		bpsForActualFrame = thesong.getBPS(actualBPM);
		moveForActualFrame = -((float)(bpsForActualFrame*timechart))*speedmod +  changeBPM;
		TMainCamera.position = fillVector(3f, moveForActualFrame - 5, -10f);
		
		if(arrowFrozen.Any()){
			foreach(var el in arrowFrozen.Keys){
				posMoveArrow = el.goArrow.transform.position;
				el.goArrow.transform.position = fillVector(posMoveArrow.x, moveForActualFrame, posMoveArrow.z);
				posMoveArrow = el.goArrow.transform.position;
				divMoveArrow = ((el.posEnding.y - posMoveArrow.y)/2f);
				el.goFreeze.transform.position = fillVector(el.goFreeze.transform.position.x, (posMoveArrow.y + divMoveArrow) , el.goFreeze.transform.position.z);
				el.goFreeze.transform.localScale = fillVector(1f, -divMoveArrow, 0.1f);
				el.goFreeze.renderer.material.SetTextureScale("_BumpMap", fillVector2(0.5f, -divMoveArrow));
				el.goFreeze.transform.GetChild(0).transform.localScale = fillVector((el.posEnding.y - posMoveArrow.y)/(el.posEnding.y - el.posBegining.y), 1f, 0.1f);
				el.changeColorFreeze(arrowFrozen[el], unfrozed);
			}
		}
	}
	
	
	void MoveCameraBefore(){
	
		bpsForActualFrame = thesong.getBPS(actualBPM);
		//var move = -((float)(bps*(-(1.5 - oneSecond - startTheSong)))*speedmod);
		moveForActualFrame = ((float)(bpsForActualFrame*((oneSecond - 1.5f + (startTheSong < 0f ? startTheSong : 0f))))*speedmod);
		TMainCamera.position = fillVector(3f, - 5 - moveForActualFrame, -10f);
	}
	
	
	void VerifyBPMnSTOP(){
		//BPM change verify
		if(nextSwitchBPM < countBPM){
			keybpms = thesong.bpms.ElementAt(nextSwitchBPM).Key;
			if(keybpms <= timetotalchart){
				valuebpms = thesong.bpms.ElementAt(nextSwitchBPM).Value;
				//iwashere = 1;
				bpsForActualFrame = thesong.getBPS(actualBPM);
				changeBPM += -((float)(bpsForActualFrame*(timechart - (float)(timetotalchart - keybpms))))*speedmod;
				timebpm += (double)timechart - (timetotalchart - keybpms);
				timechart = (float)(timetotalchart - keybpms);
				actualBPM = valuebpms;
				nextSwitchBPM++;
			}
		}
		
		//Stop verif
		if(nextSwitchStop < countSTOP)
		{
			keystops = thesong.stops.ElementAt(nextSwitchStop).Key;
			if(keystops <= timetotalchart){
				//iwashere = 2;
				valuestops = thesong.stops.ElementAt(nextSwitchStop).Value;
				timetotalchart = timebpm + timechart + totaltimestop;
				timechart -= (float)timetotalchart - (float)keystops;
				timestop += (float)timetotalchart - (float)keystops;
				totaltimestop += (float)timetotalchart - (float)keystops;
				timetotalchart = timebpm + timechart + totaltimestop;
				actualstop = valuestops;
				nextSwitchStop++;
			}
		}
	}
	
	#endregion
	
	#region GUI
	void RefreshGUIPart(){
		if(scoreToDisplay == Precision.FANTASTIC && timeDisplayScore > limitDisplayScore){
			judgeSprite.color = fillColor(judgeSprite.color.r, judgeSprite.color.g, judgeSprite.color.b, alphaFantasticLimit + Mathf.PingPong(Time.time*speedClignotementFantastic, 1f - alphaFantasticLimit));
		}else{
			judgeSprite.color = fillColor(judgeSprite.color.r, judgeSprite.color.g, judgeSprite.color.b, alphaFantasticLimit);
		}
		
		if(judgeSprite.enabled && timeDisplayScore > limitDisplayScore)
		{
			judgeSprite.enabled = false;
		}
		
		if(actualZoom > 1f){
			actualZoom -= speedZoomDecrease*Time.deltaTime;
			judgeSprite.transform.localScale = actualBaseScaleJudge*actualZoom;
		}
		
		
		if(comboLabel.color.a > 0.40f)
		{
			comboLabel.color = fillColor(comboLabel.color.r, comboLabel.color.g, comboLabel.color.b, comboLabel.color.a - Time.deltaTime*speedAlphaCombo);
		}
		
		if(fastSprite.color.a > 0f)
		{
			fastSprite.color = fillColor (1f, 1f, 1f, fastSprite.color.a - speedFastSlowAlpha*Time.deltaTime);	
		}
		
		if(slowSprite.color.a > 0f)
		{
			slowSprite.color = fillColor (1f, 1f, 1f, slowSprite.color.a - speedFastSlowAlpha*Time.deltaTime);
		}
		
		timeDisplayScore += Time.deltaTime;
		
	}
	
	void ClignFailed(){
		clearFail.color = fillColor(clearFail.color.r, clearFail.color.g, clearFail.color.b, alphaFailLimit + Mathf.PingPong(Time.time*speedClignotementFail, 1f - alphaFailLimit));
	}
	#endregion
	
	#region Inputs verify
	//Valid or deny the frozen arrow
	void VerifyValidFrozenArrow(){
		if(arrowFrozen.Any()){
			if(keyToRemove.Any()) keyToRemove.Clear();
			
			for(int i=0; i<arrowFrozen.Count;i++){
				poolArrow = arrowFrozen.ElementAt(i);
				arrowFrozen[poolArrow.Key] += Time.deltaTime;
				
				if(poolArrow.Key.timeEndingFreeze <= timetotalchart){
					switch(poolArrow.Key.arrowPos){
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
					scoreCount[poolArrow.Key.arrowType == ArrowType.FREEZE ? "FREEZE" : "ROLL"] += 1;
					desactivateGameObject(poolArrow.Key.goArrow);
					desactivateGameObject(poolArrow.Key.goFreeze);
					
					keyToRemove.Add(poolArrow.Key);
				}
				
				
				if(poolArrow.Value >= unfrozed && !keyToRemove.Contains(poolArrow.Key)){
					poolArrow.Key.goArrow.GetComponent<ArrowScript>().missed = true;
					switch(poolArrow.Key.arrowPos){
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
					keyToRemove.Add(poolArrow.Key);
				}
				
				
			}
			
			
			
			foreach(var k in keyToRemove){
				arrowFrozen.Remove(k);
			}
			
		}
	}
	
	
	
	//Verify keys Input at this frame
	
	void VerifyKeysInput(){
		
		if((Input.GetKeyDown(KeyCodeLeft) || Input.GetKeyDown(SecondaryKeyCodeLeft) ) && (arrowLeftList.Any())){
			arrowTarget[0].localScale = arrowTarget[0].localScale*percentScaleInput;
			arrowSelected = findNextLeftArrow();
			realprec = arrowSelected.time - (timetotalchart); //Retirer ou non le Time.deltaTime ?
			prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				
				if(arrowSelected.imJump){
					
					freezeValidateUpdate(arrowSelected, prec, realprec);
					
				}else{
					if(arrowSelected.arrowType == ArrowType.NORMAL || arrowSelected.arrowType == ArrowType.MINE){
						desactivateGameObject(arrowSelected.goArrow);
						
					}else{
						arrowSelected.goArrow.GetComponent<ArrowScript>().valid = true;
						arrowFrozen.Add(arrowSelected,0f);
						arrowSelected.displayFrozenBar();
						StartParticleFreezeLeft(true);
						
					}
					precAverage.Add(realprec);
					ttp = timeToPrec(prec);
					scoreCount[ttp.ToString()] += 1 ;
					if(arrowSelected.imHand) scoreCount["HANDS"] += 1 ;
					GainCombo(1, ttp);
					removeArrow(arrowLeftList, arrowSelected);
					StartParticleLeft(ttp);
					GainScoreAndLife(ttp.ToString());
					displayPrec(prec);
				}
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
					if(arrowSelected.imJump){
					
						freezeMissedUpdate(arrowSelected, prec, realprec);
						
					}else{
						precAverage.Add(realprec);
						ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						arrowSelected.goArrow.GetComponent<ArrowScript>().missed = true;
						removeArrow(arrowLeftList, arrowSelected);
						StartParticleLeft(ttp);
						displayPrec(prec);
						GainScoreAndLife(ttp.ToString());
					}
					ComboStop(false);
			}
			
			if(prec > precToTime(Precision.FANTASTIC) && prec < precToTime(Precision.WAYOFF)){
				if(realprec < 0f)
				{
					slowSprite.color = fillColor(1f, 1f, 1f, 1f);
					fastSprite.color = fillColor(1f, 1f, 1f, 0f);
				}else{
					fastSprite.color = fillColor(1f, 1f, 1f, 1f);
					slowSprite.color = fillColor (1f, 1f, 1f, 0f);
				}
			}
			
		}
		
		
		if((Input.GetKeyDown(KeyCodeDown)  || Input.GetKeyDown(SecondaryKeyCodeDown))  && (arrowDownList.Any())){
			arrowTarget[1].localScale = arrowTarget[1].localScale*percentScaleInput;
			arrowSelected = findNextDownArrow();
			
			realprec = arrowSelected.time - (timetotalchart);
			prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(arrowSelected.imJump){
					freezeValidateUpdate(arrowSelected, prec, realprec);
				}else{
					if(arrowSelected.arrowType == ArrowType.NORMAL || arrowSelected.arrowType == ArrowType.MINE){
						desactivateGameObject(arrowSelected.goArrow);
						
					}else{
						arrowSelected.goArrow.GetComponent<ArrowScript>().valid = true;
						arrowFrozen.Add(arrowSelected,0f);
						arrowSelected.displayFrozenBar();
						StartParticleFreezeDown(true);
						
					}
					precAverage.Add(realprec);
					ttp = timeToPrec(prec);
					scoreCount[ttp.ToString()] += 1;
					if(arrowSelected.imHand) scoreCount["HANDS"] += 1 ;
					GainCombo(1, ttp);
					removeArrow(arrowDownList, arrowSelected);
					StartParticleDown(ttp);
					GainScoreAndLife(ttp.ToString());
					displayPrec(prec);
				}
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
					if(arrowSelected.imJump){
					
						freezeMissedUpdate(arrowSelected, prec, realprec);
					}else{
						precAverage.Add(realprec);
						ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						arrowSelected.goArrow.GetComponent<ArrowScript>().missed = true;
						removeArrow(arrowDownList, arrowSelected);
						StartParticleDown(ttp);
						displayPrec(prec);
						GainScoreAndLife(ttp.ToString());
					}
					ComboStop(false);
			}
			
			if(prec > precToTime(Precision.FANTASTIC) && prec < precToTime(Precision.WAYOFF)){
				if(realprec < 0f)
				{
					slowSprite.color = fillColor(1f, 1f, 1f, 1f);
					fastSprite.color = fillColor(1f, 1f, 1f, 0f);
				}else{
					fastSprite.color = fillColor(1f, 1f, 1f, 1f);
					slowSprite.color = fillColor (1f, 1f, 1f, 0f);
				}	
			}
		}
		
		
		if((Input.GetKeyDown(KeyCodeUp) || Input.GetKeyDown(SecondaryKeyCodeUp)) && (arrowUpList.Any())){
			arrowTarget[2].localScale = arrowTarget[2].localScale*percentScaleInput;
			arrowSelected = findNextUpArrow();
			realprec = arrowSelected.time - (timetotalchart);
			prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(arrowSelected.imJump){
					freezeValidateUpdate(arrowSelected, prec, realprec);
				}else{
					if(arrowSelected.arrowType == ArrowType.NORMAL || arrowSelected.arrowType == ArrowType.MINE){
						desactivateGameObject(arrowSelected.goArrow);
						
					}else{
						arrowSelected.goArrow.GetComponent<ArrowScript>().valid = true;
						arrowFrozen.Add(arrowSelected,0f);
						arrowSelected.displayFrozenBar();
						StartParticleFreezeUp(true);
						
					}
					precAverage.Add(realprec);
					ttp = timeToPrec(prec);
					scoreCount[ttp.ToString()] += 1;
					if(arrowSelected.imHand) scoreCount["HANDS"] += 1 ;
					GainCombo(1, ttp);
					removeArrow(arrowUpList, arrowSelected);
					StartParticleUp(ttp);
					GainScoreAndLife(ttp.ToString());
					displayPrec(prec);
				}
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
					if(arrowSelected.imJump){
					
						freezeMissedUpdate(arrowSelected, prec, realprec);
					}else{
						precAverage.Add(realprec);
						ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						arrowSelected.goArrow.GetComponent<ArrowScript>().missed = true;
						removeArrow(arrowUpList, arrowSelected);
						StartParticleUp(ttp);
						displayPrec(prec);
						GainScoreAndLife(ttp.ToString());
					}
					ComboStop(false);
			}
			
			if(prec > precToTime(Precision.FANTASTIC) && prec < precToTime(Precision.WAYOFF)){
				if(realprec < 0f)
				{
					slowSprite.color = fillColor(1f, 1f, 1f, 1f);
					fastSprite.color = fillColor(1f, 1f, 1f, 0f);
				}else{
					fastSprite.color = fillColor(1f, 1f, 1f, 1f);
					slowSprite.color = fillColor (1f, 1f, 1f, 0f);
				}	
			}
		
		}
		
		
		if((Input.GetKeyDown(KeyCodeRight) || Input.GetKeyDown(SecondaryKeyCodeRight)) && (arrowRightList.Any())){
			arrowTarget[3].localScale = arrowTarget[3].localScale*percentScaleInput;
			arrowSelected = findNextRightArrow();
			realprec = arrowSelected.time - (timetotalchart);
			prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(arrowSelected.imJump){
					freezeValidateUpdate(arrowSelected, prec, realprec);
				}else{
					if(arrowSelected.arrowType == ArrowType.NORMAL || arrowSelected.arrowType == ArrowType.MINE){
						desactivateGameObject(arrowSelected.goArrow);
						
					}else{
						arrowSelected.goArrow.GetComponent<ArrowScript>().valid = true;
						arrowFrozen.Add(arrowSelected,0f);
						arrowSelected.displayFrozenBar();
						StartParticleFreezeRight(true);
						
					}
					precAverage.Add(realprec);
					ttp = timeToPrec(prec);
					scoreCount[ttp.ToString()] += 1;
					if(arrowSelected.imHand) scoreCount["HANDS"] += 1 ;
					GainCombo(1, ttp);
					removeArrow(arrowRightList, arrowSelected);
					StartParticleRight(ttp);
					GainScoreAndLife(ttp.ToString());
					displayPrec(prec);
				}
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
					if(arrowSelected.imJump){
					
						freezeMissedUpdate(arrowSelected, prec, realprec);
					}else{
						precAverage.Add(realprec);
						ttp = timeToPrec(prec);
						scoreCount[ttp.ToString()] += 1;
						arrowSelected.goArrow.GetComponent<ArrowScript>().missed = true;
						removeArrow(arrowRightList, arrowSelected);
						StartParticleRight(ttp);
						displayPrec(prec);
						GainScoreAndLife(ttp.ToString());
					}
					ComboStop(false);
			}
			
			if(prec > precToTime(Precision.FANTASTIC) && prec < precToTime(Precision.WAYOFF)){
				if(realprec < 0f)
				{
					slowSprite.color = fillColor(1f, 1f, 1f, 0.8f);
					fastSprite.color = fillColor(1f, 1f, 1f, 0f);
				}else{
					fastSprite.color = fillColor(1f, 1f, 1f, 0.8f);
					slowSprite.color = fillColor (1f, 1f, 1f, 0f);
				}	
			}
		}
		
		
		if(!inNetworkMode && Input.GetKeyDown(KeyCode.Escape) && thesong.duration*0.75f > timetotalchart){
			if(!clear && !fail){
				Screen.showCursor = true;
				if(!displayValue[5]) RenderSettings.skybox.SetColor("_Tint", fillColor(0.5f, 0.5f, 0.5f, 0.5f));
				GetComponent<FadeManager>().enabled = true;
				GetComponent<FadeManager>().FadeIn("solo");
			}
		}
	}
	
	
	
	void freezeValidateUpdate(Arrow ar, double prec, double realprec)
	{
		ar.alreadyValid = true;
		ar.goArrow.GetComponent<ArrowScript>().valid = true;
		
		if(!ar.neighboors.Any(c => c.alreadyValid == false)){
			keyToCheck[0] = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
			keyToCheck[1] = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
			keyToCheck[2] = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
			keyToCheck[3] = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
			if(keyToCheck[0] != null){
				if(keyToCheck[0].arrowType == ArrowType.NORMAL || keyToCheck[0].arrowType == ArrowType.MINE){
					desactivateGameObject(keyToCheck[0].goArrow);
					
				}else{
					arrowFrozen.Add(keyToCheck[0],0f);
					keyToCheck[0].displayFrozenBar();
					StartParticleFreezeLeft(true);
					
				}
				
				removeArrow(arrowLeftList, keyToCheck[0]);
				StartParticleLeft(timeToPrec(prec));
			}
			if(keyToCheck[1] != null){
				if(keyToCheck[1].arrowType == ArrowType.NORMAL || keyToCheck[1].arrowType == ArrowType.MINE){
					desactivateGameObject(keyToCheck[1].goArrow);
					
				}else{
					arrowFrozen.Add(keyToCheck[1],0f);
					keyToCheck[1].displayFrozenBar();
					StartParticleFreezeDown(true);
					
				}
				
				removeArrow(arrowDownList, keyToCheck[1]);
				StartParticleDown(timeToPrec(prec));
			}
			if(keyToCheck[2] != null){
				if(keyToCheck[2].arrowType == ArrowType.NORMAL || keyToCheck[2].arrowType == ArrowType.MINE){
					desactivateGameObject(keyToCheck[2].goArrow);
					
				}else{
					arrowFrozen.Add(keyToCheck[2],0f);
					keyToCheck[2].displayFrozenBar();
					StartParticleFreezeUp(true);
					
				}
				removeArrow(arrowUpList, keyToCheck[2]);
				StartParticleUp(timeToPrec(prec));
			}
			if(keyToCheck[3] != null){
				if(keyToCheck[3].arrowType == ArrowType.NORMAL || keyToCheck[3].arrowType == ArrowType.MINE){
					desactivateGameObject(keyToCheck[3].goArrow);
					
				}else{
					arrowFrozen.Add(keyToCheck[3],0f);
					keyToCheck[3].displayFrozenBar();
					StartParticleFreezeRight(true);
					
				}
				removeArrow(arrowRightList, keyToCheck[3]);
				StartParticleRight(timeToPrec(prec));
			}
			precAverage.Add(realprec);
			ttp = timeToPrec(prec);
			GainScoreAndLife(ttp.ToString());
			scoreCount[ttp.ToString()] += 1;
			scoreCount[ar.imHand ? "HANDS" : "JUMPS"] += 1;
			if(ar.imHand && ar.neighboors.Count == 2) scoreCount["JUMPS"] += 1;
			displayPrec(prec);
			GainCombo(ar.neighboors.Count, ttp);
			
		}
	}
	
	void freezeMissedUpdate(Arrow ar, double prec, double realprec)
	{
		ar.alreadyValid = true;
		ar.goArrow.GetComponent<ArrowScript>().missed = true;
		if(!ar.neighboors.Any(c => c.alreadyValid == false)){
			keyToCheck[0] = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.LEFT);
			keyToCheck[1] = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.DOWN);
			keyToCheck[2] = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.UP);
			keyToCheck[3] = ar.neighboors.FirstOrDefault(c => c.arrowPos == ArrowPosition.RIGHT);
			if(keyToCheck[0] != null){
				
				removeArrow(arrowLeftList, keyToCheck[0]);
				StartParticleLeft(timeToPrec(prec));
			}
			if(keyToCheck[1] != null){
				
				
				removeArrow(arrowDownList, keyToCheck[1]);
				StartParticleDown(timeToPrec(prec));
			}
			if(keyToCheck[2] != null){
				
				removeArrow(arrowUpList, keyToCheck[2]);
				StartParticleUp(timeToPrec(prec));
			}
			if(keyToCheck[3] != null){
				
				removeArrow(arrowRightList, keyToCheck[3]);
				StartParticleRight(timeToPrec(prec));
			}
			precAverage.Add(realprec);
			ttp = timeToPrec(prec);
			scoreCount[ttp.ToString()] += 1;
			GainScoreAndLife(ttp.ToString());
			displayPrec(prec);
		}
	
	}
	
	
	//Verify Key for all frames
	void VerifyKeysOutput(){
		if(arrowFrozen.Any()){
			if(Input.GetKey(KeyCodeLeft) || Input.GetKey(SecondaryKeyCodeLeft)){
				poolArrow = arrowFrozen.FirstOrDefault(c => c.Key.arrowPos == ArrowPosition.LEFT);
				if(!poolArrow.Equals(default(KeyValuePair<Arrow, float>)))
				{
					if(poolArrow.Key.arrowType == ArrowType.FREEZE || (poolArrow.Key.arrowType == ArrowType.ROLL && (Input.GetKeyDown(KeyCodeLeft) || Input.GetKeyDown(SecondaryKeyCodeLeft))))
					{
						arrowFrozen[poolArrow.Key] = 0f;
					}
				}
			}
			
			
			if(Input.GetKey(KeyCodeDown) || Input.GetKey(SecondaryKeyCodeDown)){
				poolArrow = arrowFrozen.FirstOrDefault(c => c.Key.arrowPos == ArrowPosition.DOWN);
				if(!poolArrow.Equals(default(KeyValuePair<Arrow, float>)))
				{
					if(poolArrow.Key.arrowType == ArrowType.FREEZE || (poolArrow.Key.arrowType == ArrowType.ROLL && (Input.GetKeyDown(KeyCodeDown) || Input.GetKeyDown(SecondaryKeyCodeDown))))
					{
						arrowFrozen[poolArrow.Key] = 0f;
					}
				}
			}
			
			
			if(Input.GetKey(KeyCodeUp) || Input.GetKey(SecondaryKeyCodeUp)){
				poolArrow = arrowFrozen.FirstOrDefault(c => c.Key.arrowPos == ArrowPosition.UP);
				if(!poolArrow.Equals(default(KeyValuePair<Arrow, float>)))
				{
					if(poolArrow.Key.arrowType == ArrowType.FREEZE || (poolArrow.Key.arrowType == ArrowType.ROLL && (Input.GetKeyDown(KeyCodeUp) || Input.GetKeyDown(SecondaryKeyCodeUp))))
					{
						arrowFrozen[poolArrow.Key] = 0f;
					}
				}
			}
			
			
			if(Input.GetKey(KeyCodeRight) || Input.GetKey(SecondaryKeyCodeRight)){
				poolArrow = arrowFrozen.FirstOrDefault(c => c.Key.arrowPos == ArrowPosition.RIGHT);
				if(!poolArrow.Equals(default(KeyValuePair<Arrow, float>)))
				{
					if(poolArrow.Key.arrowType == ArrowType.FREEZE || (poolArrow.Key.arrowType == ArrowType.ROLL && (Input.GetKeyDown(KeyCodeRight) || Input.GetKeyDown(SecondaryKeyCodeRight))))
					{
						arrowFrozen[poolArrow.Key] = 0f;
					}
				}
			}
			
		}
		
	}
	
	#endregion
	
	#region Particules	
	void StartParticleLeft(Precision prec){
		poolPS = precLeft[prec < Precision.DECENT && combo >= 100 ? prec.ToString() + "C" : prec.ToString()];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
	}
	
	void StartParticleDown(Precision prec){
		poolPS = precDown[prec < Precision.DECENT && combo >= 100 ? prec.ToString() + "C" : prec.ToString()];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
	}
	
	void StartParticleUp(Precision prec){
		poolPS = precUp[prec < Precision.DECENT && combo >= 100 ? prec.ToString() + "C" : prec.ToString()];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
	}
	
	void StartParticleRight(Precision prec){
		poolPS = precRight[prec < Precision.DECENT && combo >= 100 ? prec.ToString() + "C" : prec.ToString()];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
	}
	
	void StartParticleFreezeLeft(bool state){
		poolPS = precLeft["FREEZE"];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
		poolPS.time = 1f;
		poolPS.loop = state;
	}
	
	void StartParticleFreezeRight(bool state){
		poolPS = precRight["FREEZE"];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
		poolPS.time = 1f;
		poolPS.loop = state;
		
	}
	
	void StartParticleFreezeDown(bool state){
		poolPS = precDown["FREEZE"];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
		poolPS.time = 1f;
		poolPS.loop = state;
		
	}
	
	void StartParticleFreezeUp(bool state){
		poolPS = precUp["FREEZE"];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
		poolPS.time = 1f;
		poolPS.loop = state;
		
	}
	
	public void StartParticleMineLeft(){
		poolPS = precLeft["MINE"];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
		
	}
	
	public void StartParticleMineRight(){
		poolPS = precRight["MINE"];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
		
	}
	
	public void StartParticleMineDown(){
		poolPS = precDown["MINE"];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
		
	}
	
	public void StartParticleMineUp(){
		poolPS = precUp["MINE"];
		if(!poolPS.gameObject.activeInHierarchy) poolPS.gameObject.SetActive(true);
		poolPS.Play();
		
	}
	
	#endregion
	
	#region util
	
	public Color fillColor(float r, float g, float b, float a)
	{
		poolColor.r = r;
		poolColor.g = g;
		poolColor.b = b;
		poolColor.a = a;
		
		return poolColor;
	}
	
	public Vector3 fillVector(float x, float y, float z)
	{
		poolVector.x = x;
		poolVector.y = y;
		poolVector.z = z;
		
		return poolVector;
	}
				
	public Vector2 fillVector2(float x, float y){
		poolVector2.x = x;
		poolVector2.y = y;
					
		return poolVector2;
	}
	
	public void GainScoreAndLife(string s){
		if(!fail){
			if(lifeBase[s] <= 0 || combo >= DataManager.Instance.regenComboAfterMiss){
				life += lifeBase[s];
				if(life > 100f){
					life = 100f;	
				}else if(life < 0f){
					life = 0f;	
				}
				//if(!lifeGraph.ContainsKey(timetotalchart)) lifeGraph.Add(timetotalchart, life);
				timeOfLife.Add(timetotalchart);
				numberLife.Add(life);
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
			scoreRefresh();
			if(!inNetworkMode)
			{
				if(score >= persoRecordScore && scoreBeatLabel.effectColor == normalbeatColor)
				{
					scoreBeatLabel.effectColor = beatthepersonalColor;
					scoreBeatLabel.text = recordScore.ToString("00.00") + "%";
				}
				if(score >= recordScore && scoreBeatLabel.effectColor == beatthepersonalColor)
				{
					scoreBeatLabel.effectColor = beatthebeatColor;
				}
			}
		}
		
		if(life <= 0f || scoreInverse < targetScoreInverse){
			//Debug.Log ("Fail at life");
			if(!inNetworkMode || !clear) fail = true;
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
		theTimeBar.updatePS(ct, combo);
		if(comboLabel.color.g == dangerColor.b && combo >= 25)
		{
			comboLabel.color = normalColor;
			comboLabel.effectColor = normalOutlineColor;
		}else if(combo >= 100)
		{
			switch(ct)
			{
			case ComboType.FULLFANTASTIC:
				comboLabel.color = FFCColor;
				comboLabel.effectColor = FFCOutlineColor;
				break;
			case ComboType.FULLEXCELLENT:
				comboLabel.color = FPCColor;
				comboLabel.effectColor = FPCOutlineColor;
				break;
			case ComboType.FULLCOMBO:
				comboLabel.color = maxColor;
				comboLabel.effectColor = maxOutlineColor;
				break;
			}
		}
		comboLabel.color = fillColor(comboLabel.color.r, comboLabel.color.g, comboLabel.color.b, 1f);
		comboRefresh();
		theTimeBar.HitBar(prec);
		if(isFullExComboRace && ct == ComboType.FULLCOMBO){
			//Debug.Log("Fail at FEC race");
			if(!inNetworkMode || !clear) fail = true;
		}
	}
	
	void scoreRefresh(){
		
		scoreLabel.text = score.ToString("00.00") + "%";
		
	}
	
	
	void comboRefresh(){
		comboLabel.text = combo.ToString();
	}

	public void ComboStop(bool miss){
		theTimeBar.breakPS();
		//timeCombo.Add(timetotalchart, combo);
		timeOfCombo.Add(timetotalchart);
		numberCombo.Add(combo);
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
		comboLabel.color = dangerColor;
		comboLabel.effectColor = dangerOutlineColor;
		comboRefresh();
		if(isFullComboRace || isFullExComboRace){
			//Debug.Log ("Fail at combo race");
			if(!inNetworkMode || !clear) fail = true;
		}
		if(miss){
			comboMisses++;
		}else{
			comboMisses = 0;
		}
	}
	
	
	void desactivateGameObject(GameObject go)
	{
		//prev : GetComponent("ArrowScript");
		if(go.GetComponent<ArrowScript>() != null)
		{
			go.GetComponent<ArrowScript>().enabled = false;
		}
		if(go.transform.renderer != null)
		{
			go.transform.renderer.enabled = false;	
		}
		for(int i=0; i < go.transform.childCount; i++)
		{
			go.transform.GetChild(i).renderer.enabled = false;	
		}
		
		
	}
	
	public bool desactiveGameObjectMissed(GameObject go, GameObject gofreeze, float distance)
	{
		if((go.transform.position.y - distance) >= arrowTarget[0].position.y)
		{
			desactivateGameObject(go);
			if(gofreeze != null) desactivateGameObject(gofreeze);
			return true;
		}
		return false;
	}
	
	public void setButtonChoice(bool retry)
	{
		if(speedmodok && !deadAndGiveUp && !deadAndRetry)
		{
			if(retry)
			{
				deadAndRetry = true;
			}else
			{
				deadAndGiveUp = true;	
			}
		}
	}
	
	void SendDataToDatamanager(){
		DataManager.Instance.scoreEarned = score;
		DataManager.Instance.precAverage = precAverage;
		for(int i=0; i<timeOfCombo.Count; i++)
		{
			if(!timeCombo.ContainsKey(timeOfCombo[i])) timeCombo.Add(timeOfCombo[i], numberCombo[i]);	
		}
		if(!timeCombo.ContainsKey(timetotalchart)) timeCombo.Add(timetotalchart, combo);
		DataManager.Instance.timeCombo = timeCombo;
		for(int i=0; i<timeOfLife.Count; i++)
		{
			if(!lifeGraph.ContainsKey(timeOfLife[i])) lifeGraph.Add(timeOfLife[i], numberLife[i]);	
		}
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
		ProfileManager.Instance.currentProfile.updateGameTime(timetotalchart);
	}
	
	public void askingForInfo()
	{
		ncs.objectToSend(score, life, 
		(ct != ComboType.NONE && combo < 100) ?
			5
		:
			(ct == ComboType.NONE && combo < 25) ?
				4
			:
				(int)ct
		, fail);
	}
	#endregion
	
	#region Precision
	double precToTime(Precision prec){
		if(prec <= Precision.WAYOFF){
			return DataManager.Instance.PrecisionValues[prec];	
		}
		return 0;
	}
	
	
	public void displayPrec(double prec){
		judgeSprite.enabled = true;
		timeDisplayScore = 0f;
		scoreToDisplay = timeToPrec(prec);
		if(scoreToDisplay != Precision.DECENT && scoreToDisplay != Precision.WAYOFF)
		{
			actualZoom = zoomPower;
		}else
		{
			actualZoom = 1f;
		}
		judgeSprite.spriteName = scoreToDisplay.ToString();
		judgeSprite.MakePixelPerfect();
		actualBaseScaleJudge = baseScaleJudgeSprite[(int)scoreToDisplay]*coefficientScaleJudge;
		judgeSprite.transform.localScale = actualBaseScaleJudge*actualZoom;
	}
	
	Precision timeToPrec(double prec){
	
		precFounded = DataManager.Instance.PrecisionValues.FirstOrDefault(c => (prec <= c.Value));
		
		return precFounded.Equals(default(KeyValuePair<Precision, double>)) ? Precision.MISS : precFounded.Key;
		
		
	}
	
	
	#endregion
	
	#region Arrow and time
	
	public Arrow findNextUpArrow(){

		return arrowUpList.First();
			
	}
	
	public Arrow findNextDownArrow(){

		return arrowDownList.First();
			
	}
	
	public Arrow findNextLeftArrow(){

		return arrowLeftList.First();
			
	}
	
	public Arrow findNextRightArrow(){

		return arrowRightList.First();
			
	}
	
	public Arrow findNextUpMine(){

		return mineUpList.First();
			
	}
	
	public Arrow findNextDownMine(){

		return mineDownList.First();
			
	}
	
	public Arrow findNextLeftMine(){

		return mineLeftList.First();
			
	}
	
	public Arrow findNextRightMine(){

		return mineRightList.First();
			
	}
	//Remove key from arrow list
	public void removeArrowFromList(Arrow ar, ArrowPosition state){
		
		switch(state){
			case ArrowPosition.LEFT:
				removeArrow(arrowLeftList, ar);
				break;
			case ArrowPosition.DOWN:
				removeArrow(arrowDownList, ar);
				break;
			case ArrowPosition.UP:
				removeArrow(arrowUpList, ar);
				break;
			case ArrowPosition.RIGHT:
				removeArrow(arrowRightList, ar);
				break;
				
		}
	}
	
	public void removeMineFromList(Arrow ar, ArrowPosition state){
		
		switch(state){
			case ArrowPosition.LEFT:
				removeArrow(mineLeftList, ar);
				break;
			case ArrowPosition.DOWN:
				removeArrow(mineDownList, ar);
				break;
			case ArrowPosition.UP:
				removeArrow(mineUpList, ar);
				break;
			case ArrowPosition.RIGHT:
				removeArrow(mineRightList,ar);
				break;
				
		}
	}
	
	public void removeArrow(List<Arrow> lar, Arrow ar)
	{
		trash.Add(ar);
		lar.Remove(ar);
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
		trash = new List<Arrow>();
		
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
				
				
				// Traitement
				
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
				var tripleselect = 0;
				
				for(int i =0;i<4; i++){
					if(note[i] == '1'){
						
						barr = true;
						tripleselect++;
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						goArrow.SetActive(true);
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
						//goArrow.SetActive(false);
						GetComponent<ManageGameObject>().Add(goArrow);
						//barrow = true;
					}else if(note[i] == '2'){
						barr = true;
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						goArrow.SetActive(true);
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
						//goArrow.SetActive(false);
						GetComponent<ManageGameObject>().Add(goArrow);
					}else if(note[i] == '3'){
						barr = true;
						var theArrow = ArrowFreezed[i];
						if(theArrow != null){
							var goFreeze = (GameObject) Instantiate(theArrow.arrowType == ArrowType.FREEZE ? freeze : roll, new Vector3(i*2, (theArrow.goArrow.transform.position.y + ((-ypos - theArrow.goArrow.transform.position.y)/2f)) , 1f), freeze.transform.rotation);
							goFreeze.SetActive(true);
							goFreeze.transform.localScale = new Vector3(1f, -((-ypos - theArrow.goArrow.transform.position.y)/2f), 0.5f);
							goFreeze.renderer.material.SetTextureScale("_BumpMap", fillVector2(0.5f, goFreeze.transform.localScale.y));
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
						goArrow.SetActive(true);
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
						//goArrow.SetActive(false);
						GetComponent<ManageGameObject>().Add(goArrow);
					}else if(note[i] == 'M'){
						var goArrow = (GameObject) Instantiate(mines, new Vector3(i*2, -ypos, 0f), mines.transform.rotation);
						goArrow.SetActive(true);
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
				
				for(int i=0;i<4;i++){
					if(ArrowFreezed[i] != null && barr) tripleselect++;
				}
				
				if(tripleselect >= 3f){
					foreach(var el in listNeighboors){
						el.imHand = true;
					}
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
			}else if((posmesure+23)%48 == 0){
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
