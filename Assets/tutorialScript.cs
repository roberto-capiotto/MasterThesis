using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class tutorialScript : MonoBehaviour {

	int counter=0;
	float camSize;
	GameObject rocket,planet,wormhole,wormhole2;
	Rocket rocketManager;
	Planet planetManager;
	Wormhole wormholeManager,wormhole2Manager;
	Vector3 initialPosition;
	public Text text;
	public Text text2;
	int cont=100;
	bool gen=false;

	void Start () {
		rocket = GameObject.Find ("Rocket");
		rocketManager = rocket.GetComponent ("Rocket") as Rocket;
		camSize = Camera.main.orthographicSize;
		initialPosition=Camera.main.transform.position;
	}

	void Update () {
		if(rocket.transform.position.y>camSize || rocket.transform.position.y<-camSize ||
		   rocket.transform.position.x-this.transform.position.x>2*camSize || rocket.transform.position.x<-2*camSize){

			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			this.transform.position=initialPosition;
		}

		if(gen){
			text2.text="";
			if(planetManager.GetIfCounting()){
				cont = planetManager.GetCounter();
				text2.text="Time left: "+cont.ToString();
			}
			if(cont<=0)
				text2.text="Planet Destroyed";
		}
	}

	public void Change(){
		if(counter==3){
			//TODO:
			Destroy(wormhole);
			Destroy(wormhole2);
			Destroy(planet);
			// planet and swing
			text.text="Wormhole move the rocket from a point to another";
			text2.text="and shoots it only right";
			counter++;
		}
		else{
			if(counter==2){
				gen=false;
				// two planets and wormholes
				planetManager.SetPlanetType("");
				text.text="Wormhole move the rocket from a point to another";
				text2.text="and shoots it only right";
				wormhole = Instantiate(Resources.Load("Wormhole")) as GameObject;
				wormhole.name="Wormhole";
				wormhole.transform.position=new Vector3(4,1,0);
				wormhole2 = Instantiate(Resources.Load("Wormhole")) as GameObject;
				wormhole2.name="Wormhole";
				wormhole2.transform.position=new Vector3(4,-1,0);
				wormholeManager = wormhole.GetComponent ("Wormhole") as Wormhole;
				wormhole2Manager = wormhole2.GetComponent ("Wormhole") as Wormhole;
				wormholeManager.SetExit(wormhole2);
				wormhole2Manager.SetExit(wormhole);
				counter++;
			}
			else{
				if(counter==1){
					// planet and exploding planet
					text.text="The planet on the right is an exploding planet";
					planetManager = planet.GetComponent ("Planet") as Planet;
					planetManager.SetPlanetType("count");
					gen=true;
					counter++;
				}
				else{
					if(counter==0){
						// two planets
						text.text="Try to shoot the rocket from a planet to another";
						text2.text="";
						initialPosition=new Vector3(4,0,-10);
						Camera.main.transform.position=initialPosition;
						planet = Instantiate(Resources.Load("Planet")) as GameObject;
						planet.name="Planet";
						planet.transform.position=new Vector3(8,0,0);
						counter++;
					}
				}
			}
		}
	}
}
