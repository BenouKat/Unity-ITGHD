using UnityEngine;
using System.Collections;

public class LifeBar : MonoBehaviour {

	
	public GameObject[] lifebar;
	public GameObject Ring;
	public Material matLifeBar;
	private Material[] matCube;
	
	public float speedBrillance;
	public float maxBrillance;
	public float lowBrillance;
	public Vector3[] baseScale;
	
	public float speedClignDanger;
	
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
	
	private bool turnActivated;
	
	public float speedTurn;
	public float speedTurnMax;
	
	private float numberStep;
	
	public UISprite lifeHUD;
	public UILabel lifeInfo;
	
	public Color colorMax;
	public Color colorNormal;
	public Color colorOutlineDanger;
	public Color colorDanger;
	private Color white = new Color(1f, 1f, 1f, 1f);
	
	//Pool variable
	private Color poolColor = new Color(0f, 0f, 0f, 1f);
	private int poolIndex = 0;
	private Vector3 poolVector = new Vector3(0f, 0f, 0f);
	private float lerpDanger;
	private float oldRealLife;
	
	void Start () {
		oldRealLife = -50f;
		matCube = new Material[lifebar.Length];
		lerpDanger = 0f;
		realLife = 40f;
		objectivLife = 50f;
		numberStep = (75f/(float)(lifebar.Length - 1));
		var index = realLife <= 25 ? 0 : ((int)((realLife - 25f)/numberStep) + 1);
		
		for(int i=0; i<lifebar.Length; i++){
			matCube[i] = lifebar[i].renderer.material;
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
				lifeInfo.color = colorDanger;
				lifeInfo.effectColor = colorOutlineDanger;
				lifebar[0].transform.localScale = baseScale[0]*(realLife/25f);
				lifebar[1].transform.localScale = baseScale[1]*0f;
			}else if(realLife < 100f){
				lifeInfo.color = white;
				lifeInfo.effectColor = colorNormal;
				poolIndex = (int)((realLife - 25f)/numberStep) + 1;
				lifebar[poolIndex - 1].transform.localScale = baseScale[poolIndex - 1];
				lifebar[poolIndex].transform.localScale = baseScale[poolIndex]*(((realLife - 25f)%numberStep)/numberStep);
				if(poolIndex != lifebar.Length - 1) lifebar[poolIndex + 1].transform.localScale = baseScale[poolIndex + 1]*0f;
			}else{
				lifeInfo.color = white;
				lifeInfo.effectColor = colorMax;
				lifebar[lifebar.Length - 1].transform.localScale = baseScale[lifebar.Length - 1];
			}
			
			realLife = Mathf.Lerp(realLife, objectivLife, thelerp);
			if(Mathf.Abs(realLife - objectivLife) < limit) realLife = objectivLife;
			
			if(realLife != oldRealLife)
			{
				lifeInfo.text = realLife < 100f ? (realLife - 0.5f).ToString("00") + "%" : "SYNC";
				oldRealLife = realLife;
			}
			
				
		}
		
		if(realLife >= 100f)
		{
			if(Ring.transform.eulerAngles.x < 60f)
			{
				poolVector.x = 60f;
				poolVector.y = Ring.transform.eulerAngles.y;
				poolVector.z = Ring.transform.eulerAngles.z;
				Ring.transform.eulerAngles = Vector3.Lerp(Ring.transform.eulerAngles, poolVector, speedTurnMax*Time.deltaTime);
				if(Ring.transform.eulerAngles.x >= 59.99f)
				{
					turnActivated = true;
					Ring.transform.eulerAngles = poolVector;
				}
			}else if(!turnActivated){
				turnActivated = true;	
			}
			Ring.transform.Rotate(0f, 0f, speedTurn*Time.deltaTime);
		}else if(turnActivated)
		{
			poolVector.x = 0f;
			poolVector.y = Ring.transform.eulerAngles.y;
			poolVector.z = 0f;
			Ring.transform.eulerAngles = Vector3.Lerp(Ring.transform.eulerAngles, poolVector, speedTurnMax*Time.deltaTime);
			if(Vector3.Distance(Ring.transform.eulerAngles, poolVector) < 0.01f)
			{
				turnActivated = false;
				Ring.transform.eulerAngles = poolVector;
			}
		}
		
		
		if(realLife <= 25f)
		{
			lifeHUD.color = Color.Lerp(white, colorDanger, Mathf.PingPong(lerpDanger, 1f));
			lerpDanger += speedClignDanger*Time.deltaTime;
		}else if(lerpDanger != 0f)
		{
			lifeHUD.color = white;
			lerpDanger = 0f;
		}
		
		matCube[0].SetFloat("_Shininess", lowBrillance + Mathf.PingPong((Time.time*speedBrillance), maxBrillance - lowBrillance) + 0.001f);
		
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
