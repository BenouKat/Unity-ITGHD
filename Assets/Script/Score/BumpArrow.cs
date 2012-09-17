using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BumpArrow : MonoBehaviour {
	
	
	public List<GameObject> arrowMatList;
	
	private List<Material> MatList;
	private float time;
	public float timeclign;
	public float speeddecreaseclign;
	public float limit;
	// Use this for initialization
	void Start () {
		MatList = new List<Material>();
		foreach(var ar in arrowMatList){
			MatList.Add(ar.renderer.material);	
		}
	}
	
	
	void Update(){
		if(time > timeclign){
			foreach(var mat in MatList){
				mat.color = new Color(1f, 1f, 1f, 1f);	
			}
			time = 0;
		}else if(MatList.First().color.r > limit){
			foreach(var mat in MatList){
				mat.color = new Color(mat.color.r - Time.deltaTime/speeddecreaseclign, mat.color.g - Time.deltaTime/speeddecreaseclign, mat.color.b - Time.deltaTime/speeddecreaseclign, 1f);	
			}
		}
		time += Time.deltaTime;
			
	}
	
}

