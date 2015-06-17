using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {

	// rocket vars
	GameObject rocket;
	Rocket rocketManager;
	Vector3 initialPosition;
	int fuelForShoot=50;
	// planet vars
	public string planetType;	/* "count" for exploding planet */
	public int counter=5;
	SphereCollider myCollider;
//	public float planetSize;
	int orbitLevel;
	public bool clockWise=false;
	// phisics vars
	float gravity;
	float acceleration=180f;
	float rocketVelocity=60f;
	float newangle;
	Quaternion rotate = new Quaternion (0, 0, 0, 0);
	// mouseClick vars
	Vector3 mousePosClick;
	Vector3 mousePosBefore;
	Vector3 mousePosNew;
	bool mouseClicksStarted = false;
	int mouseClicks = 0;
	float mouseTimerLimit = .25f;
	float timeClickUp;
	// flags
	bool shoot=false;
	bool collision=false;
	bool raise=false;
	
	void Start () {
		rocket = GameObject.Find ("Rocket");
		myCollider = transform.GetComponent<SphereCollider>();
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
//		planetSize=Random.Range(1.8f,2.2f);
		initialPosition=new Vector3
			(this.transform.position.x ,this.transform.position.y+(this.transform.localScale.y/2)+myCollider.radius,0);
		rocket.transform.position=initialPosition;
		orbitLevel=3;

		// TODO: random planetSize
		// it will scale also orbits

		if(planetType.Equals("count")){
			//this.transform.localScale= new Vector3 (this.transform.localScale.x/2,this.transform.localScale.y/2,this.transform.localScale.z/2);
			// graphics
			Renderer rend = GetComponent<Renderer>();
			rend.material.shader = Shader.Find("Specular");
			rend.material.SetColor("_Color", Color.yellow);
		}
		else{
			/*if(planetType.Equals("planet")){
				// graphics
			}*/
//			this.transform.localScale=new Vector3(planetSize,planetSize,2);
		}
	}

	void FixedUpdate () {
		if(shoot){
			// less distance from planet, more fuel required
			if(rocketManager.Consume((4-orbitLevel)*fuelForShoot/5)){
				print("shoot");
				rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
				rocketManager.SetShootPosition(rocket.transform.position);
				if(clockWise)
					rocket.rigidbody.AddForce(rocket.transform.right * acceleration);
				else
					rocket.rigidbody.AddForce(-rocket.transform.right * acceleration);
			}
			else{
				shoot=false;
				rocketManager.SetInitialPosition();
				rocketManager.FullRefill();
			}
		}
	}
	
	void OnCollisionEnter (Collision gravityCollision)
	{
		if(planetType.Equals("count")){
			// starts counter
			InvokeRepeating("CountDown",1,1);
		}
		collision=true;
		print("touch @ "+gravityCollision.transform.position.x+" "+gravityCollision.transform.position.y+" "+gravityCollision.transform.position.z);
		float ang=tan (gravityCollision.transform.position-this.transform.position);
		print("angle in: "+ang);
		float m = tan (gravityCollision.transform.position-rocketManager.GetShootPosition());
		print("angle throw: "+m);
		/* TODO: all
		if(ang<45 || ang>315){
			if(m>180)
				print ("YES ang: "+ang+" m: "+m);
			else
				print ("NO ang: "+ang+" m: "+m);
		}
		if(ang>45 && ang<135){//TODO: check
			if(m>270)
				print ("YES ang: "+ang+" m: "+m);
			else
				print ("NO ang: "+ang+" m: "+m);
		}
		if(ang>135 && ang<225){//TODO: check
			if(m<180)
				print ("YES ang: "+ang+" m: "+m);
			else
				print ("NO ang: "+ang+" m: "+m);
		}
		if(ang>225 && ang<315){//TODO: check
			if(m>90)
				print ("YES ang: "+ang+" m: "+m);
			else
				print ("NO ang: "+ang+" m: "+m);
		}*/
	}
	
	void OnCollisionStay(Collision collision){
		rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
		// check if the planet is rotating clockWise or not
		if(clockWise)
			rocket.rigidbody.AddForce (-(Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity));
		else
			rocket.rigidbody.AddForce ((Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity));
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
		shoot=false;
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
			
		}else{
//			Debug.Log("Single Clicked");
			if(raise){
				if(rocketManager.Consume(fuelForShoot/2)){
					// increase orbit size
					myCollider.radius += 0.15f;
					orbitLevel++;
					if(myCollider.radius>=0.9f)
						raise=false;
				}
				else{
					rocketManager.SetInitialPosition();
					rocketManager.FullRefill();
				}
			}
			else{
				if(rocketManager.Consume(fuelForShoot/2)){
					// decrease orbit size
					myCollider.radius -= 0.15f;
					orbitLevel--;
					// add force for continue collision
					rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*100);
					if(myCollider.radius<=0.6f)
						raise=true;
				}
				else{
					rocketManager.SetInitialPosition();
					rocketManager.FullRefill();
				}
			}
			
		}
		mouseClicksStarted = false;
		mouseClicks = 0;
	}

	void CountDown()
	{
		counter--;
		// TODO: use GUI TEXT for showing seconds remaining
		print(counter+"!!!");
		if(counter < 1)
		{
			print ("Count down finished");
			CancelInvoke("CountDown");
			// check if rocket is orbiting here
			if(collision){
				// shoot rocket away
				rocket.rigidbody.AddForce((rocket.transform.position-this.transform.position).normalized * acceleration * this.transform.localScale.x/2);
			}
			// destroy planet
			// TODO: add some animation
			Destroy(this.gameObject);
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
