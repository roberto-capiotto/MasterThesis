using UnityEngine;
using System.Collections;

public class SatelliteScript : MonoBehaviour {
	
	GameObject rocket,planet;
	Rocket rocketManager;
	bool collision=false;
	bool clockwise=false;
	
	void Start () {
		// each satellite may have different dimensions
		// this.transform.localScale=new Vector3(Random.Range(0.4f,0.6f),Random.Range(0.4f,0.6f),1);

		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		
		Renderer rend = GetComponent<Renderer>();
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_Color", Color.green);

		planet = this.transform.parent.gameObject;
	}

	void FixedUpdate(){
		if(collision){
			//	rocket die
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			collision=false;
		}
		if(clockwise)
			transform.RotateAround(planet.transform.position, -Vector3.forward, 10 * Time.deltaTime);
		else
			transform.RotateAround(planet.transform.position, Vector3.forward, 10 * Time.deltaTime);
	}
	
	void OnCollisionEnter()
	{
		collision=true;
	}

	/* if rotation=true  --> clockwise
	 * if rotation=false --> counterclockwise
	 */
	public void SetRotation(bool rotation){
		clockwise=rotation;
	}

	public bool IsUp(){
		if(this.transform.position.y>planet.transform.position.y+planet.transform.localScale.y/4)
			return true;
		return false;
	}

	public bool IsDown(){
		if(this.transform.position.y<planet.transform.position.y-planet.transform.localScale.y/4)
			return true;
		return false;
	}

	public bool IsRight(){
		if(this.transform.position.x>planet.transform.position.x+planet.transform.localScale.x/4)
			return true;
		return false;
	}
	
	public bool IsLeft(){
		if(this.transform.position.x<planet.transform.position.x-planet.transform.localScale.x/4)
			return true;
		return false;
	}
}