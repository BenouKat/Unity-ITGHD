using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
	
	private delegate void updateMethod();
	private updateMethod updat;
	public float speedUp;
	private float trueSpeedUp;
	
	
	public float limiteslowUp;
	public float limitesUp;
	
	public float addUp;
	
	
	public float speedTang;
	private float trueSpeedTang;
	public float limiteslowTangRight;
	public float limiteslowTangLeft;
	public float addTang;
	public float addStart;
	private float sign;
	
	private bool firststart = true;
	
	private float time;
	private bool enterPushed;
	
	public float speedFadeMenu;
	
	
	
	private Dictionary<string, GameObject> goToBack;
	private string forbiddenTouch;
	public float speedSlide;
	// Use this for initialization
	void Start () {
		//LoadManager.Instance.Loading();
		this.updat = UpdateUp;
		sign = -1f;
		Camera.main.transform.eulerAngles = new Vector3(66f, 32.5f, 0f);
		trueSpeedUp = speedUp;
		trueSpeedTang = 200;
		time = 0f;
		enterPushed = false;
		goToBack = new Dictionary<string, GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		this.updat();
		if(time >= 5f && !enterPushed){
			if(Input.GetKeyDown(KeyCode.Return)){
				this.updat = UpdateMainMenu;
				enterPushed = true;
			}
		}else{
			time += Time.deltaTime;	
		}
	}
	
	void UpdateUp () {
		Camera.main.transform.Rotate(new Vector3(-51f*(Time.deltaTime/trueSpeedUp), 0f, 0f));
		
		if(Camera.main.transform.eulerAngles.x <= limitesUp){
			Camera.main.transform.eulerAngles = new Vector3(15f, 32.5f, 0f);
			updat = UpdateTang;
		}
		
		if(Camera.main.transform.eulerAngles.x <= limiteslowUp){
			trueSpeedUp += Time.deltaTime*addUp;
		}
	}
	
	void UpdateTang () {
		if(firststart){
			trueSpeedTang -= Time.deltaTime*addStart;
			if(trueSpeedTang <= speedTang){
				firststart = false;	
			}
		}
		
		Camera.main.transform.Rotate(new Vector3(0f, sign*51f*(Time.deltaTime/trueSpeedTang), 0f));
		
		if(Camera.main.transform.eulerAngles.y <= 15 && sign == -1f ||
			Camera.main.transform.eulerAngles.y >= 45 && sign == 1f ){
			sign *= -1f;
		}
		
		if(Camera.main.transform.eulerAngles.y <= limiteslowTangLeft){
			trueSpeedTang -= sign*Time.deltaTime*addTang;
		}else if(Camera.main.transform.eulerAngles.y >= limiteslowTangRight){
			trueSpeedTang += sign*Time.deltaTime*addTang;
		}else if(!firststart){
				trueSpeedTang = speedTang;
		}
	}
	
	
	void UpdateMainMenu(){
		
		
		if(Camera.main.transform.position.x <= 0.001f){
			Camera.main.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			Camera.main.transform.position = new Vector3(0f, -35f, -30f);
		}else{
			Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, Quaternion.LookRotation(new Vector3(0f, 0f, 1f)), Time.deltaTime/speedFadeMenu);
			Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0f, -35f, -30f), Time.deltaTime/speedFadeMenu);
		}
		
		if(Camera.main.transform.position.x <= 1f){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
			RaycastHit hit;
					
			if(Physics.Raycast(ray, out hit))
			{
				var theGo = hit.transform.gameObject;
				if(theGo != null && theGo.tag == "MenuItem"){
					if(theGo.name.Contains("Cube")){
						theGo.transform.localPosition = Vector3.Lerp(theGo.transform.localPosition, new Vector3(-28f, theGo.transform.localPosition.y, theGo.transform.localPosition.z), Time.deltaTime/speedSlide);
						if(!goToBack.ContainsKey(theGo.transform.GetChild(0).name)) goToBack.Add(theGo.transform.GetChild(0).name, theGo);
						forbiddenTouch = theGo.transform.GetChild(0).name;
					}else{
						theGo.transform.parent.localPosition = Vector3.Lerp(theGo.transform.parent.localPosition, new Vector3(-28f, theGo.transform.parent.localPosition.y, theGo.transform.parent.localPosition.z), Time.deltaTime/speedSlide);
						if(!goToBack.ContainsKey(theGo.name)) goToBack.Add(theGo.name, theGo.transform.parent.gameObject);
						forbiddenTouch = theGo.name;
					}
				}else{
					forbiddenTouch = "";
				}
			}else{
				forbiddenTouch = "";
			}	
			var toDelete = new List<string>();
			foreach(var el in goToBack){
				if(el.Key != forbiddenTouch){
					el.Value.transform.localPosition = Vector3.Lerp(el.Value.transform.localPosition, new Vector3(-20f, el.Value.transform.localPosition.y, el.Value.transform.localPosition.z), Time.deltaTime/speedSlide);
					if(el.Value.transform.localPosition.x >= -20.01f){
						el.Value.transform.localPosition = new Vector3(-20f, el.Value.transform.localPosition.y, el.Value.transform.localPosition.z);
						toDelete.Add(el.Key);
					}
				}
			}
			foreach(var del in toDelete){
				goToBack.Remove(del);	
			}
		}
	}
}
