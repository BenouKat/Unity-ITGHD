using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;

public class OpenChart{
	
	
	private static OpenChart instance;
	
	public static OpenChart Instance{
		get{
			if(instance == null){
			instance = new OpenChart();	
			}
			return instance;
		
		}
	}
	
	private OpenChart(){
	
	}
	
	
	
	
	
	
	private static string songContent;
	
	
	
	
	
	public Dictionary<Difficulty, Song> readChart(string directory){
		
		
		
		//read file
		
		var files = (string[]) Directory.GetFiles(directory);
		
		var stream = files.FirstOrDefault(c => (c.Contains(".sm") || c.Contains(".SM")) && !c.Contains(".old")  && !c.Contains(".dwi") && !c.Contains(".DWI") && !c.Contains("._"));
		if(stream == null) stream = files.FirstOrDefault(c => (c.Contains(".dwi") || c.Contains(".DWI"))   && !c.Contains(".old") && !c.Contains("._"));
		if(stream == null) return null;
		StreamReader sr = new StreamReader(stream);
		songContent = sr.ReadToEnd();
    	sr.Close();
		
		//split all line and put on a list
		var thesong = songContent.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		List<string> listLine = new List<string>();
		listLine.AddRange(thesong);
		List<int> indexNotes = new List<int>();
		
		for(int i=0; i < listLine.Count;i++){
			if(listLine.ElementAt(i).Contains("NOTES")){
				indexNotes.Add(i);
			}
		}
		
		Dictionary<Difficulty, Song>  outputSongs = new Dictionary<Difficulty, Song> ();
		
		try{
			//get generic information
			var thetitle = listLine.FirstOrDefault(c => c.Contains("TITLE")).Split(':');
			var title = thetitle[1].Replace(";","");
			var thesubtitle = listLine.FirstOrDefault(c => c.Contains("SUBTITLE")).Split(':');
			var subtitle = thesubtitle[1].Replace(";","");
			var theartist = listLine.FirstOrDefault(c => c.Contains("ARTIST")).Split(':');
			var artist = theartist[1].Replace(";","");
			var theoffset = listLine.FirstOrDefault(c => c.Contains("OFFSET")).Split(':');
			var offset = System.Convert.ToDouble(theoffset[1].Replace(";",""));
			var thesamplestart = listLine.FirstOrDefault(c => c.Contains("SAMPLESTART")).Split(':');
			var samplestart = System.Convert.ToDouble(thesamplestart[1].Replace(";",""));
			var thesamplelenght = listLine.FirstOrDefault(c => c.Contains("SAMPLELENGTH")).Split(':');
			var samplelenght = System.Convert.ToDouble(thesamplelenght[1].Replace(";",""));
			//"file://" + 
			var theBanner = listLine.FirstOrDefault(c => c.Contains("BANNER")).Split(':');
			var bannerTemp = files.FirstOrDefault(c => c.Contains(theBanner[1].Replace(";",""))).Replace('\\', '/');
			var banner = "noBanner";
			if(!String.IsNullOrEmpty(bannerTemp)){
				banner = "file://" +  bannerTemp;
			}
			var theBpmListMesured = new Dictionary<double, double>();
			var theStopListMesured = new Dictionary<double, double>();
			
			var theBpmList = new Dictionary<double, double>();
			var theStopList = new Dictionary<double, double>();
			
			var theBpmMesureList = new List<double>();
			var theStopMesureList = new List<double>();
			
			
			
			//getting bpms with mesure
			string[] thebpm = listLine.FirstOrDefault(c => c.Contains("BPMS")).Split(':');
			string thebpmline = thebpm[1];
			string[] splitbpm = thebpmline.Split(',');
			/*double previousbps = 0;
			double previoustime = 0;
			double previousmesure = 0;
			List<double> timeToMesureBPM = new List<double>();*/
			foreach(string s in splitbpm){
				string[] mysplit = s.Replace(";","").Split('=');
				if(!theBpmListMesured.ContainsKey(System.Convert.ToDouble(mysplit[0]))) theBpmListMesured.Add(System.Convert.ToDouble(mysplit[0]), System.Convert.ToDouble(mysplit[1]));
				//theBpmList.Add(previousbps == 0 ? 0 : previoustime + (System.Convert.ToDouble(mysplit[0]) - previousmesure)/previousbps, System.Convert.ToDouble(mysplit[1]));
				/*previousbps = System.Convert.ToDouble(mysplit[1])/(double)60.0;
				previoustime = theBpmList.Last().Key;
				previousmesure = System.Convert.ToDouble(mysplit[0]);
				timeToMesureBPM.Add(System.Convert.ToDouble(mysplit[0]));
				Debug.Log ("bpm : " + theBpmList.Last().Key);*/
			}
			
			//getting stops mesured
			
			int stopbegin = listLine.IndexOf(listLine.FirstOrDefault(c => c.Contains("STOPS")));
			string thestop = "";
			bool stopTheRool = false;
			if(!listLine.ElementAt(stopbegin).Contains("STOPS:;")){
				for(int i=stopbegin; !stopTheRool; i++){
					if( listLine.ElementAt(i).Contains("//")){
						thestop += listLine.ElementAt(i).Split('/')[0];
					}else{
						thestop += listLine.ElementAt(i);
					}
					
					if(listLine.ElementAt(i).Contains(";")) stopTheRool = true;
				}
				thestop = thestop.Replace("/n", "");
				thestop = thestop.Replace(";", "");
			}
			
			
			if(thestop != ""){
				string[] thestopsplit = thestop.Split(':');
				string thestopline = thestopsplit[1];
				string[] splitstop = thestopline.Split(',');
				/*previousbps = theBpmList.First().Value;
				previoustime = 0;
				previousmesure = 0;*/
				foreach(string s in splitstop){
	
					var mysplit = s.Split('=');
					
					theStopListMesured.Add(System.Convert.ToDouble(mysplit[0]), System.Convert.ToDouble(mysplit[1]));
					/*
					var theMesure = timeToMesureBPM.IndexOf(timeToMesureBPM.FirstOrDefault(d => d == timeToMesureBPM.Where(c => c <= System.Convert.ToDouble(mysplit[0])).Max()));
					previousbps = System.Convert.ToDouble(theBpmList.ElementAt(theMesure).Value)/(double)60.0;
					previoustime = theBpmList.ElementAt(theMesure).Key;
					previousmesure = timeToMesureBPM.ElementAt(theMesure);
					
					theStopList.Add(previoustime + ((System.Convert.ToDouble(mysplit[0])) - previousmesure)/previousbps, System.Convert.ToDouble(mysplit[1]));
					
					Debug.Log("real mesure : " + (System.Convert.ToDouble(mysplit[0])) + " analyse : " + theStopList.Last().Key + " : " + theStopList.Last().Value + " : " + 
						theMesure + " previous time : " + previoustime + " previous mesure : " + 
						previousmesure + " previous bps " + previousbps + " (" + previousbps*60.0 + ") bpm");*/
				}
			}
			
			//change bpm and stops mesured by time
			//Bug encore.
			double previousbps = 0;
			double stoptime = 0;
			double previoustime = 0;
			double previousmesure = 0;
			
			while(theStopListMesured.Count != 0 && theBpmListMesured.Count != 0){
				if((theStopListMesured.First().Key < theBpmListMesured.First().Key)){
					
					
					theStopList.Add(previoustime + stoptime + (theStopListMesured.First().Key - previousmesure)/previousbps, theStopListMesured.First().Value);
					theStopMesureList.Add(theStopListMesured.First().Key);
					
					previoustime += (theStopListMesured.First().Key - previousmesure)/previousbps;
					previousmesure = theStopListMesured.First().Key;
					stoptime += theStopListMesured.First().Value;
					
					
					theStopListMesured.Remove(theStopListMesured.First().Key);
				
				
				}else if((theStopListMesured.First().Key > theBpmListMesured.First().Key)){
					
					
					theBpmList.Add(previousbps == 0 ? 0 : previoustime + stoptime + (theBpmListMesured.First().Key - previousmesure)/previousbps, theBpmListMesured.First().Value);
					theBpmMesureList.Add(theBpmListMesured.First().Key);
					
					previoustime += (previousbps == 0 ? 0 : (theBpmListMesured.First().Key - previousmesure)/previousbps);
					previousbps = theBpmList.Last().Value/(double)60.0;
					previousmesure = theBpmListMesured.First().Key;
					
					
					theBpmListMesured.Remove(theBpmListMesured.First().Key);
					
				}else if(theStopListMesured.First().Key == theBpmListMesured.First().Key){
					
					
					theStopList.Add(previousbps == 0 ? 0 : previoustime + stoptime + (theStopListMesured.First().Key - previousmesure)/previousbps, theStopListMesured.First().Value);
					theStopMesureList.Add(theStopListMesured.First().Key);
					
					theBpmList.Add(previousbps == 0 ? 0 : previoustime + stoptime + (theBpmListMesured.First().Key - previousmesure)/previousbps, theBpmListMesured.First().Value);
					theBpmMesureList.Add(theBpmListMesured.First().Key);
					
					previoustime += (previousbps == 0 ? 0 : (theBpmListMesured.First().Key - previousmesure)/previousbps);
					previousbps = theBpmList.Last().Value/(double)60.0;
					previousmesure = theBpmListMesured.First().Key;
					stoptime += theStopListMesured.First().Value;
					
					
					
					theStopListMesured.Remove(theStopListMesured.First().Key);
					theBpmListMesured.Remove(theBpmListMesured.First().Key);
				}
				
			}
			
			while(theStopListMesured.Count != 0){
	
					
					theStopList.Add(previoustime + stoptime + (theStopListMesured.First().Key - previousmesure)/previousbps, theStopListMesured.First().Value);
					theStopMesureList.Add(theStopListMesured.First().Key);
					
					previoustime += (theStopListMesured.First().Key - previousmesure)/previousbps;
					previousmesure = theStopListMesured.First().Key;
					stoptime += theStopListMesured.First().Value;
					
					
					theStopListMesured.Remove(theStopListMesured.First().Key);
				
			}
			
			while(theBpmListMesured.Count != 0){
				
					
					theBpmList.Add(previousbps == 0 ? 0 : previoustime + stoptime + (theBpmListMesured.First().Key - previousmesure)/previousbps, theBpmListMesured.First().Value);
					theBpmMesureList.Add(theBpmListMesured.First().Key);
					
					previoustime += (previousbps == 0 ? 0 : (theBpmListMesured.First().Key - previousmesure)/previousbps);
					previousbps = theBpmList.Last().Value/(double)60.0;
					previousmesure = theBpmListMesured.First().Key;
					
					
					theBpmListMesured.Remove(theBpmListMesured.First().Key);
					
			}
			
			//debug
			/*foreach(var el in theStopList){
				Debug.Log(el.Key);	
			}*/
			
			//For all difficulties
			foreach(int index in indexNotes){
			
				var theNewsong = new Song();
				
				//easy variable getted
				theNewsong.title = title;
				theNewsong.subtitle = subtitle;
				theNewsong.artist = artist;
				theNewsong.banner = banner;
				theNewsong.offset = offset;
				theNewsong.samplestart = samplestart;
				theNewsong.samplelenght = samplelenght;
				theNewsong.bpms = theBpmList;
				theNewsong.stops = theStopList;
				theNewsong.mesureBPMS = theBpmMesureList;
				theNewsong.mesureSTOPS = theStopMesureList;
				/*if(files.FirstOrDefault(c => c.Contains(".ogg")) == null){
					foreach(var fil in files){
						Debug.Log(fil);	
					}
				}*/
				theNewsong.song = "file://" + files.FirstOrDefault(c => c.Contains(".ogg") || c.Contains(".OGG")).Replace('\\', '/');
				
				
				//getting song information
				int beginInformation = index;
				string dl = "";
				var theinfo = listLine.ElementAt(beginInformation + 1).Replace(":","").Trim();
				if(theinfo.Contains("double")){
					dl = "D";
				}else
				if(!theinfo.Contains("single") || theinfo.Contains("pump") || theinfo.Contains("ez2") || 
						theinfo.Contains("para") || theinfo.Contains("ds3ddx") || theinfo.Contains("pnm") || 
						theinfo.Contains("bm") || theinfo.Contains("maniax") || theinfo.Contains("techno")){
					dl = "STOP";
				}
				theNewsong.stepartist = listLine.ElementAt(beginInformation + 2).Replace(":","").Trim();
				theNewsong.setDifficulty(dl + listLine.ElementAt(beginInformation + 3).Replace(":","").Trim());
				theNewsong.level = System.Convert.ToInt32(listLine.ElementAt(beginInformation + 4).Replace(":","").Trim());
				
				
				//getting stepchart
				int beginstepchart = beginInformation+6;
				while(listLine.ElementAt(beginstepchart).Contains("//") || listLine.ElementAt(beginstepchart).Trim() == ""){
					beginstepchart++;	
				}
				
				var numberOfSteps = 0;
				var numberOfMines = 0;
				var numberOfRoll = 0;
				var numberOfFreezes = 0;
				var numberOfJump = 0;
				var numberOfStepsWJ = 0;
				theNewsong.stepchart.Add(new List<string>());
				for(int i = beginstepchart; !listLine.ElementAt(i).Contains(";"); i++){
					if(listLine.ElementAt(i).Contains(",")){
						theNewsong.stepchart.Add(new List<string>());
					}else{
						theNewsong.stepchart.Last().Add(listLine.ElementAt(i));	
						numberOfSteps += listLine.ElementAt(i).Count(c => c == '1');
						numberOfSteps += listLine.ElementAt(i).Count(c => c == '2');
						numberOfSteps += listLine.ElementAt(i).Count(c => c == '4');
						numberOfFreezes += listLine.ElementAt(i).Count(c => c == '2');
						numberOfRoll += listLine.ElementAt(i).Count(c => c == '4');
						numberOfMines += listLine.ElementAt(i).Count(c => c == 'M');
						numberOfStepsWJ += listLine.ElementAt(i).Count(c => c == '1');
						numberOfStepsWJ += listLine.ElementAt(i).Count(c => c == '2');
						numberOfStepsWJ += listLine.ElementAt(i).Count(c => c == '4');
						
						var countmesure = listLine.ElementAt(i).Count(c => c == '1') + listLine.ElementAt(i).Count(c => c == '2') + listLine.ElementAt(i).Count(c => c == '4');
						if(countmesure >= 2){
							numberOfStepsWJ -= countmesure;
							numberOfJump++;
						}
					}
				}
				
				theNewsong.numberOfSteps = numberOfSteps;
				theNewsong.numberOfFreezes = numberOfFreezes;
				theNewsong.numberOfRolls = numberOfRoll;
				theNewsong.numberOfMines = numberOfMines;
				theNewsong.numberOfJumps = numberOfJump;
				theNewsong.numberOfStepsWithoutJumps = numberOfStepsWJ;
					//A mettre
				//if(outputSongs.ContainsKey(theNewsong.difficulty))
				if(dl != "STOP") outputSongs.Add(theNewsong.difficulty, theNewsong);
				
			
			}
		}catch(Exception e){
			Debug.Log(directory + " // " + e.Message + " //st : " + e.StackTrace);	
		}
		
		return outputSongs;
		
	}
	
	
	void fakeCreation(Song s){
				
		var theBPMCounter = 1;
		var theSTOPCounter = 0;
		double mesurecount = 0;
		double prevmesure = 0;
		double timecounter = 0;
		double timeBPM = 0;
		double timestop = 0;
		double timetotal = 0;
		float prec = 0.001f;
		
		//stepBySecondsAverage
		double timestart = -10f;
		double stoptime = 0;
		int countStep = 0;
		double stepbysecAv = 0f;
		
		
		//stepmax
		double maxStepPerSeconds = 0f;
		int numberStepBetweenTwoBeat = 0;
		double timestartMax = -10f;
		double maxLenght = 0f;
		
		
		//longestStream
		double lenghtStream = 0f;
		double maxLenghtStream = 0f;
		double speedOfMaxStream;
		
		
		//Footswitch
		int numberOfFootswitch = 0;
		int[] casevalidate = new int[4]; //left down up right
		casevalidate[0] = 0;
		casevalidate[1] = 0;
		casevalidate[2] = 0;
		casevalidate[3] = 0;
		
		//Cross
		int numberOfCross = 0;
		int[] caseCrossvalidate = new int[4]; //left down up right
		caseCrossvalidate[0] = 0;
		caseCrossvalidate[1] = 0;
		caseCrossvalidate[2] = 0;
		caseCrossvalidate[3] = 0;
		
		//Hands
		int numberOfHands = 0;
		
		//freeze
		var freezed = new int[4];
		freezed[0] = 0;
		freezed[1] = 0;
		freezed[2] = 0;
		freezed[3] = 0;
		foreach(var mesure in s.stepchart){
			
			for(int beat=0;beat<mesure.Count;beat++){
			
			#region BPMChange
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
				
				#endregion
				timecounter += (mesurecount - prevmesure)/bps;
				
				
				timetotal = timecounter + timeBPM + timestop;
				
				char[] note = mesure.ElementAt(beat).Trim().ToCharArray();
				
				if((beat*4f)%(mesure.Count) == 0){
					var newMax = numberStepBetweenTwoBeat/(timetotal - timestartMax);
					if(Mathf.Abs((float)(newMax - maxStepPerSeconds)) < 0.001f){
						maxLenght += (timetotal - timestartMax);
						lenghtStream += (timetotal - timestartMax);
						if(lenghtStream > maxLenghtStream){
							speedOfMaxStream = newMax;
							maxLenghtStream = lenghtStream;
						}
					}else if(maxStepPerSeconds < newMax){
						maxStepPerSeconds = newMax;
						maxLenght = (timetotal - timestartMax);
						maxLenghtStream = (timetotal - timestartMax);
					}else if(maxStepPerSeconds > newMax){
						maxLenghtStream = (timetotal - timestartMax);
					}
					
				
					numberStepBetweenTwoBeat = 0;
					timestartMax = timetotal;
				}
				var barr = false;
				
				var iselected = -1;
				var doubleselection = false;
				var tripleselect = 0;
				
				for(int i =0;i<4; i++){
					
					if(note[i] == '1'){
						if(iselected == -1){
							iselected = i;
						}else{
							doubleselection = true;
						}
						tripleselect++;
						barr = true;
						
					}else if(note[i] == '2'){
						barr = true;
						if(iselected == -1){
							iselected = i;
						}else{
							doubleselection = true;
						}
						freezed[i] = 1;
					}else if(note[i] == '3'){
						freezed[i] = 0;
					}else if(note[i] == '4'){
						barr = true;
						if(iselected == -1){
							iselected = i;
						}else{
							doubleselection = true;
						}
						freezed[i] = 1;
					}else if(note[i] == 'M'){
						
						
					
					}
					
				}
				
				for(int i=0;i<4;i++){
					if(freezed[i] == 1) tripleselect++;
				}
				
				if(tripleselect >= 3f){
					numberOfHands++;
				}
				
				if(barr == true){
				
					if(timestart == -10f) timestart = timetotal;
					stoptime = timetotal;
					countStep++;
					numberStepBetweenTwoBeat++;
					
					if(!doubleselection){
						switch(iselected){
							case 0:
								if(((casevalidate[1] == 2 && casevalidate[2] == 0) || (casevalidate[2] == 2 && casevalidate[1] == 0)) && casevalidate[0] == 0 && casevalidate[3] == 1){
									numberOfFootswitch++;
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 0;
								}else{
									casevalidate[0] = 1;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 0;
								}
							
								if(caseCrossvalidate[0] == 0 && (((caseCrossvalidate[1] == 1 || caseCrossvalidate[1] == 2) && caseCrossvalidate[2] == 0) 
								|| (caseCrossvalidate[1] == 0 && (caseCrossvalidate[2] == 1 || caseCrossvalidate[2] == 2)) && caseCrossvalidate[3] == 1)){
									numberOfCross++;
									caseCrossvalidate[0] = 1;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}else{
									caseCrossvalidate[0] = 1;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}
								break;
							case 1:
								if(((casevalidate[0] == 0 && casevalidate[3] == 1) || (casevalidate[0] == 1 && casevalidate[3] == 0)) && casevalidate[2] == 0 &&  casevalidate[1] < 2){
									casevalidate[1]++;
								}else{
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 0;
								}
							
								if((caseCrossvalidate[0] == 1 || caseCrossvalidate[3] == 1)){
									if(caseCrossvalidate[0] == 1 && caseCrossvalidate[1] == 1 && caseCrossvalidate[2] == 2 && caseCrossvalidate[3] == 0){
										caseCrossvalidate[2] = 0;
									}
									if(caseCrossvalidate[0] == 0 && caseCrossvalidate[1] == 1 && caseCrossvalidate[2] == 2 && caseCrossvalidate[3] == 1){
										caseCrossvalidate[2] = 0;
									}
								
									caseCrossvalidate[1] = 2;
									if(caseCrossvalidate[2] == 2) caseCrossvalidate[2] = 1;
								
								}else{
									caseCrossvalidate[0] = 0;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}
								break;
							case 2:
								if(((casevalidate[0] == 0 && casevalidate[3] == 1) || (casevalidate[0] == 1 && casevalidate[3] == 0)) && casevalidate[1] == 0 &&  casevalidate[2] < 2){
									casevalidate[2]++;
								}else{
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 0;
								}
							
								if((caseCrossvalidate[0] == 1 || caseCrossvalidate[3] == 1)){
									if(caseCrossvalidate[0] == 1 && caseCrossvalidate[1] == 2 && caseCrossvalidate[2] == 1 && caseCrossvalidate[3] == 0){
										caseCrossvalidate[1] = 0;
									}
									if(caseCrossvalidate[0] == 0 && caseCrossvalidate[1] == 2 && caseCrossvalidate[2] == 1 && caseCrossvalidate[3] == 1){
										caseCrossvalidate[1] = 0;
									}
								
									caseCrossvalidate[2] = 2;
									if(caseCrossvalidate[1] == 2) caseCrossvalidate[1] = 1;
									
									
								}else{
									caseCrossvalidate[0] = 0;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}
								break;
							case 3:
								if(((casevalidate[1] == 2 && casevalidate[2] == 0) || (casevalidate[2] == 2 && casevalidate[1] == 0)) && casevalidate[3] == 0 && casevalidate[0] == 1){
									numberOfFootswitch++;
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 1;
								}else{
									casevalidate[0] = 0;
									casevalidate[1] = 0;
									casevalidate[2] = 0;
									casevalidate[3] = 1;
								}
							
								if(caseCrossvalidate[0] == 1 && (( caseCrossvalidate[1] == 2 && caseCrossvalidate[2] == 0) 
									|| (caseCrossvalidate[1] == 0 &&  caseCrossvalidate[2] == 2) && caseCrossvalidate[3] == 0)){
									numberOfCross++;
									caseCrossvalidate[0] = 0;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 0;
								}else{
									caseCrossvalidate[0] = 0;
									caseCrossvalidate[1] = 0;
									caseCrossvalidate[2] = 0;
									caseCrossvalidate[3] = 1;
								}
								break;
						}
					}
				}
				
					
				#region ChangeBPM
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
				
				#endregion
				prevmesure = mesurecount;
				mesurecount += (4f/(float)mesure.Count);
				
			}
		}
		
		stepbysecAv = (double)countStep/(stoptime - timestart);
		s.stepPerSecondAverage = stepbysecAv;
		s.stepPerSecondMaximum = maxStepPerSeconds;
		s.timeMaxStep = maxLenght;
		s.stepPerSecondStream = speedOfMaxStream;
		s.longestStream = maxLenghtStream;
		s.numberOfFootswitch = numberOfFootswitch;
		s.numberOfCross = numberOfCross;
		s.numberOfHands = numberOfHands;
	}
	
	
}
