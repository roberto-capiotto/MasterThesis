using UnityEngine;
using System.Collections;

public class PCG : MonoBehaviour {

	public Transform Planet;
	public Transform Rocket;
	bool creation=false;
	GameObject rocket,planet;

	// Use this for initialization
	void Start () {
		rocket = Instantiate(Resources.Load("Rocket")) as GameObject;
		rocket.name="Rocket";
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		//rocket=Instantiate(Rocket, new Vector3(3, 0, 0), Quaternion.identity) as GameObject;
		//rocket.name = rocket.name.Replace("(Clone)", ""); 
		//Instantiate(Planet, new Vector3(0, 0, 0), Quaternion.identity);
		//Planet.name="Planet";
		creation=true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool GetCreation(){
		return creation;
	}
}
