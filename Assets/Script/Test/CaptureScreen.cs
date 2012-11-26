using UnityEngine;
using System.Collections;

public class CaptureScreen : MonoBehaviour {
	
	private float time;
	
	// Use this for initialization
	void Start () {
		time = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(time >= 5f){
			Application.CaptureScreenshot("Screen" + Time.time + ".png");
			time = 0f;
		}else{
			time += Time.deltaTime;	
		}
	}
}
