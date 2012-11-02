using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SerializableSongStorage {

	public List<SerializableSong> store;
	
	
	public SerializableSongStorage(){
		store = new List<SerializableSong>();
	}
	
	public void packTheStore(){
		var allSongs = LoadManager.Instance.ListSong();
		foreach(var packs in allSongs){
			foreach(var songs in packs){
				foreach(var song in songs){
					var ss = new SerializableSong();
					ss.transfertSave(song.Value, packs.Key, songs.Key);
					store.Add(ss);
				}
			}
		}
	}
	
	public List<SerializableSong> getStore(){
		return store;
	}
	
	
	
	
}