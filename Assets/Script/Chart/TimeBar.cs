using UnityEngine;
using System.Collections;

public class TimeBar : MonoBehaviour {
	
	public GameObject hitBar;
	public GameObject hitBar2;
	public GameObject cubeHitBar;
	public Transform cursorTimeObject;
	private Material hitBarMaterial;
	private Material hitBar2Material;
	private Material cubeHitBarMaterial;
	
	public float ecartWithBeginY;
	public float beginY;
	
	private float timeBegin;
	private float timeEnd;
	
	private Color black = new Color(0.4f, 0.4f, 0.4f, 1f);
	private Color white = new Color(1f, 1f, 1f, 1f);
	private float lerpColor;
	public float speedLerpColor;
	
	public ParticleSystem psFFC;
	public ParticleSystem psFEC;
	public ParticleSystem psBigCombo;
	public ParticleSystem psLowCombo;
	public ParticleSystem psBadCombo;
	public ParticleSystem psCrashCombo;
	
	private bool activeCombo;
	
	public UILabel labelTime;
	
	private Vector3 poolVector;
	
	void Start () {
		lerpColor = 1f;
		psLowCombo.Play();
		activeCombo = true;
		hitBarMaterial = hitBar.renderer.material;
		hitBar2Material = hitBar2.renderer.material;
		cubeHitBarMaterial = cubeHitBar.renderer.material;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(lerpColor < 1f){
			lerpColor += Time.deltaTime*speedLerpColor;
			hitBarMaterial.color = Color.Lerp(hitBarMaterial.color, black, lerpColor);
			hitBar2Material.color = Color.Lerp(hitBar2Material.color, black, lerpColor);
		}
		
		
	}
	
	
	public void HitBar(Precision prec){
		hitBarMaterial.color = DataManager.Instance.precColor[(int)prec];
		hitBar2Material.color = DataManager.Instance.precColor[(int)prec];
		lerpColor = 0f;
	}
	
	public void setDuration(float begin, float end){
		timeBegin = begin;
		timeEnd = end;
		refreshLabel(end);
	}
	
	public void updateTimeBar(float timetotal){
		if(timetotal >= timeBegin){
			poolVector.x = cursorTimeObject.position.x;
			poolVector.y = beginY + ecartWithBeginY*((timetotal - timeBegin)/(timeEnd - timeBegin));
			poolVector.z = cursorTimeObject.position.z;
			cursorTimeObject.position = poolVector;
			refreshLabel(timeEnd - timetotal);
		}
	}
	
	public void updatePS(ComboType ct, int combo){
		if(combo >= 100){
			if(psLowCombo.isPlaying) psLowCombo.Stop();
			if(ct == ComboType.FULLFANTASTIC){
				if(!psFFC.gameObject.active) psFFC.gameObject.active = true;
				if(!psFFC.isPlaying) psFFC.Play();
			}else if(ct == ComboType.FULLEXCELLENT){
				if(!psFEC.gameObject.active) psFEC.gameObject.active = true;
				if(!psFEC.isPlaying) psFEC.Play();
				if(psFFC.isPlaying) psFFC.Stop();
			}else{
				if(!psBigCombo.gameObject.active) psBigCombo.gameObject.active = true;
				if(psFFC.isPlaying) psFFC.Stop();
				if(psFEC.isPlaying) psFEC.Stop();
				if(!psBigCombo.isPlaying) psBigCombo.Play();
			}
		}else if(combo >= 25 && !activeCombo){
			if(!psLowCombo.gameObject.active) psLowCombo.gameObject.active = true;
			psBadCombo.Stop();
			psLowCombo.Play();
			activeCombo = true;
		}
	}
	
	public void breakPS(){
		if(activeCombo){
			activeCombo = false;
			psFFC.Stop();
			psFEC.Stop();
			psBigCombo.Stop();
			psLowCombo.Stop();
			if(!psBadCombo.gameObject.active) psBadCombo.gameObject.active = true;
			psBadCombo.Play();
			if(!psCrashCombo.gameObject.active) psCrashCombo.gameObject.active = true;
			psCrashCombo.Play();
		}
	}
	
	public void refreshLabel(float timeArgs)
	{
		if(timeArgs >= 0f)
		{
			labelTime.text = (timeArgs/60f).ToString("0") + ":" + (timeArgs%60).ToString("00");
		}else
		{
			labelTime.text = "0:00";	
		}
	}
	
	
}
