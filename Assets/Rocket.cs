using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	
	public int fuel;
	public int maxFuel=2000;
	Vector3 initialPosition;
	Vector3 shootPosition;

	void Start () {
		initialPosition=new Vector3(0 ,1.4f,0);
		SetInitialPosition();
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.red);
		fuel=maxFuel;
	}

	void FixedUpdate () {
	}

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

	public void SetInitialPosition(){
		this.rigidbody.velocity = (new Vector3 (0, 0, 0));
		this.transform.position=initialPosition;
	}

	// use this function with checkpoint
	public void ChangeInitialPosition(Vector3 vec){
		initialPosition=vec;
	}

	public Vector3 GetInitialPosition(){
		return initialPosition;
	}

	public void SetShootPosition(Vector3 pos){
		shootPosition=pos;
	}
	
	public Vector3 GetShootPosition(){
		return shootPosition;
	}
}
