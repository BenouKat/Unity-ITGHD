using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ManageGameObject : MonoBehaviour {

	public Dictionary<float, GameObject> listarrow;
	
	public Transform cameraTransform;
	
	public float zoneAppear = 10f;
// Use this for initialization
	void Awake () {
		listarrow = new Dictionary<float, GameObject>();
	}
	
	
	void FixedUpdate() {
		if(listarrow.Any()){
			var next = listarrow.First();
			if(next.Key > (cameraTransform.position.y - zoneAppear)){
				activeGameObject(next.Value);
				listarrow.Remove(next.Key);
			}
			
		}
	}
	
	
	public void DoTheStartSort(){
		listarrow.OrderByDescending(c => c.Key);
		for(int i=0; i<listarrow.Count; i++){
			if(listarrow.ElementAt(i).Key > (cameraTransform.position.y - zoneAppear)){
				activeGameObject(listarrow.ElementAt(i).Value);
				listarrow.Remove(listarrow.ElementAt(i).Key);
				i--;
			}
		}
	
	}
	
	
	public void Add(GameObject go){
		var ypos = go.transform.position.y;
		var counter = 0;
		while(listarrow.ContainsKey(ypos) && counter < 10){
			ypos -= 0.01f;
			counter++;
		}
		if(!listarrow.ContainsKey(ypos)){
			supressGameObject(go);
			listarrow.Add(ypos, go);
		}
	}
	
	public void supressGameObject(GameObject go)
	{
		if(go.GetComponent("ArrowScript") != null) go.GetComponent<ArrowScript>().enabled = false;
		if(go.transform.GetChildCount() > 0)
		{
			for(int i=0; i < go.transform.GetChildCount(); i++)
			{
				go.transform.GetChild(i).renderer.enabled = false;	
			}
		}
		go.transform.renderer.enabled = false;
	}
	
	public void activeGameObject(GameObject go)
	{
		if(go.GetComponent("ArrowScript") != null) go.GetComponent<ArrowScript>().enabled = true;
		if(go.transform.GetChildCount() > 0)
		{
			for(int i=0; i < go.transform.GetChildCount(); i++)
			{
				go.transform.GetChild(i).renderer.enabled = true;	
			}
		}
		go.transform.renderer.enabled = true;
	}

}