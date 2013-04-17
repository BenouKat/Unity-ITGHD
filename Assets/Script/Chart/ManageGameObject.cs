using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ManageGameObject : MonoBehaviour {
	
	private Dictionary<float, GameObject> listArrowTemp;
	private GameObject[] listArrow;
	private float[] listPos;
	private int poolIndex;
	private int totalCount;
	
	public Transform cameraTransform;
	
	public float zoneAppear = 10f;

	
	
	// Use this for initialization
	void Awake () {
		listArrowTemp = new Dictionary<float, GameObject>();
	}
	
	
	//Trouver un autre systeme ?
	void FixedUpdate() {
		while(poolIndex < totalCount && listPos[poolIndex] > (cameraTransform.position.y - zoneAppear) && poolIndex < totalCount)
		{
			activeGameObject(listArrow[poolIndex]);
			poolIndex++;
		}
	}
	
	
	public void DoTheStartSort(){
		listArrowTemp.OrderByDescending(c => c.Key);
		for(int i=0; i<listArrowTemp.Count; i++){
			if(listArrowTemp.ElementAt(i).Key > (cameraTransform.position.y - zoneAppear)){
				activeGameObject(listArrowTemp.ElementAt(i).Value);
				listArrowTemp.Remove(listArrowTemp.ElementAt(i).Key);
				i--;
			}
		}
		
		listArrow = new GameObject[listArrowTemp.Count];
		listPos = new float[listArrowTemp.Count];
		
		for(int i=0; i<listArrowTemp.Count; i++)
		{
			listArrow[i] = listArrowTemp.ElementAt(i).Value;
			listPos[i] = listArrowTemp.ElementAt(i).Key;
		}
		
		totalCount = listArrowTemp.Count;
		listArrowTemp.Clear();
		
		poolIndex = 0;
	}
	
	
	public void Add(GameObject go){
		var ypos = go.transform.position.y;
		var counter = 0;
		while(listArrowTemp.ContainsKey(ypos) && counter < 10){
			ypos -= 0.01f;
			counter++;
		}
		if(!listArrowTemp.ContainsKey(ypos)){
			supressGameObject(go);
			listArrowTemp.Add(ypos, go);
		}
	}
	
	public void supressGameObject(GameObject go)
	{
		if(go.GetComponent("ArrowScript") != null) go.GetComponent<ArrowScript>().enabled = false;
		if(go.transform.GetChildCount() > 0)
		{
			if(go.transform.renderer != null)
			{
				go.transform.renderer.enabled = false;	
			}
			for(int i=0; i < go.transform.GetChildCount(); i++)
			{
				go.transform.GetChild(i).renderer.enabled = false;	
			}
		}else
		{
			go.transform.renderer.enabled = false;
		}
		
	}
	
	public void activeGameObject(GameObject go)
	{
		if(go.GetComponent("ArrowScript") != null) go.GetComponent<ArrowScript>().enabled = true;
		if(go.transform.GetChildCount() > 0)
		{
			if(go.transform.renderer != null)
			{
				go.transform.renderer.enabled = false;	
			}
			for(int i=0; i < go.transform.GetChildCount(); i++)
			{
				go.transform.GetChild(i).renderer.enabled = true;	
			}
		}else
		{
			go.transform.renderer.enabled = true;
		}
		
	}

}