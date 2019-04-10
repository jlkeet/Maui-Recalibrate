using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	// Floats
	public float maxSpeed = 3.0f;
	public float speed = 35.0f;
	public float minJumpPower;
	public float maxJumpPower;


	// Booleans
	public bool grounded = false;
	public bool jump = false;
	public bool jumpCancel = false;
	public bool playerDisabled = false;
	public bool facingLeft;
	public bool facingRight;

	// Player Stats
	public int curHealth;
	public int maxHealth = 5;

	// Audio
	public AudioSource[] sounds;
	public AudioSource noise1;
	public AudioSource noise2;
	public AudioSource knuckleBone;

	// Animation
	private Animator anim;

	// Other Components
	private Rigidbody2D rb2d;
	private gameMaster gm;

	private int _playerLayer;
	private int _platformLayer;

	private float _vx;
	private float _vy;

	private PlayerAttackTwo attackTwo;


	// Ground Check

	public LayerMask whatIsGround;
	public Transform groundCheck;
	Transform _transform;


	void Awake() {
		
		// determine the player's specified layer
		_playerLayer = this.gameObject.layer;

		// determine the platform's specified layer
		_platformLayer = LayerMask.NameToLayer("Platform");

		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();

		attackTwo = GetComponent<PlayerAttackTwo> ();
	}

	// Use this for initialization
	void Start () {
		rb2d = gameObject.GetComponent<Rigidbody2D> ();
		anim = gameObject.GetComponent<Animator> ();

		sounds = GetComponents<AudioSource> ();
		noise1 = sounds [0];
		noise2 = sounds [1];
		knuckleBone = sounds [2];

		curHealth = maxHealth;
		gm = GameObject.FindGameObjectWithTag ("GameMaster").GetComponent<gameMaster> ();
	}

	// Update is called once per frame
	void Update () {

		// Check to see if character is grounded by raycasting from the middle of the player
		// down to the groundCheck position and see if collected with gameobjects on the
		// whatIsGround layer
		grounded = Physics2D.Linecast(_transform.position, groundCheck.position, whatIsGround);  

		// get the current vertical velocity from the rigidbody component
		_vy = rb2d.velocity.y;
		Physics2D.IgnoreLayerCollision(_playerLayer, _platformLayer, (_vy > 0.0f)); 

		// Check player speed and if grounded
		anim.SetBool ("Grounded", grounded);
		anim.SetFloat ("Speed", Mathf.Abs (rb2d.velocity.x));

		// Check player control methods
		playerMoving ();
		playerMove ();

		// Health methods

		if (curHealth > maxHealth) {
			curHealth = maxHealth;
		}

		if (curHealth < 0) {
			curHealth = 0;
		}

		if (curHealth <= 0) {
			Die ();
		}

		if (Input.GetButtonDown("Jump") && grounded)   // Player starts pressing the button
			jump = true;
		if (Input.GetButtonUp("Jump") && !grounded)     // Player stops pressing the button
			jumpCancel = true;
	}

	void FixedUpdate () {

		Vector3 easeVelocity = rb2d.velocity;
		easeVelocity.y = rb2d.velocity.y;
		easeVelocity.z = 0.0f;
		easeVelocity.x *=0.75f;

		// Fake friction for easing x speed of player.
		if (grounded) {
			rb2d.velocity = easeVelocity;
		}

		// Check if attacking
		stopMoveWhileAttack ();
		playerJumping ();
	}

	void playFootsteps () {
		noise2.Play();
	}

	IEnumerator DeathOne() {

		// freeze the player
		FreezeMotion();

		// play the death animation
		anim.SetTrigger("DeathOne");

		// After waiting tell the GameManager to reset the game
		yield return new WaitForSeconds(3.0f);

		// Restart
		//Application.LoadLevel (Application.loadedLevel);
		UnityEngine.SceneManagement.SceneManager.LoadScene("Testing1");
		UnFreezeMotion ();
	}

	public void Die() {
		StartCoroutine (DeathOne());
	}

	void playerMoving() {
		if (Input.GetAxis ("Horizontal") < -0.1f) {
			facingLeft = true;
			facingRight = false;
			transform.localScale = new Vector3 (-0.3f, 0.3f, 0.3f);
		}
		if (Input.GetAxis ("Horizontal") > 0.1f) {
			facingLeft = false;
			facingRight = true;
			transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		}
	}

	public void playerJumping() {

		// Normal jump (full speed)
		if (jump)
		{
			rb2d.velocity = new Vector2(rb2d.velocity.x, maxJumpPower);
			jump = false;
			noise1.Play ();
		}
		// Cancel the jump when the button is no longer pressed
		if (jumpCancel)
		{
			if (rb2d.velocity.y > minJumpPower)
				rb2d.velocity = new Vector2(rb2d.velocity.x, minJumpPower);
			jumpCancel = false;
		}
			
	}

	void stopMoveWhileAttack() {

		//Check to see if player is attacking, if not then movement is ok
		if (!gameObject.GetComponent<PlayerAttackTwo>().attackingTwo) {
			// Player Movement
			playerMove();
	
				if (rb2d.velocity.x > maxSpeed) {
					rb2d.velocity = new Vector2 (maxSpeed, rb2d.velocity.y);
				}
				if (rb2d.velocity.x < -maxSpeed) {
					rb2d.velocity = new Vector2 (-maxSpeed, rb2d.velocity.y);
				}
			}
		}

	public void DamageTaken(int dmg) {
		curHealth -= dmg;
		gameObject.GetComponent<Animation> ().Play ("PlayerDamageFlash");
	}

	public IEnumerator Knockback(float knockDur, float knockbPwr, Vector3 knockbDir) {
		float timer = 0;
		playerDisabled = true;
		rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
		while (knockDur > timer) {
			timer += Time.deltaTime;
			if (facingRight) {
				rb2d.AddForce (new Vector3 (transform.localScale.x * -250, knockbPwr, transform.position.z));
			}
			if (facingLeft) {
				rb2d.AddForce (new Vector3 (transform.localScale.x * -250, knockbPwr, transform.position.z));
			}
		}
		yield return 0;
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag ("Currency")) {
			knuckleBone.Play ();
			Destroy (col.gameObject);
			gm.points += 1;
		}
	}

	// if the player collides with a MovingPlatform, then make it a child of that platform
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
	}

	// if the player exits a collision with a moving platform, then unchild it
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
	}

	public void playerMove() {
		
		float h = Input.GetAxis ("Horizontal");

		if (grounded && playerDisabled == true) {
			playerDisabled = false;
		}

		// Check if player is disabled
		if (!playerDisabled) {
			// Player movement controller
			rb2d.AddForce ((Vector2.right * speed) * h);
		}
	}

	// public function on victory over the level
	public void Victory() {
		//PlaySound(victorySFX);
		StartCoroutine (NextLevel());
	}
		
	IEnumerator NextLevel() {

		// freeze the player
		FreezeMotion();

		// play the victory animation
		anim.SetTrigger("Victory");

		// After waiting tell the GameManager to reset the game
		yield return new WaitForSeconds(3.0f);

		// Restart
		UnityEngine.SceneManagement.SceneManager.LoadScene("Testing1");
		UnFreezeMotion ();
	}

	void FreezeMotion() {
		playerDisabled = true;
		rb2d.isKinematic = true;
		rb2d.simulated = false;
	}

	void UnFreezeMotion() {
		playerDisabled = false;
		rb2d.isKinematic = false;
		rb2d.simulated = true;
	}

	// To handle triggered attack

	public void SetAttackColliderAnim() {
		attackTwo.attackTriggerTwo.enabled = true;
	}

}

