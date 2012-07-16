using UnityEngine;
using System.Collections;

public class Arrow {
	
	public GameObject goArrow;
	public GameObject goFreeze;
	public ParticleSystem EndFreeze;
	
	public ArrowType arrowType;
	
	public double time;
	public double timeEndingFreeze;
	public Vector3 posEnding;
	public Vector3 posBegining;
	
	
	public enum ArrowType{
		NORMAL,
		FREEZE,
		ROLL,
		MINE
	}
	
	public Arrow(GameObject go, ArrowType at, double passedTime){
		
		go.GetComponent<ArrowScript>().setArrowAssociated(this);
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
		goFreeze.transform.GetChild(0).renderer.material.color = new Color(col.r, col.g, col.b, 0f);
	}
	
	
	public void displayFrozenBar(){
		var col = goFreeze.transform.GetChild(0).renderer.material.color;
		goFreeze.transform.GetChild(0).renderer.material.color = new Color(col.r, col.g, col.b, 0.4f);	
		goFreeze.renderer.material.color = new Color(1f, 1f, 1f, 1f);
	}
	
	public void changeColorFreeze(float valueFreeze, float maxValue){	
		goFreeze.renderer.material.color = new Color(1f - (valueFreeze/maxValue)*1.5f, 1f - (valueFreeze/maxValue)*1.5f, 1f - (valueFreeze/maxValue)*1.5f, 1f);
	}
	
	
}
