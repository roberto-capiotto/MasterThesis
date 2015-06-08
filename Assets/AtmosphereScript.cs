using UnityEngine;
using System.Collections;

public class AtmosphereScript : MonoBehaviour {

	float gravity;
	float acceleration=180f;
	GameObject rocket;

	// Use this for initialization
	void Start () {
		rocket=GameObject.Find("Rocket");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter (Collision gravityCollision)
	{
		print("enter");
	}

	void OnCollisionStay (Collision collider) {

			gravity=acceleration*(this.transform.localScale.x-1)*
				(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
					(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
		print("localscale: "+this.transform.localScale.x+" gravity: "+gravity);
			rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity);
	}

	void OnCollisionExit (Collision gravityCollision)
	{
		print("exit");
	}
}
