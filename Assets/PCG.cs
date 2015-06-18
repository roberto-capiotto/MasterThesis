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
		rocket = Instantiate(Resources.Load("Rocket")) as GameObject;
		rocket.name="Rocket";
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		myCollider = planet.transform.GetComponent<SphereCollider>();
		initialPosition=new Vector3
			(planet.transform.position.x ,planet.transform.position.y+(planet.transform.localScale.y/2)+myCollider.radius,0);
		rocket.transform.position=initialPosition;
		beforePosition=new Vector3(0,0,0);
		newPosition=new Vector3(0,0,0);
		creation=true;
		cam=GameObject.Find("Main Camera");
		(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).enabled = true;
		GeneratePlanet(newPosition);
	}

	void FixedUpdate () {/*
		if(gen<2){
			x=Random.Range(4f*(gen+1),8f*(gen+1));
			y=Random.Range(-2f,2f);
			planet = Instantiate(Resources.Load("Planet")) as GameObject;
			newPosition = new Vector3(x,y,0);
			planet.transform.position= newPosition;
			planet.name="Planet";
			gen++;
		}
	*/
	}

	void GeneratePlanet(Vector3 pos){
		beforePosition=pos;
		x=Random.Range(4f+beforePosition.x,8f+beforePosition.x);
		y=Random.Range(-2f,2f);
		newPosition=new Vector3(x,y,0);
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.transform.position= newPosition;
		planet.name="Planet";
		gen++;
		print (gen+" "+x+" "+y);
		if(gen<4){
			GeneratePlanet(newPosition);
		}
	}

	public bool GetCreation(){
		return creation;
	}
}
