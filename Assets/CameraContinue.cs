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
	public float camSize;
	float leftBound;
	float upBound;
	float downBound;
	float rightBound;
	public float deltaX;
	public float deltaY;
	// flags
	bool moving=false;
	bool right=false;
	bool left=false;
	bool up=false;
	bool down=false;
	bool setPosition=false;
	
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
		//deltaX=4+Camera.main.orthographicSize*3/2;
		//deltaY=4+Camera.main.orthographicSize*3/2;
		deltaX=5*3/2.0f;
		deltaY=5*3/2.0f;
		
		// TODO: define NEW static bound
		/* Bound values
		 * 0 is left
		 * 1 is up
		 * 2 is down
		 * 3 is right
		 */
		SetBound(-camSize*2,0);
		SetBound(camSize*2,1);
		SetBound(-camSize*5,2);
		SetBound(camSize*5,3);
	}
	
	void FixedUpdate () {
		
		text.text="Fuel: "+rocketManager.GetFuel();
		
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
				if(right){
					position=new Vector3(position.x+deltaX,position.y,-10);
					print ("RIGHT: x: "+position.x+" y: "+position.y+" dX: "+deltaX);
				}
				if(up){
					position=new Vector3(position.x,position.y+deltaY,-10);
					print ("UP: x: "+position.x+" y: "+position.y+" dY: "+deltaY);
				}
				if(down){
					position=new Vector3(position.x,position.y-deltaY,-10);
					print ("DOWN: x: "+position.x+" y: "+position.y+" dY: "+deltaY);
				}
				if(left){
					position=new Vector3(position.x-deltaX,position.y,-10);
					print ("LEFT: x: "+position.x+" y: "+position.y+" dX: "+deltaX);
				}
				setPosition=false;
			}
			
			// TODO: optimize. The rocket should always be almost @ center
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
	
	/* TYPE values
	 * 0 is left
	 * 1 is up
	 * 2 is down
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
	
	/* TYPE values
	 * 0 is left
	 * 1 is up
	 * 2 is down
	 */
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
	
	public void SetInitialPosition(Vector3 pos){
		initialPosition=pos;
		print ("INI: x "+initialPosition.x+" y "+initialPosition.y);
	}
	
	public void SetThisAsInitialPosition(){
		initialPosition=this.transform.position;
		print ("INI: x "+initialPosition.x+" y "+initialPosition.y);
	}

	public void ResetPosition(){
		this.transform.position=initialPosition;
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
	}
}
