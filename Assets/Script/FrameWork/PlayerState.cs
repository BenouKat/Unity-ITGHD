using UnityEngine;
using System.Collections;

public class PlayerState {

	public int position;
	public string name;
	public float score;
	public float life;
	public int comboType;
	public bool hasFinished;
	public bool hasFailed;
	public string statistics;
	
	public int fcount;
	public int excount;
	public int gcount;
	public int dcount;
	public int wcount;
	public int mcount;
	public float firstMistake;
	public int average;
	public int maxCombo;
	
	public PlayerState()
	{
		position = 0;
		score = 0f;
		life = 50f;
		comboType = 3;
		hasFinished = false;
		hasFailed = false;
	}
	
	public PlayerState(string thename, float thescore, bool thehasFailed, int thect, int tfcount, int texcount, int tgcount, int tdcount, int twcount, int tmcount, float tfirstMistake, int taverage, int tmaxCombo)
	{
		name = thename;
		score = thescore;
		comboType = thect;
		hasFailed = thehasFailed;
		fcount = tfcount;
		excount = texcount;
		gcount = tgcount;
		dcount = tdcount;
		wcount = twcount;
		mcount = tmcount;
		firstMistake = tfirstMistake;
		average = taverage;
		maxCombo = tmaxCombo;
	}
	
	public void fillPlayerState(float fscore, float flife, int fct)
	{
		score = fscore;
		life = flife;
		comboType = fct;
	}
}
