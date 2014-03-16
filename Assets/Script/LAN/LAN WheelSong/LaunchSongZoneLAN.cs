using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LaunchSongZoneLAN : MonoBehaviour {
	
	
	//Prevoir quelque chose de différent
	
	
	private GeneralScriptLAN gs;
	
	public ParticleSystem Explode1;
	//public ParticleSystem Explode2;
	//public ParticleSystem Explode3;
	public Camera cameraLaunch;
	
	
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
	
	private List<string> leaderBoardDisplayed;
	public List<GameObject> goLeaderboard;
	public float speedLeaderboardMove;
	public Rect posLabelLeaderboard;
	
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
			for(int i=0; i<goLeaderboard.Count; i++)
			{
				goLeaderboard[i].transform.localPosition = Vector3.Lerp(goLeaderboard[i].transform.localPosition, new Vector3(0f, goLeaderboard[i].transform.localPosition.y, 0f), speedLeaderboardMove*Time.deltaTime);	
			}
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
			
			GUI.color = new Color(1f, 1f, 1f, 1f);
			
			for(int i=0; i<goLeaderboard.Count; i++)
			{
				var point2D = cameraLaunch.WorldToScreenPoint(goLeaderboard[i].transform.position);
				GUI.Label(new Rect(point2D.x + (posLabelLeaderboard.x*Screen.width), Screen.height - point2D.y + (posLabelLeaderboard.y*Screen.height), posLabelLeaderboard.width*Screen.width, posLabelLeaderboard.height*Screen.height), leaderBoardDisplayed[i], "SongInfoLittle");
			}
			
			
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
		
		if(time > 3f){
			if(alphaBlack < 1f){
				alphaBlack += Time.deltaTime/speedAlphaBlack;	
				gs.songClip.volume -= Time.deltaTime/speedAlphaBlack;	
				
				if(alphaBlack >= 1f && gs.getZoneOption().isRatedSong()){
					displayLoading = true;
				}
			}else{
				Network.SetSendingEnabled(0, false);
				Network.isMessageQueueRunning = false;
				LANManager.Instance.statut = LANStatut.GAME;
				Network.SetLevelPrefix(9); //Numero du level à mettre;
				LANManager.Instance.actualRound++;
				Application.LoadLevel("LANChartScene");
			}
				
		}else if(alphaSongLaunch[5] >= 1){
			time += Time.deltaTime;	
		}	
	}
	
	public void activate()
	{
		gs.Fond1.gameObject.SetActive(false);
		Explode1.Play();
		//Explode2.Play();
		//Explode3.Play();
		gs.songClip.Stop ();
		gs.songClip.clip = launchSong;
		gs.songClip.loop = false;
		gs.songClip.Play();
		activeModule = true;
	}
	
	public void setLeaderboard(string lb)
	{
		leaderBoardDisplayed = new List<string>();
		var listString = lb.Split(':');
		for(int i=0; i<3; i++)
		{
			if(listString.Length <= i || LANManager.Instance.actualRound == 0)
			{
				leaderBoardDisplayed.Add("---");	
			}else
			{
				leaderBoardDisplayed.Add(listString[i].Split(';')[0] + " (" + listString[i].Split(';')[2] + ")");
			}
		}
		
		var stringYou = "-";
		var pos = 0;
		for(int i=0; i<listString.Length; i++)
		{
			if(listString[i].Split(';')[0] == ProfileManager.Instance.currentProfile.name && listString[i].Split(';')[1] == ProfileManager.Instance.currentProfile.idFile)
			{
				pos = i + 1;
				stringYou = listString[i];
				
					
				break;
			}
		}
		if(LANManager.Instance.actualRound == 0)
		{
			pos = 0;	
		}
		switch(pos)
		{
		case 0:
			stringYou = "There's no leader yet";
			break;
		case 1:
			stringYou = "You are the leader";
			break;
		case 2:
			stringYou = "You are the vice-leader";
			break;
		case 3:
			stringYou = "You are on the podium";
			break;
		default:
			stringYou = "Your rank : " + pos + " (" + stringYou.Split(';')[2] + ")";
			break;
		}
		leaderBoardDisplayed.Add(stringYou);
	}
}
