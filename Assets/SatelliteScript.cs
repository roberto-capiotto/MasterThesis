﻿using UnityEngine;
using System.Collections;

public class SatelliteScript : MonoBehaviour {
	
	GameObject rocket;
	Rocket rocketManager;
	Vector3 myPos;
	bool collision=false;
	
	void Start () {
		// each satellite may have different dimensions
		// this.transform.localScale=new Vector3(Random.Range(0.4f,0.6f),Random.Range(0.4f,0.6f),1);
		
		myPos = this.transform.position;
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.green);
		
	}
	
	void FixedUpdate(){
		this.transform.position = myPos;
		if(collision){
			//	rocket die
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			collision=false;
		}
	}
	
	void OnCollisionEnter()
	{
		collision=true;
	}
}