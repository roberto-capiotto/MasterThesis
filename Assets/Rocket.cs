using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

	// Use this for initialization
	GameObject planet;
	bool collided=false;
	float acceleration=80.0f;

	void Start () {
		//this.rigidbody.AddForce(acceleration,0f,0f);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter (Collider collider) {
		collided=true;	
		planet=collider.gameObject;
	}

	void OnTriggerExit(Collider collider){
		collided=false;
	}
}
