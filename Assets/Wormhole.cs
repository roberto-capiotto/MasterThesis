using UnityEngine;
using System.Collections;

public class Wormhole : MonoBehaviour {

	public GameObject exit;
	GameObject rocket;
	float acceleration=180f;
	bool collision=false;

	void Start () {
		rocket = GameObject.Find ("Rocket");
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.black);
	}

	void FixedUpdate () {
		if(collision){
			// The rocket loses all its characteristics of direction and velocity
			// the wormhole shots it right with an its own velocity
			rocket.rigidbody.velocity=new Vector3(0,0,0);
			rocket.transform.position=new Vector3 (exit.transform.position.x+this.transform.localScale.x/2+rocket.transform.localScale.x/2+0.5f,
			                                         exit.transform.position.y,
			                                         exit.transform.position.z);
			rocket.rigidbody.AddForce(acceleration,0f,0f);
			collision=false;
		}
	}
	
	void OnCollisionEnter (Collision collider) {
		collision=true;
	}
}
