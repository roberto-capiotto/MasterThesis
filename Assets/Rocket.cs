using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	
	public int fuel;
	public int maxFuel=2000;
	Vector3 initialPosition;
	Vector3 shootPosition;
	public float timer;
	public bool onStart=true;
	public bool colliding=true;
	bool replace;
	bool check=false;
	Planet collPlanet;
	DockingStation dock;

	void Start () {
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.red);
		fuel=maxFuel;
	}

	void FixedUpdate () {
	}

	/************************************************************************
	 *  FUEL METHODS
	 ************************************************************************/

	public void FullRefill(){
		fuel=maxFuel;
	}

	public void Refill(int propellant){
		if(fuel+propellant<maxFuel)
			fuel+=propellant;
		else
			fuel=maxFuel;
	}

	public bool Consume(int propellant){
		fuel-=propellant;
		if(fuel<0){
			print ("NOT ENOUGH FUEL");
			return false;
		}
		return true;
	}

	public int GetFuel(){
		return fuel;
	}

	/************************************************************************
	 *  POSITION METHODS
	 ************************************************************************/

	public void SetInitialPosition(){
		this.rigidbody.velocity = (new Vector3 (0, 0, 0));
		this.transform.position=initialPosition;
		onStart=true;
	}

	public void ChangeInitialPosition(Vector3 vec){
		initialPosition=vec;
		print ("CHANGED! x:"+initialPosition.x+" y: "+initialPosition.y);
	}

	public Vector3 GetInitialPosition(){
		return initialPosition;
	}

	public void SetReplace(bool flag){
		replace=flag;
	}

	public bool GetReplace(){
		return replace;
	}
	
	public void SetShootPosition(Vector3 pos){
		shootPosition=pos;
	}
	
	public Vector3 GetShootPosition(){
		return shootPosition;
	}

	/************************************************************************
	 *  CURRENT STATUS METHODS
	 ************************************************************************/

	public void SetOnStart(bool start){
		onStart=start;
	}

	public void SetColliding(bool coll){
		colliding=coll;
	}

	public bool GetColliding(){
		return colliding;
	}

	public void SetCollPlanet(Planet p){
		collPlanet=p;
	}

	public Planet GetCollPlanet(){
		return collPlanet;
	}

	public void SetCollDocking(DockingStation d){
		dock=d;
	}
	
	public DockingStation GetCollDocking(){
		return dock;
	}

	public bool GetCheck(){
		return check;
	}

	public void SetCheck(bool checkFlag){
		check=checkFlag;
	}

	/************************************************************************
	 *  TIMER METHODS
	 ************************************************************************/

	public void SetTimer(float t){
		timer=t;
	}
	
	public float GetTimer(){
		return timer;
	}
}
