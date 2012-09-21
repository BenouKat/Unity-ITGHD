using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreScript : MonoBehaviour {
	
	
	public float speed = 0.05f;
	public float speedSphere = 0.01f;
	public GameObject center;
	private List<GameObject> sphere;
	
	
	//Values :
	//speed : 0.05
	//speedsphere : 0.01
	
	//Bronze
	//sphere : -1.5/0/0	
	
	//Silver
	//sphere : 1.5/1.5/0 et -1.5/1.5/0
	
	//Gold
	//sphere : 2/1.5/0	et 2/-1.5/0 et 0/2.5/0
	
	//Quad
	//sphere : -1.5/-2/0 et 0/2/0 et 3/-2/0 et -3/-2/0
	void Start(){
		sphere = new List<GameObject>();
		for(int i=0;i<gameObject.transform.GetChildCount();i++){
			if(gameObject.transform.GetChild(i).name.Contains("Sphere")){
				sphere.Add(	gameObject.transform.GetChild(i).gameObject);
			}
		}
	}
	
	
	void Update(){
		
		switch(sphere.Count){
			case 1:	
				sphere.ElementAt(0).transform.RotateAround(center.transform.position, center.transform.up, (Time.deltaTime/speedSphere));
				break;
			case 2:	
				sphere.ElementAt(0).transform.RotateAround(center.transform.position, Vector3.Lerp(center.transform.up,center.transform.right,0.5f).normalized, (Time.deltaTime/speedSphere));
				sphere.ElementAt(1).transform.RotateAround(center.transform.position, Vector3.Lerp(center.transform.up,-center.transform.right,0.5f).normalized, (Time.deltaTime/speedSphere));
				break;
			case 3:	
				sphere.ElementAt(0).transform.RotateAround(center.transform.position, Vector3.Lerp(center.transform.up,center.transform.right,0.5f).normalized, (Time.deltaTime/speedSphere));
				sphere.ElementAt(1).transform.RotateAround(center.transform.position, Vector3.Lerp(center.transform.up,-center.transform.right,0.5f).normalized, (Time.deltaTime/speedSphere));
				sphere.ElementAt(2).transform.RotateAround(center.transform.position, center.transform.right, (Time.deltaTime/speedSphere));
				break;
			case 4:	
				sphere.ElementAt(0).transform.RotateAround(center.transform.position, center.transform.up, (Time.deltaTime/speedSphere));
				sphere.ElementAt(1).transform.RotateAround(center.transform.position, center.transform.right, (Time.deltaTime/speedSphere));
				sphere.ElementAt(2).transform.RotateAround(center.transform.position, Vector3.Lerp(center.transform.up,center.transform.right,0.5f).normalized, (Time.deltaTime/speedSphere));
				sphere.ElementAt(3).transform.RotateAround(center.transform.position, Vector3.Lerp(center.transform.up,-center.transform.right,0.5f).normalized, (Time.deltaTime/speedSphere));
				break;
		}
		
		transform.Rotate(0f, Time.deltaTime/speed, 0f);
		
	}
	
		
	
}

