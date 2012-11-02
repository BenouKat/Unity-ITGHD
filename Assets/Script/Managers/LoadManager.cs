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
	public bool alreadyLoaded;
	
	private LoadManager(){
		alreadyLoaded = false;
		
	}
	
	public void Loading(){
		bannerPack = new Dictionary<string, Texture2D>();
		//string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/"); 	//RELEASE
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + DataManager.Instance.DEBUGPATH + "Songs/");  	//DEBUG
		var length = lastDir((string) packpath[0]).Count();
		songs = new Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>>();
		foreach(string el in packpath){
			songs.Add(lastDir(el)[length - 1], new Dictionary<string, Dictionary<Difficulty, Song>>());
			var path = Directory.GetFiles(el).FirstOrDefault(c => c.Contains(".png") || c.Contains(".jpg") || c.Contains(".jpeg"));
			if(!String.IsNullOrEmpty(path)){
				WWW www = new WWW("file://" + path);
				Debug.Log(@path);
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
			string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + DataManager.Instance.DEBUGPATH + "Songs/" + el.Key);		//DEBUG
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
			songs[el.Key].OrderBy(c => c.Key);
		}
		
		
		//DEBUG
		if(songs.Keys.Count < 5){
			var cou = songs.Keys.Count + 1;
			for(int i=0; i<5-cou; i++){
				songs.Add("Empty folder " + i + 1, new Dictionary<string, Dictionary<Difficulty, Song>>());
				bannerPack.Add("Empty folder " + i + 1, (Texture2D) Resources.Load("Cublast"));
			}
		}
		songs.OrderBy(c => c.Key);
		alreadyLoaded = true;
	}
	
	
	
	public void LoadingFromCacheFile(SerializableSongStorage sss){
		bannerPack = new Dictionary<string, Texture2D>();
		//string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + "/Songs/"); 	//RELEASE
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + DataManager.Instance.DEBUGPATH + "Songs/");  	//DEBUG
		var length = lastDir((string) packpath[0]).Count();
		songs = new Dictionary<string, Dictionary<string, Dictionary<Difficulty, Song>>>();
		foreach(string el in packpath){
			songs.Add(lastDir(el)[length - 1], new Dictionary<string, Dictionary<Difficulty, Song>>());
			var path = Directory.GetFiles(el).FirstOrDefault(c => c.Contains(".png") || c.Contains(".jpg") || c.Contains(".jpeg"));
			if(!String.IsNullOrEmpty(path)){
				WWW www = new WWW("file://" + path);
				Debug.Log(@path);
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
			string[] songpath = (string[]) Directory.GetDirectories(Application.dataPath + DataManager.Instance.DEBUGPATH + "Songs/" + el.Key);		//DEBUG
			var lengthsp = lastDir ((string) songpath[0]).Count();
			foreach(string sp in songpath){
				//Debug.Log("new song : " + lastDir(sp)[lengthsp - 1]);
				var dic = new Dictionary<Difficulty, Song>();
				var sameSong = sss.getStore().Where(c => c.packName == el.Key && c.songFileName == lastDir(sp)[lengthsp - 1]);
				if(sameSong.Count > 0){
					
					foreach(var oneSong in sameSong){
						var theUnpackedSong = new Song();
						oneSong.transfertLoad(theUnpackedSong);
						dic.Add(theUnpackedSong.difficulty, theUnpackedSong);
					}
				}else{
					dic = OpenChart.Instance.readChart(sp.Replace('\\', '/'));
				}
				if(dic != null && dic.Count != 0) songs[el.Key].Add(lastDir(sp)[lengthsp - 1] , dic);
				/*Debug.Log("Song : " + lastDir(sp)[lengthsp - 1] + 
					" added / pack : " + el.Key + 
					" / number diff : " + songs[el.Key][lastDir(sp)[lengthsp - 1]].Count() +
					" / number step expert : " + songs[el.Key][lastDir(sp)[lengthsp - 1]][Difficulty.EXPERT].numberOfSteps);*/
					
			}
			songs[el.Key].OrderBy(c => c.Key);
		}
		
		
		//DEBUG
		if(songs.Keys.Count < 5){
			var cou = songs.Keys.Count + 1;
			for(int i=0; i<5-cou; i++){
				songs.Add("Empty folder " + i + 1, new Dictionary<string, Dictionary<Difficulty, Song>>());
				bannerPack.Add("Empty folder " + i + 1, (Texture2D) Resources.Load("Cublast"));
			}
		}
		songs.OrderBy(c => c.Key);
		alreadyLoaded = true;
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
		
		switch(DataManager.Instance.sortMethod){
		
			case Sort.NAME:
				if(previousList.Count == 0){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.title.ToLower().Contains(contains.ToLower())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.title.ToLower().Contains(contains.ToLower()))){
							finalList.Add(song.Value.First().Value.title + "[" + packs.Key + "]", song.Value);
						}
					}
				}else{
					finalList = previousList.Where(r => r.Value.First().Value.title.ToLower().Contains(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
				}
			break;
			
		
			case Sort.STARTWITH:
				if(previousList.Count == 0){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.title.ToLower().StartWith(contains.ToLower())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.title.ToLower().StartWith(contains.ToLower()))){
							finalList.Add(song.Value.First().Value.title + "[" + packs.Key + "]", song.Value);
						}
					}
				}else{
					finalList = previousList.Where(r => r.Value.First().Value.title.ToLower().StartWith(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
				}
			break;
			
			case Sort.ARTIST:
				if(previousList.Count == 0){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.artist.ToLower().Contains(contains.ToLower())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.artist.ToLower().Contains(contains.ToLower()))){
							finalList.Add(song.Value.First().Value.title + "[" + packs.Key + "]", song.Value);
						}
					}
				}else{
					finalList = previousList.Where(r => r.Value.First().Value.artist.ToLower().Contains(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
				}
			break;
			
			
			case Sort.STEPARTIST:
				if(previousList.Count == 0){
					foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.stepartist.ToLower().Contains(contains.ToLower())).Count() > 0)){
						foreach(var song in packs.Value.Where(r => r.Value.First().Value.stepartist.ToLower().Contains(contains.ToLower()))){
							finalList.Add(song.Value.First().Value.title + "[" + packs.Key + "]", song.Value);
						}
					}
				}else{
					finalList = previousList.Where(r => r.Value.First().Value.stepartist.ToLower().Contains(contains.ToLower())).ToDictionary(v => v.Key, v => v.Value);
				}
			break;
			
			//remplacer par des try parse
			case Sort.DIFFICULTY:
				foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.Count(s => s.Value.level == (int)contains) > 0).Count() > 0)){
					foreach(var song in packs.Value.Where(r => r.Value.Count(s => s.Value.level == (int)contains) > 0)){
						finalList.Add(song.Value.First().Value.title + "[" + packs.Key + "]", song.Value);
					}
				}
			break;
			
			//remplacer par des try parse
			case Sort.BPM:
				foreach(var packs in songs.Where(c => c.Value.Where(d => d.Value.First().Value.bpms.Where(e => e.Value == (int)contains) > 0).Count() > 0)){
					foreach(var song in packs.Value.Where(r => r.Value.First().Value.bpms.Where(s => s.Value == (int)contains) > 0)){
						finalList.Add(song.Value.First().Value.title + "[" + packs.Key + "]", song.Value);
					}
				}
			break;
		
		}
		return finalList;
	}
	
	public bool isAllowedToSearch(string search){
		switch(DataManager.Instance.sortMethod){
			case Sort.NAME:
				return search.Trim().Length >= 3;
			break;
			case Sort.STARTWITH:
				return search.Trim().Length >= 1;
			break;
			case Sort.ARTIST:
				return search.Trim().Length >= 3;
			break;
			case Sort.STEPARTIST:
				return search.Trim().Length >= 3;
			break;
			case Sort.DIFFICULTY:
				return search.Trim().Length >= 1;
			break;
			case Sort.BPM:
				return search.Trim().Length >= 1;
			break;
		}
	}
	
	public Dictionary<string, Texture2D> ListTexture(){
		return bannerPack;
	}
	
	
	private void renameSharpFolder(){
		string[] packpath = (string[]) Directory.GetDirectories(Application.dataPath + DataManager.Instance.DEBUGPATH + "Songs/");
		for(int i=0; i< packpath.Length; i++){
			if(packpath[i].Contains("#")){
				Directory.Move(packpath[i], packpath[i].Replace("#", ""));
			}
			string[] songpath = Directory.GetDirectories(packpath[i]);
			for(int j=0; j< songpath.Length; j++){
				if(songpath[i].Contains("#")){
					Directory.Move(songpath[i], songpath[i].Replace("#", ""));
				}
			}
		}
		
	}
	
	
	
	public bool SaveCache () {

		if(!Directory.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache/")){
				Directory.CreateDirectory(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache");
		}
		if(File.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache/" + "dataSong.cache")){
			File.Delete(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache/" + "dataSong.cache");
		}
		
		Stream stream = File.Open(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache/" + "dataSong.cache", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
	    bformatter.Binder = new VersionDeserializationBinder(); 
		
		var sss = new SerializableSongStorage();
		sss.packTheStore();
		try{
			bformatter.Serialize(stream, sss);
			stream.Close();
			
		}catch(Exception e){
			
			stream.Close();
			File.Delete(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache/" + "dataSong.cache");
			sss.getStore().Clear();
			delete sss;
			Debug.Log(e.Message);
			return false;
		}
		sss.getStore().Clear();
		delete sss;
		return true;
		
		
	}
	
	public void LoadCache () {
	
		if(File.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache" + "dataSong.cache")){
			var file = Directory.GetFiles(Application.dataPath + DataManager.Instance.DEBUGPATH + "Cache").FirstOrDefault(c => c.Contains("dataSong.cache"))
			var sss = new SerializableSongStorage ();
			Stream stream = File.Open(file, FileMode.Open);
			BinaryFormatter bformatter = new BinaryFormatter();
			bformatter.Binder = new VersionDeserializationBinder(); 
			sss = (SerializableSongStorage)bformatter.Deserialize(stream);
			stream.Close();
			
			LoadingFromCacheFile(sss);
			sss.getStore().Clear();
			delete sss;
		}
	}
}
