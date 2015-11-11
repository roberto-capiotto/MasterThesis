using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PCG_randomPlace : MonoBehaviour {
	
	bool creation=false;
	GameObject rocket,planet,cam;
	GameObject wormhole,wormhole2,swing,blackHole,dockingStation;
	Wormhole wormholeManager,wormhole2Manager;
	Rocket rocketManager;
	Planet planetManager;
	Camera myCam;
	float x,y,rand,randX,randY,camSize=5;
	public Vector3 initialPosition;
	Vector3 newPosition;
	Vector3 camPosition;
	Vector3 lastPosition;
	public Vector3 startingCorner;
	GameObject endPlanet;
	SphereCollider myCollider;
	float deltaLevel;
	public CameraScript myCamera;
	bool scrollCamera=false;
	public int level=1;
	float maxFlyTime=10;
	float randomObject;
	public Button closeButton;
	public Button retryButton;
	public Button shootButton;
	float upBound;
	float downBound;
	float rightBound;
	GameObject[] planets = new GameObject[10];
	
	void Start () {
		// get Camera
		cam=GameObject.Find("Main Camera");
		myCam=cam.GetComponent<Camera>();
		// generate Rocket
		rocket = Instantiate(Resources.Load("Rocket")) as GameObject;
		rocket.name="Rocket";
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		
		// generate startPlanet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		myCollider = planet.transform.GetComponent<SphereCollider>();
		planetManager = planet.GetComponent ("Planet") as Planet;
		planetManager.SetPlanetType("first");
		if(Random.Range(0,2)==0)
			planetManager.DestroySatellite(0);
		else
			planetManager.DestroySatellite(2);
		
		// place startPlanet and move Camera
		rand=Random.Range(0,5);
		rand=2;
		print ("RAND: "+rand);
		startingCorner=Vector3.zero;
		planet.transform.position=new Vector3(startingCorner.x,startingCorner.y-(level-1)*4*rand-rand*camSize*3/2,0);
		camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y,-10);
		myCamera.SetInitialPosition(camPosition);
		myCamera.transform.position=camPosition;
		myCamera.ShowFuelText();
		
		// place Rocket
		initialPosition=new Vector3
			(planet.transform.position.x ,planet.transform.position.y+(planet.transform.localScale.y/2)+myCollider.radius,0);
		rocketManager.ChangeInitialPosition(initialPosition);
		rocketManager.SetInitialPosition();
		print ("x: "+initialPosition.x+" y: "+initialPosition.y);
		rocketManager.SetCollPlanet(planetManager);
		
		// the rocket and the first planet was generated
		creation=true;
		// enable the movement of the Camera
		myCamera.enabled=true;

		startingCorner=planet.transform.position;
		endPlanet=GenerateLevel(startingCorner);
		
		//deltaLevel = camSize*4;
		myCam.orthographicSize=9;

		Vector3 coord = Vector3.zero;
		coord.x = -Screen.width/2+80;
		coord.y = Screen.height/2-20;
		retryButton.GetComponent<RectTransform>().localPosition = coord;
		
		coord.x = Screen.width/2-80;
		coord.y = Screen.height/2-20;
		closeButton.GetComponent<RectTransform>().localPosition = coord;

		coord.x = Screen.width/2-80;
		coord.y = -Screen.height/2+20;
		shootButton.GetComponent<RectTransform>().localPosition = coord;
	}
	
	void FixedUpdate () {
		if(!rocketManager.GetColliding() && Time.time-rocketManager.GetTimer()>maxFlyTime){
			print ("reset due to flying");
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			rocketManager.SetColliding(true);
			myCamera.ResetPosition();
		}

		if(rocketManager.GetReplace()){
			myCamera.ResetPosition();
			rocketManager.SetReplace(false);
		}

		if(planetManager.levelCompleted){
			
			planetManager.levelCompleted=false;

			// modify Deltas
			myCam.orthographicSize=myCam.orthographicSize+4;
			myCamera.SetDeltas(myCamera.GetDeltaX()+4,myCamera.GetDeltaY()+4);
			
			// move Camera
			print ("dx "+myCamera.GetDeltaX()+" dy "+myCamera.GetDeltaY());
			camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y,-10);
			//myCamera.SetPost(camPosition.x+myCamera.GetDeltaX());
			// THIS IS THE LIMIT OF THIS LEVEL
			camPosition=new Vector3(lastPosition.x+2*myCamera.GetDeltaX(),camPosition.y,-10);
			myCamera.SetLimit(camPosition);
			print ("camX: "+camPosition.x +" lastX: "+ lastPosition.x +" DX: "+ myCamera.GetDeltaX());
			myCamera.SetInitialPosition(camPosition);
			//myCamera.transform.position=camPosition;
			//camPosition= new Vector3(endPlanet.transform.position.x,endPlanet.transform.position.y,-10);
			scrollCamera=true;
			
			// set new rocket initialPosition
			initialPosition=new Vector3
				(endPlanet.transform.position.x ,endPlanet.transform.position.y+(endPlanet.transform.localScale.y/2)+myCollider.radius,0);
			rocketManager.ChangeInitialPosition(initialPosition);
			rocketManager.onStart=true;
			
			// Generate New Level
			startingCorner=endPlanet.transform.position;
			endPlanet=GenerateLevel(startingCorner);
		}
		// horizontal scroll to New Level
		if(scrollCamera){
			if(myCamera.transform.position.x<camPosition.x){
				myCamera.transform.position = new Vector3(myCamera.transform.position.x+0.2f,myCamera.transform.position.y,-10);
				/*if(myCamera.transform.position.x<camPosition.x){
						myCamera.transform.position = new Vector3(myCamera.transform.position.x+0.1f,myCamera.transform.position.y,-10);
					}*/
			}
			else{
				scrollCamera=false;
				myCamera.transform.position=camPosition;
				myCamera.SetThisAsInitialPosition();
			}
		}
	}
	
	GameObject GenerateLevel(Vector3 pos){

		lastPosition=new Vector3(0,0,0);

		Vector3[] selectedPlanets = new Vector3[10];
		bool recreate,retry=false;

		int j=0,k=0,l=0;
		for(;l<9;l++){
			recreate=true;
			while(recreate){
				randX=Random.Range(8f,25f+(level-1)*16f);
				randY=Random.Range(-1.0f,1.0f);
				x=pos.x+randX*Mathf.Cos(randY);
				y=pos.y+randX*Mathf.Sin(randY);
				print("TRY: x="+x+" y="+y+" randX="+randX+" randY="+randY+" Cos:"+Mathf.Cos(randY)+" Sin: "+Mathf.Sin(randY));
				for(j=0;j<selectedPlanets.Length;j++){
					if(Mathf.Abs(x-selectedPlanets[j].x)<5 && Mathf.Abs(y-selectedPlanets[j].y)<5){
						j=10;
						retry=true;
						print ("ERROR - OVERLAP "+l);
					}
				}
				if(!retry){
					recreate=false;
				}
				retry=false;
			}
			newPosition=new Vector3(x,y,0);
			planet = Instantiate(Resources.Load("Planet")) as GameObject;
			planet.transform.position= newPosition;
			planet.name="Planet";
			planetManager = planet.GetComponent ("Planet") as Planet;
			planetManager.DestroySatellite(Random.Range(0,4));
			selectedPlanets[k] = newPosition;
			planets[k] = planet;
			k++;
			print("CREATE "+l);
			if(planet.transform.position.x>lastPosition.x)
				lastPosition=planet.transform.position;
			// searching for downBound
			if(planet.transform.position.y<downBound)
				downBound=planet.transform.position.y;
			// searching for upBound
			if(planet.transform.position.y>upBound)
				upBound=planet.transform.position.y;
		}
		//lastPosition = new Vector3(lastPosition.x+5,lastPosition.y,0);

		// generate endPlanet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		planetManager = planet.GetComponent ("Planet") as Planet;
		planetManager.SetPlanetType("end");
		if(Random.Range(0,2)==0)
			planetManager.DestroySatellite(0);
		else
			planetManager.DestroySatellite(2);
		// place endPlanet
		rand=Random.Range(0,5);
		rand=rand-2;
		print ("endRAND: "+rand);
		planet.transform.position=new Vector3(pos.x+(level-1)*16+4*camSize*3/2,pos.y-(level-1)*4*rand-rand*camSize*3/2,0);
		planets[k]=planet;

		print ("pos.X: "+pos.x+" planetPos: "+planet.transform.position.x);
		rightBound = planet.transform.position.x+2*myCamera.GetDeltaX();
		upBound = upBound + myCamera.GetDeltaY();
		downBound = downBound - myCamera.GetDeltaY();
		if(rand==4)
			downBound = planet.transform.position.y-myCamera.GetDeltaY();
		if(rand==0)
			upBound = planet.transform.position.y+myCamera.GetDeltaY();
		
		if(myCamera.GetBound(1)<upBound)
			myCamera.SetBound(upBound,1);
		
		if(myCamera.GetBound(2)>downBound)
			myCamera.SetBound(downBound,2);
		
		myCamera.SetBound(rightBound,3);

		level++;
		return planet;
	}

	public void Shoot(){
		if(rocketManager.GetColliding()){
			rocketManager.GetCollPlanet().SetShoot(true);
		}
	}

	public void Retry(){
		if(!rocketManager.onStart)
			rocketManager.SetInitialPosition();
		for(int l=0;l<10;l++){
			Destroy(planets[l]);
		}
		level--;
		endPlanet=GenerateLevel(startingCorner);
	}

	public void Close(){
		Application.LoadLevel("mainMenu");
	}
	
	public bool GetCreation(){
		return creation;
	}
}
