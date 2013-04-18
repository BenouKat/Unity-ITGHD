using UnityEngine;
using System.Collections;
using System;
public class FadeManager : MonoBehaviour {
	
	
	private Texture2D fade;
	
	public float posxfinal;
	public float speedFade;
	private float posFadex;
	
	private FadeState fs;
	
	public bool startFeded;
	
	public bool disableWhenFinish;
	
	public int depth = -20;
	
	private string levelToLoad;
	// Use this for initialization
	void Start () {
		fade = (Texture2D) Resources.Load("Fade");
		fs = FadeState.NONE;
		if(startFeded){
			fs = FadeState.DISPLAY;
			posFadex = posxfinal;
		}
		
		if(DataManager.Instance.quickMode) speedFade = 0.01f;
		
	}
	
	
	void OnGUI(){
		GUI.depth = depth;
		if(fs != FadeState.NONE){
			switch(fs){
			case FadeState.FADEIN:
				GUI.DrawTexture(new Rect(posFadex*Screen.width, 0f, Screen.width*2f, Screen.height), fade);
				posFadex -= Time.deltaTime/speedFade;
				if(posFadex <= posxfinal){
					fs = FadeState.DISPLAY;	
					if(!String.IsNullOrEmpty(levelToLoad)) Application.LoadLevel(DataManager.Instance.giveLevelToLoad(levelToLoad));
				}
				break;
			case FadeState.FADEOUT:
				GUI.DrawTexture(new Rect(posFadex*Screen.width, 0f, Screen.width*2f, Screen.height), fade);
				posFadex += Time.deltaTime/speedFade;
				if(posFadex >= 1f){
					fs = FadeState.NONE;
					if(disableWhenFinish)
					{
						this.enabled = false;	
					}
				}
				break;
			case FadeState.DISPLAY:
				GUI.DrawTexture(new Rect(posFadex*Screen.width, 0f, Screen.width*2f, Screen.height), fade);
				break;
			}
		}
	}
	
	public void FadeIn(){
		posFadex = 1f;
		levelToLoad = "";
		fs = FadeState.FADEIN;
	}
	
	public void FadeOut(){
		posFadex = posxfinal;
		fs = FadeState.FADEOUT;
	}
	
	public void FadeIn(string levelName){
		posFadex = 1f;
		levelToLoad = levelName;
		fs = FadeState.FADEIN;
		
	}
	
	public void setStartFadeOn(){
		startFeded = true;
		fs = FadeState.DISPLAY;
		posFadex = posxfinal;
	}
}
