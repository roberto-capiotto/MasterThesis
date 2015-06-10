using UnityEngine;
using System.Collections;

public class AsteroidForce : MonoBehaviour {
	
	Vector3 myPos;
	
	void Start () {
		// this instruction enable you having asteroid of different dimension
		// i don't like to use it
		//this.transform.localScale=new Vector3(Random.Range(0.6f,0.8f),Random.Range(0.4f,0.6f),1);

		myPos = this.transform.position;
		this.rigidbody.freezeRotation=true;
		this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
		this.transform.rigidbody.constraints = RigidbodyConstraints.FreezeRotationY;
		this.transform.rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
		this.rigidbody.freezeRotation=true;
		
	}
	
	void Update(){
		this.transform.position = myPos;
	}
	
	void OnCollisionStay(Collision collision)
	{
		//	TODO: set the correct force and define behaviour
		collision.rigidbody.AddForce(new Vector3(Random.Range(-20,20),Random.Range(-20,20),0));
	}
}