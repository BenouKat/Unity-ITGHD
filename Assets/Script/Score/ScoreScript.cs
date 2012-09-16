using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ScoreScript : MonoBehaviour {
	
	
	
	//Changer la police en fonction de la résolution
	
	public GUISkin skin;
	public Rect posScoringTitle;
	public Rect posCritism;
	public Rect posNote;
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
	
	public LineRenderer graph;
	
	private Dictionary<string, Texture2D> dicTex;
	
	private int[] decoupeScore;
	private string noteToDisplay;
	private string critismToDisplay;
	private double averagePrec;
	private int sens;
	private float percentSens;
	private int[] numberOfOthers;
	// Use this for initialization
	void Start () {
		//initTest();
			
		
		numberOfOthers = new int[5];
		numberOfOthers[0] = DataManager.Instance.songSelected.numberOfFreezes;
		numberOfOthers[1] = DataManager.Instance.songSelected.numberOfRolls;
		numberOfOthers[2] = DataManager.Instance.songSelected.numberOfJumps;
		numberOfOthers[3] = DataManager.Instance.songSelected.numberOfHands;
		numberOfOthers[4] = DataManager.Instance.songSelected.numberOfMines;
		
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

		ProcessScore();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void AllLabel(bool shadow){
		GUI.Label(resizeRectGeneralOffset(resizeRect(posTitleArtistBPM), shadow), DataManager.Instance.songSelected.title + "\n" + DataManager.Instance.songSelected.artist + " - " + DataManager.Instance.songSelected.stepartist, "SongInfo");
		
		if (!shadow) GUI.color = DataManager.Instance.diffColor[(int)DataManager.Instance.difficultySelected];
		GUI.Label(resizeRectGeneralOffset(resizeRect(posDiffNumber), shadow), DataManager.Instance.songSelected.level.ToString() , "Level"); 
		if(!shadow) GUI.color = new Color(1f, 1f, 1f, 1f);
		GUI.Label(resizeRectGeneralOffset(resizeRect(posMods), shadow), "mods1, mods2, mods3, mods4, mods5, mods6");
		
		for(int i=0; i<(int)ScoreCount.FREEZE;i++){
			if(!shadow) GUI.color = DataManager.Instance.precColor[i];
			GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posNotationRow1, offsetPosNot, i), shadow), 
				DataManager.Instance.scoreCount[((ScoreCount)i).ToString()].ToString(), "Numbers");
		
			if(!shadow) GUI.color = new Color(1f, 1f, 1f, 1f);
			
			GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posNotationRow1Tot, offsetPosNot, i), shadow), 
				"(" + (((float)DataManager.Instance.scoreCount[((ScoreCount)i).ToString()] / (float)DataManager.Instance.songSelected.numberOfStepsWithoutJumps)*100f).ToString("0") + "%)");
		
		}
		
		
		for(int i=(int)ScoreCount.FREEZE; i<(int)ScoreCount.NONE;i++){
			if(!shadow && DataManager.Instance.scoreCount[((ScoreCount)i).ToString()] != numberOfOthers[i - (int)ScoreCount.FREEZE]){	
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
		
		if(!shadow) GUI.color = new Color(1f, 1f, 1f, 1f);
		
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 0), shadow), "First ex or less : " + DataManager.Instance.firstEx.ToString("0.00") + "%");
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 1), shadow), "First great or less : " + DataManager.Instance.firstGreat.ToString("0.00") + "%");
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 2), shadow), "First misteak : " + DataManager.Instance.firstMisteak.ToString("0.00") + "%");
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 3), shadow), "Average precision : " + averagePrec.ToString("0.000") + "ms");
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 4), shadow), "Average Timing : " + ((DataManager.Instance.perfect || DataManager.Instance.fullFantCombo) ? "None" : (sens > 0) ? "Too slow (" + percentSens.ToString("00") + "%)" : (sens < 0) ? "Too fast (" + percentSens.ToString("00") + "%)" : "Mixed fast/slow"));
		GUI.Label(resizeRectGeneralOffset(resizeRectOfY(posInfo, offsetPosInfo, 5), shadow), "Max Combo : ");
	}
	
	void OnGUI(){
		
		
		GUI.Label(new Rect(0f, 0f, 100, 100), Screen.width + "x" + Screen.height);
		GUI.skin = skin;
		
		
		GUI.DrawTexture(resizeRect(posScoringTitle), dicTex["ScoreTitle"]);
		if(noteToDisplay == "FAIL"){
			posNote.x = 0.625f;
			posNote.width = 0.3f; //dirty !!! Fix later :)
		}
		GUI.DrawTexture(resizeRect(posNote), dicTex[noteToDisplay]);
		if(!String.IsNullOrEmpty(critismToDisplay)){
			GUI.DrawTexture(resizeRect(posCritism), dicTex[critismToDisplay]);
		}
		
		
		for(int i=0;i<5;i++){
			if(i > 1 || (i == 0 && (float)(decoupeScore[0]) != 0f) || (i == 1 && (float)(decoupeScore[1]) != 0f)) GUI.DrawTexture(resizeRectOfX(posScore, offsetScore, i), dicTex["S" + decoupeScore[i]]);
			
		}
		GUI.DrawTexture(new Rect((posScore.x + offsetScore*2 + ((float)offsetScore/2f))*Screen.width, posScore.y*Screen.height, posScore.width*Screen.width, posScore.height*Screen.height), dicTex["DOT"]);
		GUI.DrawTexture(new Rect((posScore.x + offsetScore*5)*Screen.width, posScore.y*Screen.height, posScore.width*Screen.width, posScore.height*Screen.height), dicTex["PERCENT"]);
		
		
		GUI.DrawTexture(resizeRect(posDiff), dicTex[DataManager.Instance.difficultySelected.ToString()]);
		
		for(int i=0; i<(int)ScoreCount.FREEZE;i++){
			
			GUI.DrawTexture(resizeRectOfY(posTexNotationRow1, offsetPosTex, i), dicTex[((ScoreCount)i).ToString().ToUpper()]);
		}
		for(int i=(int)ScoreCount.FREEZE; i<(int)ScoreCount.NONE;i++){
			
			GUI.DrawTexture(resizeRectOfY(posTexNotationRow2, offsetPosTex, i - (int)ScoreCount.FREEZE), dicTex[((ScoreCount)i).ToString().ToUpper()]);
		}
		
		if(dicTex.ContainsKey("COMBO")) GUI.DrawTexture(resizeRect(posCombo), dicTex["COMBO"]);
		GUI.color = new Color(0f,0f,0f,0.7f);
		AllLabel(true);
		GUI.color = new Color(1f,1f,1f,1f);
		AllLabel(false);
		
	}
	
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
	
	void ProcessScore(){
	
		sens = 0;
	
		var decoupe = DataManager.Instance.scoreEarned.ToString("000.00").Trim();
		decoupeScore[0] = System.Int32.Parse(""+decoupe[0]);
		decoupeScore[1] = System.Int32.Parse(""+decoupe[1]);
		decoupeScore[2] = System.Int32.Parse(""+decoupe[2]);
		decoupeScore[3] = System.Int32.Parse(""+decoupe[4]);
		decoupeScore[4] = System.Int32.Parse(""+decoupe[5]);
		
		if(DataManager.Instance.fail){
			noteToDisplay = "FAIL";
			critismToDisplay = "";
		}else{
			noteToDisplay = "Note" + DataManager.Instance.giveNoteOfScore(DataManager.Instance.scoreEarned).Split(';')[1];
			critismToDisplay = DataManager.Instance.giveNoteOfScore(DataManager.Instance.scoreEarned).Split(';')[0];
			if(critismToDisplay == "=") critismToDisplay = "";
		}
		
		
		double theav = 0;
		int signmoins = 0;
		int signplus = 0;
		for(int i=0; i<DataManager.Instance.precAverage.Count; i++){
			theav += Mathf.Abs((float)DataManager.Instance.precAverage[i]);
			if(DataManager.Instance.precAverage[i] > DataManager.Instance.PrecisionValues[Precision.FANTASTIC]) signplus++;
			if(DataManager.Instance.precAverage[i] < -DataManager.Instance.PrecisionValues[Precision.FANTASTIC]) signmoins++;
		}
		var l = DataManager.Instance.precAverage.Where(c => Mathf.Abs((float)c) > DataManager.Instance.PrecisionValues[Precision.FANTASTIC]).Count();
		averagePrec = theav/(double)l;
		if(((float)signplus/(float)l) > 0.6f){ 
			percentSens = ((float)signplus/(float)l)*100f;
			sens = 1;
		}
		if(((float)signmoins/(float)l) > 0.6f){
			percentSens = ((float)signplus/(float)l)*100f;
			sens = -1;
		}
		
		
		
		var life = new double[200];
		
		var thecut = DataManager.Instance.songSelected.duration/(double)200;
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
				life[indexTab] = theaddCut/thenumberaddCut;
				thetimecut -= thecut;	
				thenumberaddCut = 1;
				theaddCut = thelastCut;
				indexTab++;
			}
		}
		
		var thelasttime = thetimecut;
		var thelastvalue = DataManager.Instance.lifeGraph.Last().Value;
		while(thelasttime + thecut < DataManager.Instance.songSelected.duration && indexTab < 200){
			life[indexTab] = thelastvalue;
			indexTab++;
		};
		if(indexTab < 200)life[indexTab] = thelastvalue;
		
		for(int i=0;i<200;i++){
			graph.SetPosition(i, new Vector3( -80f + (120f*((float)i/200f)) ,-20f + 10f*(((float)life[i] - 50f)/50f), 0f));
		}
	}
	
	
	void initTest(){
	
		LoadManager.Instance.Loading();
		
		DataManager.Instance.songSelected = LoadManager.Instance.FindSong("SongTest", "Hide and Seek")[Difficulty.EXPERT];
		DataManager.Instance.scoreEarned = 88.00f;
	
		DataManager.Instance.precAverage = new List<double>();
		DataManager.Instance.precAverage.Add(0.05);
		DataManager.Instance.precAverage.Add(0.02);
		DataManager.Instance.precAverage.Add(-0.05);
		DataManager.Instance.precAverage.Add(-0.001);
		DataManager.Instance.precAverage.Add(0.05);
		DataManager.Instance.precAverage.Add(0.02);
		DataManager.Instance.precAverage.Add(-0.05);
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
		DataManager.Instance.fullFantCombo = false;
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
	}
	
}
