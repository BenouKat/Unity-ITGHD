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
	
	public void Start(){
		display = false;
		valueTextField = DataManager.Instance.userGOS.ToString("0.000");
		validValue = true;
	}
	
	public void Update(){
		if(Input.GetButtonDown("F1")){
			display = !display;
		}
	}
	
	public void OnGUI(){
		if(display){
			valueTextField = GUI.TextField(new Rect(posLabel.x*Screen.width, posLabel.y*Screen.height, posLabel.width*Screen.width, posLabel.height*Screen.height), valueTextField);
			double valued = 0;
			if(Double.TryParse(valueTextField, out valued)){
				DataManager.Instance.userGOS = (float)valued;
			}else{
				validValue = false;
			}
		}
	}
	
}