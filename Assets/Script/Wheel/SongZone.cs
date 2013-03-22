using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SongZone : MonoBehaviour {
	
	public Camera camerasong;
	public GameObject cubeSong;
	
	
	private Dictionary<GameObject, string> songCubePack;
	private GeneralScript gs;
	
	
	//SongList
	private string packSelected;
	private string packLocked;
	private GameObject cubeSelected;
	public float decalCubeSelected;
	public float startSongListY; //2f
	public float decalSongListY; //-3f
	public Vector2 posXModule;
	public float decalLabelX;
	public float decalLabelY;
	public float limite;
	private Vector3 posBaseCameraSong;
	public float speedPop;
	private bool popin;
	private bool popout;
	
	public Rect posSonglist;
	private GameObject particleOnPlay;
	public int numberToDisplay;
	private int startnumber;
	private int currentstartnumber;
	public float speedCameraDefil;
	public float offsetSubstitle;
	private bool customSearch;
	
	private Dictionary<string, Dictionary<Difficulty, Song>> songList;
	private Dictionary<string, Dictionary<Difficulty, Song>> searchList;
	public bool locked;
	private ErrorLabel error;
	
	
	//Search Bar
	public Rect SearchBarPos;
	
	private string search;
	private string searchOldValue;
	public Rect posSwitchSearch;
	
	private bool activeModule;
	// Use this for initialization
	void Start () {
		gs = GetComponent<GeneralScript>();
		error = GetComponent<ErrorLabel>();
		popin = false;
		popout = false;
		activeModule = true;
		posBaseCameraSong = camerasong.transform.position;
		startnumber = 0;
		currentstartnumber = 0;
		customSearch = false;
		packLocked = "null";
		
		songCubePack = new Dictionary<GameObject, string>();
		
		createCubeSong();
		
		locked = false;
		
		
		search = "";
		searchOldValue = "";
		songList = new Dictionary<string, Dictionary<Difficulty, Song>>();
		
		activeSongList(GetComponent<GeneralScript>().getZonePack().getActivePack());
		
		if(DataManager.Instance.mousePosition != -1){
			startnumber = DataManager.Instance.mousePosition;
			currentstartnumber = startnumber;
		}
		
		
	}
	
	
	
	// Update is called once per frame
	void Update () {
	
		
		if(activeModule && !gs.getZoneInfo().isExiting())
		{
			//Selection
			Ray ray = camerasong.ScreenPointToRay(Input.mousePosition);	
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
						var splitedNameSong = songCubePack[papa.gameObject].Split(';');
						if(gs.songSelected == null || ((gs.songSelected.First().Value.title + ";" + gs.songSelected.First().Value.subtitle) != splitedNameSong[0] + ";" + splitedNameSong[1]))
						{
							packLocked = packSelected;
							gs.songSelected = LoadManager.Instance.FindSong(String.IsNullOrEmpty(packSelected) ? splitedNameSong[3] : packSelected, splitedNameSong[2]);
							gs.getZoneInfo().refreshDifficultyDisplayed();
							gs.refreshBanner();
							onCubeSelected(true);
							cubeSelected = papa.gameObject;
							onCubeSelected(false);
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
							var splitedNameSong = songCubePack[papa.gameObject].Split(';');
							if(gs.songSelected == null || ((gs.songSelected.First().Value.title + ";" + gs.songSelected.First().Value.subtitle) != splitedNameSong[0] + ";" + splitedNameSong[1]))
							{
								packLocked = packSelected;
								gs.songSelected = LoadManager.Instance.FindSong(String.IsNullOrEmpty(packSelected) ? splitedNameSong[3] : packSelected, splitedNameSong[2]);
								gs.getZoneInfo().refreshDifficultyDisplayed();
								gs.refreshBanner();
								onCubeSelected(true);
								cubeSelected = papa.gameObject;
								onCubeSelected(false);
							}
							particleOnPlay = thepart;
							particleOnPlay.active = true;
						}else{
							locked = true;
						}
					}
					
				}else if(particleOnPlay != null && particleOnPlay.active){
					
					if(!locked){
						unFocus();
					}
				}
				
				
				
			}else if(particleOnPlay != null && particleOnPlay.active){
				if(!locked){
					unFocus();
				}
			}
			
			//inputs
			if(Input.GetMouseButtonDown(1)){
				locked = false;
				if(particleOnPlay == null || !particleOnPlay.active)
				{
					unFocus();	
				}
			}
			
			if((Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.UpArrow)) && startnumber > 0){
				startnumber -= DataManager.Instance.mouseMolSpeed;
				if(startnumber < 0) startnumber = 0;
				
			}else if((Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.DownArrow)) && startnumber < (songList.Count() - numberToDisplay + 1)){
				var songcount = songList.Count() - numberToDisplay + 1;
				if(startnumber < songcount){
					startnumber += DataManager.Instance.mouseMolSpeed;
					if(startnumber > songcount) startnumber = songcount;
				}
				
			}
			
			
			//Move song list
			var oldpos = camerasong.transform.position.y;
			if(Mathf.Abs(camerasong.transform.position.y - (startSongListY - decalSongListY*startnumber)) <= 0.1f){
				camerasong.transform.position = new Vector3(camerasong.transform.position.x, startSongListY - decalSongListY*startnumber, camerasong.transform.position.z);
				
			}else{
				 
				camerasong.transform.position = Vector3.Lerp(camerasong.transform.position, new Vector3(camerasong.transform.position.x, startSongListY - decalSongListY*startnumber, camerasong.transform.position.z), Time.deltaTime/speedCameraDefil);
				
			}
			var newpos = camerasong.transform.position.y;
		
			if(oldpos > newpos){
			
				foreach(var cubeel2 in songCubePack.Where(c => !c.Key.active && (c.Key.transform.position.y > camerasong.transform.position.y - decalSongListY*numberToDisplay) && !(c.Key.transform.position.y > camerasong.transform.position.y + startSongListY) && !String.IsNullOrEmpty(c.Value))){
					cubeel2.Key.SetActiveRecursively(true);
					if(cubeSelected == null || cubeSelected != cubeel2.Key || packSelected != packLocked) cubeel2.Key.transform.FindChild("Selection").gameObject.active = false;
					
				}
				
				
				foreach(var cubeel in songCubePack.Where(c => c.Key.active && (c.Key.transform.position.y > camerasong.transform.position.y + startSongListY))){
					cubeel.Key.SetActiveRecursively(false);
					if(startnumber > currentstartnumber) currentstartnumber++;
				}
	
				
				
			}else if(oldpos < newpos){
				
				foreach(var cubeel2 in songCubePack.Where(c => c.Key.active && (c.Key.transform.position.y < camerasong.transform.position.y - decalSongListY*numberToDisplay))){
	
					cubeel2.Key.SetActiveRecursively(false);
					
				}
				
				
				foreach(var cubeel in songCubePack.Where(c => !c.Key.active && (c.Key.transform.position.y < camerasong.transform.position.y + 5f) && (c.Key.transform.position.y > camerasong.transform.position.y - decalSongListY*(numberToDisplay - 2)) && !String.IsNullOrEmpty(c.Value))){
	
					cubeel.Key.SetActiveRecursively(true);
					if(startnumber < currentstartnumber) currentstartnumber--;
					if(cubeSelected == null || cubeSelected != cubeel.Key || packSelected != packLocked) cubeel.Key.transform.FindChild("Selection").gameObject.active = false;
					
				}
				
				
			}
		}
		
		//Popin/out
		if(popin)
		{
			camerasong.transform.position = Vector3.Lerp(camerasong.transform.position, new Vector3(posXModule.x, camerasong.transform.position.y, camerasong.transform.position.z), Time.deltaTime*speedPop);
			
			if(Math.Abs(camerasong.transform.position.x - posXModule.x) <= limite)
			{
				popin = false;	
				camerasong.transform.position = new Vector3(posXModule.x, camerasong.transform.position.y, camerasong.transform.position.z);
			}
		}
		
		if(popout)
		{
			camerasong.transform.position = Vector3.Lerp(camerasong.transform.position, new Vector3(posXModule.y, camerasong.transform.position.y, camerasong.transform.position.z), Time.deltaTime*speedPop);
			
			if(Math.Abs(camerasong.transform.position.x - posXModule.y) <= limite)
			{
				popout = false;	
				camerasong.transform.position = new Vector3(posXModule.y, camerasong.transform.position.y, camerasong.transform.position.z);	
			}
		}
	}
	
	public void GUIModule()
	{
		if(activeModule)
		{
			var begin = currentstartnumber;
			
			for(int i=begin; (i<numberToDisplay + currentstartnumber) && i<songList.Count; i++){
	
					var point2D = camerasong.WorldToScreenPoint(songCubePack.ElementAt(i).Key.transform.position);
					point2D.x += decalLabelX*Screen.width;
					point2D.y = Screen.height - point2D.y;
					point2D.y += decalLabelY*Screen.height;
	
					var title = songCubePack.ElementAt(i).Value.Split(';')[0];
					if(title.Length > 35) title = title.Remove(35, title.Length - 35) + "...";
					var subtitle = songCubePack.ElementAt(i).Value.Split(';')[1];
					if(subtitle.Length > 50) subtitle = subtitle.Remove(50, subtitle.Length - 50) + "...";
					
					var alphaText = 1f;
				
					if(i == begin && (camerasong.transform.position.y < startSongListY - decalSongListY*currentstartnumber))
					{
						alphaText = 1f - Math.Abs(camerasong.transform.position.y - (startSongListY - decalSongListY*currentstartnumber))/decalSongListY;
					}
					
					
					GUI.color = new Color(0f, 0f, 0f, alphaText);
					GUI.Label(new Rect(point2D.x +1f , point2D.y +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), title, "songlabel");
					GUI.Label(new Rect(point2D.x +1f , point2D.y + (offsetSubstitle*Screen.height) +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), subtitle, "infosong");
					
					GUI.color = new Color(1f, 1f, 1f, alphaText);
					GUI.Label(new Rect(point2D.x, point2D.y, posSonglist.width*Screen.width, posSonglist.height*Screen.height), title, "songlabel");
					GUI.Label(new Rect(point2D.x, point2D.y + (offsetSubstitle*Screen.height) +1f, posSonglist.width*Screen.width, posSonglist.height*Screen.height), subtitle, "infosong");
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
					if(!customSearch)
					{
						searchList = LoadManager.Instance.ListSong(new Dictionary<string, Dictionary<Difficulty, Song>>(), search.Trim());
						activeSongList(searchList);
						customSearch = true;
					}else
					{
						if(search.Length < searchOldValue.Length)
						{
							activeSongList(LoadManager.Instance.ListSong(searchList, search.Trim()));
						}else
						{
							activeSongList(LoadManager.Instance.ListSong(songList, search.Trim()));	
						}
							
					}
					
					gs.getZonePack().onPopout();
					//custom
					
					unFocus();
					
					var	num = 0;
					error.displayError = (DataManager.Instance.sortMethod >= Sort.DIFFICULTY && !Int32.TryParse(search, out num));
				}else if(!LoadManager.Instance.isAllowedToSearch(search) && searchOldValue.Trim().Length > search.Trim().Length){
					activeSongList(GetComponent<GeneralScript>().getZonePack().getActivePack());
					gs.getZonePack().onPopin();
					customSearch = false;
					//recover
					unFocus();
					
					error.displayError = false;
				}
				if(songList.Count == 0 && LoadManager.Instance.isAllowedToSearch(search)){
					desactiveSongList();
					//no entry
					
					GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
					GUI.Label(new Rect(posSonglist.x*Screen.width, posSonglist.y*Screen.height, posSonglist.width*Screen.width, posSonglist.height*Screen.height), "No entry", "songlabel");
					GUI.color = new Color(1f, 1f, 1f, 1f);
				}
			}
			searchOldValue = search;
		}
	}
	
	void createCubeSong(){
		
		var maximumSize = 0;
		for(int i=0; i < LoadManager.Instance.ListSong().Count; i++)
		{
			if(maximumSize < LoadManager.Instance.ListSong().ElementAt(i).Value.Count)
			{
				maximumSize = LoadManager.Instance.ListSong().ElementAt(i).Value.Count;
			}
		}
		
		var pos = startSongListY + decalSongListY;
		for(int i = 0; i < maximumSize; i++)
		{
			var thego = (GameObject) Instantiate(cubeSong, new Vector3(-25f, pos, 20f), cubeSong.transform.rotation);
			pos -= decalSongListY;
			thego.SetActiveRecursively(false);
			songCubePack.Add(thego, "");
		}
	}
	
	
	void unFocus()
	{
		onCubeSelected(true);
		cubeSelected = null;
		gs.getZoneInfo().disableDifficultyDisplayed();
		gs.songSelected = null;
		gs.onFadeOutBanner();
		gs.shutSong();
		locked = false;
		if(particleOnPlay != null)
		{
			particleOnPlay.active = false;	
		}
	}
	
	void onCubeSelected(bool reverse)
	{
		if(reverse)
		{
			
		}else
		{
			
		}
	}
	
	public void activeSongList(string pack)
	{
		packSelected = pack;
		songList = LoadManager.Instance.ListSong()[pack];
		startnumber = 0;
		currentstartnumber = 0;
		camerasong.transform.position = posBaseCameraSong;
		
		
		for(int i=0; i < songList.Count; i++)
		{
			var key = songCubePack.ElementAt(i).Key;
			if(!key.active)
			{
				key.SetActiveRecursively(i <= numberToDisplay);
			}
			var songChecked = songList.ElementAt(i).Value.First().Value;
			songCubePack[key] = songChecked.title + ";" + songChecked.subtitle + ";" + songList.ElementAt(i).Key;
			if(packLocked == packSelected && gs.songSelected != null && ((gs.songSelected.First().Value.title + ";" + gs.songSelected.First().Value.subtitle) == (songChecked.title + ";" + songChecked.subtitle)))
			{
				key.transform.FindChild("Selection").gameObject.active = true;	
			}else
			{
				key.transform.FindChild("Selection").gameObject.active = false;
				
			}
		}
		for(int i=songList.Count; i < songCubePack.Count; i++)
		{
			var key = songCubePack.ElementAt(i).Key;
			if(key.active) key.SetActiveRecursively(false);
			songCubePack[key] = "";
		}
	}
	
	public void activeSongList(Dictionary<string, Dictionary<Difficulty, Song>> pack)
	{
		packSelected = "";
		songList = pack;
		startnumber = 0;
		currentstartnumber = 0;
		camerasong.transform.position = posBaseCameraSong;
		
		if(songList.Count > songCubePack.Count)
		{
			var pos = startSongListY  - decalSongListY*songCubePack.Count;
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
			if(!key.active)
			{
				key.SetActiveRecursively(i <= numberToDisplay);
			}
			var songChecked = songList.ElementAt(i).Value.First().Value;
			songCubePack[key] = songChecked.title + ";" + songChecked.subtitle + ";" + songList.ElementAt(i).Key;
			if(packLocked == packSelected && gs.songSelected != null && ((gs.songSelected.First().Value.title + ";" + gs.songSelected.First().Value.subtitle) == (songChecked.title + ";" + songChecked.subtitle)))
			{
				key.transform.FindChild("Selection").gameObject.active = true;	
			}else
			{
				key.transform.FindChild("Selection").gameObject.active = false;	
			}
		}
		for(int i=songList.Count; i < songCubePack.Count; i++)
		{
			var key = songCubePack.ElementAt(i).Key;
			if(key.active) key.SetActiveRecursively(false);
			songCubePack[key] = "";
		}
	}
	
	public void desactiveSongList()
	{
		for(int i=0; i < songCubePack.Count; i++)
		{
			var key = songCubePack.ElementAt(i).Key;
			if(key.active) key.SetActiveRecursively(false);
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
	
	public int getStartNumber()
	{
		return !LoadManager.Instance.isAllowedToSearch(search) ? startnumber : -1;
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
