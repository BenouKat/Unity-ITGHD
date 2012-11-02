using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class Profile{
	
	
	
	//General
	public string name;
	public string password;
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
	public string lastSpeedmodUsed;
	public string lastBPM;
	public bool inBPMMode;
	public int numberOfSkinSelected;
	
	//general
	public float userGOS;
	public float mouseMolSpeed;
	public bool dancepadMode;
	public bool offsetDebugMode;
	
	//Profiles
	public ProfileDownloadType PDT;
	
	//Audio
	public float generalVolume;
	
	//Video
	public bool enableBloom;
	public bool enableDepthOfField;
	public float antiAliasing;
	
	//Cache
	public bool useTheCacheSystem;
	
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
	
	public Profile (string id, string pass)
	{
		name = id;
		password = pass;
		scoreOnSong = new List<SongInfoProfil>();
		lastSpeedmodUsed = "";
		lastBPM = "";
		inBPMMode = false;
		userGOS = 0.0f;
		mouseMolSpeed = 1f;
		dancepadMode = false;
		offsetDebugMode = false;
		PDT =  ProfileDownloadType.ALL;
		generalVolume = 1f;
		enableBloom = true;
		enableDepthOfField = true;
		useTheCacheSystem = true;
		loadOptions();
	}
	
	public void saveASong(SongInfoProfil sip, float scoreEarned, double speedmodPref, bool fail){
		SongInfoProfil thesip = sip.Copy();
		thesip.score = scoreEarned;
		thesip.speedmodpref = speedmodPref;
		thesip.fail = fail;
		if(scoreOnSong.Any(c => c.CompareId(thesip))){
			var theold = scoreOnSong.FirstOrDefault(c => c.CompareId(thesip));
			if(theold.score < scoreEarned){
				scoreOnSong.Remove(theold);
				scoreOnSong.Add(thesip);	
			}else{
				theold.speedmodpref = speedmodPref;
			}
		}else{
			scoreOnSong.Add(thesip);	
		}
		
	}
	
	public void loadOptions(){
	
		DataManager.Instance.userGOS = this.userGOS;
		DataManager.Instance.mouseMolSpeed = this.mouseMolSpeed;
		DataManager.Instance.dancepadMode = this.dancepadMode;
		DataManager.Instance.offsetDebugMode = this.offsetDebugMode;
		DataManager.Instance.PDT = this.PDT;
		DataManager.Instance.generalVolume = this.generalVolume;
		DataManager.Instance.enableBloom = this.enableBloom;
		DataManager.Instance.enableDepthOfField = this.enableDepthOfField;
		DataManager.Instance.useTheCacheSystem = this.useTheCacheSystem;
	}
	
}


