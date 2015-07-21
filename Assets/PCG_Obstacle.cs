using UnityEngine;
using System.Collections;

public class PCG_Obstacle : MonoBehaviour {
	
	bool creation=false;
	GameObject rocket,planet,cam,asteroid;
	Rocket rocketManager;
	Planet planetManager;
	Camera myCam;
	float x,y,rand,randX,randY,endRand,camSize=5;
	Vector3 initialPosition;
	Vector3 newPosition;
	Vector3 endPosition;
	Vector3 camPosition;
	Vector3 startingCorner;
	GameObject startPlanet;
	SphereCollider myCollider;
	float deltaLevel;
	public CameraMovement myCamera;
	bool scrollCamera=false;
	public int level=1;
	public bool[] path;
	
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
		myCam.orthographicSize=9;
	}
	
	void FixedUpdate () {
		if(planetManager.levelCompleted){
			myCamera.transform.position=endPosition;
			startingCorner=new Vector3(endPosition.x,startingCorner.y-12*(level-1)*camSize/2-deltaLevel,0);
			startPlanet=GenerateLevel(startingCorner);
			// unlock DownBound and RightBound
			myCamera.SetBound(startingCorner.y-level*camSize*8,2);
			myCamera.SetBound(startingCorner.x+level*camSize*8,3);
			
			// move Rocket
			initialPosition=new Vector3
				(startPlanet.transform.position.x ,startPlanet.transform.position.y+(startPlanet.transform.localScale.y/2)+myCollider.radius,0);
			rocketManager.ChangeInitialPosition(initialPosition);
			rocketManager.SetInitialPosition();
			
			// unlock LeftBound and UpBound
			myCamera.SetBound(startingCorner.x-2*camSize,0);
			myCamera.SetBound(startingCorner.y+camSize*2,1);
			
			// move Camera
			camPosition= new Vector3(startPlanet.transform.position.x,startPlanet.transform.position.y,-10);
			scrollCamera=true;
			(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).enabled = false;
		}
		if(scrollCamera){
			if(myCamera.transform.position.y>camPosition.y){
				myCamera.transform.position = new Vector3(myCamera.transform.position.x,myCamera.transform.position.y-0.2f,-10);
			}
			else{
				scrollCamera=false;
				myCamera.transform.position=camPosition;
				(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).enabled = true;
				(cam.GetComponent( "CameraMovement" ) as MonoBehaviour).Invoke("SetThisAsInitialPosition",1);
			}
		}
	}
	
	GameObject GenerateLevel(Vector3 pos){
		// generate startPlanet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		myCollider = planet.transform.GetComponent<SphereCollider>();
		GameObject retPlan = planet;
		planetManager = planet.GetComponent ("Planet") as Planet;
		planetManager.DestroySatellite(Random.Range(0,4));
		// place startPlanet
		rand=Random.Range(0,5);
		print ("RAND: "+rand);
		if(rand==0){
			planet.transform.position=new Vector3(pos.x,pos.y-(level-1)*4-camSize/2,0);
		}else{
			if(rand==4){
				planet.transform.position=new Vector3(pos.x,pos.y-(level-1)*4*(rand-1/2.0f)-(rand-1/2.0f)*camSize*3/2,0);
			}
			else{
				planet.transform.position=new Vector3(pos.x,pos.y-(level-1)*4*rand-rand*camSize*3/2,0);
			}
		}

		int i=0,j=0;
		for(;i<3;i++){
			for(j=0;j<3;j++){
				randX=Random.Range(-1.5f,1.5f);
				randY=Random.Range(-1.5f,1.5f);
				x=pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2+randX*level;
				y=pos.y-(level-1)*4*(j+1)-(j+1)*camSize*3/2+randY*level;
				newPosition=new Vector3(x,y,0);
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.transform.position= newPosition;
				planet.name="Planet";
				planetManager = planet.GetComponent ("Planet") as Planet;
				planetManager.DestroySatellite(Random.Range(0,4));
			}
		}
		// generate endPlanet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		planetManager = planet.GetComponent ("Planet") as Planet;
		planetManager.SetPlanetType("end");
		// place endPlanet
		endRand=Random.Range(0,5);
		print ("endRAND: "+endRand);
		if(rand==0){
			planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2,pos.y-(level-1)*4*rand-camSize/2,0);
		}
		else{
			if(rand==4){
				planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2,pos.y-(level-1)*4*(rand-1/2.0f)-(rand-1/2.0f)*camSize*3/2,0);
			}
			else{
				planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2,pos.y-(level-1)*4*rand-rand*camSize*3/2,0);
			}
		}
		
		endPosition=planet.transform.position;
		BuildPath();
		level++;
		return retPlan;
	}

	void BuildPath(){
		path = new bool[9];
		int i;
		for(i=0;i<9;i++)
			path[i]=false;

		// first column
		if(rand==0){
			path[0]=true;
		}
		if(rand==1){
			path[0]=true;
			path[1]=true;
		}
		if(rand==2){
			path[0]=true;
			path[1]=true;
			path[2]=true;
		}
		if(rand==3){
			path[1]=true;
			path[2]=true;
		}
		if(rand==4){
			path[2]=true;
		}

		// last column
		if(endRand==0){
			path[6]=true;
		}
		if(endRand==1){
			path[6]=true;
			path[7]=true;
		}
		if(endRand==2){
			path[6]=true;
			path[7]=true;
			path[8]=true;
		}
		if(endRand==3){
			path[7]=true;
			path[8]=true;
		}
		if(endRand==4){
			path[8]=true;
		}

		// connect
		if(path[0] && path[6])
			path[3]=true;
		if(path[0] && path[7])
			path[3]=true;
		if(path[1] && path[6])
			path[4]=true;
		if(path[1] && path[7])
			path[4]=true;
		if(path[1] && path[8])
			path[5]=true;
		if(path[2] && path[7])
			path[4]=true;
		if(path[2] && path[8])
			path[5]=true;

		// up couple
		if(!(path[0] && path[1])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4+camSize*3/2;
			y=startingCorner.y-(level-1)*4*(3/2.0f)-(3/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		if(!(path[3] && path[4])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*2+2*camSize*3/2;
			y=startingCorner.y-(level-1)*4*(3/2.0f)-(3/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		if(!(path[6] && path[7])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*3+3*camSize*3/2;
			y=startingCorner.y-(level-1)*4*(3/2.0f)-(3/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}

		// down couple
		if(!(path[1] && path[2])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4+camSize*3/2;
			y=startingCorner.y-(level-1)*4*(5/2.0f)-(5/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		if(!(path[4] && path[5])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*2+2*camSize*3/2;
			y=startingCorner.y-(level-1)*4*(5/2.0f)-(5/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		if(!(path[7] && path[8])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*3+3*camSize*3/2;
			y=startingCorner.y-(level-1)*4*(5/2.0f)-(5/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}

		// left couple
		if(!(path[0] && path[3])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*(3/2.0f)+(3/2.0f)*camSize*3/2;
			y=startingCorner.y-(level-1)*4-camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		if(!(path[1] && path[4])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*(3/2.0f)+(3/2.0f)*camSize*3/2;
			y=startingCorner.y-(level-1)*4*2-2*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		if(!(path[2] && path[5])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*(3/2.0f)+(3/2.0f)*camSize*3/2;
			y=startingCorner.y-(level-1)*4*3-3*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}

		// right couple
		if(!(path[3] && path[6])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*(5/2.0f)+(5/2.0f)*camSize*3/2;
			y=startingCorner.y-(level-1)*4-camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		if(!(path[4] && path[7])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*(5/2.0f)+(5/2.0f)*camSize*3/2;
			y=startingCorner.y-(level-1)*4*2-2*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		if(!(path[5] && path[8])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=startingCorner.x+(level-1)*4*(5/2.0f)+(5/2.0f)*camSize*3/2;
			y=startingCorner.y-(level-1)*4*3-3*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
		}
		
	}
	
	public bool GetCreation(){
		return creation;
	}
}