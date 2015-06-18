using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	
	GameObject rocket;
	Rocket rocketManager;
	// camera vars
	Vector3 initialPosition;
	Vector3 position;
	float camSize;
	float leftBound;
	float upBound;
	float downBound;
	// flags
	bool moving=false;
	bool right=false;
	bool left=false;
	bool up=false;
	bool down=false;

	/* DONE: define how to move in the level
	 * Up and down boundaries will be defined after the generation
	 * Now the boundaries are fixed
	 * especially up and down
	 * eventually left
	 */
	void Start () {
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		camSize = Camera.main.orthographicSize;
		initialPosition=Camera.main.transform.position;
		position = new Vector3(0,0,0);

		// define statics bound, now not used
		SetBound(-2*camSize,0);
		SetBound(camSize*2,1);
		SetBound(-camSize*2,2);
	}
	
	void FixedUpdate () {
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

		if(Mathf.Abs(rocket.transform.position.x-this.transform.position.x)>camSize){
			moving=true;
			// if moving right
			if(rocket.transform.position.x-this.transform.position.x>camSize)
				right=true;
			// if moving left
			else
				left=true;
		}
		if(Mathf.Abs(rocket.transform.position.y-this.transform.position.y)>camSize){
			moving=true;
			// if moving up
			if(rocket.transform.position.y-this.transform.position.y>camSize)
				up=true;
			// if moving down
			else
				down=true;
		}
		if(moving){
			// set destination position
			position=new Vector3(rocket.transform.position.x,rocket.transform.position.y,-10);
			// TODO: optimize. The rocket should always be almost @ center
			// better controls should became the game better
			// we are moving the camera 0.2 every update
			// it seems ok
			if(right){
				this.transform.position = new Vector3(this.transform.position.x+0.2f,this.transform.position.y,-10);
				if(this.transform.position.x-position.x>0){
					right=false;
				}
			}
			if(left){
				this.transform.position = new Vector3(this.transform.position.x-0.2f,this.transform.position.y,-10);
				if(this.transform.position.x-position.x<0){
					left=false;
				}
			}
			if(up){
				this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y+0.2f,-10);
				if(this.transform.position.y-position.y>0){
					up=false;
				}
			}
			if(down){
				this.transform.position = new Vector3(this.transform.position.x,this.transform.position.y-0.2f,-10);
				if(this.transform.position.y-position.y<0){
					down=false;
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
		else{
			return 0;
		}
	}

	void reset(){
		moving=false;
		right=false;
		left=false;
		up=false;
		down=false;
	}
}
