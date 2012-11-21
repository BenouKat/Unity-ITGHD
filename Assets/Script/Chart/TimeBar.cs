using UnityEngine;
using System.Collections;

public class TimeBar : MonoBehaviour {
	
	public GameObject hitBar;
	public Transform cursorTimeObject;
	private Material hitBarMaterial;
	private Material cubeHitBarMaterial;
	
	public float ecartWithBeginY;
	public float endY;
	
	private float timeBegin;
	private float timeEnd;
	
	private Color black = new Color(0f, 0f, 0f, 1f);
	private Color white = new Color(1f, 1f, 1f, 1f);
	private float lerpColor;
	
	public ParticuleSystem psFFC;
	public ParticuleSystem psFEC;
	public ParticuleSystem psBigCombo;
	public ParticuleSystem psLowCombo;
	public ParticuleSystem psBadCombo;
	
	void Start () {
		lerpColor = 1f;
		psLowCombo.Play();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(lerpColor < 1f){
			hitBarMaterial.color = Color.Lerp(hitBarMaterial.color, black, lerpColor);
			cubeHitBarMaterial.color =  Color.Lerp(hitBarMaterial.color, white, lerpColor);
		}
		
		
	}
	
	
	public void HitBar(Precision prec){
		hitBarMaterial.color = DataManager.Instance.precColor[(int)prec];
		cubeHitBarMaterial.color = DataManager.Instance.precColor[(int)prec];
		lerpColor = 0f;
	}
	
	public void setDuration(float begin, float end){
		timeBegin = begin;
		timeEnd = end;
	}
	
	public void updateTimeBar(double timetotal){
		if(timetotal >= timeBegin){
			cursorTimeObject.position = new Vector3(cursorTimeObject.position.x, endY - ecartWithBeginY*((timetotal - timeBegin)/timeEnd), cursorTimeObject.position.z);
		}
	}
	
	public void updatePS(ComboType ct, int combo){
		if(combo >= 100 && psLowCombo.isPlaying){
			psLowCombo.Stop();
			if(ct == ComboType.FULLFANTASTIC){
				if(!psFFC.gameObject.active) psFFC.gameObject.active = true;
				psFFC.Play();
			}else if(ct == ComboType.FULLEXCELLENT){
				if(!psFEC.gameObject.active) psFEC.gameObject.active = true;
				psFEC.Play();
			}else{
				if(!psBigCombo.gameObject.active) psBigCombo.gameObject.active = true;
				psBigCombo.Play();
			}
		}else if(combo >= 40 && psBadCombo.isPlaying){
			if(!psLowCombo.gameObject.active) psLowCombo.gameObject.active = true;
			psBadCombo.Stop();
			psLowCombo.Play();
		}
	}
	
	public void breakPS(){
		psFFC.Stop();
		psFEC.Stop();
		psBigCombo.Stop();
		psLowCombo.Stop();
		if(!psBadCombo.gameObject.active) psBadCombo.gameObject.active = true;
		psBadCombo.Play();
	}
	
	
}
