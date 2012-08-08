using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Song {

	//global information
	public string title;
	public string subtitle;
	public string artist;
	public Texture2D banner;
	public Texture2D background;
	public AudioClip music;
	public double offset;
	public double samplestart;
	public double samplelenght;
	public List<double> mesureBPMS;
	public List<double> mesureSTOPS;
	public Dictionary<double, double> bpms;
	public Dictionary<double, double> stops;
	public string stepartist;
	public Difficulty difficulty;
	public int level;
	public int numberOfSteps;
	public int numberOfStepsWithoutJumps;
	public int numberOfFreezes;
	public int numberOfRolls;
	public int numberOfMines;
	public int numberOfJumps;
	public double stepPerSecondAverage;
	public double stepPerSecondMaximum;
	public double timeMaxStep;
	public int numberOfCross;
	public int numberOfFootswitch;
	
	
	
	public AudioClip song;
	
	//stepchart
	public List<List<string>> stepchart;
	
	
	public double distanceRed;
	
	public Song(){
		bpms = new Dictionary<double, double>();
		stops = new Dictionary<double, double>();
		stepchart = new List<List<string>>();
		
	}
	
	public double getBPS(double bpmValue){
		double bps = bpmValue/(double)60.0;
		return bps;
	}
	
	public void setDifficulty(string dif){
		switch(dif){
		case "Challenge":
			difficulty = Difficulty.EXPERT;
			break;
		case "Hard":
			difficulty = Difficulty.HARD;
			break;
		case "Medium":
			difficulty = Difficulty.MEDIUM;
			break;
		case "Easy":
			difficulty = Difficulty.EASY;
			break;
		case "Beginner":
			difficulty = Difficulty.BEGINNER;
			break;
		case "DChallenge":
			difficulty = Difficulty.DEXPERT;
			break;
		case "DHard":
			difficulty = Difficulty.DHARD;
			break;
		case "DMedium":
			difficulty = Difficulty.DMEDIUM;
			break;
		case "DEasy":
			difficulty = Difficulty.DEASY;
			break;
		case "DBeginner":
			difficulty = Difficulty.DBEGINNER;
			break;
		}
	}
	
	
	
	
}