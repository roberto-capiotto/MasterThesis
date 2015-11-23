using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Planet : MonoBehaviour {

	// rocket vars
	GameObject rocket;
	Rocket rocketManager;
	int fuelForShoot=50;
	Vector3 newPosition;
	// planet vars
	public string planetType;	/* "count" for exploding planet
	                             * "checkpoint" for checkpoint planet
	                             * "first" for firstLevel planet
	                             * "end" for endLevel planet
	                             */
	public int counter=5;
	public SphereCollider myCollider;
	int orbitLevel=2;
	public Text text;
	Color32 color = new Color32(215, 95, 95, 27);
	Color32 backgroundColor = new Color32(49, 77, 121, 255);
	// satellite vars
	public GameObject inSatellite;
	public GameObject outSatellite;
	SatelliteScript inSat;
	SatelliteScript outSat;
	float randomPlacement;
	float inRadius=1.3f;
	float outRadius=2.05f;
	// phisics vars
	float gravity;
	float acceleration=180f;
	float rocketVelocity=60f;
	float newangle;
	Quaternion rotate = new Quaternion (0, 0, 0, 0);
	float angleCollision;
	float y,m,x,q,myM;
	float distance,minDistance;
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
	bool counting=false;
	bool up=false;
	bool down=false;
	public bool left=false;
	public bool right=false;
	public bool clockwise=false;
	bool rotating=false;
	public bool levelCompleted=false;
	
	void Start () {
		rocket = GameObject.Find ("Rocket");
		myCollider = transform.GetComponent<SphereCollider>();
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		SetPlanetType(planetType);

		// place inSatellite
		randomPlacement = Random.Range(0,4);
		if(randomPlacement<1){
			inSatellite.transform.position=new Vector3(this.transform.position.x,this.transform.position.y+inRadius,0);
		}
		else{
			if(randomPlacement<2){
				inSatellite.transform.position=new Vector3(this.transform.position.x-inRadius,this.transform.position.y,0);
			}
			else{
				if(randomPlacement<3){
					inSatellite.transform.position=new Vector3(this.transform.position.x,this.transform.position.y-inRadius,0);
				}
			}
		}

		// place outSatellite
		randomPlacement = Random.Range(0,4);
		if(randomPlacement<1){
			outSatellite.transform.position=new Vector3(this.transform.position.x,this.transform.position.y+outRadius,0);
		}
		else{
			if(randomPlacement<2){
				outSatellite.transform.position=new Vector3(this.transform.position.x-outRadius,this.transform.position.y,0);
			}
			else{
				if(randomPlacement<3){
					outSatellite.transform.position=new Vector3(this.transform.position.x,this.transform.position.y-outRadius,0);
				}
			}
		}

		// set rotation of Satellites
		inSat = inSatellite.GetComponent ("SatelliteScript") as SatelliteScript;
		outSat = outSatellite.GetComponent ("SatelliteScript") as SatelliteScript;
		if(Random.Range(0,2)==0){
			inSat.SetRotation(true);
			outSat.SetRotation(false);
		}
		else{
			inSat.SetRotation(false);
			outSat.SetRotation(true);
		}
	}

	void FixedUpdate () {
		if(shoot){
			// less distance from planet, more fuel required
			if(rocketManager.Consume((4-orbitLevel)*fuelForShoot/5)){
				print("shoot");
				rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
				rocketManager.SetShootPosition(rocket.transform.position);
				rotating=false;
				if(clockwise)
					rocket.rigidbody.AddForce(rocket.transform.right *2* acceleration);
				else
					rocket.rigidbody.AddForce(-rocket.transform.right *2* acceleration);
			}
			else{
				shoot=false;
				rocketManager.SetInitialPosition();
				rocketManager.FullRefill();
			}
		}
		if(collision==false && Time.time-rocketManager.GetTimer()>1){
			if(orbitLevel==1){
				orbitLevel=2;
				myCollider.radius += 0.3f;
				raise=false;
			}
		}
	}

	/************************************************************************
	 *  COLLISION METHODS
	 ************************************************************************/
	
	void OnCollisionEnter (Collision gravityCollision)
	{
		rocketManager.SetCollPlanet(this);
		if(planetType.Equals("checkpoint")){
			// checkpoint reached!!
			if(this.transform.position.x>rocketManager.GetInitialPosition().x){
				// change initial position of rocket
				// change initial position of camera
				newPosition=new Vector3(this.transform.position.x,this.transform.position.y+1.3f,0);
				rocketManager.ChangeInitialPosition(newPosition);
				rocketManager.onStart=true;
				rocketManager.SetCheck(true);
				print ("CHECK");
			}
		}
		else{
			if(planetType.Equals("count")){
				// start counter
				InvokeRepeating("CountDown",1,1);
				rocketManager.onStart=false;
			}
			else{
				if(planetType.Equals("end")){
					// start counter: check collision is ok
					InvokeRepeating("CountEnd",1,1);
				}
				else{
					if(planetType.Equals("first")){
						// set camera position
						rocketManager.onStart=true;
					}
					else{
						rocketManager.onStart=false;
					}
				}
			}
		}
		collision=true;
		rocketManager.SetColliding(true);
		if(!rotating){
			rotating=true;
			angleCollision=tan(gravityCollision.transform.position - this.transform.position);
			// find where the rocket will touch the planet
			if(gravityCollision.transform.position.x>this.transform.position.x)
				right=true;
			else
				left=true;
			if(gravityCollision.transform.position.y>this.transform.position.y)
				up=true;
			else
				down=true;
			
			// calculate the shootingAngle
			m = tan (rocket.transform.position-rocketManager.GetShootPosition());

			// interpole data!
			y=rocket.transform.position.y;
			x=rocket.transform.position.x;
			myM = (rocketManager.GetShootPosition().y-y)/(rocketManager.GetShootPosition().x-x);
			q = rocket.transform.position.y-myM*rocket.transform.position.x;
			minDistance=Mathf.Sqrt(Mathf.Pow(x-this.transform.position.x,2)+Mathf.Pow(y-this.transform.position.y,2));
			distance=minDistance;

			while(distance==minDistance){
				if(rocket.transform.position.x>rocketManager.GetShootPosition().x)
					x+=0.2f;
				else
					x-=0.2f;
				y=myM*x+q;
				distance=Mathf.Sqrt(Mathf.Pow(x-this.transform.position.x,2)+Mathf.Pow(y-this.transform.position.y,2));
				if(distance<minDistance)
					minDistance=distance;
			}

			if(minDistance<1){
				// decrease orbit size
				myCollider.radius -= 0.3f;
				raise=true;
				orbitLevel--;
				// add force for continue collision
				gravity=acceleration*(this.transform.localScale.x-1)*
					(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
						(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
				rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*250);
			}

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
		}
	}
	
	void OnCollisionStay(Collision collision){
		rocket.rigidbody.velocity = (new Vector3 (0, 0, 0));
		// check if the planet is rotating clockwise or not
		if(clockwise)
			rocket.rigidbody.AddForce (-(Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity*2));
		else
			rocket.rigidbody.AddForce ((Quaternion.Euler (0, 0, 90) * (rocket.transform.position - this.transform.position).normalized * rocketVelocity*2));
		newangle = tan (rocket.transform.position - this.transform.position);
		rotate.eulerAngles = new Vector3 (0, 0, newangle - 90);
		rocket.transform.rotation = rotate;

		gravity=acceleration*(this.transform.localScale.x-1)*
			(this.transform.localScale.x-1)*(this.transform.localScale.x-1)/
				(this.transform.position-rocket.transform.position).sqrMagnitude*Time.deltaTime;
		rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*2);

		// color the current orbit
		if(orbitLevel==1){
			Renderer rend = this.transform.GetChild(0).GetComponent<Renderer>();
			rend.material.shader = Shader.Find("Specular");
			rend.material.SetColor("_Color", Color.yellow);
		}
		else{
			Renderer rend = this.transform.GetChild(0).GetComponent<Renderer>();
			rend.material.shader = Shader.Find("Transparent/Bumped Diffuse");
			//rend.material.SetColor("_Color", Color.blue);
			rend.material.SetColor("_Color", backgroundColor);
			rend = this.transform.GetChild(1).GetComponent<Renderer>();
			rend.material.shader = Shader.Find("Transparent/Bumped Diffuse");
			rend.material.SetColor("_Color", Color.yellow);
		}
	}
	
	void OnCollisionExit (Collision coll){
		collision=false;
		shoot=false;
		up=false;
		down=false;
		left=false;
		right=false;
		rocketManager.SetTimer(Time.time);
		rocketManager.SetColliding(false);

		// decolor everything
		Renderer rend = this.transform.GetChild(0).GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Transparent/Bumped Diffuse");
		rend.material.SetColor("_Color", color);
		rend = this.transform.GetChild(1).GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Transparent/Bumped Diffuse");
		rend.material.SetColor("_Color", color);
	}

	/************************************************************************
	 *  MOUSE METHODS
	 ************************************************************************/
	
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
		}else{
//			Debug.Log("Single Clicked");
			if(raise){
				if(rocketManager.Consume(fuelForShoot/2)){
					// increase orbit size
					myCollider.radius += 0.3f;
					raise=false;
					orbitLevel++;
				}
				else{
					rocketManager.SetInitialPosition();
					rocketManager.FullRefill();
				}
			}
			else{
				if(rocketManager.Consume(fuelForShoot/2)){
					// decrease orbit size
					myCollider.radius -= 0.3f;
					raise=true;
					orbitLevel--;
					// add force for continue collision
					rocket.rigidbody.AddForce(-(rocket.transform.position-this.transform.position).normalized * gravity*250);
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

	/************************************************************************
	 *  TIMER METHODS
	 ************************************************************************/
	
	void CountDown()
	{
		counting=true;
		counter--;
		text.text="EXPLODING IN ... "+counter;
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
			text.text="";
		}
	}

	void CountEnd()
	{
		if(collision){
			counting=true;
			counter--;
		}
		else{
			counting=false;
			counter=5;
		}
		if(counter < 1){
			CancelInvoke("CountEnd");
			levelCompleted=true;
			rocketManager.onStart=true;
			rocketManager.SetCollPlanet(this);
		}
	}

	public bool GetIfCounting(){
		return counting;
	}
	
	public void SetCounter(int cont){
		counter=cont;
	}
	
	public int GetCounter(){
		return counter;
	}

	/************************************************************************
	 *  TEXT METHODS
	 ************************************************************************/

	public void MoveText(Vector3 p){
		text.transform.position=p;
	}

	public void SetText(Text txt){
		text=txt;
	}

	/************************************************************************
	 *  PLANET METHODS
	 ************************************************************************/

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
	
	public void SetRotation(bool rotation){
		clockwise=rotation;
	}
	
	public void SetPlanetType(string type){
		planetType=type;
		if(type.Equals("count")){
			Renderer rend = GetComponent<Renderer>();
			rend.material.shader = Shader.Find("Specular");
			rend.material.SetColor("_Color", Color.yellow);
		}
		else{
			if(type.Equals("checkpoint")){
				Renderer rend = GetComponent<Renderer>();
				rend.material.shader = Shader.Find("Specular");
				rend.material.SetColor("_Color", Color.grey);
			}
			else{
				if(type.Equals("end")){
					Renderer rend = GetComponent<Renderer>();
					rend.material.shader = Shader.Find("Specular");
					rend.material.SetColor("_Color", Color.magenta);
				}
				else{
					Renderer rend = GetComponent<Renderer>();
					rend.material.shader = Shader.Find("Specular");
					rend.material.SetColor("_Color", Color.white);
				}
			}
		}
	}

	public bool GetCollision(){
		return collision;
	}

	public void SetMaxLevel(){
		if(orbitLevel==1){
			myCollider.radius += 0.3f;
			raise=false;
			orbitLevel=2;
		}
	}

	public void SetShoot(bool value){
		shoot=value;
	}

	/* if 0 destroy all
	 * if 1 destroy inSat
	 * if 2 destroy outSat
	 */
	public void DestroySatellite(int par){
		if(par==0){
			Destroy(inSatellite);
			Destroy(outSatellite);
		}
		if(par==1){
			Destroy(inSatellite);
		}
		if(par==2){
			Destroy(outSatellite);
		}
		if(par==3){
			;//destroy nothing
		}
	}
}