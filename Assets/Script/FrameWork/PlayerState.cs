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
	
	public PlayerState()
	{
		position = 0;
		score = 0f;
		life = 50f;
		comboType = 3;
		hasFinished = false;
		hasFailed = false;
	}
	
	public void fillPlayerState(float fscore, float flife, int fct)
	{
		score = fscore;
		life = flife;
		comboType = fct;
	}
}
