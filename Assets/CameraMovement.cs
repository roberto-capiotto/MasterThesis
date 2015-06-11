using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	float camSize;
	GameObject rocket;
	Vector3 position;
	Rocket rocketManager;

	// DONE: define how to move in the level
	// we have to define boundaries?
	// especially up and down
	// eventually left
	void Start () {
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		camSize = Camera.mainCamera.orthographicSize;
		position = new Vector3(0,0,0);
	}
	
	void Update () {
		// upper bound and lower bound
		if(Mathf.Abs(rocket.transform.position.y-position.y)>camSize){
			rocketManager.SetInitialPosition();
			this.transform.position = new Vector3(rocketManager.GetInitialPosition().x,rocketManager.GetInitialPosition().y,-10);
		}
		// left bound
		// I suppose the level will be totally on right side
		// It works because the starting point is in (0,0,0)
		// if it changes, also this control will be changed
		if(rocket.transform.position.x<-2*camSize){
			rocketManager.SetInitialPosition();
		}
		// TODO: soft move of camera
		// now is strongly switched
		if(Mathf.Abs(rocket.transform.position.x-position.x)>camSize){
			position=new Vector3(rocket.transform.position.x,rocket.transform.position.y,-10);
			this.transform.position = position;
		}
	}
}
