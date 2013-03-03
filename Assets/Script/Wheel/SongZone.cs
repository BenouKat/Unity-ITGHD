using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SongZone : MonoBehaviour {
	
	
	public GameObject cubeSong;
	public GameObject cubeBase;
	
	
	
	private Dictionary<GameObject, string> songCubePack;
	private Dictionary<GameObject, string> customSongCubePack;
	private Dictionary<GameObject, string> LinkCubeSong;
	private Dictionary<GameObject, string> customLinkCubeSong;
	private Dictionary<Difficulty, Song> songSelected;
	
	//SongList
	private GameObject cubeSelected;
	public Rect posSonglist;
	public float ecartSong;
	private GameObject particleOnPlay;
	public int numberToDisplay;
	private int startnumber;
	private int currentstartnumber;
	public float speedCameraDefil;
	private float posLabel;
	public float offsetSubstitle;
	private Vector3 basePosCubeBase;
	
	
	private bool locked;
	
	
	
	//Search Bar
	public Rect SearchBarPos;
	private Dictionary<string, Dictionary<Difficulty, Song>> songList;
	private string search;
	private string searchOldValue;
	public Rect posSwitchSearch;
	
	// Use this for initialization
	void Start () {
		startnumber = 0;
		currentstartnumber = 0;
		posLabel = 0f;
		
		songCubePack = new Dictionary<GameObject, string>();
		LinkCubeSong = new Dictionary<GameObject, string>();
		customSongCubePack = new Dictionary<GameObject, string>();
		customLinkCubeSong = new Dictionary<GameObject, string>();
		
		createCubeSong();
		
		locked = false;
		
		
		search = "";
		searchOldValue = "";
		songList = new Dictionary<string, Dictionary<Difficulty, Song>>();
		
		
		if(DataManager.Instance.mousePosition != -1){
			startnumber = DataManager.Instance.mousePosition;
			currentstartnumber = startnumber;
		}
		
		basePosCubeBase = new Vector3(cubeBase.transform.position.x, 5f, cubeBase.transform.position.z);
		cubeBase.transform.position = new Vector3(basePosCubeBase.x, basePosCubeBase.y - (3f*startnumber), basePosCubeBase.z);
	}
	
	// Update is called once per frame
	void Update () {
	
		var packOnRender = LoadManager.Instance.isAllowedToSearch(search) ? songList : LoadManager.Instance.ListSong()[packs.ElementAt(nextnumberPack).Key];
		var linkOnRender = LoadManager.Instance.isAllowedToSearch(search) ? customLinkCubeSong : LinkCubeSong;
		Dictionary<GameObject, string> songCubeOnRender = LoadManager.Instance.isAllowedToSearch(search) ? customSongCubePack : songCubePack;
		
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
		
		if(Input.GetMouseButtonDown(1)){
			locked = false;
		}
		
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
			
			
		}
	}
	
	void OnGUI()
	{
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
		
	}
	
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
	
	void createCubeSong(Dictionary<string, Dictionary<Difficulty, Song>> theSongList){
		var pos = 2f;
		foreach(var cubes in customSongCubePack){
			Destroy(cubes.Key);
		}
		customSongCubePack.Clear();
		customLinkCubeSong.Clear();
		foreach(var song in theSongList){
			var thego = (GameObject) Instantiate(cubeSong, new Vector3(-25f, pos, 0f), cubeSong.transform.rotation);
			pos -= 3f;
			thego.SetActiveRecursively(false);
			customSongCubePack.Add(thego,packs.ElementAt(nextnumberPack).Key);
			customLinkCubeSong.Add(thego, song.Value.First().Value.title + "/" + song.Value.First().Value.subtitle);

		}
	}
	
	void DestroyCustomCubeSong(){
		foreach(var cubes in customSongCubePack){
			Destroy(cubes.Key);
		}
		customSongCubePack.Clear();
		customLinkCubeSong.Clear();
	}
	
	IEnumerator AnimSearchBar(bool reverse){
		if(!reverse){
			while(Mathf.Abs(packs.First().Value.transform.position.y - 17f) > limite){
				foreach(var pa in packs){
					pa.Value.transform.position = Vector3.Lerp(pa.Value.transform.position, new Vector3(pa.Value.transform.position.x, 17f, pa.Value.transform.position.z), Time.deltaTime/speedMove);
				}
				yield return new WaitForFixedUpdate();
			}
			foreach(var pa in packs){
				pa.Value.transform.position = new Vector3(pa.Value.transform.position.x, 17f, pa.Value.transform.position.z);
			}
		}else{
			while(Mathf.Abs(packs.First().Value.transform.position.y - 13f) > limite){
				foreach(var pa in packs){
					pa.Value.transform.position = Vector3.Lerp(pa.Value.transform.position, new Vector3(pa.Value.transform.position.x, 13f, pa.Value.transform.position.z), Time.deltaTime/speedMove);
				}
				yield return new WaitForFixedUpdate();
			}
			foreach(var pa in packs){
				pa.Value.transform.position = new Vector3(pa.Value.transform.position.x, 13f, pa.Value.transform.position.z);
			}
		}
		
	}
	
	string sortToString(Sort s){
		switch(s){
			case Sort.NAME:
				return "Name contain : ";
			case Sort.STARTWITH:
				return "Name start : ";
			case Sort.ARTIST:
				return "Artist : ";
			case Sort.STEPARTIST:
				return "Stepartist : ";
			case Sort.DIFFICULTY:
				return "Difficulty : ";
			case Sort.BPM:
				return "BPM : ";
			default:
				return "ERROR";
		}
	}
}
