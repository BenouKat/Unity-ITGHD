using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ErrorLabel : MonoBehaviour {
	
	public bool displayError;
	
	public Rect posLabelError;
	
	public void Start(){
		displayError = false;
	}
	
	public void OnGUI(){
		if(displayError){
			GUI.Label(new Rect(posLabelError.x*Screen.width, posLabelError.y*Screen.height, posLabelError.width*Screen.width, posLabelError.height*Screen.height), "The search field is numeric only");
		}
	}
	
}