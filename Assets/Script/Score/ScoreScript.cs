using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreScript : MonoBehaviour {
	
	
	public Rect posScoringTitle;
	
	public Rect posNote;
	
	public Rect posScore;
	
	public float offsetScore;
	
	public Dictionary<string, Texture2D> dicTex;
	
	private int[] decoupeScore;
	private string noteToDisplay;
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
		decoupeScore = new int[5];
		for(int i=0;i<decoupeScore.Count();i++){
			decoupeScore[i] = 0;	
		}
		DataManager.Instance.scoreEarned = 90f;
		ProcessScore();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.DrawTexture(resizeRect(posScoringTitle), dicTex["ScoreTitle"]);
		
		//Test
		GUI.DrawTexture(resizeRect(posNote), dicTex[noteToDisplay]);
		
		for(int i=0;i<5;i++){
			if(i > 1 || (i == 0 && (float)(decoupeScore[0]) != 0f) || (i == 1 && (float)(decoupeScore[1]) != 0f)) GUI.DrawTexture(new Rect((posScore.x + offsetScore)*Screen.width, posScore.y*Screen.height, posScore.width*Screen.width, posScore.height*Screen.height), dicTex["S" + decoupeScore[i]]);
		}
	}
			
	Rect resizeRect(Rect r){
		return new Rect(r.x*Screen.width, r.y*Screen.height, r.width*Screen.width, r.height*Screen.height);
	}
	
	void ProcessScore(){
		var decoupe = DataManager.Instance.scoreEarned.ToString("000.00").Trim();
		decoupeScore[0] = decoupe[0];
		decoupeScore[1] = decoupe[1];
		decoupeScore[2] = decoupe[2];
		decoupeScore[3] = decoupe[4];
		decoupeScore[4] = decoupe[5];
		
		noteToDisplay = "Note" + DataManager.Instance.giveNoteOfScore(DataManager.Instance.scoreEarned).Split(';')[1];
	}
	
}
