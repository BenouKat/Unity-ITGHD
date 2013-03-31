using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class OptionZoneLAN : MonoBehaviour {
	
	private GeneralScriptLAN gs;
	
	public GameObject cacheOption;
	
	//Option mode
	private bool[] stateLoading;
	public float timeOption = 0.1f;
	public Rect posOptionTitle = new Rect(0.1f, 0.2325f, 0.12f, 0.07f);
	public float offsetYOption = 0.067f;
	public LineRenderer[] optionSeparator;
	private Material matCache;
	private float fadeAlphaOptionTitle;
	
	//Option mode Items
	public Rect[] posItem = new Rect[9]{new Rect(0.3f, 0.2425f, 0.05f, 0.05f), new Rect(0.3f, 0.3075f, 0.05f, 0.05f), new Rect(0.45f, 0.38f, 0.05f, 0.05f),
		new Rect(0.45f, 0.445f, 0.05f, 0.05f), new Rect(0.45f, 0.51f, 0.05f, 0.05f), new Rect(0.45f, 0.575f, 0.05f, 0.05f),
		new Rect(0.15f, 0.655f, 0.05f, 0.05f), new Rect(0.45f, 0.71f, 0.05f, 0.05f), new Rect(0.45f, 0.775f, 0.05f, 0.05f)};
	public Rect[] posItemLabel = new Rect[9]{new Rect(0.22f, 0.25f, 0.5f, 0.5f), new Rect(0.22f, 0.32f, 0.5f, 0.5f), new Rect(0.22f, 0.385f, 0.5f, 0.5f),
		new Rect(0.22f, 0.45f, 0.5f, 0.5f), new Rect(0.22f, 0.515f, 0.5f, 0.5f), new Rect(0.22f, 0.58f, 0.5f, 0.5f),
		new Rect(0f, 0f, 0f, 0f), new Rect(0.22f, 0.715f, 0.5f, 0.5f), new Rect(0.22f, 0.78f, 0.5f, 0.5f)};
	public Rect selectedImage = new Rect(0.0015f, 0f, 0.05f, 0.05f);
	public float offsetXDisplayBPMSwitch = 0.35f;
	public float offsetXDisplayBPMValue = 0.46f;
	public float offsetWidthDisplayBPMSV = 0.08f;
	public float offsetXDisplay = 0.065f;
	public float borderXDisplay = -0.08f;
	public float offsetSpeedRateX = 0f;
	public float ecartForBack = 0.15f;
	private int difficultySelected;
	private Judge scoreJudgeSelected;
	private Judge hitJudgeSelected;
	private Judge lifeJudgeSelected;
	private int skinSelected;
	private int raceSelected;
	private bool[] displaySelected;
	private int deathSelected;
	private bool speedmodok;
	private bool rateok;
	private string ratestring;
	
	//Anim options
	public float timeFadeOut = 0.15f;
	public float timeFadeOutDisplay = 0.15f;
	private float[] alphaText;
	private float[] alphaDisplay;
	private float[] offsetFading;
	private float[] offsetPreviousFading;
	private bool[] isFading;
	private bool[] isFadingDisplay;
	private int previousSelected;
	public float offsetBaseFading = 0.05f;
	private bool animok;
	
	public bool activeModule;
	// Use this for initialization
	void Start () {
		gs = GetComponent<GeneralScriptLAN>();
		activeModule = false;
		speedmodok = true;
		rateok = true;
		animok = true;
		
		matCache = cacheOption.renderer.material;
		
		fadeAlphaOptionTitle = 1f;
		stateLoading = new bool[9];
		displaySelected = new bool[DataManager.Instance.aDisplay.Length];
		for(int i=0;i<stateLoading.Length-1;i++) stateLoading[i] = false;
		for(int j=0;j<DataManager.Instance.aDisplay.Length;j++) displaySelected[j] = DataManager.Instance.songSelected != null ? DataManager.Instance.displaySelected[j] : false;
		
		
		
		
		DataManager.Instance.BPMEntryMode = ProfileManager.Instance.currentProfile.inBPMMode;
		
		difficultySelected = (int)gs.getZoneInfo().getActualySelected();
		scoreJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.scoreJudgeSelected : Judge.NORMAL;
		hitJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.hitJudgeSelected : Judge.NORMAL;
		lifeJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.lifeJudgeSelected : Judge.NORMAL;
		skinSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.skinSelected : 0;
		raceSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.raceSelected : 0;
		deathSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.deathSelected :0;
		
		
		
		
		
		alphaText = new float[stateLoading.Length];
		isFading = new bool[stateLoading.Length];
		alphaDisplay = new float[stateLoading.Length];
		isFadingDisplay = new bool[stateLoading.Length];
		offsetFading = new float[stateLoading.Length];
		offsetPreviousFading = new float[stateLoading.Length];
		for(int i=0;i<stateLoading.Length;i++) offsetFading[i] = 0f;
		for(int i=0;i<stateLoading.Length;i++) offsetPreviousFading[i] = 0f;
		for(int i=0;i<stateLoading.Length;i++) isFading[i] = false;
		for(int i=0;i<stateLoading.Length;i++) alphaText[i] = 1f;
		for(int i=0;i<stateLoading.Length;i++) isFadingDisplay[i] = false;
		for(int i=0;i<stateLoading.Length;i++) alphaDisplay[i] = displaySelected[i] ? 1f : 0f;
		
		if(DataManager.Instance.quickMode){
			timeOption = 0.02f;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape) && animok && activeModule){
			StartCoroutine(endOptionFade());
			animok = false;
		}
		
		if(matCache.color.a > 0f){
			fadeAlphaOptionTitle -= Time.deltaTime/timeOption;
			matCache.color = new Color(matCache.color.r, matCache.color.g, matCache.color.b, fadeAlphaOptionTitle);
		}
	}
	
	
	public void GUIModule()
	{
		//Option
		if(activeModule){
			for(int i=0;i<stateLoading.Length;i++){
				if(stateLoading[i]){
					GUI.DrawTexture(new Rect(posOptionTitle.x*Screen.width, (posOptionTitle.y + offsetYOption*i)*Screen.height, posOptionTitle.width*Screen.width, posOptionTitle.height*Screen.height), gs.tex["Option" + (i+1)]);
					switch(i){
					
						/**
							SPEEDMOD
						*/
						case 0:
							if(!DataManager.Instance.BPMEntryMode){
								gs.speedmodstring = GUI.TextArea (new Rect(posItem[0].x*Screen.width, posItem[0].y*Screen.height, posItem[0].width*Screen.width, posItem[0].height*Screen.height), gs.speedmodstring.Trim(), 5);
							}else{
								gs.bpmstring = GUI.TextArea (new Rect(posItem[0].x*Screen.width, posItem[0].y*Screen.height, posItem[0].width*Screen.width, posItem[0].height*Screen.height), gs.bpmstring.Trim(), 5);
								if(!String.IsNullOrEmpty(gs.bpmstring)){
									double resultbpm;
									if(System.Double.TryParse(gs.bpmstring, out resultbpm)){
										var bpmtotest = gs.songSelected.First().Value.bpmToDisplay;
										if(bpmtotest.Contains("->")){
											gs.speedmodstring = (System.Convert.ToDouble(resultbpm/System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode]))).ToString("0.00");
										}else{
											gs.speedmodstring = (resultbpm/System.Convert.ToDouble(bpmtotest)).ToString("0.00");
										}
									}else{
										gs.speedmodstring = "?";
									}
								}else{
									gs.speedmodstring = "";
								}
							}
							if(!String.IsNullOrEmpty(gs.speedmodstring)){
								double result;
								if(System.Double.TryParse(gs.speedmodstring, out result)){
									if(result >= (double)0.25 && result <= (double)15){
										gs.speedmodSelected = (float)result;
										speedmodok = true;
										var bpmdisplaying = gs.songSelected.First().Value.bpmToDisplay;
										if(bpmdisplaying.Contains("->")){
											if(!DataManager.Instance.BPMEntryMode) gs.bpmstring = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode])*gs.speedmodSelected).ToString("0");
											bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[0])*gs.speedmodSelected).ToString("0") + "->" + (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[1])*gs.speedmodSelected).ToString("0");
										}else{
											if(!DataManager.Instance.BPMEntryMode) gs.bpmstring = (System.Convert.ToDouble(bpmdisplaying)*gs.speedmodSelected).ToString("0");
											bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying)*gs.speedmodSelected).ToString("0");
											
										}
										
										GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod : x" + gs.speedmodSelected.ToString("0.00") + " (" + bpmdisplaying + " BPM)");
									}else{
										GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
										GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod must be between x0.25 and x15");
										GUI.color = new Color(1f, 1f, 1f, 1f);
										speedmodok = false;
										if(!DataManager.Instance.BPMEntryMode) gs.bpmstring = "1";
									}
								}else{
									GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
									GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod is not a valid value");
									GUI.color = new Color(1f, 1f, 1f, 1f);
									speedmodok = false;
									if(!DataManager.Instance.BPMEntryMode) gs.bpmstring = "?";
								}
							}else{
								GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
								GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Empty value");
								GUI.color = new Color(1f, 1f, 1f, 1f);
								speedmodok = false;
								if(!DataManager.Instance.BPMEntryMode) gs.bpmstring = "";
							}
							
							if(GUI.Button(new Rect((posItem[0].x + offsetXDisplayBPMSwitch)*Screen.width, posItem[0].y*Screen.height, (posItem[0].width + offsetWidthDisplayBPMSV)*Screen.width, posItem[0].height*Screen.height), DataManager.Instance.BPMEntryMode ? "By multip." : "by BPM", "labelGoLittle")){
								DataManager.Instance.BPMEntryMode = !DataManager.Instance.BPMEntryMode;
							}
							if(DataManager.Instance.BPMEntryMode && gs.songSelected.First().Value.bpmToDisplay.Contains("->")){
								if(GUI.Button(new Rect((posItem[0].x + offsetXDisplayBPMValue)*Screen.width, posItem[0].y*Screen.height, (posItem[0].width + offsetWidthDisplayBPMSV)*Screen.width, posItem[0].height*Screen.height), 	  DataManager.Instance.BPMChoiceMode == 0 ? "For higher" : "For lower", "labelGoLittle")){
									DataManager.Instance.BPMChoiceMode++;
									if(DataManager.Instance.BPMChoiceMode == 2) DataManager.Instance.BPMChoiceMode = 0;
								}
							}
							
						break;
						
						/**
							DIFFICULTY
						*/
						case 1:
							GUI.color = new Color(1f, 1f, 1f, alphaText[1]);
							GUI.DrawTexture(new Rect(posItemLabel[1].x*Screen.width, (posItemLabel[1].y - offsetFading[1])*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), gs.tex[((Difficulty)gs.songSelected.ElementAt(difficultySelected).Key).ToString()]);
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[1]);
							if(isFading[1]) GUI.Label(new Rect(posItemLabel[1].x*Screen.width, (posItemLabel[1].y + offsetPreviousFading[1])*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), gs.tex[((Difficulty)previousSelected).ToString()]);
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(gs.songSelected.Count > 1)
							{
								if(GUI.Button(new Rect((posItem[1].x - ecartForBack)*Screen.width, posItem[1].y*Screen.height, posItem[1].width*Screen.width, posItem[1].height*Screen.height), "", "LBackward")){
									previousSelected = difficultySelected;
									if(difficultySelected == 0){
										difficultySelected = gs.songSelected.Count - 1;
									}else{
										difficultySelected--;
									}
									StartCoroutine(OptionAnim(1, true));
								}
								if(GUI.Button(new Rect((posItem[1].x + ecartForBack)*Screen.width, posItem[1].y*Screen.height, posItem[1].width*Screen.width, posItem[1].height*Screen.height), "", "LForward")){
									previousSelected = difficultySelected;
									if(difficultySelected == gs.songSelected.Count - 1){
										difficultySelected = 0;
									}else{
										difficultySelected++;
									}
									StartCoroutine(OptionAnim(1, false));
								}
							}
							
						break;
						/**
							SKIN
						*/
						case 2:
							GUI.color = new Color(1f, 1f, 1f, alphaText[2]);
							GUI.Label(new Rect(posItemLabel[2].x*Screen.width, (posItemLabel[2].y - offsetFading[2])*Screen.height, posItemLabel[2].width*Screen.width, posItemLabel[2].height*Screen.height), DataManager.Instance.aSkin[skinSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[2]);
							if(isFading[2]) GUI.Label(new Rect(posItemLabel[2].x*Screen.width, (posItemLabel[2].y + offsetPreviousFading[2])*Screen.height, posItemLabel[2].width*Screen.width, posItemLabel[2].height*Screen.height), DataManager.Instance.aSkin[previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(GUI.Button(new Rect((posItem[2].x - ecartForBack)*Screen.width, posItem[2].y*Screen.height, posItem[2].width*Screen.width, posItem[2].height*Screen.height), "", "LBackward")){
								previousSelected = skinSelected;
								if(skinSelected == 0){
									skinSelected = DataManager.Instance.aSkin.Length - 1;
								}else{
									skinSelected--;
								}
								StartCoroutine(OptionAnim(2, true));
							}
							if(GUI.Button(new Rect((posItem[2].x + ecartForBack)*Screen.width, posItem[2].y*Screen.height, posItem[2].width*Screen.width, posItem[2].height*Screen.height), "", "LForward")){
								previousSelected = skinSelected;
								if(skinSelected == DataManager.Instance.aSkin.Length - 1){
									skinSelected = 0;
								}else{
									skinSelected++;
								}
								StartCoroutine(OptionAnim(2, false));
							}
						break;
						/**
							HIT JUDGE
						*/
						case 3:
							GUI.color = new Color(1f, 1f, 1f, alphaText[3]);
							GUI.Label(new Rect(posItemLabel[3].x*Screen.width, (posItemLabel[3].y - offsetFading[3])*Screen.height, posItemLabel[3].width*Screen.width, posItemLabel[3].height*Screen.height), DataManager.Instance.dicHitJudge[hitJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[3]);
							if(isFading[3]) GUI.Label(new Rect(posItemLabel[3].x*Screen.width, (posItemLabel[3].y + offsetPreviousFading[3])*Screen.height, posItemLabel[3].width*Screen.width, posItemLabel[3].height*Screen.height), DataManager.Instance.dicHitJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(hitJudgeSelected > Judge.NORMAL){
								if(GUI.Button(new Rect((posItem[3].x - ecartForBack)*Screen.width, posItem[3].y*Screen.height, posItem[3].width*Screen.width, posItem[3].height*Screen.height), "", "LBackward")){
									previousSelected = (int)hitJudgeSelected;
									hitJudgeSelected--;
									StartCoroutine(OptionAnim(3, true));
								}
							}
							if(hitJudgeSelected < Judge.EXPERT){
								if(GUI.Button(new Rect((posItem[3].x + ecartForBack)*Screen.width, posItem[3].y*Screen.height, posItem[3].width*Screen.width, posItem[3].height*Screen.height), "", "LForward")){
									previousSelected = (int)hitJudgeSelected;
									hitJudgeSelected++;
									StartCoroutine(OptionAnim(3, false));
								}
							}
						break;
						/**
							SCORE JUDGE
						*/
						case 4:
							GUI.color = new Color(1f, 1f, 1f, alphaText[4]);
							GUI.Label(new Rect(posItemLabel[4].x*Screen.width, (posItemLabel[4].y - offsetFading[4])*Screen.height, posItemLabel[4].width*Screen.width, posItemLabel[4].height*Screen.height), DataManager.Instance.dicScoreJudge[scoreJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[4]);
							if(isFading[4]) GUI.Label(new Rect(posItemLabel[4].x*Screen.width, (posItemLabel[4].y + offsetPreviousFading[4])*Screen.height, posItemLabel[4].width*Screen.width, posItemLabel[4].height*Screen.height), DataManager.Instance.dicScoreJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(scoreJudgeSelected > Judge.NORMAL){
								if(GUI.Button(new Rect((posItem[4].x - ecartForBack)*Screen.width, posItem[4].y*Screen.height, posItem[4].width*Screen.width, posItem[4].height*Screen.height), "", "LBackward")){
									previousSelected = (int)scoreJudgeSelected;
									scoreJudgeSelected--;
									StartCoroutine(OptionAnim(4, true));
								}
							}
							if(scoreJudgeSelected < Judge.EXPERT){
								if(GUI.Button(new Rect((posItem[4].x + ecartForBack)*Screen.width, posItem[4].y*Screen.height, posItem[4].width*Screen.width, posItem[4].height*Screen.height), "", "LForward")){
									previousSelected = (int)scoreJudgeSelected;
									scoreJudgeSelected++;
									StartCoroutine(OptionAnim(4, false));
								}
							}
						break;
						/**
							LIFE JUDGE
						*/
						case 5:
							GUI.color = new Color(1f, 1f, 1f, alphaText[5]);
							GUI.Label(new Rect(posItemLabel[5].x*Screen.width, (posItemLabel[5].y - offsetFading[5])*Screen.height, posItemLabel[5].width*Screen.width, posItemLabel[5].height*Screen.height), DataManager.Instance.dicLifeJudge[lifeJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[5]);
							if(isFading[5]) GUI.Label(new Rect(posItemLabel[5].x*Screen.width, (posItemLabel[5].y + offsetPreviousFading[5])*Screen.height, posItemLabel[5].width*Screen.width, posItemLabel[5].height*Screen.height), DataManager.Instance.dicLifeJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(lifeJudgeSelected > Judge.NORMAL){
								if(GUI.Button(new Rect((posItem[5].x - ecartForBack)*Screen.width, posItem[5].y*Screen.height, posItem[5].width*Screen.width, posItem[5].height*Screen.height), "", "LBackward")){
									previousSelected = (int)lifeJudgeSelected;
									lifeJudgeSelected--;
									StartCoroutine(OptionAnim(5, true));
								}
							}
							if(lifeJudgeSelected < Judge.EXPERT){
								if(GUI.Button(new Rect((posItem[5].x + ecartForBack)*Screen.width, posItem[5].y*Screen.height, posItem[5].width*Screen.width, posItem[5].height*Screen.height), "", "LForward")){
									previousSelected = (int)lifeJudgeSelected;
									lifeJudgeSelected++;
									StartCoroutine(OptionAnim(5, false));
								}
							}
						break;
						/**
							DISPLAY
						*/
						case 6:
							for(int j=4; j<DataManager.Instance.aDisplay.Length; j++){
								if(displaySelected[j]){
									GUI.color = new Color(1f, 1f, 1f, alphaDisplay[j]);
									GUI.DrawTexture(new Rect((posItem[6].x - borderXDisplay + offsetXDisplay*j + selectedImage.x)*Screen.width, (posItem[6].y + selectedImage.y)*Screen.height, selectedImage.width*Screen.width, selectedImage.height*Screen.height), gs.tex["ChoiceBar"]);
									GUI.color = new Color(1f, 1f, 1f, 1f);
								}
							
								if(GUI.Button(new Rect((posItem[6].x - borderXDisplay + offsetXDisplay*j)*Screen.width, posItem[6].y*Screen.height, posItem[6].width*Screen.width, posItem[6].height*Screen.height), DataManager.Instance.aDisplay[j], "labelNormal") && !isFadingDisplay[j]){
									displaySelected[j] = !displaySelected[j];
									StartCoroutine(OptionAnimDisplay(j, !displaySelected[j]));
								}
							}
						break;
					
					}
				
				}
			}
		
		}	
		
		
	}
	
	
	IEnumerator startOptionFade(){
		
		activeModule = true;
		while(gs.getZoneInfo().isEntering())
		{
			yield return new WaitForSeconds(Time.deltaTime);	
		}
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
		cacheOption.active = false;
		cacheOption.transform.position = posInit;
		animok = true;
		gs.getZonePack().onPopin();
		gs.getZoneSong().onPopin();
		gs.getZoneInfo().onExitOption();
		activeModule = false;
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
			alphaDisplay[i] = 1f;
			while(alphaDisplay[i] > 0){
				alphaDisplay[i] -= Time.deltaTime/timeFadeOutDisplay;
				yield return new WaitForFixedUpdate();
			}
			alphaDisplay[i] = 0f;
		}else{
			alphaDisplay[i] = 0f;
			while(alphaDisplay[i] < 1f){
				alphaDisplay[i] += Time.deltaTime/timeFadeOutDisplay;
				yield return new WaitForFixedUpdate();
			}
			alphaDisplay[i] = 1f;
		}
		
		isFadingDisplay[i] = false;
		
	}
	
	public void fillDataManager()
	{
		DataManager.Instance.rateSelected = 0f;
		DataManager.Instance.difficultySelected = gs.songSelected.ElementAt(difficultySelected).Key;
		DataManager.Instance.skinSelected = skinSelected;
		DataManager.Instance.scoreJudgeSelected = scoreJudgeSelected;
		DataManager.Instance.hitJudgeSelected = hitJudgeSelected;
		DataManager.Instance.lifeJudgeSelected = lifeJudgeSelected;
		DataManager.Instance.raceSelected = raceSelected;
		DataManager.Instance.displaySelected = displaySelected;
		DataManager.Instance.deathSelected = deathSelected;
	}
	
	public void onPopin()
	{
		difficultySelected = (int)gs.getZoneInfo().getActualySelected();
		StartCoroutine(startOptionFade());
		animok = false;
	}
	
	public void onPopout()
	{
		StartCoroutine(endOptionFade());
		animok = false;
	}
	
	public void instantClose()
	{
		if(activeModule)
		{
			for(int i=0;i<optionSeparator.Length; i++){
				optionSeparator[i].enabled = false;
			}	
			activeModule = false;
		}
	}
	
	public bool isRatedSong()
	{
		return false;
	}
	
	public bool isReadyToClose()
	{
		return speedmodok && rateok;
	}
	
	public bool isOkToAnim()
	{
		return animok;	
	}
}
