using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
public class OptionManager : MonoBehaviour {

	public Camera[] cameraForOption;
	
	public void Awake(){
		
		foreach(var cam in cameraForOption){
			//Bloom and DOF option
		/*
		cam.GetComponent<Bloom>.enabled = DataManager.Instance.enableBloom;
		cam.GetComponent<DepthOfField34>.enabled = DataManager.Instance.enableDOF;*/
		}
		
		AudioListener.volume = DataManager.Instance.generalVolume/100f;
	
	}
}
