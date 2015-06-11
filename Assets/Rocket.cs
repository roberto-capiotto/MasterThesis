﻿using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	
	bool collided=false;
	//float acceleration=80.0f;
	public int people=0;
	Vector3 initialPosition;
	//SphereCollider myCollider;

	// TODO: define initialPosition
	// now fixed
	// then there will be checkpoint or something other
	void Start () {
		//myCollider = transform.GetComponent<SphereCollider>();
		initialPosition=new Vector3(0 ,1.6f,0);
		SetInitialPosition();
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.red);
	}

	void Update () {
	}

	void OnCollisionEnter (Collision collision) {
		collided=true;	
	}

	void OnCollisionExit(Collision collision){
		collided=false;
	}

	public void SavePeople(int num){
		people+=num;
	}

	public void SetInitialPosition(){
		this.rigidbody.velocity = (new Vector3 (0, 0, 0));
		this.transform.position=initialPosition;
	}

	public Vector3 GetInitialPosition(){
		return initialPosition;
	}
}
