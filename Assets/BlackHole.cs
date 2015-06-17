using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {

	GameObject rocket;
	Rocket rocketManager;

	void Start () {
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.black);
	}

	void OnCollisionEnter(){
		rocketManager.SetInitialPosition();
		rocketManager.FullRefill();
	}
}
