using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PackZone : MonoBehaviour {
	
	public GameObject miniCubePack;
	public Camera camerapack;
	
	private Dictionary<GameObject, string> packs;
	
	//PackList
	private int numberPack;
	private int nextnumberPack;
	public Transform[] packpos; //pos base = new Vector3(0f, 13, 20f)
	public Vector2 posYModule; //x : low, Y : high
	public float speedPop;
	public float decalYLabel;
	public float decalXLabel;
	public float wd;
	public float ht;
	public Rect posBackward;
	public Rect posForward;
	public float ecart;
	public float speedMove;
	public float limite;
	
	private float fadeAlpha;
	
	private bool movinForward;
	private bool movinBackward;
	
	private bool activeModule;
	// Use this for initialization
	void Start () {
		
		activeModule = true;
		numberPack = 0;
		nextnumberPack = 0;
		
		
		packs = new Dictionary<GameObject, string>();
		
		var tempPos = new List<GameObject>();
		var tempPack = new Dictionary<GameObject, string>();
		var position = 0;
		var thePackPosition = -1;
		
		foreach(var el in LoadManager.Instance.ListSong().Keys){
			var thego = (GameObject) Instantiate(miniCubePack, new Vector3(0f, 13f, 20f), miniCubePack.transform.rotation);
			if(LoadManager.Instance.ListTexture().ContainsKey(el)) thego.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[el];
			if(tempPack.ContainsKey(el)){ 
				tempPack.Add(thego, el + "(" + tempPack.Count + ")");	
				if(el + "(" + tempPack.Count + ")" == DataManager.Instance.packSelected) thePackPosition = position; 
			}
			else
			{ 
				tempPack.Add(thego, el); 
				if(el == DataManager.Instance.packSelected) thePackPosition = position;
			}
			position++;
		}

		if(DataManager.Instance.packSelected != "" && thePackPosition != -1){
			for(int i=thePackPosition;i<tempPos.Count;i++){
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
			for(int i=0;i<thePackPosition;i++){
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
		}else{
			for(int i=0;i<tempPos.Count;i++){
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
		}
		
		organiseCube(0);
		
		activePack(packs.ElementAt(0).Key);
		
		fadeAlpha = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		//Move forward pack
		if(movinForward){
			
			if(moveCube(Time.deltaTime/speedMove)){
				movinForward = false;
				fadeAlpha = 0.5f;
				decreaseCubeF();
				fadeAlpha = 0f;
				numberPack = nextnumberPack;
				organiseCube(numberPack);
			}else{
				decreaseCubeF();
			}	
		}
		
		//Move backward pack
		if(movinBackward){
			if(moveCube(Time.deltaTime/speedMove)){
				movinBackward = false;	
				decalFade = 0f;
				decalFadeM = 0f;
				fadeAlpha = 0.5f;
				decreaseCubeB();
				fadeAlpha = 0f;
				numberPack = nextnumberPack;
				organiseCube(numberPack);
			}else{
				decreaseCubeB();
			}
		}
	}
	
	void OnGUI()
	{
		if(!LoadManager.Instance.isAllowedToSearch(search)){
			for(int i=0; i < packs.Count; i++)
			{
				if(packs.Count <= 7 || (i >= PrevInt(numberPack, 3) && i <= NextInt(numberPack, 3)))
				{
					var point2D = camerapack.WorldPointToScreen(packs.ElementAt(i).Key.transform.position);
					GUI.Color = packs.ElementAt(i).Key.renderer.material.color;
					GUI.Label(new Rect(point2D.x + (decalXLabel*Screen.width), point2D.y + (decalXLabel*Screen.width), wd*Screen.width, ht*Screen.height), packs.ElementAt(i).Value);
					
				}
			}
			GUI.Color = new Color(1f, 1f, 1f, 1f);
		
			if(activeModule)
			{
				if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","LBackward") && !movinBackward && !movinForward){
					nextnumberPack = PrevInt(numberPack, 1);
					organiseCube(numberPack);
					movinBackward = true;
				}
				if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height + ecart*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","Backward") && !movinBackward && !movinForward ){
					nextnumberPack = PrevInt(numberPack, 3);
					organiseCube(nextnumberPack);
					numberPack = nextnumberPack;
				}
				
				if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","LForward") && !movinBackward && !movinForward)
				{
					nextnumberPack = NextInt(numberPack, 1);
					organiseCube(numberPack);
					movinForward = true;
				}
				if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height + ecart*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","Forward") && !movinBackward && !movinForward){
					nextnumberPack = NextInt(numberPack, 3);
					organiseCube(nextnumberPack);
					numberPack = nextnumberPack;
				}
			}
		}
	}
	
	
	void organiseCube(int start){
		
	
		packs.ElementAt(start).Key.transform.position = packpos[2];
		packs.ElementAt(start).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		
		packs.ElementAt(NextInt(start, 1)).Key.transform.position = packpos[3];
		packs.ElementAt(NextInt(start, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
		
		packs.ElementAt(NextInt(start, 2)).Key.transform.position = packpos[4];
		packs.ElementAt(NextInt(start, 2)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		packs.ElementAt(PrevInt(start, 1)).Key.transform.position = packpos[1];
		packs.ElementAt(PrevInt(start, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f);
		
		packs.ElementAt(PrevInt(start, 2)).Key.transform.position = packpos[0];
		packs.ElementAt(PrevInt(start, 2)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		
		for(int i=0; i<packs.Count;i++){
			var reverse = (i < PrevInt(numberPack, 2) || i > NextInt(numberPack, 2);
			if((i < PrevInt(numberPack, 2) || i > NextInt(numberPack, 2))
			{
				packs.ElementAt(i).Key.transform.position = packpos[5]; //out
				packs.ElementAt(i).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
			}
			
		}
	}
	
	bool moveCube(float speed, float limit){
		
	
		packs.ElementAt(PrevInt(nextnumberpack, 2)).transform.position = Vector3.Lerp(packs.ElementAt(PrevInt(nextnumberpack, 2)).transform.position, packpos[0], speed);
		
		packs.ElementAt(PrevInt(nextnumberpack, 1)).transform.position = Vector3.Lerp(packs.ElementAt(PrevInt(nextnumberpack, 1)).transform.position, packpos[1], speed);
		
		packs.ElementAt(nextnumberpack).transform.position = Vector3.Lerp(packs.ElementAt(nextnumberPack).transform.position, packpos[2], speed);
		
		packs.ElementAt(NextInt(nextnumberpack, 1)).transform.position = Vector3.Lerp(packs.ElementAt(NextInt(nextnumberpack, 1)).transform.position, packpos[3], speed);
		
		packs.ElementAt(NextInt(nextnumberpack, 2)).transform.position = Vector3.Lerp(packs.ElementAt(NextInt(nextnumberpack, 1)).transform.position, packpos[4], speed);
		
		return Math.Abs(packs.ElementAt(nextnumberpack).transform.position.x - packpos[2].x) <= limit;
	}
	
	
	void decreaseCubeF(){
		
		//Color
		fadeAlpha = Mathf.Lerp(fadeAlpha, 0.5f, Time.deltaTime/speedMove);
		packs.ElementAt(numberPack).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f - fadeAlpha);
		packs.ElementAt(NextInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f + fadeAlpha);
		packs.ElementAt(NextInt(numberPack, 2)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f + fadeAlpha);
		packs.ElementAt(PrevInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f - fadeAlpha);
		
		
		
	}
	
	
	
	void decreaseCubeB(){
		
		fadeAlpha = Mathf.Lerp(fadeAlpha, 0.5f, Time.deltaTime/speedMove);
		packs.ElementAt(numberPack).Key.renderer.material.color = new Color(1f, 1f, 1f, 1f - fadeAlpha);
		packs.ElementAt(NextInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f - fadeAlpha);
		packs.ElementAt(PrevInt(numberPack, 1)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0.5f + fadeAlpha);
		packs.ElementAt(PrevInt(numberPack, 2)).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f + fadeAlpha);
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
	
	
	
	public void popin()
	{
		for(int i=0; i < packs.Count; i++)
		{
			packs.ElementAt(i).Key.transform.position = Vector3.Lerp(packs.ElementAt(i).Key.transform.position, new Vector3(packs.ElementAt(i).Key.transform.position.x, posYModule.x, packs.ElementAt(i).Key.transform.position.z), Time.deltaTime/speedPop);
		}
	
	}
	
	
	public void popout()
	{
		for(int i=0; i < packs.Count; i++)
		{
			packs.ElementAt(i).Key.transform.position = Vector3.Lerp(packs.ElementAt(i).Key.transform.position, new Vector3(packs.ElementAt(i).Key.transform.position.x, posYModule.y, packs.ElementAt(i).Key.transform.position.z), Time.deltaTime/speedPop);
		}
	
	}
	
	
	void activePack(){
		GetComponent<GeneralScript>().getSongPack().setSongList(LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberpack).Value]);
	}
	
	
}
