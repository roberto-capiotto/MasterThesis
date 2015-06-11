using UnityEngine;
using System.Collections;

public class Swing : MonoBehaviour {
	// public variables
	string expandFrame;
	string clickFrame;
	string rotationFrame;
	float expandAcceleration = 500;
	public float increasingSpeed = 3;
	public float decreasingSpeed = 1;
	public int frameIndex;
	public float upperSize;
	public float downSize;
	public float maxExpandSpeed = 0.1f;
	public float maxRotationSpeed = 5;
	float shootAcceleration = 200f;
	public string colour;
	bool expandTrapp = true;
	// flag
	bool increase;
	bool down;
	bool RotationExpand;
	bool colliding = false;
	bool doubleClick = true;
	bool longClick;
	bool trapIncrease = false;
	bool trapDecrease;
	bool moved = false;
	bool oldAlienChangeStatus;
	//flag about cometLikeBehaviour
	bool startRound=true;
	bool imUp=false;
	bool imDown=false;
	bool imLeft=false;
	bool imRight=false;
	// other params
	float timeClickUp;
	float timeClickDown = Mathf.Infinity;
	float distanceExpand;
	float gravity;
	float acceleration=180f;
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
	public SphereCollider myCollider;
	
	//TODO: be careful on imLeft imRight
	// many things are going bad
	
	void Start ()
	{
		rocket = GameObject.Find ("Rocket");
		myCollider = transform.GetComponent<SphereCollider>();
		down = false;
	}
	
	void Update ()
	{
		resizePlanet ();
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
				// and now you are @ south pole
				if(rocket.transform.position.x>this.transform.position.x && rocket.transform.position.y<this.transform.position.y)
					shoot(Vector3.right);
			}
			// if you was @ south pole
			if(imDown && !startRound){
				// and now you are @ north pole
				if(rocket.transform.position.x>this.transform.position.x && rocket.transform.position.y>this.transform.position.y)
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
					// and now you are @ south pole
					if(rocket.transform.position.x<this.transform.position.x && rocket.transform.position.y<this.transform.position.y){
						shoot(Vector3.right);
					}
				}
				// if you was @ south pole
				if(imDown && !startRound){
					// and now you are @ north pole
					if(rocket.transform.position.x<this.transform.position.x && rocket.transform.position.y>this.transform.position.y){
						shoot(Vector3.right);
					}
				}
			}
		}
	}
	
	void OnCollisionEnter (Collision gravityCollision)
	{
		if(myCollider.radius>=1f){
			// decrease orbit size
			print("lowerize");
			myCollider.radius -= 0.15f;
			// add force for continue collision
			rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*100);
		}

		colliding = true;
		print(gravityCollision.transform.position.x);
		if(rocket.transform.position.x>this.transform.position.x){
			imRight=true;
			print("*********I'M RIGHT*******");
		}
		else{
			imLeft=true;
			print("*********I'M LEFT********");
		}
	}

	void OnCollisionStay (Collision collider) {

		gravity=acceleration*(this.transform.localScale.x-1)*
			(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
				(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
		
		rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity);
	}

	void OnCollisionExit (Collision gravityCollision)
	{
		colliding = false;
		startRound=true;
		imUp=false;
		imDown=false;
		imLeft=false;
		imRight=false;
	}
	
	void shoot (Vector3 shootingDirection)
	{
		if(colliding)
		{
			rocket.rigidbody.velocity=new Vector3(0,0,0);
			// it works just for horizontal shooting (left and right)
			// code must be improved passing a parameter to function shoot() with the correct angle
			rocket.rigidbody.AddForce(shootingDirection * shootAcceleration * (this.transform.parent.gameObject).transform.localScale.x/2);
		}
	}

	public void SetRadius(float radius){
		myCollider.radius=radius;
	}

	void resizePlanet ()
	{
		if (Input.GetMouseButtonDown (0)) {
			
			mousePosClick = Input.mousePosition;
			
			if (MouseTouch ()) {
				moved = false;
				down = true;	
				mousePosBefore = Input.mousePosition;
			}
		}
		
		if (Input.GetMouseButton (0) && down && Time.time - timeClickDown > 0.2f) {
			
			mousePosNew = (Input.mousePosition);
			angleNew = tan (WorldCoordinate (mousePosNew) - transform.position);
			distanceExpand = distanceCenterPlanetExpand (mousePosNew) - distanceCenterPlanetExpand (mousePosBefore);
			angleBefore = tan (WorldCoordinate (mousePosBefore) - transform.position);
			angleMovement = angleNew - angleBefore;
			mousePosBefore = (Input.mousePosition);
			
			if (distanceExpand + angleMovement != 0) {
				moved = true;
			}
			
			if (RotationExpand) {
				rotate ();
			}
			
			if (!RotationExpand) {
				
				//expand ();
				//TODO:remove
				//shoot(Vector3.zero);
				if (expandTrapp) {
					expandTrapp = false;
				}
			}
			
		} 
		if (Input.GetMouseButtonUp (0)) {
			expandTrapp = true;
			if (Time.time - timeClickUp > 0.4f) {
				doubleClick = false;
			} else {
				doubleClick = true;
			}
			timeClickUp = Time.time;
			down = false;
		}
	}
	
	
	// return true if the click is on the object 
	bool MouseTouch ()
	{
		
		if (Physics.Raycast (WorldCoordinate (mousePosClick), Vector3.forward, out hit, Mathf.Infinity, 1 << 9)) {//,11))
			if (this.transform.GetInstanceID () == hit.collider.transform.GetInstanceID ()) {
				timeClickDown = Time.time;
				trapIncrease = true;	
			}
		}
		
		if (Physics.Raycast (WorldCoordinate (mousePosClick), Vector3.forward, out hit, Mathf.Infinity, 1 << 15)) {//,11))
			if (collider.transform.GetInstanceID () == hit.collider.transform.parent.transform.GetInstanceID ()) {
				RotationExpand = true;
				return true;
			}
		}
		if (Physics.Raycast (WorldCoordinate (mousePosClick), Vector3.forward, out hit, Mathf.Infinity, 1 << 16)) {//,11))
			if (collider.transform.GetInstanceID () == hit.collider.transform.parent.transform.GetInstanceID ()) {
				RotationExpand = false;
				return true;
			}
		}
		
		
		return false;
	}
	
	
	//transform a cordinate in pixel in dUnity coordinate
	Vector3 WorldCoordinate (Vector3 mouseCoordinates)
	{
		return Camera.main.ScreenToWorldPoint (mouseCoordinates);
	}
	
	//return the distance between the center of the panet and the coordinate
	float distanceCenterPlanetExpand (Vector3 mousePositiona)
	{
		return modulo (WorldCoordinate (new Vector3 (0, mousePositiona.y, 0)) - WorldCoordinate (new Vector3 (mousePosClick.x, 0, 0)));
		
	}
	
	float distanceCenterPlanet (Vector3 mousePositiona)
	{
		return modulo ((WorldCoordinate (mousePositiona) - transform.position));
	}
	
	float modulo (Vector3 modulo)
	{
		return Mathf.Sqrt (modulo.x * modulo.x + modulo.y * modulo.y);
	}
	
	void rotate ()
	{
		this.transform.Rotate (new Vector3 (0, 0, Mathf.Clamp (angleMovement, -maxRotationSpeed, maxRotationSpeed)));
		
		if (colliding) {
			rocket.transform.RotateAround (this.transform.position, new Vector3 (0f, 0f, 1f), 0.5f * Mathf.Clamp (angleMovement, -maxRotationSpeed, maxRotationSpeed));
		}
		
	}
	
	void expand ()
	{
		Vector3 delta = new Vector3 (2f * Mathf.Clamp (distanceExpand, -maxExpandSpeed, maxExpandSpeed), 2f * Mathf.Clamp (distanceExpand, -maxExpandSpeed, maxExpandSpeed), 0);
		transform.localScale = new Vector3 (Mathf.Clamp (delta.x + transform.localScale.x, downSize, upperSize), Mathf.Clamp (delta.y + transform.localScale.y, downSize, upperSize), 1);
		if (!colliding) {
			rocket.rigidbody.AddForce (-(rocket.transform.position - (this.transform.position)).normalized * expandAcceleration * delta.sqrMagnitude);
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

	
	public string getColour ()
	{
		return colour;
	}
	
}	


/*using UnityEngine;
using System.Collections;

public class Swing : MonoBehaviour {

	GameObject rocket;
//	GameObject joint;
	bool collided=false;

	void Start () {
		rocket = GameObject.Find ("Rocket");
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.cyan);
	}

	void Update () {
		if(collided){
			//Create HingeJoints
			/*joint = gameObject.AddComponent<HingeJoint> ();
			joint.axis = Vector3.back; /// (0,0,-1)
			joint.anchor = Vector3.zero;
			joint.connectedBody = anchor.rigidbody;
			anchorJoint = anchor.AddComponent<HingeJoint> ();
			anchorJoint.axis = Vector3.back; /// (0,0,-1)
			anchorJoint.anchor = Vector3.zero;
		}
	}

	void OnCollisionEnter(Collision collision){
		collided=true;
		// capisci da che parte arrivi e dove devi girare
		// unisci gli oggetti
		// quando sei arrivato spara
	}

	void OnCollisionExit(Collision collision){
		collided = false;
	}
}
*/