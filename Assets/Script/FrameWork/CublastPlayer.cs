using UnityEngine;
using System.Collections;

public class CublastPlayer{

	public string name;
	
	public string idFile;
	
	public int victoryOnline;
	
	public int difficultyMode;
	
	//In Score tournament or Elimination
	public double scores;
	
	//In Free For All or Point Tournament
	public int points;
	
	//Only server will access this data
	public string packName;
	
	//Only server will access this data
	public bool isReady;
	
	public LANStatut statut;
	
	//0 - Data not arrived
	//1 - Ok
	//2 - Ko
	public int songChecked;
	
	//0 - Data not arrived
	//1 - Ok
	//2 - Ko
	public int vote;
	
	public CublastPlayer(string name, string idFile)
	{
		this.name = name;
		this.idFile = idFile;
		this.difficultyMode = 0;
		this.isReady = false;
		this.statut = LANStatut.ROOM;
		this.points = 0;
		this.scores = 0;
	}
	
	public CublastPlayer(string name, int vict, bool ready, string idFile)
	{
		this.name = name;
		this.idFile = idFile;
		this.victoryOnline = vict;
		this.difficultyMode = 0;
		this.isReady = ready;
		this.statut = LANStatut.ROOM;
	}
	
	public string getScore()
	{
		switch(LANManager.Instance.modeLANselected)
		{
		case LANMode.FFA:
			return points + " rounds";
		case LANMode.SCORETOURN:
			return scores + "%";
		case LANMode.POINTTOURN:
			return points + " pts";
		case LANMode.ELIMINATION:
			return scores + "%";
		default:
			return "null";
		}
	}
	
	public double getScoreNumeric()
	{
		if(LANManager.Instance.modeLANselected == LANMode.FFA || LANManager.Instance.modeLANselected == LANMode.POINTTOURN)
		{
			return System.Convert.ToDouble(points);	
		}
		return scores;
	}
	
}
