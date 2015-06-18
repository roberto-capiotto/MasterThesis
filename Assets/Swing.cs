using UnityEngine;
using System.Collections;

public class Swing : MonoBehaviour {

	float shootAcceleration = 200f;
	// flag
	bool increase;
	bool RotationExpand;
	bool colliding = false;
	bool longClick;
	bool trapDecrease;
	bool oldAlienChangeStatus;
	//flag about swingBehaviour
	bool startRound=true;
	bool imUp=false;
	bool imDown=false;
	bool imLeft=false;
	bool imRight=false;
	// other params
	float timeClickUp;
	float distanceExpand;
	double k;
	float x;
	float y;
	float angleBefore;
	float angleNew;
	float angleMovement;
	float deltaPlanet;
	Vector3 mousePosClick;
	Vector3 mousePosBefore;
	Vector3 mousePosNew;
	float camerascreenWidht;
	float camerascreenHight;
	RaycastHit hit;
	GameObject rocket;
	SphereCollider myCollider;
	bool clockwise=false;
	float rocketVelocity=60f;
	float newangle;
	Quaternion rotate = new Quaternion (0, 0, 0, 0);
	
	//TODO: be careful on imLeft imRight
	// many things are going bad
	
	void Start ()
	{
		rocket = GameObject.Find ("Rocket");
		myCollider = transform.GetComponent<SphereCollider>();
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.cyan);
	}
	
	void FixedUpdate ()
	{
		if(colliding && imRight){
			// rocket comes from right
			// if rocket arrives @ pole
			if(rocket.transform.position.x<this.transform.position.x && startRound && colliding){
				startRound=false;
				// south pole
				if(rocket.transform.position.y<this.transform.position.y){
					imDown=true;
					print ("i'm down from right");
				}
				// north pole
				else{
					imUp=true;
					print ("i'm up from right");
				}
			}
			// if you was @ north pole
			if(imUp && !startRound){
				// and now you are @ left pole
				if(rocket.transform.position.y<this.transform.position.y)
					shoot(Vector3.left);
			}
			// if you was @ south pole
			if(imDown && !startRound){
				// and now you are @ left pole
				if(rocket.transform.position.y>this.transform.position.y)
					shoot(Vector3.right);
			}
		}
		else{
			if(colliding && imLeft){
				// rocket comes from left
				// if rocket arrives @ pole
				if(rocket.transform.position.x>this.transform.position.x && startRound && colliding){
					startRound=false;
					// south pole
					if(rocket.transform.position.y<this.transform.position.y){
						imDown=true;
						print ("i'm down from left");
					}
					// north pole
					else{
						imUp=true;
						print ("i'm up from left");
					}
				}
				// if you was @ north pole
				if(imUp && !startRound){
					// and now you are @ right pole
					if(rocket.transform.position.y<this.transform.position.y){
						shoot(Vector3.right);
					}
				}
				// if you was @ south pole
				if(imDown && !startRound){
					// and now you are @ right pole
					if(rocket.transform.position.y>this.transform.position.y){
						shoot(Vector3.left);
					}
				}
			}
		}
		if(colliding){
//			rocket.transform.parent=this.transform;
		}
		//transform.Rotate(Vector3.forward * Time.deltaTime*50);
	}

	void OnCollisionEnter (Collision gravityCollision)
	{
		colliding = true;
		print(gravityCollision.transform.position.x);
		if(myCollider.radius>=1f){
			// decrease orbit size
			print("lowerize");
			myCollider.radius -= 0.15f;
		}
		
		if(rocket.transform.position.x>this.transform.position.x){
			imRight=true;
			print("*********I'M RIGHT*******");
		}
		else{
			imLeft=true;
			print("*********I'M LEFT********");
		}

		if(rocket.transform.position.y>this.transform.position.y){
			clockwise=true;
		}
		else
			clockwise=false;
	}

	void OnCollisionStay (Collision collider) {

		rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
		if(clockwise)
			rocket.rigidbody.AddForce (-(Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity*2));
		else
			rocket.rigidbody.AddForce ((Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity*2));
		newangle = tan (rocket.transform.position - this.transform.position);
		rotate.eulerAngles = new Vector3 (0, 0, newangle - 90);
		rocket.transform.rotation = rotate;
	}

	void OnCollisionExit (Collision gravityCollision)
	{
		colliding = false;
		startRound=true;
		imUp=false;
		imDown=false;
		imLeft=false;
		imRight=false;
//		rocket.transform.parent=null;
	}
	
	void shoot (Vector3 shootingDirection)
	{
		if(colliding)
		{
			rocket.rigidbody.velocity=new Vector3(0,0,0);
			// it works just for horizontal shooting (left and right)
			// code must be improved passing a parameter to function shoot() with the correct angle
			if(shootingDirection==Vector3.right)
				rocket.rigidbody.AddForce(rocket.transform.right * shootAcceleration*3);
			if(shootingDirection==Vector3.left)
				rocket.rigidbody.AddForce(-rocket.transform.right * shootAcceleration*3);
			//rocket.rigidbody.AddForce(shootingDirection * shootAcceleration * 3 * this.transform.localScale.x/2);
		}
	}

	public void SetRadius(float radius){
		myCollider.radius=radius;
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