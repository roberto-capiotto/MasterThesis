using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {
	
	bool collided=false;
	//float acceleration=80.0f;
	public int fuel;
	public int maxFuel=1000;
	Vector3 initialPosition;
	//SphereCollider myCollider;

	// TODO: define initialPosition
	// now fixed
	// then there will be checkpoint or something other
	void Start () {
		//myCollider = transform.GetComponent<SphereCollider>();
		initialPosition=new Vector3(0 ,1.6f,0);
		SetInitialPosition();
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.red);
		fuel=maxFuel;
	}

	void Update () {
	}

	void OnCollisionEnter (Collision collision) {
		collided=true;	
	}

	void OnCollisionExit(Collision collision){
		collided=false;
	}

	public void Refill(int num){
		if(fuel+num<maxFuel)
			fuel+=num;
		else
			fuel=maxFuel;
	}

	public void SetInitialPosition(){
		this.rigidbody.velocity = (new Vector3 (0, 0, 0));
		this.transform.position=initialPosition;
	}

	public Vector3 GetInitialPosition(){
		return initialPosition;
	}
}
