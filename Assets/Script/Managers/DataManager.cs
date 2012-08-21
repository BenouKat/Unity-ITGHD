using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DataManager{

	
	
	
	
	//0 - Fantastic
	//1 - Excellent
	//2 - Great
	//3 - Decent
	//4 - WayOff
	//5 - Miss
	//6 - Freeze
	//7 - Unfreeze
	//8 - Mines
	public Dictionary<string, float> ScoreWeightValues;
	
	public Dictionary<string, float> LifeWeightValues;
	
	public Dictionary<Precision, double> PrecisionValues;
	
	public float[] posYDiff;
	
	public float[] posYZoneDiff;
	
	public Color[] diffColor;
	
	public float globalOffsetSeconds = -0.100f;
	
	public int regenComboAfterMiss = 5;
	
	private static DataManager instance;
	
	public Dictionary<Judge, string> dicScoreJudge;
	public Dictionary<Judge, string> dicHitJudge;
	public Dictionary<Judge, string> dicLifeJudge;
	public string[] aSkin;
	public string[] aRace;
	public string[] aDisplay;
	public string[] aDeath;
	
	
	public static DataManager Instance{
		get{
			if(instance == null){ 
				instance = new DataManager();
				instance.Init();
			}
			return instance;
		}
	}
	
	
	
	public void Init(){
		ScoreWeightValues = new Dictionary<string, float>();
		ScoreWeightValues.Add("FANTASTIC",1f);
		ScoreWeightValues.Add("EXCELLENT",0.8f);
		ScoreWeightValues.Add("GREAT",0.4f);
		ScoreWeightValues.Add("DECENT",0f);
		ScoreWeightValues.Add("WAYOFF",-1.2f);
		ScoreWeightValues.Add("MISS",-2.4f);
		ScoreWeightValues.Add("FREEZE",1f);
		ScoreWeightValues.Add("UNFREEZE",0f);
		ScoreWeightValues.Add("MINE",-1.2f);
		
		
		LifeWeightValues = new Dictionary<string, float>();
		LifeWeightValues.Add("FANTASTIC",0.8f);
		LifeWeightValues.Add("EXCELLENT",0.7f);
		LifeWeightValues.Add("GREAT",0.4f);
		LifeWeightValues.Add("DECENT",0f);
		LifeWeightValues.Add("WAYOFF",-5f);
		LifeWeightValues.Add("MISS",-10f);
		LifeWeightValues.Add("FREEZE",0.8f);
		LifeWeightValues.Add("UNFREEZE",-8f);
		LifeWeightValues.Add("MINE",-5f);
		
		PrecisionValues = new Dictionary<Precision, double>();
		PrecisionValues.Add(Precision.FANTASTIC, 0.0215);
		PrecisionValues.Add(Precision.EXCELLENT, 0.043);
		PrecisionValues.Add(Precision.GREAT, 0.102);
		PrecisionValues.Add(Precision.DECENT, 0.135);
		PrecisionValues.Add(Precision.WAYOFF, 0.180);
		
		posYDiff = new float[6];
		posYDiff[0] = 0.19f;
		posYDiff[1] = -0.04f;
		posYDiff[2] = -0.27f;
		posYDiff[3] = -0.49f;
		posYDiff[4] = -0.72f;
		posYDiff[5] = -0.95f;
		
		posYZoneDiff = new float[6];
		posYZoneDiff[0] = 2.2f;
		posYZoneDiff[1] = -0.7f;
		posYZoneDiff[2] = -3.7f;
		posYZoneDiff[3] = -6.7f;
		posYZoneDiff[4] = -9.6f;
		posYZoneDiff[5] = -12.5f;
		
		diffColor = new Color[6];
		diffColor[0] = new Color(0.68f, 0.40f, 1f, 1f);
		diffColor[1] = new Color(0.396f, 1f, 0.415f, 1f);
		diffColor[2] = new Color(0.965f, 1f, 0.47f, 1f);
		diffColor[3] = new Color(1f, 0.208f, 0.208f, 1f);
		diffColor[4] = new Color(0.208f, 0.57f, 1f, 1f);
		diffColor[5] = new Color(1f, 1f, 1f, 1f);
		
		InitDicOption();
		
	}
	
	public void InitDicOption(){
		dicScoreJudge = new Dictionary<Judge, string>();
		dicHitJudge = new Dictionary<Judge, string>();
		dicLifeJudge = new Dictionary<Judge, string>();
		aSkin = new string[4];
		aRace = new string[11];
		aDisplay = new string[9];
		aDeath = new string[3];
		
		dicScoreJudge.Add(Judge.BEGINNER, "Children");
		dicScoreJudge.Add(Judge.EASY, "Sympathic");
		dicScoreJudge.Add(Judge.NORMAL, "Normal");
		dicScoreJudge.Add(Judge.HARD, "Horrible");
		dicScoreJudge.Add(Judge.EXPERT, "Hellish");
		
		dicHitJudge.Add(Judge.BEGINNER, "Blind");
		dicHitJudge.Add(Judge.EASY, "Soft");
		dicHitJudge.Add(Judge.NORMAL, "Normal");
		dicHitJudge.Add(Judge.HARD, "Sniper");
		dicHitJudge.Add(Judge.EXPERT, "Asian");
		
		dicLifeJudge.Add(Judge.BEGINNER, "Candy");
		dicLifeJudge.Add(Judge.EASY, "Nice");
		dicLifeJudge.Add(Judge.NORMAL, "Normal");
		dicLifeJudge.Add(Judge.HARD, "Painful");
		dicLifeJudge.Add(Judge.EXPERT, "Mortal");
		
		aSkin[0] = "Cublast";
		aSkin[1] = "Dacepad";
		aSkin[2] = "Bubble";
		aSkin[3] = "Strange";
		
		aRace[0] = "None";
		aRace[1] = "C Race";
		aRace[2] = "B Race";
		aRace[3] = "A Race";
		aRace[4] = "S Race";
		aRace[5] = "Bronze race";
		aRace[6] = "Silver race";
		aRace[7] = "Gold race";
		aRace[8] = "Quad race";
		aRace[9] = "FC Race";
		aRace[10] = "FEC Race";
		
		aDisplay[0] = "No mine";
		aDisplay[1] = "No jump";
		aDisplay[2] = "No hands";
		aDisplay[3] = "No judge";
		aDisplay[4] = "No target";
		aDisplay[5] = "No background";
		aDisplay[6] = "No lifebar";
		aDisplay[7] = "No score";
		aDisplay[8] = "No song bar";
		
		aDeath[0] = "Immediatly";
		aDeath[1] = "After 30 misses";
		aDeath[2] = "Never";
	}
	
	public void LoadScoreJudge(Judge j){
		switch(j){
			case Judge.BEGINNER:
				ScoreWeightValues["EXCELLENT"] = 1f;
				ScoreWeightValues["GREAT"] = 0.9f;
				ScoreWeightValues["DECENT"] = 0.5f;
				ScoreWeightValues["WAYOFF"] = 0f;
				ScoreWeightValues["MISS"] = 0f;
				ScoreWeightValues["MINE"] = -0.01f;
			break;
			case Judge.EASY:
				ScoreWeightValues["EXCELLENT"] = 0.9f;
				ScoreWeightValues["GREAT"] = 0.6f;
				ScoreWeightValues["DECENT"] = 0f;
				ScoreWeightValues["WAYOFF"] = 0f;
				ScoreWeightValues["MISS"] = -1f;
				ScoreWeightValues["MINE"] = -0.5f;
			break;
			case Judge.NORMAL:
				ScoreWeightValues["EXCELLENT"] = 0.8f;
				ScoreWeightValues["GREAT"] = 0.4f;
				ScoreWeightValues["DECENT"] = 0f;
				ScoreWeightValues["WAYOFF"] = -1.2f;
				ScoreWeightValues["MISS"] = -2.4f;
				ScoreWeightValues["MINE"] = -1.2f;
			break;
			case Judge.HARD:
				ScoreWeightValues["EXCELLENT"] = 0.5f;
				ScoreWeightValues["GREAT"] = 0.2f;
				ScoreWeightValues["DECENT"] = -0.5f;
				ScoreWeightValues["WAYOFF"] = -2f;
				ScoreWeightValues["MISS"] = -4f;
				ScoreWeightValues["MINE"] = -2f;
			break;
			case Judge.EXPERT:
				ScoreWeightValues["EXCELLENT"] = 0.3f;
				ScoreWeightValues["GREAT"] = 0f;
				ScoreWeightValues["DECENT"] = -2f;
				ScoreWeightValues["WAYOFF"] = -5f;
				ScoreWeightValues["MISS"] = -10f;
				ScoreWeightValues["MINE"] = -10f;
			break;
		}
	}
	
	
	public void LoadHitJudge(Judge j){
		switch(j){
			case Judge.BEGINNER:
				PrecisionValues[Precision.FANTASTIC] = 0.135;
				PrecisionValues[Precision.EXCELLENT] = 0.180;
				PrecisionValues[Precision.GREAT] = 0.280;
				PrecisionValues[Precision.DECENT] = 0.350;
				PrecisionValues[Precision.WAYOFF] = 0.5;
			break;
			case Judge.EASY:
				PrecisionValues[Precision.FANTASTIC] = 0.043;
				PrecisionValues[Precision.EXCELLENT] = 0.102;
				PrecisionValues[Precision.GREAT] = 0.180;
				PrecisionValues[Precision.DECENT] = 0.240;
				PrecisionValues[Precision.WAYOFF] = 0.3;
			break;
			case Judge.NORMAL:	
				PrecisionValues[Precision.FANTASTIC] = 0.0215;
				PrecisionValues[Precision.EXCELLENT] = 0.043;
				PrecisionValues[Precision.GREAT] = 0.102;
				PrecisionValues[Precision.DECENT] = 0.135;
				PrecisionValues[Precision.WAYOFF] = 0.180;
			break;
			case Judge.HARD:	
				PrecisionValues[Precision.FANTASTIC] = 0.015;
				PrecisionValues[Precision.EXCELLENT] = 0.035;
				PrecisionValues[Precision.GREAT] = 0.070;
				PrecisionValues[Precision.DECENT] = 0.1;
				PrecisionValues[Precision.WAYOFF] = 0.135;
			break;
			case Judge.EXPERT:	
				PrecisionValues[Precision.FANTASTIC] = 0.0075;
				PrecisionValues[Precision.EXCELLENT] = 0.015;
				PrecisionValues[Precision.GREAT] = 0.0215;
				PrecisionValues[Precision.DECENT] = 0.043;
				PrecisionValues[Precision.WAYOFF] = 0.102;
			break;
		}
		
	}
	
	
	public void LoadLifeJudge(Judge j){
		switch(j){
			case Judge.BEGINNER:
				LifeWeightValues["FANTASTIC"] = 5f;
				LifeWeightValues["EXCELLENT"] = 2f;
				LifeWeightValues["GREAT"] = 1f;
				LifeWeightValues["DECENT"] = 0.2f;
				LifeWeightValues["WAYOFF"] = -0.2f;
				LifeWeightValues["MISS"] = -1f;
				LifeWeightValues["MINE"] = -0.1f;
				LifeWeightValues["FREEZE"] = 5f;
				LifeWeightValues["UNFREEZE"] = -0.2f;
				regenComboAfterMiss = 0;
			break;
			case Judge.EASY:
				LifeWeightValues["FANTASTIC"] = 1.5f;
				LifeWeightValues["EXCELLENT"] = 1f;
				LifeWeightValues["GREAT"] = 0.5f;
				LifeWeightValues["DECENT"] = 0f;
				LifeWeightValues["WAYOFF"] = -2.5f;
				LifeWeightValues["MISS"] = -5f;
				LifeWeightValues["MINE"] = -2.5f;
				LifeWeightValues["FREEZE"] = 1.5f;
				LifeWeightValues["UNFREEZE"] = -3.5f;
				regenComboAfterMiss = 2;
			break;
			case Judge.NORMAL:	
				LifeWeightValues["FANTASTIC"] = 0.8f;
				LifeWeightValues["EXCELLENT"] = 0.7f;
				LifeWeightValues["GREAT"] = 0.4f;
				LifeWeightValues["DECENT"] = 0f;
				LifeWeightValues["WAYOFF"] = -5f;
				LifeWeightValues["MISS"] = -10f;
				LifeWeightValues["MINE"] = -5f;
				LifeWeightValues["FREEZE"] = 0.8f;
				LifeWeightValues["UNFREEZE"] = -8f;
				regenComboAfterMiss = 5;
			break;
			case Judge.HARD:	
				LifeWeightValues["FANTASTIC"] = 0.5f;
				LifeWeightValues["EXCELLENT"] = 0.4f;
				LifeWeightValues["GREAT"] = 0.05f;
				LifeWeightValues["DECENT"] = -5f;
				LifeWeightValues["WAYOFF"] = -20f;
				LifeWeightValues["MISS"] = -25f;
				LifeWeightValues["MINE"] = -10f;
				LifeWeightValues["FREEZE"] = 0.5f;
				LifeWeightValues["UNFREEZE"] = -20f;
				regenComboAfterMiss = 10;
			break;
			case Judge.EXPERT:	
				LifeWeightValues["FANTASTIC"] = 0.2f;
				LifeWeightValues["EXCELLENT"] = 0.1f;
				LifeWeightValues["GREAT"] = 0f;
				LifeWeightValues["DECENT"] = -20f;
				LifeWeightValues["WAYOFF"] = -50f;
				LifeWeightValues["MISS"] = -100f;
				LifeWeightValues["MINE"] = -25f;
				LifeWeightValues["FREEZE"] = 0f;
				LifeWeightValues["UNFREEZE"] = -25f;
				regenComboAfterMiss = 20;
			break;
		}
		
		
	}
}
