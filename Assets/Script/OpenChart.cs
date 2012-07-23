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
	
	
	
	
	
	public List<Song> readChart(string chartname){
		
		
		
		//read file
		var files = (string[]) Directory.GetFiles(Application.dataPath + "/Songs/" + chartname);
		StreamReader sr = new StreamReader(files.FirstOrDefault(c => c.Contains(".sm")));
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
		
		List<Song> outputSongs = new List<Song>();
		
		
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
			theBpmListMesured.Add(System.Convert.ToDouble(mysplit[0]), System.Convert.ToDouble(mysplit[1]));
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
		if(!listLine.ElementAt(stopbegin).Contains("STOPS:;")){
			for(int i=stopbegin; listLine.ElementAt(i).Trim() != ";"; i++){
				thestop += listLine.ElementAt(i);
			}
			
			thestop.Replace("/n", "");
			thestop.Replace(";", "");
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
			theNewsong.offset = offset;
			theNewsong.samplestart = samplestart;
			theNewsong.samplelenght = samplelenght;
			theNewsong.bpms = theBpmList;
			theNewsong.stops = theStopList;
			theNewsong.mesureBPMS = theBpmMesureList;
			theNewsong.mesureSTOPS = theStopMesureList;
			var thewww = new WWW("file://" + files.FirstOrDefault(c => c.Contains(".ogg")).Replace('\\', '/'));
			
			while(!thewww.isDone){ }
			theNewsong.song = thewww.GetAudioClip(false, true);
			
			/*Debug.Log(files.FirstOrDefault(c => c.Contains(".ogg")).Replace(".ogg", ""));
			theNewsong.song = (AudioClip) Resources.Load("Broken the Moon");
			Debug.Log(theNewsong.song.length);*/
			//getting song information
			int beginInformation = index;
			theNewsong.stepartist = listLine.ElementAt(beginInformation + 2).Replace(":","").Trim();
			theNewsong.setDifficulty(listLine.ElementAt(beginInformation + 3).Replace(":","").Trim());
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
			outputSongs.Add(theNewsong);
			
		
		}
		
		return outputSongs;
	}
	
	
}
