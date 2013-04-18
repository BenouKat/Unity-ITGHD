using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GeneralScriptLAN : MonoBehaviour {
	
	
	#region variable	
	//GameObject library
	
	public GameObject plane;
	public GUISkin skin;
	
	public AudioSource songClip;
	public AudioSource mainThemeClip;
	public ParticleSystem Fond1;
	
	private PackZoneLAN packZone;
	private SongZoneLAN songZone;
	private InfoZoneLAN infoZone;
	private OptionZoneLAN optionZone;
	private LaunchSongZoneLAN launchSongZone;
	
	//List And dictionary
	
	public Dictionary<Difficulty, Song> songSelected;
	
	public Dictionary<string, Texture2D> tex;
	
	
	//General feature
	public string speedmodstring;
	public string bpmstring;
	public float speedmodSelected;
	
	//Banner
	private Texture2D actualBanner;
	public float speedFadeAlpha = 1f;
	private float alphaBanner;
	private bool FadeOutBanner;
	private bool FadeInBanner;
	public Vector3 posBannerOption = new Vector3(0f, 12f, 20f);
	public Vector3 recoverPosBanner;
	public Vector3 posBannerSong = new Vector3(0f, 10f, 20f);
	public Vector3 scaleBannerSong = new Vector3(3f, 1f, 0.8f);
	
	//General GUI
	private float timeFade;
	
	
	public Rect Jouer = new Rect(0.75f, 0.92f, 0.12f, 0.05f);
	
	
	//Declancheurs
	
	private bool fadedOut;
	
	
	//Move to option mode
	public float speedMoveOption = 0.2f;
	public float limiteMoveOption = 0.05f;
	private float offsetXDiff;
	private float offsetYDiff;
	private Vector2 departOptionDiff;
	private Vector2 moveOptionDiff;
	
	
	//General Update
	public float timeBeforeDisplay = 0.4f;
	private float time;
	private bool alreadyRefresh;
	private bool alreadyFade;
	
	//Vote
	public bool inVoteMode;
	public bool releaseHiddenVote;
	public Rect labelVote;
	public Rect buttonYes;
	public Rect buttonNo;
	private bool alreadyVote;
	
	//Sound
	AudioClip actualClip;
	public float speedAudioVolume = 1.5f;
	
	private NetworkWheelScript nws;
	public bool pickSetted;
	public bool launchProposition;
	private bool isReadyWithOptions;
	private bool quitAsked;
	public float speedAlphaQuitAsked; 
	private float alphaQuitAsked;
	#endregion
	// Use this for initialization
	
	
	//test 
	
	void Awake()
	{
		DataManager.Instance.removeRatedSong();
	}
	
	void Start () {
		
		launchProposition = false;
		pickSetted = false;
		inVoteMode = false;
		releaseHiddenVote = false;
		alreadyVote = false;
		isReadyWithOptions = false;
		quitAsked = false;
		alphaQuitAsked = 0f;
		nws = GetComponent<NetworkWheelScript>();
		packZone = GetComponent<PackZoneLAN>();
		songZone = GetComponent<SongZoneLAN>();
		infoZone = GetComponent<InfoZoneLAN>();
		optionZone = GetComponent<OptionZoneLAN>();
		launchSongZone = GetComponent<LaunchSongZoneLAN>();
		timeFade = 0f;
		
		
		tex = new Dictionary<string, Texture2D>();
		tex.Add("BEGINNER", (Texture2D) Resources.Load("beginner"));
		tex.Add("EASY", (Texture2D) Resources.Load("easy"));
		tex.Add("MEDIUM", (Texture2D) Resources.Load("medium"));
		tex.Add("HARD", (Texture2D) Resources.Load("hard"));
		tex.Add("EXPERT", (Texture2D) Resources.Load("expert"));
		tex.Add("EDIT", (Texture2D) Resources.Load("edit"));
		tex.Add("graph", (Texture2D) Resources.Load("graph"));
		tex.Add("bouton", (Texture2D) Resources.Load("Button"));
		tex.Add("Option1", (Texture2D) Resources.Load("Speedmod"));
		tex.Add("Option2", (Texture2D) Resources.Load("Difficulty"));
		tex.Add("Option3", (Texture2D) Resources.Load("Skin"));
		tex.Add("Option4", (Texture2D) Resources.Load("HitJudge"));
		tex.Add("Option5", (Texture2D) Resources.Load("ScoreJudge"));
		tex.Add("Option6", (Texture2D) Resources.Load("LifeJudge"));
		tex.Add("Option7", (Texture2D) Resources.Load("Display"));
		tex.Add("Black", (Texture2D) Resources.Load("black"));
		tex.Add("Cache", (Texture2D) Resources.Load("CacheNameWheel"));
		tex.Add("ChoiceBar", (Texture2D) Resources.Load("ChoiceDisplay"));
		tex.Add("Failed", (Texture2D) Resources.Load("Fail"));
		tex.Add("BAD", (Texture2D) Resources.Load("NoteBAD"));
		tex.Add("C", (Texture2D) Resources.Load("NoteC"));
		tex.Add("B", (Texture2D) Resources.Load("NoteB"));
		tex.Add("A", (Texture2D) Resources.Load("NoteA"));
		tex.Add("S", (Texture2D) Resources.Load("NoteS"));
		
		
		actualBanner = new Texture2D(512,256);
		
		recoverPosBanner = plane.transform.position;
		
		if(!String.IsNullOrEmpty(ProfileManager.Instance.currentProfile.lastSpeedmodUsed)){
			speedmodstring = ProfileManager.Instance.currentProfile.lastSpeedmodUsed;
			speedmodSelected = (float)System.Convert.ToDouble(ProfileManager.Instance.currentProfile.lastSpeedmodUsed);
		}else{
			speedmodSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.speedmodSelected : 2f;
			speedmodstring = speedmodSelected.ToString("0.00");
		}
		
		
		if(DataManager.Instance.songSelected != null){
			var bpmtotest = DataManager.Instance.songSelected.bpmToDisplay;
			if(bpmtotest.Contains("->")){
				bpmstring = (System.Convert.ToDouble(System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode])*speedmodSelected)).ToString("0");
			}else{
				bpmstring = (System.Convert.ToDouble(bpmtotest)*speedmodSelected).ToString("0");
			}
		}else{
			bpmstring = String.IsNullOrEmpty(ProfileManager.Instance.currentProfile.lastBPM) ? "300" : ProfileManager.Instance.currentProfile.lastBPM;
		}
		
		
		actualClip = new AudioClip();
		
		time = 0f;
		alreadyRefresh = true;
		alphaBanner = 1f;
		FadeOutBanner = false;
		FadeInBanner = false;
		fadedOut = false;
		alreadyFade = false;

		refreshPackBanner();
		
		GetComponent<ChatScript>().activeChat(true);
		
		if(DataManager.Instance.quickMode){
			speedMoveOption = 0.01f;
		}
		
		LANManager.Instance.statut = LANStatut.SELECTSONG;
		if(!LANManager.Instance.isCreator)
		{
			networkView.RPC("sendStatut", RPCMode.Server, Network.player, (int)LANStatut.SELECTSONG);
		}else
		{
			LANManager.Instance.players.ElementAt(0).Value.statut = LANStatut.SELECTSONG;	
		}
	}
	
	// Update is called once per frame
	public void OnGUI () {
		
		

		GUI.skin = skin;
		
	
		packZone.GUIModule();
		GUI.color = new Color(1f, 1f, 1f, 1f);
		songZone.GUIModule();
		GUI.color = new Color(1f, 1f, 1f, 1f);
		infoZone.GUIModule();
		GUI.color = new Color(1f, 1f, 1f, 1f);
		optionZone.GUIModule();
		GUI.color = new Color(1f, 1f, 1f, 1f);
		launchSongZone.GUIModule();
		GUI.color = new Color(1f, 1f, 1f, 1f);
		#region optionPlayGUI
		//Option/jouer
		if(songSelected != null && pickSetted && !launchSongZone.activeModule && !nws.isSearching && !launchProposition && !inVoteMode && !getZoneOption().activeModule){

			if(GUI.Button(new Rect(Jouer.x*Screen.width, Jouer.y*Screen.height, Jouer.width*Screen.width, Jouer.height*Screen.height), LANManager.Instance.isPicker ? "Select" : "Suggest", "labelGo")){
				launchProposition = true;
				var theSelected = songSelected[getZoneInfo().getActualySelected()];
				if(LANManager.Instance.isCreator)
				{
					nws.callSong(Network.player, theSelected.title, theSelected.subtitle, theSelected.numberOfSteps, (int)theSelected.difficulty, theSelected.level);
				}else
				{
					networkView.RPC ("callSong", RPCMode.Server, Network.player, theSelected.title, theSelected.subtitle, theSelected.numberOfSteps, (int)theSelected.difficulty, theSelected.level);	
				}
			}
		
		}
		
		if(getZoneOption().activeModule && !isReadyWithOptions)
		{
			if(GUI.Button(new Rect(Jouer.x*Screen.width, Jouer.y*Screen.height, Jouer.width*Screen.width, Jouer.height*Screen.height), "Ready", "labelGo")){
				isReadyWithOptions = true;
				if(LANManager.Instance.isCreator)
				{
					nws.getPlayerReady(Network.player);
				}else
				{
					networkView.RPC("getPlayerReady", RPCMode.Server, Network.player);
				}
				
			}
			
		}
		
		if(inVoteMode && releaseHiddenVote)
		{
			GUI.Label(new Rect(labelVote.x*Screen.width, labelVote.y*Screen.height, labelVote.width*Screen.width, labelVote.height*Screen.height), alreadyVote ? TextManager.Instance.texts["LAN"]["VOTEOK"] : TextManager.Instance.texts["LAN"]["VOTEAsk"], "centeredBigLabel");
		
			if(!alreadyVote)
			{
				if(GUI.Button(new Rect(buttonYes.x*Screen.width, buttonYes.y*Screen.height, buttonYes.width*Screen.width, buttonYes.height*Screen.height), "Accept", "labelGo"))
				{
					if(LANManager.Instance.isCreator)
					{
						nws.getResultVote(Network.player, 1);
					}else
					{
						networkView.RPC("getResultVote", RPCMode.Server, Network.player, 1);
					}
					GetComponent<ChatScript>().sendDirectMessage(ProfileManager.Instance.currentProfile.name, TextManager.Instance.texts["LAN"]["VOTEValid"]);
					alreadyVote = true;
				}
				
				if(GUI.Button(new Rect(buttonNo.x*Screen.width, buttonNo.y*Screen.height, buttonNo.width*Screen.width, buttonNo.height*Screen.height), "Deny", "labelGo"))
				{
					if(LANManager.Instance.isCreator)
					{
						nws.getResultVote(Network.player, 2);
					}else
					{
						networkView.RPC("getResultVote", RPCMode.Server, Network.player, 2);
					}
					GetComponent<ChatScript>().sendDirectMessage(ProfileManager.Instance.currentProfile.name, TextManager.Instance.texts["LAN"]["VOTEFail"]);
					alreadyVote = true;
				}
			}
		
		}
		
		if(quitAsked)
		{
			GUI.Color = new Color(1f, 1f, 1f, alphaQuitAsked);
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), tex["Black"]);
			
			GUI.Color = new Color(1f, 0.1f, 0.1f, 1f);
			GUI.Label = GUI.Label(new Rect(labelVote.x*Screen.width, (labelVote.y - 0.4f)*Screen.height , labelVote.width*Screen.width, labelVote.height*Screen.height), TextManager.Instance.texts["LAN"]["QUIT"], "centeredBigLabel");
			
			GUI.Color = new Color(1f, 1f, 1f, 1f);
			if(GUI.Button(new Rect(buttonYes.x*Screen.width, (buttonYes.y - 0.4f)*Screen.height, buttonYes.width*Screen.width, buttonYes.height*Screen.height), "Stay", "labelGo"))
			{
				quitAsked = false;
			}
				
			GUI.Color = new Color(1f, 0.1f, 0.1f, 1f);
			if(GUI.Button(new Rect(buttonNo.x*Screen.width, (buttonNo.y - 0.4f)*Screen.height, buttonNo.width*Screen.width, buttonNo.height*Screen.height), "Quit", "labelGo"))
			{
				Network.Disconnect();
				Application.LoadLevel("LAN");
			}
		}
		
		#endregion
		
	
		
		
	
	}
	
	public void launchOption()
	{
		getZonePack().onPopout();
		getZoneSong().onPopout();
		getZoneInfo().onEnterOption();
		getZoneOption().onPopin();
		GetComponent<ChatScript>().activeChat(false);
	}
	
	void Update(){
		
		if(LANManager.Instance.isCreator)
		{
			if(nws.isSearching)
			{
				nws.isSongAvailable();
			}
			
			if(nws.isRefreshVote)
			{
				nws.refreshVoteMode();	
			}
			
		}
		
		
		if(!alreadyFade && timeFade >= 0.1f){
			
			GetComponent<FadeManager>().FadeOut();
			alreadyFade = true;
			
		}else if(!alreadyFade){
			timeFade += Time.deltaTime;	
		}
	
		if(!pickSetted && LANManager.Instance.isCreator && nws.isPlayerStatutReady(LANStatut.SELECTSONG))
		{
			nws.setPicker();
			networkView.RPC("notifyReadyToChoose", RPCMode.Others);
			pickSetted = true;
		}
		
		if(LANManager.Instance.isCreator && getZoneOption().activeModule)
		{
			nws.refreshOptionMode();	
		}
		
		if(Input.GetKeyDown(KeyCode.ESCAPE) && !quitAsked && !getZoneOption().activeModule && !inVoteMode)
		{
			quitAsked = true;
		}
		
		if(quitAsked && alphaQuitAsked <= 0.8f)
		{
			alphaQuitAsked += speedAlphaQuitAsked*Time.deltaTime;
		}else if(!quitAsked && alphaQuitAsked > 0f)
		{
			alphaQuitAsked -= speedAlphaQuitAsked*Time.deltaTime;
		}
		
		#region MoveToOptionUpdate
	
		
		
		
		
		//refreshBanner and sound
		if(songSelected != null && !alreadyRefresh){
			if(time >= timeBeforeDisplay){
				plane.renderer.material.mainTexture = actualBanner;
				var thebanner = songSelected.First().Value.GetBanner(actualBanner);
				
				startTheSongStreamed();
				if(thebanner != null){
					actualBanner = thebanner;	
				}else{
					plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[getZonePack().getActivePack()];
				}
				
				alreadyRefresh = true;
				FadeInBanner = true;
			}else{
				time += Time.deltaTime;	
				onFadeOutBanner();
			}
		}
		
		//Fade Banner
		if(FadeInBanner){
			alphaBanner += Time.deltaTime/speedFadeAlpha;
			plane.renderer.material.color = new Color(plane.renderer.material.color.r, plane.renderer.material.color.g, plane.renderer.material.color.b, alphaBanner);	
			if(alphaBanner >= 1){
				FadeInBanner = false;
				FadeOutBanner = false;
			}
		}else if(FadeOutBanner && alphaBanner > 0){
			alphaBanner -= Time.deltaTime/speedFadeAlpha;
			plane.renderer.material.color = new Color(plane.renderer.material.color.r, plane.renderer.material.color.g, plane.renderer.material.color.b, alphaBanner);
			if(alphaBanner <= 0){
				FadeOutBanner = false;	
			}
		}
		
		
		#endregion
		
		
		
			
			
			
		if(Input.GetKeyDown(KeyCode.Escape) && !getZoneOption().activeModule && !fadedOut){
				fadedOut = true;
				GetComponent<FadeManager>().FadeIn("mainmenu");	
		}
			
			
			
			
			
		
		
		#region Audio
			
		if(songSelected != null && alreadyRefresh && mainThemeClip.volume > 0){
				mainThemeClip.volume -= Time.deltaTime/speedAudioVolume;
		}
		if((songSelected == null || !alreadyRefresh) && mainThemeClip.volume < 1){
				mainThemeClip.volume += Time.deltaTime/speedAudioVolume;
		}
			
		#endregion
		
	}
	
	public void popinVoteMode()
	{
		
		if(getZoneSong().activeModule)
		{
			getZoneSong().onPopout();
		}
		if(!GetComponent<ChatScript>().isActive())
		{
			GetComponent<ChatScript>().forcePopinChat();	
		}
		GetComponent<ChatScript>().setLockButton(true);
		getZonePack().onPopout();
		songSelected = nws.lastSongChecked.Value;
		getZoneSong().locked = true;
		getZoneInfo().setActualySelected(nws.lastSongChecked.Key);
		getZoneInfo().refreshDifficultyDisplayedVote();
		refreshBanner();
		if(LANManager.Instance.isPicker)
		{
			alreadyVote = true;	
		}
	}
	
	public void popoutVoteMode()
	{
		getZoneSong().onPopin();
		GetComponent<ChatScript>().forcePopoutChat();
		GetComponent<ChatScript>().setLockButton(false);
		getZonePack().onPopin();
		getZoneInfo().disableDifficultyDisplayedVote();
		getZoneSong().locked = false;
		songSelected = null;
		alreadyVote = false;
	}
	
	public void popinOption()
	{
		getZonePack().onPopout();
		getZoneSong().onPopout();
		getZoneInfo().onEnterOption();
		getZoneOption().onPopin();
		GetComponent<ChatScript>().activeChat(false);
		if(LANManager.Instance.isCreator)
		{
			var lb = LANManager.Instance.parseLeaderboardToString(LANManager.Instance.getLeaderboard());
			networkView.RPC("getLeaderboard", RPCMode.Others, lb);
			nws.getLeaderboard(lb);
		
		}
		
	}
	
	public void play()
	{
		
		DataManager.Instance.speedmodSelected = speedmodSelected;
		
		getZoneOption().fillDataManager();
		
		DataManager.Instance.songSelected = songSelected[DataManager.Instance.difficultySelected];
		
		DataManager.Instance.packSelected = getZonePack().getActivePack();
		DataManager.Instance.mousePosition = getZoneSong().getStartNumber();
		
		///Save prefs
		ProfileManager.Instance.currentProfile.lastSpeedmodUsed = speedmodstring;
		ProfileManager.Instance.currentProfile.lastBPM = bpmstring;
		ProfileManager.Instance.currentProfile.inBPMMode = DataManager.Instance.BPMEntryMode;
		
		getZoneOption().instantClose();
		getZoneLaunchSong().activate();
	}
	
	public void refreshBanner()
	{
		time = 0f;
		alreadyRefresh = false;	
	}
	
	public void refreshPackBanner()
	{
		alphaBanner = 1f;
		FadeInBanner = true;
		plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[getZonePack().getActivePack()];
	}
	
	public PackZoneLAN getZonePack()
	{
		return packZone;
	}
	
	public SongZoneLAN getZoneSong()
	{
		return songZone;
	}
	
	public OptionZoneLAN getZoneOption()
	{
		return optionZone;
	}
	
	public InfoZoneLAN getZoneInfo()
	{
		return infoZone;
	}
	
	public LaunchSongZoneLAN getZoneLaunchSong()
	{
		return launchSongZone;
	}
	
	public void onFadeInBanner()
	{
		FadeInBanner = true;	
	}
	
	public void onFadeOutBanner()
	{
		FadeOutBanner = true;	
	}
	
	public void shutSong()
	{
		songClip.Stop();	
	}
	
	#region sound
	IEnumerator startTheSongUnstreamed(){
		
		DestroyImmediate(actualClip);
		var path = songSelected.First().Value.GetAudioClipUnstreamed();
		var thewww = new WWW(path);
		while(!thewww.isDone){
			yield return new WaitForFixedUpdate();
		}
		Debug.Log ("coucou");
		actualClip = thewww.GetAudioClip(false, false, AudioType.OGGVORBIS);
		thewww.Dispose();
		songClip.clip = actualClip;
		songClip.time = (float)songSelected.First().Value.samplestart;
		songClip.loop = true;
		songClip.Play();
	}
	
	public void startTheSongStreamed(){
		
		DestroyImmediate(actualClip);
		
		actualClip = songSelected.First().Value.GetAudioClip();
		songClip.clip = actualClip;
		songClip.time = (float)songSelected.First().Value.samplestart;
		songClip.PlayOneShot(actualClip);
	}
	#endregion
	
	#region util
	
	
	
	public string refreshPreference(double score)
	{
		var queryreturn = "";
		var kv = ProfileManager.Instance.FindTheBestScore(songSelected[getZoneInfo().getActualySelected()].sip);
		var bestfriendscore = kv.Key;
		var bestnamefriendscore = kv.Value;
		var isScoreFail = false;
		
		var mystats = ProfileManager.Instance.FindTheSongStat(songSelected[getZoneInfo().getActualySelected()].sip);
		if(mystats != null){
			score = mystats.score;
			isScoreFail = mystats.fail;
			speedmodstring = mystats.speedmodpref.ToString("0.00");
			speedmodSelected = (float)mystats.speedmodpref;
			var bpmtotest = songSelected.First().Value.bpmToDisplay;
			if(bpmtotest.Contains("->")){
				bpmstring = (System.Convert.ToDouble(System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode])*speedmodSelected)).ToString("0");
			}else{
				bpmstring = (System.Convert.ToDouble(bpmtotest)*speedmodSelected).ToString("0");
			}
		}else{
			score = -1;
			isScoreFail = false;
			speedmodstring = DataManager.Instance.songSelected != null ? DataManager.Instance.speedmodSelected.ToString("0.00") : "2.00";
			speedmodSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.speedmodSelected : 2f;
			if(DataManager.Instance.BPMEntryMode){
				double resultbpm;
				if(System.Double.TryParse(bpmstring, out resultbpm)){
					var bpmtotest = songSelected.First().Value.bpmToDisplay;
					if(bpmtotest.Contains("->")){
						speedmodstring = (resultbpm/System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode])).ToString("0.00");
						speedmodSelected = (float)(resultbpm/System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode]));
					}else{
						speedmodstring = (resultbpm/System.Convert.ToDouble(bpmtotest)).ToString("0.00");
						speedmodSelected = (float)(resultbpm/System.Convert.ToDouble(bpmtotest));
					}
				}else{
					speedmodstring = "?";
				}
				
			}else{
				var bpmtotest = songSelected.First().Value.bpmToDisplay;
				if(bpmtotest.Contains("->")){
					bpmstring = (System.Convert.ToDouble(System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode])*speedmodSelected)).ToString("0");
				}else{
					bpmstring = (System.Convert.ToDouble(bpmtotest)*speedmodSelected).ToString("0");
				}
			}
			
		}
		queryreturn = score + ";" + bestfriendscore + ";" + bestnamefriendscore + ";" + (isScoreFail ? "1" : "0");
		return queryreturn;
	}
	
	
	
	
	
	#endregion
}
