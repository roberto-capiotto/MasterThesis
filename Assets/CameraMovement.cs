using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	float camSize;
	GameObject rocket;

	void Start () {
		rocket = GameObject.Find ("Rocket");
		camSize = Camera.mainCamera.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = new Vector3 (rocket.transform.position.x,rocket.transform.position.y,-10);
	}
}
