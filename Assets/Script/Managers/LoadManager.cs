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
	private Dictionary<string, Texture2D> bannerPack;
	
	private LoadManager(){

		
	}
	
	public void Loading(){
		bannerPack = new Dictionary<string, Texture2D>();
		//string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/"); 	//RELEASE
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + "/../Songs/");  	//DEBUG
		var length = lastDir((string) packpath[0]).Count();
		songs = new Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>>();
		foreach(string el in packpath){
			songs.Add(lastDir(el)[length - 1], new Dictionary<string, Dictionary<Difficulty, Song>>());
			var path = Directory.GetFiles(el).FirstOrDefault(c => c.Contains(".png") || c.Contains(".jpg") || c.Contains(".jpeg"));
			if(!String.IsNullOrEmpty(path)){
				WWW www = new WWW("file://" + path);
       			Texture2D texTmp = new Texture2D(256, 128);
				while(!www.isDone){}
        		www.LoadImageIntoTexture(texTmp);
				bannerPack.Add(lastDir(el)[length - 1], texTmp);
			}else{
				bannerPack.Add(lastDir(el)[length - 1], (Texture2D) Resources.Load("Cublast"));
			}
			
			//Debug.Log("new pack : " + lastDir(el)[length - 1]);
		}
		
		
		
		foreach(var el in songs){
			//string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/" + el.Key);		//RELEASE
			string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + "/../Songs/" + el.Key);		//DEBUG
			var lengthsp = lastDir ((string) songpath[0]).Count();
			foreach(string sp in songpath){
				//Debug.Log("new song : " + lastDir(sp)[lengthsp - 1]);
				var dic = OpenChart.Instance.readChart(sp.Replace('\\', '/'));
				if(dic != null && dic.Count != 0) songs[el.Key].Add(lastDir(sp)[lengthsp - 1] , dic);
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
	
	public Dictionary<string, Dictionary<Difficulty, Song>> ListSong(Dictionary<string, Dictionary<Difficulty, Song>> previousList, string contains){
		
		var finalList = new Dictionary<string, Dictionary<Difficulty, Song>>();
		if(previousList.Count == 0){
			foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.title.ToLower().Contains(contains.ToLower())).Count() > 0)){
				foreach(var song in packs.Value.Where(r => r.Value.First().Value.title.ToLower().Contains(contains.ToLower()))){
					finalList.Add(song.Value.First().Value.title + "[" + packs.Key + "]", song.Value);
				}
			}
		}else{
			finalList = previousList.Where(r => r.Value.First().Value.title.ToLower().Contains(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
		}
		
		return finalList;
	}
	
	public Dictionary<string, Texture2D> ListTexture(){
		return bannerPack;
		
	}
}
