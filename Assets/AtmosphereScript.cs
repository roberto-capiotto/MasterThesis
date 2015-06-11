using UnityEngine;
using System.Collections;

public class AtmosphereScript : MonoBehaviour {

	float gravity;
	float acceleration=180f;
	GameObject rocket;
	GameObject swing;
	Swing swingManager;
	SphereCollider orbitCollider;

	// Use this for initialization
	void Start () {
		rocket=GameObject.Find("Rocket");
		swing=GameObject.Find("Swing");
		swingManager = swing.GetComponent ("Swing") as Swing;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider gravityCollision)
	{
		print(gravityCollision.transform.position.x);
		orbitCollider = transform.GetComponent<SphereCollider>();
		swingManager.SetRadius(orbitCollider.radius);
	}

	void OnTriggerStay (Collider collider) {

			gravity=acceleration*(this.transform.localScale.x-1)*
				(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
					(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
		print("localscale: "+this.transform.localScale.x+" gravity: "+gravity);
			rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity);
	}

	void OnTriggerExit (Collider gravityCollision)
	{
		print("exit");
	}
}
