using UnityEngine;
using System.Collections;

// Class responsible for the code that controls the bullet
public class BulletController : MonoBehaviour {

	private Rigidbody2D rb;
    // Ref to Rigidbody2D of bullet
    public Transform bulletSpriteTransform;
    // Ref to the transform of the GameObject Sprite that is inside the GameObject Bullet
    private bool updateAngle = true;
    // bool that says whether or not to update the GameObject Sprite rotation based on traj. Bullet
    // This bool is to say that after the bullet collides with some other body, the rotation
    // the bullet should no longer be updated based on the trajectory
    public GameObject bulletSmoke;
    // Ref to the gameObject BulletSmoke, which contains the particle system that makes the bullet trail
    public CircleCollider2D destructionCircle;
	public static GroundController groundController;
    public GameObject explosion;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		//rb.velocity = new Vector2(5f, 10f);
	}
	
	// Update is called once per frame
	void Update () {
		if( updateAngle ){
			Vector2 dir = new Vector2(rb.velocity.x, rb.velocity.y);
            // Determination of the angle of the velocity vector
            dir.Normalize();			
			float angle = Mathf.Asin (dir.y)*Mathf.Rad2Deg;
			if( dir.x < 0f ){
				angle = 180 - angle;
			}

            //Debug.Log("angle = " + angle);


            // Updating the Sprite rotation (GameObject containing the Sprite Render of our bullet)
            // according to the angle of the trajectory
            bulletSpriteTransform.localEulerAngles = new Vector3(0f, 0f, angle+45f);
		}
	}

	void OnCollisionEnter2D( Collision2D coll ){
        // When the bullet collides with another body other than the Player it
        // no longer updates rotation based on path
        // and the particle effect of the bullet trail is disabled
        if ( coll.collider.tag == "Ground" ){
            GameObject exp = Instantiate(explosion);
            exp.transform.position = this.transform.position;
            exp.SetActive(true);
            updateAngle = false;
			bulletSmoke.SetActive(false);
			groundController.DestroyGround( destructionCircle );
			Destroy(gameObject);
		}
	}
}
