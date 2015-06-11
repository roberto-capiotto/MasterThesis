﻿using UnityEngine;
using System.Collections;

public class Wormhole : MonoBehaviour {

	public GameObject exit;
	float acceleration=180f;

	void Start () {
	
	}

	void Update () {
	
	}

	// I suppose that rocket loses all its characteristics of velocity and direction and follow the wormhole rules
	// the wormhole shots right
	void OnCollisionEnter (Collision collider) {
		collider.rigidbody.velocity=new Vector3(0,0,0);
		collider.transform.position=new Vector3 (exit.transform.position.x+this.transform.localScale.x/2+collider.transform.localScale.x/2+0.5f,
		                                         exit.transform.position.y,
		                                         exit.transform.position.z);
		collider.rigidbody.AddForce(acceleration,0f,0f);
	}
}
