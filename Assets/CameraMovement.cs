using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	float camSize;
	GameObject rocket;
	Vector3 position;
	Rocket rocketManager;
	bool moving=false;
	bool right=false;
	bool left=false;
	float leftBound;
	float upBound;
	float downBound;

	// DONE: define how to move in the level
	// we have to define boundaries?
	// especially up and down
	// eventually left
	void Start () {
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		camSize = Camera.mainCamera.orthographicSize;
		position = new Vector3(0,0,0);

		// define statics bound, now not used
		SetBound(-camSize,0);
		SetBound(camSize,1);
		SetBound(-camSize,2);
	}
	
	void Update () {
		// upper bound and lower bound
		if(Mathf.Abs(rocket.transform.position.y-position.y)>camSize){
			rocketManager.SetInitialPosition();
		}
		// left bound
		// I suppose the level will be totally on right side
		// It works because the starting point is in (0,0,0)
		// if it changes, also this control will be changed
		if(rocket.transform.position.x<-2*camSize){
			rocketManager.SetInitialPosition();
		}

		if(!moving){
			if(Mathf.Abs(rocket.transform.position.x-position.x)>camSize){
				moving=true;
				// if moving right
				if(rocket.transform.position.x-position.x>camSize)
					right=true;
				// if moving left
				else
					left=true;
				// set destination position
				position=new Vector3(rocket.transform.position.x,rocket.transform.position.y,-10);
			}
		}
		else{
			// TODO: optimize. The rocket should always be almost @ center
			// better controls should became the game better
			// we are moving the camera 0.1 every update
			// it seems ok
			if(right){
				this.transform.position = new Vector3(this.transform.position.x+0.1f,this.transform.position.y,-10);
				if(this.transform.position.x-position.x>0){
					right=false;
					moving=false;
				}
			}
			else{
				this.transform.position = new Vector3(this.transform.position.x-0.1f,this.transform.position.y,-10);
				if(this.transform.position.x-position.x<0){
					left=false;
					moving=false;
				}
			}
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
	}
}
