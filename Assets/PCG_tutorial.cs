using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PCG_tutorial : MonoBehaviour {
	
	int counter=0;
	GameObject rocket,firstPlanet,planet,planet2,wormhole,wormhole2,swing,blackHole,asteroid,asteroid2,asteroid3,asteroid4,dockingStation,satellite,satellite2;
	GameObject endPlanet,cam;
	Rocket rocketManager;
	Planet planetManager,planetManager2;
	Wormhole wormholeManager,wormhole2Manager;
	Vector3 initialPosition,rocketInitialPosition,absoluteInitialPosition;
	public Text text;
	public Text text2;
	bool fuel=false;
	bool exe=false;
	public bool gen=false;
	bool scroll=false;
	bool scrollCamera=false;
	Vector3 camPosition;
	Vector3 startingCorner;
	Vector3 newPosition;
	Vector3 endPosition;
	Vector3 lastPosition;
	float x,y,rand,randX,randY,camSize=5;
	float postX;
	public int level=1;
	public CameraContinue myCamera;
	Camera myCam;
	SphereCollider myCollider;
	GameObject[] list;
	int cont=0;
	float maxFlyTime=10;
	bool locked=true;
	public Button continueButton;
	public Button closeButton;
	public Button retryButton;
	float upBound;
	float downBound;
	float rightBound;
	GameObject[] planets = new GameObject[10];
	bool slide=false;
	bool addOnce=false;
	Vector3 coord = Vector3.zero;
	
	void Start () {
		cam=GameObject.Find("Main Camera");
		myCam=cam.GetComponent<Camera>();
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		firstPlanet = GameObject.Find ("Planet");
		planetManager = firstPlanet.GetComponent ("Planet") as Planet;
		planetManager.SetPlanetType("first");
		planetManager.DestroySatellite(0);
		myCollider = firstPlanet.transform.GetComponent<SphereCollider>();
		//camSize = Camera.main.orthographicSize;
		initialPosition=Camera.main.transform.position;
		myCamera.SetInitialPosition(initialPosition);
		rocketInitialPosition=rocketManager.GetInitialPosition();
		absoluteInitialPosition=rocketInitialPosition;
		list = new GameObject[100];
		/* TODO: BOUNDARY IS A HELL
		left=firstPlanet.transform.position.x-camSize*16/10;
		up=firstPlanet.transform.position.x-camSize;
		right=firstPlanet.transform.position.x+camSize*16/10;
		down=firstPlanet.transform.position.x+camSize;*/
		coord.x = -Screen.width/2+80;
		coord.y = -Screen.height/2+20;
		continueButton.GetComponent<RectTransform>().localPosition = coord;

		coord.x = this.transform.position.x;
		coord.y = this.transform.position.y+Screen.height/3;
		text.rectTransform.localPosition = coord;

		coord.y = this.transform.position.y-Screen.height/3;
		text2.rectTransform.localPosition = coord;

		coord.x = Screen.width/2-80;
		coord.y = Screen.height/2-20;
		closeButton.GetComponent<RectTransform>().localPosition = coord;

		coord.x = Screen.width;
		coord.y = -Screen.height;
		retryButton.GetComponent<RectTransform>().localPosition = coord;
	}

	Vector3 WorldCoordinate (Vector3 mouseCoordinates)
	{
		Camera.main.ScreenToWorldPoint (mouseCoordinates);
		return Camera.main.ScreenToWorldPoint (mouseCoordinates);
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
		
		camSize = Camera.main.orthographicSize;
		if(locked){
			/* OUT OF SCREEN*/
			if(Mathf.Abs(rocket.transform.position.x-Camera.main.transform.position.x)>camSize*16/10 ||
			   Mathf.Abs(rocket.transform.position.y-Camera.main.transform.position.y)>camSize){
				rocketManager.SetInitialPosition();
				rocketManager.FullRefill();
				myCamera.ResetPosition();
				scroll=false;
				if(counter==1 && !gen){
					text2.text="";
				}
				//Camera.main.transform.position=initialPosition;
			}
		}
		
		if(fuel){
			text2.text="Fuel remaining: "+rocketManager.GetFuel();
		}

		/************************************************************************
		 *  TUTORIAL WITHOUT SATELLITES
		 ************************************************************************/
		if(counter==1 && !exe){
			if(planetManager.GetCollision()){
				exe=true;
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.name="Planet";
				planet.transform.position=new Vector3(16,0,0);
				planetManager2 = planet.GetComponent ("Planet") as Planet;
				planetManager2.SetPlanetType("first");
				planetManager2.DestroySatellite(0);
				list[cont++]=planet;
				scroll=true;
				camPosition=new Vector3(12,0,-10);
				text2.text="Again!";
			}
		}
		else{
			if(counter==1){
				if(planetManager.GetCollision() && !gen){
					scroll=true;
					camPosition=new Vector3(12,0,-10);
					text2.text="Again!";
				}
				if(planetManager2.GetCollision() && !gen){
					gen=true;
					locked=false;
					rocketManager.ChangeInitialPosition(new Vector3(planet.transform.position.x,planet.transform.position.y+planet.transform.localScale.y/2+myCollider.radius,0));
					startingCorner=planet.transform.position;
					endPlanet=GenerateLevel(startingCorner,"free");
					(cam.GetComponent( "CameraContinue" ) as MonoBehaviour).enabled = true;
					Camera.main.orthographicSize=9;
					myCamera.transform.position=new Vector3(startingCorner.x+5*3/2.0f,startingCorner.y-5*3/2.0f,-10);
					myCamera.SetInitialPosition(myCamera.transform.position);
					myCamera.ShowFuelText();
					rocketManager.FullRefill();
					initialPosition=myCamera.transform.position;
					myCamera.SetBound(startingCorner.x-camSize/2,0);
					print ("BOUND LEFT: "+myCamera.GetBound(0));
					myCamera.SetBound(camSize*2,1);
					myCamera.SetBound(-camSize*5,2);
					myCamera.SetBound(camSize*7,3);
					myCamera.SetPost(initialPosition.x+myCamera.GetDeltaX());
					text.text="Play a level";
					text2.text="";
					coord.x = -Screen.width/2+80;
					coord.y = Screen.height/2-20;
					retryButton.GetComponent<RectTransform>().localPosition = coord;
				}
				if(planetManager.levelCompleted){
					print ("COMPLETE");
					text.text="";
					planetManager.levelCompleted=false;
					startingCorner=new Vector3(endPosition.x,endPosition.y+(level-1)*4*rand+rand*5*3/2,0);
					
					// modify Deltas
					myCam.orthographicSize=myCam.orthographicSize+4;
					myCamera.SetDeltas(myCamera.GetDeltaX()+4,myCamera.GetDeltaY()+4);
					print ("dx "+myCamera.GetDeltaX()+" dy "+myCamera.GetDeltaY());
					
					// move Camera
					camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y,-10);
					postX=camPosition.x+myCamera.GetDeltaX();
					print ("PostX: "+postX);
					myCamera.SetPost(postX);
					// THIS IS THE LIMIT OF THIS LEVEL
					camPosition=new Vector3(lastPosition.x+2*myCamera.GetDeltaX(),camPosition.y,-10);
					myCamera.SetLimit(camPosition);
					print ("camX: "+camPosition.x +" lastX: "+ lastPosition.x +" DX: "+ myCamera.GetDeltaX());

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
		}

		/************************************************************************
		 *  TUTORIAL WITH SATELLITES
		 ************************************************************************/
		if(counter==2 && !exe){
			if(planetManager.GetCollision()){
				exe=true;
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.name="Planet";
				planet.transform.position=new Vector3(16,0,0);
				planetManager2 = planet.GetComponent ("Planet") as Planet;
				planetManager2.SetPlanetType("first");
				planetManager2.DestroySatellite(0);
				list[cont++]=planet;
				scroll=true;
				camPosition=new Vector3(12,0,-10);
				myCamera.SetBound(planet.transform.position.x-camSize/2,0);
				myCamera.SetBound(camSize*2,1);
				myCamera.SetBound(-camSize*5,2);
				myCamera.SetBound(camSize*7,3);
			}
		}
		else{
			if(counter==2){
				if(planetManager.GetCollision() && !gen){
					scroll=true;
					camPosition=new Vector3(12,0,-10);
				}
				if(planetManager2.GetCollision() && !gen){
					gen=true;
					locked=false;
					rocketManager.ChangeInitialPosition(new Vector3(planet.transform.position.x,planet.transform.position.y+planet.transform.localScale.y/2+myCollider.radius,0));
					startingCorner=planet.transform.position;
					endPlanet=GenerateLevel(startingCorner,"with");
					(cam.GetComponent( "CameraContinue" ) as MonoBehaviour).enabled = true;
					Camera.main.orthographicSize=9;
					// RESET DELTAS
					myCamera.SetDeltas(5*3/2.0f,5*3/2.0f);
					myCamera.transform.position=new Vector3(startingCorner.x+5*3/2.0f,startingCorner.y-5*3/2.0f,-10);
					myCamera.SetInitialPosition(myCamera.transform.position);
					myCamera.ShowFuelText();
					rocketManager.FullRefill();
					initialPosition=myCamera.transform.position;
					myCamera.SetPost(initialPosition.x+myCamera.GetDeltaX());
					text.text="Play a level";
					text2.text="";
					coord.x = -Screen.width/2+80;
					coord.y = Screen.height/2-20;
					retryButton.GetComponent<RectTransform>().localPosition = coord;
				}
				if(planetManager.levelCompleted){
					print ("COMPLETE");
					text.text="";
					planetManager.levelCompleted=false;
					startingCorner=new Vector3(endPosition.x,endPosition.y+(level-1)*4*rand+rand*5*3/2,0);
					
					// modify Deltas
					myCam.orthographicSize=myCam.orthographicSize+4;
					myCamera.SetDeltas(myCamera.GetDeltaX()+4,myCamera.GetDeltaY()+4);
					
					// move Camera
					print ("dx "+myCamera.GetDeltaX()+" dy "+myCamera.GetDeltaY());
					camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y,-10);
					postX=camPosition.x+myCamera.GetDeltaX();
					myCamera.SetPost(postX);
					// THIS IS THE LIMIT OF THIS LEVEL
					camPosition=new Vector3(lastPosition.x+2*myCamera.GetDeltaX(),camPosition.y,-10);
					myCamera.SetLimit(camPosition);
					print ("camX: "+camPosition.x +" lastX: "+ lastPosition.x +" DX: "+ myCamera.GetDeltaX());
					
					myCamera.SetInitialPosition(camPosition);
					scrollCamera=true;
					
					// set new rocket initialPosition
					rocketInitialPosition=new Vector3
						(endPlanet.transform.position.x ,endPlanet.transform.position.y+(endPlanet.transform.localScale.y/2)+myCollider.radius,0);
					rocketManager.ChangeInitialPosition(rocketInitialPosition);
					rocketManager.onStart=true;
					
					// Generate New Level
					endPlanet=GenerateLevel(startingCorner,"with");
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
		}
		if(counter==3){
			if(planetManager.counter==0){
				text2.text="EXPLODED!!";
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

		lastPosition=new Vector3(0,0,0);

		int i=0,j=0;
		for(;i<3;i++){
			for(j=0;j<3;j++){
				randX=Random.Range(-1.3f,1.3f);
				randY=Random.Range(-1.3f,1.3f);
				print("randX: "+randX+" randY: "+randY);
				x=pos.x+(level-1)*4*(i+1)+(i+1)*5*3/2+randX*level;
				y=pos.y-(level-1)*4*(j+1)-(j+1)*5*3/2+randY*level;
				newPosition=new Vector3(x,y,0);
				planet = Instantiate(Resources.Load("Planet")) as GameObject;
				planet.transform.position= newPosition;
				planet.name="Planet";
				planets[i*3+j]=planet;
				planetManager = planet.GetComponent ("Planet") as Planet;
				list[cont++]=planet;
				if(i==2){
					if(planet.transform.position.x>lastPosition.x)
						lastPosition=planet.transform.position;
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
				if(s.Equals("free"))
					planetManager.DestroySatellite(0);
				else
					planetManager.DestroySatellite(Random.Range(0,4));
			}
		}
		//lastPosition = new Vector3(lastPosition.x+5,lastPosition.y,0);

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
		planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*5*3/2,pos.y-(level-1)*4*rand-rand*5*3/2,0);
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

		list[cont++]=planet;
		endPosition=planet.transform.position;
		level++;
		myCamera.SetLevel(level);
		return planet;
	}

	public void Retry(){
		rocketManager.SetInitialPosition();
		for(int l=0;l<10;l++){
			Destroy(planets[l]);
		}
		level--;
		if(counter==1)
			endPlanet=GenerateLevel(startingCorner,"free");
		else
			endPlanet=GenerateLevel(startingCorner,"with");
		myCamera.RemoveLastStep();
	}

	public void Close(){
		Application.LoadLevel("mainMenu");
	}

	public void Change(){
		if(counter==9){
			Application.LoadLevel("mainMenu");
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
			myCamera.SetThisAsInitialPosition();
			text.text="This is the end of the tutorial";
			text2.text="Now you are ready to play";
			counter++;
			// place button out of bound
			Vector3 coord = Vector3.zero;
			coord.x = myCamera.transform.position.x-Screen.width;
			coord.y = myCamera.transform.position.y-Screen.height;
			continueButton.GetComponent<RectTransform>().localPosition = coord;
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
					planet.transform.position=new Vector3(8,0,0);
					Destroy(planet2);
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
						planet.transform.position=new Vector3(9.5f,2,0);
						planetManager = planet.GetComponent ("Planet") as Planet;
						planetManager.DestroySatellite(0);
						planet2 = Instantiate(Resources.Load("Planet")) as GameObject;
						planet2.transform.position=new Vector3(6,-2.5f,0);
						planetManager2 = planet2.GetComponent ("Planet") as Planet;
						planetManager2.DestroySatellite(0);
						asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
						asteroid.name="Asteroid";
						asteroid.transform.position=new Vector3(3,0,0);
						asteroid2 = Instantiate(Resources.Load("Asteroid")) as GameObject;
						asteroid2.name="Asteroid";
						asteroid2.transform.position=new Vector3(2,-3,0);
						asteroid3 = Instantiate(Resources.Load("Asteroid")) as GameObject;
						asteroid3.name="Asteroid";
						asteroid3.transform.position=new Vector3(9,-1,0);
						asteroid4 = Instantiate(Resources.Load("Asteroid")) as GameObject;
						asteroid4.name="Asteroid";
						asteroid4.transform.position=new Vector3(-3,-3,0);
						blackHole = Instantiate(Resources.Load("BlackHole")) as GameObject;
						blackHole.name="BlackHole";
						blackHole.transform.position=new Vector3(5,2,0);
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
									print ("PLANET: "+cont);
									for(int i=0;i<cont;i++){
										Destroy(list[i]);
									}
									for(int i=0;i<cont;i++){
										list[i]=null;
									}
									planet = Instantiate(Resources.Load("Planet")) as GameObject;
									planet.name="Planet";
									planet.transform.position=new Vector3(8,0,0);
									planetManager = planet.GetComponent ("Planet") as Planet;
									planetManager.SetText(text2);
									planetManager.SetPlanetType("count");
									planetManager.DestroySatellite(0);
									exe=false;
									gen=false;
									locked=true;
									myCamera.SetBound(-camSize,0);
									rocketManager.ChangeInitialPosition(absoluteInitialPosition);
									rocketManager.SetInitialPosition();
									rocketManager.FullRefill();
									text.text="The planet on the right is an exploding planet";
									text2.text="";
									initialPosition=new Vector3(4,0,-10);
									myCamera.SetInitialPosition(initialPosition);
									(cam.GetComponent( "CameraContinue" ) as MonoBehaviour).enabled = false;
									Camera.main.transform.position=initialPosition;
									Camera.main.orthographicSize=5;
									myCamera.NotShowFuelText();
									coord.x = Screen.width;
									coord.y = -Screen.height;
									retryButton.GetComponent<RectTransform>().localPosition = coord;
									counter++;
								}
								else{
									if(counter==1){
										// two planets, right one with asteroids
										print ("PLANET: "+cont);
										for(int i=0;i<cont;i++){
											Destroy(list[i]);
										}
										for(int i=0;i<cont;i++){
											list[i]=null;
										}
										exe=false;
										gen=false;
										locked=true;
										level=1;
										myCamera.SetLevel(level);
										myCamera.RemoveCameraStep();
										planet = Instantiate(Resources.Load("Planet")) as GameObject;
										planet.name="Planet";
										planet.transform.position=new Vector3(8,0,0);
										planetManager = planet.GetComponent ("Planet") as Planet;
										planetManager.SetPlanetType("");
										list[cont++]=planet;
										rocketManager.ChangeInitialPosition(absoluteInitialPosition);
										rocketManager.SetInitialPosition();
										rocketManager.FullRefill();
										text.text="A planet may have satellites in its orbits";
										text2.text="If you hit one of them, you lose";
										initialPosition=new Vector3(4,0,-10);
										myCamera.SetInitialPosition(initialPosition);
										(cam.GetComponent( "CameraContinue" ) as MonoBehaviour).enabled = false;
										Camera.main.transform.position=initialPosition;
										Camera.main.orthographicSize=5;
										myCamera.NotShowFuelText();
										coord.x = Screen.width;
										coord.y = -Screen.height;
										retryButton.GetComponent<RectTransform>().localPosition = coord;
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
											myCamera.SetInitialPosition(initialPosition);
											myCamera.NotShowFuelText();
											planet = Instantiate(Resources.Load("Planet")) as GameObject;
											planet.name="Planet";
											planet.transform.position=new Vector3(8,0,0);
											list[cont++]=planet;
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
