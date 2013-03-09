using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GeneralScript : MonoBehaviour {
	
	#region variable	
	//GameObject library
	
	public GameObject plane;
	public GUISkin skin;
	public LineRenderer separator;
	
	public AudioSource songClip;
	public AudioSource mainThemeClip;
	public ParticleSystem Line1;
	public ParticleSystem Fond1;
	
	private PackZone packZone;
	private SongZone songZone;
	private InfoZone infoZone;
	private OptionZone optionZone;
	private LaunchSongZone launchSongZone;
	
	//List And dictionary
	
	public Dictionary<Difficulty, Song> songSelected;
	
	public Dictionary<string, Texture2D> tex;
	
	
	//General feature
	private string speedmodstring;
	private string bpmstring;
	private float speedmodSelected;
	
	//Banner
	private Texture2D actualBanner;
	public float speedFadeAlpha;
	private float alphaBanner;
	private bool FadeOutBanner;
	private bool FadeInBanner;
	
	
	//General GUI
	private float totalAlpha;
	private string textButton;
	private float timeFade;
	private ErrorLabel error;
	public Rect posLabelLoading;
	public Rect Jouer;
	public Rect Option;
	
	
	//Declancheurs
	
	private bool movinOption;
	private bool OptionMode;
	private bool movinNormal;
	private bool movinSong;
	private bool SongMode;
	private bool fadedOut;
	private bool displayLoading;
	
	
	//Move to option mode
	public float speedMoveOption;
	public float limiteMoveOption;
	private float basePositionSeparator = -5.5f;
	private float offsetXDiff;
	private float offsetYDiff;
	private Vector2 departOptionDiff;
	private Vector2 moveOptionDiff;
	public Vector2 arriveOptionDiff;
	private bool animok;
	
	
	
	
	
	
	
	
	
	//General Update
	public float timeBeforeDisplay;
	private float time;
	private bool alreadyRefresh;
	private bool alreadyFade;
	
	//Sound
	AudioClip actualClip;
	public float speedAudioVolume;
	
	#endregion
	// Use this for initialization
	
	
	//test 
	
	void Awake()
	{
		DataManager.Instance.removeRatedSong();
	}
	
	void Start () {
		
		packZone = GetComponent<PackZone>();
		songZone = GetComponent<SongZone>();
		infoZone = GetComponent<InfoZone>();
		optionZone = GetComponent<OptionZone>();
		launchSongZone = GetComponent<LaunchSongZone>();
		
		timeFade = 0f;
		if(!LoadManager.Instance.alreadyLoaded) ProfileManager.Instance.CreateTestProfile();
		if(!LoadManager.Instance.alreadyLoaded) TextManager.Instance.LoadTextFile();
		if(!LoadManager.Instance.alreadyLoaded) LoadManager.Instance.Loading();
		error = GetComponent<ErrorLabel>();
		
		
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
		tex.Add("Option2", (Texture2D) Resources.Load("Rate"));
		tex.Add("Option3", (Texture2D) Resources.Load("Skin"));
		tex.Add("Option4", (Texture2D) Resources.Load("HitJudge"));
		tex.Add("Option5", (Texture2D) Resources.Load("ScoreJudge"));
		tex.Add("Option6", (Texture2D) Resources.Load("LifeJudge"));
		tex.Add("Option7", (Texture2D) Resources.Load("Display"));
		tex.Add("Option8", (Texture2D) Resources.Load("Race"));
		tex.Add("Option9", (Texture2D) Resources.Load("Death"));
		tex.Add("Black", (Texture2D) Resources.Load("black"));
		
		tex.Add("Failed", (Texture2D) Resources.Load("Fail"));
		tex.Add("BAD", (Texture2D) Resources.Load("NoteBAD"));
		tex.Add("C", (Texture2D) Resources.Load("NoteC"));
		tex.Add("B", (Texture2D) Resources.Load("NoteB"));
		tex.Add("A", (Texture2D) Resources.Load("NoteA"));
		tex.Add("S", (Texture2D) Resources.Load("NoteS"));
		
		
		
		actualBanner = new Texture2D(256,512);
		
		
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
		
		totalAlpha = 0f;
		
		
		
		
		alreadyRefresh = true;
		
		
		movinOption = false;
		OptionMode = false;
		SongMode = false;
		movinSong = false;
		
		fadedOut = false;
		displayLoading = false;
		
		animok = true;
		
		
		textButton = "Option";
		
		alphaBanner = 1f;
		
		
		FadeOutBanner = false;
		FadeInBanner = false;
		
		
		
		
			
		
		alreadyFade = false;
			
		
		
		
		
		
		//quickMode a déplacer aussi
		if(DataManager.Instance.quickMode){
			speedMoveOption = 0.01f;
		}
	}
	
	// Update is called once per frame
	public void OnGUI () {
		
		

		
		
	
		
		#region optionPlayGUI
		//Option/jouer
		if(songSelected != null){
		
			
		
			
			//Jouer
			if(GUI.Button(new Rect(Jouer.x*Screen.width, Jouer.y*Screen.height, Jouer.width*Screen.width, Jouer.height*Screen.height), "Play", "labelGo")){
				
				DataManager.Instance.songSelected =  songSelected[getZoneInfo().getActualySelected()];
				DataManager.Instance.difficultySelected = getZoneInfo().getActualySelected();
				DataManager.Instance.speedmodSelected = speedmodSelected;
				
				getZoneOption().fillDataManager();
				
				DataManager.Instance.packSelected = getZonePack().getActivePack();
				DataManager.Instance.mousePosition = getZoneSong().getStartNumber();
				
				///Save prefs
				ProfileManager.Instance.currentProfile.lastSpeedmodUsed = speedmodstring;
				ProfileManager.Instance.currentProfile.lastBPM = bpmstring;
				ProfileManager.Instance.currentProfile.inBPMMode = DataManager.Instance.BPMEntryMode;
				
				getZonePack().onPopout();
				getZoneSong().onPopout();
				getZoneInfo().onEnterLaunch();
				getZoneOption().instantClose();
				getZoneLaunchSong().activate();
				
			}
		
			//Option
			if(GUI.Button(new Rect(Option.x*Screen.width, Option.y*Screen.height, Option.width*Screen.width, Option.height*Screen.height), textButton, "labelGo") && !movinOption && !movinNormal && animok){
				if(textButton == "Option"){
					getZonePack().onPopout();
					getZoneSong().onPopout();
					getZoneInfo().onEnterOption();
					getZoneOption().onPopin();
					textButton = "Back";
					
				}else{
					getZoneOption().onPopout();
				}
				animok = false;
			}
			
		}
		
		
		#endregion
		
	
		
		
	
	}
	
	
	
	void Update(){
		
		if(!alreadyFade && timeFade > 0.1f){
			GetComponent<FadeManager>().FadeOut();
			alreadyFade = true;
		}else{
			timeFade += Time.deltaTime;	
		}
		#region MoveToOptionUpdate
	
		
		
		
		
		//refreshBanner and sound
		if(songSelected != null && !alreadyRefresh){
			if(time >= timeBeforeDisplay){
				plane.renderer.material.mainTexture = actualBanner;
				var thebanner = songSelected.First().Value.GetBanner(actualBanner);
				//StartCoroutine(startTheSongUnstreamed());
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
		}else if(FadeOutBanner){
			alphaBanner -= Time.deltaTime/speedFadeAlpha;
			plane.renderer.material.color = new Color(plane.renderer.material.color.r, plane.renderer.material.color.g, plane.renderer.material.color.b, alphaBanner);
			if(alphaBanner <= 0){
				FadeOutBanner = false;	
			}
		}
		
		
		#endregion
		
		
		
			
			
			
		if(Input.GetKeyDown(KeyCode.Escape) && !getZoneOption().activeModule){
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
	
	public void refreshBanner()
	{
		alreadyRefresh = false;	
	}
	
	public PackZone getZonePack()
	{
		return packZone;
	}
	
	public SongZone getZoneSong()
	{
		return songZone;
	}
	
	public OptionZone getZoneOption()
	{
		return optionZone;
	}
	
	public InfoZone getZoneInfo()
	{
		return infoZone;
	}
	
	public LaunchSongZone getZoneLaunchSong()
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
