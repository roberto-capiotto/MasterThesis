using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	float camSize;
	GameObject rocket;
	Vector3 position;

	// TODO: define how to move in the level
	// we have to define boundaries?
	// especially up and down
	// eventually left
	void Start () {
		rocket = GameObject.Find ("Rocket");
		camSize = Camera.mainCamera.orthographicSize;
		position = new Vector3(0,0,0);
	}

	// TODO: soft move of camera
	// now is strongly switched
	void Update () {
		if(Mathf.Abs(rocket.transform.position.x-position.x)>camSize || Mathf.Abs(rocket.transform.position.y-position.y)>camSize){
			position=new Vector3(rocket.transform.position.x,rocket.transform.position.y,-10);
			this.transform.position = position;
		}
	}
}
