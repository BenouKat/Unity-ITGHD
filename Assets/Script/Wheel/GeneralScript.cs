using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GeneralScript : MonoBehaviour {
	
	#region variable	
	//GameObject library
	
	
	public GUISkin skin;
	public LineRenderer separator;
	
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
		
		
		
		
		
		
			
		
		alreadyFade = false;
			
		
		
		
		
		
		//quickMode a déplacer aussi
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
		
		
		
		if(!movinNormal && !movinOption && !OptionMode){
			
			
			#region input
			//Unlock
			
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
