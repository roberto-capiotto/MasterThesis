using UnityEngine;
using System.Collections;

public class AtmosphereScript : MonoBehaviour {

	float gravity;
	float acceleration=180f;
	GameObject rocket;
	
	void Start () {
		rocket=GameObject.Find("Rocket");
	}

	void FixedUpdate () {
	}

	void OnTriggerEnter (Collider gravityCollision)
	{
		print(gravityCollision.transform.position.x);
	}

	void OnTriggerStay (Collider collider) {

			gravity=acceleration*(this.transform.localScale.x-1)*
				(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
					(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
			rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*3);
	}

	void OnTriggerExit (Collider gravityCollision)
	{
		print("exit");
	}
}
