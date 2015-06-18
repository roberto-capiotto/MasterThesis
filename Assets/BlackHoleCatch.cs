using UnityEngine;
using System.Collections;

public class BlackHoleCatch : MonoBehaviour {

	float gravity;
	float acceleration=180f;
	GameObject rocket;

	void Start () {
		rocket=GameObject.Find("Rocket");
	}

	void FixedUpdate () {
	}
	
	void OnTriggerEnter (Collider gravityCollision){
	}
	
	void OnTriggerStay (Collider collider) {
		
		gravity=acceleration*(this.transform.localScale.x-1)*
			(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
				(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
		print("localscale: "+this.transform.localScale.x+" gravity: "+gravity);
		rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity/50);
	}
	
	void OnTriggerExit (Collider gravityCollision){
	}
}
