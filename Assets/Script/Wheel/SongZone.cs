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
	private GameObject cubeSelected;
	public float decalCubeSelected;
	public float startSongListY; //2f
	public float decalSongListY; //-3f
	public float decalLabelX;
	public float decalLabelY;
	private Vector3 posBaseCameraSong;
	private bool popin;
	private bool popout;
	
	public Rect posSonglist;
	private GameObject particleOnPlay;
	public int numberToDisplay;
	private int startnumber;
	private int currentstartnumber;
	public float speedCameraDefil;
	public float offsetSubstitle;
	
	private Dictionary<string, Dictionary<Difficulty, Song>> songList;
	private bool locked;
	
	
	
	//Search Bar
	public Rect SearchBarPos;
	
	private string search;
	private string searchOldValue;
	public Rect posSwitchSearch;
	
	private bool activeModule;
	// Use this for initialization
	void Start () {
		gs = GetComponent<GeneralScript>();
		
		popin = false;
		popout = false;
		activeModule = true;
		posBaseCameraSong = camerasong.transform.position;
		startnumber = 0;
		currentstartnumber = 0;
		
		songCubePack = new Dictionary<GameObject, string>();
		LinkCubeSong = new Dictionary<GameObject, string>();
		
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
						gs.songSelected = LoadManager.Instance.FindSong[packSelected][splitedNameSong[2]];
						gs.getZoneInfo().refreshDifficultyDisplayed();
						cubeSelected = papa.gameObject;
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
							gs.songSelected = LoadManager.Instance.FindSong[packSelected][splitedNameSong[2]];
							gs.getZoneInfo().refreshDifficultyDisplayed();
							cubeSelected = papa.gameObject;
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
		}
		
		if(Input.GetAxis("Mouse ScrollWheel") > 0 && startnumber > 0){
			startnumber -= DataManager.Instance.mouseMolSpeed;
			if(startnumber < 0) startnumber = 0;
			
		}else if(Input.GetAxis("Mouse ScrollWheel") < 0 && startnumber < (songList.Count() - numberToDisplay + 1)){
			var songcount = songList.Count() - numberToDisplay + 1;
			if(startnumber < songcount){
				startnumber += DataManager.Instance.mouseMolSpeed;
				if(startnumber > songcount) startnumber = songcount;
			}
			
			
		}
		
		
		//Move song list
		var oldpos = camerasong.transform.position.y;
		if(Mathf.Abs(camerasong.transform.position.y - decalSongListY*startnumber) <= 0.1f){
			camerasong.transform.position = new Vector3(camerasong.transform.position.x, - decalSongListY*startnumber, camerasong.transform.position.z);

		}else{
			 
			camerasong.transform.position = Vector3.Lerp(camerapack.transform.position, new Vector3(camerasong.transform.position.x, decalSongListY*startnumber, camerasong.transform.position.z), Time.deltaTime/speedCameraDefil);
			
		}
		var newpos = camerapack.transform.position.y;
		
		if(oldpos > newpos){
		
			foreach(var cubeel2 in songCubePack.Where(c => !c.Key.active && (c.Key.transform.position.y > camerasong.transform.position.y - decalSongListY*numberToDisplay) && !(c.Key.transform.position.y > camerasong.transform.position.y + startSongListY))){
				cubeel2.Key.SetActiveRecursively(true);
				if(cubeSelected == null || cubeSelected != cubeel2.Key) cubeel2.Key.transform.FindChild("Selection").gameObject.active = false;
				
			}
			
			
			foreach(var cubeel in songCubePack.Where(c => c.Key.active && (c.Key.transform.position.y > camerasong.transform.position.y + startSongListY))){
				cubeel.Key.SetActiveRecursively(false);
				if(startnumber > currentstartnumber) currentstartnumber++;
			}

			
			
		}else if(oldpos < newpos){
			
			foreach(var cubeel2 in songCubeOnRender.Where(c => c.Key.active && (c.Key.transform.position.y < camerasong.transform.position.y - decalSongListY*numberToDisplay))){

				cubeel2.Key.SetActiveRecursively(false);
				
			}
			
			
			foreach(var cubeel in songCubeOnRender.Where(c => !c.Key.active && (c.Key.transform.position.y < camerasong.transform.position.y + 5f) && (c.Key.transform.position.y > camerasong.transform.position.y - decalSongListY*(numberToDisplay - 2)))){

				cubeel.Key.SetActiveRecursively(true);
				if(startnumber < currentstartnumber) currentstartnumber--;
				if(cubeSelected == null || cubeSelected != cubeel.Key) cubeel.Key.transform.FindChild("Selection").gameObject.active = false;
				
			}
			
			
		}
		
		
		//Popin/out
		if(popin)
		{
			camerasong.transform.position = Vector3.Lerp(camerasong.transform.position, new Vector3(camerasong.transform.position.x, posYModule.x, camerasong.transform.position.z), Time.deltaTime/speedPop);
			
			if(Math.Abs(camerasong.transform.position.y - posYModule.x <= limite))
			{
				popin = false;	
				camerapack.transform.position = new Vector3(camerasong.transform.position.x, posYModule.x, camerasong.transform.position.z);
			}
		}
		
		if(popout)
		{
			camerasong.transform.position = Vector3.Lerp(camerasong.transform.position, new Vector3(camerasong.transform.position.x, posYModule.y, camerasong.transform.position.z), Time.deltaTime/speedPop);
			
			if(Math.Abs(camerasong.transform.position.y - posYModule.y <= limite))
			{
				popout = false;	
				camerasong.transform.position = new Vector3(camerasong.transform.position.x, posYModule.y, camerasong.transform.position.z);	
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
				//custom
				
				unFocus();
				
				var	num = 0;
				error.displayError = (DataManager.Instance.sortMethod >= Sort.DIFFICULTY && !Int32.TryParse(search, out num));
			}else if(!LoadManager.Instance.isAllowedToSearch(search) && searchOldValue.Trim().Length > search.Trim().Length){
				activeSongList(LoadManager.Instance.ListSong()[GetComponent<GeneralScript>().getZonePack().getActivePack()]);
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
	
	
	void unFocus()
	{
		cubeSelected = null;
		gs.getInfoZone().disableDifficultyDisplayed();
		gs.songSelected = null;
		locked = false;
		if(particleOnPlay != null)
		{
			particleOnPlay.active = false;	
		}
	}
	
	public void activeSongList(string pack)
	{
		packSelected = pack;
		songList = LoadManager.Instance.ListSong()[pack];
		startnumber = 0;
		currentstartnumber = 0;
		camerasong.transform.position = posBaseCameraSong;
		
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
			if(!key.active) key.SetActiveRecursivly(i <= numberToDisplay);
			songCubePack[songCubePack.ElementAt(i).Key] = songList.ElementAt(i).First().Value.title + ";" + songList.ElementAt(i).First().Value.subtitle + ";" + songList.ElementAt(i).Key;
		}
		for(int i=songList.Count; i < songCubePack.Count; i++)
		{
			if(key.active) key.SetActiveRecursivly(false);
		}
	}
	
	public void desactiveSongList()
	{
		for(int i=0; i < songCubePack.Count; i++)
		{
			if(key.active) key.SetActiveRecursivly(false);
		}
	}
	
	public void onPopin()
	{
		popin = true;
		activeModule = true;
	}
	
	
	public void onPopout()
	{
		popout = true;
		activeModule = false;
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
