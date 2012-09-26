using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

[Serializable]
public class Profile{
	
	
	
	//General
	public string name;
	public string password;
	public string email;
	public string idFile;
	
	
	//Parameters
	public float globalOffsetSeconds;
	public KeyCode KeyCodeUp;
	public KeyCode KeyCodeDown;
	public KeyCode KeyCodeLeft;
	public KeyCode KeyCodeRight;
	public KeyCode SecondaryKeyCodeUp;
	public KeyCode SecondaryKeyCodeDown;
	public KeyCode SecondaryKeyCodeLeft;
	public KeyCode SecondaryKeyCodeRight;
	public int numberOfSkinSelected;
	public int speedmolette;
	public bool padMode;
	
	//audio
	//video
	
	//Songs
	public List<SongInfoProfil> scoreOnSong;
	
	//Achivement
	public int numberOfAchivement;
	//list achievement : faire une string de 0 et de 1
	//divers paramètres pour les achievements (temps joué, combo, etc...)
	
	
	
	public Profile ()
	{
		scoreOnSong = new List<SongInfoProfil>();
	}
	
	public Profile (string id, string pass, string mail)
	{
		name = id;
		password = pass;
		email = mail;
		scoreOnSong = new List<SongInfoProfil>();
		
		//test
		var test = new SongInfoProfil("songname", "sub", 1002, Difficulty.EXPERT, 15);
		scoreOnSong.Add(test);
	}
	
	public void SaveProfile () {

		if(String.IsNullOrEmpty(idFile)){
			idFile = name + "-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + 
				DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + 
					DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
		}
		if(!Directory.Exists(Application.dataPath + "/../Profiles/")){
				Directory.CreateDirectory(Application.dataPath + "/../Profiles");
		}
		if(File.Exists(Application.dataPath + "/../Profiles/" + idFile + ".profile")){
			File.Move(Application.dataPath + "/../Profiles/" + idFile + ".profile", Application.dataPath + "/../Profiles/" + idFile + ".oldSave");
		}
		
		PlayerPrefs.SetString("idProfile", idFile);
		
		
		Stream stream = File.Open(Application.dataPath + "/../Profiles/" + idFile + ".profile", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
	    bformatter.Binder = new VersionDeserializationBinder(); 
		
		try{
			bformatter.Serialize(stream, this);
			stream.Close();
			
			if(File.Exists(Application.dataPath + "/../Profiles/" + idFile + ".oldSave")){
				File.Delete(Application.dataPath + "/../Profiles/" + idFile + ".oldSave");
			}
		}catch{
			
			stream.Close();
			File.Delete(Application.dataPath + "/../Profiles/" + idFile + ".profile");
			if(File.Exists(Application.dataPath + "/../Profiles/" + idFile + ".oldSave")){
				File.Move(Application.dataPath + "/../Profiles/" + idFile + ".oldSave", Application.dataPath + "/../Profiles/" + idFile + ".profile");
			}
		}
		
		
	}
	
	public void LoadProfiles () {
	
		
		foreach(var file in Directory.GetFiles(Application.dataPath + "/../Profiles").Where(c => c.Contains(".profile"))){
			
			Profile pr = new Profile ();
			Stream stream = File.Open(file, FileMode.Open);
			BinaryFormatter bformatter = new BinaryFormatter();
			bformatter.Binder = new VersionDeserializationBinder(); 
			pr = (Profile)bformatter.Deserialize(stream);
			stream.Close();
			
			ProfileManager.Instance.profiles.Add(pr);
		}
		
	}
	
}


