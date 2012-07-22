using UnityEngine;
using System.Collections;

public class LifeBar : MonoBehaviour {
	
	private GameObject goLifeBar;
	private Material goLifeBarSoclecolor;
	private ParticleSystem psLowSocle;
	private ParticleSystem psMaxLifeSocle;
	private ParticleSystem psMaxLife;
	private ParticleSystem psLifeUp;
	
	private float realLife;
	private float objectivLife;
	private float thelostlife;
	
	
	//public float speedLerp;
	public float speedclignotement;
	private float signClignotement;
	private float thecolor;
	public float thelerp;
	public float limit;
	// Use this for initialization
	void Start () {
		goLifeBar = (GameObject) gameObject.transform.FindChild("LifeBar").gameObject;
		goLifeBarSoclecolor = (Material) gameObject.transform.FindChild("LifeBocalSocle").renderer.material;
		psLowSocle = (ParticleSystem) gameObject.transform.FindChild("Smoke").particleSystem;
		psMaxLifeSocle = (ParticleSystem) gameObject.transform.FindChild("SmokeLight").particleSystem;
		psLifeUp = (ParticleSystem) gameObject.transform.FindChild("LifeUp").particleSystem;
		psMaxLife = (ParticleSystem) gameObject.transform.FindChild("LifeMax").particleSystem;
		realLife = 50f;
		objectivLife = 50f;
		signClignotement = 1f;
		thecolor = 1f;
		//thelerp = 0f;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		goLifeBar.transform.localScale = new Vector3(2f, realLife/10f, 2f);
		goLifeBar.transform.position = new Vector3(0f, -(10f - realLife/10f), 20f);
		var r = 0f;
		var g = 0f;
		var b = 0f;
		if(realLife < 33f){
			r = 1f;
			g = realLife <= 20f ? 0f : (realLife - 20f)/13f;
			b = 0f;
		}
		if(realLife >= 33f && realLife < 66f){
			r = 1 - ((realLife - 33f)/33f);
			g = 1f;
			b = 0f;
		}
		
		if(realLife >= 66f){
			r = 0f;
			g = 1f;
			b = (realLife - 66f)/34f;
		}
		
		goLifeBar.renderer.material.color = new Color(r,g,b, 1f);
		if(realLife < 20f){
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
		
		if(realLife != objectivLife){
			realLife = Mathf.Lerp(realLife, objectivLife, thelerp);
			if(Mathf.Abs(realLife - objectivLife) < limit) realLife = objectivLife;
		}
		
		psLifeUp.transform.localPosition = new Vector3(0f, -((5f - goLifeBar.transform.localScale.y))*2f , 20f);
		
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
		
		if(newlife <= 20f && !psLowSocle.isPlaying){
			psLowSocle.Play();
		}else if(newlife > 20f && psLowSocle.isPlaying){
			psLowSocle.Stop();
		}
		
		
	}
}
