using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class InfoZone : MonoBehaviour {
	
	
	
	public Camera cameradiff;
	public GameObject PSCore;
	public GameObject RayCore;
	public LineRenderer graph;
	
	
	private Dictionary<Difficulty, GameObject> diffSelected;
	private Dictionary<Difficulty, Color> diffActiveColor;
	
	private Dictionary<int, ParticleSystem> PSDiff;
	private Dictionary<int, GameObject> RayDiff;
	public List<GameObject> medals;
	
	//A mettre dans le généralScript
	private GeneralScript gs;
	
	
	
	
	//Difficulty
	public Rect posDifficulty;
	private int[] diffNumber;
	public Rect posNumberDiff;
	
	private Difficulty actualySelected;
	private Difficulty trulySelected;
	private Difficulty onHoverDifficulty;
	
	public float diffZoom;
	public float decalInfoDiffX;
	public float decalInfoNumDiffX;
	public float decalInfoDiffY;
	public float decalInfoNumDiffY;
	
	public Vector3 recoverPosition;
	public Vector3 posDiffOption;
	public Vector3 posDiffLaunch;
	
	public float speedMoveDiff;
	
	//InfoSong
	public Rect posGraph;
	public Rect posInfo;
	public Rect posInfo2;
	public Rect posInfo3;
	public Rect posInfo4;
	public Rect posInfo5;
	public Rect posMaxinten;
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
	
	
	private bool enterOption;
	private bool enterLaunch;
	private bool exitOption;
	
	
	private bool activeModule;
	// Use this for initialization
	void Start () {
		gs = GetComponent<GeneralScript>();
		
		activeModule = true;
		
		diffSelected = new Dictionary<Difficulty, GameObject>();
		diffActiveColor = new Dictionary<Difficulty, Color>();
		PSDiff = new Dictionary<int, ParticleSystem>();
		RayDiff = new Dictionary<int, GameObject>();
		
		
		
		
		
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
		
		gs.refreshBanner();
		
		diffNumber = new int[6];
		for(int i=0;i<6; i++){ diffNumber[i] = 0; }
		
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
					displayGraph();
					verifyScore();
				}
			}else{
				onHoverDifficulty = Difficulty.NONE;
			}
		}else{
			onHoverDifficulty = Difficulty.NONE;	
		}
		
		if(enterOption)
		{
			diffSelected[actualySelected].transform.position = Vector3.Lerp(diffSelected[actualySelected].transform.position, posDiffOption, Time.deltaTime/speedMoveDiff);
			if(Vector3.Distance(diffSelected[actualySelected].transform.position, posDiffOption) <= 0.1f)
			{
				diffSelected[actualySelected].transform.position = posDiffOption;
				enterOption = false;
			}
		}
		
		if(exitOption)
		{
			diffSelected[actualySelected].transform.position = Vector3.Lerp(diffSelected[actualySelected].transform.position, recoverPosition, Time.deltaTime/speedMoveDiff);
			if(Vector3.Distance(diffSelected[actualySelected].transform.position, recoverPosition) <= 0.1f)
			{
				diffSelected[actualySelected].transform.position = recoverPosition;
				exitOption = false;
			}
		}
		
		if(enterLaunch)
		{
			diffSelected[actualySelected].transform.position = Vector3.Lerp(diffSelected[actualySelected].transform.position, posDiffLaunch, Time.deltaTime/speedMoveDiff);
			if(Vector3.Distance(diffSelected[actualySelected].transform.position, posDiffLaunch) <= 0.1f)
			{
				diffSelected[actualySelected].transform.position = posDiffLaunch;
				enterLaunch = false;
			}
		}
		
	}
	
	void OnGUI()
	{
		//SongDifficulty
		if(gs.songSelected != null){
			var decalDiffX = decalInfoDiffX*Screen.width;
			var decalNumDiffX = decalInfoNumDiffX*Screen.width;
			var decalDiffY = decalInfoDiffY*Screen.height;
			var decalNumDiffY = decalInfoNumDiffY*Screen.height;
			
			var realwidth = posDifficulty.width*Screen.width;
			var realheight = posDifficulty.height*Screen.height;
			var diffwidth = posNumberDiff.width*Screen.width;
			var diffheight = posNumberDiff.height*Screen.height;
			
			for(int i=0; i<=(int)Difficulty.EDIT; i++){
				if(diffNumber[i] != 0 && (activeModule || (diffNumber[i] == (int)actualySelected)))
				{
					var point2D = cameradiff.WorldToScreenPoint(diffSelected[(Difficulty)i].transform.position);
					
					GUI.color = new Color(1f, 1f, 1f, 1f);
					var zoom = 0f;
					if(onHoverDifficulty  == (Difficulty)i) zoom = diffZoom; 
					GUI.DrawTexture(new Rect(point2D.x + decalDiffX - zoom*Screen.width, point2D.y + decalDiffY - zoom*Screen.height, realwidth + zoom*2f*Screen.width, realheight + zoom*2f*Screen.height), gs.tex[((Difficulty)i).ToString()]);

					GUI.color = new Color(DataManager.Instance.diffColor[i].r, DataManager.Instance.diffColor[i].g, DataManager.Instance.diffColor[i].b, 1f);
					GUI.Label(new Rect(point2D.x + decalNumDiffX , point2D.y + decalNumDiffY, diffwidth, diffheight), gs.songSelected[(Difficulty)i].level.ToString(), "numberdiff");
				}
				
			}
			GUI.color = new Color(1f, 1f, 1f, 1f);
			GUI.DrawTexture(new Rect(posGraph.x*Screen.width, posGraph.y*Screen.height, posGraph.width*Screen.width, posGraph.height*Screen.height), gs.tex["graph"]);
		}
		
		//Song Info
		if(gs.songSelected != null && activeModule){ 
			
			var theSong = gs.songSelected[actualySelected];
			//BPM
			GUI.Label(new Rect(BPMDisplay.x*Screen.width , BPMDisplay.y*Screen.height, BPMDisplay.width*Screen.width, BPMDisplay.height*Screen.height), "BPM\n" + theSong.bpmToDisplay, "bpmdisplay");
			
			//Artist n stepartist
			GUI.Label(new Rect(artistnstepDisplay.x*Screen.width , artistnstepDisplay.y*Screen.height, artistnstepDisplay.width*Screen.width, artistnstepDisplay.height*Screen.height), theSong.artist + " - Step by : " + gs.songSelected[(Difficulty)actualySelected].stepartist);
			
			//Number of step
			GUI.Label(new Rect(posInfo.x*Screen.width , (posInfo.y + offsetInfo*0f )*Screen.height, posInfo.width*Screen.width, posInfo.height*Screen.height), theSong.numberOfSteps + " Steps", "infosong");
			//Number of jumps						   
			GUI.Label(new Rect(posInfo.x*Screen.width , (posInfo.y + offsetInfo*1f )*Screen.height, posInfo.width*Screen.width, posInfo.height*Screen.height), theSong.numberOfJumps + " Jumps", "infosong");
			//Number of hands						   
			GUI.Label(new Rect(posInfo.x*Screen.width , (posInfo.y + offsetInfo*2f )*Screen.height, posInfo.width*Screen.width, posInfo.height*Screen.height), theSong.numberOfHands + " Hands", "infosong");
			//Number of mines						  
			GUI.Label(new Rect(posInfo2.x*Screen.width , (posInfo2.y + offsetInfo*0f )*Screen.height, posInfo2.width*Screen.width, posInfo2.height*Screen.height), theSong.numberOfMines + " Mines", "infosong");
			//Number of freeze							
			GUI.Label(new Rect(posInfo2.x*Screen.width , (posInfo2.y + offsetInfo*1f )*Screen.height, posInfo2.width*Screen.width, posInfo2.height*Screen.height), theSong.numberOfFreezes + " Freezes", "infosong");
			//Number of rolls						   
			GUI.Label(new Rect(posInfo2.x*Screen.width , (posInfo2.y + offsetInfo*2f )*Screen.height, posInfo2.width*Screen.width, posInfo2.height*Screen.height), theSong.numberOfRolls + " Rolls", "infosong");
			//Number of cross						    
			GUI.Label(new Rect(posInfo3.x*Screen.width , (posInfo3.y + offsetInfo*2f )*Screen.height, posInfo3.width*Screen.width, posInfo3.height*Screen.height), theSong.numberOfCross + " Cross pattern", "infosong");
			//Number of footswitch						
			GUI.Label(new Rect(posInfo3.x*Screen.width , (posInfo3.y + offsetInfo*3f )*Screen.height, posInfo3.width*Screen.width, posInfo3.height*Screen.height), theSong.numberOfFootswitch + " Footswitch pat.", "infosong");
			//Average Intensity					   	
			//GUI.Label(new Rect(posInfo.x*Screen.width , (posInfo.y + offsetInfo*8f )*Screen.height, posInfo.width*Screen.width, posInfo.height*Screen.height), "Av. Intensity : " + songSelected[(Difficulty)actualySelected].stepPerSecondAverage.ToString("0.00") + " SPS", "infosong");
			
			//Max Intensity					
			GUI.Label(new Rect(posMaxinten.x*Screen.width , (posMaxinten.y)*Screen.height, posMaxinten.width*Screen.width, posMaxinten.height*Screen.height), "Max : " + theSong.stepPerSecondMaximum.ToString("0.00") + " SPS ("+ ((System.Convert.ToDouble(theSong.stepPerSecondMaximum)/4f)*60f).ToString("0") + " BPM)", "infosong");
			//Longest Stream (TimePerSecond)		    
			GUI.Label(new Rect(posInfo3.x*Screen.width , (posInfo3.y + offsetInfo*1f)*Screen.height, posInfo3.width*Screen.width, posInfo3.height*Screen.height), System.Convert.ToDouble(theSong.stepPerSecondStream) < 8f ? "No stream" : "Max stream : " + theSong.longestStream.ToString("0.00") + " sec (" + theSong.stepPerSecondStream.ToString("0.00") + " SPS)", "infosong");
			//Number of BPM change						
			GUI.Label(new Rect(posInfo4.x*Screen.width , (posInfo4.y + offsetInfo*0f)*Screen.height, posInfo4.width*Screen.width, posInfo4.height*Screen.height), theSong.bpms.Count - 1 + " BPM changes", "infosong");
			//Number of stops						    
			GUI.Label(new Rect(posInfo4.x*Screen.width , (posInfo4.y + offsetInfo*1f)*Screen.height, posInfo4.width*Screen.width, posInfo4.height*Screen.height), theSong.stops.Count + " Stops", "infosong");
		
			//Personnal best					
			GUI.Label(new Rect(posInfo5.x*Screen.width , (posInfo5.y + offsetInfo*0f)*Screen.height, posInfo5.width*Screen.width, posInfo5.height*Screen.height), score == -1 ? "Never scored" : "Best : " + score.ToString("0.00") + "%" + (isScoreFail ? " (Fail)" : "") , "infosong");
			//Friend best :					    
			GUI.Label(new Rect(posInfo5.x*Screen.width , (posInfo5.y + offsetInfo*1f)*Screen.height, posInfo5.width*Screen.width, posInfo5.height*Screen.height), bestfriendscore == -1 ? "No friend scored" : "Record : " + bestfriendscore.ToString("0.00") + "%", "infosong");
			//Friend name :					    
			GUI.Label(new Rect(posInfo5.x*Screen.width , (posInfo5.y + offsetInfo*2f)*Screen.height, posInfo5.width*Screen.width, posInfo5.height*Screen.height), bestnamefriendscore == "-" ? "" : "Record owner : " + bestnamefriendscore, "infosong");
		
			if(score != -1 && score < 96f){
				if(!isScoreFail){
					GUI.DrawTexture(new Rect(posNote.x*Screen.width, posNote.y*Screen.height, posNote.width*Screen.width, posNote.height*Screen.height), 
						gs.tex[DataManager.Instance.giveNoteOfScore((float)score).Split(';')[1]]);
			
				}else{
					GUI.DrawTexture(new Rect(posSpecialNote.x*Screen.width, posSpecialNote.y*Screen.height, posSpecialNote.width*Screen.width, posSpecialNote.height*Screen.height), 
						gs.tex["Failed"]);
				}
			}
			
		}
		
	}
	
	void displayGraph(){
		var thesong = gs.songSelected[actualySelected];
		for(int i=0;i<100;i++){
			var thepos = new Vector3(departGraphX + (topGraphX - departGraphX)*((float)i/100f), departGraphY + (topGraphY - departGraphY)*((float)thesong.intensityGraph[i]/(float)thesong.stepPerSecondMaximum), 2f);
			graph.SetPosition(i, thepos);	
		}
			
	}
	
	public void refreshDifficultyDisplayed()
	{
		refreshNumberDiff();
		activeDiff();
		PSDiff[(int)actualySelected].gameObject.active = false;
		activeDiffPS();
		PSDiff[(int)actualySelected].gameObject.active = true;
		displayGraph();
		verifyScore();
		graph.enabled = true;
	}
	
	public void disableDifficultyDisplayed()
	{
		graph.enabled = false;
		PSDiff[(int)actualySelected].gameObject.active = false;
		desactiveDiff();
	}
	
	public Difficulty getActualySelected()
	{
		return actualySelected;	
	}
	
	public void activeDiffPS(){
		if(gs.songSelected.ContainsKey(trulySelected)){
			actualySelected = trulySelected;	
		}else{
			var min = 99;
			var mini = (int)trulySelected;
			for(int i=(int)Difficulty.BEGINNER; i<=(int)Difficulty.EDIT; i++){
				if(gs.songSelected.ContainsKey((Difficulty)i)){
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
	
	public void activeDiff(){
		var countpos = 0;
		for(int i=(int)Difficulty.BEGINNER; i<=(int)Difficulty.EDIT; i++){
			if(gs.songSelected.ContainsKey((Difficulty)i)){
				
				diffSelected[(Difficulty)i].transform.position = new Vector3(diffSelected[(Difficulty)i].transform.position.x, DataManager.Instance.posYDiff[countpos], diffSelected[(Difficulty)i].transform.position.z);
				PSDiff[i].transform.position = new Vector3(PSDiff[i].transform.position.x, DataManager.Instance.posYZoneDiff[countpos], PSDiff[i].transform.position.z);
				RayDiff[i].transform.position = new Vector3(RayDiff[i].transform.position.x, DataManager.Instance.posYZoneDiff[countpos], RayDiff[i].transform.position.z);
				countpos++;
				for(int j=0; j<diffSelected[(Difficulty)i].transform.GetChildCount(); j++){
					if((int.Parse(diffSelected[(Difficulty)i].transform.GetChild(j).name)) <= gs.songSelected[(Difficulty)i].level){
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
	
	public void desactiveDiff(){
		for(int i=(int)Difficulty.BEGINNER; i<=(int)Difficulty.EDIT; i++){
			RayDiff[i].transform.Translate(0f, -100f, 0f);
			diffSelected[(Difficulty)i].transform.Translate(0f, -100f, 0f);
		}
	}
	
	public void refreshNumberDiff(){
		for(int i=(int)Difficulty.BEGINNER; i<=(int)Difficulty.EDIT; i++){
			if(gs.songSelected.ContainsKey((Difficulty)i)){
				
				diffNumber[i] = gs.songSelected[(Difficulty)i].level;
				
			}else{
				diffNumber[i] = 0;
			}
		}
	}
	
	public void verifyScore(){
		var oldscore = score;
		
		var result = gs.refreshPreference(score).Split(';');
		score = System.Convert.ToDouble(result[0]);
		bestfriendscore = System.Convert.ToDouble(result[1]);
		bestnamefriendscore = result[2];
		isScoreFail = result[3] == "1";
		if(DataManager.Instance.giveNoteOfScore((float)score) != DataManager.Instance.giveNoteOfScore((float)oldscore) && oldscore >= 96f){
			medals.FirstOrDefault(c => c.name == DataManager.Instance.giveNoteOfScore((float)oldscore).Split(';')[1]).SetActiveRecursively(false);
		}
		if(score >= 96f){
			medals.FirstOrDefault(c => c.name == DataManager.Instance.giveNoteOfScore((float)score).Split(';')[1]).SetActiveRecursively(true);
		}
		
	}
	
	
	public void onEnterOption()
	{
		enterOption = true;
		PSDiff[(int)actualySelected].gameObject.active = false;
		for(int i=0;i<RayDiff.Count;i++){
			RayDiff[i].active = false;	
		}
		recoverPosition = diffSelected[actualySelected].transform.position;
		graph.enabled = false;
		activeModule = false;
	}
	
	public void onExitOption()
	{
		exitOption = true;
		PSDiff[(int)actualySelected].gameObject.active = true;
		for(int i=0;i<RayDiff.Count;i++){
			RayDiff[i].active = true;	
		}
		graph.enabled = true;
		activeModule = true;
	}
	
	public void onEnterLaunch()
	{
		enterLaunch = true;
		PSDiff[(int)actualySelected].gameObject.active = false;
		for(int i=0;i<RayDiff.Count;i++){
			RayDiff[i].active = false;	
		}
		for(int i=0;i<diffSelected.Count;i++){
			if(i != (int)actualySelected)
			{
				diffSelected.ElementAt(i).Value.transform.Translate(0f, -200f, 0f);
			}
		}
		graph.enabled = false;
		activeModule = false;
	}
	
	public double getScore()
	{
		return score;	
	}
	
	public double getBestFriendScore()
	{
		return bestfriendscore;
	}
	
	public string getBestFriendName()
	{
		return bestnamefriendscore;	
	}
	
	public bool isFail()
	{
		return isScoreFail;
	}
	
}
