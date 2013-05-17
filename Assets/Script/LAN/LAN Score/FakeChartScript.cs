using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FakeChartScript : MonoBehaviour {
	
	
	public List<GameObject> targetArrows;
	
	public float speedInstanciate;
	
	private float time;
	
	public GameObject redCube;
	public GameObject blueCube;
	public GameObject greenCube;
	
	
	// Use this for initialization
	void Start () {
		time = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(time >= 1f)
		{
			int randColor = (int)(Random.value*3.999f);
			int randPos = (int)(Random.value*3.999f);
			if(randColor != 3)
			{
				var theTarget = targetArrows.ElementAt(randPos);
				GameObject theGoInst = (GameObject) Instantiate((randColor  == 0) ? redCube : ((randColor == 1) ? blueCube : greenCube), theTarget.transform.position, theTarget.transform.rotation);
				theGoInst.transform.localPosition = new Vector3(theGoInst.transform.localPosition.x, theGoInst.transform.localPosition.y, -40f);
				theGoInst.transform.FindChild("Destroy Cube Particle").gameObject.active = false;
				theGoInst.transform.FindChild("Explode Cube Particle").gameObject.active = false;
			}
			time = 0f;
		}
		time += speedInstanciate*Time.deltaTime;
	}
}
