using UnityEngine;
using System.Collections;

// Responsible for creating the barrels that fall at the beginning of the game
public class SpawnerController : MonoBehaviour {

	public GameObject oilDrumPrefab; // ref for the GameObject (pre-made) that gives rise to the 5 barrels (clones) we create

    // Use this for initialization
    void Start () {
        // Creating the 5 barrels and drawing a random position in x
        for ( int i = 0; i < 5; i++ ){
			float posX = Random.Range(-10f, 10f);
			GameObject oilDrum = Instantiate(oilDrumPrefab);
			oilDrum.transform.position = new Vector3( posX, transform.position.y, 0f );
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
