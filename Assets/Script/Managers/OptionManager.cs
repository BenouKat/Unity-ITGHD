using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
public class OptionManager : MonoBehaviour {

	public Camera[] cameraForOption;
	
	public bool disableOnAwake = false;
	
	public void Awake(){
		
		if(ProfileManager.Instance.currentProfile != null){
			
			if(!DataManager.Instance.onlyOnGame || (DataManager.Instance.onlyOnGame && Application.loadedLevelName.Contains("ChartScene"))){
				foreach(var cam in cameraForOption){
					
					if(cam.GetComponent<BloomAndLensFlares>() != null) cam.GetComponent<BloomAndLensFlares>().enabled = DataManager.Instance.enableBloom;
					if(cam.GetComponent<DepthOfField34>() != null) cam.GetComponent<DepthOfField34>().enabled = DataManager.Instance.enableDepthOfField;
				}
			}
			
			AudioListener.volume = DataManager.Instance.generalVolume;
		}
		
		if(disableOnAwake){
			gameObject.active = false;	
		}
	
	}
	
	public void reloadEffect()
	{
		foreach(var cam in cameraForOption){
			if(cam.GetComponent<BloomAndLensFlares>() != null) cam.GetComponent<BloomAndLensFlares>().enabled = DataManager.Instance.enableBloom || DataManager.Instance.onlyOnGame;
			if(cam.GetComponent<DepthOfField34>() != null) cam.GetComponent<DepthOfField34>().enabled = DataManager.Instance.enableDepthOfField || DataManager.Instance.onlyOnGame;
		}
		
		AudioListener.volume = DataManager.Instance.generalVolume;
	}
}
