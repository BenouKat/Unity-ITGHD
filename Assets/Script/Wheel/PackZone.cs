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
	public Vector3[] packpos = new Vector3[6]{new Vector3(-30f, 13f, 20f), new Vector3(-15f, 13f, 20f), new Vector3(0f, 13f, 20f),
	new Vector3(15f, 13f, 20f), new Vector3(30f, 13f, 20f), new Vector3(50f, 13f, 20f)};
	public Vector2 posYModule = new Vector2(0f, -30f);
	public float speedPop = 3f;
	public float decalYLabel = 0.05f;
	public float decalXLabel = -0.075f;
	public float wd = 0.15f;
	public float ht = 0.2f;
	public Rect posBackward = new Rect(0.01f, 0.02f, 0.05f, 0.075f);
	public Rect posForward = new Rect(0.94f, 0.02f, 0.05f, 0.075f);
	public float ecart = 0.1f;
	public float speedMove = 0.05f;
	public float limite = 0.1f;
	private bool popin;
	private bool popout;
	
	private float fadeAlpha;
	
	private bool movinForward;
	private bool movinBackward;
	
	private GeneralScript gs;
	private bool activeModule;
	// Use this for initialization
	void Start () {
		
		if(!LoadManager.Instance.alreadyLoaded) ProfileManager.Instance.CreateTestProfile();
		if(!LoadManager.Instance.alreadyLoaded) TextManager.Instance.LoadTextFile();
		if(!LoadManager.Instance.alreadyLoaded) LoadManager.Instance.Loading();
		
		gs = GetComponent<GeneralScript>();
		popin = false;
		popout = false;
		activeModule = true;
		numberPack = 0;
		nextnumberPack = 0;
		
		
		packs = new Dictionary<GameObject, string>();
		
		var tempPack = new Dictionary<GameObject, string>();
		var position = 0;
		var thePackPosition = -1;
		
		foreach(var el in LoadManager.Instance.ListSong().Keys){
			var thego = (GameObject) Instantiate(miniCubePack, new Vector3(0f, 13f, 20f), miniCubePack.transform.rotation);
			thego.SetActive(true);
			if(LoadManager.Instance.ListTexture().ContainsKey(el)) thego.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[el];
			tempPack.Add(thego, el); 
			if(el == DataManager.Instance.packSelected) thePackPosition = position;
			
			position++;
		}
		

		if(DataManager.Instance.packSelected != "" && thePackPosition != -1){
			for(int i=thePackPosition;i<tempPack.Count;i++){
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
			for(int i=0;i<thePackPosition;i++){
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
		}else{
			for(int i=0;i<tempPack.Count;i++){
				packs.Add(tempPack.ElementAt(i).Key, tempPack.ElementAt(i).Value);
			}
		}
		
		organiseCube(0);
		
		fadeAlpha = 0f;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		//Move forward pack
		if(movinForward){
			
			if(moveCube(Time.deltaTime/speedMove, limite)){
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
			if(moveCube(Time.deltaTime/speedMove, limite)){
				movinBackward = false;	
				fadeAlpha = 0.5f;
				decreaseCubeB();
				fadeAlpha = 0f;
				numberPack = nextnumberPack;
				organiseCube(numberPack);
			}else{
				decreaseCubeB();
			}
		}
		
		if(popin)
		{
			camerapack.transform.position = Vector3.Lerp(camerapack.transform.position, new Vector3(camerapack.transform.position.x, posYModule.x, camerapack.transform.position.z), speedPop*Time.deltaTime);
			
			if(Math.Abs(camerapack.transform.position.y - posYModule.x) <= limite)
			{
				popin = false;	
				camerapack.transform.position = new Vector3(camerapack.transform.position.x, posYModule.x, camerapack.transform.position.z);
			}
		}
		
		if(popout)
		{
			camerapack.transform.position = Vector3.Lerp(camerapack.transform.position, new Vector3(camerapack.transform.position.x, posYModule.y, camerapack.transform.position.z), speedPop*Time.deltaTime);
			
			if(Math.Abs(camerapack.transform.position.y - posYModule.y) <= limite)
			{
				popout = false;	
				camerapack.transform.position = new Vector3(camerapack.transform.position.x, posYModule.y, camerapack.transform.position.z);	
			}
		}
	}
	
	public void GUIModule()
	{
		if(activeModule){
			for(int i=0; i < packs.Count; i++)
			{
				if(packs.Count <= 7 || (i >= PrevInt(numberPack, 3) && i <= NextInt(numberPack, 3)))
				{
					var point2D = camerapack.WorldToScreenPoint(packs.ElementAt(i).Key.transform.position);
					point2D.y = Screen.height - point2D.y;
					GUI.color = packs.ElementAt(i).Key.renderer.material.color;
					GUI.Label(new Rect(point2D.x + (decalXLabel*Screen.width), point2D.y + (decalYLabel*Screen.height), wd*Screen.width, ht*Screen.height), packs.ElementAt(i).Value);
					
				}
			}
			GUI.color = new Color(1f, 1f, 1f, 1f);
		
			if(activeModule)
			{
				if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","LBackward") && !movinBackward && !movinForward){
					nextnumberPack = PrevInt(numberPack, 1);
					organiseCube(numberPack);
					setActivePack();
					if(!gs.getZoneSong().locked)
					{
						gs.refreshPackBanner();
						gs.refreshBanner();
					}
					movinBackward = true;
				}
				if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height + ecart*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","Backward") && !movinBackward && !movinForward ){
					nextnumberPack = PrevInt(numberPack, 3);
					organiseCube(nextnumberPack);
					setActivePack();
					if(!gs.getZoneSong().locked)
					{
						gs.refreshPackBanner();
						gs.refreshBanner();
					}
					numberPack = nextnumberPack;
				}
				
				if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","LForward") && !movinBackward && !movinForward)
				{
					nextnumberPack = NextInt(numberPack, 1);
					organiseCube(numberPack);
					setActivePack();
					if(!gs.getZoneSong().locked)
					{
						gs.refreshPackBanner();
						gs.refreshBanner();
					}
					movinForward = true;
				}
				if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height + ecart*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","Forward") && !movinBackward && !movinForward){
					nextnumberPack = NextInt(numberPack, 3);
					organiseCube(nextnumberPack);
					setActivePack();
					if(!gs.getZoneSong().locked)
					{
						gs.refreshPackBanner();
						gs.refreshBanner();
					}
					numberPack = nextnumberPack;
				}
			}
		}
	}
	
	
	void organiseCube(int start){
		
		//Debug.Log("start : " + start + " // size pack : " + packs.Count + " // size pos : " + packpos.Length);
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
		
		if(packs.Count > 5)
		{
			for(int i=0; i<packs.Count;i++){
				var reverse = PrevInt(start, 2) > NextInt(start, 2);
				if((!reverse && (i < PrevInt(start, 2) || i > NextInt(start, 2))) || (reverse && (i < PrevInt(start, 2) && i > NextInt(start, 2))))
				{
					packs.ElementAt(i).Key.transform.position = packpos[5]; //out
					packs.ElementAt(i).Key.renderer.material.color = new Color(1f, 1f, 1f, 0f);
				}
				
			}
		}
	}
	
	bool moveCube(float speed, float limit){
		
	
		packs.ElementAt(PrevInt(nextnumberPack, 2)).Key.transform.position = Vector3.Lerp(packs.ElementAt(PrevInt(nextnumberPack, 2)).Key.transform.position, packpos[0], speed);
		
		packs.ElementAt(PrevInt(nextnumberPack, 1)).Key.transform.position = Vector3.Lerp(packs.ElementAt(PrevInt(nextnumberPack, 1)).Key.transform.position, packpos[1], speed);
		
		packs.ElementAt(nextnumberPack).Key.transform.position = Vector3.Lerp(packs.ElementAt(nextnumberPack).Key.transform.position, packpos[2], speed);
		
		packs.ElementAt(NextInt(nextnumberPack, 1)).Key.transform.position = Vector3.Lerp(packs.ElementAt(NextInt(nextnumberPack, 1)).Key.transform.position, packpos[3], speed);
		
		packs.ElementAt(NextInt(nextnumberPack, 2)).Key.transform.position = Vector3.Lerp(packs.ElementAt(NextInt(nextnumberPack, 1)).Key.transform.position, packpos[4], speed);
		
		return Math.Abs(packs.ElementAt(nextnumberPack).Key.transform.position.x - packpos[2].x) <= limit;
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
	
	
	
	public void onPopin()
	{
		popin = true;
		popout = false;	
		activeModule = true;
	}
	
	
	public void onPopout()
	{
		popout = true;
		popin = false;
		activeModule = false;
		
	}
	
	
	public string getActivePack(){
		return packs.ElementAt(nextnumberPack).Value;
	}
	
	public void setActivePack(){
		
		GetComponent<GeneralScript>().getZoneSong().activeSongList(packs.ElementAt(nextnumberPack).Value);	
	}
	
	
}
