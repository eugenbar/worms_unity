using UnityEngine;
using System.Collections;

// Class that contains the OnClick () function, which is called by clicking the button
public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // OnClick () function changes scene
    // Remember that the scenes that the game will contain are in File-> Build Settings
    public void OnClick(){
		Application.LoadLevel("wormScene");
	}
}
