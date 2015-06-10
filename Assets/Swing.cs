using UnityEngine;
using System.Collections;

public class Swing : MonoBehaviour {

	GameObject rocket;
	GameObject joint;
	bool collided=false;

	void Start () {
		rocket = GameObject.Find ("Rocket");
	}

	void Update () {
		if(collided){
			//Create HingeJoints
			/*joint = gameObject.AddComponent<HingeJoint> ();
			joint.axis = Vector3.back; /// (0,0,-1)
			joint.anchor = Vector3.zero;
			joint.connectedBody = anchor.rigidbody;
			anchorJoint = anchor.AddComponent<HingeJoint> ();
			anchorJoint.axis = Vector3.back; /// (0,0,-1)
			anchorJoint.anchor = Vector3.zero;*/
		}
	}

	void OnCollisionEnter(Collision collision){
		collided=true;
		// capisci da che parte arrivi e dove devi girare
		// unisci gli oggetti
		// quando sei arrivato spara
	}

	void OnCollisionExit(Collision collision){
		collided = false;
	}
}
