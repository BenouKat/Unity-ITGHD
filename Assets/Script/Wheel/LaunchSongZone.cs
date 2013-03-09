using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LaunchSongZone : MonoBehaviour {
	
	
	public ParticleSystem Explode1;
	public ParticleSystem Explode2;
	public ParticleSystem Explode3;
	
	
	public AudioClip launchSong;
	
	//moveToSong
	private Vector2 departSongDiff;
	private Vector2 moveSongDiff;
	public Vector2 arriveSongDiff;
	public Rect posSongTitle;
	public Rect posSubTitle;
	public Rect posArtist;
	public Rect posStepArtist;
	public Rect posBestScore;
	public Rect posTopProfileScore;
	private float[] alphaSongLaunch;
	public float speedAlphaSongLaunch;
	private float alphaBlack;
	public float speedAlphaBlack;
	private float time;
	
	private bool activeModule;
	private bool startAppear;
	// Use this for initialization
	void Start () {
		
		activeModule = false;
		startAppear = false;
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
	
	void OnGUI()
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
				GUI.Label(new Rect(posBestScore.x*Screen.width, posBestScore.y*Screen.height, posBestScore.width*Screen.width, posBestScore.height*Screen.height), score == -1 ? "First try" : "Best Score : " + score.ToString("0.00") + "%" + (isScoreFail ? " (Fail)" : ""), "SongInfoLittle");
				GUI.color = new Color(1f, 1f, 1f, alphaSongLaunch[5]);	
				GUI.Label(new Rect(posTopProfileScore.x*Screen.width, posTopProfileScore.y*Screen.height, posTopProfileScore.width*Screen.width, posTopProfileScore.height*Screen.height), bestfriendscore == -1 ? "No Friends Score Entry" : "Friends Top Score : " + bestfriendscore.ToString("0.00") + "%" + " (" + bestnamefriendscore + ")" , "SongInfoLittle");
				GUI.color = new Color(1f, 1f, 1f, alphaBlack);
				GUI.DrawTexture(new Rect(0f, 0f, Screen.width+1, Screen.height+1), tex["Black"]);
				
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
				
				if(alphaBlack >= 1f && DataManager.Instance.rateSelected != 0f){
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
		gs.Line1.Stop ();
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
