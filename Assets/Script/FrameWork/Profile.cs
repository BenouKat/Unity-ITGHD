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
	public int mouseMolSpeed;
	public bool dancepadMode;
	public bool quickMode;
	public bool useTheCacheSystem;
	
	//Profiles
	public ProfileDownloadType PDT;
	
	//Audio
	public float generalVolume;
	
	//Video
	public bool enableBloom;
	public bool enableDepthOfField;
	public int antiAliasing;
	
	
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
		mouseMolSpeed = 1;
		dancepadMode = false;
		quickMode = false;
		PDT =  ProfileDownloadType.ALL;
		generalVolume = 1f;
		enableBloom = true;
		enableDepthOfField = true;
		useTheCacheSystem = true;
		antiAliasing = QualitySettings.antiAliasing;
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
		DataManager.Instance.quickMode = this.quickMode;
		DataManager.Instance.PDT = this.PDT;
		DataManager.Instance.generalVolume = this.generalVolume;
		DataManager.Instance.enableBloom = this.enableBloom;
		DataManager.Instance.enableDepthOfField = this.enableDepthOfField;
		DataManager.Instance.antiAliasing = this.antiAliasing;
		DataManager.Instance.useTheCacheSystem = this.useTheCacheSystem;
	}
	
	public void saveOptions(){
	
		this.userGOS = DataManager.Instance.userGOS;
		this.mouseMolSpeed = DataManager.Instance.mouseMolSpeed;
		this.dancepadMode = DataManager.Instance.dancepadMode;
		this.quickMode = DataManager.Instance.quickMode;
		this.PDT = DataManager.Instance.PDT;
		this.generalVolume = DataManager.Instance.generalVolume;
		this.enableBloom = DataManager.Instance.enableBloom;
		this.enableDepthOfField = DataManager.Instance.enableDepthOfField;
		this.antiAliasing = DataManager.Instance.antiAliasing;
		this.useTheCacheSystem = DataManager.Instance.useTheCacheSystem;
	}
	
}


