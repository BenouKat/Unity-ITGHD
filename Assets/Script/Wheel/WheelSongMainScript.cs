using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class WheelSongMainScript : MonoBehaviour {
	
	
	public GameObject miniCubePack;
	public Camera camerapack;
	public GUISkin skin;
	public GameObject cubeSong;
	
	private int numberPack;
	private int nextnumberPack;
	private Dictionary<string, GameObject> packs;
	private Dictionary<GameObject, float> cubesPos;
	private Dictionary<GameObject, string> songCubePack;
	private Dictionary<Difficulty, Song> songSelected;
	public float x10;
	public float xm10;
	public float y;
	public float wd;
	public float ht;
	
	public Rect posBackward;
	public Rect posForward;
	public float ecart;
	
	
	public Rect posSonglist;
	public float ecartSong;
	private GameObject particleOnPlay;
	public int numberToDisplay;
	public int startnumber;
	public float speedCameraDefil;
	private float posLabel;
	
	private bool locked;
	
	private bool movinForward;
	private bool movinBackward;
	private bool movinForwardFast;
	private bool movinBackwardFast;
	public float speedMove;
	public float limite;
	
	private float decalFade;
	private float decalFadeM;
	private float fadeAlpha;
	// Use this for initialization
	void Start () {
		numberPack = 0;
		nextnumberPack = 0;
		startnumber = 0;
		packs = new Dictionary<string, GameObject>();
		cubesPos = new Dictionary<GameObject, float>();
		songCubePack = new Dictionary<GameObject, string>();
		LoadManager.Instance.Loading();
		while(packs.Count() < 5){
			foreach(var el in LoadManager.Instance.ListSong().Keys){
				var thego = (GameObject) Instantiate(miniCubePack, new Vector3(0f, 13f, 20f), miniCubePack.transform.rotation);
				thego.renderer.material.mainTexture = LoadManager.Instance.ListTexture()[el];
				packs.Add(el, thego);	
				cubesPos.Add(thego, 0f);
			}
		}
		organiseCube();
		createCubeSong();
		activePack(packs.ElementAt(0).Key);
		decalFade = 0f;
		decalFadeM = 0f;
		fadeAlpha = 0f;
		goalDefil = 2f;
		posLabel = 0f;
		locked = false;
	}
	
	// Update is called once per frame
	public void OnGUI () {
		GUI.skin = skin;
		
		//PACKS
		var decal = ((Screen.width - wd*Screen.width)/2);
		if(movinBackward) GUI.Label(new Rect((xm10*2f + decalFadeM)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(PrevInt(numberPack, 2)).Key);
		if(!movinForward) GUI.Label(new Rect((xm10 + decalFadeM)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(PrevInt(numberPack, 1)).Key);
		GUI.Label(new Rect(decalFade > 0 ? decalFade*Screen.width + decal : decalFadeM*Screen.width + decal  , y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(numberPack).Key);
		if(!movinBackward) GUI.Label(new Rect((x10 + decalFade)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(NextInt(numberPack, 1)).Key);
		if(movinForward) GUI.Label(new Rect((x10*2f + decalFade)*Screen.width + decal, y*Screen.height, wd*Screen.width, ht*Screen.height), packs.ElementAt(NextInt(numberPack, 2)).Key);
		
		
		
		if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","LBackward") && !movinBackward){
			nextnumberPack = PrevInt(numberPack, 1);
			activePack(packs.ElementAt(nextnumberPack).Key);
			movinBackward = true;
		}
		if(GUI.Button(new Rect(posBackward.x*Screen.width, posBackward.y*Screen.height + ecart*Screen.height, posBackward.width*Screen.width, posBackward.height*Screen.height),"","Backward")){
			nextnumberPack = PrevInt(numberPack, 3);
			activePack(packs.ElementAt(nextnumberPack).Key);
			movinBackwardFast = true;
		}
		
		if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","LForward") && !movinForward)
		{
			nextnumberPack = NextInt(numberPack, 1);
			activePack(packs.ElementAt(nextnumberPack).Key);
			movinForward = true;
		}
		if(GUI.Button(new Rect(posForward.x*Screen.width, posForward.y*Screen.height + ecart*Screen.height, posForward.width*Screen.width, posForward.height*Screen.height),"","Forward")){
			nextnumberPack = NextInt(numberPack, 3);
			activePack(packs.ElementAt(nextnumberPack).Key);
			movinForwardFast = true;
		}
		
		
		//SONGS
		 
		var thepos = -posLabel;
		for(int i=0; i<LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].Count; i++){
			if(thepos >= 0f && thepos <= numberToDisplay){
				
				var el = LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].ElementAt(i);
				GUI.color = new Color(0f, 0f, 0f, 1f);
				GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.title, "songlabel");
				GUI.color = new Color(1f, 1f, 1f, 1f);
				GUI.Label(new Rect(posSonglist.x*Screen.width, (posSonglist.y + ecartSong*thepos)*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.title, "songlabel");
			}else if(thepos > -1f && thepos < 0f){
				var el = LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].ElementAt(i);
				GUI.color = new Color(0f, 0f, 0f, 1f + thepos);
				GUI.Label(new Rect(posSonglist.x*Screen.width +1f , (posSonglist.y + ecartSong*thepos)*Screen.height +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.title, "songlabel");
				GUI.color = new Color(1f, 1f, 1f, 1f + thepos);
				GUI.Label(new Rect(posSonglist.x*Screen.width, (posSonglist.y + ecartSong*thepos)*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), el.Value.First().Value.title, "songlabel");
			}
			thepos++;
		}
		
		
	}
	
	void Update(){
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
		
		
		
		
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
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
						songSelected = LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].FirstOrDefault(c => c.Value.First().Value.title == songCubePack[papa.gameObject]);
						particleOnPlay.active = false;
					}
					particleOnPlay = thepart;
					particleOnPlay.active = true;
				}
				
				if(Input.GetMouseDown(0)){
					
					if(locked){
						var papa = theGo.transform.parent;
						var thepart = papa.Find("Selection").gameObject;
						if(particleOnPlay != null && particleOnPlay != thepart && particleOnPlay.active) 
						{
							songSelected = LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key].FirstOrDefault(c => c.Value.First().Value.title == songCubePack[papa.gameObject]);
							particleOnPlay.active = false;
						}
						particleOnPlay = thepart;
						particleOnPlay.active = true;
					}else{
						locked = true;
					}
				}
				
				if(Input.GetMouseDown(1)){
					locked = false;
				}
				
			}else if(particleOnPlay != null && particleOnPlay.active){
				
				particleOnPlay.active = false;
			}
		}else if(particleOnPlay != null && particleOnPlay.active){
			particleOnPlay.active = false;
		}
		
		
		
		
		if(Input.GetAxis("Mouse ScrollWheel") < 0 && startnumber > 0){
			startnumber++;
			
			
		}else if(Input.GetAxis("Mouse ScrollWheel") > 0 && startnumber < songCubePack.Count - numberToDisplay){
			startnumber--;
		}
		
		
		if(Mathf.Abs(camerapack.transform.position.x - 2f - 3f*startnumber) <= 0.01f){
			camerapack.transform.position = new Vector3(2f - 3f*startnumber, camerapack.transform.position.y, camerapack.transform.position.z);
			posLabel = startnumber;
		}else{
			Vector3.Lerp(camerapack.transform.position,Vector3(2f - 3f*startnumber, camerapack.transform.position.y, camerapack.transform.position.z), Time.deltaTime/speedCameraDefil);
			Mathf.Lerp(posLabel, startnumber, Time.deltaTime/speedCameraDefil);
			foreach(var cubeel in songCubePack.Where(c => c.Key.active && c.Key.transform.position.x < camerapack.transform.position.x + 2f){
				cubeel.Key.setActiveRecursivly(false);
			}
			foreach(var cubeel in songCubePack.Where(c => !c.Key.active && c.Key.transform.position.x < camerapack.transform.position.x + 2f){
				cubeel.Key.setActiveRecursivly(true);
				cubeel.Key.transform.FindChild("Selection").gameObject.active = false;
			}
		}
		
	}
	
	
	
	void createCubeSong(){
		
		foreach(var el in LoadManager.Instance.ListSong()){
			var pos = 2f;
			foreach(var song in el.Value){
				var thego = (GameObject) Instantiate(cubeSong, new Vector3(-25f, pos, 0f), cubeSong.transform.rotation);
				pos -= 3f;
				thego.SetActiveRecursively(false);
				songCubePack.Add(thego,el.Key);
			}
		}
	}
	
	
	void activePack(string s){
		foreach(var el in songCubePack){
			if(el.Value == s){
				el.Key.SetActiveRecursively(true);
				el.Key.transform.FindChild("Selection").gameObject.active = false;
			}else if(el.Key.active){
				el.Key.SetActiveRecursively(false);
			}
		}
	}
	
	
	#region cubes
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
