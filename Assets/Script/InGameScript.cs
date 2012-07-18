using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InGameScript : MonoBehaviour {
	
	
	//Game object song scene
	public GameObject arrow;
	public GameObject freeze;
	public Camera MainCamera;
	public GameObject arrowLeft;
	public GameObject arrowRight;
	public GameObject arrowDown;
	public GameObject arrowUp;
	public GameObject particlePrec;
	
	
	private Song thesong;
	
	private double timebpm; //Temps joué sur la totalité, non live, non remise à 0
	private float timechart; //Temps joué sur le bpm actuel en live avec remise à 0
	private float timestop; //Temps utilisé pour le freeze
	private float totaltimestop; //Temps total à être stoppé
	private double timetotalchart; //Temps joué sur la totalité en live (timebpm + timechart)
	private float changeBPM; //Position en y depuis un changement de bpm (nouvelle reference)
	
	private int nextSwitchBPM; //Prochain changement de bpm
	private int nextSwitchStop;
	
	private double actualBPM; //Bpm actuel
	
	private double actualstop;
	
	private bool dontstopmenow;
	
	public float speedmod = 4f; //speedmod (trop lent ?)
	
	//Temps pour le lachement de freeze
	public float unfrozed = 0.350f;
	
	//Temps pour l'affichage des scores
	public float limitDisplayScore = 1f;
	public float timeDisplayScore = 0f;
	public Precision scoreToDisplay;
	public Rect posScore = new Rect(0.38f,0.3f,0.25f,0.25f);
	
	//fps count
	private long _count;
	private long fps;
	private float _timer;
	
	//stepchart divisé par input
	private List<Arrow> arrowLeftList;
	private List<Arrow> arrowRightList;
	private List<Arrow> arrowDownList;
	private List<Arrow> arrowUpList;
	
	//Particle System
	private GameObject precLeft;
	private GameObject precRight;
	private GameObject precUp;
	private GameObject precDown;
	
	//Dico des arrow prises en freeze
	private Dictionary<Arrow, float> arrowFrozen;
	
	
	//bdd de textures
	private Dictionary<string, Texture2D> TextureBase;
	
	
	
	// Use this for initialization
	
	public enum Precision{
		FANTASTIC,
		EXCELLENT,
		GREAT,
		DECENT,
		WAYOFF,
		MISS,
		FREEZE,
		NONE
	}
	
	
	void Start () {
		thesong = OpenChart.Instance.readChart("Still Blastin'")[0];
		createTheChart(thesong);
		Application.targetFrameRate = -1;
		nextSwitchBPM = 1;
		nextSwitchStop = 0;
		actualBPM = thesong.bpms.First().Value;
		actualstop = (double)0;
		changeBPM = 0;
		
		_count = 0L;
		
		timebpm = (double)0;
		timechart = 0f;
		timetotalchart = (double)0;
		
		
		arrowFrozen = new Dictionary<Arrow, float>();
		dontstopmenow = true;
		
		
		//Prepare the scene
		
		precLeft = (GameObject) arrowLeft.transform.GetChild(0).gameObject;
		precDown = (GameObject) arrowDown.transform.GetChild(0).gameObject;
		precRight = (GameObject) arrowRight.transform.GetChild(0).gameObject;
		precUp = (GameObject) arrowUp.transform.GetChild(0).gameObject;
		
		//Textures
		TextureBase = new Dictionary<string, Texture2D>();
		TextureBase.Add("FANTASTIC", (Texture2D) Resources.Load("Fantastic"));
		TextureBase.Add("EXCELLENT", (Texture2D) Resources.Load("Excellent"));
		TextureBase.Add("GREAT", (Texture2D) Resources.Load("Great"));
		TextureBase.Add("DECENT", (Texture2D) Resources.Load("Decent"));
		TextureBase.Add("WAYOFF", (Texture2D) Resources.Load("Wayoff"));
		TextureBase.Add("MISS", (Texture2D) Resources.Load("Miss"));
		
		scoreToDisplay = Precision.NONE;
	}
	
	
	
	
	//only for FPS
	void OnGUI(){
		GUI.Label(new Rect(0.9f*Screen.width, 0.05f*Screen.height, 200f, 200f), fps.ToString());	
		GUI.Label(new Rect(0.9f*Screen.width, 0.1f*Screen.height, 200f, 200f), ((float)Screen.width/(float)Screen.height).ToString());
		
		GUI.DrawTexture(new Rect(posScore.x*Screen.width, posScore.y*Screen.height, posScore.width*Screen.width, posScore.height*Screen.height), TextureBase["FANTASTIC"]);
	}
	
	
	
	
	
	
	// Update is called once per frame
	void Update () {
		
		
		//FPS
		_count++;
		_timer += Time.deltaTime;
		if(_timer >= 1f){
			fps = _count;
			_count = 0L;
			_timer = 0;
		}
		
		
		//timetotal for this frame
		timetotalchart = timebpm + timechart + totaltimestop;
		
		//Move Arrows
		MoveArrows();
			
		//Verify inputs
		VerifyValidFrozenArrow();
		VerifyKeysInput();
		VerifyKeysOutput();
		
		
		
		//BPM change verify
		if(nextSwitchBPM < thesong.bpms.Count && (thesong.bpms.ElementAt(nextSwitchBPM).Key <= timetotalchart)){
			
			
			var bps = thesong.getBPS(actualBPM);
			changeBPM += -((float)(bps*(timechart - (float)(timetotalchart - thesong.bpms.ElementAt(nextSwitchBPM).Key))))*speedmod;
			timebpm += (double)timechart - (double)(timetotalchart - thesong.bpms.ElementAt(nextSwitchBPM).Key);
			timechart = 0f;
			actualBPM = thesong.bpms.ElementAt(nextSwitchBPM).Value;
			
			nextSwitchBPM++;
		}
		
		
		//Stop verif
		if(nextSwitchStop < thesong.stops.Count && (thesong.stops.ElementAt(nextSwitchStop).Key <= timetotalchart)){
			timetotalchart = timebpm + timechart + totaltimestop;
			timechart -= (float)timetotalchart - (float)thesong.stops.ElementAt(nextSwitchStop).Key;
			timetotalchart = timebpm + timechart + totaltimestop;
			actualstop = thesong.stops.ElementAt(nextSwitchStop).Value;
			dontstopmenow =  true;
			nextSwitchStop++;
			//Debug.Log("entry stop : " + timetotalchart);
		}
		
		//timechart ++
		if(actualstop != 0){
			if(timestop >= actualstop){
				timechart += timestop - (float)actualstop;
				totaltimestop += (float)actualstop;
				timetotalchart = timebpm + timechart + totaltimestop;
				//Debug.Log("exit stop : " + timetotalchart);
				actualstop = (double)0;
				timestop = 0f;
				//may be ?
				//timechart += Time.deltaTime;
			}else if(!dontstopmenow){
				
				timestop += Time.deltaTime;
				//totaltimestop += Time.deltaTime;
			}else{
				dontstopmenow = false;
			}
			//Debug.Log("stopped : " + timestop);
		}else{
			timechart += Time.deltaTime;
		}
		
		
		
	}
	
	
	
	//For moving arrows or do some displaying things
	void MoveArrows(){
		
		var bps = thesong.getBPS(actualBPM);
		MainCamera.transform.position = new Vector3(3f, -((float)(bps*timechart))*speedmod +  changeBPM - 6, -10f);
		arrowLeft.transform.position = new Vector3(0f, -((float)(bps*timechart))*speedmod  + changeBPM, 2f);
		arrowRight.transform.position = new Vector3(6f, -((float)(bps*timechart))*speedmod + changeBPM, 2f);
		arrowDown.transform.position = new Vector3(2f, -((float)(bps*timechart))*speedmod + changeBPM, 2f);
		arrowUp.transform.position = new Vector3(4f, -((float)(bps*timechart))*speedmod + changeBPM, 2f);
		
		foreach(var el in arrowFrozen.Keys){
			var pos = el.goArrow.transform.position;
			el.goArrow.transform.position = new Vector3(pos.x, -((float)(bps*timechart))*speedmod  + changeBPM, pos.z);
			pos = el.goArrow.transform.position;
			
			el.goFreeze.transform.position = new Vector3(el.goFreeze.transform.position.x, (pos.y + ((el.posEnding.y - pos.y)/2f)) , el.goFreeze.transform.position.z);
			el.goFreeze.transform.localScale = new Vector3(1f, -((el.posEnding.y - pos.y)/2f), 0.1f);
			el.goFreeze.transform.GetChild(0).transform.localScale = new Vector3((el.posEnding.y - pos.y)/(el.posEnding.y - el.posBegining.y), 1f, 0.1f);
			el.changeColorFreeze(arrowFrozen[el], unfrozed);
		}
	}
	
	
	
	//Valid or deny the frozen arrow
	void VerifyValidFrozenArrow(){
		if(arrowFrozen.Count > 0){
			var KeyToRemove = new List<Arrow>();
			for(int i=0; i<arrowFrozen.Count;i++){
				var el = arrowFrozen.ElementAt(i);
				arrowFrozen[el.Key] += Time.deltaTime;
				
				if(el.Key.timeEndingFreeze <= timetotalchart){
					switch((int)el.Key.goArrow.transform.position.x){
					case 0:
						StartParticleFreezeLeft(false);
						break;
					case 2:
						StartParticleFreezeDown(false);
						break;
					case 4:
						StartParticleFreezeUp(false);
						break;
					case 6:
						StartParticleFreezeRight(false);
						break;
					}
					DestroyImmediate(el.Key.goArrow);
					DestroyImmediate(el.Key.goFreeze);
					
					KeyToRemove.Add(el.Key);
				}
				
				
				if(el.Value >= unfrozed && !KeyToRemove.Contains(el.Key)){
					el.Key.goArrow.GetComponent<ArrowScript>().missed = true;
					switch((int)el.Key.goArrow.transform.position.x){
					case 0:
						StartParticleFreezeLeft(false);
						break;
					case 2:
						StartParticleFreezeDown(false);
						break;
					case 4:
						StartParticleFreezeUp(false);
						break;
					case 6:
						StartParticleFreezeRight(false);
						break;
					}
					KeyToRemove.Add(el.Key);
				}
				
				
			}
			
			
			
			foreach(var k in KeyToRemove){
				arrowFrozen.Remove(k);
			}
			
		}
	}
	
	
	void StartParticleLeft(Precision prec){
		((ParticleSystem) precLeft.transform.FindChild(prec.ToString()).particleSystem).Play();
	}
	
	void StartParticleDown(Precision prec){
		((ParticleSystem) precDown.transform.FindChild(prec.ToString()).particleSystem).Play();
	}
	
	void StartParticleUp(Precision prec){
		((ParticleSystem) precUp.transform.FindChild(prec.ToString()).particleSystem).Play();
	}
	
	void StartParticleRight(Precision prec){
		((ParticleSystem) precRight.transform.FindChild(prec.ToString()).particleSystem).Play();
	}
	
	void StartParticleFreezeLeft(bool state){
		var ps = ((ParticleSystem) precLeft.transform.FindChild("FREEZE").particleSystem);
		ps.Play();
		ps.loop = state;
		
	}
	
	void StartParticleFreezeRight(bool state){
		var ps = ((ParticleSystem) precRight.transform.FindChild("FREEZE").particleSystem);
		ps.Play();
		ps.loop = state;
		
	}
	
	void StartParticleFreezeDown(bool state){
		var ps = ((ParticleSystem) precDown.transform.FindChild("FREEZE").particleSystem);
		ps.Play();
		ps.loop = state;
		
	}
	
	void StartParticleFreezeUp(bool state){
		var ps = ((ParticleSystem) precUp.transform.FindChild("FREEZE").particleSystem);
		ps.Play();
		ps.loop = state;
		
	}
	
	
	
	double precToTime(Precision prec){
		switch(prec){
		case Precision.FANTASTIC:
			return 0.0215;
		case Precision.EXCELLENT:
			return 0.043;
		case Precision.GREAT:
			return 0.102;
		case Precision.DECENT:
			return 0.135;
		case Precision.WAYOFF:
			return 0.180;
		default:
			return 0;
		}
	}
	
	Precision timeToPrec(double prec){
		if(prec <= 0.0215){
			return Precision.FANTASTIC;
		}else if(prec <= 0.043){
			return Precision.EXCELLENT;
		}else if(prec <= 0.102){
			return Precision.GREAT;
		}else if(prec <= 0.135){
			return Precision.DECENT;
		}else if(prec <= 0.180){
			return Precision.WAYOFF;
		}
		return Precision.NONE;
		
	}
	
	//Verify keys Input at this frame
	//Null ref exception quand il n'y a plus de flèche !
	void VerifyKeysInput(){
		if(Input.GetKeyDown(KeyCode.LeftArrow) && arrowLeftList.Any()){
			var ar = findNextLeftArrow();
			double prec = Mathf.Abs((float)(ar.time - (timetotalchart - Time.deltaTime)));
			//Debug.Log("AL ! " + prec);
			if(prec < precToTime(Precision.GREAT)){ //great
				if(ar.arrowType == Arrow.ArrowType.NORMAL || ar.arrowType == Arrow.ArrowType.MINE){
					arrowLeftList.Remove(ar);
					DestroyImmediate(ar.goArrow);
					StartParticleLeft(timeToPrec(prec));
				}else{
					arrowLeftList.Remove(ar);
					arrowFrozen.Add(ar,0f);
					ar.displayFrozenBar();
					StartParticleLeft(timeToPrec(prec));
					StartParticleFreezeLeft(true);
				}
				//Debug.Log("Great !");
				
			}else if(prec < precToTime(Precision.WAYOFF)){ //miss
				ar.goArrow.GetComponent<ArrowScript>().missed = true;
				arrowLeftList.Remove(ar);
				StartParticleLeft(timeToPrec(prec));
				//Debug.Log("Miss... !");
			}else{
				//Debug.Log("Not so close");
			}
			
			
			if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 0f && c.Key.arrowType == Arrow.ArrowType.ROLL))
			{
				var froz = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 0f);
				arrowFrozen[froz.Key] = 0f;
			}
		
		}
		
		
		if(Input.GetKeyDown(KeyCode.DownArrow) && arrowDownList.Any()){
			var ar = findNextDownArrow();
			double prec = Mathf.Abs((float)(ar.time - (timetotalchart - Time.deltaTime)));
			//Debug.Log("AL ! " + prec);
			if(prec < (double)0.102){ //great
				if(ar.arrowType == Arrow.ArrowType.NORMAL || ar.arrowType == Arrow.ArrowType.MINE){
					arrowDownList.Remove(ar);
					DestroyImmediate(ar.goArrow);
					StartParticleDown(timeToPrec(prec));
				}else{
					arrowDownList.Remove(ar);
					arrowFrozen.Add(ar,0f);
					ar.displayFrozenBar();
					StartParticleDown(timeToPrec(prec));
					StartParticleFreezeDown(true);
				}
				//Debug.Log("Great !");
			//Start coroutine score
			}else if(prec < (double)0.300){ //miss
				ar.goArrow.GetComponent<ArrowScript>().missed = true;
				arrowDownList.Remove(ar);
				StartParticleDown(timeToPrec(prec));
				//Debug.Log("Miss... !");
			}else{
				//Debug.Log("Not so close");
			}
		
			if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 2f && c.Key.arrowType == Arrow.ArrowType.ROLL))
			{
				var froz = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 2f);
				arrowFrozen[froz.Key] = 0f;
			}
		}
		
		
		if(Input.GetKeyDown(KeyCode.UpArrow) && arrowUpList.Any()){
			var ar = findNextUpArrow();
			double prec = Mathf.Abs((float)(ar.time - (timetotalchart - Time.deltaTime)));
			//Debug.Log("AL ! " + prec);
			if(prec < (double)0.102){ //great
				if(ar.arrowType == Arrow.ArrowType.NORMAL || ar.arrowType == Arrow.ArrowType.MINE){
					arrowUpList.Remove(ar);
					DestroyImmediate(ar.goArrow);
					StartParticleUp(timeToPrec(prec));
				}else{
					arrowUpList.Remove(ar);
					arrowFrozen.Add(ar,0f);
					ar.displayFrozenBar();
					StartParticleUp(timeToPrec(prec));
					StartParticleFreezeUp(true);
				}
			//	Debug.Log("Great !");
			//Start coroutine score
			}else if(prec < (double)0.300){ //miss
				ar.goArrow.GetComponent<ArrowScript>().missed = true;
				arrowUpList.Remove(ar);
				StartParticleUp(timeToPrec(prec));
				//Debug.Log("Miss... !");
			}else{
				//Debug.Log("Not so close");
			}
			
			if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 4f && c.Key.arrowType == Arrow.ArrowType.ROLL))
			{
				var froz = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 4f);
				arrowFrozen[froz.Key] = 0f;
			}
		
		}
		
		
		if(Input.GetKeyDown(KeyCode.RightArrow) && arrowRightList.Any()){
			var ar = findNextRightArrow();
			double prec = Mathf.Abs((float)(ar.time - (timetotalchart - Time.deltaTime)));
			//Debug.Log("AL ! " + prec);
			if(prec < (double)0.102){ //great
				if(ar.arrowType == Arrow.ArrowType.NORMAL || ar.arrowType == Arrow.ArrowType.MINE){
					arrowRightList.Remove(ar);
					DestroyImmediate(ar.goArrow);
					StartParticleRight(timeToPrec(prec));
				}else{
					arrowRightList.Remove(ar);
					arrowFrozen.Add(ar,0f);
					ar.displayFrozenBar();
					StartParticleRight(timeToPrec(prec));
					StartParticleFreezeRight(true);
				}
				//Debug.Log("Great !");
			//Start coroutine score
			}else if(prec < (double)0.300){ //miss
				ar.goArrow.GetComponent<ArrowScript>().missed = true;
				arrowRightList.Remove(ar);
				StartParticleRight(timeToPrec(prec));
				//Debug.Log("Miss... !");
			}else{
				//Debug.Log("Not so close");
			}
			
			if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 6f && c.Key.arrowType == Arrow.ArrowType.ROLL))
			{
				var froz = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 6f);
				arrowFrozen[froz.Key] = 0f;
			}
		
		}
		
	}
	
	
	//Verify Key for all frames
	void VerifyKeysOutput(){
		if(arrowFrozen.Count > 0){
			if(Input.GetKey(KeyCode.LeftArrow)){
				if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 0f && c.Key.arrowType == Arrow.ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 0f);
					arrowFrozen[ar.Key] = 0f;
				}
			
			}
			
			
			if(Input.GetKey(KeyCode.DownArrow)){
				if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 2f && c.Key.arrowType == Arrow.ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 2f);
					arrowFrozen[ar.Key] = 0f;
				}
			
			}
			
			
			if(Input.GetKey(KeyCode.UpArrow)){
				if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 4f && c.Key.arrowType == Arrow.ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 4f);
					arrowFrozen[ar.Key] = 0f;
				}
			}
			
			
			if(Input.GetKey(KeyCode.RightArrow)){
				if(arrowFrozen.Any(c => c.Key.goArrow.transform.position.x == 6f && c.Key.arrowType == Arrow.ArrowType.FREEZE)){
					var ar = arrowFrozen.First(c => c.Key.goArrow.transform.position.x == 6f);
					arrowFrozen[ar.Key] = 0f;
				}
			}
		}
	}
	
	
	
	
	
	//Create the chart
	void createTheChart(Song s){
		
		var ypos = 0f;
		arrowLeftList = new List<Arrow>();
		arrowUpList = new List<Arrow>();
		arrowDownList = new List<Arrow>();
		arrowRightList = new List<Arrow>();
		
		
		var theBPMCounter = 1;
		var theSTOPCounter = 0;
		double mesurecount = 0;
		double prevmesure = 0;
		double timecounter = 0;
		double timeBPM = 0;
		double timestop = 0;
		double timetotal = 0;
		float prec = 0.001f;
		
		var ArrowFreezed = new Arrow[4];
		foreach(var mesure in s.stepchart){
			
			for(int beat=0;beat<mesure.Count;beat++){
			
			
				var bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
				if(theBPMCounter < s.bpms.Count && theSTOPCounter < s.stops.Count){
					while((theBPMCounter < s.bpms.Count && theSTOPCounter < s.stops.Count) 
						&& (s.mesureBPMS.ElementAt(theBPMCounter) < mesurecount - prec || s.mesureSTOPS.ElementAt(theSTOPCounter) < mesurecount - prec)){
					
						if(s.mesureBPMS.ElementAt(theBPMCounter) < s.mesureSTOPS.ElementAt(theSTOPCounter)){
							timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							
							timeBPM += timecounter;
							timecounter = 0;
							prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
							theBPMCounter++;
							bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
							//Debug.Log("And bpm change before / bpm");
						}else if(s.mesureBPMS.ElementAt(theBPMCounter) > s.mesureSTOPS.ElementAt(theSTOPCounter)){
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And stop change before");
						}else{
							timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							timeBPM += timecounter;
							timecounter = 0;
							prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
							theBPMCounter++;
							bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
							
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And bpm change before");
							//Debug.Log("And stop change before");
						}
						
					}
				}else if(theBPMCounter < s.bpms.Count){
					while((theBPMCounter < s.bpms.Count) && s.mesureBPMS.ElementAt(theBPMCounter) < mesurecount - prec){
						
						timecounter += (s.mesureBPMS.ElementAt(theBPMCounter) - prevmesure)/bps;
							
						timeBPM += timecounter;
						timecounter = 0;
						prevmesure = s.mesureBPMS.ElementAt(theBPMCounter);
						theBPMCounter++;
						bps = s.getBPS(s.bpms.ElementAt(theBPMCounter-1).Value);
					
					}
				}else if((theSTOPCounter < s.stops.Count) && theSTOPCounter < s.stops.Count){
					while(s.mesureSTOPS.ElementAt(theSTOPCounter) < mesurecount - prec){
						
						timestop += s.stops.ElementAt(theSTOPCounter).Value;
						theSTOPCounter++;
					
					}
				}
				
				
				timecounter += (mesurecount - prevmesure)/bps;
				
				
				timetotal = timecounter + timeBPM + timestop;
				
				char[] note = mesure.ElementAt(beat).Trim().ToCharArray();
				//var barrow = false;
				for(int i =0;i<4; i++){
					if(note[i] == '1'){
						//var theArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos  + (float)s.offset, 0f), arrow.transform.rotation);
						
						
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						var theArrow = new Arrow(goArrow, Arrow.ArrowType.NORMAL, timetotal);
						switch(i){
						case 0:
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							arrowDownList.Add(theArrow);
							break;
						case 2:
							arrowUpList.Add(theArrow);
							break;
						case 3:
							arrowRightList.Add(theArrow);
							break;
						}
						
						//barrow = true;
					}else if(note[i] == '2'){
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						var theArrow = new Arrow(goArrow, Arrow.ArrowType.FREEZE, timetotal);
						ArrowFreezed[i] = theArrow;
						switch(i){
						case 0:
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							arrowDownList.Add(theArrow);
							break;
						case 2:
							arrowUpList.Add(theArrow);
							break;
						case 3:
							arrowRightList.Add(theArrow);
							break;
						}
						
					}else if(note[i] == '3'){
						var theArrow = ArrowFreezed[i];
						var goFreeze = (GameObject) Instantiate(freeze, new Vector3(i*2, (theArrow.goArrow.transform.position.y + ((-ypos - theArrow.goArrow.transform.position.y)/2f)) , 0.5f), freeze.transform.rotation);
						goFreeze.transform.localScale = new Vector3(1f, -((-ypos - theArrow.goArrow.transform.position.y)/2f), 0.1f);
						goFreeze.transform.GetChild(0).renderer.material.color = theArrow.goArrow.renderer.material.color;
						theArrow.setArrowFreeze(timetotal, new Vector3(i*2,-ypos, 0f), goFreeze, null);
					
					}else if(note[i] == '4'){
						var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						var theArrow = new Arrow(goArrow, Arrow.ArrowType.ROLL, timetotal);
						ArrowFreezed[i] = theArrow;
						switch(i){
						case 0:
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							arrowDownList.Add(theArrow);
							break;
						case 2:
							arrowUpList.Add(theArrow);
							break;
						case 3:
							arrowRightList.Add(theArrow);
							break;
						}
						
					}else if(note[i] == 'M'){
						/*var goArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, -ypos, 0f), arrow.transform.rotation);
						goArrow.renderer.material.color = chooseColor(beat + 1, mesure.Count);
						var theArrow = new Arrow(goArrow, Arrow.ArrowType.MINE, timetotal);
						switch(i){
						case 0:
							arrowLeftList.Add(theArrow);
							break;
						case 1:
							arrowDownList.Add(theArrow);
							break;
						case 2:
							arrowUpList.Add(theArrow);
							break;
						case 3:
							arrowRightList.Add(theArrow);
							break;
						}*/
						
					
					}
					
				}
				
				
				
				//if(barrow)Debug.Log("ARROW : " + timetotal);
				//if(!barrow)Debug.Log("no arrow : " + timetotal);	
				
				
				if(theBPMCounter < s.bpms.Count){
					if(Mathf.Abs((float)(s.mesureBPMS.ElementAt(theBPMCounter) - mesurecount)) < prec){
							timeBPM += timecounter;
							timecounter = 0;
							theBPMCounter++;
							//Debug.Log("And bpm change");
					}
				}
				
				if(theSTOPCounter < s.stops.Count){
					if(Mathf.Abs((float)(s.mesureSTOPS.ElementAt(theSTOPCounter) - mesurecount)) < prec){
							timestop += s.stops.ElementAt(theSTOPCounter).Value;
							theSTOPCounter++;
							//Debug.Log("And stop");
					}
				}
				prevmesure = mesurecount;
				mesurecount += (4f/(float)mesure.Count);
				
				
				ypos += (4f/(float)mesure.Count)*speedmod;
				
			}
		}
	}
	
	
	
	
	
	//Remove key from arrow list
	public void removeArrowFromList(Arrow ar, string state){
		
		switch(state){
			case "left":
				arrowLeftList.Remove(ar);
				break;
			case "down":
				arrowDownList.Remove(ar);
				break;
			case "up":
				arrowUpList.Remove(ar);
				break;
			case "right":
				arrowRightList.Remove(ar);
				break;
				
		}
	}
	
	
	
	
	
	public Arrow findNextUpArrow(){

		return arrowUpList.FirstOrDefault(s => s.time == arrowUpList.Min(c => c.time));
			
	}
	
	public Arrow findNextDownArrow(){

		return arrowDownList.FirstOrDefault(s => s.time == arrowDownList.Min(c => c.time));
			
	}
	
	public Arrow findNextLeftArrow(){

		return arrowLeftList.FirstOrDefault(s => s.time == arrowLeftList.Min(c => c.time));
			
	}
	
	public Arrow findNextRightArrow(){

		return arrowRightList.FirstOrDefault(s => s.time == arrowRightList.Min(c => c.time));
			
	}
	
	
	public double getTotalTimeChart(){
		return timetotalchart;	
	}
	
	
	
	Color chooseColor(int posmesure, int mesuretot){
		
		
		switch(mesuretot){
		case 4:	
			return new Color(1f, 0f, 0f, 1f);
		
			
		case 8:
			if(posmesure%2 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}
			return new Color(1f, 0f, 0f, 1f);
			
			
		case 12:
			if((posmesure-1)%3 == 0){
				return new Color(1f, 0f, 0f, 1f);
			}
			return new Color(1f, 0f, 1f, 1f);
			
		case 16:
			if((posmesure-1)%4 == 0){
				return new Color(1f, 0f, 0f, 1f);
			}else if((posmesure+1)%4 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}
			return new Color(0f, 1f, 0f, 1f);
		
			
		case 24:
			if((posmesure+2)%6 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}else if((posmesure-1)%6 == 0){
				return new Color(1f, 0f, 0f, 1f);
			}
			return new Color(1f, 0f, 1f, 1f);
			
			
		case 32:
			if((posmesure-1)%8 == 0){
				return new Color(1f, 0f, 0f, 1f);
			}else if((posmesure+3)%8 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}else if((posmesure+1)%4 == 0){
				return new Color(0f, 1f, 0f, 1f);
			}
			return new Color(1f, 0.8f, 0f, 1f);
			
		case 48:
			if((posmesure+2)%6 == 0){
				return new Color(0f, 1f, 0f, 1f);
			}else if((posmesure-1)%12 == 0){
				return new Color(1f, 0f, 0f, 1f);
			}else if((posmesure+5)%12 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}
			return new Color(1f, 0f, 1f, 1f);
		
			
		case 64:
			if((posmesure-1)%16 == 0){
				return new Color(1f, 0f, 0f, 1f);
			}else if((posmesure+7)%16 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}else if((posmesure+3)%8 == 0){
				return new Color(0f, 1f, 0f, 1f);
			}else if((posmesure+1)%4 == 0){
				return new Color(1f, 0.8f, 0f, 1f);
			}
			return new Color(0f, 1f, 0.8f, 1f);
			
		case 192:
			if((posmesure-1)%48 == 0){
				return new Color(1f, 0f, 0f, 1f);
			}else if((posmesure+13)%48 == 0){
				return new Color(0f, 0f, 1f, 1f);
			}else if((posmesure+11)%24 == 0){
				return new Color(0f, 1f, 0f, 1f);
			}else if((posmesure+5)%12 == 0){
				return new Color(1f, 0.8f, 0f, 1f);
			}
			return new Color(0f, 1f, 0.8f, 1f);
			
		default:
			return new Color(1f, 1f, 1f, 1f);
			
		}
	
		
	}
}
