using UnityEngine;
using System.Collections;



public class MineScript : MonoBehaviour {
	public GameObject arrowLeft;
	public GameObject Engine;
	private Arrow associatedArrow;
	
	public string state;
	
	private InGameScript igs;
	private KeyCode associatedKeyCode;
	public bool missed;
	public bool valid;
	public bool alreadyScored;
	
	private delegate void sMethod();
	private sMethod pm;
	
	// Use this for initialization
	void Start () {
		igs = Engine.GetComponent<InGameScript>();

		switch((int)transform.position.x){
			case 0:
				state = "left";
				associatedKeyCode = KeyCode.LeftArrow;
				this.pm = igs.StartParticleMineLeft;
				break;
			case 2:
				state = "down";
				associatedKeyCode = KeyCode.DownArrow;
				this.pm = igs.StartParticleMineDown;
				break;
			case 4:
				state = "up";
				associatedKeyCode = KeyCode.UpArrow;
				this.pm = igs.StartParticleMineUp;
				break;
			case 6:
				state = "right";
				associatedKeyCode = KeyCode.RightArrow;
				this.pm = igs.StartParticleMineRight;
				break;
		}
		missed = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		if(!missed){
			if(associatedArrow.time <= igs.getTotalTimeChart() && Input.GetKey(associatedKeyCode)){
				this.pm();
				igs.GainScoreAndLife("MINE");
				igs.removeArrowFromList(associatedArrow, state);
				DestroyImmediate(arrowLeft);
			}else  if(associatedArrow.time <= (igs.getTotalTimeChart() - (double)0.07)){
				missed = true;
				igs.removeArrowFromList(associatedArrow, state);
			}
		}
		
	}
	
	public void setArrowAssociated(Arrow ar){
		associatedArrow = ar;
	}
}
