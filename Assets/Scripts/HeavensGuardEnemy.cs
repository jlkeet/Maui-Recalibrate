using UnityEngine;
using System.Collections;

public class HeavensGuardEnemy: MonoBehaviour {

	public float moveSpeed = 2.0f;  // enemy move speed when moving
	public int damageAmount = 1; // probably deal a lot of damage to kill player immediately

	public float stunnedTime = 0.02f;   // how long to wait at a waypoint

	public string stunnedLayer = "StunnedEnemy";  // name of the layer to put enemy on when stunned
	public string playerLayer = "Player";  // name of the layer to put enemy on when stunned

	public bool isStunned = false;  // flag for isStunned

	public GameObject[] myWaypoints; // to define the movement waypoints

	public float waitAtWaypointTime = 1f;   // how long to wait at a waypoint

	public bool loopWaypoints = true; // should it loop through the waypoints

	public bool oneTime = false;

	// Integers 
	public int curHealth;
	public int maxHealth;

	// SFXs
	public AudioClip stunnedSFX;
	public AudioClip attackSFX;
	public AudioSource deathSFX;
	public AudioSource[] enemySounds;

	// private variables below

	// store references to components on the gameObject
	Transform _transform;
	Rigidbody2D _rigidbody;
	Animator _animator;
	AudioSource _audio;
	Player player;

	// movement tracking
	int _myWaypointIndex = 0; // used as index for My_Waypoints
	float _moveTime; 
	float _vx = 0f;
	bool _moving = true;

	// store the layer number the enemy is on (setup in Awake)
	int _enemyLayer;

	// store the layer number the enemy should be moved to when stunned
	int _stunnedLayer;

	void Awake() {

		enemySounds = GetComponents<AudioSource> ();

		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();

		_rigidbody = GetComponent<Rigidbody2D> ();
		if (_rigidbody==null) // if Rigidbody is missing
			Debug.LogError("Rigidbody2D component missing from this gameobject");

		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError("Animator component missing from this gameobject");

		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}

		// setup moving defaults
		_moveTime = 0f;
		_moving = true;

		// determine the enemies specified layer
		_enemyLayer = this.gameObject.layer;

		// determine the stunned enemy layer number
		_stunnedLayer = LayerMask.NameToLayer(stunnedLayer);

		// make sure collision are off between the playerLayer and the stunnedLayer
		// which is where the enemy is placed while stunned
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), _stunnedLayer, true); 

		curHealth = maxHealth;
	}

	void Start() {
		deathSFX = enemySounds[0];
	}

	// if not stunned then move the enemy when time is > _moveTime
	void Update () {
		if (!isStunned)
		{
			if (Time.time >= _moveTime) {
				EnemyMovement();
			} else {
				_animator.SetBool("Moving", false);
			}
		}

		if (curHealth <= 0) {
			StartCoroutine (HeavenDie ());

			if (!oneTime) {
				playSFX ();
				oneTime = true;
			}
		}
	}

	// Move the enemy through its rigidbody based on its waypoints
	void EnemyMovement() {
		// if there isn't anything in My_Waypoints
		if ((myWaypoints.Length != 0) && (_moving)) {

			// make sure the enemy is facing the waypoint (based on previous movement)
			Flip (_vx);

			// determine distance between waypoint and enemy
			_vx = myWaypoints[_myWaypointIndex].transform.position.x-_transform.position.x;

			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if (Mathf.Abs(_vx) <= 0.05f) {
				// At waypoint so stop moving
				_rigidbody.velocity = new Vector2(0, 0);

				// increment to next index in array
				_myWaypointIndex++;

				// reset waypoint back to 0 for looping
				if(_myWaypointIndex >= myWaypoints.Length) {
					if (loopWaypoints)
						_myWaypointIndex = 0;
					else
						_moving = false;
				}

				// setup wait time at current waypoint
				_moveTime = Time.time + waitAtWaypointTime;
			} else {
				// enemy is moving
				_animator.SetBool("Moving", true);

				// Set the enemy's velocity to moveSpeed in the x direction.
				_rigidbody.velocity = new Vector2(_transform.localScale.x * moveSpeed, _rigidbody.velocity.y);
			}

		}
	}

	// flip the enemy to face torward the direction he is moving in
	void Flip(float _vx) {

		//		// get the current scale
		Vector3 localScale = _transform.localScale;

		if ((_vx < 0f) && (localScale.x <= 0.80f)) {
			//Debug.Log ("Going Left!");
			localScale = new Vector3 (-0.80f, 0.80f, 1.0f);
		} else if ((_vx > 0f) && (localScale.x >= -0.80f)) {
			//Debug.Log ("Going Right!");
			localScale = new Vector3 (0.80f, 0.80f, 1.0f);
		}
		//		// update the scale
		_transform.localScale = localScale;
	}

	// Attack player
	//	void OnTriggerEnter(Collider2D collision)
	//	{
	//		//Debug.Log ("Entered Collider");
	//		if (collision.isTrigger != true) {
	//			if ((collision.tag == "Player") && !isStunned)
	//			{
	//				Debug.Log ("Knows its player");
	//				Player player = collision.gameObject.GetComponent<Player>();
	//			if (!player.playerDisabled) {
	//				// Make sure the enemy is facing the player on attack
	//				Flip(collision.transform.position.x-_transform.position.x);
	//				
	//				// attack sound
	//				playSound(attackSFX);
	//				
	//				// stop moving
	//				 _rigidbody.velocity = new Vector2(0, 0);
	//				
	//				// apply damage to the player
	//				player.DamageTaken (damageAmount);
	//				
	//				// stop to enjoy killing the player
	//				 _moveTime = Time.time + stunnedTime;
	//			}
	//		}
	//	}
	//	}


	void OnTriggerEnter2D(Collider2D col) {

		Player player = col.gameObject.GetComponent<Player>();
		if (col.isTrigger != true) {
			if (col.CompareTag ("Player")) {
				if (!player.playerDisabled) {
					// Make sure the enemy is facing the player on attack
					Flip (col.transform.position.x - _transform.position.x);

					// attack sound
					//playSound(attackSFX);
					if (!GameObject.Find("Player").GetComponent<PlayerAttack>().attacking) {
						player.DamageTaken (1);
						StartCoroutine (player.Knockback (0.05f, 240.0f, player.transform.position));
					}

					// stop moving
					_rigidbody.velocity = new Vector2 (0, 0);

					// stop to enjoy killing the player
					_moveTime = Time.time + stunnedTime;
				}
			}
		}
	}

	// if the Enemy collides with a MovingPlatform, then make it a child of that platform
	// so it will go for a ride on the MovingPlatform
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
	}

	// if the enemy exits a collision with a moving platform, then unchild it
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
	}

	// play sound through the audiosource on the gameobject
	void playSound(AudioClip clip)
	{
		_audio.PlayOneShot(clip);
	}

	// setup the enemy to be stunned
	public void Stunned()
	{
		HeavensGuardEnemy heavensGuard = gameObject.GetComponent<HeavensGuardEnemy>();
		if (!isStunned) 
		{
			isStunned = true;

			// provide the player with feedback that enemy is stunned
			//playSound(stunnedSFX);
			//_animator.SetTrigger("EnemyHit");
			if (curHealth > 0) {
				StartCoroutine (heavensGuard.Knockback (0.03f, 80.0f, heavensGuard.transform.position));
			}
			//			// stop moving
			//			_rigidbody.velocity = new Vector2(0, 0);

			// switch layer to stunned layer so no collisions with the player while stunned
			this.gameObject.layer = _stunnedLayer;

			// start coroutine to stand up eventually
			StartCoroutine (Stand ());

		}
	}

	// coroutine to unstun the enemy and stand back up
	IEnumerator Stand()
	{
		yield return new WaitForSeconds(stunnedTime); 

		// no longer stunned
		isStunned = false;

		// switch layer back to regular layer for regular collisions with the player
		this.gameObject.layer = _enemyLayer;

		// provide the player with feedback
		_animator.SetTrigger("EnemyStand");
	}

	IEnumerator HeavenDie()
	{
		FreezeMotion ();
		_animator.SetTrigger ("HeavensDeath");

		yield return new WaitForSeconds(0.333f);

		Destroy (gameObject);

	}

	void FreezeMotion() {
		//playerDisabled = true;
		_rigidbody.isKinematic = true;
		_rigidbody.simulated = false;
	}

	public IEnumerator Knockback(float knockDur, float knockbPwr, Vector3 knockbDir) {
		float timer = 0;
		// playerDisabled = true;
		// rb2d.velocity = new Vector2 (rb2d.velocity.x, 0);
		_animator.SetTrigger("EnemyHit");

		// stop moving
		_rigidbody.velocity = new Vector2(0, 0);
		while (knockDur > timer) {
			timer += Time.deltaTime;
			_rigidbody.AddForce (new Vector3 (transform.localScale.x * -15, knockbPwr, transform.position.z));
			//			if (facingLeft) {
			//				_rigidbody.AddForce (new Vector3 (transform.localScale.x * -250, knockbPwr, transform.position.z));
			//			}
		}
		yield return 0;
	}

	void playSFX() {

		// attack sound
		deathSFX.Play();
	}

	public void Damage(int damage) {
		curHealth -= damage;
		Stunned ();
	}
}
