using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	/**
	 * 1: PCG_tutorial
	 * 2: PCG_continue
	 * 3: PCG_randomObject
	 * 4: PCG_obstacle
	 * 5: PCG_randomPlace
	 */

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
		}
	}
}
