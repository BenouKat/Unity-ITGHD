using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LaunchSongZoneLAN : MonoBehaviour {
	
	private GeneralScriptLAN gs;
	
	public ParticleSystem Explode1;
	public ParticleSystem Explode2;
	public ParticleSystem Explode3;
	
	
	public AudioClip launchSong;
	
	//moveToSong
	private Vector2 departSongDiff;
	private Vector2 moveSongDiff;
	public Rect posSongTitle = new Rect(0.225f, 0.35f, 0.5f, 0.1f);
	public Rect posSubTitle = new Rect(0.3f, 0.42f, 0.5f, 0.1f);
	public Rect posArtist = new Rect(0.225f, 0.475f, 0.5f, 0.1f);
	public Rect posStepArtist = new Rect(0.225f, 0.55f, 0.5f, 0.1f);
	public Rect posBestScore = new Rect(0.38f, 0.65f, 0.5f, 0.1f);
	public Rect posTopProfileScore = new Rect(0.38f, 0.72f, 0.5f, 0.1f);
	private float[] alphaSongLaunch;
	public float speedAlphaSongLaunch = 0.2f;
	private float alphaBlack;
	public float speedAlphaBlack = 1f;
	private float time;
	public Rect posLabelLoading = new Rect(0f, 0.95f, 0.2f, 0.2f);
	
	private bool displayLoading;
	
	public bool activeModule;
	// Use this for initialization
	void Start () {
		gs = GetComponent<GeneralScriptLAN>();
		activeModule = false;
		displayLoading = false;
		time = 0f;
		alphaSongLaunch = new float[6];
		for(int i=0;i<6; i++){ alphaSongLaunch[i] = 0f; }
		
		alphaBlack = 0f;
		
		if(DataManager.Instance.quickMode){
			speedAlphaSongLaunch = 0.1f;
			speedAlphaBlack = 0.1f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(activeModule)
		{
			appear();	
		}
	}
	
	public void GUIModule()
	{
		if(activeModule){

				GUI.color = new Color(1f, 1f, 1f, alphaSongLaunch[0]);	
				GUI.Label(new Rect(posSongTitle.x*Screen.width, posSongTitle.y*Screen.height, posSongTitle.width*Screen.width, posSongTitle.height*Screen.height), DataManager.Instance.songSelected.title, "SongInfoBig");
				GUI.color = new Color(1f, 1f, 1f, alphaSongLaunch[1]);	
				GUI.Label(new Rect(posSubTitle.x*Screen.width, posSubTitle.y*Screen.height, posSubTitle.width*Screen.width, posSubTitle.height*Screen.height), DataManager.Instance.songSelected.subtitle, "infosong");
				GUI.color = new Color(1f, 1f, 1f, alphaSongLaunch[2]);	
				GUI.Label(new Rect(posArtist.x*Screen.width, posArtist.y*Screen.height, posArtist.width*Screen.width, posArtist.height*Screen.height), "By " + DataManager.Instance.songSelected.artist, "songlabel");
				GUI.color = new Color(1f, 1f, 1f, alphaSongLaunch[3]);	
				GUI.Label(new Rect(posStepArtist.x*Screen.width, posStepArtist.y*Screen.height, posStepArtist.width*Screen.width, posStepArtist.height*Screen.height), "Stepchart : " + DataManager.Instance.songSelected.stepartist, "songlabel");
				GUI.color = new Color(1f, 1f, 1f, alphaSongLaunch[4]);	
				GUI.Label(new Rect(posBestScore.x*Screen.width, posBestScore.y*Screen.height, posBestScore.width*Screen.width, posBestScore.height*Screen.height), gs.getZoneInfo().getScore() == -1 ? "First try" : "Best Score : " + gs.getZoneInfo().getScore().ToString("0.00") + "%" + (gs.getZoneInfo().isFail() ? " (Fail)" : ""), "SongInfoLittle");
				GUI.color = new Color(1f, 1f, 1f, alphaSongLaunch[5]);	
				GUI.Label(new Rect(posTopProfileScore.x*Screen.width, posTopProfileScore.y*Screen.height, posTopProfileScore.width*Screen.width, posTopProfileScore.height*Screen.height), gs.getZoneInfo().getBestFriendScore() == -1 ? "No Friends Score Entry" : "Friends Top Score : " + gs.getZoneInfo().getBestFriendScore().ToString("0.00") + "%" + " (" + gs.getZoneInfo().getBestFriendName() + ")" , "SongInfoLittle");
				GUI.color = new Color(1f, 1f, 1f, alphaBlack);
				GUI.DrawTexture(new Rect(0f, 0f, Screen.width+1, Screen.height+1), gs.tex["Black"]);
				
				if(displayLoading)
				{
					GUI.color = new Color(1f, 1f, 1f, 1f);
					GUI.Label(new Rect(posLabelLoading.x*Screen.width, posLabelLoading.y*Screen.height, posLabelLoading.width*Screen.width, posLabelLoading.height*Screen.height), "Generating rated song...", "infosong");
				}
				
		}	
		
	}
	
	void appear()
	{
		if(alphaSongLaunch[5] < 1 ){
			for(int i=0;i<6;i++){
				if(alphaSongLaunch[i] < 1){
					alphaSongLaunch[i] += Time.deltaTime/speedAlphaSongLaunch;
					i = 6;
				}
			}
		}
		
		if(time > 1f){
			if(alphaBlack < 1f){
				alphaBlack += Time.deltaTime/speedAlphaBlack;	
				gs.songClip.volume -= Time.deltaTime/speedAlphaBlack;	
				
				if(alphaBlack >= 1f && gs.getZoneOption().isRatedSong()){
					displayLoading = true;
				}
			}else{
				DataManager.Instance.loadRatedSong();
				Application.LoadLevel("ChartScene");
			}
				
		}else if(alphaSongLaunch[5] >= 1){
			time += Time.deltaTime;	
		}	
	}
	
	public void activate()
	{
		gs.Fond1.gameObject.active = false;
		Explode1.Play();
		Explode2.Play();
		Explode3.Play();
		gs.songClip.Stop ();
		gs.songClip.clip = launchSong;
		gs.songClip.loop = false;
		gs.songClip.Play();
		activeModule = true;
	}
}
