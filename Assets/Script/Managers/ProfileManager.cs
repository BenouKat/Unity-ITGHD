using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProfileManager{
	
	private static ProfileManager instance;
	
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
	}
	
	public bool verifyCurrentProfile(){
		
		if(PlayerPrefs.HasKey("idProfile")){
			currentProfile = profiles.FirstOrDefault(c => c.idFile == PlayerPrefs.GetString("idProfile"));
			return true;
		}
		return false;
	}
}


