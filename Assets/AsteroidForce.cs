using UnityEngine;
using System.Collections;

public class AsteroidForce : MonoBehaviour {
	
	Vector3 myPos;
	GameObject rocket;
	Rocket rocketManager;
	
	void Start () {
		// this instruction enable you having asteroid of different dimension
		//this.transform.localScale=new Vector3(Random.Range(0.6f,0.8f),Random.Range(0.4f,0.6f),1);

		myPos = this.transform.position;
		this.rigidbody.freezeRotation=true;
		this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
		this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
		this.transform.rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
		this.rigidbody.freezeRotation=true;
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;

		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.green);
		
	}
	
	void Update(){
		this.transform.position = myPos;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		//	rocket die
		rocketManager.SetInitialPosition();
	}
}