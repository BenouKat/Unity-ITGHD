using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Arrow {
	
	public GameObject goArrow;
	public GameObject goFreeze;
	public ParticleSystem EndFreeze;
	
	public ArrowType arrowType;
	
	public double time;
	public double timeEndingFreeze;
	public Vector3 posEnding;
	public Vector3 posBegining;
	
	public bool alreadyValid;
	public bool imJump;
	
	public List<Arrow> neighboors;
	
	
	public Arrow(GameObject go, ArrowType at, double passedTime){
		alreadyValid = false;
		imJump = false;
		if(at != ArrowType.MINE){go.GetComponent<ArrowScript>().setArrowAssociated(this);}
		else{ go.GetComponent<MineScript>().setArrowAssociated(this); }
		goArrow = go;
		arrowType = at;
		time = passedTime;
		posBegining = goArrow.transform.position;
	}
	
	
	public void setArrowFreeze(double ptimeEndingFreeze, Vector3 pos, GameObject freeze, ParticleSystem particule){
		timeEndingFreeze = ptimeEndingFreeze;
		goFreeze = freeze;
		EndFreeze = particule;
		posEnding = pos;
		var col = goFreeze.transform.GetChild(0).renderer.material.color;
		if(arrowType == ArrowType.FREEZE) {
			goFreeze.renderer.material.color = new Color(0.4f, 0.4f, 0.4f, 1f);
		}else{
			goFreeze.renderer.material.color = new Color(0.4f, 0.4f, 0f, 1f);
		}
		goFreeze.transform.GetChild(0).renderer.material.color = new Color(col.r, col.g, col.b, 0f);
	}
	
	
	public void displayFrozenBar(){
		var col = goFreeze.transform.GetChild(0).renderer.material.color;
		goFreeze.transform.GetChild(0).renderer.material.color = new Color(col.r, col.g, col.b, 0.6f);	
		if(arrowType == ArrowType.FREEZE) {
			goFreeze.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		}else{
			goFreeze.renderer.material.color = new Color(1f, 1f, 0f, 1f);
		}
	}
	
	public void changeColorFreeze(float valueFreeze, float maxValue){	
		var val = (Mathf.Pow(valueFreeze, 0.5f)/Mathf.Pow(maxValue, 0.5f));
		if(arrowType == ArrowType.FREEZE) {
			goFreeze.renderer.material.color = new Color(1f - val*1.5f, 1f - val*1.5f, 1f - val*1.5f, 1f);
		}else{
			goFreeze.renderer.material.color = new Color(1f - val*1.5f, 1f - val*1.5f, 0f, 1f);
		}
		
	}
	
	
}
