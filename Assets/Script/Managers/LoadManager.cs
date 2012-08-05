using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
public class LoadManager{

	private static LoadManager instance;
	
	public static LoadManager Instance{
		get{
			if(instance == null){
			instance = new LoadManager();	
			}
			return instance;
		
		}
	}
	
	private Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>> songs;
	
	private LoadManager(){

		
	}
	
	public void Loading(){
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/");
		var length = lastDir((string) packpath[0]).Count();
		songs = new Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>>();
		foreach(string el in packpath){
			songs.Add(lastDir(el)[length - 1], new Dictionary<string, Dictionary<Difficulty, Song>>());
			//Debug.Log("new pack : " + lastDir(el)[length - 1]);
		}
		
		
		
		foreach(var el in songs){
			string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/" + el.Key);
			var lengthsp = lastDir ((string) songpath[0]).Count();
			foreach(string sp in songpath){
				//Debug.Log("new song : " + lastDir(sp)[lengthsp - 1]);
				var dic = OpenChart.Instance.readChart(sp.Replace('\\', '/'));
				songs[el.Key].Add(lastDir(sp)[lengthsp - 1] , dic);
				/*Debug.Log("Song : " + lastDir(sp)[lengthsp - 1] + 
					" added / pack : " + el.Key + 
					" / number diff : " + songs[el.Key][lastDir(sp)[lengthsp - 1]].Count() +
					" / number step expert : " + songs[el.Key][lastDir(sp)[lengthsp - 1]][Difficulty.EXPERT].numberOfSteps);*/
					
			}
		}
	}
	
	private string[] lastDir(string dir){
		return dir.Replace('\\', '/').Split ('/');
	}

	public Dictionary<Difficulty, Song> FindSong(string pack, string song){
		return songs[pack][song];
		
	}
	
	public Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>> ListSong(){
		return songs;
		
	}
}
