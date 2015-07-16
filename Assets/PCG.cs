using UnityEngine;
using System.Collections;

public class PCG : MonoBehaviour {
	
	bool creation=false;
	GameObject rocket,planet,cam;
	Rocket rocketManager;
	Camera myCam;
	public int gen=0;
	float x,y,rand,camSize=5;
	Vector3 initialPosition;
	Vector3 beforePosition;
	Vector3 newPosition;
	Vector3 startingCorner;
	SphereCollider myCollider;
	
	void Start () {
		// get Camera
		cam=GameObject.Find("Main Camera");
		myCam=cam.GetComponent<Camera>();
		// generate Rocket
		rocket = Instantiate(Resources.Load("Rocket")) as GameObject;
		rocket.name="Rocket";
		//rocket.transform.position = new Vector3(-10,-10,0);
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		// generate Planet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		myCollider = planet.transform.GetComponent<SphereCollider>();
		gen++;
		// place Planet
		rand=Random.Range(0,4);
		print ("RAND: "+rand);
		planet.transform.position=new Vector3(0,-rand*camSize,0);
		// move Camera
		if(rand==0){
			myCam.transform.position=new Vector3(planet.transform.position.x,planet.transform.position.y-camSize/2,-10);
		}
		else{
			if(rand==4){
				myCam.transform.position=new Vector3(planet.transform.position.x,planet.transform.position.y+camSize/2,-10);
			}
			else{
				myCam.transform.position=new Vector3(planet.transform.position.x,planet.transform.position.y,-10);
			}
		}
		(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).Invoke("SetThisAsInitialPosition",1);
		// place Rocket
		initialPosition=new Vector3
			(planet.transform.position.x ,planet.transform.position.y+(planet.transform.localScale.y/2)+myCollider.radius,0);
		rocketManager.ChangeInitialPosition(initialPosition);
		rocketManager.SetInitialPosition();
		print ("x: "+initialPosition.x+" y: "+initialPosition.y);
		startingCorner=Vector3.zero;
		beforePosition=Vector3.zero;
		newPosition=planet.transform.position;
		// the rocket and the first planet was generated
		creation=true;
		// enable the movement of the Camera
		(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).enabled = true;
		GenerateLevel(newPosition);
	}

	void FixedUpdate () {
	}

	void GenerateLevel(Vector3 pos){
		int i=0,j=0;
		for(;i<3;i++){
			for(j=0;j<3;j++){
				x=startingCorner.x+(i+1)*camSize*3/2;
				y=startingCorner.y-(j+1)*camSize*3/2;
				newPosition=new Vector3(x,y,0);
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.transform.position= newPosition;
				planet.name="Planet";
			}
		}
	}

	public bool GetCreation(){
		return creation;
	}
}
