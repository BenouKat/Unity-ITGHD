using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OpeningLANScene : MonoBehaviour {
	
	public GameObject ring;
	public GameObject[] optionCube;
	private int optionSelected;
	
	public Rect posTitleOption;
	public float ratioNotSelected;
	public Rect posBack;
	public Rect posForw;
	public Vector2 diffPosArrowsRound;
	public Rect posLabelInfoRound;
	public Rect posLabelRound;
	
	public Rect infoOption;
	
	private float alphaClign;
	
	private bool activeBack;
	private bool activeForw;
	
	public Dictionary<string, Texture2D> tex;
	
	// Use this for initialization
	void Start () {
		tex = new Dictionary<string, Texture2D>();
		tex.Add("option0", (Texture2D) Resources.Load("FFA"));
		tex.Add("option1", (Texture2D) Resources.Load("ScoreTournament"));
		tex.Add("option2", (Texture2D) Resources.Load("PointTournament"));
		tex.Add("option3", (Texture2D) Resources.Load("Elimination"));
		
		optionSelected = 0;
		alphaClign = 1f;
	}
	
	
	void OnGUI()
	{
		for(int i=0; i<optionCube.Length; i++)
		{
			var pos2D = Camera.main.WorldToScreenPoint(optionCube[i].transform.position);
			if(i == optionSelected)
			{
				GUI.color = new Color(1f, 1f, 1f, alphaClign);
				GUI.DrawTexture(new Rect(pos2D.x + (posTitleOption.x*Screen.width), pos2D.y + (posTitleOption.y*Screen.height), posTitleOption.width*Screen.width, posTitleOption.height*Screen.height), tex["option" + i]);
			}else
			{
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				GUI.DrawTexture(new Rect(pos2D.x + (posTitleOption.x*Screen.width), pos2D.y + (posTitleOption.y*Screen.height), posTitleOption.width*Screen.width*ratioNotSelected, posTitleOption.height*Screen.height*ratioNotSelected), tex["option" + i]);
			}
			
		}
		
		if(GUI.Button(new Rect(posBack.x*Screen.width, posBack.y*Screen.height, posBack.width*Screen.width, posBack.height*Screen.height), "buttonBack") && !activeBack && !activeForw){
			if(optionSelected > 0)
			{
				optionSelected--;
				activeBack = true;
			}
		}
		
		if(GUI.Button(new Rect(posForw.x*Screen.width, posForw.y*Screen.height, posForw.width*Screen.width, posForw.height*Screen.height), "buttonForw") && !activeBack && !activeForw){
			if(optionSelected < optionCube.Length - 1)
			{
				optionSelected++;
				activeForw = true;
			}
		}
		
		GUI.Label(new Rect(infoOption.x*Screen.width, infoOption.y*Screen.height, infoOption.width*Screen.width, infoOption.height*infoOption.height), TextManager.Instance.texts["LAN"]["MENUOption" + optionSelected]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
}
