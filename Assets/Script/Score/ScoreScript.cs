using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ScoreScript : MonoBehaviour {
	
	
	
	//Changer la police en fonction de la résolution
	
	#region Declaration
	public GUISkin skin;
	
	//Position element
	public Rect posScoringTitle;
	public Rect posCritism;
	public Rect posNote;
	public Vector2 specialFailPos = new Vector2(0.625f, 0.3f);
	public Rect posScore;
	public float offsetScore;
	public Rect posTitleArtistBPM;
	public Rect posDiff;
	public Rect posDiffNumber;
	public Rect posMods;
	public Rect posNotationRow1;
	public Rect posNotationRow1Tot;
	public Rect posNotationRow2;
	public Rect posNotationRow2Tot;
	public float offsetPosNot;
	public Rect posTexNotationRow1;
	public Rect posTexNotationRow2;
	public float offsetPosTex;
	public Rect posInfo;
	public float offsetPosInfo;
	public Rect posCombo;
	public Rect posRetry;
	public Rect posQuit;
	
	
	//Graph
	public GameObject graph;
	
	public Material bannerMat;
	
	public GameObject cubeDangerZone;
	
	
	//Medals
	public List<GameObject> medals;
	public Cubemap cmToChange;
	public Camera camToRender;
	
	//Texture
	private Dictionary<string, Texture2D> dicTex;
	
	//Score
	private int[] decoupeScore;
	
	//Note
	private string noteToDisplay;
	private string critismToDisplay;
	
	//Infos
	private double averagePrec;
	private int sens;
	private float percentSens;
	private int[] numberOfOthers;
	private int maxCombo;
	private string stringmod;
	
	//Fade
	private bool fadeToChartScene;
	private bool fadeToFreeScene;
	private float time;
	private bool fadeok;
	
	
	//Clignotement alpha
	private float alphaCombo;
	public float speedAlphaCombo;
	public float limitAlphaCombo;
	private float sensAlphaCombo;
	
	
	//Transition entry
	private bool statok;
	private bool graphok;
	private float alphaTransition;
	public float speedAlphaTrans;
	public Camera CameraMov;
	public GameObject cadreScore;
	public GameObject cadreGraph;
	public GameObject graphRenderer;
	public List<GameObject> openerCadreScore;
	public float center;
	public List<GameObject> openerGraphScore;
	public Vector2 centergraph;
	public Material cadreScoreMat;
	public Material cadreGraphMat;
	private List<LineRenderer> lrToActivate;
	private List<GameObject> dzToActivate;
	public float speedTransTableau;
	public float speedCameraMov;
	public float speedFlash;
	private float FlashCadre;
	public float FlashGraph;
	private bool transistionok;
	
	//TEST
	private float prectime;
	// Use this for initialization
	
	#endregion
	
	void Start () {
		//initTest();
			
		
		//Ajout dans le dictionnaire de texture
		
		dicTex = new Dictionary<string, Texture2D>();
		dicTex.Add("ScoreTitle", (Texture2D) Resources.Load("ScoringResult"));
		dicTex.Add("NoteS", (Texture2D) Resources.Load("NoteS"));
		dicTex.Add("NoteA", (Texture2D) Resources.Load("NoteA"));
		dicTex.Add("NoteB", (Texture2D) Resources.Load("NoteB"));
		dicTex.Add("NoteC", (Texture2D) Resources.Load("NoteC"));
		dicTex.Add("NoteBAD", (Texture2D) Resources.Load("NoteBAD"));
		dicTex.Add("FAIL", (Texture2D) Resources.Load("Fail"));
		for(int i=0;i<10;i++){
			dicTex.Add("S" + i, (Texture2D) Resources.Load("Numbers/S" + i));
		}
		dicTex.Add("PERCENT", (Texture2D) Resources.Load("Numbers/Percent"));
		dicTex.Add("DOT", (Texture2D) Resources.Load("Numbers/Dot"));
		decoupeScore = new int[5];
		for(int i=0;i<decoupeScore.Count();i++){
			decoupeScore[i] = 0;	
		}
		dicTex.Add("BEGINNER", (Texture2D) Resources.Load("Beginner"));
		dicTex.Add("EASY", (Texture2D) Resources.Load("easy"));
		dicTex.Add("MEDIUM", (Texture2D) Resources.Load("medium"));
		dicTex.Add("HARD", (Texture2D) Resources.Load("hard"));
		dicTex.Add("EXPERT", (Texture2D) Resources.Load("expert"));
		dicTex.Add("EDIT", (Texture2D) Resources.Load("edit"));
		
		for(int i=0;i<(int)ScoreCount.NONE;i++){
			var name = ((ScoreCount)i).ToString().ToLower();
			name = name.Replace(name[0], name[0].ToString().ToUpper().ToCharArray()[0]);
			dicTex.Add(((ScoreCount)i).ToString().ToUpper(), (Texture2D) Resources.Load(name));
		}
		
		
		dicTex.Add("-", (Texture2D) Resources.Load("underNote"));
		dicTex.Add("+", (Texture2D) Resources.Load("upperNote"));
		
		if(DataManager.Instance.perfect || DataManager.Instance.fullFantCombo 
			|| DataManager.Instance.fullExCombo || DataManager.Instance.fullCombo) 
			dicTex.Add("COMBO", DataManager.Instance.perfect ? (Texture2D) Resources.Load("Perfect") : 
				DataManager.Instance.fullFantCombo ? (Texture2D) Resources.Load("FFC") : 
				DataManager.Instance.fullExCombo ? (Texture2D) Resources.Load("FEC") : (Texture2D) Resources.Load("FC"));
		
		
		//Skybox scene
		RenderSettings.skybox = DataManager.Instance.skyboxList[DataManager.Instance.skyboxIndexSelected];
		//for optim
		numberOfOthers = new int[5];
		numberOfOthers[0] = DataManager.Instance.songSelected.numberOfFreezes;
		numberOfOthers[1] = DataManager.Instance.songSelected.numberOfRolls;
		numberOfOthers[2] = DataManager.Instance.songSelected.numberOfJumps;
		numberOfOthers[3] = DataManager.Instance.songSelected.numberOfHands;
		numberOfOthers[4] = DataManager.Instance.songSelected.numberOfMines;
		
		
		//Score
		
		fadeToFreeScene = false;
		fadeToChartScene = false;
		time = 0f;
		fadeok = false;
		alphaCombo = limitAlphaCombo;
		sensAlphaCombo = 1f;
		alphaTransition = 0f;
		transistionok = false;
		statok = false;
		graphok = false;
		FlashCadre = 1f;
		FlashGraph = 1f;
		ProcessScore();
	}
	
	// Update is called once per frame
	void Update () {
		if(!fadeok && time > 0.1f){
			GetComponent<FadeManager>().FadeOut();	
			fadeok = true;
			time = 0f;
		}
		
		if(fadeok && !transistionok){
			
			//Camera mov
			CameraMov.transform.eulerAngles = Vector3.Lerp(CameraMov.transform.eulerAngles, new Vector3(0f, 0f, 0f), Time.deltaTime/speedCameraMov);
			if(CameraMov.transform.eulerAngles.x <= 0.01f){
				CameraMov.transform.eulerAngles = new Vector3(0f, 0f, 0f);
				alphaTransition += Time.deltaTime/speedAlphaTrans;
				if(alphaTransition >= 1){
					alphaTransition = 1f;
					transistionok = true;	
				}
			}
			
			//Cadre stat
			if(time > 0.5f){
				if(Mathf.Abs(openerCadreScore.First().transform.position.y - 7.5f) <= 0.1f && !statok){
					for(int i=0;i<openerCadreScore.Count;i++){
						openerCadreScore.ElementAt(i).active = false;
					}	
					statok = true;
					cadreScore.SetActiveRecursively(true);
				}else{
					for(int i=0;i<openerCadreScore.Count;i++){
						openerCadreScore.ElementAt(i).transform.position = Vector3.Lerp(openerCadreScore.ElementAt(i).transform.position, new Vector3(openerCadreScore.ElementAt(i).transform.position.x, center + Mathf.Pow(-1, i)*3.2f, openerCadreScore.ElementAt(i).transform.position.z), Time.deltaTime/speedTransTableau );
					}
				}
				
				if(statok && FlashCadre > 0){
					FlashCadre -= Time.deltaTime/speedFlash;
					if(FlashCadre < 0) FlashCadre = 0f;
					cadreScoreMat.color = new Color(0f, 0.075f + (1-0.075f)*FlashCadre, 0.05f + (1-0.05f)*FlashCadre);
				}
				
				//Cadre graph
				if(Mathf.Abs(openerGraphScore.First().transform.position.x - 4f) <= 0.1f && !graphok){
					if(Mathf.Abs(openerGraphScore.First().transform.position.y - 2f) <= 0.1f && !graphok){
						for(int i=0;i<openerGraphScore.Count;i++){
							openerGraphScore.ElementAt(i).active = false;
						}	
						graphok = true;
						cadreGraph.SetActiveRecursively(true);
						foreach(var ls in lrToActivate){
							ls.gameObject.SetActiveRecursively(true);	
						}
						foreach(var ds in dzToActivate){
							ds.SetActiveRecursively(true);	
						}
					}else{
						for(int i=0;i<openerGraphScore.Count;i++){
							openerGraphScore.ElementAt(i).transform.position = Vector3.Lerp(openerGraphScore.ElementAt(i).transform.position, new Vector3(openerGraphScore.ElementAt(i).transform.position.x, centergraph.y + Mathf.Pow(-1, (int)((float)i/2f))*1f, openerGraphScore.ElementAt(i).transform.position.z), Time.deltaTime/speedTransTableau );
						}
					}
				}else{
					for(int i=0;i<openerGraphScore.Count;i++){
						openerGraphScore.ElementAt(i).transform.position = Vector3.Lerp(openerGraphScore.ElementAt(i).transform.position, new Vector3(centergraph.x + Mathf.Pow(-1, i)*6f, openerGraphScore.ElementAt(i).transform.position.y, openerGraphScore.ElementAt(i).transform.position.z), Time.deltaTime/speedTransTableau );
					}
				}
				
				if(graphok && FlashGraph > 0){
					FlashGraph -= Time.deltaTime/speedFlash;
					if(FlashGraph < 0) FlashGraph = 0f;
					cadreGraphMat.color = new Color(0f, 0.155f + (1-0.155f)*FlashGraph, 0.08f + (1-0.08f)*FlashGraph);
				}
			
			}else{
				time += Time.deltaTime;	
			}
			
		}else{
			if(time > 1.5f){
				if(fadeToChartScene){
					Application.LoadLevel("ChartScene");	
				}else if(fadeToFreeScene){
					Application.LoadLevel("Free");	
				}
			}
			
			
			if(!fadeok || fadeToChartScene || fadeToFreeScene){
				time += Time.deltaTime;	
			}
			
			if(fadeToChartScene || fadeToFreeScene){
				audio.volume -= Time.deltaTime/1.5f;	
			}
			
			
			alphaCombo += sensAlphaCombo*Time.deltaTime/speedAlphaCombo;
			if((sensAlphaCombo == -1f && alphaCombo < limitAlphaCombo) || (sensAlphaCombo == 1f && alphaCombo > 1f)){
				sensAlphaCombo *= -1f;	
			}
		}
		
	}
	
	
	#region GUI
	void AllLabel(bool shadow){
		GUI.Label(resizeRectGeneralOffset(resizeRect(posTitleArtistBPM), shadow), DataManager.Instance.songSelected.title + "\n" + DataManager.Instance.songSelected.artist + " - " + DataManager.Instance.songSelected.stepartist, "SongInfo");
		var col = DataManager.Instance.diffColor[(int)DataManager.Instance.difficultySelected];
		if (!shadow) GUI.color = new Color(col.r, col.g, col.b, alphaTransition);
		GUI.Label(resizeRectGeneralOffset(resizeRect(posDiffNumber), shadow), DataManager.Instance.songSelected.level.ToString() , "Level"); 
		if(!shadow) GUI.color = new Color(1f, 1f, 1f, alphaTransition);
		GUI.Label(resizeRectGeneralOffset(resizeRect(posMods), shadow), stringmod);
		
		if(statok){
			for(int i=0; i<(int)ScoreCount.FREEZE;i++){
				if(!shadow) GUI.color = DataManager.Instance.precColor[i];
				GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posNotationRow1, offsetPosNot, i), shadow), 
					DataManager.Instance.scoreCount[((ScoreCount)i).ToString()].ToString(), "Numbers");
			
				if(!shadow) GUI.color = new Color(1f, 1f, 1f, 1f);
				
				GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posNotationRow1Tot, offsetPosNot, i), shadow), 
					"(" + (((float)DataManager.Instance.scoreCount[((ScoreCount)i).ToString()] / (float)DataManager.Instance.songSelected.numberOfStepsWithoutJumps)*100f).ToString("0") + "%)");
			
			}
			
			
			for(int i=(int)ScoreCount.FREEZE; i<(int)ScoreCount.NONE;i++){
				if(!shadow && ((ScoreCount)i).ToString() != "MINE" && 
					DataManager.Instance.scoreCount[((ScoreCount)i).ToString()] != numberOfOthers[i - (int)ScoreCount.FREEZE] ){	
					GUI.color = DataManager.Instance.precColor[5];
				}else if(!shadow && ((ScoreCount)i).ToString() == "MINE" && 
					DataManager.Instance.scoreCount[((ScoreCount)i).ToString()] != 0){	
					GUI.color = DataManager.Instance.precColor[5];
				}else if(!shadow){
					GUI.color = new Color(1f, 1f, 1f, 1f);
				}
				
				GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posNotationRow2, offsetPosNot, i - (int)ScoreCount.FREEZE), shadow), 
					DataManager.Instance.scoreCount[((ScoreCount)i).ToString()].ToString(), "Numbers");
				
				if(!shadow) GUI.color = new Color(1f, 1f, 1f, 1f);
				
				
				GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posNotationRow2Tot, offsetPosNot, i - (int)ScoreCount.FREEZE), shadow), 
					"/ " + numberOfOthers[i - (int)ScoreCount.FREEZE]);
				
			}
		}
		
		if(!shadow) GUI.color = new Color(1f, 1f, 1f, alphaTransition);
		
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 0), shadow), "First ex or less : " + ((DataManager.Instance.firstEx == -1) ? "Never" : ((DataManager.Instance.firstEx/DataManager.Instance.songSelected.duration)*100f).ToString("0.00") + "%"));
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 1), shadow), "First great or less : " + ((DataManager.Instance.firstGreat == -1) ? "Never" : ((DataManager.Instance.firstGreat/DataManager.Instance.songSelected.duration)*100f).ToString("0.00") + "%"));
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 2), shadow), "First misteak : " + ((DataManager.Instance.firstMisteak == -1) ? "Never" : ((DataManager.Instance.firstMisteak/DataManager.Instance.songSelected.duration)*100f).ToString("0.00") + "%"));
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 3), shadow), "Average precision : " + averagePrec.ToString("0.000") + "ms");
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 4), shadow), "Average Timing : " + ((DataManager.Instance.perfect || DataManager.Instance.fullFantCombo) ? "None" : (sens > 0) ? "Too slow (" + percentSens.ToString("00") + "%)" : (sens < 0) ? "Too fast (" + percentSens.ToString("00") + "%)" : "Mixed fast/slow"));
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 5), shadow), "Max Combo : " + maxCombo);
	}
	
	void OnGUI(){
		
		
		GUI.Label(new Rect(0f, 0f, 100, 100), Screen.width + "x" + Screen.height);
		GUI.skin = skin;
		
		
		
		GUI.DrawTexture(resizeRect(posScoringTitle), dicTex["ScoreTitle"]);
		
		if(noteToDisplay != "MEDAL"){
			if(noteToDisplay == "FAIL"){
				posNote.x = specialFailPos.x;
				posNote.width = specialFailPos.y;
			}
			GUI.DrawTexture(resizeRect(posNote), dicTex[noteToDisplay]);
			if(!String.IsNullOrEmpty(critismToDisplay)){
				GUI.DrawTexture(resizeRect(posCritism), dicTex[critismToDisplay]);
			}
		}
		
		
		GUI.color = new Color(1f, 1f, 1f, alphaTransition);
		for(int i=0;i<5;i++){
			if(i > 1 || (i == 0 && (float)(decoupeScore[0]) != 0f) || (i == 1 && (((float)(decoupeScore[0]) != 0f) || (float)(decoupeScore[1]) != 0f))) GUI.DrawTexture(resizeRectOfX(posScore, offsetScore, i), dicTex["S" + decoupeScore[i]]);
			
		}
		GUI.DrawTexture(new Rect((posScore.x + offsetScore*2 + ((float)offsetScore/2f))*Screen.width, posScore.y*Screen.height, posScore.width*Screen.width, posScore.height*Screen.height), dicTex["DOT"]);
		GUI.DrawTexture(new Rect((posScore.x + offsetScore*5)*Screen.width, posScore.y*Screen.height, posScore.width*Screen.width, posScore.height*Screen.height), dicTex["PERCENT"]);
		
		
		GUI.DrawTexture(resizeRect(posDiff), dicTex[DataManager.Instance.difficultySelected.ToString()]);
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		if(statok){
			for(int i=0; i<(int)ScoreCount.FREEZE;i++){
				
				GUI.DrawTexture(resizeRectOfY(posTexNotationRow1, offsetPosTex, i), dicTex[((ScoreCount)i).ToString().ToUpper()]);
			}
			for(int i=(int)ScoreCount.FREEZE; i<(int)ScoreCount.NONE;i++){
				
				GUI.DrawTexture(resizeRectOfY(posTexNotationRow2, offsetPosTex, i - (int)ScoreCount.FREEZE), dicTex[((ScoreCount)i).ToString().ToUpper()]);
			}
		}
		
		GUI.color = new Color(1f, 1f, 1f, alphaTransition);
		if(dicTex.ContainsKey("COMBO")){
			GUI.color = new Color(1f, 1f, 1f, alphaCombo);
			GUI.DrawTexture(resizeRect(posCombo), dicTex["COMBO"]);
			
		}
		GUI.color = new Color(0f,0f,0f,0.7f*alphaTransition);
		AllLabel(true);
		GUI.color = new Color(1f,1f,1f,alphaTransition);
		AllLabel(false);
		
		GUI.color = new Color(1f, 1f, 1f, alphaTransition);
		if(GUI.Button(resizeRect(posRetry), "Retry") && fadeok && !fadeToChartScene && !fadeToFreeScene){
			fadeToChartScene = true;
			GetComponent<FadeManager>().FadeIn();	
		}
		
		if(GUI.Button(resizeRect(posQuit), "Quit") && fadeok && !fadeToChartScene && !fadeToFreeScene){
			fadeToFreeScene = true;
			GetComponent<FadeManager>().FadeIn();	
		}
	}
	#endregion
	
	#region util
	Rect resizeRect(Rect r){
		return new Rect(r.x*Screen.width, r.y*Screen.height, r.width*Screen.width, r.height*Screen.height);
	}
	
	Rect resizeRectOfY(Rect r, float offset, int ite){
		return new Rect(r.x*Screen.width, (r.y + offset*ite)*Screen.height, r.width*Screen.width, r.height*Screen.height);
	}
	
	Rect resizeRectOfX(Rect r, float offset, int ite){
		return new Rect((r.x + offset*ite)*Screen.width, r.y*Screen.height, r.width*Screen.width, r.height*Screen.height);
	}
	
	Rect resizeRectGeneralOffset(Rect r, bool doIt){
		if(doIt) return new Rect(r.x + 2f, r.y + 1f, r.width, r.height);	
		return r;
		
	}
	#endregion
	
	#region process
	void ProcessScore(){
	
		bannerMat.mainTexture = DataManager.Instance.songSelected.GetBanner();
			
		sens = 0;
	
		//Score
		var decoupe = DataManager.Instance.scoreEarned.ToString("000.00").Trim();
		decoupeScore[0] = System.Int32.Parse(""+decoupe[0]);
		decoupeScore[1] = System.Int32.Parse(""+decoupe[1]);
		decoupeScore[2] = System.Int32.Parse(""+decoupe[2]);
		decoupeScore[3] = System.Int32.Parse(""+decoupe[4]);
		decoupeScore[4] = System.Int32.Parse(""+decoupe[5]);
		
		//Note
		if(DataManager.Instance.fail){
			noteToDisplay = "FAIL";
			critismToDisplay = "";
		}else if(DataManager.Instance.scoreEarned >= 96f){
			noteToDisplay = "MEDAL";
		}else{
			noteToDisplay = "Note" + DataManager.Instance.giveNoteOfScore(DataManager.Instance.scoreEarned).Split(';')[1];
			critismToDisplay = DataManager.Instance.giveNoteOfScore(DataManager.Instance.scoreEarned).Split(';')[0];
			if(critismToDisplay == "=") critismToDisplay = "";
		}
		
		//Percent and stat
		double theav = 0;
		int signmoins = 0;
		int signplus = 0;
		var listprec = DataManager.Instance.precAverage.Where(c => Mathf.Abs((float)c) > DataManager.Instance.PrecisionValues[Precision.FANTASTIC]).ToList();
		for(int i=0; i<listprec.Count; i++){
			theav += Mathf.Abs((float)listprec[i]);
			if(listprec[i] > DataManager.Instance.PrecisionValues[Precision.FANTASTIC]) signplus++;
			if(listprec[i] < -DataManager.Instance.PrecisionValues[Precision.FANTASTIC]) signmoins++;
		}
		var l = listprec.Count();
		averagePrec = theav/(double)l;
		if(((float)signplus/(float)l) > 0.6f){ 
			percentSens = ((float)signplus/(float)l)*100f;
			sens = 1;
		}
		if(((float)signmoins/(float)l) > 0.6f){
			percentSens = ((float)signmoins/(float)l)*100f;
			sens = -1;
		}
		
		//Mods
		stringmod = "";
		stringmod += "x" + DataManager.Instance.speedmodSelected + ", ";
		if(DataManager.Instance.hitJudgeSelected != Judge.NORMAL) stringmod += DataManager.Instance.dicHitJudge[DataManager.Instance.hitJudgeSelected] + ", ";
		if(DataManager.Instance.lifeJudgeSelected != Judge.NORMAL) stringmod += DataManager.Instance.dicLifeJudge[DataManager.Instance.lifeJudgeSelected] + ", ";
		if(DataManager.Instance.scoreJudgeSelected != Judge.NORMAL) stringmod += DataManager.Instance.dicScoreJudge[DataManager.Instance.scoreJudgeSelected] + ", ";
		
		for(int i=0; i< DataManager.Instance.displaySelected.Count(); i++){
			if(DataManager.Instance.displaySelected[i]){
				stringmod += DataManager.Instance.aDisplay[i] + ", ";	
			}
		}
		stringmod = stringmod.Remove(stringmod.Length - 2, 2);
		
		//Combo
		maxCombo = DataManager.Instance.timeCombo.Max(c => c.Value);
		
		//Graph
		var life = new double[200];
		var dangerZone = new Dictionary<float, float>();
		var deathZone = new Dictionary<float, float>();
		var beginDanger = 0f;
		var endDanger = 0f;
		var iminDangerZone = false;
		var iminDeathZone = false;
			
			
		var thecut = DataManager.Instance.songSelected.duration/(double)200;
		double thelastgoodvalue = 50;
		/*var theaddCut = (double)0;
		var thelastCut = (double)0;
		var thenumberaddCut = 0;
		var thetimecut = (double)0;
		var indexTab = 0;*/
		
		
		for(int i=0; i<200; i++){
			if(i == 0){
				life[i] = 50;
			}else{
				if(DataManager.Instance.lifeGraph.Where(c => c.Key <= thecut*i).Count() == 0){
					life[i] = thelastgoodvalue;
				}else{
					double moy = 0;
					int numbermoy = 0;
					List<double> keyToRemove = new List<double>();
					foreach(var val in DataManager.Instance.lifeGraph.Where(c => c.Key <= (thecut*i))){
						moy += val.Value;
						thelastgoodvalue = val.Value;
						keyToRemove.Add(val.Key);
						numbermoy++;
					}
					foreach(var rem in keyToRemove){
						DataManager.Instance.lifeGraph.Remove(rem);	
					}
					life[i] = moy/(double)numbermoy;
				}
				
			}
			
			if(life[i] > 5f && iminDeathZone){
				iminDeathZone = false;
				endDanger = i;
				var offset = 0f;
				if(i != 0){
					offset = (5f-(float)life[i-1])/(float)(life[i] - life[i - 1]);
				}
				deathZone.Add(beginDanger, endDanger + offset);
			}
			
			if((life[i] > 25f) && iminDangerZone){
				endDanger = i;
				iminDangerZone = false;
				var offset = 0f;
				if(i != 0){
					offset = (25f-(float)life[i-1])/(float)(life[i] - life[i - 1]);
				}
				dangerZone.Add(beginDanger, endDanger + offset);
			}
			
			if(life[i] <= 5f && !iminDeathZone){
				iminDeathZone = true;
				var offset = 0f;
				if(i != 0){
					offset = (5f-(float)life[i])/(float)(life[i - 1] - life[i]);
				}
				beginDanger = i - offset;
			}
			
			if(life[i] <= 25f && !iminDangerZone){
				iminDangerZone = true;
				var offset = 0f;
				if(i != 0){
					offset = (25f-(float)life[i])/(float)(life[i - 1] - life[i]);
				}
				beginDanger = i - offset;
			}
		}
		
		/* old system
		
		var theaddCut = (double)0;
		var thelastCut = (double)0;
		var thenumberaddCut = 0;
		var thetimecut = (double)0;
		var indexTab = 0;
		
		for(int i=0;i<DataManager.Instance.lifeGraph.Count;i++){
			if(i == 0){
				thetimecut = DataManager.Instance.lifeGraph.ElementAt(i).Key;
			}else{
				thetimecut += DataManager.Instance.lifeGraph.ElementAt(i).Key - DataManager.Instance.lifeGraph.ElementAt(i- 1).Key;
			}
			
			theaddCut += DataManager.Instance.lifeGraph.ElementAt(i).Value;
			thelastCut = DataManager.Instance.lifeGraph.ElementAt(i).Value;
			thenumberaddCut++;
			while(thetimecut > thecut){
				life[indexTab] = theaddCut/(double)thenumberaddCut;
				thetimecut -= thecut;	
				thenumberaddCut = 1;
				theaddCut = thelastCut;
				
				if(life[indexTab] > 5f && iminDeathZone){
					iminDeathZone = false;
					endDanger = indexTab;
					var offset = 0f;
					if(indexTab != 0){
						offset = (5f-(float)life[indexTab-1])/(float)(life[indexTab] - life[indexTab - 1]);
					}
					deathZone.Add(beginDanger, endDanger + offset);
				}
				
				if((life[indexTab] > 25f) && iminDangerZone){
					endDanger = indexTab;
					iminDangerZone = false;
					var offset = 0f;
					if(indexTab != 0){
						offset = (25f-(float)life[indexTab-1])/(float)(life[indexTab] - life[indexTab - 1]);
					}
					dangerZone.Add(beginDanger, endDanger + offset);
				}
				
				if(life[indexTab] <= 5f && !iminDeathZone){
					iminDeathZone = true;
					var offset = 0f;
					if(indexTab != 0){
						offset = (5f-(float)life[indexTab])/(float)(life[indexTab - 1] - life[indexTab]);
					}
					beginDanger = indexTab - offset;
				}
				
				if(life[indexTab] <= 25f && !iminDangerZone){
					iminDangerZone = true;
					var offset = 0f;
					if(indexTab != 0){
						offset = (25f-(float)life[indexTab])/(float)(life[indexTab - 1] - life[indexTab]);
					}
					beginDanger = indexTab - offset;
				}
				
				indexTab++;
				
				
				
			}
		}
		
		var thelasttime = thetimecut;
		var thelastvalue = DataManager.Instance.lifeGraph.Last().Value;
		while(thelasttime + thecut < DataManager.Instance.songSelected.duration && indexTab < 200){
			life[indexTab] = thelastvalue;
			indexTab++;
		};
		if(indexTab < 200)life[indexTab] = thelastvalue;*/
		
		
		var indexfe = -1;
		var indexfg = -1;
		var indexfm = -1;
		var dur = DataManager.Instance.songSelected.duration;
		
		if(DataManager.Instance.firstEx == -1){
			indexfe = 200;
		}
		if(DataManager.Instance.firstGreat == -1){
			indexfg = 200;	
		}
		if(DataManager.Instance.firstMisteak == -1){
			indexfm = 200;	
		}
		for(int i=0; i<200; i++){
			if(indexfe == -1 && DataManager.Instance.firstEx - DataManager.Instance.firstArrow <= (dur/200f)*i){
				indexfe = i;	
			}
			if(indexfg == -1 && DataManager.Instance.firstGreat - DataManager.Instance.firstArrow <= (dur/200f)*i){
				indexfg = i;	
			}
			if(indexfm == -1 && DataManager.Instance.firstMisteak - DataManager.Instance.firstArrow <= (dur/200f)*i){
				indexfm = i;	
			}
		}
		var graphFant = ((GameObject) Instantiate(graph, graph.transform.position, graph.transform.rotation)).GetComponent<LineRenderer>();
		var graphEx = ((GameObject) Instantiate(graph, graph.transform.position, graph.transform.rotation)).GetComponent<LineRenderer>();
		var graphGreat = ((GameObject) Instantiate(graph, graph.transform.position, graph.transform.rotation)).GetComponent<LineRenderer>();
		var graphAll = ((GameObject) Instantiate(graph, graph.transform.position, graph.transform.rotation)).GetComponent<LineRenderer>();
		lrToActivate = new List<LineRenderer>();
		if(indexfe != 0){
			graphFant.SetVertexCount(indexfe + 1);
			//graphFant.SetColors(DataManager.Instance.precColor[0], DataManager.Instance.precColor[0]);
			graphFant.materials.First().color = DataManager.Instance.precColor[0];
			for(int i=0;i<=indexfe;i++){
				graphFant.SetPosition(i, new Vector3( -160f + (240f*((float)i/200f)) , 19f*(((float)life[i] - 50f)/50f), 0f));
			}
			graphFant.gameObject.SetActiveRecursively(false);
			lrToActivate.Add(graphFant);
		}else{
			Destroy(graphFant.gameObject);	
		}
		
		if(indexfe != indexfg && indexfe != 200){
			graphEx.SetVertexCount(indexfg - indexfe + 1);
			//graphEx.SetColors(DataManager.Instance.precColor[1], DataManager.Instance.precColor[1]);
			graphEx.materials.First().color = DataManager.Instance.precColor[1];
			for(int i=indexfe;i<=indexfg;i++){
				graphEx.SetPosition(i - indexfe, new Vector3( -160f + (240f*((float)i/200f)) , 19f*(((float)life[i] - 50f)/50f), 0f));
			}	
			graphEx.gameObject.SetActiveRecursively(false);
			lrToActivate.Add(graphEx);
		}else{
			Destroy(graphEx.gameObject);	
		}
		
		if(indexfg != indexfm && indexfg != 200){
			graphGreat.SetVertexCount(indexfm - indexfg + 1);
			//graphGreat.SetColors(DataManager.Instance.precColor[2], DataManager.Instance.precColor[2]);
			graphGreat.materials.First().color = DataManager.Instance.precColor[2];
			for(int i=indexfg;i<=indexfm;i++){
				graphGreat.SetPosition(i - indexfg, new Vector3( -160f + (240f*((float)i/200f)) , 19f*(((float)life[i] - 50f)/50f), 0f));
			}	
			graphGreat.gameObject.SetActiveRecursively(false);
			lrToActivate.Add(graphGreat);
		}else{
			Destroy(graphGreat.gameObject);	
		}
		
		if(indexfm != 200){
			graphAll.SetVertexCount(200 - indexfm);
			//graphAll.SetColors(new Color(0f, 0.3f, 0.1f, 1f), new Color(0f, 0.3f, 0.1f, 1f));
			graphAll.materials.First().color = new Color(0.85f, 1f, 0.85f, 1f);
			for(int i=indexfm;i<200;i++){
				graphAll.SetPosition(i - indexfm, new Vector3( -160f + (240f*((float)i/200f)) , 19f*(((float)life[i] - 50f)/50f), 0f));
			}
			graphAll.gameObject.SetActiveRecursively(false);
			lrToActivate.Add(graphAll);
		}else{
			Destroy(graphAll);	
		}
		
		/*
		for(int i=0;i<200;i++){
			
			graph.SetPosition(i, new Vector3( -160f + (240f*((float)i/200f)) , 19f*(((float)life[i] - 50f)/50f), 0f));
		}*/
		
		//Graph dangerzone
		
		foreach(var dz in dangerZone){
			var debut = (dz.Key/200f)*12f - 8f;	
			var fin = (dz.Value/200f)*12f - 8f;	
			
			var thezone = (GameObject) Instantiate(cubeDangerZone, new Vector3(((fin + debut)/2f), cubeDangerZone.transform.position.y, cubeDangerZone.transform.position.z), cubeDangerZone.transform.rotation);
			thezone.transform.localScale = new Vector3((fin - debut), thezone.transform.localScale.y, thezone.transform.localScale.z);
			thezone.renderer.material.color = new Color(1f, 1f, 0f, 0.3f);
			thezone.SetActiveRecursively(false);
			dzToActivate.Add(thezone);
		}
		
		foreach(var dz in deathZone){
			var debut = ((float)dz.Key/200f)*12f - 8f;	
			var fin = ((float)dz.Value/200f)*12f - 8f;	
			
			var thezone = (GameObject) Instantiate(cubeDangerZone, new Vector3(((fin + debut)/2f), cubeDangerZone.transform.position.y, cubeDangerZone.transform.position.z), cubeDangerZone.transform.rotation);
			thezone.transform.localScale = new Vector3((fin - debut), thezone.transform.localScale.y, thezone.transform.localScale.z);
			thezone.renderer.material.color = new Color(1f, 0f, 0f, 0.3f);
			thezone.SetActiveRecursively(false);
			dzToActivate.Add(thezone);
		}
		
		
		
		//Medal
		var indexMedal = -1;
		if(DataManager.Instance.scoreEarned >= 96f){
				indexMedal = (int)(DataManager.Instance.scoreEarned - 96);
				if(indexMedal > 0) indexMedal -= 1;
		}
		if(indexMedal != -1){
			medals.ElementAt(indexMedal).SetActiveRecursively(true);
		}
		
		
		camToRender.RenderToCubemap(cmToChange);
		camToRender.gameObject.active = false;
		
	}
	#endregion
	
	void initTest(){
	
		LoadManager.Instance.Loading();
		
		var rand = (int)(UnityEngine.Random.value*DataManager.Instance.skyboxList.Count);
		if(rand == DataManager.Instance.skyboxList.Count){
			rand--;	
		}
		DataManager.Instance.skyboxIndexSelected = rand;
			
		DataManager.Instance.songSelected = LoadManager.Instance.FindSong("SongTest", "Hide and Seek")[Difficulty.EXPERT];
		DataManager.Instance.scoreEarned = 100.00f;
		
		DataManager.Instance.precAverage = new List<double>();
		DataManager.Instance.precAverage.Add(0.05);
		DataManager.Instance.precAverage.Add(0.02);
		DataManager.Instance.precAverage.Add(-0.05);
		DataManager.Instance.precAverage.Add(-0.001);
		DataManager.Instance.precAverage.Add(0.05);
		DataManager.Instance.precAverage.Add(0.02);
		DataManager.Instance.precAverage.Add(-0.05);
		DataManager.Instance.precAverage.Add(-0.06);
		DataManager.Instance.precAverage.Add(-0.09);
		DataManager.Instance.precAverage.Add(-0.001);
		
		DataManager.Instance.timeCombo = new Dictionary<double, int>();
	
		DataManager.Instance.lifeGraph = new Dictionary<double, double>();
		var dur = DataManager.Instance.songSelected.duration;
		for(double i=0; i<(dur-1);i += (dur - 1)/200f){
			DataManager.Instance.lifeGraph.Add((double)i, (double)UnityEngine.Random.value*100);	
		}
		DataManager.Instance.firstEx = 15.2;
		
		DataManager.Instance.firstGreat = 31.2;
		
		DataManager.Instance.firstMisteak = 50.7;
		
		DataManager.Instance.perfect = false;
		DataManager.Instance.fullFantCombo = true;
		DataManager.Instance.fullExCombo = true;
		DataManager.Instance.fullCombo = true;
		DataManager.Instance.fail = false;
		DataManager.Instance.scoreCount = new Dictionary<string, int>();
		DataManager.Instance.difficultySelected = Difficulty.EXPERT;
		
		foreach(ScoreCount el2 in Enum.GetValues(typeof(ScoreCount))){
			if(el2 != ScoreCount.NONE){
				DataManager.Instance.scoreCount.Add(el2.ToString(), (int)(UnityEngine.Random.value*DataManager.Instance.songSelected.numberOfStepsWithoutJumps));
				//Debug.Log(el2.ToString());
			}
		}
		DataManager.Instance.speedmodSelected = 4f;
		DataManager.Instance.hitJudgeSelected = Judge.EASY;
		DataManager.Instance.lifeJudgeSelected = Judge.HARD;
		DataManager.Instance.scoreJudgeSelected = Judge.EXPERT;
		DataManager.Instance.displaySelected = new bool[DataManager.Instance.aDisplay.Count()];
		DataManager.Instance.displaySelected[0] = true;
		DataManager.Instance.displaySelected[1] = true;
		DataManager.Instance.displaySelected[2] = true;
		DataManager.Instance.displaySelected[3] = true;
		DataManager.Instance.displaySelected[4] = true;
		DataManager.Instance.displaySelected[5] = true;
		DataManager.Instance.displaySelected[6] = true;
		DataManager.Instance.displaySelected[7] = true;
		DataManager.Instance.displaySelected[8] = true;
		
		
		DataManager.Instance.timeCombo.Add(0.752,50);
		DataManager.Instance.timeCombo.Add(5.752,250);
		DataManager.Instance.timeCombo.Add(10.752,60);
	}
	
}
