using UnityEngine;
using System.Collections;

public class ManageParticule : MonoBehaviour {


// Use this for initialization
	void Start () {
	
	}
	
	
	void FixedUpdate() {
		if(gameObject.active && !gameObject.transform.particleSystem.isPlaying){
			gameObject.active = false;
		}
	
	}

}