using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class InfoZone : MonoBehaviour {
	
	
	public GameObject plane;
	public Camera cameradiff;
	public GameObject PSCore;
	public GameObject RayCore;
	public LineRenderer graph;
	
	
	private Dictionary<Difficulty, GameObject> diffSelected;
	private Dictionary<Difficulty, Color> diffActiveColor;
	
	private Dictionary<int, ParticleSystem> PSDiff;
	private Dictionary<int, GameObject> RayDiff;
	public List<GameObject> medals;
	
	
	//Banner
	private Texture2D actualBanner;
	public float speedFadeAlpha;
	private float alphaBanner;
	private bool FadeOutBanner;
	private bool FadeInBanner;
	
	
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
	
	
	
	//InfoSong
	public Rect posGraph;
	public Rect posInfo;
	public Rect posInfo2;
	public Rect posInfo3;
	public Rect posInfo4;
	public Rect posInfo5;
	public Rect posMaxinten;
	public Rect Jouer;
	public Rect Option;
	public float offsetInfo;
	public float departGraphY;
	public float topGraphY;
	public float departGraphX;
	public float topGraphX;
	public Rect BPMDisplay;
	public Rect artistnstepDisplay;
	private double score;
	private double bestfriendscore;
	private string bestnamefriendscore;
	private bool isScoreFail;
	public Rect posNote;
	public Rect posSpecialNote;
	
	
	
	// Use this for initialization
	void Start () {
		
		diffSelected = new Dictionary<Difficulty, GameObject>();
		diffActiveColor = new Dictionary<Difficulty, Color>();
		PSDiff = new Dictionary<int, ParticleSystem>();
		RayDiff = new Dictionary<int, GameObject>();
		
		
		actualBanner = new Texture2D(256,512);
		
		
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
		
		desactiveDiff();
		
		plane.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[packs.ElementAt(0).Key];
		diffNumber = new int[6];
		for(int i=0;i<6; i++){ diffNumber[i] = 0; }
		alphaBanner = 1f;
		
		
		FadeOutBanner = false;
		FadeInBanner = false;
		graph.enabled = false;
		
		actualySelected =  DataManager.Instance.difficultySelected;
		trulySelected = DataManager.Instance.difficultySelected;
		onHoverDifficulty = Difficulty.NONE;
		
	}
	
	// Update is called once per frame
	void Update () {
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
	}
	
	void OnGUI()
	{
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
		
	}
	
	void displayGraph(Dictionary<Difficulty, Song> so){
		var thesong = so[actualySelected];
		for(int i=0;i<100;i++){
			var thepos = new Vector3(departGraphX + (topGraphX - departGraphX)*((float)i/100f), departGraphY + (topGraphY - departGraphY)*((float)thesong.intensityGraph[i]/(float)thesong.stepPerSecondMaximum), 2f);
			graph.SetPosition(i, thepos);	
		}
			
	}
	
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
}
