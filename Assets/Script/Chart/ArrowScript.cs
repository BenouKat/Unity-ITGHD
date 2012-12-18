using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {
	public GameObject Engine;
	private Arrow associatedArrow;
	
	public ArrowPosition state;
	
	private InGameScript igs;
	public bool missed;
	public bool valid;
	public bool alreadyScored;
	private double precWayoff;
	// Use this for initialization
	void Start () {
		igs = Engine.GetComponent<InGameScript>();
		
		switch((int)transform.position.x){
			case 0:
				state = ArrowPosition.LEFT;
				break;
			case 2:
				state = ArrowPosition.DOWN;
				break;
			case 4:
				state = ArrowPosition.UP;
				break;
			case 6:
				state = ArrowPosition.RIGHT;
				break;
		}
		missed = false;
		valid = false;
		alreadyScored = false;
		precWayoff = DataManager.Instance.PrecisionValues[Precision.WAYOFF];
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!missed && !valid && associatedArrow.time <= (igs.getTotalTimeChart() - precWayoff)){
			missed = true;
			
			if(!alreadyScored){
				igs.AddMissToScoreCombo();
				igs.ComboStop(true);
				igs.displayPrec(1);
				igs.GainScoreAndLife("MISS");
				igs.stateSpeed = 0f;
			}
			
			if(associatedArrow.imJump){
				foreach(var el in associatedArrow.neighboors){
					if(el.goArrow != null){
						el.goArrow.GetComponent<ArrowScript>().alreadyScored = true;
						igs.removeArrowFromList(el, el.goArrow.GetComponent<ArrowScript>().state);	
					}
				}
			}else{
				igs.removeArrowFromList(associatedArrow, state);	
			}
			
			
		}else if(missed){
			igs.desactiveGameObjectMissed(associatedArrow.goArrow, associatedArrow.goFreeze, associatedArrow.distanceDisappear);
		}
	}
	
	public void setArrowAssociated(Arrow ar){
		associatedArrow = ar;
	}
}
