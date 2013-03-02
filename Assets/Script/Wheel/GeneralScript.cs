using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GeneralScript : MonoBehaviour {
	
	#region variable	
	//GameObject library
	
	
	public GUISkin skin;
	public GameObject plane;
	public LineRenderer separator;
	public GameObject cacheOption;
	public AudioSource songClip;
	public AudioSource mainThemeClip;
	public ParticleSystem Line1;
	public ParticleSystem Fond1;
	public ParticleSystem Explode1;
	public ParticleSystem Explode2;
	public ParticleSystem Explode3;
	
	//List And dictionary
	
	private Dictionary<string, Texture2D> tex;
	
	
	
	
	//General GUI
	private float totalAlpha;
	private string textButton;
	private float timeFade;
	private ErrorLabel error;
	public Rect posLabelLoading;
	
	
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
	
	
	
	
	//Anim options
	public float timeFadeOut;
	public float timeFadeOutDisplay;
	private float[] alphaText;
	private float[] alphaDisplay;
	private float[] offsetFading;
	private float[] offsetPreviousFading;
	private bool[] isFading;
	private bool[] isFadingDisplay;
	private int previousSelected;
	public float offsetBaseFading;
	
	
	
	
	//General Update
	public float timeBeforeDisplay;
	private float time;
	private bool alreadyRefresh;
	private bool alreadyFade;
	
	//Sound
	AudioClip actualClip;
	public float speedAudioVolume;
	public AudioClip launchSong;
	
	#endregion
	// Use this for initialization
	
	
	//test 
	
	void Awake()
	{
		DataManager.Instance.removeRatedSong();
	}
	
	void Start () {
		
		
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
		
		
		
		
		
		
		
		
		
		
		
		
		plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[packs.ElementAt(0).Key];
		diffNumber = new int[6];
		actualClip = new AudioClip();
		for(int i=0;i<6; i++){ diffNumber[i] = 0; }
		decalFade = 0f;
		decalFadeM = 0f;
		fadeAlpha = 0f;
		posLabel = 0f;
		time = 0f;
		alphaBanner = 1f;
		totalAlpha = 0f;
		alphaBlack = 0f;
		
		alphaSongLaunch = new float[6];
		for(int i=0;i<6; i++){ alphaSongLaunch[i] = 0f; }
		alreadyRefresh = true;
		FadeOutBanner = false;
		FadeInBanner = false;
		graph.enabled = false;
		movinOption = false;
		OptionMode = false;
		SongMode = false;
		movinSong = false;
		locked = false;
		fadedOut = false;
		displayLoading = false;
		
		animok = true;
		speedmodok = true;
		rateok = true;
		
		textButton = "Option";
		matCache = cacheOption.renderer.material;
		fadeAlphaOptionTitle = 1f;
		stateLoading = new bool[9];
		displaySelected = new bool[DataManager.Instance.aDisplay.Length];
		for(int i=0;i<stateLoading.Length-1;i++) stateLoading[i] = false;
		for(int j=0;j<DataManager.Instance.aDisplay.Length;j++) displaySelected[j] = DataManager.Instance.songSelected != null ? DataManager.Instance.displaySelected[j] : false;
		
		
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
		DataManager.Instance.BPMEntryMode = ProfileManager.Instance.currentProfile.inBPMMode;
		
		rateSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.rateSelected : 0f;
		scoreJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.scoreJudgeSelected : Judge.NORMAL;
		hitJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.hitJudgeSelected : Judge.NORMAL;
		lifeJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.lifeJudgeSelected : Judge.NORMAL;
		skinSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.skinSelected : 0;
		raceSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.raceSelected : 0;
		deathSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.deathSelected :0;
		
		
		
		ratestring = rateSelected.ToString("00");
		
		alphaText = new float[stateLoading.Length];
		isFading = new bool[stateLoading.Length];
		alphaDisplay = new float[stateLoading.Length];
		isFadingDisplay = new bool[stateLoading.Length];
		offsetFading = new float[stateLoading.Length];
		offsetPreviousFading = new float[stateLoading.Length];
		for(int i=0;i<stateLoading.Length;i++) offsetFading[i] = 0f;
		for(int i=0;i<stateLoading.Length;i++) offsetPreviousFading[i] = 0f;
		for(int i=0;i<stateLoading.Length;i++) isFading[i] = false;
		for(int i=0;i<stateLoading.Length;i++) alphaText[i] = 1f;
		for(int i=0;i<stateLoading.Length;i++) isFadingDisplay[i] = false;
		for(int i=0;i<stateLoading.Length;i++) alphaDisplay[i] = displaySelected[i] ? 1f : 0f;
		
		
		search = "";
		searchOldValue = "";
		songList = new Dictionary<string, Dictionary<Difficulty, Song>>();
			
		
		alreadyFade = false;
			
		actualySelected =  DataManager.Instance.difficultySelected;
		trulySelected = DataManager.Instance.difficultySelected;
		onHoverDifficulty = Difficulty.NONE;
		
		if(DataManager.Instance.mousePosition != -1){
			startnumber = DataManager.Instance.mousePosition;
			currentstartnumber = startnumber;
		}
		
		basePosCubeBase = new Vector3(cubeBase.transform.position.x, 5f, cubeBase.transform.position.z);
		cubeBase.transform.position = new Vector3(basePosCubeBase.x, basePosCubeBase.y - (3f*startnumber), basePosCubeBase.z);
		
		//quickMode
		if(DataManager.Instance.quickMode){
			speedMoveOption = 0.01f;
			speedMoveSong = 0.01f;
			speedAlphaSongLaunch = 0.1f;
			speedAlphaBlack = 0.1f;
			timeOption = 0.02f;
		}
	}
	
	// Update is called once per frame
	public void OnGUI () {
		GUI.skin = skin;
		
		
		
		GUI.color = new Color(1f, 1f, 1f, 1f - totalAlpha);
		
		if(!OptionMode && !SongMode && !movinSong){
			#region packGUI
			//Packs
			if(!LoadManager.Instance.isAllowedToSearch(search)){
				var decal = ((Screen.width - wd*Screen.width)/2);
				if(movinBackward) GUI.Label(new Rect((xm10*2f + decalFadeM)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(PrevInt(numberPack, 2)).Key);
				if(!movinForward) GUI.Label(new Rect((xm10 + decalFadeM)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(PrevInt(numberPack, 1)).Key);
				GUI.Label(new Rect(decalFade > 0 ? decalFade*Screen.width + decal : decalFadeM*Screen.width + decal  , y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(numberPack).Key);
				if(!movinBackward) GUI.Label(new Rect((x10 + decalFade)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(NextInt(numberPack, 1)).Key);
				if(movinForward) GUI.Label(new Rect((x10*2f + decalFade)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(NextInt(numberPack, 2)).Key);
				
			
			
				if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","LBackward") && !movinBackward && !movinNormal && !movinOption){
					nextnumberPack = PrevInt(numberPack, 1);
					activePack(packs.ElementAt(nextnumberPack).Key);
					movinBackward = true;
					startnumber = 0;
					currentstartnumber = 0;
					camerapack.transform.position = new Vector3(0f, 0f, 0f);
					cubeBase.transform.position = basePosCubeBase;
				}
				if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height + ecart*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","Backward") && !movinNormal && !movinOption){
					nextnumberPack = PrevInt(numberPack, 3);
					activePack(packs.ElementAt(nextnumberPack).Key);
					movinBackwardFast = true;
					startnumber = 0;
					currentstartnumber = 0;
					camerapack.transform.position = new Vector3(0f, 0f, 0f);
					cubeBase.transform.position = basePosCubeBase;
				}
				
				if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","LForward") && !movinForward && !movinNormal && !movinOption)
				{
					nextnumberPack = NextInt(numberPack, 1);
					activePack(packs.ElementAt(nextnumberPack).Key);
					movinForward = true;
					startnumber = 0;
					currentstartnumber = 0;
					camerapack.transform.position = new Vector3(0f, 0f, 0f);
					cubeBase.transform.position = basePosCubeBase;
				}
				if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height + ecart*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","Forward") && !movinNormal && !movinOption){
					nextnumberPack = NextInt(numberPack, 3);
					activePack(packs.ElementAt(nextnumberPack).Key);
					movinForwardFast = true;
					startnumber = 0;
					camerapack.transform.position = new Vector3(0f, 0f, 0f);
					cubeBase.transform.position = basePosCubeBase;
				}
			}
			#endregion
			
			#region songGUI
			//SONGS
			
			var packOnRender = LoadManager.Instance.isAllowedToSearch(search) ? songList : LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key];
			var thepos = -posLabel;
			for(int i=0; i<packOnRender.Count; i++){
				if(thepos >= 0f && thepos <= numberToDisplay){
					
					var el = packOnRender.ElementAt(i);
					var title = el.Value.First().Value.title;
					if(title.Length > 35) title = title.Remove(35, title.Length - 35) + "...";
					var subtitle = el.Value.First().Value.subtitle;
					if(subtitle.Length > 50) subtitle = subtitle.Remove(50, subtitle.Length - 50) + "...";
					
					GUI.color = new Color(0f, 0f, 0f, 1f - totalAlpha);
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), title, "songlabel");
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos + offsetSubstitle)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), subtitle, "infosong");
					GUI.color = new Color(1f, 1f, 1f, 1f - totalAlpha);
					GUI.Label(new Rect(posSonglist.x*Screen.width, (posSonglist.y + ecartSong*thepos)*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), title, "songlabel");
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos + offsetSubstitle)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), subtitle, "infosong");
				}else if(thepos > -1f && thepos < 0f){
					var el = packOnRender.ElementAt(i);
					
					var title = el.Value.First().Value.title;
					if(title.Length > 35) title = title.Remove(35, title.Length - 35) + "...";
					var subtitle = el.Value.First().Value.subtitle;
					if(subtitle.Length > 50) subtitle = subtitle.Remove(50, subtitle.Length - 50) + "...";
					
					GUI.color = new Color(0f, 0f, 0f, 1f + thepos);
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), title, "songlabel");
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos + offsetSubstitle)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), subtitle, "infosong");
					GUI.color = new Color(1f, 1f, 1f, 1f + thepos);
					GUI.Label(new Rect(posSonglist.x*Screen.width, (posSonglist.y + ecartSong*thepos)*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), title, "songlabel");
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos + offsetSubstitle)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), subtitle, "infosong");
				}
				thepos++;
			}
			#endregion
		
			//Perte de focus ?
			#region SearchBar
				if(GUI.Button(new Rect(posSwitchSearch.x*Screen.width, posSwitchSearch.y*Screen.height, posSwitchSearch.width*Screen.width, posSwitchSearch.height*Screen.height), sortToString(DataManager.Instance.sortMethod), "labelGoLittle")){
					DataManager.Instance.sortMethod++;
					if((int)DataManager.Instance.sortMethod > (int)Sort.BPM){
						DataManager.Instance.sortMethod = (Sort)0;
					}
					search = "";
				}
				search = GUI.TextArea(new Rect(SearchBarPos.x*Screen.width, SearchBarPos.y*Screen.height, SearchBarPos.width*Screen.width, SearchBarPos.height*Screen.height), search.Trim());
				
				if(search != searchOldValue){
					if(!String.IsNullOrEmpty(search.Trim()) && LoadManager.Instance.isAllowedToSearch(search)){
						songList = LoadManager.Instance.ListSong(songList, search.Trim());
						if(particleOnPlay != null){
							cubeSelected = null;
							songSelected = null;
							FadeOutBanner = true;
							graph.enabled = false;
							songClip.Stop();
							PSDiff[(int)actualySelected].gameObject.active = false;
							desactiveDiff();
							particleOnPlay.active = false;
							locked = false;
						}
						startnumber = 0;
						currentstartnumber = 0;
						camerapack.transform.position = new Vector3(0f, 0f, 0f);
						cubeBase.transform.position = basePosCubeBase;
						desactivePack();
						createCubeSong(songList);
						activeCustomPack();
						StartCoroutine(AnimSearchBar(false));
						var	num = 0;
						error.displayError = (DataManager.Instance.sortMethod >= Sort.DIFFICULTY && !Int32.TryParse(search, out num));
					}else if(!LoadManager.Instance.isAllowedToSearch(search) && searchOldValue.Trim().Length > search.Trim().Length){
						songList.Clear();
						if(particleOnPlay != null){
							cubeSelected = null;
							songSelected = null;
							FadeOutBanner = true;
							graph.enabled = false;
							songClip.Stop();
							PSDiff[(int)actualySelected].gameObject.active = false;
							desactiveDiff();
							particleOnPlay.active = false;
							locked = false;
						}
						startnumber = 0;
						currentstartnumber = 0;
						camerapack.transform.position = new Vector3(0f, 0f, 0f);
						cubeBase.transform.position = basePosCubeBase;
						activePack(packs.ElementAt(nextnumberPack).Key);
						DestroyCustomCubeSong();
						StartCoroutine(AnimSearchBar(true));
						error.displayError = false;
					}
					if(songList.Count == 0 && LoadManager.Instance.isAllowedToSearch(search)){
						GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
						GUI.Label(new Rect(posSonglist.x*Screen.width, posSonglist.y*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), "No entry", "songlabel");
						GUI.color = new Color(1f, 1f, 1f, 1f);
					}
				}
				searchOldValue = search;
			
			#endregion
		}
		
		GUI.color = new Color(1f, 1f, 1f, 1f - totalAlpha);
		
		#region songdifficultyGUI
		//SongDifficulty
		if(songSelected != null && !OptionMode && !SongMode){
			var realx = posDifficulty.x*Screen.width;
			var realy = posDifficulty.y*Screen.height;
			var realwidth = posDifficulty.width*Screen.width;
			var realheight = posDifficulty.height*Screen.height;
			var diffx = posNumberDiff.x*Screen.width;
			var diffy = posNumberDiff.y*Screen.height;
			var diffwidth = posNumberDiff.width*Screen.width;
			var diffheight = posNumberDiff.height*Screen.height;
			var theRealEcart = ecartDiff*Screen.height;
			var ecartjump = 0f;
			for(int i=0; i<=(int)Difficulty.EDIT; i++){
				if(diffNumber[i] != 0){
					if((!movinNormal) || (movinNormal && diffNumber[i] != 0 && i != (int)actualySelected)){
						GUI.color = new Color(1f, 1f, 1f, 1f  - totalAlpha);
						var zoom = 0f;
						if(onHoverDifficulty  == (Difficulty)i) zoom = diffZoom; 
						GUI.DrawTexture(new Rect(realx + offsetX[i]*Screen.width - zoom*Screen.width, realy + theRealEcart*ecartjump - zoom*Screen.height, realwidth + zoom*2f*Screen.width, realheight + zoom*2f*Screen.height), tex[((Difficulty)i).ToString()]);
						/*GUI.color = new Color(0f, 0f, 0f, 1f);
						GUI.Label(new Rect(diffx + 1f, diffy + theRealEcart*ecartjump + 1f, diffwidth, diffheight), songSelected[(Difficulty)i].level.ToString(), "numberdiff");
						*/
						GUI.color = new Color(DataManager.Instance.diffColor[i].r, DataManager.Instance.diffColor[i].g, DataManager.Instance.diffColor[i].b, 1f - totalAlpha);
						GUI.Label(new Rect(diffx, diffy + theRealEcart*ecartjump, diffwidth, diffheight), songSelected[(Difficulty)i].level.ToString(), "numberdiff");
					}
					ecartjump++;
				}
			}
			GUI.color = new Color(1f, 1f, 1f, 1f  - totalAlpha);
			GUI.DrawTexture(new Rect(posGraph.x*Screen.width, posGraph.y*Screen.height, posGraph.width*Screen.width, posGraph.height*Screen.height), tex["graph"]);
		}
		#endregion
		
		GUI.color = new Color(1f, 1f, 1f, 1f - totalAlpha);
		
		
		#region songinfoGUI
		//Song Info
		if(songSelected != null && !OptionMode && !SongMode){ 
			//BPM
			GUI.Label(new Rect(BPMDisplay.x*Screen.width , BPMDisplay.y*Screen.height, BPMDisplay.width*Screen.width, BPMDisplay.height*Screen.height), "BPM\n" + songSelected[(Difficulty)actualySelected].bpmToDisplay, "bpmdisplay");
			
			//Artist n stepartist
			GUI.Label(new Rect(artistnstepDisplay.x*Screen.width , artistnstepDisplay.y*Screen.height, artistnstepDisplay.width*Screen.width, artistnstepDisplay.height*Screen.height), songSelected[(Difficulty)actualySelected].artist + " - Step by : " + songSelected[(Difficulty)actualySelected].stepartist);
			
			//Number of step
			GUI.Label(new Rect(posInfo.x*Screen.width , (posInfo.y + offsetInfo*0f )*Screen.height, posInfo.width*Screen.width, posInfo.height*Screen.height), songSelected[(Difficulty)actualySelected].numberOfSteps + " Steps", "infosong");
			//Number of jumps						   
			GUI.Label(new Rect(posInfo.x*Screen.width , (posInfo.y + offsetInfo*1f )*Screen.height, posInfo.width*Screen.width, posInfo.height*Screen.height),  songSelected[(Difficulty)actualySelected].numberOfJumps + " Jumps", "infosong");
			//Number of hands						   
			GUI.Label(new Rect(posInfo.x*Screen.width , (posInfo.y + offsetInfo*2f )*Screen.height, posInfo.width*Screen.width, posInfo.height*Screen.height), songSelected[(Difficulty)actualySelected].numberOfHands + " Hands", "infosong");
			//Number of mines						  
			GUI.Label(new Rect(posInfo2.x*Screen.width , (posInfo2.y + offsetInfo*0f )*Screen.height, posInfo2.width*Screen.width, posInfo2.height*Screen.height), songSelected[(Difficulty)actualySelected].numberOfMines + " Mines", "infosong");
			//Number of freeze							
			GUI.Label(new Rect(posInfo2.x*Screen.width , (posInfo2.y + offsetInfo*1f )*Screen.height, posInfo2.width*Screen.width, posInfo2.height*Screen.height), songSelected[(Difficulty)actualySelected].numberOfFreezes + " Freezes", "infosong");
			//Number of rolls						   
			GUI.Label(new Rect(posInfo2.x*Screen.width , (posInfo2.y + offsetInfo*2f )*Screen.height, posInfo2.width*Screen.width, posInfo2.height*Screen.height), songSelected[(Difficulty)actualySelected].numberOfRolls + " Rolls", "infosong");
			//Number of cross						    
			GUI.Label(new Rect(posInfo3.x*Screen.width , (posInfo3.y + offsetInfo*2f )*Screen.height, posInfo3.width*Screen.width, posInfo3.height*Screen.height), songSelected[(Difficulty)actualySelected].numberOfCross + " Cross pattern", "infosong");
			//Number of footswitch						
			GUI.Label(new Rect(posInfo3.x*Screen.width , (posInfo3.y + offsetInfo*3f )*Screen.height, posInfo3.width*Screen.width, posInfo3.height*Screen.height), songSelected[(Difficulty)actualySelected].numberOfFootswitch + " Footswitch pat.", "infosong");
			//Average Intensity					   	
			//GUI.Label(new Rect(posInfo.x*Screen.width , (posInfo.y + offsetInfo*8f )*Screen.height, posInfo.width*Screen.width, posInfo.height*Screen.height), "Av. Intensity : " + songSelected[(Difficulty)actualySelected].stepPerSecondAverage.ToString("0.00") + " SPS", "infosong");
			
			//Max Intensity					
			GUI.Label(new Rect(posMaxinten.x*Screen.width , (posMaxinten.y)*Screen.height, posMaxinten.width*Screen.width, posMaxinten.height*Screen.height), "Max : " + songSelected[(Difficulty)actualySelected].stepPerSecondMaximum.ToString("0.00") + " SPS ("+ ((System.Convert.ToDouble(songSelected[(Difficulty)actualySelected].stepPerSecondMaximum)/4f)*60f).ToString("0") + " BPM)", "infosong");
			//Longest Stream (TimePerSecond)		    
			GUI.Label(new Rect(posInfo3.x*Screen.width , (posInfo3.y + offsetInfo*1f)*Screen.height, posInfo3.width*Screen.width, posInfo3.height*Screen.height), System.Convert.ToDouble(songSelected[(Difficulty)actualySelected].stepPerSecondStream) < 8f ? "No stream" : "Max stream : " + songSelected[(Difficulty)actualySelected].longestStream.ToString("0.00") + " sec (" + songSelected[(Difficulty)actualySelected].stepPerSecondStream.ToString("0.00") + " SPS)", "infosong");
			//Number of BPM change						
			GUI.Label(new Rect(posInfo4.x*Screen.width , (posInfo4.y + offsetInfo*0f)*Screen.height, posInfo4.width*Screen.width, posInfo4.height*Screen.height), songSelected[(Difficulty)actualySelected].bpms.Count - 1 + " BPM changes", "infosong");
			//Number of stops						    
			GUI.Label(new Rect(posInfo4.x*Screen.width , (posInfo4.y + offsetInfo*1f)*Screen.height, posInfo4.width*Screen.width, posInfo4.height*Screen.height), songSelected[(Difficulty)actualySelected].stops.Count + " Stops", "infosong");
		
			//Personnal best					
			GUI.Label(new Rect(posInfo5.x*Screen.width , (posInfo5.y + offsetInfo*0f)*Screen.height, posInfo5.width*Screen.width, posInfo5.height*Screen.height), score == -1 ? "Never scored" : "Best : " + score.ToString("0.00") + "%" + (isScoreFail ? " (Fail)" : "") , "infosong");
			//Friend best :					    
			GUI.Label(new Rect(posInfo5.x*Screen.width , (posInfo5.y + offsetInfo*1f)*Screen.height, posInfo5.width*Screen.width, posInfo5.height*Screen.height), bestfriendscore == -1 ? "No friend scored" : "Record : " + bestfriendscore.ToString("0.00") + "%", "infosong");
			//Friend name :					    
			GUI.Label(new Rect(posInfo5.x*Screen.width , (posInfo5.y + offsetInfo*2f)*Screen.height, posInfo5.width*Screen.width, posInfo5.height*Screen.height), bestnamefriendscore == "-" ? "" : "Record owner : " + bestnamefriendscore, "infosong");
		
			if(score != -1 && score < 96f){
				if(!isScoreFail){
					GUI.DrawTexture(new Rect(posNote.x*Screen.width, posNote.y*Screen.height, posNote.width*Screen.width, posNote.height*Screen.height), 
						tex[DataManager.Instance.giveNoteOfScore((float)score).Split(';')[1]]);
			
				}else{
					GUI.DrawTexture(new Rect(posSpecialNote.x*Screen.width, posSpecialNote.y*Screen.height, posSpecialNote.width*Screen.width, posSpecialNote.height*Screen.height), 
						tex["Failed"]);
				}
			}
			
		}
		#endregion
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		#region optionPlayGUI
		//Option/jouer
		if(songSelected != null && !movinSong && !SongMode){
		
			
		
			
			//Jouer
			if(GUI.Button(new Rect(Jouer.x*Screen.width, Jouer.y*Screen.height, Jouer.width*Screen.width, Jouer.height*Screen.height), "Play", "labelGo") && !movinOption && !movinNormal && speedmodok && rateok && animok){
				
				DataManager.Instance.songSelected =  songSelected[actualySelected];
				DataManager.Instance.difficultySelected = actualySelected;
				DataManager.Instance.speedmodSelected = speedmodSelected;
				DataManager.Instance.rateSelected = rateSelected;
				DataManager.Instance.skinSelected = skinSelected;
				DataManager.Instance.scoreJudgeSelected = scoreJudgeSelected;
				DataManager.Instance.hitJudgeSelected = hitJudgeSelected;
				DataManager.Instance.lifeJudgeSelected = lifeJudgeSelected;
				DataManager.Instance.raceSelected = raceSelected;
				DataManager.Instance.displaySelected = displaySelected;
				DataManager.Instance.deathSelected = deathSelected;
				DataManager.Instance.packSelected = !LoadManager.Instance.isAllowedToSearch(search) ? packs.ElementAt(numberPack).Key : "";
				DataManager.Instance.mousePosition = !LoadManager.Instance.isAllowedToSearch(search) ? startnumber : -1;
				
				///Save prefs
				ProfileManager.Instance.currentProfile.lastSpeedmodUsed = speedmodstring;
				ProfileManager.Instance.currentProfile.lastBPM = bpmstring;
				ProfileManager.Instance.currentProfile.inBPMMode = DataManager.Instance.BPMEntryMode;
					
				PSDiff[(int)actualySelected].gameObject.active = false;
				for(int i=0;i<RayDiff.Count;i++){
					RayDiff[i].active = false;	
				}
				var ecartjump = 0;
				for(int i=0; i<(int)actualySelected; i++){
					if(diffNumber[i] != 0){
						ecartjump++;
					}
				}
				departSongDiff = new Vector2(posDifficulty.x + offsetX[(int)actualySelected], posDifficulty.y + ecartDiff*ecartjump);
				moveSongDiff = new Vector2(posDifficulty.x+ offsetX[(int)actualySelected], posDifficulty.y + ecartDiff*ecartjump);
				graph.enabled = false;
				for(int i=0;i<diffSelected.Count;i++){
					diffSelected.ElementAt(i).Value.transform.Translate(0f, -200f, 0f);
				}
				movinSong = true;
				OptionMode = false;
				for(int i=0;i<optionSeparator.Length; i++){
					optionSeparator[i].enabled = false;
				}
				Line1.Stop ();
				Fond1.gameObject.active = false;
				Explode1.Play();
				Explode2.Play();
				Explode3.Play();
				songClip.Stop ();
				songClip.clip = launchSong;
				songClip.loop = false;
				songClip.Play();
			}
		
			//Option
			if(GUI.Button(new Rect(Option.x*Screen.width, Option.y*Screen.height, Option.width*Screen.width, Option.height*Screen.height), textButton, "labelGo") && !movinOption && !movinNormal && animok){
				if(textButton == "Option"){
					PSDiff[(int)actualySelected].gameObject.active = false;
					for(int i=0;i<RayDiff.Count;i++){
						RayDiff[i].active = false;	
					}
					var ecartjump = 0;
					for(int i=0; i<(int)actualySelected; i++){
						if(diffNumber[i] != 0){
							ecartjump++;
						}
					}
					departOptionDiff = new Vector2(posDifficulty.x + offsetX[(int)actualySelected], posDifficulty.y + ecartDiff*ecartjump);
					moveOptionDiff = new Vector2(posDifficulty.x+ offsetX[(int)actualySelected], posDifficulty.y + ecartDiff*ecartjump);
					graph.enabled = false;
					movinOption = true;
					OptionMode = true;
					textButton = "Back";
					
				}else{
					StartCoroutine(endOptionFade());
				}
				animok = false;
			}
			
		}
		
		//During move to option
		if(OptionMode || movinNormal || movinSong || SongMode){
			var realx = posDifficulty.x*Screen.width;
			var realy = posDifficulty.y*Screen.height;
			var realwidth = posDifficulty.width*Screen.width;
			var realheight = posDifficulty.height*Screen.height;
			var diffx = posNumberDiff.x*Screen.width;
			var diffy = posNumberDiff.y*Screen.height;
			var diffwidth = posNumberDiff.width*Screen.width;
			var diffheight = posNumberDiff.height*Screen.height;
			var theRealEcart = ecartDiff*Screen.height;
			var ecartjump = 0;
			for(int i=0; i<(int)actualySelected; i++){
				if(diffNumber[i] != 0){
					ecartjump++;
				}
			}
			GUI.DrawTexture(new Rect(realx + offsetX[(int)actualySelected]*Screen.width + offsetXDiff*Screen.width, realy + theRealEcart*ecartjump + offsetYDiff*Screen.height, realwidth, realheight), tex[(actualySelected).ToString()]);
			GUI.color = DataManager.Instance.diffColor[(int)actualySelected];
			GUI.Label(new Rect(diffx + offsetXDiff*Screen.width, diffy + theRealEcart*ecartjump + offsetYDiff*Screen.height, diffwidth, diffheight), songSelected[actualySelected].level.ToString(), "numberdiff");
					
		}
		
		
		
		#endregion
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		
		
		#region OptionMode
		
		//Option
		if((OptionMode || movinNormal) && !movinOption){
			for(int i=0;i<stateLoading.Length;i++){
				if(stateLoading[i]){
					GUI.DrawTexture(new Rect(posOptionTitle.x*Screen.width, (posOptionTitle.y + offsetYOption*i)*Screen.height, posOptionTitle.width*Screen.width, posOptionTitle.height*Screen.height), tex["Option" + (i+1)]);
					switch(i){
						case 0:
							//speedmod offsetXDisplayBPMSwitch
							if(!DataManager.Instance.BPMEntryMode){
								speedmodstring = GUI.TextArea (new Rect(posItem[0].x*Screen.width, posItem[0].y*Screen.height, posItem[0].width*Screen.width, posItem[0].height*Screen.height), speedmodstring.Trim(), 5);
							}else{
								bpmstring = GUI.TextArea (new Rect(posItem[0].x*Screen.width, posItem[0].y*Screen.height, posItem[0].width*Screen.width, posItem[0].height*Screen.height), bpmstring.Trim(), 5);
								if(!String.IsNullOrEmpty(bpmstring)){
									double resultbpm;
									if(System.Double.TryParse(bpmstring, out resultbpm)){
										var bpmtotest = songSelected.First().Value.bpmToDisplay;
										if(bpmtotest.Contains("->")){
											speedmodstring = (System.Convert.ToDouble(resultbpm/System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode]))).ToString("0.00");
										}else{
											speedmodstring = (resultbpm/System.Convert.ToDouble(bpmtotest)).ToString("0.00");
										}
									}else{
										speedmodstring = "?";
									}
								}else{
									speedmodstring = "";
								}
							}
							if(!String.IsNullOrEmpty(speedmodstring)){
								double result;
								if(System.Double.TryParse(speedmodstring, out result)){
									if(result >= (double)0.25 && result <= (double)15){
										speedmodSelected = (float)result;
										speedmodok = true;
										var bpmdisplaying = songSelected.First().Value.bpmToDisplay;
										if(bpmdisplaying.Contains("->")){
											if(!DataManager.Instance.BPMEntryMode) bpmstring = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode])*speedmodSelected).ToString("0");
											bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[0])*speedmodSelected*(1f + (rateSelected/100f))).ToString("0") + "->" + (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[1])*speedmodSelected*(1f + (rateSelected/100f))).ToString("0");
										}else{
											if(!DataManager.Instance.BPMEntryMode) bpmstring = (System.Convert.ToDouble(bpmdisplaying)*speedmodSelected).ToString("0");
											bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying)*speedmodSelected*(1f + (rateSelected/100f))).ToString("0");
											
										}
										
										GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod : x" + speedmodSelected.ToString("0.00") + " (" + bpmdisplaying + " BPM)");
									}else{
										GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
										GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod must be between x0.25 and x15");
										GUI.color = new Color(1f, 1f, 1f, 1f);
										speedmodok = false;
										if(!DataManager.Instance.BPMEntryMode) bpmstring = "1";
									}
								}else{
									GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
									GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod is not a valid value");
									GUI.color = new Color(1f, 1f, 1f, 1f);
									speedmodok = false;
									if(!DataManager.Instance.BPMEntryMode) bpmstring = "?";
								}
							}else{
								GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
								GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Empty value");
								GUI.color = new Color(1f, 1f, 1f, 1f);
								speedmodok = false;
								if(!DataManager.Instance.BPMEntryMode) bpmstring = "";
							}
							
							if(GUI.Button(new Rect((posItem[0].x + offsetXDisplayBPMSwitch)*Screen.width, posItem[0].y*Screen.height, (posItem[0].width + offsetWidthDisplayBPMSV)*Screen.width, posItem[0].height*Screen.height), DataManager.Instance.BPMEntryMode ? "By multip." : "by BPM", "labelGoLittle")){
								DataManager.Instance.BPMEntryMode = !DataManager.Instance.BPMEntryMode;
							}
							if(DataManager.Instance.BPMEntryMode && songSelected.First().Value.bpmToDisplay.Contains("->")){
								if(GUI.Button(new Rect((posItem[0].x + offsetXDisplayBPMValue)*Screen.width, posItem[0].y*Screen.height, (posItem[0].width + offsetWidthDisplayBPMSV)*Screen.width, posItem[0].height*Screen.height), 	  DataManager.Instance.BPMChoiceMode == 0 ? "For higher" : "For lower", "labelGoLittle")){
									DataManager.Instance.BPMChoiceMode++;
									if(DataManager.Instance.BPMChoiceMode == 2) DataManager.Instance.BPMChoiceMode = 0;
								}
							}
							
						break;
						case 1:
							//Rate
							ratestring = GUI.TextArea (new Rect(posItem[1].x*Screen.width, posItem[1].y*Screen.height, posItem[1].width*Screen.width, posItem[1].height*Screen.height), ratestring.Trim(), 4);
							if(!String.IsNullOrEmpty(ratestring)){
								int rateresult = 0;
								if(System.Int32.TryParse(ratestring, out rateresult)){
									if(rateresult >= -90 && rateresult <= 100){
										rateSelected = rateresult;
										rateok = true;
										GUI.Label(new Rect((posItemLabel[1].x + offsetSpeedRateX)*Screen.width, posItemLabel[1].y*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), "Rate : " + rateSelected.ToString("00") + "%");
									}else{
										GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
										GUI.Label(new Rect((posItemLabel[1].x + offsetSpeedRateX)*Screen.width, posItemLabel[1].y*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), "Rate must be between -90% and +100%");
										GUI.color = new Color(1f, 1f, 1f, 1f);
										rateok = false;
									}
								}else{
									GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
									GUI.Label(new Rect((posItemLabel[1].x + offsetSpeedRateX)*Screen.width, posItemLabel[1].y*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), "Rate is not a valid value");
									GUI.color = new Color(1f, 1f, 1f, 1f);
									rateok = false;
								}
							}else{
								GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
								GUI.Label(new Rect((posItemLabel[1].x + offsetSpeedRateX)*Screen.width, posItemLabel[1].y*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), "Empty Value");
								GUI.color = new Color(1f, 1f, 1f, 1f);
								rateok = false;
							}
							
						break;
						case 2:
							//skin
							GUI.color = new Color(1f, 1f, 1f, alphaText[2]);
							GUI.Label(new Rect(posItemLabel[2].x*Screen.width, (posItemLabel[2].y - offsetFading[2])*Screen.height, posItemLabel[2].width*Screen.width, posItemLabel[2].height*Screen.height), DataManager.Instance.aSkin[skinSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[2]);
							if(isFading[2]) GUI.Label(new Rect(posItemLabel[2].x*Screen.width, (posItemLabel[2].y + offsetPreviousFading[2])*Screen.height, posItemLabel[2].width*Screen.width, posItemLabel[2].height*Screen.height), DataManager.Instance.aSkin[previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(GUI.Button(new Rect((posItem[2].x - ecartForBack)*Screen.width, posItem[2].y*Screen.height, posItem[2].width*Screen.width, posItem[2].height*Screen.height), "", "LBackward")){
								previousSelected = skinSelected;
								if(skinSelected == 0){
									skinSelected = DataManager.Instance.aSkin.Length - 1;
								}else{
									skinSelected--;
								}
								StartCoroutine(OptionAnim(2, true));
							}
							if(GUI.Button(new Rect((posItem[2].x + ecartForBack)*Screen.width, posItem[2].y*Screen.height, posItem[2].width*Screen.width, posItem[2].height*Screen.height), "", "LForward")){
								previousSelected = skinSelected;
								if(skinSelected == DataManager.Instance.aSkin.Length - 1){
									skinSelected = 0;
								}else{
									skinSelected++;
								}
								StartCoroutine(OptionAnim(2, false));
							}
						break;
						case 3:
							//hit
							GUI.color = new Color(1f, 1f, 1f, alphaText[3]);
							GUI.Label(new Rect(posItemLabel[3].x*Screen.width, (posItemLabel[3].y - offsetFading[3])*Screen.height, posItemLabel[3].width*Screen.width, posItemLabel[3].height*Screen.height), DataManager.Instance.dicHitJudge[hitJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[3]);
							if(isFading[3]) GUI.Label(new Rect(posItemLabel[3].x*Screen.width, (posItemLabel[3].y + offsetPreviousFading[3])*Screen.height, posItemLabel[3].width*Screen.width, posItemLabel[3].height*Screen.height), DataManager.Instance.dicHitJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(hitJudgeSelected > Judge.BEGINNER){
								if(GUI.Button(new Rect((posItem[3].x - ecartForBack)*Screen.width, posItem[3].y*Screen.height, posItem[3].width*Screen.width, posItem[3].height*Screen.height), "", "LBackward")){
									previousSelected = (int)hitJudgeSelected;
									hitJudgeSelected--;
									StartCoroutine(OptionAnim(3, true));
								}
							}
							if(hitJudgeSelected < Judge.EXPERT){
								if(GUI.Button(new Rect((posItem[3].x + ecartForBack)*Screen.width, posItem[3].y*Screen.height, posItem[3].width*Screen.width, posItem[3].height*Screen.height), "", "LForward")){
									previousSelected = (int)hitJudgeSelected;
									hitJudgeSelected++;
									StartCoroutine(OptionAnim(3, false));
								}
							}
						break;
						case 4:
							//score
							GUI.color = new Color(1f, 1f, 1f, alphaText[4]);
							GUI.Label(new Rect(posItemLabel[4].x*Screen.width, (posItemLabel[4].y - offsetFading[4])*Screen.height, posItemLabel[4].width*Screen.width, posItemLabel[4].height*Screen.height), DataManager.Instance.dicScoreJudge[scoreJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[4]);
							if(isFading[4]) GUI.Label(new Rect(posItemLabel[4].x*Screen.width, (posItemLabel[4].y + offsetPreviousFading[4])*Screen.height, posItemLabel[4].width*Screen.width, posItemLabel[4].height*Screen.height), DataManager.Instance.dicScoreJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(scoreJudgeSelected > Judge.BEGINNER){
								if(GUI.Button(new Rect((posItem[4].x - ecartForBack)*Screen.width, posItem[4].y*Screen.height, posItem[4].width*Screen.width, posItem[4].height*Screen.height), "", "LBackward")){
									previousSelected = (int)scoreJudgeSelected;
									scoreJudgeSelected--;
									StartCoroutine(OptionAnim(4, true));
								}
							}
							if(scoreJudgeSelected < Judge.EXPERT){
								if(GUI.Button(new Rect((posItem[4].x + ecartForBack)*Screen.width, posItem[4].y*Screen.height, posItem[4].width*Screen.width, posItem[4].height*Screen.height), "", "LForward")){
									previousSelected = (int)scoreJudgeSelected;
									scoreJudgeSelected++;
									StartCoroutine(OptionAnim(4, false));
								}
							}
						break;
						case 5:
							//life judge
							GUI.color = new Color(1f, 1f, 1f, alphaText[5]);
							GUI.Label(new Rect(posItemLabel[5].x*Screen.width, (posItemLabel[5].y - offsetFading[5])*Screen.height, posItemLabel[5].width*Screen.width, posItemLabel[5].height*Screen.height), DataManager.Instance.dicLifeJudge[lifeJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[5]);
							if(isFading[5]) GUI.Label(new Rect(posItemLabel[5].x*Screen.width, (posItemLabel[5].y + offsetPreviousFading[5])*Screen.height, posItemLabel[5].width*Screen.width, posItemLabel[5].height*Screen.height), DataManager.Instance.dicLifeJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(lifeJudgeSelected > Judge.BEGINNER){
								if(GUI.Button(new Rect((posItem[5].x - ecartForBack)*Screen.width, posItem[5].y*Screen.height, posItem[5].width*Screen.width, posItem[5].height*Screen.height), "", "LBackward")){
									previousSelected = (int)lifeJudgeSelected;
									lifeJudgeSelected--;
									StartCoroutine(OptionAnim(5, true));
								}
							}
							if(lifeJudgeSelected < Judge.EXPERT){
								if(GUI.Button(new Rect((posItem[5].x + ecartForBack)*Screen.width, posItem[5].y*Screen.height, posItem[5].width*Screen.width, posItem[5].height*Screen.height), "", "LForward")){
									previousSelected = (int)lifeJudgeSelected;
									lifeJudgeSelected++;
									StartCoroutine(OptionAnim(5, false));
								}
							}
						break;
						case 6:
							//display
							for(int j=0; j<DataManager.Instance.aDisplay.Length; j++){
								if(displaySelected[j]){
									GUI.color = new Color(1f, 1f, 1f, alphaDisplay[j]);
									GUI.DrawTexture(new Rect((posItem[6].x - borderXDisplay + offsetXDisplay*j + selectedImage.x)*Screen.width, (posItem[6].y + selectedImage.y)*Screen.height, selectedImage.width*Screen.width, selectedImage.height*Screen.height), tex["bouton"]);
									GUI.color = new Color(1f, 1f, 1f, 1f);
								}
							
								if(GUI.Button(new Rect((posItem[6].x - borderXDisplay + offsetXDisplay*j)*Screen.width, posItem[6].y*Screen.height, posItem[6].width*Screen.width, posItem[6].height*Screen.height), DataManager.Instance.aDisplay[j], "labelNormal") && !isFadingDisplay[j]){
									displaySelected[j] = !displaySelected[j];
									StartCoroutine(OptionAnimDisplay(j, !displaySelected[j]));
								}
							}
						break;
						case 7:
							//race
							GUI.color = new Color(1f, 1f, 1f, alphaText[7]);
							GUI.Label(new Rect(posItemLabel[7].x*Screen.width, (posItemLabel[7].y - offsetFading[7])*Screen.height, posItemLabel[7].width*Screen.width, posItemLabel[7].height*Screen.height), DataManager.Instance.aRace[raceSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[7]);
							if(isFading[7]) GUI.Label(new Rect(posItemLabel[7].x*Screen.width, (posItemLabel[7].y + offsetPreviousFading[7])*Screen.height, posItemLabel[7].width*Screen.width, posItemLabel[7].height*Screen.height), DataManager.Instance.aRace[previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(GUI.Button(new Rect((posItem[7].x - ecartForBack)*Screen.width, posItem[7].y*Screen.height, posItem[7].width*Screen.width, posItem[7].height*Screen.height), "", "LBackward")){
								previousSelected = raceSelected;
								if(raceSelected == 0){
									raceSelected = DataManager.Instance.aRace.Length - 1;
								}else{
									raceSelected--;
								}
								StartCoroutine(OptionAnim(7, true));
							}
							if(GUI.Button(new Rect((posItem[7].x + ecartForBack)*Screen.width, posItem[7].y*Screen.height, posItem[7].width*Screen.width, posItem[7].height*Screen.height), "", "LForward")){
								previousSelected = raceSelected;
								if(raceSelected == DataManager.Instance.aRace.Length - 1){
									raceSelected = 0;
								}else{
									raceSelected++;
								}
								StartCoroutine(OptionAnim(7, false));
							}
						break;
						case 8:
							//death
							GUI.color = new Color(1f, 1f, 1f, alphaText[8]);
							GUI.Label(new Rect(posItemLabel[8].x*Screen.width, (posItemLabel[8].y - offsetFading[8])*Screen.height, posItemLabel[8].width*Screen.width, posItemLabel[8].height*Screen.height), DataManager.Instance.aDeath[deathSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[8]);
							if(isFading[8]) GUI.Label(new Rect(posItemLabel[8].x*Screen.width, (posItemLabel[8].y + offsetPreviousFading[8])*Screen.height, posItemLabel[8].width*Screen.width, posItemLabel[8].height*Screen.height), DataManager.Instance.aDeath[previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(GUI.Button(new Rect((posItem[8].x - ecartForBack)*Screen.width, posItem[8].y*Screen.height, posItem[8].width*Screen.width, posItem[8].height*Screen.height), "", "LBackward")){
								previousSelected = deathSelected;
								if(deathSelected == 0){
									deathSelected = DataManager.Instance.aDeath.Length - 1; 
								}else{
									deathSelected--;
								}
								StartCoroutine(OptionAnim(8, true));
							}
							if(GUI.Button(new Rect((posItem[8].x + ecartForBack)*Screen.width, posItem[8].y*Screen.height, posItem[8].width*Screen.width, posItem[8].height*Screen.height), "", "LForward")){
								previousSelected = deathSelected;
								if(deathSelected == DataManager.Instance.aDeath.Length - 1){
									deathSelected = 0;
								}else{
									deathSelected++;
								}
								StartCoroutine(OptionAnim(8, false));
							}
						break;
					
					}
				
				}
			}
		
		}
		
		#endregion
	
		#region SongMode
		
		if(SongMode){

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
		//Move to option
		if(movinOption){
			foreach(var el in packs){
				el.Value.transform.position = Vector3.Lerp(el.Value.transform.position, new Vector3(el.Value.transform.position.x, 20f, el.Value.transform.position.z), Time.deltaTime/speedMoveOption);
			}	
			foreach(var eld in diffSelected){
				if(eld.Key != actualySelected) eld.Value.transform.position = Vector3.Lerp(eld.Value.transform.position, new Vector3(5f, eld.Value.transform.position.y, eld.Value.transform.position.z), Time.deltaTime/speedMoveOption);
			}
			foreach(var m in medals){
				m.transform.position = Vector3.Lerp(m.transform.position, new Vector3(30f, m.transform.position.y, m.transform.position.z),Time.deltaTime/speedMoveOption); 
			}
			camerapack.transform.position = Vector3.Lerp(camerapack.transform.position, new Vector3(25f, camerapack.transform.position.y, camerapack.transform.position.z), Time.deltaTime/speedMoveOption);
			basePositionSeparator = Mathf.Lerp(basePositionSeparator, -50f, Time.deltaTime/speedMoveOption);
			separator.SetPosition(0, new Vector3(basePositionSeparator, -20f, 20f));
			separator.SetPosition(1, new Vector3(basePositionSeparator, 12f, 20f));
			moveOptionDiff.x = Mathf.Lerp(moveOptionDiff.x, arriveOptionDiff.x, Time.deltaTime/speedMoveOption);
			offsetXDiff = moveOptionDiff.x - departOptionDiff.x;
			moveOptionDiff.y = Mathf.Lerp(moveOptionDiff.y, arriveOptionDiff.y, Time.deltaTime/speedMoveOption);
			offsetYDiff = moveOptionDiff.y - departOptionDiff.y;
			diffSelected[actualySelected].transform.position = Vector3.Lerp(diffSelected[actualySelected].transform.position, new Vector3(-1.2f, 0.75f, 2f), Time.deltaTime/speedMoveOption);
			plane.transform.position = Vector3.Lerp(plane.transform.position, new Vector3(0f, 12f, 20f), Time.deltaTime/speedMoveOption);
			totalAlpha = Mathf.Lerp(totalAlpha, 1f, Time.deltaTime/speedMoveOption);
			
			if(Mathf.Abs(camerapack.transform.position.x - 25f) <= limiteMoveOption){
				
				foreach(var el in packs){
					el.Value.transform.position = new Vector3(el.Value.transform.position.x, 20f, el.Value.transform.position.z);
				}	
				foreach(var eld in diffSelected){
					if(eld.Key != actualySelected) eld.Value.transform.position = new Vector3(5f, eld.Value.transform.position.y, eld.Value.transform.position.z);
				}
				foreach(var m in medals){
					m.transform.position = new Vector3(30f, m.transform.position.y, m.transform.position.z); 
				}
				camerapack.transform.position = new Vector3(25f, camerapack.transform.position.y, camerapack.transform.position.z);
				totalAlpha = 1f;
				separator.SetPosition(0, new Vector3(-50f, -20f, 20f));
				separator.SetPosition(1, new Vector3(-50f, 12f, 20f));
				offsetXDiff = arriveOptionDiff.x - departOptionDiff.x;
				offsetYDiff = arriveOptionDiff.y - departOptionDiff.y;
				diffSelected[actualySelected].transform.position = new Vector3(-1.2f, 0.75f, 2f);
				plane.transform.position = new Vector3(0f, 12f, 20f);
				movinOption = false;
				StartCoroutine(startOptionFade());
			}
		}
		
		
		
		if(movinSong && !SongMode){

				foreach(var el in packs){
					el.Value.transform.position = Vector3.Lerp(el.Value.transform.position, new Vector3(el.Value.transform.position.x, 20f, el.Value.transform.position.z), Time.deltaTime/speedMoveSong);
				}	
				foreach(var eld in diffSelected){
					if(eld.Key != actualySelected) eld.Value.transform.position = Vector3.Lerp(eld.Value.transform.position, new Vector3(5f, eld.Value.transform.position.y, eld.Value.transform.position.z), Time.deltaTime/speedMoveSong);
				}
				foreach(var m in medals){
					m.transform.position = Vector3.Lerp(m.transform.position, new Vector3(30f, m.transform.position.y, m.transform.position.z),Time.deltaTime/speedMoveSong); 
				}
				camerapack.transform.position = Vector3.Lerp(camerapack.transform.position, new Vector3(25f, camerapack.transform.position.y, camerapack.transform.position.z), Time.deltaTime/speedMoveSong);
				basePositionSeparator = Mathf.Lerp(basePositionSeparator, -50f, Time.deltaTime/speedMoveSong);
				separator.SetPosition(0, new Vector3(basePositionSeparator, -20f, 20f));
				separator.SetPosition(1, new Vector3(basePositionSeparator, 12f, 20f));
				moveSongDiff.x = Mathf.Lerp(moveSongDiff.x, arriveSongDiff.x, Time.deltaTime/speedMoveSong);
				offsetXDiff = moveSongDiff.x - departSongDiff.x;
				moveSongDiff.y = Mathf.Lerp(moveSongDiff.y, arriveSongDiff.y, Time.deltaTime/speedMoveSong);
				offsetYDiff = moveSongDiff.y - departSongDiff.y;
				plane.transform.position = Vector3.Lerp(plane.transform.position, new Vector3(0f, 10f, 20f), Time.deltaTime/speedMoveSong);
				plane.transform.localScale = Vector3.Lerp(plane.transform.localScale, new Vector3(3f, 2f, 0.8f), Time.deltaTime/speedMoveSong);
				totalAlpha = Mathf.Lerp(totalAlpha, 1f, Time.deltaTime/speedMoveSong*2f);
				if(Mathf.Abs(plane.transform.position.y - 10f) <= limiteMoveOption){
					
					foreach(var el in packs){
						el.Value.transform.position = new Vector3(el.Value.transform.position.x, 20f, el.Value.transform.position.z);
					}	
					foreach(var eld in diffSelected){
						if(eld.Key != actualySelected) eld.Value.transform.position = new Vector3(5f, eld.Value.transform.position.y, eld.Value.transform.position.z);
					}
					
					camerapack.transform.position = new Vector3(25f, camerapack.transform.position.y, camerapack.transform.position.z);
					totalAlpha = 1f;
					separator.SetPosition(0, new Vector3(-50f, -20f, 20f));
					separator.SetPosition(1, new Vector3(-50f, 12f, 20f));
					offsetXDiff = arriveSongDiff.x - departSongDiff.x;
					offsetYDiff = arriveSongDiff.y - departSongDiff.y;
					plane.transform.position = new Vector3(0f, 10f, 20f);
					plane.transform.localScale = new Vector3(3f, 2f, 0.8f);
					SongMode = true;
					time = 0f;
				}
			
			
		}
		
		if(SongMode){
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
					songClip.volume -= Time.deltaTime/speedAlphaBlack;	
					
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
		
		
		//Move to normal
		if(movinNormal){
			foreach(var el in packs){
				el.Value.transform.position = Vector3.Lerp(el.Value.transform.position, new Vector3(el.Value.transform.position.x, 13f, el.Value.transform.position.z), Time.deltaTime/speedMoveOption);
			}	
			foreach(var eld in diffSelected){
				if(eld.Key != actualySelected) eld.Value.transform.position = Vector3.Lerp(eld.Value.transform.position, new Vector3(0.45f, eld.Value.transform.position.y, eld.Value.transform.position.z), Time.deltaTime/speedMoveOption);
			}
			foreach(var m in medals){
				m.transform.position = Vector3.Lerp(m.transform.position, new Vector3(22f, m.transform.position.y, m.transform.position.z),Time.deltaTime/speedMoveOption); 
			}
			camerapack.transform.position = Vector3.Lerp(camerapack.transform.position, new Vector3(0f, camerapack.transform.position.y, camerapack.transform.position.z), Time.deltaTime/speedMoveOption);
			basePositionSeparator = Mathf.Lerp(basePositionSeparator, -5.5f, Time.deltaTime/speedMoveOption);
			separator.SetPosition(0, new Vector3(basePositionSeparator, -20f, 20f));
			separator.SetPosition(1, new Vector3(basePositionSeparator, 12f, 20f));
			moveOptionDiff.x = Mathf.Lerp(moveOptionDiff.x, departOptionDiff.x, Time.deltaTime/speedMoveOption);
			offsetXDiff = moveOptionDiff.x - departOptionDiff.x;
			moveOptionDiff.y = Mathf.Lerp(moveOptionDiff.y, departOptionDiff.y, Time.deltaTime/speedMoveOption);
			offsetYDiff = moveOptionDiff.y - departOptionDiff.y;
			totalAlpha = Mathf.Lerp(totalAlpha, 0f, Time.deltaTime/speedMoveOption);
			var ecartjump = 0;
			for(int i=0; i<(int)actualySelected; i++){
				if(diffNumber[i] != 0){
					ecartjump++;
				}
			}
			diffSelected[actualySelected].transform.position = Vector3.Lerp(diffSelected[actualySelected].transform.position, new Vector3(0.45f, DataManager.Instance.posYDiff[ecartjump], 2f), Time.deltaTime/speedMoveOption);
			plane.transform.position = Vector3.Lerp(plane.transform.position, new Vector3(10f, 7f, 20f), Time.deltaTime/speedMoveOption);
			
			if(camerapack.transform.position.x <= limiteMoveOption){
				
				
				foreach(var el in packs){
					el.Value.transform.position = new Vector3(el.Value.transform.position.x, 13f, el.Value.transform.position.z);
				}	
				foreach(var eld in diffSelected){
					if(eld.Key != actualySelected) eld.Value.transform.position = new Vector3(0.45f, eld.Value.transform.position.y, eld.Value.transform.position.z);
				}
				foreach(var m in medals){
					m.transform.position = new Vector3(22f, m.transform.position.y, m.transform.position.z); 
				}
				camerapack.transform.position = new Vector3(0f, camerapack.transform.position.y, camerapack.transform.position.z);
				totalAlpha = 0f;
				separator.SetPosition(0, new Vector3(-5.5f, -20f, 20f));
				separator.SetPosition(1, new Vector3(-5.5f, 12f, 20f));
				offsetXDiff = 0f;
				offsetYDiff = 0f;
				diffSelected[actualySelected].transform.position = new Vector3(0.45f, DataManager.Instance.posYDiff[ecartjump], 2f);
				plane.transform.position = new Vector3(10f, 7f, 20f);
				movinNormal = false;
				PSDiff[(int)actualySelected].gameObject.active = true;
				for(int i=0;i<RayDiff.Count;i++){
					RayDiff[i].active = true;	
				}
				graph.enabled = true;
				textButton = "Option";
			}
		}
		#endregion
		
		#region packUpdate
		//Move forward pack
		if(movinForward){
			foreach(var el in packs){
				el.Value.transform.position = Vector3.Lerp(el.Value.transform.position, new Vector3(cubesPos[el.Value]- 10f, el.Value.transform.position.y, el.Value.transform.position.z), Time.deltaTime/speedMove);
			}
			decalFade = Mathf.Lerp(decalFade, -x10, Time.deltaTime/speedMove);
			decalFadeM = Mathf.Lerp(decalFadeM, xm10, Time.deltaTime/speedMove);
			
			if(Mathf.Abs(cubesPos.First().Key.transform.position.x - (cubesPos.First().Value - 10f)) <= limite){
				movinForward = false;
				decalFade = 0f;
				decalFadeM = 0f;
				fadeAlpha = 0.5f;
				decreaseCubeF();
				fadeAlpha = 0f;
				numberPack = NextInt(numberPack, 1);
				nextnumberPack = numberPack;
				for(int i=0;i<cubesPos.Count();i++){
					cubesPos.ElementAt(i).Key.transform.position = new Vector3(cubesPos.ElementAt(i).Value - 10f, cubesPos.ElementAt(i).Key.transform.position.y, cubesPos.ElementAt(i).Key.transform.position.z);
					cubesPos[cubesPos.ElementAt(i).Key] = cubesPos.ElementAt(i).Key.transform.position.x;
				}
				cubesPos.ElementAt(NextInt(numberPack, 2)).Key.transform.position = new Vector3(20f, 13f, 20f);
				cubesPos[cubesPos.ElementAt(NextInt(numberPack, 2)).Key] = cubesPos.ElementAt(NextInt(numberPack, 2)).Key.transform.position.x;
			}else{
				decreaseCubeF();
			}	
		}
		
		//Move backward pack
		if(movinBackward){
			foreach(var elb in packs){
				elb.Value.transform.position = Vector3.Lerp(elb.Value.transform.position, new Vector3(cubesPos[elb.Value] + 10f, elb.Value.transform.position.y, elb.Value.transform.position.z), Time.deltaTime/speedMove);
			}
			decalFade = Mathf.Lerp(decalFade, x10, Time.deltaTime/speedMove);
			decalFadeM = Mathf.Lerp(decalFadeM, -xm10, Time.deltaTime/speedMove);
			if(Mathf.Abs(cubesPos.First().Key.transform.position.x - (cubesPos.First().Value + 10f)) <= limite){
				movinBackward = false;	
				decalFade = 0f;
				decalFadeM = 0f;
				fadeAlpha = 0.5f;
				decreaseCubeB();
				fadeAlpha = 0f;
				numberPack = PrevInt(numberPack, 1);
				nextnumberPack = numberPack;
				
				for(int i=0;i<cubesPos.Count();i++){
					cubesPos.ElementAt(i).Key.transform.position = new Vector3(cubesPos.ElementAt(i).Value + 10f, cubesPos.ElementAt(i).Key.transform.position.y, cubesPos.ElementAt(i).Key.transform.position.z);
					cubesPos[cubesPos.ElementAt(i).Key] = cubesPos.ElementAt(i).Key.transform.position.x;
				}
				cubesPos.ElementAt(PrevInt(numberPack, 2)).Key.transform.position = new Vector3(-20f, 13f, 20f);
				cubesPos[cubesPos.ElementAt(PrevInt(numberPack, 2)).Key] = cubesPos.ElementAt(PrevInt(numberPack, 2)).Key.transform.position.x;
			}else{
				decreaseCubeB();
			}
		}
		
		//move forward fast pack
		if(movinForwardFast){
			cubesPos.ElementAt(PrevInt(numberPack, 1)).Key.transform.position = new Vector3(20f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(PrevInt(numberPack, 1)).Key] = 20f;
			cubesPos.ElementAt(numberPack).Key.transform.position = new Vector3(20f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(numberPack).Key] = 20f;
			cubesPos.ElementAt(NextInt(numberPack, 1)).Key.transform.position = new Vector3(20f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(NextInt(numberPack, 1)).Key] = 20f;
			cubesPos.ElementAt(NextInt(numberPack, 2)).Key.transform.position = new Vector3(-10f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(NextInt(numberPack, 2)).Key] = -10f;
			cubesPos.ElementAt(NextInt(numberPack, 3)).Key.transform.position = new Vector3(0f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(NextInt(numberPack, 3)).Key] = 0f;
			cubesPos.ElementAt(NextInt(numberPack, 4)).Key.transform.position = new Vector3(10f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(NextInt(numberPack, 4)).Key] = 10f;
			
			fastDecreaseCubeF();
			
			numberPack = NextInt(numberPack, 3);
			nextnumberPack = numberPack;
			
			
			movinForwardFast = false;
			
		}
		
		//Move backward fast pack
		if(movinBackwardFast){
			cubesPos.ElementAt(NextInt(numberPack, 1)).Key.transform.position = new Vector3(-20f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(NextInt(numberPack, 1)).Key] = -20f;
			cubesPos.ElementAt(numberPack).Key.transform.position = new Vector3(-20f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(numberPack).Key] = -20f;
			cubesPos.ElementAt(PrevInt(numberPack, 1)).Key.transform.position = new Vector3(-20f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(PrevInt(numberPack, 1)).Key] = -20f;
			cubesPos.ElementAt(PrevInt(numberPack, 2)).Key.transform.position = new Vector3(10f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(PrevInt(numberPack, 2)).Key] = 10f;
			cubesPos.ElementAt(PrevInt(numberPack, 3)).Key.transform.position = new Vector3(0f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(PrevInt(numberPack, 3)).Key] = 0f;
			cubesPos.ElementAt(PrevInt(numberPack, 4)).Key.transform.position = new Vector3(-10f, 13f, 20f);
			cubesPos[cubesPos.ElementAt(PrevInt(numberPack, 4)).Key] = -10f;
			
			fastDecreaseCubeB();
			
			numberPack = PrevInt(numberPack, 3);
			nextnumberPack = numberPack;
			
			movinBackwardFast = false;
			
		}
		
		#endregion
		
		
		
		if(!movinNormal && !movinOption && !OptionMode){
			
			#region ListChanges
			var packOnRender = LoadManager.Instance.isAllowedToSearch(search) ? songList : LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key];
			var linkOnRender = LoadManager.Instance.isAllowedToSearch(search) ? customLinkCubeSong : LinkCubeSong;
			Dictionary<GameObject, string> songCubeOnRender = LoadManager.Instance.isAllowedToSearch(search) ? customSongCubePack : songCubePack;
			#endregion
			
			#region Raycast
			//Raycast songlist
			Ray ray = camerapack.ScreenPointToRay(Input.mousePosition);	
			RaycastHit hit;
				
			if(Physics.Raycast(ray, out hit))
			{
				var theGo = hit.transform.gameObject;
				if(theGo != null && theGo.tag == "ZoneText"){
					
					if(!locked){
						var papa = theGo.transform.parent;
						var thepart = papa.Find("Selection").gameObject;
						if(particleOnPlay != null && particleOnPlay != thepart && particleOnPlay.active) 
						{
							
							particleOnPlay.active = false;
						}
						if(songSelected == null || ((songSelected.First().Value.title + "/" + songSelected.First().Value.subtitle) != linkOnRender[papa.gameObject])){
							songSelected = packOnRender.FirstOrDefault(c => (c.Value.First().Value.title + "/" + c.Value.First().Value.subtitle) == linkOnRender[papa.gameObject]).Value;
							activeNumberDiff(songSelected);
							activeDiff(songSelected);
							PSDiff[(int)actualySelected].gameObject.active = false;
							activeDiffPS(songSelected);
							PSDiff[(int)actualySelected].gameObject.active = true;
							displayGraph(songSelected);
							verifyScore();
							graph.enabled = true;
							cubeSelected = papa.gameObject;
							alreadyRefresh = false;
							songClip.Stop();
							//if(time >= timeBeforeDisplay) plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[packs.ElementAt(nextnumberPack).Key];
							time = 0f;
							if(alphaBanner > 0) FadeOutBanner = true;
						}
						particleOnPlay = thepart;
						particleOnPlay.active = true;
					}
					
					if(Input.GetMouseButtonDown(0)){
						
						if(locked){
							var papa = theGo.transform.parent;
							var thepart = papa.Find("Selection").gameObject;
							if(particleOnPlay != null && particleOnPlay != thepart && particleOnPlay.active) 
							{
								particleOnPlay.active = false;
							}
							if(songSelected == null || ((songSelected.First().Value.title + "/" + songSelected.First().Value.subtitle) != linkOnRender[papa.gameObject])){
								songSelected = packOnRender.FirstOrDefault(c => (c.Value.First().Value.title + "/" + c.Value.First().Value.subtitle) == linkOnRender[papa.gameObject]).Value;
								activeNumberDiff(songSelected);
								activeDiff(songSelected);
								PSDiff[(int)actualySelected].gameObject.active = false;
								activeDiffPS(songSelected);
								PSDiff[(int)actualySelected].gameObject.active = true;
								displayGraph(songSelected);
								verifyScore();
								graph.enabled = true;
								cubeSelected = papa.gameObject;
								alreadyRefresh = false;
								songClip.Stop();
								if(alphaBanner > 0) FadeOutBanner = true;
								//if(time >= timeBeforeDisplay) plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[packs.ElementAt(nextnumberPack).Key];
								time = 0f;
							}
							particleOnPlay = thepart;
							particleOnPlay.active = true;
						}else{
							locked = true;
						}
					}
					
				}else if(particleOnPlay != null && particleOnPlay.active){
					
					if(!locked){
						cubeSelected = null;
						songSelected = null;
						FadeOutBanner = true;
						graph.enabled = false;
						songClip.Stop();
						PSDiff[(int)actualySelected].gameObject.active = false;
						desactiveDiff();
						particleOnPlay.active = false;
						foreach(var med in medals)
						{
							if(med.active) med.SetActiveRecursively(false);
						}
					}
				}
				
				
				
			}else if(particleOnPlay != null && particleOnPlay.active){
				if(!locked){
					cubeSelected = null;
					songSelected = null;
					FadeOutBanner = true;
					songClip.Stop();
					PSDiff[(int)actualySelected].gameObject.active = false;
					graph.enabled = false;
					desactiveDiff();
					particleOnPlay.active = false;
					foreach(var med in medals)
					{
						if(med.active) med.SetActiveRecursively(false);
					}
				}
			}
			
			//Raycast difficulty list
			Ray ray2 = cameradiff.ScreenPointToRay(Input.mousePosition);	
			RaycastHit hit2;
				
			if(Physics.Raycast(ray2, out hit2))
			{
				var theGo = hit2.transform.gameObject;
				if(theGo != null && theGo.tag == "ZoneDiff"){
					onHoverDifficulty = (Difficulty)int.Parse(theGo.name);
					if(Input.GetMouseButtonDown(0)){
						PSDiff[(int)actualySelected].gameObject.active = false;
						actualySelected = (Difficulty)int.Parse(theGo.name);
						trulySelected = (Difficulty)int.Parse(theGo.name);
						PSDiff[(int)actualySelected].gameObject.active = true;
						displayGraph(songSelected);
						verifyScore();
					}
				}else{
					onHoverDifficulty = Difficulty.NONE;
				}
			}else{
				onHoverDifficulty = Difficulty.NONE;	
			}
			
			#endregion
			
			#region input
			//Unlock
			if(Input.GetMouseButtonDown(1)){
				locked = false;
			}
			
			if(Input.GetKeyDown(KeyCode.Escape) && !movinSong && !SongMode && !fadedOut){
				if(OptionMode && animok){
					StartCoroutine(endOptionFade());
					animok = false;
				}else if(!OptionMode){
					fadedOut = true;
					GetComponent<FadeManager>().FadeIn("mainmenu");	
				}
			}
			//Scrollwheel
			if(Input.GetAxis("Mouse ScrollWheel") > 0 && startnumber > 0){
				startnumber -= DataManager.Instance.mouseMolSpeed;
				if(startnumber < 0) startnumber = 0;
				
			}else if(Input.GetAxis("Mouse ScrollWheel") < 0 && startnumber < (songCubeOnRender.Where(c => packs.ElementAt(nextnumberPack).Key == c.Value).Count() - numberToDisplay + 1)){
				var songcount = songCubeOnRender.Where(c => packs.ElementAt(nextnumberPack).Key == c.Value).Count() - numberToDisplay + 1;
				if(startnumber < songcount){
					startnumber += DataManager.Instance.mouseMolSpeed;
					if(startnumber > songcount) startnumber = songcount;
				}
				
				
			}
			
			#endregion
			
			#region MoveSongList
			//Move song list
			var oldpos = camerapack.transform.position.y;
			if(Mathf.Abs(camerapack.transform.position.y - 3f*startnumber) <= 0.1f){
				camerapack.transform.position = new Vector3(camerapack.transform.position.x, - 3f*startnumber, camerapack.transform.position.z);
				posLabel = startnumber;
			}else{
				 
				camerapack.transform.position = Vector3.Lerp(camerapack.transform.position, new Vector3(camerapack.transform.position.x, -3f*startnumber, camerapack.transform.position.z), Time.deltaTime/speedCameraDefil);
				
				posLabel = Mathf.Lerp(posLabel, startnumber, Time.deltaTime/speedCameraDefil);
				
			}
			var newpos = camerapack.transform.position.y;
			
			//Move song list
			if(oldpos > newpos){
			
				foreach(var cubeel2 in songCubeOnRender.Where(c => !c.Key.active && (c.Key.transform.position.y > camerapack.transform.position.y - 3f*numberToDisplay) && !(c.Key.transform.position.y > camerapack.transform.position.y + 2f) && packs.ElementAt(nextnumberPack).Key == c.Value)){
					cubeel2.Key.SetActiveRecursively(true);
					if(cubeSelected == null || cubeSelected != cubeel2.Key) cubeel2.Key.transform.FindChild("Selection").gameObject.active = false;
					
				}
				
				
				foreach(var cubeel in songCubeOnRender.Where(c => c.Key.active && (c.Key.transform.position.y > camerapack.transform.position.y + 2f) && packs.ElementAt(nextnumberPack).Key == c.Value)){
					cubeel.Key.SetActiveRecursively(false);
					if(startnumber > currentstartnumber) currentstartnumber++;
					cubeBase.transform.position = new Vector3(basePosCubeBase.x, basePosCubeBase.y - (3f*currentstartnumber), basePosCubeBase.z);
				}
				/*var cubeel2 = songCubeOnRender.FirstOrDefault(c => !c.Key.active && (c.Key.transform.position.y > camerapack.transform.position.y - 3f*numberToDisplay) && !(c.Key.transform.position.y > camerapack.transform.position.y + 2f) && packs.ElementAt(nextnumberPack).Key == c.Value).Key;
				if(cubeel2 != null) {
					cubeel2.SetActiveRecursively(true);
					if(cubeSelected == null || cubeSelected != cubeel2) cubeel2.transform.FindChild("Selection").gameObject.active = false;
				}
				
				
				var cubeel = songCubeOnRender.FirstOrDefault(c => c.Key.active && (c.Key.transform.position.y > camerapack.transform.position.y + 2f) && packs.ElementAt(nextnumberPack).Key == c.Value).Key;
				if(cubeel != null) {
					cubeel.SetActiveRecursively(false);
					cubeBase.transform.position = new Vector3(cubeBase.transform.position.x, cubeBase.transform.position.y -3f, cubeBase.transform.position.z);
				}
				*/
				
				
			}else if(oldpos < newpos){
				
				foreach(var cubeel2 in songCubeOnRender.Where(c => c.Key.active && (c.Key.transform.position.y < camerapack.transform.position.y - 3f*numberToDisplay) && packs.ElementAt(nextnumberPack).Key == c.Value)){

					cubeel2.Key.SetActiveRecursively(false);
					
				}
				
				
				foreach(var cubeel in songCubeOnRender.Where(c => !c.Key.active && (c.Key.transform.position.y < camerapack.transform.position.y + 5f) && (c.Key.transform.position.y > camerapack.transform.position.y - 3f*(numberToDisplay - 2)) && packs.ElementAt(nextnumberPack).Key == c.Value)){

					cubeel.Key.SetActiveRecursively(true);
					if(startnumber < currentstartnumber) currentstartnumber--;
					if(cubeSelected == null || cubeSelected != cubeel.Key) cubeel.Key.transform.FindChild("Selection").gameObject.active = false;
					cubeBase.transform.position = new Vector3(basePosCubeBase.x, basePosCubeBase.y - (3f*currentstartnumber), basePosCubeBase.z);
					
				}
				
				/*
				var cubeel2 = songCubeOnRender.FirstOrDefault(c => c.Key.active && (c.Key.transform.position.y < camerapack.transform.position.y - 3f*numberToDisplay) && packs.ElementAt(nextnumberPack).Key == c.Value).Key;
				if(cubeel2 != null) {
					cubeel2.SetActiveRecursively(false);
				}
				
				var cubeel = songCubeOnRender.FirstOrDefault(c => !c.Key.active && (c.Key.transform.position.y < camerapack.transform.position.y + 5f) && (c.Key.transform.position.y > camerapack.transform.position.y - 3f*(numberToDisplay - 2)) && packs.ElementAt(nextnumberPack).Key == c.Value).Key;
				if(cubeel != null) {
					cubeel.SetActiveRecursively(true);
					if(cubeSelected == null || cubeSelected != cubeel) cubeel.transform.FindChild("Selection").gameObject.active = false;
					cubeBase.transform.position = new Vector3(cubeBase.transform.position.x, cubeBase.transform.position.y +3f, cubeBase.transform.position.z);
				}*/
			}
			
			#endregion
			
			#region Banner
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
						plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[packs.ElementAt(nextnumberPack).Key];
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
			
			
			
			
			
		}
		
		#region Audio
			
			if(songSelected != null && alreadyRefresh && mainThemeClip.volume > 0){
					mainThemeClip.volume -= Time.deltaTime/speedAudioVolume;
			}
			if((songSelected == null || !alreadyRefresh) && mainThemeClip.volume < 1){
					mainThemeClip.volume += Time.deltaTime/speedAudioVolume;
			}
			
		#endregion
		
		#region option
			
			if(OptionMode && matCache.color.a > 0f){
				fadeAlphaOptionTitle -= Time.deltaTime/timeOption;
				matCache.color = new Color(matCache.color.r, matCache.color.g, matCache.color.b, fadeAlphaOptionTitle);
			}
			
		#endregion
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
	
	void verifyScore(){
		var oldscore = score;
		
		var kv = ProfileManager.Instance.FindTheBestScore(songSelected[actualySelected].sip);
		bestfriendscore = kv.Key;
		bestnamefriendscore = kv.Value;
		var mystats = ProfileManager.Instance.FindTheSongStat(songSelected[actualySelected].sip);
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
		if(DataManager.Instance.giveNoteOfScore((float)score) != DataManager.Instance.giveNoteOfScore((float)oldscore) && oldscore >= 96f){
			medals.FirstOrDefault(c => c.name == DataManager.Instance.giveNoteOfScore((float)oldscore).Split(';')[1]).SetActiveRecursively(false);
		}
		if(score >= 96f){
			medals.FirstOrDefault(c => c.name == DataManager.Instance.giveNoteOfScore((float)score).Split(';')[1]).SetActiveRecursively(true);
		}
		
	}
	
	
	
	
	
	#endregion
}
