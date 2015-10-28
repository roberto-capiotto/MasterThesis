using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public Button tutorialButton;
	public Button continueButton;
	public Button obstacleButton;
	public Button randomObjectButton;
	public Button randomPlaceButton;
	public Button quitButton;

	public Text title;
	public Text subtitle;

	void Start(){
		Vector3 coord = Vector3.zero;
		coord.x = -Screen.width/3+80;
		coord.y = Screen.height/6+30;
		tutorialButton.GetComponent<RectTransform>().localPosition = coord;

		coord.x = 0;
		continueButton.GetComponent<RectTransform>().localPosition = coord;

		coord.x = Screen.width/3-80;
		obstacleButton.GetComponent<RectTransform>().localPosition = coord;

		coord.x = -Screen.width/4+80;
		coord.y = -Screen.height/6+30;
		randomObjectButton.GetComponent<RectTransform>().localPosition = coord;

		coord.x = Screen.width/4-80;
		randomPlaceButton.GetComponent<RectTransform>().localPosition = coord;

		coord.x = 0;
		coord.y = -Screen.height/3+30;
		quitButton.GetComponent<RectTransform>().localPosition = coord;
		
		coord.x = 0;
		coord.y = Screen.height/3+20;
		title.rectTransform.localPosition = coord;
		
		coord.y = -Screen.height/10*4.5f+15;
		subtitle.rectTransform.localPosition = coord;
	}

	public void Select(int value){
		switch(value){
		case 1:
			Application.LoadLevel("PCG-tutorial");
			break;
		case 2:
			Application.LoadLevel("PCG-continue");
			break;
		case 3:
			Application.LoadLevel("PCG-random-objects");
			break;
		case 4:
			Application.LoadLevel("PCG-obstacle");
			break;
		case 5:
			Application.LoadLevel("PCG-random-place");
			break;
		case 6:
			Application.Quit();
			break;
		}
	}
}
