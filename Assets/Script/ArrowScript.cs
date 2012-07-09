using UnityEngine;
using System.Collections;

public class ArrowScript : MonoBehaviour {
	public GameObject arrowLeft;
	public GameObject Engine;
	public float seuilMiss = 4f;
	
	public string state;
	
	private InGameScript igs;
	public bool missed;
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
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!missed && (transform.position.y - arrowLeft.transform.position.y) >= seuilMiss){
			missed = true;
			igs.removeArrowFromList(gameObject, state);
		}
	}
}
