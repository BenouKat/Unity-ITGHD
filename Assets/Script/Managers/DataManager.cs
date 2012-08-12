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
	
	public float[] posYDiff;
	
	public Color[] diffColor;
	
	public float globalOffsetSeconds = -0.100f;
	
	private static DataManager instance;
	
	
	
	
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
		
		posYDiff = new float[6];
		posYDiff[0] = 0.19f;
		posYDiff[1] = -0.04f;
		posYDiff[2] = -0.27f;
		posYDiff[3] = -0.49f;
		posYDiff[4] = -0.72f;
		posYDiff[5] = -0.95f;
		
		diffColor = new Color[6];
		diffColor[0] = new Color(0.68f, 0.40f, 1f, 1f);
		diffColor[1] = new Color(0.396f, 1f, 0.415f, 1f);
		diffColor[2] = new Color(0.965f, 1f, 0.47f, 1f);
		diffColor[3] = new Color(1f, 0.208f, 0.208f, 1f);
		diffColor[4] = new Color(0.208f, 0.57f, 1f, 1f);
		diffColor[5] = new Color(1f, 1f, 1f, 1f);
		
	}
}
