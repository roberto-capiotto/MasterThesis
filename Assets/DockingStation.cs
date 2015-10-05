using UnityEngine;
using System.Collections;

public class DockingStation : MonoBehaviour {

	Rocket rocketManager;
	GameObject rocket;
	public int fuel=1000;
	int fuelForShoot=10;
	// phisics vars
	float acceleration=180f;
	float rocketVelocity=60f;
	float newangle;
	float gravity;
	float angleCollision;
	float m;
	Quaternion rotate = new Quaternion (0, 0, 0, 0);
	// mouseClick vars
	int mouseClicks = 0;
	bool mouseClicksStarted = false;
	float mouseTimerLimit = .25f;
	float timeClickUp;
	// flags
	bool shoot=false;
	bool collision=false;
	bool up=false;
	bool down=false;
	bool left=false;
	bool right=false;
	bool clockwise=false;

	void Start () {
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.blue);
	}

	void FixedUpdate () {
		if(shoot){
			if(rocketManager.Consume(fuelForShoot)){
				print("shoot");
				rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
				rocketManager.SetShootPosition(rocket.transform.position);
				if(clockwise)
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

	void OnCollisionEnter(Collision gravityCollision){
		rocketManager.onStart=false;
		rocketManager.SetColliding(true);
		angleCollision=tan(rocket.transform.position - this.transform.position);
		// find where the rocket will touch the spaceStation
		if(rocket.transform.position.x>this.transform.position.x)
			right=true;
		else
			left=true;
		if(rocket.transform.position.y>this.transform.position.y)
			up=true;
		else
			down=true;
		
		// calculate the shootingAngle
		m = tan (gravityCollision.transform.position-rocketManager.GetShootPosition());
		
		// 1st quad
		if(right && up){
			if(m<90 || m>=270){
				clockwise=true;
			}
			else{
				if(m>=90 && m<180){
					clockwise=false;
				}
				else{
					if(m<225){
						if(angleCollision>35)
							clockwise=false;
						else
							clockwise=true;
					}
					else{
						if(angleCollision>55)
							clockwise=false;
						else
							clockwise=true;
					}
				}
			}
		}
		
		//2nd quad
		if(left && up){
			if(m<90){
				clockwise=true;
			}
			else{
				if(m>=90 && m<270){
					clockwise=false;
				}
				else{
					if(m<315){
						if(angleCollision>145)
							clockwise=false;
						else
							clockwise=true;
					}
					else{
						if(angleCollision>125)
							clockwise=false;
						else
							clockwise=true;
					}
				}
			}
		}
		
		//3rd quad
		if(left && down){
			if(m>180){
				clockwise=false;
			}
			else{
				if(m>=90){
					clockwise=true;
				}
				else{
					if(m<45){
						if(angleCollision>235)
							clockwise=false;
						else
							clockwise=true;
					}
					else{
						if(angleCollision>215)
							clockwise=false;
						else
							clockwise=true;
					}
				}
			}
		}
		
		// 4th quad
		if(right && down){
			if(m<90){
				clockwise=false;
			}
			else{
				if(m>=180){
					clockwise=true;
				}
				else{
					if(m<135){
						if(angleCollision>305)
							clockwise=false;
						else
							clockwise=true;
					}
					else{
						if(angleCollision>325)
							clockwise=false;
						else
							clockwise=true;
					}
				}
			}
		}
		// refill
		rocketManager.Refill(fuel);
		collision=true;
	}

	void OnCollisionStay(Collision collision){
		rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));

		gravity=acceleration*(this.transform.localScale.x-1)*
			(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
				(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
		rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*3);

		if(clockwise)
			rocket.rigidbody.AddForce (-(Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity));
		else
			rocket.rigidbody.AddForce ((Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity));
		newangle = tan (rocket.transform.position - this.transform.position);
		rotate.eulerAngles = new Vector3 (0, 0, newangle - 90);
		rocket.transform.rotation = rotate;
	}

	void OnCollisionExit(){
		shoot=false;
		collision=false;
		rocketManager.SetTimer(Time.time);
		rocketManager.SetColliding(false);
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
			//	Double Click
			if(collision)
				shoot=true;			
		}
		mouseClicksStarted = false;
		mouseClicks = 0;
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
