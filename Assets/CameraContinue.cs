using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraContinue : MonoBehaviour {
	
	GameObject rocket;
	Rocket rocketManager;
	public Text text;
	// camera vars
	public Vector3 initialPosition;
	Vector3 position;
	public Vector3 limit = new Vector3(-10,0,0);
	public float post;
	public float camSize;
	public float leftBound;
	public float upBound;
	public float downBound;
	public float rightBound;
	public float deltaX;
	public float deltaY;
	public int level=1;
	public int curLevel=1;
	public Vector3[] cameraStep = new Vector3[20];
	// flags
	bool moving=false;
	bool right=false;
	bool left=false;
	bool up=false;
	bool down=false;
	bool setPosition=false;
	bool showFuel=false;
	
	/* DONE: define how to move in the level
	 * We have a predefined schema with predefined dimensions;
	 * startPlanet on left, endPlanet on right
	 * Up and down boundaries are fixed. We don't want to reach other levels
	 */
	void Start () {

		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		camSize = Camera.main.orthographicSize;
		initialPosition=Camera.main.transform.position;
		position = new Vector3(0,0,0);
		deltaX=5*3/2.0f;
		deltaY=5*3/2.0f;

		/* Initial Boundaries value
		 * 0 is left
		 * 1 is up
		 * 2 is down
		 * 3 is right
		 */
		SetBound(-camSize,0);
		SetBound(camSize*2,1);
		SetBound(-camSize*5,2);
		SetBound(camSize*7,3);

		// place fuelText
		Vector3 coord = Vector3.zero;
		coord.x = this.transform.position.x+Screen.width/2-50;
		coord.y = this.transform.position.y+Screen.height/2-50;
		text.rectTransform.localPosition = coord;
	}
	
	void FixedUpdate () {

		if(showFuel)
			text.text="Fuel: "+rocketManager.GetFuel();

		camSize = Camera.main.orthographicSize;
		// upper bound and lower bound
		if(rocket.transform.position.y>GetBound(1) || rocket.transform.position.y<GetBound(2)){
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			this.transform.position=initialPosition;
			reset();
		}
		
		/* left bound
		 * Once generated the first (starting) planet, the others will be all on right side
		 */
		if(rocket.transform.position.x<GetBound(0)){
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			this.transform.position=initialPosition;
			reset();
		}
		
		/* right bound
		 */
		if(rocket.transform.position.x>GetBound(3)){
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			this.transform.position=initialPosition;
			reset();
		}

		if(curLevel==level){
			/* CONTINUE MOVING */
			if(!rocketManager.onStart){
				if(Mathf.Abs(rocket.transform.position.x-this.transform.position.x)>deltaX/2){
					if(!moving)
						setPosition=true;
					moving=true;
					// if moving right
					if(rocket.transform.position.x-this.transform.position.x>deltaX/2)
						right=true;
					// if moving left
					else
						left=true;
				}
				if(Mathf.Abs(rocket.transform.position.y-this.transform.position.y)>deltaY/2){
					if(!moving)
						setPosition=true;
					moving=true;
					// if moving up
					if(rocket.transform.position.y-this.transform.position.y>deltaY/2)
						up=true;
					// if moving down
					else
						down=true;
				}
			}
		}

		/* OUT OF SCREEN*/
		if(Mathf.Abs(rocket.transform.position.x-this.transform.position.x)>camSize*16/10){
			if(!moving)
				setPosition=true;
			moving=true;
			// if moving right
			if(rocket.transform.position.x-this.transform.position.x>camSize*16/10)
				right=true;
			// if moving left
			else
				left=true;
		}
		if(Mathf.Abs(rocket.transform.position.y-this.transform.position.y)>camSize){
			if(!moving)
				setPosition=true;
			moving=true;
			// if moving up
			if(rocket.transform.position.y-this.transform.position.y>camSize)
				up=true;
			// if moving down
			else
				down=true;
		}
		if(moving){
			if(setPosition){
				// set destination position
				position=this.transform.position;
				print ("BEFORE: x: "+position.x+" y: "+position.y);
				if(up){
					position=new Vector3(position.x,position.y+deltaY,-10);
					print ("UP: x: "+position.x+" y: "+position.y+" dY: "+deltaY);
				}
				if(down){
					position=new Vector3(position.x,position.y-deltaY,-10);
					print ("DOWN: x: "+position.x+" y: "+position.y+" dY: "+deltaY);
				}
				if(right){
					if(curLevel!=level){
						if(curLevel==level-2){
							curLevel=level;
							position.x=initialPosition.x;
							if(Mathf.Abs (initialPosition.y-this.transform.position.y)<deltaY/2)
								position.y = initialPosition.y;
							else{
								if(this.transform.position.y>initialPosition.y){
									if(Mathf.Abs (initialPosition.y+deltaY-this.transform.position.y)<deltaY/2){
										position.y = initialPosition.y;
									}
									// CHECK x2
								}
								else{
									if(Mathf.Abs (initialPosition.y-deltaY-this.transform.position.y)<deltaY/2){
										position.y = initialPosition.y;
									}
									// CHECK x2
								}
							}
							print ("special move right");
							if(position.y>this.transform.position.y)
								up=true;
							else
								if(position.y<this.transform.position.y)
									down=true;
						}
						else{
							curLevel++;
							position=cameraStep[curLevel-1];
							if(position.y>this.transform.position.y)
								up=true;
							else
								if(position.y<this.transform.position.y)
									down=true;
						}
					}
					else{
						if(position.x==initialPosition.x && camSize!=9){
							position=new Vector3(GetPost(),position.y,-10);
							print ("GETPOST: "+GetPost());
						}
						else
							position=new Vector3(position.x+deltaX,position.y,-10);
					}
					print ("RIGHT: x: "+position.x+" y: "+position.y+" dX: "+deltaX);
				}
				if(left){
					//if(rocket.transform.position.x<GetPost()-2*deltaX && !rocketManager.onStart){
					if(rocket.transform.position.x<GetLimit().x-deltaX && !rocketManager.onStart){
						if(curLevel==level)
							curLevel=level-2;
						else{
							if(curLevel>1)
								curLevel--;
						}
						position=cameraStep[curLevel-1];
						print ("special move left");
						if(position.y>this.transform.position.y)
							up=true;
						else
							if(position.y<this.transform.position.y)
								down=true;
					}
					else{
						if(position.x==initialPosition.x && camSize!=9)
							left=false;
						else{
							position=new Vector3(position.x-deltaX,position.y,-10);
							if(position.x<GetPost())
								position.x=initialPosition.x;
						}
					}
					print ("LEFT: x: "+position.x+" y: "+position.y+" dX: "+deltaX);
				}
				setPosition=false;
			}
			
			// The rocket should always be almost @ center
			// we are moving the camera 0.3 every update
			// it seems fluent
			if(right){
				this.transform.position = new Vector3(this.transform.position.x+0.3f,this.transform.position.y,-10);
				if(this.transform.position.x-position.x>=0){
					right=false;
					this.transform.position=new Vector3(position.x,this.transform.position.y,-10);
				}
			}
			if(left){
				this.transform.position = new Vector3(this.transform.position.x-0.3f,this.transform.position.y,-10);
				if(this.transform.position.x-position.x<=0){
					left=false;
					this.transform.position=new Vector3(position.x,this.transform.position.y,-10);
				}
			}
			if(up){
				this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y+0.3f,-10);
				if(this.transform.position.y-position.y>=0){
					up=false;
					this.transform.position=new Vector3(this.transform.position.x,position.y,-10);
				}
			}
			if(down){
				this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y-0.3f,-10);
				if(this.transform.position.y-position.y<=0){
					down=false;
					this.transform.position=new Vector3(this.transform.position.x,position.y,-10);
				}
			}
			if(!up && !down && !left && !right )
				moving=false;
		}
	}

	/************************************************************************
	 *  CAMERA POSITION METHODS
	 ************************************************************************/

	public void SetInitialPosition(Vector3 pos){
		initialPosition=pos;
		print ("INI: x "+initialPosition.x+" y "+initialPosition.y);
	}

	public Vector3 GetInitialPosition(){
		return initialPosition;
	}
	
	public void SetThisAsInitialPosition(){
		if(moving){
			initialPosition=position;
		}
		else{
			initialPosition=this.transform.position;
		}
		print ("INI: x "+initialPosition.x+" y "+initialPosition.y);
	}

	public void ResetPosition(){
		this.transform.position=initialPosition;
		reset();
	}

	public void SetOrthographicSize(float size){
		Camera.main.orthographicSize=size;
	}

	public void SetDeltas(float x,float y){
		deltaX=x;
		deltaY=y;
	}
	
	public float GetDeltaX(){
		return deltaX;
	}
	
	public float GetDeltaY(){
		return deltaY;
	}

	void reset(){
		moving=false;
		right=false;
		left=false;
		up=false;
		down=false;
		curLevel=level;
	}

	/************************************************************************
	 *  BOUNDARIES METHODS
	 ************************************************************************/
	
	/* TYPE values
	 * 0 is left
	 * 1 is up
	 * 2 is down
	 * 3 is right
	 */

	public void SetBound(float bound,int type){
		if(type==0){
			leftBound=bound;
		}
		if(type==1){
			upBound=bound;
		}
		if(type==2){
			downBound=bound;
		}
		if(type==3){
			rightBound=bound;
		}
	}

	public float GetBound(int type){
		if(type==0){
			return leftBound;
		}
		if(type==1){
			return upBound;
		}
		if(type==2){
			return downBound;
		}
		if(type==3){
			return rightBound;
		}
		else{
			return 0;
		}
	}

	// HORIZONTAL METHODS

	public void SetLimit(Vector3 l){
		limit=l;
	}

	public Vector3 GetLimit(){
		return limit;
	}

	public void SetPost(float p){
		post=p;
	}
	
	public float GetPost(){
		return post;
	}

	/************************************************************************
	 *  GAME LEVEL METHODS
	 ************************************************************************/
	
	public void SetLevel(int lvl){
		level=lvl;
		curLevel=lvl;
	}

	public int GetLevel(){
		return level;
	}

	public void SetCurLevel(int lvl){
		curLevel=lvl;
	}

	public int GetCurLevel(){
		return curLevel;
	}

	/************************************************************************
	 *  CAMERA STEP METHODS
	 ************************************************************************/
	
	public void AddCameraStep(Vector3 step){
		cameraStep[level-1] = new Vector3(step.x,step.y,-10);
	}

	public Vector3 GetCameraStep(int i){
		return cameraStep[i];
	}

	public Vector3 GetNextStep(float x){
		for(int i=0;i<20;i++)
			if(cameraStep[i].x==x)
				return cameraStep[i+1];
		return Vector3.zero;
	}

	public bool IsBackStep(float x){
		for(int i=0;i<20;i++)
			if(cameraStep[i].x==x)
				return true;
		return false;
	}

	public void RemoveLastStep(){
		cameraStep[level-1] = Vector3.zero;
	}

	public void RemoveCameraStep(){
		for(int i=0;i<20;i++)
			cameraStep[i]=Vector3.zero;
	}

	/************************************************************************
	 *  FUEL METHODS
	 ************************************************************************/
	
	public void NotShowFuelText(){
		showFuel=false;
		text.text="";
	}

	public void ShowFuelText(){
		showFuel=true;
	}
	
}
