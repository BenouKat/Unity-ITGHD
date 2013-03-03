using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class OptionZone : MonoBehaviour {
	
	public GameObject cacheOption;
	
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
	
	//Anim options
	public float timeFadeOut;
	public float timeFadeOutDisplay;
	private float[] alphaText;
	private float[] alphaDisplay;
	private float[] offsetFading;
	private float[] offsetPreviousFading;
	private bool[] isFading;
	private bool[] isFadingDisplay;
	private int previousSelected;
	public float offsetBaseFading;
	
	// Use this for initialization
	void Start () {
	
		speedmodok = true;
		rateok = true;
		
		matCache = cacheOption.renderer.material;
		
		fadeAlphaOptionTitle = 1f;
		stateLoading = new bool[9];
		displaySelected = new bool[DataManager.Instance.aDisplay.Length];
		for(int i=0;i<stateLoading.Length-1;i++) stateLoading[i] = false;
		for(int j=0;j<DataManager.Instance.aDisplay.Length;j++) displaySelected[j] = DataManager.Instance.songSelected != null ? DataManager.Instance.displaySelected[j] : false;
		
		if(!String.IsNullOrEmpty(ProfileManager.Instance.currentProfile.lastSpeedmodUsed)){
			speedmodstring = ProfileManager.Instance.currentProfile.lastSpeedmodUsed;
			speedmodSelected = (float)System.Convert.ToDouble(ProfileManager.Instance.currentProfile.lastSpeedmodUsed);
		}else{
			speedmodSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.speedmodSelected : 2f;
			speedmodstring = speedmodSelected.ToString("0.00");
		}
		
		if(DataManager.Instance.songSelected != null){
			var bpmtotest = DataManager.Instance.songSelected.bpmToDisplay;
			if(bpmtotest.Contains("->")){
				bpmstring = (System.Convert.ToDouble(System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode])*speedmodSelected)).ToString("0");
			}else{
				bpmstring = (System.Convert.ToDouble(bpmtotest)*speedmodSelected).ToString("0");
			}
		}else{
			bpmstring = String.IsNullOrEmpty(ProfileManager.Instance.currentProfile.lastBPM) ? "300" : ProfileManager.Instance.currentProfile.lastBPM;
		}
		DataManager.Instance.BPMEntryMode = ProfileManager.Instance.currentProfile.inBPMMode;
		
		rateSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.rateSelected : 0f;
		scoreJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.scoreJudgeSelected : Judge.NORMAL;
		hitJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.hitJudgeSelected : Judge.NORMAL;
		lifeJudgeSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.lifeJudgeSelected : Judge.NORMAL;
		skinSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.skinSelected : 0;
		raceSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.raceSelected : 0;
		deathSelected = DataManager.Instance.songSelected != null ? DataManager.Instance.deathSelected :0;
		
		
		
		ratestring = rateSelected.ToString("00");
		
		
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
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape) && !movinSong && !SongMode && !fadedOut){
			if(OptionMode && animok){
				StartCoroutine(endOptionFade());
				animok = false;
			}else if(!OptionMode){
				fadedOut = true;
				GetComponent<FadeManager>().FadeIn("mainmenu");	
			}
		}
	}
	
	
	void OnGUI()
	{
		//Option
		if((OptionMode || movinNormal) && !movinOption){
			for(int i=0;i<stateLoading.Length;i++){
				if(stateLoading[i]){
					GUI.DrawTexture(new Rect(posOptionTitle.x*Screen.width, (posOptionTitle.y + offsetYOption*i)*Screen.height, posOptionTitle.width*Screen.width, posOptionTitle.height*Screen.height), tex["Option" + (i+1)]);
					switch(i){
						case 0:
							//speedmod offsetXDisplayBPMSwitch
							if(!DataManager.Instance.BPMEntryMode){
								speedmodstring = GUI.TextArea (new Rect(posItem[0].x*Screen.width, posItem[0].y*Screen.height, posItem[0].width*Screen.width, posItem[0].height*Screen.height), speedmodstring.Trim(), 5);
							}else{
								bpmstring = GUI.TextArea (new Rect(posItem[0].x*Screen.width, posItem[0].y*Screen.height, posItem[0].width*Screen.width, posItem[0].height*Screen.height), bpmstring.Trim(), 5);
								if(!String.IsNullOrEmpty(bpmstring)){
									double resultbpm;
									if(System.Double.TryParse(bpmstring, out resultbpm)){
										var bpmtotest = songSelected.First().Value.bpmToDisplay;
										if(bpmtotest.Contains("->")){
											speedmodstring = (System.Convert.ToDouble(resultbpm/System.Convert.ToDouble(bpmtotest.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode]))).ToString("0.00");
										}else{
											speedmodstring = (resultbpm/System.Convert.ToDouble(bpmtotest)).ToString("0.00");
										}
									}else{
										speedmodstring = "?";
									}
								}else{
									speedmodstring = "";
								}
							}
							if(!String.IsNullOrEmpty(speedmodstring)){
								double result;
								if(System.Double.TryParse(speedmodstring, out result)){
									if(result >= (double)0.25 && result <= (double)15){
										speedmodSelected = (float)result;
										speedmodok = true;
										var bpmdisplaying = songSelected.First().Value.bpmToDisplay;
										if(bpmdisplaying.Contains("->")){
											if(!DataManager.Instance.BPMEntryMode) bpmstring = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[DataManager.Instance.BPMChoiceMode])*speedmodSelected).ToString("0");
											bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[0])*speedmodSelected*(1f + (rateSelected/100f))).ToString("0") + "->" + (System.Convert.ToDouble(bpmdisplaying.Replace(">", "").Split('-')[1])*speedmodSelected*(1f + (rateSelected/100f))).ToString("0");
										}else{
											if(!DataManager.Instance.BPMEntryMode) bpmstring = (System.Convert.ToDouble(bpmdisplaying)*speedmodSelected).ToString("0");
											bpmdisplaying = (System.Convert.ToDouble(bpmdisplaying)*speedmodSelected*(1f + (rateSelected/100f))).ToString("0");
											
										}
										
										GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod : x" + speedmodSelected.ToString("0.00") + " (" + bpmdisplaying + " BPM)");
									}else{
										GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
										GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod must be between x0.25 and x15");
										GUI.color = new Color(1f, 1f, 1f, 1f);
										speedmodok = false;
										if(!DataManager.Instance.BPMEntryMode) bpmstring = "1";
									}
								}else{
									GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
									GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Speedmod is not a valid value");
									GUI.color = new Color(1f, 1f, 1f, 1f);
									speedmodok = false;
									if(!DataManager.Instance.BPMEntryMode) bpmstring = "?";
								}
							}else{
								GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
								GUI.Label(new Rect((posItemLabel[0].x + offsetSpeedRateX)*Screen.width, posItemLabel[0].y*Screen.height, posItemLabel[0].width*Screen.width, posItemLabel[0].height*Screen.height), "Empty value");
								GUI.color = new Color(1f, 1f, 1f, 1f);
								speedmodok = false;
								if(!DataManager.Instance.BPMEntryMode) bpmstring = "";
							}
							
							if(GUI.Button(new Rect((posItem[0].x + offsetXDisplayBPMSwitch)*Screen.width, posItem[0].y*Screen.height, (posItem[0].width + offsetWidthDisplayBPMSV)*Screen.width, posItem[0].height*Screen.height), DataManager.Instance.BPMEntryMode ? "By multip." : "by BPM", "labelGoLittle")){
								DataManager.Instance.BPMEntryMode = !DataManager.Instance.BPMEntryMode;
							}
							if(DataManager.Instance.BPMEntryMode && songSelected.First().Value.bpmToDisplay.Contains("->")){
								if(GUI.Button(new Rect((posItem[0].x + offsetXDisplayBPMValue)*Screen.width, posItem[0].y*Screen.height, (posItem[0].width + offsetWidthDisplayBPMSV)*Screen.width, posItem[0].height*Screen.height), 	  DataManager.Instance.BPMChoiceMode == 0 ? "For higher" : "For lower", "labelGoLittle")){
									DataManager.Instance.BPMChoiceMode++;
									if(DataManager.Instance.BPMChoiceMode == 2) DataManager.Instance.BPMChoiceMode = 0;
								}
							}
							
						break;
						case 1:
							//Rate
							ratestring = GUI.TextArea (new Rect(posItem[1].x*Screen.width, posItem[1].y*Screen.height, posItem[1].width*Screen.width, posItem[1].height*Screen.height), ratestring.Trim(), 4);
							if(!String.IsNullOrEmpty(ratestring)){
								int rateresult = 0;
								if(System.Int32.TryParse(ratestring, out rateresult)){
									if(rateresult >= -90 && rateresult <= 100){
										rateSelected = rateresult;
										rateok = true;
										GUI.Label(new Rect((posItemLabel[1].x + offsetSpeedRateX)*Screen.width, posItemLabel[1].y*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), "Rate : " + rateSelected.ToString("00") + "%");
									}else{
										GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
										GUI.Label(new Rect((posItemLabel[1].x + offsetSpeedRateX)*Screen.width, posItemLabel[1].y*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), "Rate must be between -90% and +100%");
										GUI.color = new Color(1f, 1f, 1f, 1f);
										rateok = false;
									}
								}else{
									GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
									GUI.Label(new Rect((posItemLabel[1].x + offsetSpeedRateX)*Screen.width, posItemLabel[1].y*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), "Rate is not a valid value");
									GUI.color = new Color(1f, 1f, 1f, 1f);
									rateok = false;
								}
							}else{
								GUI.color = new Color(1f, 0.2f, 0.2f, 1f);
								GUI.Label(new Rect((posItemLabel[1].x + offsetSpeedRateX)*Screen.width, posItemLabel[1].y*Screen.height, posItemLabel[1].width*Screen.width, posItemLabel[1].height*Screen.height), "Empty Value");
								GUI.color = new Color(1f, 1f, 1f, 1f);
								rateok = false;
							}
							
						break;
						case 2:
							//skin
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
						case 3:
							//hit
							GUI.color = new Color(1f, 1f, 1f, alphaText[3]);
							GUI.Label(new Rect(posItemLabel[3].x*Screen.width, (posItemLabel[3].y - offsetFading[3])*Screen.height, posItemLabel[3].width*Screen.width, posItemLabel[3].height*Screen.height), DataManager.Instance.dicHitJudge[hitJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[3]);
							if(isFading[3]) GUI.Label(new Rect(posItemLabel[3].x*Screen.width, (posItemLabel[3].y + offsetPreviousFading[3])*Screen.height, posItemLabel[3].width*Screen.width, posItemLabel[3].height*Screen.height), DataManager.Instance.dicHitJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(hitJudgeSelected > Judge.BEGINNER){
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
						case 4:
							//score
							GUI.color = new Color(1f, 1f, 1f, alphaText[4]);
							GUI.Label(new Rect(posItemLabel[4].x*Screen.width, (posItemLabel[4].y - offsetFading[4])*Screen.height, posItemLabel[4].width*Screen.width, posItemLabel[4].height*Screen.height), DataManager.Instance.dicScoreJudge[scoreJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[4]);
							if(isFading[4]) GUI.Label(new Rect(posItemLabel[4].x*Screen.width, (posItemLabel[4].y + offsetPreviousFading[4])*Screen.height, posItemLabel[4].width*Screen.width, posItemLabel[4].height*Screen.height), DataManager.Instance.dicScoreJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(scoreJudgeSelected > Judge.BEGINNER){
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
						case 5:
							//life judge
							GUI.color = new Color(1f, 1f, 1f, alphaText[5]);
							GUI.Label(new Rect(posItemLabel[5].x*Screen.width, (posItemLabel[5].y - offsetFading[5])*Screen.height, posItemLabel[5].width*Screen.width, posItemLabel[5].height*Screen.height), DataManager.Instance.dicLifeJudge[lifeJudgeSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[5]);
							if(isFading[5]) GUI.Label(new Rect(posItemLabel[5].x*Screen.width, (posItemLabel[5].y + offsetPreviousFading[5])*Screen.height, posItemLabel[5].width*Screen.width, posItemLabel[5].height*Screen.height), DataManager.Instance.dicLifeJudge[(Judge)previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(lifeJudgeSelected > Judge.BEGINNER){
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
						case 6:
							//display
							for(int j=0; j<DataManager.Instance.aDisplay.Length; j++){
								if(displaySelected[j]){
									GUI.color = new Color(1f, 1f, 1f, alphaDisplay[j]);
									GUI.DrawTexture(new Rect((posItem[6].x - borderXDisplay + offsetXDisplay*j + selectedImage.x)*Screen.width, (posItem[6].y + selectedImage.y)*Screen.height, selectedImage.width*Screen.width, selectedImage.height*Screen.height), tex["bouton"]);
									GUI.color = new Color(1f, 1f, 1f, 1f);
								}
							
								if(GUI.Button(new Rect((posItem[6].x - borderXDisplay + offsetXDisplay*j)*Screen.width, posItem[6].y*Screen.height, posItem[6].width*Screen.width, posItem[6].height*Screen.height), DataManager.Instance.aDisplay[j], "labelNormal") && !isFadingDisplay[j]){
									displaySelected[j] = !displaySelected[j];
									StartCoroutine(OptionAnimDisplay(j, !displaySelected[j]));
								}
							}
						break;
						case 7:
							//race
							GUI.color = new Color(1f, 1f, 1f, alphaText[7]);
							GUI.Label(new Rect(posItemLabel[7].x*Screen.width, (posItemLabel[7].y - offsetFading[7])*Screen.height, posItemLabel[7].width*Screen.width, posItemLabel[7].height*Screen.height), DataManager.Instance.aRace[raceSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[7]);
							if(isFading[7]) GUI.Label(new Rect(posItemLabel[7].x*Screen.width, (posItemLabel[7].y + offsetPreviousFading[7])*Screen.height, posItemLabel[7].width*Screen.width, posItemLabel[7].height*Screen.height), DataManager.Instance.aRace[previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(GUI.Button(new Rect((posItem[7].x - ecartForBack)*Screen.width, posItem[7].y*Screen.height, posItem[7].width*Screen.width, posItem[7].height*Screen.height), "", "LBackward")){
								previousSelected = raceSelected;
								if(raceSelected == 0){
									raceSelected = DataManager.Instance.aRace.Length - 1;
								}else{
									raceSelected--;
								}
								StartCoroutine(OptionAnim(7, true));
							}
							if(GUI.Button(new Rect((posItem[7].x + ecartForBack)*Screen.width, posItem[7].y*Screen.height, posItem[7].width*Screen.width, posItem[7].height*Screen.height), "", "LForward")){
								previousSelected = raceSelected;
								if(raceSelected == DataManager.Instance.aRace.Length - 1){
									raceSelected = 0;
								}else{
									raceSelected++;
								}
								StartCoroutine(OptionAnim(7, false));
							}
						break;
						case 8:
							//death
							GUI.color = new Color(1f, 1f, 1f, alphaText[8]);
							GUI.Label(new Rect(posItemLabel[8].x*Screen.width, (posItemLabel[8].y - offsetFading[8])*Screen.height, posItemLabel[8].width*Screen.width, posItemLabel[8].height*Screen.height), DataManager.Instance.aDeath[deathSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1 - alphaText[8]);
							if(isFading[8]) GUI.Label(new Rect(posItemLabel[8].x*Screen.width, (posItemLabel[8].y + offsetPreviousFading[8])*Screen.height, posItemLabel[8].width*Screen.width, posItemLabel[8].height*Screen.height), DataManager.Instance.aDeath[previousSelected], "bpmdisplay");
							GUI.color = new Color(1f, 1f, 1f, 1f);
							if(GUI.Button(new Rect((posItem[8].x - ecartForBack)*Screen.width, posItem[8].y*Screen.height, posItem[8].width*Screen.width, posItem[8].height*Screen.height), "", "LBackward")){
								previousSelected = deathSelected;
								if(deathSelected == 0){
									deathSelected = DataManager.Instance.aDeath.Length - 1; 
								}else{
									deathSelected--;
								}
								StartCoroutine(OptionAnim(8, true));
							}
							if(GUI.Button(new Rect((posItem[8].x + ecartForBack)*Screen.width, posItem[8].y*Screen.height, posItem[8].width*Screen.width, posItem[8].height*Screen.height), "", "LForward")){
								previousSelected = deathSelected;
								if(deathSelected == DataManager.Instance.aDeath.Length - 1){
									deathSelected = 0;
								}else{
									deathSelected++;
								}
								StartCoroutine(OptionAnim(8, false));
							}
						break;
					
					}
				
				}
			}
		
		}	
		
		
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
