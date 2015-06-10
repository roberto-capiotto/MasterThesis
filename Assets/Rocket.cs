using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

	// Use this for initialization
	bool collided=false;
	//float acceleration=80.0f;
	public int people=0;

	void Start () {
		//this.rigidbody.AddForce(acceleration,0f,0f);
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter (Collision collision) {
		collided=true;	
	}

	void OnCollisionExit(Collision collision){
		collided=false;
	}

	public void SavePeople(int num){
		people+=num;
	}
}
