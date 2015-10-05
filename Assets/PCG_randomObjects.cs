using UnityEngine;
using System.Collections;

public class PCG_randomObjects : MonoBehaviour {
	
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
	Vector3 endPosition;
	Vector3 camPosition;
	public Vector3 startingCorner;
	GameObject endPlanet;
	SphereCollider myCollider;
	float deltaLevel;
	public CameraContinue myCamera;
	bool scrollCamera=false;
	public int level=1;
	float maxFlyTime=10;
	float randomObject;
	
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
		/*if(rand==0){
			planet.transform.position=new Vector3(startingCorner.x,startingCorner.y-(level-1)*4-camSize/2,0);
			camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y-5,-10);
		}else{
			if(rand==4){
				planet.transform.position=new Vector3(startingCorner.x,startingCorner.y-(level-1)*4*(rand-1/2.0f)-(rand-1/2.0f)*camSize*3/2,0);
				camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y+4,-10);
			}
			else{
				planet.transform.position=new Vector3(startingCorner.x,startingCorner.y-(level-1)*4*rand-rand*camSize*3/2,0);
				camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y,-10);
			}
		}*/
		planet.transform.position=new Vector3(startingCorner.x,startingCorner.y-(level-1)*4*rand-rand*camSize*3/2,0);
		camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y,-10);
		/*if(rand==0){
			camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y-7.5f,-10);
		}else{
			if(rand==4){
				camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y+7.5f,-10);
			}
			else{
				camPosition=new Vector3(planet.transform.position.x+7.5f,planet.transform.position.y,-10);
			}
		}*/
		myCamera.SetInitialPosition(camPosition);
		myCamera.transform.position=camPosition;
		
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
	}
	
	void FixedUpdate () {
		if(!rocketManager.GetColliding() && Time.time-rocketManager.GetTimer()>maxFlyTime){
			print ("reset due to flying");
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			rocketManager.SetColliding(true);
			myCamera.ResetPosition();
		}
		
		if(planetManager.levelCompleted){
			
			planetManager.levelCompleted=false;
			
			//myCamera.transform.position=endPosition;
			startingCorner=new Vector3(endPosition.x,endPosition.y+(level-1)*4*rand+rand*camSize*3/2,0);
			//startingCorner=new Vector3(endPosition.x,startingCorner.y-12*camSize/2-deltaLevel,0);
			
			// unlock DownBound and RightBound
			myCamera.SetBound(startingCorner.y-level*camSize*5,2);
			myCamera.SetBound(startingCorner.x+level*camSize*5,3);
			
			// unlock LeftBound and UpBound
			//			myCamera.SetBound(startingCorner.x-camSize,0);
			myCamera.SetBound(startingCorner.y+camSize,1);
			
			// modify Deltas
			myCam.orthographicSize=myCam.orthographicSize+4;
			myCamera.SetDeltas(myCamera.GetDeltaX()+4,myCamera.GetDeltaY()+4);
			
			// move Camera
			print ("dx "+myCamera.GetDeltaX()+" dy "+myCamera.GetDeltaY());
			camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y,-10);
			/*if(rand==0){
				camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y-myCamera.GetDeltaY(),-10);
			}else{
				if(rand==4){
					camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y+myCamera.GetDeltaY(),-10);
				}
				else{
					camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y,-10);
				}
			}*/
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
		
		int i=0,j=0;
		for(;i<3;i++){
			for(j=0;j<3;j++){
				//randX=0;
				//randY=0;
				randX=Random.Range(-1.5f,1.5f);
				randY=Random.Range(-1.5f,1.5f);
				x=pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2+randX*level;
				y=pos.y-(level-1)*4*(j+1)-(j+1)*camSize*3/2+randY*level;
				newPosition=new Vector3(x,y,0);
				randomObject=Random.Range(0,1.0f);
				if(randomObject<0.65f){
					planet = Instantiate(Resources.Load("Planet")) as GameObject;
					planet.transform.position= newPosition;
					planet.name="Planet";
					planetManager = planet.GetComponent ("Planet") as Planet;
					planetManager.DestroySatellite(Random.Range(0,4));
				}
				else{
					if(randomObject<0.75f){
						swing = Instantiate(Resources.Load("Swing")) as GameObject;
						swing.transform.position=newPosition;
						swing.name="Swing";
					}
					else{
						if(randomObject<0.9f){
							dockingStation = Instantiate(Resources.Load("DockingStation")) as GameObject;
							dockingStation.transform.position=newPosition;
							dockingStation.name="DockingStation";
						}
						else{
//							if(randomObject<0.95f){
								blackHole = Instantiate(Resources.Load("BlackHole")) as GameObject;
								blackHole.transform.position=newPosition;
								blackHole.name="DockingStation";
/*							}
							else{
								wormhole = Instantiate(Resources.Load("Wormhole")) as GameObject;
								wormhole.transform.position=newPosition;
								wormhole.name="Wormhole";
								wormhole2 = Instantiate(Resources.Load("Wormhole")) as GameObject;
								wormhole2.transform.position=new Vector3(newPosition.x,newPosition.y+10,0);
								wormhole2.name="Wormhole";
								wormholeManager = wormhole.GetComponent ("Wormhole") as Wormhole;
								wormhole2Manager = wormhole2.GetComponent ("Wormhole") as Wormhole;
								wormholeManager.SetExit(wormhole2);
								wormhole2Manager.SetExit(wormhole);
							}*/
						}
					}
				}
			}
		}
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
		/*if(rand==0){
			planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2,pos.y-(level-1)*4*rand-camSize/2,0);
		}
		else{
			if(rand==4){
				planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2,pos.y-(level-1)*4*(rand-1/2.0f)-(rand-1/2.0f)*camSize*3/2,0);
			}
			else{
				planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2,pos.y-(level-1)*4*rand-rand*camSize*3/2,0);
			}
		}*/
		
		endPosition=planet.transform.position;
		level++;
		return planet;
	}
	
	public bool GetCreation(){
		return creation;
	}
}
