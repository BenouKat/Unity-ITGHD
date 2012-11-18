using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class ErrorLabel : MonoBehaviour {
	
	public bool displayError;
	
	public Rect posLabelError;
	
	public GUISkin skin;
	
	public void Start(){
		displayError = false;
	}
	
	public void OnGUI(){
		GUI.skin = skin;
		if(displayError){
			GUI.color = new Color(0f, 0f, 0f, 1f);
			GUI.Label(new Rect(posLabelError.x*Screen.width + 1, posLabelError.y*Screen.height + 1, posLabelError.width*Screen.width, posLabelError.height*Screen.height), "The search field is numeric only");
			GUI.color = new Color(1f, 0.1f, 0.1f, 1f);
			GUI.Label(new Rect(posLabelError.x*Screen.width, posLabelError.y*Screen.height, posLabelError.width*Screen.width, posLabelError.height*Screen.height), "The search field is numeric only");
		}
	}
	
}