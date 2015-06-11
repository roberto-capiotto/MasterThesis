using UnityEngine;
using System.Collections;

public class DockingStation : MonoBehaviour {

	Rocket rocketManager;
	GameObject rocket;
	public int people=1;
	bool doubleClick=false;
	float timeClickUp;
	float acceleration=180f;
	float rocketVelocity=60f;
	float newangle;
	Quaternion rotate = new Quaternion (0, 0, 0, 0);
	float gravity;
	bool mouseClicksStarted = false;
	int mouseClicks = 0;
	float mouseTimerLimit = .25f;
	bool shoot=false;
	bool collision=false;

	void Start () {
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
	}

	void Update () {
		if(shoot){
			print("shoot");
			rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
			rocket.rigidbody.AddForce(rocket.transform.right * acceleration);
		}
//		tap();
	}

	void OnCollisionEnter(){
		// save people
		rocketManager.SavePeople(people);
		people=0;
		collision=true;
	}

	void OnCollisionStay(Collision collision){
		rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
		rocket.rigidbody.AddForce (-(Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity));
		newangle = tan (rocket.transform.position - this.transform.position);
		rotate.eulerAngles = new Vector3 (0, 0, newangle - 90);
		rocket.transform.rotation = rotate;
		
		gravity=acceleration*(this.transform.localScale.x-1)*
			(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
				(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
		rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*3);
	}

	void OnCollisionExit(){
		shoot=false;
		collision=false;
	}

	public void OnMouseDown(){
		mouseClicks++;
		if(mouseClicksStarted){
			return;
		}
		mouseClicksStarted = true;
		Invoke("checkMouseDoubleClick",mouseTimerLimit);
	}
	
	
	private void checkMouseDoubleClick()
	{
		if(mouseClicks > 1){
			//			Debug.Log("Double Clicked");
			if(collision)
				shoot=true;
			/*			rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
			rocket.rigidbody.AddForce(rocket.transform.right * acceleration);*/
			//			rocket.rigidbody.velocity=rocket.transform.right * acceleration/10;
			
		}
		mouseClicksStarted = false;
		mouseClicks = 0;
	}

/*	void tap ()
	{
		if (Input.GetMouseButtonUp (0)) {
			// distinguish between single and double tap
			if (Time.time - timeClickUp > 0.4f) {
				doubleClick = false;
			} else {
				doubleClick = true;
			}
			timeClickUp = Time.time;

			// if double tap
			if(doubleClick){
				// shoot
				rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
				rocket.rigidbody.velocity=rocket.transform.right * acceleration/10;
			}
		}
	}
*/
	float tan (Vector3 pos)
	{
		if (pos.x >= 0) {
			if (pos.x == 0) {
				if (pos.y >= 0) {
					return 90f;
				} else {
					return 270f;
				}
			}
			
			if (pos.y >= 0) {
				return(Mathf.Atan (pos.y / pos.x) * 180f / Mathf.PI);
			} else {
				return(360 + Mathf.Atan (pos.y / pos.x) * 180 / Mathf.PI);
			}
		} else {
			if (pos.y >= 0) {
				return(180f + Mathf.Atan (pos.y / pos.x) * 180 / Mathf.PI);
			} else {
				return(180f + Mathf.Atan (pos.y / pos.x) * 180 / Mathf.PI);
			}
		}
	}
}
