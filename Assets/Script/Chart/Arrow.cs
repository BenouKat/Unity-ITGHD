using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Arrow {
	
	public GameObject goArrow;
	public GameObject goFreeze;
	public ArrowPosition arrowPos;
	public ParticleSystem EndFreeze;
	
	private float distanceDisappearBase = 5f;
	public float distanceDisappear;
	
	public ArrowType arrowType;
	
	public double time;
	public double timeEndingFreeze;
	public Vector3 posEnding;
	public Vector3 posBegining;
	
	public bool alreadyValid;
	public bool imJump;
	public bool imHand;
	
	public List<Arrow> neighboors;
	
	//Pool
	private Color typeFreezeNormal = new Color(0.65f, 0.65f, 0.65f, 1f);
	private Color typeRollNormal = new Color(0.725f, 0.635f, 0.24f, 1f);
	private Color col = new Color(0f, 0f, 0f, 0f);
	private float poolValueFreeze;
	
	
	public Arrow(GameObject go, ArrowType at, double passedTime){
		alreadyValid = false;
		imJump = false;
		imHand = false;
		if(at != ArrowType.MINE){go.GetComponent<ArrowScript>().setArrowAssociated(this);}
		else{ go.GetComponent<MineScript>().setArrowAssociated(this); }
		goArrow = go;
		arrowType = at;
		time = passedTime;
		posBegining = goArrow.transform.position;
		distanceDisappear = distanceDisappearBase;
	}
	
	
	public void setArrowFreeze(double ptimeEndingFreeze, Vector3 pos, GameObject freeze, ParticleSystem particule){
		timeEndingFreeze = ptimeEndingFreeze;
		goFreeze = freeze;
		EndFreeze = particule;
		posEnding = pos;
		col = goFreeze.transform.GetChild(0).renderer.material.color;
		col.a = 0f;
		goFreeze.transform.GetChild(0).renderer.material.color = col;
		distanceDisappear = distanceDisappearBase + ((goFreeze.transform.localScale.y)*2f);
	}
	
	
	public void displayFrozenBar(){
		col = goFreeze.transform.GetChild(0).renderer.material.color;
		col.a = 0.6f;
		goFreeze.transform.GetChild(0).renderer.material.color = col;	
		if(arrowType == ArrowType.FREEZE) {
			goFreeze.renderer.material.color = typeFreezeNormal;
		}else{
			goFreeze.renderer.material.color = typeRollNormal;
		}
	}
	
	public void changeColorFreeze(float valueFreeze, float maxValue){	
		//poolValueFreeze = (Mathf.Pow(valueFreeze, 0.5f)/Mathf.Pow(maxValue, 0.5f));
		poolValueFreeze = valueFreeze/maxValue;
		if(arrowType == ArrowType.FREEZE)
		{
			col.r = typeFreezeNormal.r - poolValueFreeze;
			col.g = typeFreezeNormal.g - poolValueFreeze;
			col.b = typeFreezeNormal.b - poolValueFreeze;
			col.a = 1f;
		}else
		{
			col.r = typeRollNormal.r - poolValueFreeze;
			col.g = typeRollNormal.g - poolValueFreeze;
			col.b = typeRollNormal.b - poolValueFreeze;
			col.a = 1f;
		}
		
		goFreeze.renderer.material.color = col;
	}
	
	
}
