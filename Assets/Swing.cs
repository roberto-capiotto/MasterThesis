using UnityEngine;
using System.Collections;

public class Swing : MonoBehaviour {

	float shootAcceleration = 200f;
	// flag
	bool colliding = false;
	bool up=false;
	bool down=false;
	bool left=false;
	bool right=false;
	bool clockwise=false;
	// other params
	float angleCollision;
	float angleDestination;
	float angleCurrent;
	float newangle;
	Quaternion rotate = new Quaternion (0, 0, 0, 0);
	GameObject rocket;
	Rocket rocketManager;
	float rocketVelocity=60f;
	SphereCollider myCollider;
	
	void Start ()
	{
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		myCollider = transform.GetComponent<SphereCollider>();
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.cyan);
	}
	
	void FixedUpdate ()
	{
		angleCurrent=tan(rocket.transform.position - this.transform.position);
		if(!clockwise){
			if(angleDestination<90){
				if(angleCurrent>angleDestination && angleCurrent<180)
					shoot(clockwise);
			}
			else{
				if(angleCurrent>angleDestination)
					shoot(clockwise);
			}
		}
		else{
			if(angleDestination>270){
				if(angleCurrent<angleDestination && angleCurrent>180)
					shoot(clockwise);
			}
			else{
				if(angleCurrent<angleDestination)
					shoot(clockwise);
			}
		}
	}

	void OnCollisionEnter (Collision gravityCollision)
	{
		colliding = true;
		angleCollision=tan(rocket.transform.position - this.transform.position);
		print ("angleCollision: "+angleCollision);
		if(myCollider.radius>=1f){
			// decrease orbit size
			print("lowerize");
			myCollider.radius -= 0.15f;
		}

		// find where the rocket will touch swingPlanet
		if(rocket.transform.position.x>this.transform.position.x)
			right=true;
		else
			left=true;
		if(rocket.transform.position.y>this.transform.position.y)
			up=true;
		else
			down=true;

		// calculate the shootingAngle
		float m = tan (gravityCollision.transform.position-rocketManager.GetShootPosition());

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
					if(angleCollision>45)
						clockwise=false;
					else
						clockwise=true;
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
					print ("angle: "+angleCollision+" m: "+m);
					if(angleCollision>135)
						clockwise=false;
					else
						clockwise=true;
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
					if(angleCollision>225)
						clockwise=false;
					else
						clockwise=true;
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
					if(angleCollision>315)
						clockwise=false;
					else
						clockwise=true;
				}
			}
		}

		print ("m: "+m+" angle: "+angleCollision+" ck: "+clockwise);

		/* calculate angleDestination
		 * if clockwise, sub 90
		 * if !clockwise, add 90
		 * check if overflow or underflow
		 */
		if(clockwise){
			if(angleCollision>90)
				angleDestination=angleCollision-90;
			else
				angleDestination=angleCollision-90+360;
		}
		else{
			if(angleCollision<270)
				angleDestination=angleCollision+90;
			else
				angleDestination=angleCollision+90-360;
		}
	}

	void OnCollisionStay (Collision collider) {

		rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
		if(clockwise)
			rocket.rigidbody.AddForce (-(Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity*3));
		else
			rocket.rigidbody.AddForce ((Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity*3));
		newangle = tan (rocket.transform.position - this.transform.position);
		rotate.eulerAngles = new Vector3 (0, 0, newangle - 90);
		rocket.transform.rotation = rotate;
	}

	void OnCollisionExit (Collision gravityCollision)
	{
		// reset all flags
		colliding=false;
		up=false;
		down=false;
		left=false;
		right=false;
	}
	
	void shoot (bool direction)
	{
		if(colliding)
		{
			rocket.rigidbody.velocity=new Vector3(0,0,0);
			if(direction)
				rocket.rigidbody.AddForce(rocket.transform.right * shootAcceleration*3);
			else
				rocket.rigidbody.AddForce(-rocket.transform.right * shootAcceleration*3);
		}
	}

	public void SetRadius(float raggio){
		print ("raggio "+raggio);
		myCollider.radius=raggio;
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