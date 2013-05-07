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
		comboType = 0;
		hasFinished = false;
		hasFailed = false;
	}
}
