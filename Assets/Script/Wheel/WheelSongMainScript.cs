using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class WheelSongMainScript : MonoBehaviour {
	
	#region variable	
	//GameObject library
	public GameObject miniCubePack;
	public Camera camerapack;
	public Camera cameradiff;
	public GUISkin skin;
	public GameObject cubeSong;
	public GameObject cubeBase;
	public GameObject plane;
	public GameObject PSCore;
	public GameObject RayCore;
	public LineRenderer graph;
	public LineRenderer separator;
	
	
	//List And dictionary
	private Dictionary<string, GameObject> packs;
	private Dictionary<GameObject, float> cubesPos;
	private Dictionary<GameObject, string> songCubePack;
	private Dictionary<GameObject, string> LinkCubeSong;
	private Dictionary<Difficulty, Song> songSelected;
	private Dictionary<Difficulty, GameObject> diffSelected;
	private Dictionary<Difficulty, Color> diffActiveColor;
	private Dictionary<string, Texture2D> tex;
	private Dictionary<int, ParticleSystem> PSDiff;
	private Dictionary<int, GameObject> RayDiff;
	
	
	//Banner
	private Texture2D actualBanner;
	public float speedFadeAlpha;
	private float alphaBanner;
	private bool FadeOutBanner;
	private bool FadeInBanner;
	
	//General GUI
	private float totalAlpha;
	private string textButton;
	
	//PackList
	private int numberPack;
	private int nextnumberPack;
	public float x10;
	public float xm10;
	public float y;
	public float wd;
	public float ht;
	public Rect posBackward;
	public Rect posForward;
	public float ecart;
	public float speedMove;
	public float limite;
	
	private float decalFade;
	private float decalFadeM;
	private float fadeAlpha;
	
	
	//SongList
	private GameObject cubeSelected;
	public Rect posSonglist;
	public float ecartSong;
	private GameObject particleOnPlay;
	public int numberToDisplay;
	private int startnumber;
	public float speedCameraDefil;
	private float posLabel;
	public float offsetSubstitle;
	
	//Difficulty
	public Rect posDifficulty;
	public float ecartDiff;
	public float[] offsetX;
	private int[] diffNumber;
	public Rect posNumberDiff;
	
	private Difficulty actualySelected;
	private Difficulty trulySelected;
	private Difficulty onHoverDifficulty;
	
	public float diffZoom;
	private bool locked;
	
	//InfoSong
	public Rect posGraph;
	public Rect posInfo;
	public Rect posInfo2;
	public Rect posInfo3;
	public Rect posInfo4;
	public Rect posMaxinten;
	public Rect Jouer;
	public Rect JouerTex;
	public Rect Option;
	public Rect OptionTex;
	public float offsetInfo;
	public float departGraphY;
	public float topGraphY;
	public float departGraphX;
	public float topGraphX;
	public Rect BPMDisplay;
	public Rect artistnstepDisplay;
	
	
	//Declancheurs
	private bool movinForward;
	private bool movinBackward;
	private bool movinForwardFast;
	private bool movinBackwardFast;
	private bool movinOption;
	private bool OptionMode;
	private bool movinNormal;
	
	
	
	//Move to option mode
	public float speedMoveOption;
	public float limiteMoveOption;
	private float basePositionSeparator = -5.5f;
	private float offsetXDiff;
	private float offsetYDiff;
	private Vector2 departOptionDiff;
	private Vector2 moveOptionDiff;
	public Vector2 arriveOptionDiff;
	
	
	//General Update
	public float timeBeforeDisplay;
	private float time;
	private bool alreadyRefresh;
	
	#endregion
	// Use this for initialization
	void Start () {
		numberPack = 0;
		nextnumberPack = 0;
		startnumber = 0;
		packs = new Dictionary<string, GameObject>();
		cubesPos = new Dictionary<GameObject, float>();
		songCubePack = new Dictionary<GameObject, string>();
		LinkCubeSong = new Dictionary<GameObject, string>();
		diffSelected = new Dictionary<Difficulty, GameObject>();
		diffActiveColor = new Dictionary<Difficulty, Color>();
		PSDiff = new Dictionary<int, ParticleSystem>();
		RayDiff = new Dictionary<int, GameObject>();
		LoadManager.Instance.Loading();
		actualBanner = new Texture2D(256,512);
		
		tex = new Dictionary<string, Texture2D>();
		tex.Add("BEGINNER", (Texture2D) Resources.Load("beginner"));
		tex.Add("EASY", (Texture2D) Resources.Load("easy"));
		tex.Add("MEDIUM", (Texture2D) Resources.Load("medium"));
		tex.Add("HARD", (Texture2D) Resources.Load("hard"));
		tex.Add("EXPERT", (Texture2D) Resources.Load("expert"));
		tex.Add("EDIT", (Texture2D) Resources.Load("edit"));
		tex.Add("graph", (Texture2D) Resources.Load("graph"));
		tex.Add("bouton", (Texture2D) Resources.Load("GUIBarMini"));
		
		diffSelected.Add(Difficulty.BEGINNER, GameObject.Find("DifficultyB"));
		diffSelected.Add(Difficulty.EASY, GameObject.Find("DifficultyEs"));
		diffSelected.Add(Difficulty.MEDIUM, GameObject.Find("DifficultyM"));
		diffSelected.Add(Difficulty.HARD, GameObject.Find("DifficultyH"));
		diffSelected.Add(Difficulty.EXPERT, GameObject.Find("DifficultyEx"));
		diffSelected.Add(Difficulty.EDIT, GameObject.Find("DifficultyEd"));
			
		diffActiveColor.Add(Difficulty.BEGINNER, diffSelected[Difficulty.BEGINNER].transform.GetChild(0).renderer.material.GetColor("_TintColor"));
		diffActiveColor.Add(Difficulty.EASY, diffSelected[Difficulty.EASY].transform.GetChild(0).renderer.material.GetColor("_TintColor"));
		diffActiveColor.Add(Difficulty.MEDIUM, diffSelected[Difficulty.MEDIUM].transform.GetChild(0).renderer.material.GetColor("_TintColor"));
		diffActiveColor.Add(Difficulty.HARD, diffSelected[Difficulty.HARD].transform.GetChild(0).renderer.material.GetColor("_TintColor"));
		diffActiveColor.Add(Difficulty.EXPERT, diffSelected[Difficulty.EXPERT].transform.GetChild(0).renderer.material.GetColor("_TintColor"));
		diffActiveColor.Add(Difficulty.EDIT, diffSelected[Difficulty.EDIT].transform.GetChild(0).renderer.material.GetColor("_TintColor"));
		
		for(int i=0; i<6;i++){
			PSDiff.Add(i, (ParticleSystem) PSCore.transform.FindChild(""+i).particleSystem);
		}
		
		for(int i=0; i<6;i++){
			RayDiff.Add(i, (GameObject) RayCore.transform.FindChild(""+i).gameObject);
		}
		
		
		//To do : correct "KeyAlreadyAssign"
		while(packs.Count() < 5){
			foreach(var el in LoadManager.Instance.ListSong().Keys){
				var thego = (GameObject) Instantiate(miniCubePack, new Vector3(0f, 13f, 20f), miniCubePack.transform.rotation);
				if(LoadManager.Instance.ListTexture().ContainsKey(el)) thego.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[el];
				if(packs.ContainsKey(el)){ 
					packs.Add(el + "(" + packs.Count + ")", thego);	
				}
				else
				{ 
					packs.Add(el, thego); 
				}
				cubesPos.Add(thego, 0f);
			}
		}
		organiseCube();
		createCubeSong();
		desactiveDiff();
		activePack(packs.ElementAt(0).Key);
		plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[packs.ElementAt(0).Key];
		diffNumber = new int[6];
		for(int i=0;i<6; i++){ diffNumber[i] = 0; }
		decalFade = 0f;
		decalFadeM = 0f;
		fadeAlpha = 0f;
		posLabel = 0f;
		time = 0f;
		alphaBanner = 1f;
		totalAlpha = 0f;
		locked = false;
		alreadyRefresh = true;
		FadeOutBanner = false;
		FadeInBanner = false;
		graph.enabled = false;
		movinOption = false;
		OptionMode = false;
		textButton = "Option";
		actualySelected = Difficulty.BEGINNER;
		trulySelected = Difficulty.BEGINNER;
		onHoverDifficulty = Difficulty.NONE;
		
	}
	
	// Update is called once per frame
	public void OnGUI () {
		GUI.skin = skin;
		GUI.color = new Color(1f, 1f, 1f, 1f - totalAlpha);
		
		if(!OptionMode){
			#region packGUI
			//Packs
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
				camerapack.transform.position = new Vector3(0f, 0f, 0f);
				cubeBase.transform.position = new Vector3(cubeBase.transform.position.x, 5f, cubeBase.transform.position.z);
			}
			if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height + ecart*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","Backward") && !movinNormal && !movinOption){
				nextnumberPack = PrevInt(numberPack, 3);
				activePack(packs.ElementAt(nextnumberPack).Key);
				movinBackwardFast = true;
				startnumber = 0;
				camerapack.transform.position = new Vector3(0f, 0f, 0f);
				cubeBase.transform.position = new Vector3(cubeBase.transform.position.x, 5f, cubeBase.transform.position.z);
			}
			
			if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","LForward") && !movinForward && !movinNormal && !movinOption)
			{
				nextnumberPack = NextInt(numberPack, 1);
				activePack(packs.ElementAt(nextnumberPack).Key);
				movinForward = true;
				startnumber = 0;
				camerapack.transform.position = new Vector3(0f, 0f, 0f);
				cubeBase.transform.position = new Vector3(cubeBase.transform.position.x, 5f, cubeBase.transform.position.z);
			}
			if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height + ecart*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","Forward") && !movinNormal && !movinOption){
				nextnumberPack = NextInt(numberPack, 3);
				activePack(packs.ElementAt(nextnumberPack).Key);
				movinForwardFast = true;
				startnumber = 0;
				camerapack.transform.position = new Vector3(0f, 0f, 0f);
				cubeBase.transform.position = new Vector3(cubeBase.transform.position.x, 5f, cubeBase.transform.position.z);
			}
			#endregion
			
			#region songGUI
			//SONGS
			 
			var thepos = -posLabel;
			for(int i=0; i<LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].Count; i++){
				if(thepos >= 0f && thepos <= numberToDisplay){
					
					var el = LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].ElementAt(i);
					GUI.color = new Color(0f, 0f, 0f, 1f - totalAlpha);
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.title, "songlabel");
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos + offsetSubstitle)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.subtitle, "infosong");
					GUI.color = new Color(1f, 1f, 1f, 1f - totalAlpha);
					GUI.Label(new Rect(posSonglist.x*Screen.width, (posSonglist.y + ecartSong*thepos)*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.title, "songlabel");
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos + offsetSubstitle)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.subtitle, "infosong");
				}else if(thepos > -1f && thepos < 0f){
					var el = LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].ElementAt(i);
					GUI.color = new Color(0f, 0f, 0f, 1f + thepos);
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.title, "songlabel");
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos + offsetSubstitle)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.subtitle, "infosong");
					GUI.color = new Color(1f, 1f, 1f, 1f + thepos);
					GUI.Label(new Rect(posSonglist.x*Screen.width, (posSonglist.y + ecartSong*thepos)*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.title, "songlabel");
					GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos + offsetSubstitle)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.subtitle, "infosong");
				}
				thepos++;
			}
			#endregion
		}
		
		GUI.color = new Color(1f, 1f, 1f, 1f - totalAlpha);
		
		#region songdifficultyGUI
		//SongDifficulty
		if(songSelected != null && !OptionMode){
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
		if(songSelected != null && !OptionMode){ 
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
		
			
		
		}
		#endregion
		
		GUI.color = new Color(1f, 1f, 1f, 1f);
		
		#region optionPlayGUI
		//Option/jouer
		if(songSelected != null){
		
			//JouerTex
			GUI.DrawTexture(new Rect(JouerTex.x*Screen.width, JouerTex.y*Screen.height, JouerTex.width*Screen.width, JouerTex.height*Screen.height), tex["bouton"]);
		
			
			//OptionTex
			GUI.DrawTexture(new Rect(OptionTex.x*Screen.width, OptionTex.y*Screen.height, OptionTex.width*Screen.width, OptionTex.height*Screen.height), tex["bouton"]);
		
			
		
			
			//Jouer
			if(GUI.Button(new Rect(Jouer.x*Screen.width, Jouer.y*Screen.height, Jouer.width*Screen.width, Jouer.height*Screen.height), "Play", "labelGo")){
				Debug.Log("Play " + songSelected[actualySelected].title + " in " + actualySelected.ToString() + " mode");	
			}
		
			//Option
			if(GUI.Button(new Rect(Option.x*Screen.width, Option.y*Screen.height, Option.width*Screen.width, Option.height*Screen.height), textButton, "labelGo") && !movinOption && !movinNormal){
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
					OptionMode = false;
					movinNormal = true;
					
				}
				
			}
			
		}
		
		//During move to option
		if(OptionMode || movinNormal){
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
	}
	
	
	
	void Update(){
		
		#region MoveToOptionUpdate
		//Move to option
		if(movinOption){
			foreach(var el in packs){
				el.Value.transform.position = Vector3.Lerp(el.Value.transform.position, new Vector3(el.Value.transform.position.x, 20f, el.Value.transform.position.z), Time.deltaTime/speedMoveOption);
			}	
			foreach(var eld in diffSelected){
				if(eld.Key != actualySelected) eld.Value.transform.position = Vector3.Lerp(eld.Value.transform.position, new Vector3(5f, eld.Value.transform.position.y, eld.Value.transform.position.z), Time.deltaTime/speedMoveOption);
			}
			camerapack.transform.position = Vector3.Lerp(camerapack.transform.position, new Vector3(20f, camerapack.transform.position.y, camerapack.transform.position.z), Time.deltaTime/speedMoveOption);
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
			if(Mathf.Abs(camerapack.transform.position.x - 20f) <= limiteMoveOption){
				
				foreach(var el in packs){
					el.Value.transform.position = new Vector3(el.Value.transform.position.x, 20f, el.Value.transform.position.z);
				}	
				foreach(var eld in diffSelected){
					if(eld.Key != actualySelected) eld.Value.transform.position = new Vector3(5f, eld.Value.transform.position.y, eld.Value.transform.position.z);
				}
				
				camerapack.transform.position = new Vector3(20f, camerapack.transform.position.y, camerapack.transform.position.z);
				totalAlpha = 1f;
				separator.SetPosition(0, new Vector3(-50f, -20f, 20f));
				separator.SetPosition(1, new Vector3(-50f, 12f, 20f));
				offsetXDiff = arriveOptionDiff.x - departOptionDiff.x;
				offsetYDiff = arriveOptionDiff.y - departOptionDiff.y;
				diffSelected[actualySelected].transform.position = new Vector3(-1.2f, 0.75f, 2f);
				plane.transform.position = new Vector3(0f, 12f, 20f);
				movinOption = false;
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
						if(songSelected == null || ((songSelected.First().Value.title + "/" + songSelected.First().Value.subtitle) != LinkCubeSong[papa.gameObject])){
							songSelected = LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].FirstOrDefault(c => (c.Value.First().Value.title + "/" + c.Value.First().Value.subtitle) == LinkCubeSong[papa.gameObject]).Value;
							activeNumberDiff(songSelected);
							activeDiff(songSelected);
							PSDiff[(int)actualySelected].gameObject.active = false;
							activeDiffPS(songSelected);
							PSDiff[(int)actualySelected].gameObject.active = true;
							displayGraph(songSelected);
							graph.enabled = true;
							cubeSelected = papa.gameObject;
							alreadyRefresh = false;
							
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
							if(songSelected == null || ((songSelected.First().Value.title + "/" + songSelected.First().Value.subtitle) != LinkCubeSong[papa.gameObject])){
								songSelected = LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].FirstOrDefault(c => (c.Value.First().Value.title + "/" + c.Value.First().Value.subtitle) == LinkCubeSong[papa.gameObject]).Value;
								activeNumberDiff(songSelected);
								activeDiff(songSelected);
								PSDiff[(int)actualySelected].gameObject.active = false;
								activeDiffPS(songSelected);
								PSDiff[(int)actualySelected].gameObject.active = true;
								displayGraph(songSelected);
								graph.enabled = true;
								cubeSelected = papa.gameObject;
								alreadyRefresh = false;
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
						PSDiff[(int)actualySelected].gameObject.active = false;
						desactiveDiff();
						particleOnPlay.active = false;
					}
				}
				
				
				
			}else if(particleOnPlay != null && particleOnPlay.active){
				if(!locked){
					cubeSelected = null;
					songSelected = null;
					FadeOutBanner = true;
					PSDiff[(int)actualySelected].gameObject.active = false;
					graph.enabled = false;
					desactiveDiff();
					particleOnPlay.active = false;
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
			
			
			//Scrollwheel
			if(Input.GetAxis("Mouse ScrollWheel") > 0 && startnumber > 0){
				startnumber--;
				
				
			}else if(Input.GetAxis("Mouse ScrollWheel") < 0 && startnumber < (songCubePack.Where(c => packs.ElementAt(nextnumberPack).Key == c.Value).Count() - numberToDisplay + 1)){
				startnumber++;
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
			
				var cubeel2 = songCubePack.FirstOrDefault(c => !c.Key.active && (c.Key.transform.position.y > camerapack.transform.position.y - 3f*numberToDisplay) && !(c.Key.transform.position.y > camerapack.transform.position.y + 2f) && packs.ElementAt(nextnumberPack).Key == c.Value).Key;
				if(cubeel2 != null) {
					cubeel2.SetActiveRecursively(true);
					if(cubeSelected == null || cubeSelected != cubeel2) cubeel2.transform.FindChild("Selection").gameObject.active = false;
				}
				
				
				var cubeel = songCubePack.FirstOrDefault(c => c.Key.active && (c.Key.transform.position.y > camerapack.transform.position.y + 2f) && packs.ElementAt(nextnumberPack).Key == c.Value).Key;
				if(cubeel != null) {
					cubeel.SetActiveRecursively(false);
					cubeBase.transform.position = new Vector3(cubeBase.transform.position.x, cubeBase.transform.position.y -3f, cubeBase.transform.position.z);
				}
				
				
				
			}else if(oldpos < newpos){
				
				var cubeel2 = songCubePack.FirstOrDefault(c => c.Key.active && (c.Key.transform.position.y < camerapack.transform.position.y - 3f*numberToDisplay) && packs.ElementAt(nextnumberPack).Key == c.Value).Key;
				if(cubeel2 != null) {
					cubeel2.SetActiveRecursively(false);
				}
				
				var cubeel = songCubePack.FirstOrDefault(c => !c.Key.active && (c.Key.transform.position.y < camerapack.transform.position.y + 5f) && (c.Key.transform.position.y > camerapack.transform.position.y - 3f*(numberToDisplay - 2)) && packs.ElementAt(nextnumberPack).Key == c.Value).Key;
				if(cubeel != null) {
					cubeel.SetActiveRecursively(true);
					if(cubeSelected == null || cubeSelected != cubeel) cubeel.transform.FindChild("Selection").gameObject.active = false;
					cubeBase.transform.position = new Vector3(cubeBase.transform.position.x, cubeBase.transform.position.y +3f, cubeBase.transform.position.z);
				}
			}
			
			#endregion
			
			#region Banner
			//refreshBanner
			if(songSelected != null && !alreadyRefresh){
				if(time >= timeBeforeDisplay){
					plane.renderer.material.mainTexture = actualBanner;
					var thebanner = songSelected.First().Value.GetBanner(actualBanner);
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
	}
	
	
	#region difficulty
	void activeDiffPS(Dictionary<Difficulty, Song> so){
		if(so.ContainsKey(trulySelected)){
			actualySelected = trulySelected;	
		}else{
			var min = 99;
			var mini = (int)trulySelected;
			for(int i=(int)Difficulty.BEGINNER; i<=(int)Difficulty.EDIT; i++){
				if(so.ContainsKey((Difficulty)i)){
					var abs = Mathf.Abs((float)i - (float)trulySelected);
					if(abs < min){
						min = (int)abs;	
						mini = i;
					}
				}
			}
			actualySelected = (Difficulty)mini;
			
		}
	}
	
	void activeDiff(Dictionary<Difficulty, Song> so){
		var countpos = 0;
		for(int i=(int)Difficulty.BEGINNER; i<=(int)Difficulty.EDIT; i++){
			if(so.ContainsKey((Difficulty)i)){
				//diffSelected[(Difficulty)i].SetActiveRecursively(true);	
				diffSelected[(Difficulty)i].transform.position = new Vector3(diffSelected[(Difficulty)i].transform.position.x, DataManager.Instance.posYDiff[countpos], diffSelected[(Difficulty)i].transform.position.z);
				PSDiff[i].transform.position = new Vector3(PSDiff[i].transform.position.x, DataManager.Instance.posYZoneDiff[countpos], PSDiff[i].transform.position.z);
				RayDiff[i].transform.position = new Vector3(RayDiff[i].transform.position.x, DataManager.Instance.posYZoneDiff[countpos], RayDiff[i].transform.position.z);
				countpos++;
				for(int j=0; j<diffSelected[(Difficulty)i].transform.GetChildCount(); j++){
					if((int.Parse(diffSelected[(Difficulty)i].transform.GetChild(j).name)) <= so[(Difficulty)i].level){
						if(diffSelected[(Difficulty)i].transform.GetChild(j).renderer.material.GetColor("_TintColor") != diffActiveColor[(Difficulty)i]) diffSelected[(Difficulty)i].transform.GetChild(j).renderer.material.SetColor("_TintColor",diffActiveColor[(Difficulty)i]);
					}else{
						if(diffSelected[(Difficulty)i].transform.GetChild(j).renderer.material.GetColor("_TintColor") == diffActiveColor[(Difficulty)i]) diffSelected[(Difficulty)i].transform.GetChild(j).renderer.material.SetColor("_TintColor",new Color(diffActiveColor[(Difficulty)i].r/10f, diffActiveColor[(Difficulty)i].g/10f, diffActiveColor[(Difficulty)i].b/10f, 1f));
					}
				}
			}else{
				diffSelected[(Difficulty)i].transform.Translate(0f, -100f, 0f);
				RayDiff[i].transform.Translate(0f, -100f, 0f);
				for(int j=0; j<diffSelected[(Difficulty)i].transform.GetChildCount(); j++){
					if(diffSelected[(Difficulty)i].transform.GetChild(j).renderer.material.GetColor("_TintColor") == diffActiveColor[(Difficulty)i]) diffSelected[(Difficulty)i].transform.GetChild(j).renderer.material.SetColor("_TintColor",new Color(diffActiveColor[(Difficulty)i].r/10f, diffActiveColor[(Difficulty)i].g/10f, diffActiveColor[(Difficulty)i].b/10f, 1f));
				}
			}
		}
	}
	
	void desactiveDiff(){
		for(int i=(int)Difficulty.BEGINNER; i<=(int)Difficulty.EDIT; i++){
			RayDiff[i].transform.Translate(0f, -100f, 0f);
			diffSelected[(Difficulty)i].transform.Translate(0f, -100f, 0f);
		}
	}
	
	void activeNumberDiff(Dictionary<Difficulty, Song> so){
		for(int i=(int)Difficulty.BEGINNER; i<=(int)Difficulty.EDIT; i++){
			if(so.ContainsKey((Difficulty)i)){
				
				diffNumber[i] = so[(Difficulty)i].level;
				
			}else{
				diffNumber[i] = 0;
			}
		}
	}
	#endregion
	
	#region packs
	void activePack(string s){
		foreach(var el in songCubePack){
			if(el.Value == s && el.Key.transform.position.y > - 3f*numberToDisplay){
				el.Key.SetActiveRecursively(true);
				el.Key.transform.FindChild("Selection").gameObject.active = false;
			}else if(el.Key.active){
				el.Key.SetActiveRecursively(false);
			}
		}
		plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[packs.ElementAt(nextnumberPack).Key];
		plane.renderer.material.color = new Color(plane.renderer.material.color.r, plane.renderer.material.color.g, plane.renderer.material.color.b, 1f);
		alphaBanner = 1f;
		FadeOutBanner = false;
	}
	#endregion
	
	#region graph
	void displayGraph(Dictionary<Difficulty, Song> so){
		var thesong = so[actualySelected];
		for(int i=0;i<100;i++){
			var thepos = new Vector3(departGraphX + (topGraphX - departGraphX)*((float)i/100f), departGraphY + (topGraphY - departGraphY)*(thesong.intensityGraph[i]/(float)thesong.stepPerSecondMaximum), 2f);
			graph.SetPosition(i, thepos);	
		}
			
	}
	#endregion
	
	#region cubes
	void createCubeSong(){
		
		foreach(var el in LoadManager.Instance.ListSong()){
			var pos = 2f;
			foreach(var song in el.Value){
				var thego = (GameObject) Instantiate(cubeSong, new Vector3(-25f, pos, 0f), cubeSong.transform.rotation);
				pos -= 3f;
				thego.SetActiveRecursively(false);
				songCubePack.Add(thego,el.Key);
				LinkCubeSong.Add(thego, song.Value.First().Value.title + "/" + song.Value.First().Value.subtitle);

			}
		}
	}
	
	
	void organiseCube(){
		cubesPos.ElementAt(0).Key.transform.position = new Vector3(0f, 13, 20f);
		cubesPos[cubesPos.ElementAt(0).Key] = 0f;
		cubesPos.ElementAt(0).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		
		cubesPos.ElementAt(1).Key.transform.position = new Vector3(10f, 13, 20f);
		cubesPos[cubesPos.ElementAt(1).Key] = 10f;
		cubesPos.ElementAt(1).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
		
		cubesPos.ElementAt(2).Key.transform.position = new Vector3(20f, 13, 20f);
		cubesPos[cubesPos.ElementAt(2).Key] = 20f;
		cubesPos.ElementAt(2).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		cubesPos.ElementAt(cubesPos.Count - 1).Key.transform.position = new Vector3(-10f, 13, 20f);
		cubesPos[cubesPos.ElementAt(cubesPos.Count - 1).Key] = -10f;
		cubesPos.ElementAt(cubesPos.Count - 1).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
		
		cubesPos.ElementAt(cubesPos.Count - 2).Key.transform.position = new Vector3(-20f, 13, 20f);
		cubesPos[cubesPos.ElementAt(cubesPos.Count - 2).Key] = -20f;
		cubesPos.ElementAt(cubesPos.Count - 2).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		for(int i=3; i<cubesPos.Count - 2;i++){
			cubesPos.ElementAt(i).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		}
	}
	
	void decreaseCubeF(){
		
		fadeAlpha = Mathf.Lerp(fadeAlpha, 0.5f, Time.deltaTime/speedMove);
		cubesPos.ElementAt(numberPack).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f - fadeAlpha);
		
		cubesPos.ElementAt(NextInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f + fadeAlpha);
		
		cubesPos.ElementAt(NextInt(numberPack, 2)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f + fadeAlpha);
		
		cubesPos.ElementAt(PrevInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f - fadeAlpha);
	}
	
	
	
	void decreaseCubeB(){
		
		fadeAlpha = Mathf.Lerp(fadeAlpha, 0.5f, Time.deltaTime/speedMove);
		cubesPos.ElementAt(numberPack).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f - fadeAlpha);
		
		cubesPos.ElementAt(NextInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f - fadeAlpha);
		
		
		cubesPos.ElementAt(PrevInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f + fadeAlpha);
		
		cubesPos.ElementAt(PrevInt(numberPack, 2)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f + fadeAlpha);
	}
	
	void fastDecreaseCubeF(){
		
		cubesPos.ElementAt(numberPack).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		cubesPos.ElementAt(NextInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		cubesPos.ElementAt(NextInt(numberPack, 2)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
		
		cubesPos.ElementAt(NextInt(numberPack, 3)).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		
		cubesPos.ElementAt(NextInt(numberPack, 4)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
		
		cubesPos.ElementAt(PrevInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0);
	}
	
	void fastDecreaseCubeB(){
		
		cubesPos.ElementAt(numberPack).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		cubesPos.ElementAt(NextInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		
		cubesPos.ElementAt(PrevInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		cubesPos.ElementAt(PrevInt(numberPack, 2)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
		
		cubesPos.ElementAt(PrevInt(numberPack, 3)).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		
		cubesPos.ElementAt(PrevInt(numberPack, 4)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
	}
	#endregion
	
	
	#region util
	int NextInt(int i, int recurs){
		var res = 0;
		if(i == (packs.Count - 1)){
			res = 0;	
		}else{
			res = i + 1;
		}
		
		if(recurs > 1){
			return NextInt(res, recurs - 1);
			
		}else{
			return res;
		}
	}
	
	int PrevInt(int i, int recurs){
		var res = 0;
		if(i == 0){
			res = (packs.Count - 1);	
		}else{
			res = i - 1;
		}
		
		if(recurs > 1){
			return PrevInt(res, recurs - 1);	
			
		}else{
			return res;
		}
	}
	#endregion
}
