﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PCG_tutorial : MonoBehaviour {
	
	int counter=0;
	GameObject rocket,firstPlanet,planet,wormhole,wormhole2,swing,blackHole,asteroid,asteroid2,asteroid3,asteroid4,dockingStation,satellite,satellite2;
	GameObject endPlanet,cam;
	Rocket rocketManager;
	Planet planetManager,planetManager2;
	Wormhole wormholeManager,wormhole2Manager;
	Vector3 initialPosition,rocketInitialPosition;
	public Text text;
	public Text text2;
	bool fuel=false;
	bool exe=false;
	public bool gen=false;
	bool scroll=false;
	bool scrollCamera=false;
	Vector3 camPosition;
	Vector3 startPosition,startingCorner;
	Vector3 newPosition;
	Vector3 endPosition;
	float x,y,rand,randX,randY,camSize=5;
	public int level=1;
	public CameraContinue myCamera;
	Camera myCam;
	SphereCollider myCollider;
	
	void Start () {
		cam=GameObject.Find("Main Camera");
		myCam=cam.GetComponent<Camera>();
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		firstPlanet = GameObject.Find ("Planet");
		planetManager = firstPlanet.GetComponent ("Planet") as Planet;
		planetManager.DestroySatellite(0);
		//camSize = Camera.main.orthographicSize;
		initialPosition=Camera.main.transform.position;
		rocketInitialPosition=rocketManager.GetInitialPosition();
	}
	
	void FixedUpdate () {

		camSize = Camera.main.orthographicSize;

		/* OUT OF SCREEN*/
		if(Mathf.Abs(rocket.transform.position.x-Camera.main.transform.position.x)>camSize*16/10 ||
		   Mathf.Abs(rocket.transform.position.y-Camera.main.transform.position.y)>camSize){
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			Camera.main.transform.position=initialPosition;
		}
		
		if(fuel){
			text2.text="Fuel remaining: "+rocketManager.GetFuel();
		}

		if(counter==1 && !exe){
			if(planetManager.GetCollision()){
				exe=true;
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.name="Planet";
				planet.transform.position=new Vector3(16,0,0);
				planetManager2 = planet.GetComponent ("Planet") as Planet;
				planetManager2.SetPlanetType("first");
				planetManager2.DestroySatellite(0);
				scroll=true;
				camPosition=new Vector3(12,0,-10);
			}
		}
		else{
			if(counter==1){
				if(planetManager.GetCollision() && !gen){
					scroll=true;
					camPosition=new Vector3(12,0,-10);
				}
				if(planetManager2.GetCollision() && !gen){
					gen=true;
					rocketManager.ChangeInitialPosition(new Vector3(planet.transform.position.x,planet.transform.position.y+planet.transform.localScale.y/2+myCollider.radius,0));
					startPosition=planet.transform.position;
					endPlanet=GenerateLevel(startPosition,"free");
					(cam.GetComponent( "CameraContinue" ) as MonoBehaviour).enabled = true;
					Camera.main.orthographicSize=9;
					myCamera.transform.position=new Vector3(startPosition.x+myCamera.deltaX,startPosition.y-myCamera.deltaY,-10);
					myCamera.SetInitialPosition(myCamera.transform.position);
					initialPosition=myCamera.transform.position;
					myCamera.SetBound(camSize*20,3);
					text.text="Play a level";
				}
				if(planetManager.levelCompleted){
					print ("COMPLETE");
					planetManager.levelCompleted=false;

					startingCorner=new Vector3(endPosition.x,endPosition.y+(level-1)*4*rand+rand*5*3/2-randY*level,0);
					
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

					myCamera.SetInitialPosition(camPosition);
					scrollCamera=true;
					
					// set new rocket initialPosition
					rocketInitialPosition=new Vector3
						(endPlanet.transform.position.x ,endPlanet.transform.position.y+(endPlanet.transform.localScale.y/2)+myCollider.radius,0);
					rocketManager.ChangeInitialPosition(rocketInitialPosition);
					rocketManager.onStart=true;
					
					// Generate New Level
					endPlanet=GenerateLevel(startingCorner,"free");
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
		}

		if(scroll){
			Camera.main.transform.position=new Vector3(Camera.main.transform.position.x+0.2f,Camera.main.transform.position.y,-10);
			if(Camera.main.transform.position.x>camPosition.x){
				scroll=false;
				Camera.main.transform.position=new Vector3(camPosition.x,Camera.main.transform.position.y,-10);
			}
		}

	}

	GameObject GenerateLevel(Vector3 pos,string s){
		
		int i=0,j=0;
		for(;i<3;i++){
			for(j=0;j<3;j++){
				randX=0;
				randY=0;
				//randX=Random.Range(-1.5f,1.5f);
				//randY=Random.Range(-1.5f,1.5f);
				x=pos.x+(level-1)*4*(i+1)+(i+1)*5*3/2+randX*level;
				y=pos.y-(level-1)*4*(j+1)-(j+1)*5*3/2+randY*level;
				newPosition=new Vector3(x,y,0);
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.transform.position= newPosition;
				planet.name="Planet";
				planetManager = planet.GetComponent ("Planet") as Planet;
				if(s.Equals("free"))
					planetManager.DestroySatellite(0);
				else
					planetManager.DestroySatellite(Random.Range(0,4));
			}
		}
		// generate endPlanet
		planet = Instantiate(Resources.Load("Planet")) as GameObject;
		planet.name="Planet";
		planetManager = planet.GetComponent ("Planet") as Planet;
		planetManager.SetPlanetType("end");
		if(s.Equals("free"))
			planetManager.DestroySatellite(0);
		else
			planetManager.DestroySatellite(Random.Range(0,3));
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
	
	public void Change(){
		if(counter==9){
			Application.LoadLevel("PCG");
		}
		if(counter==8){
			// end of tutorial
			Destroy(planet);
			Destroy(dockingStation);
			rocketManager.ChangeInitialPosition(rocketInitialPosition);
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			initialPosition=new Vector3(0,0,-10);
			Camera.main.transform.position=initialPosition;
			text.text="This is the end of the tutorial";
			text2.text="Now you are ready for playing the game";
			counter++;
		}
		else{
			if(counter==7){
				// two planets, right one is checkpoint
				fuel=false;
				Destroy(dockingStation);
				rocketManager.SetInitialPosition();
				rocketManager.FullRefill();
				planetManager = planet.GetComponent ("Planet") as Planet;
				planetManager.SetPlanetType("checkpoint");
				text.text="The right planet is a checkpoint";
				text2.text="If you lose, you can restart the game from there";
				counter++;
			}
			else{
				if(counter==6){
					// two planets and docking station
					Destroy(asteroid);
					Destroy(asteroid2);
					Destroy(asteroid3);
					Destroy(asteroid4);
					Destroy(blackHole);
					rocketManager.SetInitialPosition();
					rocketManager.FullRefill();
					text.text="Try moving between planet. The docking station can refill you";
					text2.text="";
					dockingStation = Instantiate(Resources.Load("DockingStation")) as GameObject;
					dockingStation.name="DockingStation";
					dockingStation.transform.position=new Vector3(10,-3.5f,0);
					fuel=true;
					counter++;
				}
				else{
					if(counter==5){
						// two planets and obstacles
						Destroy(swing);
						rocketManager.SetInitialPosition();
						rocketManager.FullRefill();
						text.text="Try moving between planets without hitting asteroids or black hole";
						text2.text="";
						planet = Instantiate(Resources.Load("Planet")) as GameObject;
						planet.name="Planet";
						planet.transform.position=new Vector3(8,0,0);
						planetManager = planet.GetComponent ("Planet") as Planet;
						planetManager.DestroySatellite(0);
						asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
						asteroid.name="Asteroid";
						asteroid.transform.position=new Vector3(4,0,0);
						asteroid2 = Instantiate(Resources.Load("Asteroid")) as GameObject;
						asteroid2.name="Asteroid";
						asteroid2.transform.position=new Vector3(5,2.5f,0);
						asteroid3 = Instantiate(Resources.Load("Asteroid")) as GameObject;
						asteroid3.name="Asteroid";
						asteroid3.transform.position=new Vector3(9,-4,0);
						asteroid4 = Instantiate(Resources.Load("Asteroid")) as GameObject;
						asteroid4.name="Asteroid";
						asteroid4.transform.position=new Vector3(-3,-3,0);
						blackHole = Instantiate(Resources.Load("BlackHole")) as GameObject;
						blackHole.name="BlackHole";
						blackHole.transform.position=new Vector3(4,-2,0);
						blackHole.transform.localScale=new Vector3(0.35f,0.35f,0.35f);
						counter++;
					}
					else{
						if(counter==4){
							// planet and swing
							Destroy(wormhole);
							Destroy(wormhole2);
							Destroy(planet);
							rocketManager.SetInitialPosition();
							rocketManager.FullRefill();
							text.text="The right planet can be used for swinging the rocket";
							text2.text="";
							swing = Instantiate(Resources.Load("Swing")) as GameObject;
							swing.name="Swing";
							swing.transform.position=new Vector3(8,0,0);
							counter++;
						}
						else{
							if(counter==3){
								// two planets and wormholes
								Destroy(planet);
								planet = Instantiate(Resources.Load("Planet")) as GameObject;
								planet.name="Planet";
								planet.transform.position=new Vector3(8,0,0);
								planetManager = planet.GetComponent ("Planet") as Planet;
								planetManager.DestroySatellite(0);
								rocketManager.SetInitialPosition();
								rocketManager.FullRefill();
								planetManager.SetPlanetType("");
								text.text="Wormhole moves the rocket from a point to another";
								text2.text="and shoots it only right";
								wormhole = Instantiate(Resources.Load("Wormhole")) as GameObject;
								wormhole.name="Wormhole";
								wormhole.transform.position=new Vector3(4,1,0);
								wormhole2 = Instantiate(Resources.Load("Wormhole")) as GameObject;
								wormhole2.name="Wormhole";
								wormhole2.transform.position=new Vector3(4,-1,0);
								wormholeManager = wormhole.GetComponent ("Wormhole") as Wormhole;
								wormhole2Manager = wormhole2.GetComponent ("Wormhole") as Wormhole;
								wormholeManager.SetExit(wormhole2);
								wormhole2Manager.SetExit(wormhole);
								counter++;
							}
							else{
								if(counter==2){
									// planet and exploding planet
									rocketManager.SetInitialPosition();
									rocketManager.FullRefill();
									text.text="The planet on the right is an exploding planet";
									text2.text="";
									planetManager = planet.GetComponent ("Planet") as Planet;
									planetManager.SetText(text2);
									planetManager.SetPlanetType("count");
									planetManager.DestroySatellite(0);
									counter++;
								}
								else{
									if(counter==1){
										// two planets, right one with asteroids
										Destroy(planet);
										planet = Instantiate(Resources.Load("Planet")) as GameObject;
										planet.name="Planet";
										planet.transform.position=new Vector3(8,0,0);
										planetManager = planet.GetComponent ("Planet") as Planet;
										rocketManager.SetInitialPosition();
										rocketManager.FullRefill();
										planetManager.SetPlanetType("");
										text.text="A planet may have satellites in its orbits";
										text2.text="If you hit one of them, you lose";
										counter++;
									}
									else{
										if(counter==0){
											// two planets
											rocketManager.SetInitialPosition();
											rocketManager.FullRefill();
											text.text="Try to shoot the rocket from a planet to another";
											text2.text="";
											initialPosition=new Vector3(4,0,-10);
											Camera.main.transform.position=initialPosition;
											planet = Instantiate(Resources.Load("Planet")) as GameObject;
											planet.name="Planet";
											myCollider = planet.transform.GetComponent<SphereCollider>();
											planet.transform.position=new Vector3(8,0,0);
											planetManager = planet.GetComponent ("Planet") as Planet;
											planetManager.DestroySatellite(0);
											counter++;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}