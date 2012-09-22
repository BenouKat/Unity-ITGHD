using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnMedal : MonoBehaviour {
	
	
	public float speed = 0.05f;
	
	
	//Values :
	//speed : 0.05
	//speedsphere : 0.01
	
	//Bronze
	//sphere : -1.5/0/0	
	
	//Silver
	//sphere : 1.5/1.5/0 et -1.5/1.5/0
	
	//Gold
	//sphere : 2/1.5/0	et 2/-1.5/0 et 0/2.5/0
	
	//Quad
	//sphere : -1.5/-2/0 et 0/2/0 et 3/-2/0 et -3/-2/0
	void Start(){
		
	}
	
	
	void Update(){
		
		
		
		transform.Rotate(0f, Time.deltaTime/speed, 0f);
		
	}
	
		
	
}

