using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class InGameScript : MonoBehaviour {
	
	#region Declarations
	//Game object song scene
	public GameObject arrow;
	public GameObject freeze;
	public GameObject mines;
	public Camera MainCamera;
	private Transform TMainCamera;
	public GameObject arrowLeft;
	public GameObject arrowRight;
	public GameObject arrowDown;
	public GameObject arrowUp;
	public Material matArrowModel;
	public GameObject particlePrec;
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
	
	//DISPLAY
	private Color bumpColor;
	public float bumpfadeSpeed = 0.5f;
	
	
	//START
	private bool firstUpdate;
	private float oneSecond;
	private float startTheSong; //Time pour démarrer la chanson
	
	//BUMP
	private int nextBump;
	private List<double> Bumps;
	public float speedBumps;
	
	
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
	public float timeFailDisappear;
	public float timeClearDisappear;
	private float zoomfail;
	public float speedzoom;
	public float speedziwp;
	private float zwip;
	public Rect posFail;
	public Rect posClear;
	private bool appearFailok;
	private bool disappearFailok;
	public float speedAlphaFailFade;
	private float failalpha;
	private bool cacheFailed;
	public float speedFadeAudio;
	private float passalpha;
	public Rect fullComboPos;
	
	
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
		var rand = (int)(UnityEngine.Random.value*DataManager.Instance.skyboxList.Count);
		if(rand == DataManager.Instance.skyboxList.Count){
			rand--;	
		}
		
		RenderSettings.skybox = DataManager.Instance.skyboxList.ElementAt(rand);
		
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
			precLeft.Add( el.ToString(), (ParticleSystem) arrowLeft.transform.GetChild(0).gameObject.transform.FindChild(el.ToString()).particleSystem );
			precDown.Add( el.ToString(), (ParticleSystem) arrowDown.transform.GetChild(0).gameObject.transform.FindChild(el.ToString()).particleSystem );
			precRight.Add( el.ToString(), (ParticleSystem) arrowRight.transform.GetChild(0).gameObject.transform.FindChild(el.ToString()).particleSystem );
			precUp.Add( el.ToString(), (ParticleSystem) arrowUp.transform.GetChild(0).gameObject.transform.FindChild(el.ToString()).particleSystem );
		}
		for(int i=0; i< particleComboCam.transform.GetChildCount(); i++){
			clearcombo.Add(particleComboCam.transform.GetChild(i).name, particleComboCam.transform.GetChild(i).particleSystem);
		}
		
		precAverage = new List<double>();
		timeCombo = new Dictionary<double, int>();
		lifeGraph = new Dictionary<double, double>();
		
		TMainCamera = MainCamera.transform;
		
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
		score = 0f;
		scoreInverse = 100f;
		firstEx = 0;
		firstGreat = 0;
		firstMisteak = 0;
		
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
	}
	
	
	
	
	//only for FPS
	void OnGUI(){
		
		//fake stuff
		GUI.Label(new Rect(0.9f*Screen.width, 0.05f*Screen.height, 200f, 200f), fps.ToString());	
			
		//end fake stuff
		if(timeDisplayScore < limitDisplayScore && !clear){

			GUI.color = new Color(1f, 1f, 1f, alpha);
			GUI.DrawTexture(new Rect(posScore.x*Screen.width - zoom, posScore.y*Screen.height, posScore.width*Screen.width + zoom*2, posScore.height*Screen.height), TextureBase[scoreToDisplay.ToString()]); 
		}
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
			
		
		for(int i=0;i<5;i++){
			if((i == 3 && displaying[3] == 0 && displaying[4] == 0) || (i == 4 && displaying[4] == 0)) break;
			GUI.DrawTexture(new Rect((posPercent.x + ecart*(4-i))*Screen.width, posPercent.y*Screen.height,  posPercent.width*Screen.width,  posPercent.height*Screen.height), TextureBase["S" + displaying[i]]);
			
		}
		GUI.DrawTexture(new Rect((posPercent.x + ((ecart*2)+(ecart/2f)))*Screen.width, posPercent.y*Screen.height,  posPercent.width*Screen.width, posPercent.height*Screen.height), TextureBase["DOT"]);
		GUI.DrawTexture(new Rect((posPercent.x + ecart*5)*Screen.width + posPercent.width, posPercent.y*Screen.height, posPercent.width*Screen.width, posPercent.height*Screen.height), TextureBase["PERCENT"]);
		
		if(combo >= 5f){
			//var czoom = zoom/4f;
			var col = matProgressBar.color;
			GUI.color = new Color(col.r , col.g, col.b, alphaCombo);
			for(int i=0; i<thetab.Length; i++){
				GUI.DrawTexture(new Rect((posCombo.x + ((ecartCombo*(thetab.Length-(i+1))/2f) -ecartCombo*((float)i/2f)))*Screen.width, 
				posCombo.y*Screen.height, posCombo.width*Screen.width, posCombo.height*Screen.height), TextureBase["C" + thetab[i]]);
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
		
		//OLD Mechanism
		//if(Event.current.type.Equals(EventType.Repaint)){
		/*
			if(score >= 10f) Graphics.DrawTexture(new Rect(posPercent.x*Screen.width, posPercent.y*Screen.height, wd, hg), TextureBase["SCORENUMBER"], new Rect(0f, - displaying.x, 1f, 0.1f), 0,0,0,0);
			Graphics.DrawTexture(new Rect(posPercent.x*Screen.width + posPercent.width*ecart, posPercent.y*Screen.height, wd, hg), TextureBase["SCORENUMBER"], new Rect(0f, - displaying.y, 1f, 0.1f), 0,0,0,0);
			Graphics.DrawTexture(new Rect(posPercent.x*Screen.width + posPercent.width*(ecart+(ecart/2f)), posPercent.y*Screen.height, wd, hgt*4), TextureBase["SCORESYMBOL"], new Rect(0f, 0f, 1f, 0.5f), 0,0,0,0);
			Graphics.DrawTexture(new Rect(posPercent.x*Screen.width + posPercent.width*ecart*2, posPercent.y*Screen.height, wd, hg), TextureBase["SCORENUMBER"], new Rect(0f, - displaying.width, 1f, 0.1f), 0,0,0,0);
			Graphics.DrawTexture(new Rect(posPercent.x*Screen.width + posPercent.width*ecart*3, posPercent.y*Screen.height, wd, hg), TextureBase["SCORENUMBER"], new Rect(0f, - displaying.height, 1f, 0.1f), 0,0,0,0);
			Graphics.DrawTexture(new Rect(posPercent.x*Screen.width + posPercent.width*ecart*4, posPercent.y*Screen.height, wd, hgt*4), TextureBase["SCORESYMBOL"], new Rect(0f, 0.5f, 1f, 0.5f), 0,0,0,0);
		*/
		
	
		
		
		
		
			/*
			if(combo >= 5){
				Graphics.DrawTexture(new Rect(posCombo.x*Screen.width + posCombo.width*(ecart*(thetab.Length-1)/2f) - czoom, posCombo.y*Screen.height, wd + czoom*2f, hg), TextureBase["COMBONUMBER"], new Rect(0f, - thetab[0], 1f, 0.1f), 0,0,0,0);
				if(combo > 9){
					Graphics.DrawTexture(new Rect(posCombo.x*Screen.width + posCombo.width*((ecart*(thetab.Length-2)/2f) - ecart*0.5f)- czoom, posCombo.y*Screen.height, wd+ czoom*2f, hg), TextureBase["COMBONUMBER"], new Rect(0f, - thetab[1], 1f, 0.1f), 0,0,0,0);
					if(combo >99){
						Graphics.DrawTexture(new Rect(posCombo.x*Screen.width + posCombo.width*((ecart*(thetab.Length-3)/2f) -ecart)- czoom, posCombo.y*Screen.height, wd+ czoom*2f, hg), TextureBase["COMBONUMBER"], new Rect(0f, - thetab[2], 1f, 0.1f), 0,0,0,0);
						if(combo >999){
							Graphics.DrawTexture(new Rect(posCombo.x*Screen.width + posCombo.width*((ecart*(thetab.Length-4)/2f) -ecart*1.5f)- czoom, posCombo.y*Screen.height, wd+ czoom*2f, hg), TextureBase["COMBONUMBER"], new Rect(0f, - thetab[3], 1f, 0.1f), 0,0,0,0);
							if(combo >9999){
								Graphics.DrawTexture(new Rect(posCombo.x*Screen.width + posCombo.width*(-ecart*2f)- czoom, posCombo.y*Screen.height , wd+ czoom*2f, hg), TextureBase["COMBONUMBER"], new Rect(0f, - thetab[4], 1f, 0.1f), 0,0,0,0);
							}
						}
					}
				}
			}*/
		//}
		
	}
	
	
	IEnumerator swipTexture(bool reverse, float height){
		
		if(!reverse){
			zwip = height/2f;
			while(zwip > 0f){
				zwip -= height/2f*Time.deltaTime/speedziwp;
				yield return new WaitForFixedUpdate();
			}
			zwip = 0f;
		}else{
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
		
		if((oneSecond >= 1f && !dead) || clear){
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
					//Debug.Log(startTheSong);
					timetotalchart = timebpm + timechart + totaltimestop;
					firstUpdate = false;
				}else{
					startTheSong -= Time.deltaTime;	
				}
			}
			
			BumpsBPM();
			
			
			if(thesong.duration < timetotalchart && !fail && !clear){
				clear = true;
				if(scoreCount["DECENT"] == 0 && scoreCount["WAYOFF"] == 0 && scoreCount["MISS"] == 0){
					if(score >= 100f || scoreInverse == 100f) perfect = true;
					if(scoreCount["EXCELLENT"] == 0 && scoreCount["GREAT"] == 0) fullFantCombo = true;
					if(scoreCount["GREAT"] == 0) fullExCombo = true;
					fullCombo = true;
				}
				oneSecond = 0f;
				failalpha = 0f;
				passalpha = 1f;
				alpha = 1f;
				sensFantastic = true;
			}
			
			if(fail){
				if((typeOfDeath != 2 && (typeOfDeath == 0 || comboMisses >= 30)) || thesong.duration < timetotalchart){
					dead = true;
					audio.Stop ();
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
					StartCoroutine(swipTexture(false, posClear.height));
					var contains = perfect ? "Perfect" : (fullFantCombo ? "FFC" : (fullExCombo ? "FEC" : ( fullCombo ? "FC" : "noPS")) );
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
			if(dead && !appearFailok && oneSecond > timeFailAppear + 1){
				StartCoroutine(swipTexture(false, posFail.height));
				appearFailok = true;
				cacheFailed = false;
			}
			if(dead && !disappearFailok && oneSecond > timeFailDisappear){
				StartCoroutine(swipTexture(true, posFail.height));
				disappearFailok = true;
			}
			if(dead && disappearFailok && oneSecond > timeFailDisappear + 1){
				
				//Passer à la scène de score
				//Proposer Retry / Score
				//si score :
				SendDataToDatamanager();
				Application.LoadLevel("ScoreScene");
				
			}
			if(dead && oneSecond > timeFailAppear){
				//zoomfail += Time.deltaTime/speedzoom;
				if(failalpha < 1) failalpha += Time.deltaTime/speedAlphaFailFade;
				ClignFailed();
			}
			
		}
		
	}
	
	
	void BumpsBPM(){
		if(nextBump < Bumps.Count && Bumps[nextBump] <= timetotalchart){
			matArrowModel.color = new Color(1f, 1f, 1f, 1f);
			nextBump++;
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
			progressBar.transform.localPosition = new Vector3(progressBar.transform.localPosition.x, - (10f - 10f*(float)((timetotalchart - firstArrow)/(lastArrow - firstArrow))), 8f);
			progressBar.transform.localScale = new Vector3(progressBar.transform.localScale.x, 10f*(float)((timetotalchart - firstArrow)/(lastArrow - firstArrow)), 1f);
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
	
	
	#region Inputs verify
	//Valid or deny the frozen arrow
	void VerifyValidFrozenArrow(){
		if(arrowFrozen.Count > 0){
			var KeyToRemove = new List<Arrow>();
			for(int i=0; i<arrowFrozen.Count;i++){
				var el = arrowFrozen.ElementAt(i);
				arrowFrozen[el.Key] += Time.deltaTime;
				
				if(el.Key.timeEndingFreeze <= timetotalchart){
					switch((int)el.Key.goArrow.transform.position.x){
					case 0:
						StartParticleFreezeLeft(false);
						break;
					case 2:
						StartParticleFreezeDown(false);
						break;
					case 4:
						StartParticleFreezeUp(false);
						break;
					case 6:
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
					switch((int)el.Key.goArrow.transform.position.x){
					case 0:
						StartParticleFreezeLeft(false);
						break;
					case 2:
						StartParticleFreezeDown(false);
						break;
					case 4:
						StartParticleFreezeUp(false);
						break;
					case 6:
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
		
		if(Input.GetKeyDown(KeyCodeLeft) && (arrowLeftList.Any() || arrowFrozen.Any())){
			var ar = findNextLeftArrow();
			double realprec = ar.time - (timetotalchart - Time.deltaTime);
			double prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				
				if(ar.imJump){
					ar.alreadyValid = true;
					ar.goArrow.GetComponent<ArrowScript>().valid = true;
					
					if(!ar.neighboors.Any(c => c.alreadyValid == false)){
						var left = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 0);
						var down = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 2);
						var up = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 4);
						var right = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 6);
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
							var left = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 0);
							var down = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 2);
							var up = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 4);
							var right = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 6);
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
			
			
			if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 0f && c.Key.arrowType == ArrowType.ROLL))
			{
				var froz = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 0f);
				arrowFrozen[froz.Key] = 0f;
			}
			
		}
		
		
		if(Input.GetKeyDown(KeyCodeDown) && (arrowDownList.Any() || arrowFrozen.Any())){
			var ar = findNextDownArrow();
			
			double realprec = ar.time - (timetotalchart - Time.deltaTime);
			double prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(ar.imJump){
					ar.alreadyValid = true;
					ar.goArrow.GetComponent<ArrowScript>().valid = true;
					
					if(!ar.neighboors.Any(c => c.alreadyValid == false)){
						var left = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 0);
						var down = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 2);
						var up = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 4);
						var right = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 6);
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
							var left = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 0);
							var down = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 2);
							var up = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 4);
							var right = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 6);
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
		
			if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 2f && c.Key.arrowType == ArrowType.ROLL))
			{
				var froz = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 2f);
				arrowFrozen[froz.Key] = 0f;
			}
		}
		
		
		if(Input.GetKeyDown(KeyCodeUp) && (arrowUpList.Any() || arrowFrozen.Any())){
			var ar = findNextUpArrow();
			double realprec = ar.time - (timetotalchart - Time.deltaTime);
			double prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(ar.imJump){
					ar.alreadyValid = true;
					ar.goArrow.GetComponent<ArrowScript>().valid = true;
					
					if(!ar.neighboors.Any(c => c.alreadyValid == false)){
						var left = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 0);
						var down = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 2);
						var up = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 4);
						var right = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 6);
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
							var left = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 0);
							var down = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 2);
							var up = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 4);
							var right = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 6);
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
			
			if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 4f && c.Key.arrowType == ArrowType.ROLL))
			{
				var froz = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 4f);
				arrowFrozen[froz.Key] = 0f;
			}
		
		}
		
		
		if(Input.GetKeyDown(KeyCodeRight) && (arrowRightList.Any() || arrowFrozen.Any())){
			var ar = findNextRightArrow();
			double realprec = ar.time - (timetotalchart - Time.deltaTime);
			double prec = Mathf.Abs((float)(realprec));
			
			if(prec < precToTime(Precision.GREAT)){ //great
				
				
				if(ar.imJump){
					ar.alreadyValid = true;
					ar.goArrow.GetComponent<ArrowScript>().valid = true;
					
					if(!ar.neighboors.Any(c => c.alreadyValid == false)){
						var left = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 0);
						var down = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 2);
						var up = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 4);
						var right = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 6);
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
							var left = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 0);
							var down = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 2);
							var up = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 4);
							var right = ar.neighboors.FirstOrDefault(c => c.goArrow.transform.position.x == 6);
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
			
			if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 6f && c.Key.arrowType == ArrowType.ROLL))
			{
				var froz = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 6f);
				arrowFrozen[froz.Key] = 0f;
			}
		
		}
		
	}
	
	
	//Verify Key for all frames
	void VerifyKeysOutput(){
		if(arrowFrozen.Count > 0){
			if(Input.GetKey(KeyCodeLeft)){
					if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 0f && c.Key.arrowType == ArrowType.FREEZE)){
						var ar = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 0f);
						arrowFrozen[ar.Key] = 0f;
					}
			}
			
			
			if(Input.GetKey(KeyCodeDown)){
				if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 2f && c.Key.arrowType == ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 2f);
					arrowFrozen[ar.Key] = 0f;
				}
			}
			
			
			if(Input.GetKey(KeyCodeUp)){
				if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 4f && c.Key.arrowType == ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 4f);
					arrowFrozen[ar.Key] = 0f;
				}
			}
			
			
			if(Input.GetKey(KeyCodeRight)){
				if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 6f && c.Key.arrowType == ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 6f);
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
		if(lifeBase[s] <= 0 || combo >= DataManager.Instance.regenComboAfterMiss){
			life += lifeBase[s];
			if(!lifeGraph.ContainsKey(timetotalchart)) lifeGraph.Add(timetotalchart, life);
			if(life > 100f){
				life = 100f;	
			}else if(life < 0f){
				life = 0f;	
			}
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
		if(ct != ComboType.NONE){
			if(ct == ComboType.FULLFANTASTIC && prec != Precision.FANTASTIC){
				if(	prec == Precision.EXCELLENT){ 
					ct = ComboType.FULLEXCELLENT;
					firstEx = timetotalchart;
				}
				else if(prec == Precision.GREAT){ 
					ct = ComboType.FULLCOMBO; 
					firstEx = timetotalchart;
					firstGreat = timetotalchart;
				}
				else { 
					firstEx = timetotalchart;
					firstGreat = timetotalchart;
					firstMisteak = timetotalchart;
					ct = ComboType.NONE; 
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
		var thescorestring = score.ToString("000.00");
		outtab[0] = System.Convert.ToInt32(""+thescorestring[5]);
		outtab[1] = System.Convert.ToInt32(""+thescorestring[4]);
		outtab[2] = System.Convert.ToInt32(""+thescorestring[2]);
		outtab[3] = System.Convert.ToInt32(""+thescorestring[1]);
		outtab[4] = System.Convert.ToInt32(""+thescorestring[0]);
		
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
		
		timeCombo.Add(timetotalchart, combo);
		combo = 0;	
		if(ct != ComboType.NONE){
			firstMisteak = timetotalchart;	
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
	public Arrow findNextUpArrow(){

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
				
				char[] note = mesure.ElementAt(beat).Trim().ToCharArray();
				
				var listNeighboors = new List<Arrow>();
				var barr = false;
				for(int i =0;i<4; i++){
					if(note[i] == '1'){
						//var theArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos  + (float)s.offset, 0f), arrow.transform.rotation);
						barr = true;
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						var theArrow = new Arrow(goArrow, ArrowType.NORMAL, timetotal);
						
						switch(i){
						case 0:
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							arrowDownList.Add(theArrow);
							break;
						case 2:
							arrowUpList.Add(theArrow);
							break;
						case 3:
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
						goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						var theArrow = new Arrow(goArrow, ArrowType.FREEZE, timetotal);
						ArrowFreezed[i] = theArrow;
						switch(i){
						case 0:
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							arrowDownList.Add(theArrow);
							break;
						case 2:
							arrowUpList.Add(theArrow);
							break;
						case 3:
							arrowRightList.Add(theArrow);
							break;
						}
						listNeighboors.Add(theArrow);
						goArrow.SetActiveRecursively(false);
						GetComponent<ManageGameObject>().Add(timetotal, goArrow);
					}else if(note[i] == '3'){
						barr = true;
						var theArrow = ArrowFreezed[i];
						var goFreeze = (GameObject) Instantiate(freeze, new Vector3(i*2, (theArrow.goArrow.transform.position.y + ((-ypos - theArrow.goArrow.transform.position.y)/2f)) , 0.5f), freeze.transform.rotation);
						goFreeze.transform.localScale = new Vector3(1f, -((-ypos - theArrow.goArrow.transform.position.y)/2f), 0.1f);
						goFreeze.transform.GetChild(0).renderer.material.color = theArrow.goArrow.renderer.material.color;
						theArrow.setArrowFreeze(timetotal, new Vector3(i*2,-ypos, 0f), goFreeze, null);
					
					}else if(note[i] == '4'){
						barr = true;
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						var theArrow = new Arrow(goArrow, ArrowType.ROLL, timetotal);
						ArrowFreezed[i] = theArrow;
						switch(i){
						case 0:
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							arrowDownList.Add(theArrow);
							break;
						case 2:
							arrowUpList.Add(theArrow);
							break;
						case 3:
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
							mineLeftList.Add(theArrow);
							break;
						case 1:
							mineDownList.Add(theArrow);
							break;
						case 2:
							mineUpList.Add(theArrow);
							break;
						case 3:
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
		GetComponent<ManageGameObject>().DoTheStartSort();
	}
	
	
	
	Color chooseColor(int posmesure, int mesuretot){
		
		
		switch(mesuretot){
		case 4:	
			return new Color(1f, 0f, 0f, 1f); //
		
			
		case 8:
			if(posmesure%2 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}
			return new Color(1f, 0f, 0f, 1f); //
			
			
		case 12:
			if((posmesure-1)%3 == 0){
				return new Color(1f, 0f, 0f, 1f); //
			}
			return new Color(1f, 0f, 1f, 1f);
			
		case 16:
			if((posmesure-1)%4 == 0){
				return new Color(1f, 0f, 0f, 1f); //
			}else if((posmesure+1)%4 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}
			return new Color(0f, 1f, 0f, 1f);
		
			
		case 24:
			if((posmesure+2)%6 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}else if((posmesure-1)%6 == 0){
				return new Color(1f, 0f, 0f, 1f); //
			}
			return new Color(1f, 0f, 1f, 1f);
			
			
		case 32:
			if((posmesure-1)%8 == 0){
				return new Color(1f, 0f, 0f, 1f); //
			}else if((posmesure+3)%8 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}else if((posmesure+1)%4 == 0){
				return new Color(0f, 1f, 0f, 1f);
			}
			return new Color(1f, 0.8f, 0f, 1f);
			
		case 48:
			if((posmesure+2)%6 == 0){
				return new Color(0f, 1f, 0f, 1f);
			}else if((posmesure-1)%12 == 0){
				return new Color(1f, 0f, 0f, 1f); //
			}else if((posmesure+5)%12 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}
			return new Color(1f, 0f, 1f, 1f);
		
			
		case 64:
			if((posmesure-1)%16 == 0){
				return new Color(1f, 0f, 0f, 1f); //
			}else if((posmesure+7)%16 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}else if((posmesure+3)%8 == 0){
				return new Color(0f, 1f, 0f, 1f);
			}else if((posmesure+1)%4 == 0){
				return new Color(1f, 0.8f, 0f, 1f);
			}
			return new Color(0f, 1f, 0.8f, 1f);
			
		case 192:
			if((posmesure-1)%48 == 0){
				return new Color(1f, 0f, 0f, 1f); //
			}else if((posmesure+13)%48 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}else if((posmesure+11)%24 == 0){
				return new Color(0f, 1f, 0f, 1f);
			}else if((posmesure+5)%12 == 0){
				return new Color(1f, 0.8f, 0f, 1f);
			}
			return new Color(0f, 1f, 0.8f, 1f);
			
		default:
			return new Color(1f, 1f, 1f, 1f);
			
		}
	
		
	}
	
	
	
	#endregion
}
