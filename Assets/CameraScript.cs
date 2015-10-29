using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraScript : MonoBehaviour {

	GameObject rocket;
	Rocket rocketManager;
	public Text text;
	// camera vars
	public Vector3 initialPosition;
	Vector3 position;
	Vector3 limit;
	public float camSize;
	float leftBound;
	float upBound;
	float downBound;
	float rightBound;
	float deltaX;
	float deltaY;
	bool showFuel=false;

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
		SetBound(-camSize*2,0);
		SetBound(camSize*2,1);
		SetBound(-camSize*5,2);
		SetBound(camSize*5,3);

		Vector3 coord = Vector3.zero;
		coord.x = this.transform.position.x+Screen.width/2-50;
		coord.y = this.transform.position.y+Screen.height/2-50;
		text.rectTransform.localPosition = coord;
	}
	
	void FixedUpdate () {
		
		if(showFuel)
			text.text="Fuel: "+rocketManager.GetFuel();

		/* OUT OF SCREEN*/
		if(Mathf.Abs(rocket.transform.position.x-this.transform.position.x)>camSize*16/10){
			ResetPosition();
		}
		if(Mathf.Abs(rocket.transform.position.y-this.transform.position.y)>camSize){
			ResetPosition();
		}

		if(!rocketManager.GetColliding() && Time.time-rocketManager.GetTimer()>0.2f){

			if(rocket.transform.position.x>this.transform.position.x){
				position=new Vector3(rocket.transform.position.x,rocket.transform.position.y,-10);
				this.transform.position = position;
			}
			if(rocket.rigidbody.velocity.x<0){
				position=new Vector3(rocket.transform.position.x,rocket.transform.position.y,-10);
				this.transform.position = position;
			}
		}

		// upper bound and lower bound
		if(rocket.transform.position.y>GetBound(1) || rocket.transform.position.y<GetBound(2)){
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			this.transform.position=initialPosition;
		}
		
		/* left bound
		 * Once generated the first (starting) planet, the others will be all on right side
		 */
		if(rocket.transform.position.x<GetBound(0)){
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			this.transform.position=initialPosition;
		}
		
		/* right bound
		 */
		if(rocket.transform.position.x>GetBound(3)){
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			this.transform.position=initialPosition;
		}
	}

	/************************************************************************
	 *  CAMERA POSITION METHODS
	 ************************************************************************/

	public void ResetPosition(){
		this.transform.position=initialPosition;
	}

	public void SetInitialPosition(Vector3 pos){
		initialPosition=pos;
		print ("INI: x "+initialPosition.x+" y "+initialPosition.y);
	}

	public void SetThisAsInitialPosition(){
		initialPosition=this.transform.position;
		print ("INI: x "+initialPosition.x+" y "+initialPosition.y);
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

	public void SetLimit(Vector3 l){
		limit=l;
	}
	
	public Vector3 GetLimit(){
		return limit;
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
