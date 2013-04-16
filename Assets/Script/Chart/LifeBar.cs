using UnityEngine;
using System.Collections;

public class LifeBar : MonoBehaviour {

	
	public GameObject[] lifebar;
	public Material matLifeBar;
	private Material[] matCube;
	private float[] decals;
	
	public float speedBrillance;
	public float maxBrillance;
	public float lowBrillance;
	public Vector3[] baseScale;
	
	public ParticleSystem psMaxLife;
	public ParticleSystem psLifeUp;
	public ParticleSystem psLowLife;
	
	private float realLife;
	private float objectivLife;
	private float thelostlife;
	public float thelerp;
	public float limit;
	
	private bool lifeUpPlayin;
	private bool lifeMaxPlayin;
	private bool lifeLowPlayin;
	
	private float numberStep;
	
	//Pool variable
	private Color poolColor = new Color(0f, 0f, 0f, 1f);
	private int poolIndex = 0;
	
	void Start () {
		matCube = new Material[lifebar.Length];
		decals = new float[lifebar.Length];
		
		realLife = 40f;
		objectivLife = 50f;
		numberStep = (75f/(float)(lifebar.Length - 1));
		var index = realLife <= 25 ? 0 : ((int)((realLife - 25f)/numberStep) + 1);
		
		for(int i=0; i<lifebar.Length; i++){
			matCube[i] = lifebar[i].renderer.material;
			decals[i] = maxBrillance - ((float)i/(float)lifebar.Length - 1f)*maxBrillance;
			if(i < index){
				lifebar[i].transform.localScale = baseScale[i];
			}else if(i > index){
				lifebar[i].transform.localScale = baseScale[i]*0f;
			}else if(index == i && i == 0){
				lifebar[index].transform.localScale = baseScale[index]*(realLife/25f);
			}else{
				lifebar[index].transform.localScale = baseScale[index]*(((realLife - 25f)%numberStep)/numberStep);
			}
		}
		
		lifeUpPlayin = false;
		lifeMaxPlayin = false;
		lifeLowPlayin = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(realLife != objectivLife){
			poolColor.r = 0f;
			poolColor.g = 0f;
			poolColor.b = 0f;
			if(realLife < 50f){
				poolColor.r = 1f;
				poolColor.g = realLife <= 25f ? 0f : (realLife - 25f)/25f;
				poolColor.b = 0f;
			}else
			if(realLife >= 50f && realLife < 75f){
				poolColor.r = 1 - ((realLife - 50f)/25f);
				poolColor.g = 1f;
				poolColor.b = 0f;
			}else
			
			if(realLife >= 75f){
				poolColor.r = 0f;
				poolColor.g = 1f;
				poolColor.b = (realLife - 75f)/26f;
			}
			
			for(int i=0; i<lifebar.Length; i++){
				matCube[i].color = poolColor;
			}
			//matLifeBar.color = new Color(r,g,b, 1f);
			
			if(realLife <= 25f){
				lifebar[0].transform.localScale = baseScale[0]*(realLife/25f);
				lifebar[1].transform.localScale = baseScale[1]*0f;
			}else if(realLife < 100f){
				poolIndex = (int)((realLife - 25f)/numberStep) + 1;
				lifebar[poolIndex - 1].transform.localScale = baseScale[poolIndex - 1];
				lifebar[poolIndex].transform.localScale = baseScale[poolIndex]*(((realLife - 25f)%numberStep)/numberStep);
				if(poolIndex != lifebar.Length - 1) lifebar[poolIndex + 1].transform.localScale = baseScale[poolIndex + 1]*0f;
			}else{
				lifebar[lifebar.Length - 1].transform.localScale = baseScale[lifebar.Length - 1];
			}
			
			realLife = Mathf.Lerp(realLife, objectivLife, thelerp);
			if(Mathf.Abs(realLife - objectivLife) < limit) realLife = objectivLife;
		}
		
		for(int i=0; i<matCube.Length; i++){
			matCube[i].SetFloat("_Shininess", lowBrillance + Mathf.PingPong((Time.time*speedBrillance) + decals[i], maxBrillance - lowBrillance) + 0.001f);
		}
	}
	
	
	public void ChangeBar(float newlife){
		objectivLife = newlife;
		if(newlife >= 100f && !lifeMaxPlayin){
			psLifeUp.Stop(); 
			lifeUpPlayin = false;
			if(!psMaxLife.gameObject.active) psMaxLife.gameObject.active = true;
			psMaxLife.Play();
			lifeMaxPlayin = true;
		}else if(newlife > realLife && newlife > 25f && !lifeUpPlayin && !lifeMaxPlayin && newlife > (thelostlife + 10f)){
			if(!psLifeUp.gameObject.active) psLifeUp.gameObject.active = true;
			psLifeUp.Play();
			lifeUpPlayin = true;
		}else if(newlife < realLife){
			psLifeUp.Stop();
			lifeUpPlayin = false;
			psMaxLife.Stop();
			lifeMaxPlayin = false;
			thelostlife = newlife;
		}
		
		if(newlife <= 25f && !lifeLowPlayin){
			if(!psLowLife.gameObject.active) psLowLife.gameObject.active = true;
			psLowLife.Play();
			lifeLowPlayin = true;
		}else if(newlife > 25f && lifeLowPlayin){
			psLowLife.Stop();
			lifeLowPlayin = false;
		}
		
		
	}
	
	
	
	
	
	
	
}
