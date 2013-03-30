using UnityEngine;
using System.Collections;

public enum Difficulty{
	BEGINNER, 
	EASY, 
	MEDIUM, 
	HARD, 
	EXPERT,
	EDIT,
	DBEGINNER, 
	DEASY, 
	DMEDIUM, 
	DHARD, 
	DEXPERT,
	DEDIT,
	NONE
}

public enum Judge{
		BEGINNER,
		EASY,
		NORMAL,
		HARD,
		EXPERT
}

public enum Sort{
	NAME,
	STARTWITH,
	ARTIST,
	STEPARTIST,
	DIFFICULTY,
	BPM
}

public enum ScoreCount{
		FANTASTIC,
		EXCELLENT,
		GREAT,
		DECENT,
		WAYOFF,
		MISS,
		FREEZE,
		ROLL,
		JUMPS,
		HANDS,
		MINE,
		NONE
}

public enum Precision{
		FANTASTIC,
		EXCELLENT,
		GREAT,
		DECENT,
		WAYOFF,
		MISS,
		FREEZE,
		UNFREEZE,
		MINE,
		NONE
}

public enum ArrowType{
		NORMAL,
		FREEZE,
		ROLL,
		MINE
}

public enum ComboType{
		FULLFANTASTIC,
		FULLEXCELLENT,
		FULLCOMBO,
		NONE
}

public enum FadeState{
		FADEIN,
		FADEOUT,
		DISPLAY,
		NONE
}

public enum ArrowPosition{
	RIGHT,
	LEFT,
	UP,
	DOWN
}

public enum LANMode{
	FFA,
	SCORETOURN,
	POINTTOURN,
	ELIMINATION,
	NONE
}

public enum PrecParticle{
		FANTASTIC,
		FANTASTICC,
		EXCELLENT,
		EXCELLENTC,
		GREAT,
		GREATC,
		DECENT,
		WAYOFF,
		FREEZE,
		MINE
	}

public enum LANConnexionState
{
	LOADING,
	INITIALIZESCENE,
	SETPLAYERPERFS,
	IDLE,
	FAIL,
	NONE
	
}

public enum LANStatut
{
	ROOM,
	SELECTSONG,
	VOTESONG,
	OPTIONSONG,
	GAME,
	RESULT,
	NONE
}
