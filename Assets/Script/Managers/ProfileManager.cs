using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Reflection;

public class ProfileManager{
	
	private static ProfileManager instance;
	
	public bool alreadyLoaded;
	
	public static ProfileManager Instance{
		get{
			if(instance == null){ 
				instance = new ProfileManager();
			}
			return instance;
		}
	}
	
	
	public List<Profile> profiles;
	public Profile currentProfile;
	
	private ProfileManager ()
	{
		profiles = new List<Profile>();
		alreadyLoaded = false;
	}
	
	public void CreateTestProfile(){
		//Test
		var p1 = new Profile("UserTest" + ((int)(UnityEngine.Random.value*100)).ToString(), "baba");
		
		profiles.Add(p1);
		currentProfile = p1;	
		SaveProfile();
	}
	
	public bool verifyCurrentProfile(){
		
		if(PlayerPrefs.HasKey("idProfile")){
			if(profiles.FirstOrDefault(c => c.idFile == PlayerPrefs.GetString("idProfile")) != null){
				currentProfile = profiles.FirstOrDefault(c => c.idFile == PlayerPrefs.GetString("idProfile"));
				currentProfile.loadOptions();
				return true;
			}
		}
		return false;
	}
	
	public void setCurrentProfile(Profile p){
		
		currentProfile = p;
		currentProfile.loadOptions();
		PlayerPrefs.SetString("idProfile", currentProfile.idFile);
	}
	
	public bool SaveProfile () {

		if(String.IsNullOrEmpty(currentProfile.idFile)){
			currentProfile.idFile = currentProfile.name + "-" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + 
				DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + 
					DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString();
		}
		if(!Directory.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/")){
				Directory.CreateDirectory(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles");
		}
		if(File.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".profile")){
			File.Move(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".profile", Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".oldSave");
		}
		
		PlayerPrefs.SetString("idProfile", currentProfile.idFile);
		
		
		Stream stream = File.Open(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".profile", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
	    bformatter.Binder = new VersionDeserializationBinder(); 
		
		try{
			bformatter.Serialize(stream, currentProfile);
			stream.Close();
			
			if(File.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".oldSave")){
				File.Delete(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".oldSave");
			}
		}catch(Exception e){
			
			stream.Close();
			File.Delete(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".profile");
			if(File.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".oldSave")){
				File.Move(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".oldSave", Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + currentProfile.idFile + ".profile");
			}
			Debug.Log(e.Message);
			return false;
		}
		return true;
		
		
	}
	
	public byte[] getProfileStream()
	{
		BinaryFormatter bf = new BinaryFormatter();
		bf.Binder = new VersionDeserializationBinder();
		MemoryStream ms = new MemoryStream();
		bf.Serialize(ms, currentProfile);
		
		return ms.ToArray();
	}
	
	public string saveProfileStream(byte[] stream)
	{
		Profile pr = new Profile();
		MemoryStream ms = new MemoryStream(stream);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Binder = new VersionDeserializationBinder();
		pr = (Profile)bf.Deserialize(ms);
		var name = "";
		if(File.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + pr.idFile + ".profile"))
		{
			File.Delete(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + pr.idFile + ".profile");	
			name = "updates ";
		}else
		{
			name = "gets ";
		}
		Stream fstream = File.Open(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + pr.idFile + ".profile", FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
	    bformatter.Binder = new VersionDeserializationBinder(); 
		name += pr.name;
		try{
			bformatter.Serialize(fstream, pr);
			fstream.Close();
		}catch(Exception e){
			
			fstream.Close();
			File.Delete(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles/" + pr.idFile + ".profile");
			Debug.Log(e.Message);
		}
		
		return name;
	}
	
	public void LoadProfiles () {
		
		if(Directory.Exists(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles")){
			foreach(var file in Directory.GetFiles(Application.dataPath + DataManager.Instance.DEBUGPATH + "Profiles").Where(c => c.Contains(".profile"))){
				
				Profile pr = new Profile ();
				Stream stream = File.Open(file, FileMode.Open);
				BinaryFormatter bformatter = new BinaryFormatter();
				bformatter.Binder = new VersionDeserializationBinder(); 
				pr = (Profile)bformatter.Deserialize(stream);
				stream.Close();
				
				ProfileManager.Instance.profiles.Add(pr);
			}
			alreadyLoaded = true;
		}
	}
	
	public KeyValuePair<double, string> FindTheBestScore(SongInfoProfil sip){
		double score = -1;
		var name = "-";
		var others = 0;
		for(int i=0;i<profiles.Count;i++){
			var fod = profiles[i].scoreOnSong.FirstOrDefault(c => c.CompareId(sip));
			if(fod != null && !fod.fail){
				if(score < fod.score){
					score = fod.score;
					name = profiles[i].name;
					others = 0;
				}else if(score == fod.score){
					others++;	
				}
			}
		}
		
		if(others > 0) name += "\n(+ " + others + ")";
		return new KeyValuePair<double, string>(score, name);
		
	}
	
	
	public SongInfoProfil FindTheSongStat(SongInfoProfil sip){
		if(currentProfile.scoreOnSong.Count > 0){
			var fod = currentProfile.scoreOnSong.FirstOrDefault(c => c.CompareId(sip));
			return fod;
		}
		return null;
	}
	
	public int SearchPlaceOnSong(SongInfoProfil sip, float scoreEarned){
		var allChallenger = profiles.Where(c => c.scoreOnSong.Any(d => d.CompareId(sip))).OrderByDescending(c => c.scoreOnSong.FirstOrDefault(d => d.CompareId(sip)).score).ToList();
		var thesip = FindTheSongStat(sip);
		if(allChallenger.Count <= 1){
			if(thesip != null && scoreEarned > thesip.score) return 0;
			return -1;
		}else if(scoreEarned > thesip.score){
			if(allChallenger.ElementAt(0).scoreOnSong.FirstOrDefault(d => d.CompareId(sip)).score <= scoreEarned){
				return 1;
			}
			if(allChallenger.ElementAt(1).scoreOnSong.FirstOrDefault(d => d.CompareId(sip)).score <= scoreEarned){
				return 2;
			}
			if(allChallenger.ElementAt(2).scoreOnSong.FirstOrDefault(d => d.CompareId(sip)).score <= scoreEarned){
				return 3;
			}
			return 0;
		}
		return -1;
	}
}


