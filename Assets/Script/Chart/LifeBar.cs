using UnityEngine;
using System.Collections;

public class LifeBar : MonoBehaviour {
	

	

	
	
	
	
	#region oldcode
	/*
	//public float speedLerp;
	public float speedclignotement;
	private float signClignotement;
	private float thecolor;
	public float thelerp;
	public float limit;
	
	private GameObject goLifeBar;
	public Material goLifeBarSoclecolor;
	private ParticleSystem psLowSocle;
	private ParticleSystem psMaxLifeSocle;
	private ParticleSystem psMaxLife;
	private ParticleSystem psLifeUp;
	
	private float realLife;
	private float objectivLife;
	private float thelostlife;
	
	void Start () {
		goLifeBar = (GameObject) gameObject.transform.FindChild("LifeBar").gameObject;
		psLowSocle = (ParticleSystem) gameObject.transform.FindChild("Smoke").particleSystem;
		psMaxLifeSocle = (ParticleSystem) gameObject.transform.FindChild("SmokeLight").particleSystem;
		psLifeUp = (ParticleSystem) gameObject.transform.FindChild("LifeUp").particleSystem;
		psMaxLife = (ParticleSystem) gameObject.transform.FindChild("LifeMax").particleSystem;
		realLife = 40f;
		objectivLife = 50f;
		signClignotement = 1f;
		thecolor = 1f;
		//thelerp = 0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(realLife != objectivLife){
			goLifeBar.transform.localScale = new Vector3(2f, realLife/10f, 2f);
			goLifeBar.transform.position = new Vector3(0f, -(10f - realLife/10f), 20f);
			var r = 0f;
			var g = 0f;
			var b = 0f;
			if(realLife < 50f){
				r = 1f;
				g = realLife <= 25f ? 0f : (realLife - 25f)/25f;
				b = 0f;
			}
			if(realLife >= 50f && realLife < 75f){
				r = 1 - ((realLife - 50f)/25f);
				g = 1f;
				b = 0f;
			}
			
			if(realLife >= 75f){
				r = 0f;
				g = 1f;
				b = (realLife - 75f)/26f;
			}
			
			goLifeBar.renderer.material.color = new Color(r,g,b, 1f);
			
			var pos = -((5f - goLifeBar.transform.localScale.y))*2f; 
			psLifeUp.transform.localPosition = new Vector3(0f, pos, 15f);
			
			realLife = Mathf.Lerp(realLife, objectivLife, thelerp);
			if(Mathf.Abs(realLife - objectivLife) < limit) realLife = objectivLife;
		}
		
		if(realLife < 25f){
			thecolor += signClignotement*(Time.deltaTime/speedclignotement);
			if(thecolor < 0f || thecolor > 1f){
				thecolor = thecolor < 0f ? 0f : 1f;
				signClignotement *= -1f;
			}
			goLifeBarSoclecolor.color = new Color(1f, thecolor, thecolor, 1f);	
		}else if(realLife >= 100f){
			goLifeBarSoclecolor.color = new Color(0f, 0.5f, 1f, 1f);
		}else{
			goLifeBarSoclecolor.color = new Color(1f, 1f, 1f, 1f);
		}
		
		
	}
	
	
	public void ChangeBar(float newlife){
		objectivLife = newlife;
		if(newlife >= 100f && !psMaxLife.isPlaying){
			if(psLifeUp.isPlaying) psLifeUp.Stop(); 
			psMaxLife.Play();
			psMaxLifeSocle.Play();
		}else if(newlife > realLife && newlife > (thelostlife + 10f) && !psLifeUp.isPlaying){
			psLifeUp.Play();
		}else if(newlife < realLife){
			if(psLifeUp.isPlaying) psLifeUp.Stop();
			if(psMaxLife.isPlaying) psMaxLife.Stop();
			if(psMaxLifeSocle.isPlaying) psMaxLifeSocle.Stop();
			thelostlife = newlife;
			
		}
		
		if(newlife <= 25f && !psLowSocle.isPlaying){
			psLowSocle.Play();
		}else if(newlife > 25f && psLowSocle.isPlaying){
			psLowSocle.Stop();
		}
		
		
	}*/
	#endregion
	

	
	//new LifeBar system
	public GameObject[] lifebar;
	public Material matLifeBar;
	private Material[] matCube;
	private float[] decals;
	
	public float speedBrillance;
	public float maxBrillance;
	public float lowBrillance;
	public Vector3[] baseScale;
	
	public ParticleSystem psMaxLife;
	public ParticleSystem psLifeUp;
	public ParticleSystem psLowLife;
	
	private float realLife;
	private float objectivLife;
	private float thelostlife;
	public float thelerp;
	public float limit;
	
	private bool lifeUpPlayin;
	private bool lifeMaxPlayin;
	private bool lifeLowPlayin;
	
	private float numberStep;
	
	void Start () {
		matCube = new Material[lifebar.Length];
		decals = new float[lifebar.Length];
		
		realLife = 40f;
		objectivLife = 50f;
		numberStep = (75f/(float)(lifebar.Length - 1));
		Debug.Log(numberStep);
		var index = realLife <= 25 ? 0 : ((int)((realLife - 25f)/numberStep) + 1);
		
		for(int i=0; i<lifebar.Length; i++){
			matCube[i] = lifebar[i].renderer.material;
			decals[i] = 1f - (i/lifebar.Length - 1);
			if(i < index){
				lifebar[i].transform.localScale = baseScale[i];
			}else if(i > index){
				lifebar[i].transform.localScale = baseScale[i]*0f;
			}else if(index == i && i == 0){
				lifebar[index].transform.localScale = baseScale[index]*(realLife/25f);
			}else{
				lifebar[index].transform.localScale = baseScale[index]*(((realLife - 25f)%numberStep)/numberStep);
			}
		}
		
		lifeUpPlayin = false;
		lifeMaxPlayin = false;
		lifeLowPlayin = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(realLife != objectivLife){
			var r = 0f;
			var g = 0f;
			var b = 0f;
			if(realLife < 50f){
				r = 1f;
				g = realLife <= 25f ? 0f : (realLife - 25f)/25f;
				b = 0f;
			}else
			if(realLife >= 50f && realLife < 75f){
				r = 1 - ((realLife - 50f)/25f);
				g = 1f;
				b = 0f;
			}else
			
			if(realLife >= 75f){
				r = 0f;
				g = 1f;
				b = (realLife - 75f)/26f;
			}
			
			for(int i=0; i<lifebar.Length; i++){
				matCube[i].color = new Color(r,g,b, 1f);
			}
			//matLifeBar.color = new Color(r,g,b, 1f);
			
			if(realLife <= 25f){
				lifebar[0].transform.localScale = baseScale[0]*(realLife/25f);
			}else if(realLife < 100f){
				var index = (int)((realLife - 25f)/numberStep) + 1;
				lifebar[index - 1].transform.localScale = baseScale[index - 1];
				lifebar[index].transform.localScale = baseScale[index]*(((realLife - 25f)%numberStep)/numberStep);
				if(index != lifebar.Length - 1) lifebar[index + 1].transform.localScale = baseScale[index + 1]*0f;
			}else{
				lifebar[lifebar.Length - 1].transform.localScale = baseScale[lifebar.Length - 1];
			}
			
			realLife = Mathf.Lerp(realLife, objectivLife, thelerp);
			if(Mathf.Abs(realLife - objectivLife) < limit) realLife = objectivLife;
		}
		/*
		for(int i=0; i<matCube.Length; i++){
			matCube[i].SetFloat("_Shininess", lowBrillance + Mathf.PingPong((Time.time*speedBrillance) + decals[i], maxBrillance - lowBrillance));
		}*/
	}
	
	
	public void ChangeBar(float newlife){
		objectivLife = newlife;
		if(newlife >= 100f && !lifeMaxPlayin){
			psLifeUp.Stop(); 
			lifeUpPlayin = false;
			if(!psMaxLife.gameObject.active) psMaxLife.gameObject.active = true;
			psMaxLife.Play();
			lifeMaxPlayin = true;
		}else if(newlife > realLife && newlife > 25f && !lifeUpPlayin && newlife > (thelostlife + 10f)){
			if(!psLifeUp.gameObject.active) psLifeUp.gameObject.active = true;
			psLifeUp.Play();
			lifeUpPlayin = true;
		}else if(newlife < realLife){
			psLifeUp.Stop();
			lifeUpPlayin = false;
			psMaxLife.Stop();
			lifeMaxPlayin = false;
			thelostlife = newlife;
		}
		
		if(newlife <= 25f && !lifeLowPlayin){
			if(!psLowLife.gameObject.active) psLowLife.gameObject.active = true;
			psLowLife.Play();
			lifeLowPlayin = true;
		}else if(newlife > 25f && lifeLowPlayin){
			psLowLife.Stop();
			lifeLowPlayin = false;
		}
		
		
	}
	
	
	
	
	
	
	
}
