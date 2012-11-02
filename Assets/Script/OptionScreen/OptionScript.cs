using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum StateOption{
	OPTIONSELECTION,
	CUBEFADEIN,
	SCREENFADEIN,
	CUBEFADEOUT,
	SCREENFADEOUT,
	OPTION
}

public class OptionScript : MonoBehaviour {
	
	
	private GameObject theSelected;
	public GameObject cubeIcon;
	public GameObject screen;
	public GameObject bgScreen;
	
	public GameObject[] menuItem;
	private Vector3[] Rotate;
	public float speedScale;
	private float loadColor;
	public float speedColor;
	public float speedColorFade;
	
	public Vector2 sizeLabelBG;
	public Rect offsetLabelOption;
	
	public Rect labelOption;
	
	
	private StateOption optionMenuMode;
	private string optionSelected;
	
	private float alphaFadeIn;
	public float speedFadeIn;
	
	public Dictionary<string, Texture2D> tex;
	
	// Use this for initialization
	void Start () {
		
		tex = new Dictionary<string, Texture2D>();
		tex.Add("labelbg", (Texture2D) Resources.Load("GUIBarMini"));
		
		optionMenuMode = StateOption.OPTIONSELECTION;
		alphaFadeIn = 1f;
	}
	
	void OnGui(){
		switch(optionMenuMode){
			case StateOption.OPTIONSELECTION:
				OnGUIOptionSelect();
				break;
			case StateOption.CUBEFADEIN:
				OnGUIOptionSelect();
				OnGUIOptionFadeIn();
				break;
			case StateOption.SCREENFADEIN:
				OnGUIOptionFadeIn();
				break;
		}
	}
	
	void OnGUIOptionSelect(){
		GUI.color = new Color(1f, 1f, 1f, alphaFadeIn);
		if(theSelected != null){
			var pos2D = Camera.main.WorldToScreenPoint(theSelected.transform.position);
			GUI.DrawTexture(new Rect(pos2D.x, pos2D.y, sizeLabelBG.x*Screen.width, sizeLabelBG.y*Screen.height), tex["labelBG"]);
			GUI.Label(new Rect(pos2D.x + offsetLabelOption.x*Screen.width, pos2D.y + offsetLabelOption.y*Screen.height, offsetLabelOption.width*Screen.width, offsetLabelOption.height*Screen.height), theSelected.name);
		}
	}
	
	void OnGUIOptionFadeIn(){
		GUI.color = new Color(1f, 1f, 1f, 1 - alphaFadeIn);
		GUI.Label(new Rect(labelOption.x*Screen.width, labelOption.y*Screen.height, labelOption.width*Screen.width, labelOption.height*Screen.height), optionSelected);
	}
	
	// Update is called once per frame
	
	void Update(){
		switch(optionMenuMode){
			case StateOption.OPTIONSELECTION:
				UpdateOptionSelect();
				break;
			case StateOption.CUBEFADEIN:
				UpdateCubeFadeIn();
				break;
			case StateOption.SCREENFADEIN:
				UpdateScreenFadeIn();
				break;
		}
	}
	
	void UpdateOptionSelect () {
		
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
			RaycastHit hit;
					
			if(Physics.Raycast(ray, out hit))
			{
				
				var theGo = hit.transform.gameObject;
				if(theGo != null){
					theSelected = theGo;
					if(!theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Play();
					theSelected.renderer.material.color = Color.Lerp(theSelected.renderer.material.color, theSelected.GetComponent<ColorGO>().myColor, loadColor);
					theSelected.transform.localScale = Vector3.Lerp(theSelected.transform.localScale, new Vector3(1.4f, 1.4f, 1.4f), Time.deltaTime/speedScale);
					if(loadColor < 1f) loadColor += Time.deltaTime*speedColor;
				}else if(theSelected != null){
					loadColor = 0f;
					if(theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Stop();
					theSelected = null;
				}
				
			}else if(theSelected != null){
				loadColor = 0f;
				if(theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Stop();
				theSelected = null;
			}
			
			var white = new Color(1f, 1f, 1f, 1f);
			for(int i = 0; i < menuItem.Length;i++){
				if(menuItem[i].renderer.material.color != white && menuItem[i] != theSelected){
					var r = menuItem[i].renderer.material.color.r < 1f ? menuItem[i].renderer.material.color.r + Time.deltaTime*speedColorFade : 1f;
					var g = menuItem[i].renderer.material.color.g < 1f ? menuItem[i].renderer.material.color.g + Time.deltaTime*speedColorFade : 1f;
					var b = menuItem[i].renderer.material.color.b < 1f ? menuItem[i].renderer.material.color.b + Time.deltaTime*speedColorFade : 1f;
					menuItem[i].renderer.material.color = new Color( r, g, b, 1f);
				}
				if(menuItem[i].transform.localScale.x > 1.21f){
					menuItem[i].transform.localScale = Vector3.Lerp(menuItem[i].transform.localScale, new Vector3(1.2f, 1.2f, 1.2f), Time.deltaTime/speedScale);
				}else if(menuItem[i].transform.localScale.x != 1.2f){
					menuItem[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);	
				}
				
			}
			
			
			if(theSelected != null && Input.GetMouseButtonDown(0)){
				if(theSelected.transform.GetChild(0).particleSystem.isPlaying) theSelected.transform.GetChild(0).particleSystem.Stop();
				optionSelected = theSelected.name;
				optionMenuMode = StateOption.CUBEFADEIN;
			}
		
	}
	
	void UpdateCubeFadeIn(){
		for(int i = 0; i < menuItem.Length;i++){
			alphaFadeIn -= speedFadeIn*Time.deltaTime;
			menuItem[i].renderer.material.color = new Color(menuItem[i].renderer.material.color.r, menuItem[i].renderer.material.color.g, menuItem[i].renderer.material.color.b, alphaFadeIn);
			cubeIcon.renderer.material.color = new Color(menuItem[i].renderer.material.color.r, menuItem[i].renderer.material.color.g, menuItem[i].renderer.material.color.b, 1 - alphaFadeIn);
			if(alphaFadeIn <= 0){
				optionMenuMode = StateOption.SCREENFADEIN;
			}
		}
	}
	
	void UpdateScreenFadeIn(){
		
	}
	
	
	
}
