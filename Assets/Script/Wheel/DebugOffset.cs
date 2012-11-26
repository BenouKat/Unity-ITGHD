using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class DebugOffset : MonoBehaviour {
	
	private bool display;
	
	public Rect posLabel;
	
	private string valueTextField;
	
	public bool validValue;
	
	public GUISkin skin;
	
	void Start(){
		display = false;
		valueTextField = DataManager.Instance.userGOS.ToString("0.000");
		validValue = true;
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.F1)){
			display = !display;
		}
	}
	
	void OnGUI(){
		GUI.skin = skin;
		GUI.depth = 0;
		if(display){
			valueTextField = GUI.TextField(new Rect(posLabel.x*Screen.width, posLabel.y*Screen.height, posLabel.width*Screen.width, posLabel.height*Screen.height), valueTextField);
			double valued = 0;
			if(Double.TryParse(valueTextField, out valued)){
				DataManager.Instance.userGOS = (float)valued;
				validValue = true;
			}else{
				validValue = false;
			}
		}
	}
	
}