using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PackZone : MonoBehaviour {
	
	public GameObject miniCubePack;
	public Camera camerapack;
	
	private Dictionary<string, GameObject> packs;
	private Dictionary<GameObject, float> cubesPos;
	
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
	
	private bool movinForward;
	private bool movinBackward;
	private bool movinForwardFast;
	private bool movinBackwardFast;
	// Use this for initialization
	void Start () {
		numberPack = 0;
		nextnumberPack = 0;
		
		
		packs = new Dictionary<string, GameObject>();
		cubesPos = new Dictionary<GameObject, float>();
		
		var tempPos = new Dictionary<GameObject, float>();
		var tempPack = new Dictionary<string, GameObject>();
		var position = 0;
		var thePackPosition = -1;
		foreach(var el in LoadManager.Instance.ListSong().Keys){
			var thego = (GameObject) Instantiate(miniCubePack, new Vector3(0f, 13f, 20f), miniCubePack.transform.rotation);
			if(LoadManager.Instance.ListTexture().ContainsKey(el)) thego.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[el];
			if(tempPack.ContainsKey(el)){ 
				tempPack.Add(el + "(" + tempPack.Count + ")", thego);	
				if(el + "(" + tempPack.Count + ")" == DataManager.Instance.packSelected) thePackPosition = position; 
			}
			else
			{ 
				tempPack.Add(el, thego); 
				if(el == DataManager.Instance.packSelected) thePackPosition = position;
			}
			tempPos.Add(thego, 0f);
			position++;
		}

		if(DataManager.Instance.packSelected != "" && thePackPosition != -1){
			for(int i=thePackPosition;i<tempPos.Count;i++){
				cubesPos.Add(tempPos.ElementAt(i).Key, tempPos.ElementAt(i).Value);
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
			for(int i=0;i<thePackPosition;i++){
				cubesPos.Add(tempPos.ElementAt(i).Key, tempPos.ElementAt(i).Value);
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
		}else{
			for(int i=0;i<tempPos.Count;i++){
				cubesPos.Add(tempPos.ElementAt(i).Key, tempPos.ElementAt(i).Value);
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
		}
		
		organiseCube();
		
		activePack(packs.ElementAt(0).Key);
	}
	
	// Update is called once per frame
	void Update () {
	
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
	
	void activeCustomPack(){
		foreach(var el in customSongCubePack){
			if(el.Key.transform.position.y > - 3f*numberToDisplay){
				el.Key.SetActiveRecursively(true);
				el.Key.transform.FindChild("Selection").gameObject.active = false;
			}else if(el.Key.active){
				el.Key.SetActiveRecursively(false);
			}
		}
	}
	
	void desactivePack(){
		foreach(var el in songCubePack){
			if(el.Key.active){
				el.Key.SetActiveRecursively(false);
			}
		}
	}
	
	
}
