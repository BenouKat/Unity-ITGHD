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
		ScoreWeightValues.Add("MINES",-1.2f);
		
		
		LifeWeightValues = new Dictionary<string, float>();
		LifeWeightValues.Add("FANTASTIC",0.8f);
		LifeWeightValues.Add("EXCELLENT",0.7f);
		LifeWeightValues.Add("GREAT",0.4f);
		LifeWeightValues.Add("DECENT",0f);
		LifeWeightValues.Add("WAYOFF",-5f);
		LifeWeightValues.Add("MISS",-10f);
		LifeWeightValues.Add("FREEZE",0.8f);
		LifeWeightValues.Add("UNFREEZE",-8f);
		LifeWeightValues.Add("MINES",-5f);
		
	}
}
