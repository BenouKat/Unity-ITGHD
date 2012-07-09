using UnityEngine;
using System.Collections;

public class CreateChart : MonoBehaviour {
	
	public GameObject arrow;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void createTheChart(Song s){
		
		var ypos = 1;
		
		foreach(var mesure in s.stepchart){
		
			switch(mesure.Count){
				case 4:
					foreach(var beat in mesure){
						char[] note = beat.Trim().ToCharArray();
						for(int i =0;i<4; i++){
							if(note[i] != 0){
								var theArrow = (GameObject) Instantiate(arrow, new Vector3(i*2, ypos, 0f), arrow.transform.rotation);
								theArrow.renderer.material.color = new Color(0f, 0f, 1f, 1f);
							}
							ypos += 1;
						}
						
					}
				break;
			}
			
			
		}
	}
}
