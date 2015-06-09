using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

	public GameObject rocket;
	public string planetType;
	public int counter=5;
	bool collision=false;
	SphereCollider myCollider;
	float gravity;
	float acceleration=180f;
	float rocketVelocity=60f;
	float newangle;
	Quaternion rotate = new Quaternion (0, 0, 0, 0);
	Vector3 mousePosClick;
	Vector3 mousePosBefore;
	Vector3 mousePosNew;
	float timeClickUp;
	bool doubleClick=true;
	bool raise=true;
	
	void Start () {
		rocket = GameObject.Find ("Rocket");
		myCollider = transform.GetComponent<SphereCollider>();
		rocket.transform.position=new Vector3
			(this.transform.position.x ,this.transform.position.y+(this.transform.localScale.y/2)+myCollider.radius,0);
	}

	void Update () {
		tap();
	}
	
	void OnCollisionEnter (Collision gravityCollision)
	{
		if(planetType.Equals("count")){
			// starts counter
			InvokeRepeating("CountDown",1,1);
		}
		collision=true;
		print("touch @ "+gravityCollision.transform.position.x+" "+gravityCollision.transform.position.y+" "+gravityCollision.transform.position.z);
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
		rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity);
	}
	
	void OnCollisionExit (Collision coll){
		collision=false;
	}

	// TODO: setup this function because now it is not well working
	// double tap = single tap (increase) + single tap (then shoot)
	// double tap = single tap (decrease) + single tap (then shoot)
	// this is not a good thing
	void tap ()
	{
		if (Input.GetMouseButtonUp (0)) {
			// distinguish between single and double tap
			if (Time.time - timeClickUp > 0.4f) {
				doubleClick = false;
			} else {
				doubleClick = true;
			}
			timeClickUp = Time.time;

			// if single tap
			if(!doubleClick){
				if(raise){
					// increase orbit size
					myCollider.radius += 0.15f;
					if(myCollider.radius>=0.9f)
						raise=false;
				}
				else{
					// decrease orbit size
					myCollider.radius -= 0.15f;
					// add force for continue collision
					rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*100);
					if(myCollider.radius<=0.6f)
						raise=true;
				}
			}
			// if double tap
			else{
				// shoot
				rocket.rigidbody.velocity=rocket.transform.right * acceleration/10;
			}
		}
	}

	void CountDown()
	{
		counter--;
		print(counter+"!!!");
		if(counter < 1)
		{
			print ("Count down finished");
			CancelInvoke("CountDown");
			// check if rocket is orbiting here
			if(collision){
				// destroy rocket
				Destroy(rocket);
				// TODO: add some restore method
			}
			// destroy planet
			Destroy(this);
		}
	}

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
