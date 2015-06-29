using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class tutorialScript : MonoBehaviour {

	int counter=0;
	float camSize;
	GameObject rocket,planet,wormhole,wormhole2,swing,blackHole,asteroid,asteroid2,asteroid3,asteroid4,dockingStation;
	Rocket rocketManager;
	Planet planetManager;
	Wormhole wormholeManager,wormhole2Manager;
	Vector3 initialPosition;
	public Text text;
	public Text text2;
	bool fuel=false;

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

		if(fuel){
			text2.text="Fuel remaining: "+rocketManager.GetFuel();
		}
	}

	public void Change(){
		if(counter==7){
			Application.LoadLevel("firstCollision");
		}
		if(counter==6){
			// end of tutorial
			fuel=false;
			Destroy(planet);
			Destroy(dockingStation);
			rocketManager.SetInitialPosition();
			rocketManager.FullRefill();
			initialPosition=new Vector3(0,0,-10);
			Camera.main.transform.position=initialPosition;
			text.text="This is the end of the tutorial";
			text2.text="Now you are ready for playing the game";
			counter++;
		}
		else{
			if(counter==5){
				// two planets and docking station
				Destroy(asteroid);
				Destroy(asteroid2);
				Destroy(asteroid3);
				Destroy(asteroid4);
				Destroy(blackHole);
				rocketManager.SetInitialPosition();
				rocketManager.FullRefill();
				text.text="Try moving between planet. The docking station can refill you";
				text2.text="";
				dockingStation = Instantiate(Resources.Load("DockingStation")) as GameObject;
				dockingStation.name="DockingStation";
				dockingStation.transform.position=new Vector3(10,-3.5f,0);
				fuel=true;
				counter++;
			}
			else{
				if(counter==4){
					// two planets and obstacles
					Destroy(swing);
					rocketManager.SetInitialPosition();
					rocketManager.FullRefill();
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
						// planet and swing
						Destroy(wormhole);
						Destroy(wormhole2);
						Destroy(planet);
						rocketManager.SetInitialPosition();
						rocketManager.FullRefill();
						text.text="The right planet can be used for swinging the rocket";
						text2.text="";
						swing = Instantiate(Resources.Load("Swing")) as GameObject;
						swing.name="Swing";
						swing.transform.position=new Vector3(8,0,0);
						counter++;
					}
					else{
						if(counter==2){
							// two planets and wormholes
							rocketManager.SetInitialPosition();
							rocketManager.FullRefill();
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
								rocketManager.SetInitialPosition();
								rocketManager.FullRefill();
								text.text="The planet on the right is an exploding planet";
								planetManager = planet.GetComponent ("Planet") as Planet;
								planetManager.SetPlanetType("count");
								planetManager.SetText(text2);
								counter++;
							}
							else{
								if(counter==0){
									// two planets
									rocketManager.SetInitialPosition();
									rocketManager.FullRefill();
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
	}
}
