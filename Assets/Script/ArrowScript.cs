using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {
	public GameObject arrowLeft;
	public GameObject Engine;
	private Arrow associatedArrow;
	
	public string state;
	
	private InGameScript igs;
	public bool missed;
	public bool valid;
	public bool alreadyScored;
	// Use this for initialization
	void Start () {
		igs = Engine.GetComponent<InGameScript>();
		
		switch((int)transform.position.x){
			case 0:
				state = "left";
				break;
			case 2:
				state = "down";
				break;
			case 4:
				state = "up";
				break;
			case 6:
				state = "right";
				break;
		}
		missed = false;
		valid = false;
		alreadyScored = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!missed && !valid && associatedArrow.time <= (igs.getTotalTimeChart() - (double)0.18)){
			missed = true;
			igs.removeArrowFromList(associatedArrow, state);
			if(!alreadyScored){
				igs.displayPrec(1);
				igs.GainScoreAndLife("MISS");
			}
			
			if(associatedArrow.imJump){
				foreach(var el in associatedArrow.neighboors){
					if(el.goArrow != null) el.goArrow.GetComponent<ArrowScript>().alreadyScored = true;
				}
			}
			
			
		}
	}
	
	public void setArrowAssociated(Arrow ar){
		associatedArrow = ar;
	}
}
