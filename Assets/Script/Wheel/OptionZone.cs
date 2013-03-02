using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class OptionZone : MonoBehaviour {
	
	
	//Option mode
	private bool[] stateLoading;
	public float timeOption;
	public Rect posOptionTitle;
	public float offsetYOption;
	public LineRenderer[] optionSeparator;
	private Material matCache;
	private float fadeAlphaOptionTitle;
	
	//Option mode Items
	public Rect[] posItem;
	public Rect[] posItemLabel;
	public Rect selectedImage;
	public float offsetXDisplayBPMSwitch;
	public float offsetXDisplayBPMValue;
	public float offsetWidthDisplayBPMSV;
	public float offsetXDisplay;
	public float borderXDisplay;
	public float offsetSpeedRateX;
	public float ecartForBack;
	private float speedmodSelected;
	private float rateSelected;
	private Judge scoreJudgeSelected;
	private Judge hitJudgeSelected;
	private Judge lifeJudgeSelected;
	private int skinSelected;
	private int raceSelected;
	private bool[] displaySelected;
	private int deathSelected;
	private string speedmodstring;
	private string bpmstring;
	private string ratestring;
	private bool speedmodok;
	private bool rateok;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	IEnumerator startOptionFade(){
		
		var posInit = cacheOption.transform.position;
		cacheOption.active = true;
		for(int i=0;i<stateLoading.Length;i++){
			matCache.color = new Color(matCache.color.r, matCache.color.g, matCache.color.b, 1f);
			stateLoading[i] = true;
			if(i == 0) optionSeparator[0].enabled = true;
			optionSeparator[i+1].enabled = true;
			fadeAlphaOptionTitle = 1f;
			yield return new WaitForSeconds(timeOption);
			cacheOption.transform.Translate(0f, -2f, 0f);
		}
		cacheOption.active = false;
		cacheOption.transform.position = posInit;
		animok = true;
	}
	
	IEnumerator endOptionFade(){
		
		var posInit = cacheOption.transform.position;
		cacheOption.transform.Translate(0f, -16f, 0f);
		cacheOption.active = true;
		for(int i=stateLoading.Length-1;i>=0;i--){
			matCache.color = new Color(matCache.color.r, matCache.color.g, matCache.color.b, 1f);
			stateLoading[i] = false;
			if(i == stateLoading.Length-1) optionSeparator[optionSeparator.Length-1].enabled = false;
			optionSeparator[i].enabled = false;
			fadeAlphaOptionTitle = 1f;
			yield return new WaitForSeconds(timeOption);
			cacheOption.transform.Translate(0f, 2f, 0f);
		}
		OptionMode = false;
		movinNormal = true;
		cacheOption.active = false;
		cacheOption.transform.position = posInit;
		animok = true;
	}
	
	IEnumerator OptionAnim(int i, bool reverse){
		isFading[i] = true;
		
		if(reverse){
			offsetFading[i] = -offsetBaseFading;
			offsetPreviousFading[i] = 0f;
			alphaText[i] = 0f;
			while(offsetFading[i] < 0){
				offsetFading[i] += offsetBaseFading*Time.deltaTime/timeFadeOut;
				offsetPreviousFading[i] -= offsetBaseFading*Time.deltaTime/timeFadeOut;
				alphaText[i] += Time.deltaTime/timeFadeOut;
				
				yield return new WaitForFixedUpdate();
			}
			
		}else{
			offsetFading[i] = offsetBaseFading;
			offsetPreviousFading[i] = 0f;
			alphaText[i] = 0f;
			while(offsetFading[i] > 0f){
				offsetFading[i] -= offsetBaseFading*Time.deltaTime/timeFadeOut;
				offsetPreviousFading[i] += offsetBaseFading*Time.deltaTime/timeFadeOut;
				alphaText[i] += Time.deltaTime/timeFadeOut;
				
				yield return new WaitForFixedUpdate();
			}
			
		}
		
		isFading[i] = false;
		offsetFading[i] = 0f;
		offsetPreviousFading[i] = offsetBaseFading;
		alphaText[i] = 1f;
	}
	
	IEnumerator OptionAnimDisplay(int i, bool reverse){
		isFadingDisplay[i] = true;
		
		if(reverse){
			alphaDisplay[i] = 0.5f;
			while(alphaDisplay[i] > 0){
				alphaDisplay[i] -= Time.deltaTime/timeFadeOutDisplay;
				yield return new WaitForFixedUpdate();
			}
			alphaDisplay[i] = 0f;
		}else{
			alphaDisplay[i] = 0f;
			while(alphaDisplay[i] < 0.5f){
				alphaDisplay[i] += Time.deltaTime/timeFadeOutDisplay;
				yield return new WaitForFixedUpdate();
			}
			alphaDisplay[i] = 0.5f;
		}
		
		isFadingDisplay[i] = false;
		
	}
}
