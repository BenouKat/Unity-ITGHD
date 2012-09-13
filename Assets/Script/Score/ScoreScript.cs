using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreScript : MonoBehaviour {
	
	
	public Rect posScoringTitle;
	public Rect posNote;
	public Rect posScore;
	public float offsetScore;
	
	public Rect posTitleArtistBPM;
	public Rect posDiff;
	public Rect posDiffNumber;
	public Rect posMods;
	
	public Rect posNotationRow1;
	public Rect posNotationRow2;
	public Rect posNotationRow3;
	public float offsetPosNot;
	public float offsetPosNot3;
	
	public Rect posInfo;
	public float offsetPosInfo;
	
	public Rect posCombo;
	
	public LineRenderer
	
	
	private Dictionary<string, Texture2D> dicTex;
	
	private int[] decoupeScore;
	private string noteToDisplay;
	private double averagePrec;
	private int sens;
	private float percentSens;
	// Use this for initialization
	void Start () {
		dicTex.Add("ScoreTitle", (Texture2D) Resources.Load("ScoringResult"));
		dicTex.Add("NoteS", (Texture2D) Resources.Load("NoteS"));
		dicTex.Add("NoteA", (Texture2D) Resources.Load("NoteA"));
		dicTex.Add("NoteB", (Texture2D) Resources.Load("NoteB"));
		dicTex.Add("NoteC", (Texture2D) Resources.Load("NoteC"));
		dicTex.Add("NoteBAD", (Texture2D) Resources.Load("NoteBAD"));
		dicTex.Add("Fail", (Texture2D) Resources.Load("Fail"));
		for(int i=0;i<10;i++){
			dicTex.Add("S" + i, (Texture2D) Resources.Load("Numbers/S" + i));
		}
		dicTex.Add("PERCENT", (Texture2D) Resources.Load("Percent"));
		dicTex.Add("DOT", (Texture2D) Resources.Load("Dot"));
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
		
		if(DataManager.Instance.perfect || DataManager.Instance.fullFantCombo || DataManager.Instance.fullExCombo || DataManager.Instance.fullCombo) dicTex.Add("COMBO", perfect ? (Texture2D) Resources.Load("Perfect") : fullFantCombo ? (Texture2D) Resources.Load("FFC") : fullExCombo ? (Texture2D) Resources.Load("FEC") : (Texture2D) Resources.Load("FC"));
		
		DataManager.Instance.scoreEarned = 90f; //test
		
		
		
		
		
		ProcessScore();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.DrawTexture(resizeRect(posScoringTitle), dicTex["ScoreTitle"]);
		
		GUI.DrawTexture(resizeRect(posNote), dicTex[noteToDisplay]);
		
		for(int i=0;i<5;i++){
			if(i > 1 || (i == 0 && (float)(decoupeScore[0]) != 0f) || (i == 1 && (float)(decoupeScore[1]) != 0f)) GUI.DrawTexture(resizeRectOfX(posScore, offsetScore, i), dicTex["S" + decoupeScore[i]]);
		}
		GUI.DrawTexture(new Rect((posScore.x + offsetScore*3 + ((float)offsetScore/2f))*Screen.width, posScore.y*Screen.height, posScore.width*Screen.width, posScore.height*Screen.height), dicTex["DOT"]);
		GUI.DrawTexture(new Rect((posScore.x + offsetScore*5)*Screen.width, posScore.y*Screen.height, posScore.width*Screen.width, posScore.height*Screen.height), dicTex["PERCENT"]);
		
		GUI.Label(resizeRect(posTitleArtistBPM), DataManager.Instance.songSelected.title + " - " + DataManager.Instance.songSelected.artist + " - " + DataManager.Instance.songSelected.stepartist);
		GUI.DrawTexture(resizeRect(posDiff), dicTex[DataManager.Instance.diffSelected.toString()]);
		GUI.Label(resizeRect(posDiffNumber), DataManager.Instance.songSelected.level);
		GUI.Label(resizeRect(posMods), "mods");
		
		if(dicTex.ContainsKey("COMBO") GUI.DrawTexture(resizeRect(posCombo), dicTex["COMBO"]);
		
		GUI.Label(resizeRectOfY(posNotationRow1, offsetPosNot, 0), DataManager.Instance.scoreCount["FANTASTIC"]);
		GUI.Label(resizeRectOfY(posNotationRow1, offsetPosNot, 1), DataManager.Instance.scoreCount["EXCELLENT"]);
		GUI.Label(resizeRectOfY(posNotationRow1, offsetPosNot, 2), DataManager.Instance.scoreCount["GREAT"]);
		GUI.Label(resizeRectOfY(posNotationRow2, offsetPosNot, 0), DataManager.Instance.scoreCount["DECENT"]);
		GUI.Label(resizeRectOfY(posNotationRow2, offsetPosNot, 1), DataManager.Instance.scoreCount["WAYOFF"]);
		GUI.Label(resizeRectOfY(posNotationRow2, offsetPosNot, 2), DataManager.Instance.scoreCount["MISS"]);
		
		GUI.Label(resizeRectOfY(posNotationRow3, offsetPosNot3, 0), DataManager.Instance.scoreCount["JUMPS"]);
		GUI.Label(resizeRectOfY(posNotationRow3, offsetPosNot3, 1), DataManager.Instance.scoreCount["HANDS"]);
		GUI.Label(resizeRectOfY(posNotationRow3, offsetPosNot3, 2), DataManager.Instance.scoreCount["FREEZE"]);
		GUI.Label(resizeRectOfY(posNotationRow3, offsetPosNot3, 3), DataManager.Instance.scoreCount["ROLL"]);
		GUI.Label(resizeRectOfY(posNotationRow3, offsetPosNot3, 4), DataManager.Instance.scoreCount["MINE"]);
		
		GUI.Label(resizeRectOfY(posInfo, offsetPosInfo, 0), "First ex or less : " + DataManager.Instance.firstEx);
		GUI.Label(resizeRectOfY(posInfo, offsetPosInfo, 1), "First great or less : " + DataManager.Instance.firstGreat);
		GUI.Label(resizeRectOfY(posInfo, offsetPosInfo, 2), "First misteak : " + DataManager.Instance.firstMisteak);
		GUI.Label(resizeRectOfY(posInfo, offsetPosInfo, 3), "Average precision : " + averagePrec);
		GUI.Label(resizeRectOfY(posInfo, offsetPosInfo, 4), "Average non fantastic cause : " + (DataManager.Instance.perfect || DataManager.Instance.fullFantCombo) ? "None" : (sens > 0) ? "Too slow (" + precentSens + "%)" : (sens < 0) ? "Too fast (" + precentSens + "%)" : "Mixed fast/slow");
		GUI.Label(resizeRectOfY(posInfo, offsetPosInfo, 4), "Max Combo : ");
		
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
	
	void ProcessScore(){
	
		sens = 0;
	
		var decoupe = DataManager.Instance.scoreEarned.ToString("000.00").Trim();
		decoupeScore[0] = decoupe[0];
		decoupeScore[1] = decoupe[1];
		decoupeScore[2] = decoupe[2];
		decoupeScore[3] = decoupe[4];
		decoupeScore[4] = decoupe[5];
		
		noteToDisplay = "Note" + DataManager.Instance.giveNoteOfScore(DataManager.Instance.scoreEarned).Split(';')[1];
		
		double theav = 0;
		int signmoins;
		int signplus;
		for(int i=0; i<DataManager.Instance.precAverage.Length; i++){
			theav += Mathf.Abs(DataManager.Instance.precAverage[i]);
			if(DataManager.Instance.precAverage[i] > DataManager.Instance.ScorePrecision["FANTASTIC"]) signplus++;
			if(DataManager.Instance.precAverage[i] < -DataManager.Instance.ScorePrecision["FANTASTIC"]) signmoins++;
		}
		var l = DataManager.Instance.precAverage.Where(c => c > DataManager.Instance.ScorePrecision["FANTASTIC"]).Length;
		averagePrec = theav/(double)l;
		if(((float)signplus/(float)l)) > 0.6f){ 
			percentSens = (float)signplus/(float)l;
			sens = 1;
		}
		if(((float)signmoins/(float)l)) > 0.6f){
			percentSens = (float)signplus/(float)l;
			sens = -1;
		}
	}
	
}
