using UnityEngine;
using System.Collections;

public class PCG : MonoBehaviour {
	
	bool creation=false;
	GameObject rocket,planet,cam;
	Rocket rocketManager;
	Planet planetManager;
	Camera myCam;
	float x,y,rand,camSize=5;
	Vector3 initialPosition;
	Vector3 newPosition;
	Vector3 startingCorner;
	GameObject startPlanet;
	SphereCollider myCollider;
	float deltaLevel;
	public CameraMovement myCamera;
	
	void Start () {
		// get Camera
		cam=GameObject.Find("Main Camera");
		myCam=cam.GetComponent<Camera>();
		// generate Rocket
		rocket = Instantiate(Resources.Load("Rocket")) as GameObject;
		rocket.name="Rocket";
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		startingCorner=Vector3.zero;
		startPlanet=GenerateLevel(startingCorner);

		// place Rocket
		initialPosition=new Vector3
			(startPlanet.transform.position.x ,startPlanet.transform.position.y+(startPlanet.transform.localScale.y/2)+myCollider.radius,0);
		rocketManager.ChangeInitialPosition(initialPosition);
		rocketManager.SetInitialPosition();
		print ("x: "+initialPosition.x+" y: "+initialPosition.y);
		// move Camera
		myCam.transform.position=new Vector3(startPlanet.transform.position.x,startPlanet.transform.position.y,-10);
		(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).Invoke("SetThisAsInitialPosition",1);

		// the rocket and the first level was generated
		creation=true;
		// enable the movement of the Camera
		(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).enabled = true;

		deltaLevel = camSize*4;
	}

	void FixedUpdate () {
		if(planetManager.levelCompleted){
			startingCorner=new Vector3(startingCorner.x+12*camSize/2,startingCorner.y-12*camSize/2-deltaLevel,0);
			startPlanet=GenerateLevel(startingCorner);
			// TODO: unlockBounds and move camera
			myCamera.SetBound(startingCorner.y-camSize*8,2);
			myCamera.SetBound(startingCorner.x+camSize*8,3);
			// place Rocket
			initialPosition=new Vector3
				(startPlanet.transform.position.x ,startPlanet.transform.position.y+(startPlanet.transform.localScale.y/2)+myCollider.radius,0);
			rocketManager.ChangeInitialPosition(initialPosition);
			rocketManager.SetInitialPosition();
			myCamera.SetBound(startingCorner.x-2*camSize,0);
			myCamera.SetBound(startingCorner.y+camSize*2,1);
			myCam.transform.position=startPlanet.transform.position;
		}
	}

	GameObject GenerateLevel(Vector3 pos){
		// generate startPlanet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		myCollider = planet.transform.GetComponent<SphereCollider>();
		GameObject retPlan = planet;
		planetManager = planet.GetComponent ("Planet") as Planet;
		planetManager.DestroySatellite(Random.Range(0,3));
		// place startPlanet
		rand=Random.Range(0,4);
		print ("RAND: "+rand);
		planet.transform.position=new Vector3(pos.x,pos.y-rand*camSize*3/2,0);
		// move Camera
		/*if(rand==0){
			planet.transform.position=new Vector3(pos.x,pos.y-camSize/2,0);
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
		(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).Invoke("SetThisAsInitialPosition",1);*/
		int i=0,j=0;
		for(;i<3;i++){
			for(j=0;j<3;j++){
				x=pos.x+(i+1)*camSize*3/2;
				y=pos.y-(j+1)*camSize*3/2;
				newPosition=new Vector3(x,y,0);
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.transform.position= newPosition;
				planet.name="Planet";
				planetManager = planet.GetComponent ("Planet") as Planet;
				planetManager.DestroySatellite(Random.Range(0,3));
			}
		}
		// generate endPlanet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		planetManager = planet.GetComponent ("Planet") as Planet;
		planetManager.SetPlanetType("end");
		// place endPlanet
		rand=Random.Range(0,4);
		print ("endRAND: "+rand);
		if(rand==0)
			planet.transform.position=new Vector3(pos.x+(i+1)*camSize*3/2,pos.y-camSize/2,0);
		else
			planet.transform.position=new Vector3(pos.x+(i+1)*camSize*3/2,pos.y-rand*camSize*3/2,0);

		return retPlan;
	}

	public bool GetCreation(){
		return creation;
	}
}
