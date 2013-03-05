using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SongZone : MonoBehaviour {
	
	
	
	public Camera camerasong;
	public GameObject cubeSong;
	
	
	
	private Dictionary<GameObject, string> songCubePack;
	private Dictionary<Difficulty, Song> songSelected;
	
	//SongList
	private GameObject cubeSelected;
	public float decalCubeSelected;
	public float startSongListY; //2f
	public float decalSongListY; //-3f
	public float decalLabelX;
	public float decalLabelY;
	
	public Rect posSonglist;
	public float ecartSong;
	private GameObject particleOnPlay;
	public int numberToDisplay;
	private int startnumber;
	private int currentstartnumber;
	public float speedCameraDefil;
	private float posLabel;
	public float offsetSubstitle;
	
	private Dictionary<string, Dictionary<Difficulty, Song>> songList;
	private bool locked;
	
	
	
	//Search Bar
	public Rect SearchBarPos;
	
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
		
		createCubeSong();
		
		locked = false;
		
		
		search = "";
		searchOldValue = "";
		songList = new Dictionary<string, Dictionary<Difficulty, Song>>();
		
		if(DataManager.Instance.mousePosition != -1){
			startnumber = DataManager.Instance.mousePosition;
			currentstartnumber = startnumber;
		}
	}
	
	public void activeSongList(Dictionary<string, Dictionary<Difficulty, Song>> list)
	{
		songList = list;
		if(songList.Count > songCubePack.Count)
		{
			var pos = startSongListY  - decalSongList*songCubePack.Count;
			for(int i=songCubePack.Count; i < songList.Count; i++)
			{
				
				var thego = (GameObject) Instantiate(cubeSong, new Vector3(-25f, pos, 0f), cubeSong.transform.rotation);
				pos -= decalSongListY;
				thego.SetActiveRecursively(false);
				songCubePack.Add(thego, "");
			}
		}
		
		for(int i=0; i < songList.Count; i++)
		{
			var key = songCubePack.ElementAt(i).Key;
			if(!key.active) key.SetActiveRecursivly(true);
			songCubePack[songCubePack.ElementAt(i).Key] = songList.ElementAt(i).First().Value.title + ";" + songList.ElementAt(i).First().Value.subtitle;
		}
		for(int i=songList.Count; i < songCubePack.Count; i++)
		{
			if(key.active) key.SetActiveRecursivly(false);
		}
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
			}

			
			
		}else if(oldpos < newpos){
			
			foreach(var cubeel2 in songCubeOnRender.Where(c => c.Key.active && (c.Key.transform.position.y < camerapack.transform.position.y - 3f*numberToDisplay) && packs.ElementAt(nextnumberPack).Key == c.Value)){

				cubeel2.Key.SetActiveRecursively(false);
				
			}
			
			
			foreach(var cubeel in songCubeOnRender.Where(c => !c.Key.active && (c.Key.transform.position.y < camerapack.transform.position.y + 5f) && (c.Key.transform.position.y > camerapack.transform.position.y - 3f*(numberToDisplay - 2)) && packs.ElementAt(nextnumberPack).Key == c.Value)){

				cubeel.Key.SetActiveRecursively(true);
				if(startnumber < currentstartnumber) currentstartnumber--;
				if(cubeSelected == null || cubeSelected != cubeel.Key) cubeel.Key.transform.FindChild("Selection").gameObject.active = false;
				
			}
			
			
		}
	}
	
	void OnGUI()
	{
		var begin = startnumber == 0 ? 0 : startnumber - 1;
		for(int i=begin; i<numberToDisplay; i++){

				var point2D = cameraSong.WorldPointToScreen(songCubePack.ElementAt(i).Key.transform.position);
				point2D.x += decalLabelX*Screen.width;
				point2D.y += decalLabelY*Screen.height;

				var title = songCubePack.ElementAt(i).Value.Split(';')[0];
				if(title.Length > 35) title = title.Remove(35, title.Length - 35) + "...";
				var subtitle = songCubePack.ElementAt(i).Value.Split(';')[1];
				if(subtitle.Length > 50) subtitle = subtitle.Remove(50, subtitle.Length - 50) + "...";
				
				var alphaText = 1f;
				if(begin != 0 && i == begin)
				{
					alphaText = Math.Abs(cameraSong.transform.position.y - startnumber*decalSongListY + startSongListY);
					if(decal > 1f)
					{
						alphaText = 1f;
					}
					
				}
				GUI.color = new Color(0f, 0f, 0f, alphaText);
				GUI.Label(new Rect(point2D.x +1f , point2D.y +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), title, "songlabel");
				GUI.Label(new Rect(point2D.x +1f , point2D.y + (offsetSubstitle*Screen.height) +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), subtitle, "infosong");
				
				GUI.color = new Color(1f, 1f, 1f, alphaText);
				GUI.Label(new Rect(point2D.x, point2D.y, posSonglist.width*Screen.width, posSonglist.height*Screen.height), title, "songlabel");
				GUI.Label(new Rect(point2D.x, point2D.y + (offsetSubstitle*Screen.height) +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), subtitle, "infosong");
		}
		
		//TO DO START HERE
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
				activeSongList(LoadManager.Instance.ListSong(songList, search.Trim()));
				//custom list
				var	num = 0;
				error.displayError = (DataManager.Instance.sortMethod >= Sort.DIFFICULTY && !Int32.TryParse(search, out num));
			}else if(!LoadManager.Instance.isAllowedToSearch(search) && searchOldValue.Trim().Length > search.Trim().Length){
				//recover
				error.displayError = false;
			}
			if(songList.Count == 0 && LoadManager.Instance.isAllowedToSearch(search)){
				//no entry
				GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
				GUI.Label(new Rect(posSonglist.x*Screen.width, posSonglist.y*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), "No entry", "songlabel");
				GUI.color = new Color(1f, 1f, 1f, 1f);
			}
		}
		searchOldValue = search;
		
	}
	
	void createCubeSong(){
		
		var maximumSize = 0;
		for(int i=0; i < LoadManager.Instance.ListSong().Count; i++)
		{
			if(maximumSize < LoadManager.Instance.ListSong().ElementAt(i).Count)
			{
				maximumSize = LoadManager.Instance.ListSong().ElementAt(i).Count;
			}
		}
		
		var pos = startSongListY;
		for(int i = 0; i < maximumSize; i++)
		{
			var thego = (GameObject) Instantiate(cubeSong, new Vector3(-25f, pos, 0f), cubeSong.transform.rotation);
			pos -= decalSongListY;
			thego.SetActiveRecursively(false);
			songCubePack.Add(thego, "");
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
