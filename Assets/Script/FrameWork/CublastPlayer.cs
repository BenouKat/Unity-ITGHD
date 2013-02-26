using UnityEngine;
using System.Collections;

public class CublastPlayer{

	public string name;
	
	public string idFile;
	
	public int victoryOnline;
	
	public int difficultyMode;
	
	public double scores;
	
	public int points;
	
	//Only server will access this data
	public string packName;
	
	public bool isReady;
	
	public CublastPlayer(string name, string idFile)
	{
		this.name = name;
		this.idFile = idFile;
		this.difficultyMode = 0;
		this.isReady = false;
	}
	
	public CublastPlayer(string name, int vict, bool ready, string idFile)
	{
		this.name = name;
		this.idFile = idFile;
		this.victoryOnline = vict;
		this.difficultyMode = 0;
		this.isReady = ready;
	}
}
