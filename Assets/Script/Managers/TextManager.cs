using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
public class TextManager {
	
	private static TextManager instance;
	
	public static TextManager Instance{
		get{
			if(instance == null){
			instance = new TextManager();	
			}
			return instance;
		
		}
	}
	
	public Dictionary<string, Dictionary<string, string>> texts;
	
	private TextManager(){

		
	}
	
	
	public void LoadTextFile(){
		TextAsset textContener = (TextAsset) Resources.Load("Texts/texts");
		
		var textContent = (string[]) textContener.text.Split(new string[] { ";" }, StringSplitOptions.None);
		texts = new Dictionary<string, Dictionary<string, string>>();
		var actuelCat = "";
		var actuelSent = "";
		for(int i=0;i<textContent.Length;i++){
			if(textContent[i].Contains("[") && textContent[i].Contains("]")){
				actuelCat = textContent[i].Replace("[", "").Replace("]", "").Trim();
				texts.Add(actuelCat, new Dictionary<string, string>());
			}else if(textContent[i].Contains("<") && textContent[i].Contains(">")){
				actuelSent = textContent[i].Replace("<", "").Replace(">","").Trim();
				texts[actuelCat].Add(actuelSent, "");
			}else if(!String.IsNullOrEmpty(textContent[i])){
				texts[actuelCat][actuelSent] = textContent[i].Trim();
			}
		}
	}
	
}
