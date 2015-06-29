using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class tutorialScript : MonoBehaviour {

	int counter=0;
	float camSize;
	GameObject rocket,planet,wormhole,wormhole2,swing,blackHole,asteroid,asteroid2,asteroid3,asteroid4;
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
		if(counter==4){
			Destroy(swing);
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			// two planets and obstacles
			text.text="Try moving between planets without hitting asteroids or black hole";
			text2.text="";
			planet = Instantiate(Resources.Load("Planet")) as GameObject;
			planet.name="Planet";
			planet.transform.position=new Vector3(8,0,0);
			asteroid = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid.name="Asteroid";
			asteroid.transform.position=new Vector3(4,0,0);
			asteroid2 = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid2.name="Asteroid";
			asteroid2.transform.position=new Vector3(5,2.5f,0);
			asteroid3 = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid3.name="Asteroid";
			asteroid3.transform.position=new Vector3(9,-4,0);
			asteroid4 = Instantiate(Resources.Load("Asteroid")) as GameObject;
			asteroid4.name="Asteroid";
			asteroid4.transform.position=new Vector3(-3,-3,0);
			blackHole = Instantiate(Resources.Load("BlackHole")) as GameObject;
			blackHole.name="BlackHole";
			blackHole.transform.position=new Vector3(4,-2,0);
			blackHole.transform.localScale=new Vector3(0.35f,0.35f,0.35f);
			counter++;
		}
		else{
			if(counter==3){
				Destroy(wormhole);
				Destroy(wormhole2);
				Destroy(planet);
				rocketManager.SetInitialPosition();
				rocketManager.FullRefill();
				// planet and swing
				text.text="The right planet can be used for swinging the rocket";
				text2.text="";
				swing = Instantiate(Resources.Load("Swing")) as GameObject;
				swing.name="Swing";
				swing.transform.position=new Vector3(8,0,0);
				counter++;
			}
			else{
				if(counter==2){
					gen=false;
					rocketManager.SetInitialPosition();
					rocketManager.FullRefill();
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
						rocketManager.SetInitialPosition();
						rocketManager.FullRefill();
						// planet and exploding planet
						text.text="The planet on the right is an exploding planet";
						planetManager = planet.GetComponent ("Planet") as Planet;
						planetManager.SetPlanetType("count");
						gen=true;
						counter++;
					}
					else{
						if(counter==0){
							rocketManager.SetInitialPosition();
							rocketManager.FullRefill();
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
}
