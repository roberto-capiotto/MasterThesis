using UnityEngine;
using UnityEngine.UI;
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
	Vector3 lastPosition;
	Vector3 startingCorner;
	GameObject startPlanet,endPlanet;
	SphereCollider myCollider;
	float deltaLevel;
	public CameraContinue myCamera;
	bool scrollCamera=false;
	public int level=1;
	public bool[] path;
	float maxFlyTime=10;
	public Button closeButton;
	public Button retryButton;
	public Button shootButton;
	float upBound;
	float downBound;
	float rightBound;
	GameObject[] planets = new GameObject[10];
	GameObject[] asteroids = new GameObject[12];
	bool slide=false;
	bool addOnce=false;
	int k;

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
			
			// move Camera
			print ("dx "+myCamera.GetDeltaX()+" dy "+myCamera.GetDeltaY());
			camPosition=new Vector3(endPlanet.transform.position.x+myCamera.GetDeltaX(),endPlanet.transform.position.y,-10);
			myCamera.SetPost(camPosition.x+myCamera.GetDeltaX());
			// THIS IS THE LIMIT OF THIS LEVEL
			camPosition=new Vector3(lastPosition.x+2*myCamera.GetDeltaX(),camPosition.y,-10);
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
				/*if(myCamera.transform.position.x<camPosition.x){
						myCamera.transform.position = new Vector3(myCamera.transform.position.x+0.1f,myCamera.transform.position.y,-10);
					}*/
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
		endRand=Random.Range(0,5);
		print ("endRAND: "+endRand);
		planet.transform.position=new Vector3(pos.x+(level-1)*4*(i+1)+(i+1)*camSize*3/2,pos.y-(level-1)*4*endRand-endRand*camSize*3/2,0);
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
		BuildPath();
		level++;
		myCamera.SetLevel(level);
		return planet;
	}

	void BuildPath(){
		path = new bool[9];
		int i;
		k=0;
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

		// straight connection
		if(path[0] && path[6])
			path[3]=true;
		if(path[1] && path[7])
			path[4]=true;
		if(path[2] && path[8])
			path[5]=true;
		// multiple connection
		if(path[0] && path[7]){
			path[3]=true;
			path[4]=true;
		}
		if(path[1] && path[6]){
			path[3]=true;
			path[4]=true;
		}
		if(path[1] && path[8]){
			path[4]=true;
			path[5]=true;
		}
		if(path[2] && path[7]){
			path[4]=true;
			path[5]=true;
		}
		if(path[0] && path[8]){
			path[3]=true;
			path[4]=true;
			path[5]=true;
		}
		if(path[2] && path[6]){
			path[3]=true;
			path[4]=true;
			path[5]=true;
		}

		// up couple
		if(!(path[0] && path[1])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[1].transform.position.x+planets[0].transform.position.x)/2;
			y=(planets[1].transform.position.y+planets[0].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4+camSize*3/2;
			//y=startingCorner.y-(level-1)*4*(3/2.0f)-(3/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}
		if(!(path[3] && path[4])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[4].transform.position.x+planets[3].transform.position.x)/2;
			y=(planets[4].transform.position.y+planets[3].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*2+2*camSize*3/2;
			//y=startingCorner.y-(level-1)*4*(3/2.0f)-(3/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}
		if(!(path[6] && path[7])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[7].transform.position.x+planets[6].transform.position.x)/2;
			y=(planets[7].transform.position.y+planets[6].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*3+3*camSize*3/2;
			//y=startingCorner.y-(level-1)*4*(3/2.0f)-(3/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}

		// down couple
		if(!(path[1] && path[2])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[2].transform.position.x+planets[1].transform.position.x)/2;
			y=(planets[2].transform.position.y+planets[1].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4+camSize*3/2;
			//y=startingCorner.y-(level-1)*4*(5/2.0f)-(5/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}
		if(!(path[4] && path[5])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[5].transform.position.x+planets[4].transform.position.x)/2;
			y=(planets[5].transform.position.y+planets[4].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*2+2*camSize*3/2;
			//y=startingCorner.y-(level-1)*4*(5/2.0f)-(5/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}
		if(!(path[7] && path[8])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[8].transform.position.x+planets[7].transform.position.x)/2;
			y=(planets[8].transform.position.y+planets[7].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*3+3*camSize*3/2;
			//y=startingCorner.y-(level-1)*4*(5/2.0f)-(5/2.0f)*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}

		// left couple
		if(!(path[0] && path[3])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[3].transform.position.x+planets[0].transform.position.x)/2;
			y=(planets[3].transform.position.y+planets[0].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*(3/2.0f)+(3/2.0f)*camSize*3/2;
			//y=startingCorner.y-(level-1)*4-camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}
		if(!(path[1] && path[4])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[4].transform.position.x+planets[3].transform.position.x)/2;
			y=(planets[4].transform.position.y+planets[3].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*(3/2.0f)+(3/2.0f)*camSize*3/2;
			//y=startingCorner.y-(level-1)*4*2-2*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}
		if(!(path[2] && path[5])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[5].transform.position.x+planets[2].transform.position.x)/2;
			y=(planets[5].transform.position.y+planets[2].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*(3/2.0f)+(3/2.0f)*camSize*3/2;
			//y=startingCorner.y-(level-1)*4*3-3*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}

		// right couple
		if(!(path[3] && path[6])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[6].transform.position.x+planets[3].transform.position.x)/2;
			y=(planets[6].transform.position.y+planets[3].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*(5/2.0f)+(5/2.0f)*camSize*3/2;
			//y=startingCorner.y-(level-1)*4-camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}
		if(!(path[4] && path[7])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[7].transform.position.x+planets[4].transform.position.x)/2;
			y=(planets[7].transform.position.y+planets[4].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*(5/2.0f)+(5/2.0f)*camSize*3/2;
			//y=startingCorner.y-(level-1)*4*2-2*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}
		if(!(path[5] && path[8])){
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			x=(planets[8].transform.position.x+planets[5].transform.position.x)/2;
			y=(planets[8].transform.position.y+planets[5].transform.position.y)/2;
			//x=startingCorner.x+(level-1)*4*(5/2.0f)+(5/2.0f)*camSize*3/2;
			//y=startingCorner.y-(level-1)*4*3-3*camSize*3/2;
			asteroid.transform.position=new Vector3(x,y,0);
			asteroids[k]=asteroid;
			k++;
		}	
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
		for(int j=0;j<k;j++){
			Destroy(asteroids[j]);
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