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
	}
	
	// Update is called once per frame
	void Update () {
	
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
