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
}
