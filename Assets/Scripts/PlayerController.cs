using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float velocity; // player x speed
    public float bulletMaxInitialVelocity;  // initial velocity of the bullet
    public float maxTimeShooting; // maximum time shooting
    public BoxCollider2D groundBC;// ref to the BoxCollider2D of the floor
    public GameObject bulletPrefab; // ref for the GameObject (Pre-made) of the bullet

    private BoxCollider2D bc; // ref to the player's BoxCollider2D
    private Rigidbody2D rb; // ref to the player's Rigidbody2D
    private Animator an; // ref for Animator of GameObject Body
    private bool shooting; // the player is shooting?
    private float timeShooting; // time the player is shooting
    private Vector2 shootDirection; // ref for normalized Vector2 that points in the direction of the shot of the player

    public GameObject shootingEffect; // ref for the GameObject that contains the particle effect of the Player throwing
    public Transform gunTransform; // ref to the Transform of the GameObject Gun (Gun contains the sprite of the gun and the sight)
    public Transform bodyTransform; // ref for the Transform of the GameObject Body (Body contains the sprite of the body of the worm)
    public Transform bulletInitialTransform; // ref for the Transform that stores the initial position of the bullet
    public Transform knifeTransform; // ref to knife Transform.

    private bool targeting; // the player is watching?
    private bool knifing; // player is knifing

    // Use this for initialization
    void Start () {
		bc = GetComponent<BoxCollider2D>();
		rb = GetComponent<Rigidbody2D>();
        // Looking for an Animator-type component in the GameObjects children of Player
        // Actually we want the Animator component that is in the GameObject Body
        an = GetComponentInChildren<Animator>();
		//gunTransform.eulerAngles = new Vector3(0f, 0f, -30f);
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetKeyDown(KeyCode.Alpha1) )
        { // The weapon becomes visible
            targeting = true;
            knifing = false;
			gunTransform.gameObject.SetActive(true);
		}
        if (Input.GetKeyDown(KeyCode.Alpha2))
        { // The knife becomes visible
            knifing = true;
            targeting = false;
            knifeTransform.gameObject.SetActive(true);
        }
        if ( targeting ){
			UpdateTargetting();
			UpdateShootDetection();
			if( shooting )
				UpdateShooting();
		}
        if (knifing)
        {
            UpdateKnifing();      
        }
        else
        {
            knifing = false;
            targeting = false;
        }
        //gunTransform.localEulerAngles = new Vector3(0f, 0f, 30f);
        UpdateMove();
	}

    // Check if Player has started shooting
    void UpdateShootDetection(){
        // GetKeyDown returns true only in the update in which the player presses the key
        // GetKey returns true while the key is pressed
        // GetKeyUp returns true in update when the player releases the key
        if ( Input.GetMouseButtonDown(0)){
			shooting = true;
			shootingEffect.SetActive(true);
			timeShooting = 0f;
		}
	}

    // If the Player is shooting, mark the time the Plyer is firing and verifies
    // If the Player has stopped firing or has already passed the time limit of firing
    // Tb calls the Shoot () function, which effectively effects the trigger
    void UpdateShooting(){
		timeShooting += Time.deltaTime;
		if(  Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) ){
			shooting = false;
			shootingEffect.SetActive(false);
			Shoot();
            targeting = false;
		}
		if( timeShooting > maxTimeShooting ){
			shooting = false;
			shootingEffect.SetActive(false);
			Shoot ();
            targeting = false;
        }
	}

    // Function that creates a GameObject Bullet from bulletPrefab
    // Positions new bullet created
    // And tb directs it in the direction the player is looking at:
    // Vector2 that originates the player and destination the mouse position
    void Shoot(){
		Vector3 mousePosScreen = Input.mousePosition;
		Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
		Vector2 playerToMouse = new Vector2( mousePosWorld.x - transform.position.x,
		                                    mousePosWorld.y - transform.position.y);
		
		playerToMouse.Normalize();

		shootDirection = playerToMouse;
		Debug.Log("Shoot!");
		GameObject bullet = Instantiate(bulletPrefab);
		bullet.transform.position = bulletInitialTransform.position;
		bullet.GetComponent<Rigidbody2D>().velocity = shootDirection*bulletMaxInitialVelocity*(timeShooting/maxTimeShooting);
	}

    // Updating the rotation of the weapon and consequently of the aim based on where the player is looking
    // Tb we must update the scale of bodyTransform for the body of our Player to be in accordance with the direction in which the player is looking
    void UpdateTargetting(){
		Vector3 mousePosScreen = Input.mousePosition;
		Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePosScreen);
		Vector2 playerToMouse = new Vector2( mousePosWorld.x - transform.position.x,
		                                    mousePosWorld.y - transform.position.y);

		playerToMouse.Normalize();

		float angle = Mathf.Asin(playerToMouse.y)*Mathf.Rad2Deg;
		if( playerToMouse.x < 0f )
			angle = 180-angle;

		if( playerToMouse.x > 0f && bodyTransform.localScale.x > 0f ){
			bodyTransform.localScale = new Vector3(-bodyTransform.localScale.x, bodyTransform.localScale.y, 0f);
		}
		else if( playerToMouse.x < 0f && bodyTransform.localScale.x < 0f ){
			bodyTransform.localScale = new Vector3(-bodyTransform.localScale.x, bodyTransform.localScale.y, 0f);
		}

		gunTransform.localEulerAngles = new Vector3(0f, 0f, angle);
	}
    void UpdateKnifing()
    {

    }
    // Update the speed of our Player based on the keys pressed
    void UpdateMove(){
		if( (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            || (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)))
        {
			rb.velocity = new Vector2(velocity,rb.velocity.y);
			if( bodyTransform.localScale.x > 0f )
				bodyTransform.localScale = new Vector3( -bodyTransform.localScale.x, bodyTransform.localScale.y, 0f );

			an.SetBool("moving", true);
		}
		else if( (!Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            || (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)))
        {
            rb.velocity = new Vector2(-velocity, rb.velocity.y);
            if ( bodyTransform.localScale.x < 0f )
				bodyTransform.localScale = new Vector3( -bodyTransform.localScale.x, bodyTransform.localScale.y, 0f );

			an.SetBool("moving", true);
		}
        else if (Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, 5);
        }
        else
        {
			rb.velocity = new Vector2(0, rb.velocity.y);
            an.SetBool("moving", false);
		}
	}

    // Function called in every frame in which there is collision between the Collider of Player and another Collider
    void OnCollisionStay2D( Collision2D other ){
        // So we update the speed in x of the Player when this is not chao
        if ( other.collider.tag == "Ground" ){
			UpdateMove();
		}
	}
}
