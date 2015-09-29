﻿using UnityEngine;
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
		rand=2;
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
		endPlanet=GenerateLevel(planet.transform.position);
		
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
			startingCorner=new Vector3(endPosition.x,endPosition.y+(level-1)*4*rand+rand*camSize*3/2-randY*level,0);
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
			endPlanet=GenerateLevel(endPlanet.transform.position);
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

		float left,up,down,right;
		left=myCamera.GetBound(0);
		up=myCamera.GetBound(1);
		down=myCamera.GetBound(2);
		right=myCamera.GetBound(3);

		Vector3[] planets = new Vector3[10];
		bool recreate=false;

		int i=3,j=0,k=0,l=0,cont=0;
		for(;l<9;l++){
			//for(j=0;j<2;j++){
				//randX=0;
				//randY=0;
				//do{
					randX=Random.Range(5f,30f);
					randY=Random.Range(-1.0f,1.0f);
					x=pos.x+randX*Mathf.Cos(randY);
					y=pos.y+randX*Mathf.Sin(randY);
					//x=pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2+randX*level;
					//y=pos.y-(level-1)*4*(j+1)-(j+1)*camSize*3/2+randY*level;
					cont++;
					print("TRY: x="+x+" y="+y+" randX="+randX+" randY="+randY+" Cos:"+Mathf.Cos(randY)+" Sin: "+Mathf.Sin(randY));
				//}while((y<down || y>up || x<left || x>right) && cont<2);
			j=0;
			recreate=false;
			while(j<planets.Length){
				if(x-planets[j].x<5 && Mathf.Abs(y-planets[j].y)<5){
					j=planets.Length;
					recreate=true;
					print ("ERROR - OVERLAP");
				}
				j++;
			}
			if(!recreate){
				newPosition=new Vector3(x,y,0);
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.transform.position= newPosition;
				planet.name="Planet";
				planetManager = planet.GetComponent ("Planet") as Planet;
				planetManager.DestroySatellite(Random.Range(0,4));
				planets[k] = newPosition;
				k++;
			}
			//}
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
		rand=rand-2;
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