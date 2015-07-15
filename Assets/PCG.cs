using UnityEngine;
using System.Collections;

public class PCG : MonoBehaviour {
	
	bool creation=false;
	GameObject rocket,planet,cam;
	public int gen=0;
	float x,y;
	Vector3 initialPosition;
	Vector3 beforePosition;
	Vector3 newPosition;
	SphereCollider myCollider;
	
	void Start () {
		// generate Rocket
		rocket = Instantiate(Resources.Load("Rocket")) as GameObject;
		rocket.name="Rocket";
		// generate Planet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		myCollider = planet.transform.GetComponent<SphereCollider>();
		gen++;
		// place Planet
		planet.transform.position=Vector3.zero;
		// place Rocket
		initialPosition=new Vector3
			(planet.transform.position.x ,planet.transform.position.y+(planet.transform.localScale.y/2)+myCollider.radius,0);
		rocket.transform.position=initialPosition;
		beforePosition=Vector3.zero;
		newPosition=planet.transform.position;
		// the rocket and the first planet was generated
		creation=true;
		// enable the movement of the Camera
		cam=GameObject.Find("Main Camera");
		(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).enabled = true;
		GeneratePlanet(newPosition);
	}

	void FixedUpdate () {
	}

	void GeneratePlanet(Vector3 pos){
		beforePosition=pos;
		x=Random.Range(5f+beforePosition.x,8f+beforePosition.x);
		y=Random.Range(-3f,3f);
		newPosition=new Vector3(x,y,0);
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.transform.position= newPosition;
		planet.name="Planet";
		if(Random.Range(-1f,1f)>0)
			(planet.GetComponent("Planet") as Planet).SetRotation(true);
		else
			(planet.GetComponent("Planet") as Planet).SetRotation(false);
		gen++;
		print (gen+" "+x+" "+y+" "+(planet.GetComponent("Planet") as Planet).clockwise);
		if(gen<10){
			GeneratePlanet(newPosition);
		}
	}

	public bool GetCreation(){
		return creation;
	}
}
