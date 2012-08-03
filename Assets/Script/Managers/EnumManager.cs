using UnityEngine;
using System.Collections;

public enum Difficulty{
	BEGINNER, 
	EASY, 
	MEDIUM, 
	HARD, 
	EXPERT,
	DBEGINNER, 
	DEASY, 
	DMEDIUM, 
	DHARD, 
	DEXPERT
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

enum PrecParticle{
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
