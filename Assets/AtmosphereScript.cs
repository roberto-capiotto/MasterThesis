using UnityEngine;
using System.Collections;

public class AtmosphereScript : MonoBehaviour {

	float gravity;
	float acceleration=180f;
	GameObject rocket;
	GameObject swing;
	Swing swingManager;
	SphereCollider orbitCollider;
	
	void Start () {
		rocket=GameObject.Find("Rocket");
		swing=GameObject.Find("Swing");
		swingManager = swing.GetComponent ("Swing") as Swing;
	}

	void FixedUpdate () {
	
	}

	void OnTriggerEnter (Collider gravityCollision)
	{
		print(gravityCollision.transform.position.x);
		orbitCollider = transform.GetComponent<SphereCollider>();
		swingManager.SetRadius(orbitCollider.radius*orbitCollider.transform.localScale.x/swing.transform.localScale.x);
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
