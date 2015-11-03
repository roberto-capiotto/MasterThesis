using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PCG_continue : MonoBehaviour {

	public int level=1;
	float maxFlyTime=10;
	// flags
	bool creation=false;
	bool slide=false;
	bool addOnce=false;
	// gameobjects
	GameObject rocket,planet,cam;
	GameObject[] planets = new GameObject[10];
	GameObject endPlanet;
	SphereCollider myCollider;
	Rocket rocketManager;
	Planet planetManager;
	// camera
	Camera myCam;
	public CameraContinue myCamera;
	bool scrollCamera=false;
	public Vector3 initialPosition;
	Vector3 newPosition;
	Vector3 endPosition;
	Vector3 camPosition;
	Vector3 lastPosition;
	public Vector3 startingCorner;
	float deltaLevel;
	float x,y,rand,randX,randY,camSize=5;
	// buttons
	public Button closeButton;
	public Button retryButton;
	public Button shootButton;
	// boundaries
	float upBound;
	float downBound;
	float rightBound;

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
		print ("RAND: "+rand);
		planet.transform.position=new Vector3(startingCorner.x,startingCorner.y-(level-1)*4*rand-rand*camSize*3/2,0);
		camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y,-10);
		/*if(rand==0){
			myCamera.SetOffset(7.5f/2);
			camPosition=new Vector3(camPosition.x,camPosition.y-myCamera.GetOffset(),-10);
		}
		else{
			if(rand==4){
				myCamera.SetOffset(7.5f/2);
				camPosition=new Vector3(camPosition.x,camPosition.y+myCamera.GetOffset(),-10);
			}
			else{
				myCamera.SetOffset(0);
				camPosition=new Vector3(camPosition.x,camPosition.y,-10);
			}
		}*/
		myCamera.SetInitialPosition(camPosition);
		myCamera.transform.position=camPosition;
		myCamera.ShowFuelText();

		// place Rocket
		initialPosition=new Vector3
			(planet.transform.position.x ,planet.transform.position.y+(planet.transform.localScale.y/2)+myCollider.radius,0);
		rocketManager.ChangeInitialPosition(initialPosition);
		rocketManager.SetInitialPosition();
		print ("x: "+initialPosition.x+" y: "+initialPosition.y);
		
		// the rocket and the first planet was generated
		creation=true;
		// enable the movement of the Camera
		myCamera.enabled=true;

		startingCorner=Vector3.zero;
		//startPlanet=GenerateLevel(startingCorner);
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

		if(rocketManager.GetColliding() && level>2 && rocketManager.GetCollPlanet().planetType.Equals("end")){
			// ctrl endPlanet
			if(myCamera.transform.position.x<rocket.transform.position.x){
				if(myCamera.IsBackStep(myCamera.transform.position.x)){
					if(myCamera.transform.position.x == myCamera.GetCameraStep(level-3).x){
						camPosition = myCamera.GetInitialPosition();
						myCamera.SetCurLevel(myCamera.GetLevel());
					}
					else{
						camPosition = myCamera.GetNextStep(myCamera.transform.position.x);
						addOnce=true;
					}
					scrollCamera=true;
					slide=true;
				}
			}
		}

		if(planetManager.levelCompleted){

			planetManager.levelCompleted=false;
			startingCorner=new Vector3(endPosition.x,endPosition.y+(level-1)*4*rand+rand*camSize*3/2,0);

			// modify Deltas
			myCam.orthographicSize=myCam.orthographicSize+4;
			myCamera.SetDeltas(myCamera.GetDeltaX()+4,myCamera.GetDeltaY()+4);
			print ("dx "+myCamera.GetDeltaX()+" dy "+myCamera.GetDeltaY());

			// move Camera
			camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y,-10);
			myCamera.SetPost(camPosition.x+myCamera.GetDeltaX());
			// THIS IS THE LIMIT OF THIS LEVEL
			camPosition=new Vector3(lastPosition.x+2*myCamera.GetDeltaX(),camPosition.y,-10);
			/*if(rand==0){
				myCamera.SetOffset(myCamera.GetDeltaY()/2);
				camPosition=new Vector3(camPosition.x,camPosition.y-myCamera.GetOffset(),-10);
			}
			else{
				if(rand==4){
					myCamera.SetOffset(myCamera.GetDeltaY()/2);
					camPosition=new Vector3(camPosition.x,camPosition.y+myCamera.GetOffset(),-10);
				}
				else{
					myCamera.SetOffset(0);
					camPosition=new Vector3(camPosition.x,camPosition.y,-10);
				}
			}*/
			myCamera.SetLimit(camPosition);
			print ("camX: "+camPosition.x +" lastX: "+ lastPosition.x +" DX: "+ myCamera.GetDeltaX());

			myCamera.SetInitialPosition(camPosition);
			scrollCamera=true;

			// set new rocket initialPosition
			initialPosition=new Vector3
				(endPlanet.transform.position.x ,endPlanet.transform.position.y+(endPlanet.transform.localScale.y/2)+myCollider.radius,0);
			rocketManager.ChangeInitialPosition(initialPosition);
			rocketManager.onStart=true;

			// Generate New Level
			endPlanet=GenerateLevel(startingCorner);
		}
		// horizontal scroll to New Level
		if(scrollCamera){
			if(myCamera.transform.position.x<camPosition.x){
				myCamera.transform.position = new Vector3(myCamera.transform.position.x+0.2f,myCamera.transform.position.y,-10);
			}
			else{
				scrollCamera=false;
				myCamera.transform.position=camPosition;
				if(!slide)
					myCamera.SetThisAsInitialPosition();
				if(addOnce)
					myCamera.SetCurLevel(myCamera.GetCurLevel()+1);
				slide=false;
				addOnce=false;
			}
		}
	}
	
	GameObject GenerateLevel(Vector3 pos){

		lastPosition=new Vector3(0,0,0);

		int i=0,j=0;
		for(;i<3;i++){
			for(j=0;j<3;j++){
				randX=Random.Range(-1.3f,1.3f);
				randY=Random.Range(-1.3f,1.3f);
				x=pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2+randX*level;
				y=pos.y-(level-1)*4*(j+1)-(j+1)*camSize*3/2+randY*level;
				newPosition=new Vector3(x,y,0);
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.transform.position= newPosition;
				planet.name="Planet";
				planets[i*3+j]=planet;
				planetManager = planet.GetComponent ("Planet") as Planet;
				planetManager.DestroySatellite(Random.Range(0,4));
				if(i==2){
					if(planet.transform.position.x>lastPosition.x)
						lastPosition=new Vector3(planet.transform.position.x+myCollider.radius+rocket.transform.localScale.x,planet.transform.position.y,0);
				}
				// searching for downBound
				if(j==2){
					if(planet.transform.position.y<downBound)
						downBound=planet.transform.position.y;
				}
				// searching for upBound
				if(j==0){
					if(planet.transform.position.y>upBound)
						upBound=planet.transform.position.y;
				}
			}
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
		print ("endRAND: "+rand);
		planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2,pos.y-(level-1)*4*rand-rand*camSize*3/2,0);
		planets[9]=planet;

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

		Vector3 step = Vector3.zero;
		step.x = pos.x+(level-1)*4*(1+1)+(1+1)*5*3/2;
		step.y = pos.y-(level-1)*4*(1+1)-(1+1)*5*3/2;
		myCamera.AddCameraStep(step);
		
		endPosition=planet.transform.position;
		level++;
		myCamera.SetLevel(level);
		return planet;
	}

	public void Shoot(){
		if(rocketManager.GetColliding()){
			rocketManager.GetCollPlanet().SetShoot(true);
		}
	}

	public void Retry(){
		rocketManager.SetInitialPosition();
		for(int l=0;l<10;l++){
			Destroy(planets[l]);
		}
		level--;
		endPlanet=GenerateLevel(startingCorner);
		myCamera.RemoveLastStep();
	}

	public void Close(){
		Application.LoadLevel("mainMenu");
	}

	public bool GetCreation(){
		return creation;
	}
}
