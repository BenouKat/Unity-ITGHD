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
		
		
		songCubePack = new Dictionary<GameObject, string>();
		LinkCubeSong = new Dictionary<GameObject, string>();
		customSongCubePack = new Dictionary<GameObject, string>();
		customLinkCubeSong = new Dictionary<GameObject, string>();
		
		createCubeSong();
	}
	
	// Update is called once per frame
	void Update () {
	
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
