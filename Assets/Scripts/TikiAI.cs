using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikiAI : MonoBehaviour {

	// Integers 
	public int curHealth;
	public int maxHealth;

	// Floats
	public float distance;
	public float wakeRange;
	public float shootInterval;
	public float bulletSpeed = 100;
	public float bulletTimer;


	// Booleans
	public bool awake = false;

	// References
	public GameObject bullet;
	public Transform target;
	public Animator anim;
	public Transform shootPointLeft;
	public Player player;


	void Awake() {

	}


	void Start() {
        anim = gameObject.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        curHealth = maxHealth;
	}


	void Update() {

        anim.SetBool ("Awake", awake);
		RangeCheck ();
	}

	void RangeCheck() {

		distance = Vector3.Distance (transform.position, target.transform.position);

		if (distance < wakeRange) {
			awake = true;
		}

		if (distance > wakeRange) {
			awake = false;
		}

		if (curHealth <= 0) {
			Destroy (gameObject);
			player.Victory ();
		}
	}

	public void Attack() {
		bulletTimer += Time.deltaTime;

			if (bulletTimer >= shootInterval) {
				Vector2 direction = target.transform.position - transform.position;
				direction.Normalize ();

				GameObject bulletClone;
				bulletClone = Instantiate (bullet, shootPointLeft.transform.position, shootPointLeft.transform.rotation) as GameObject;
				bulletClone.GetComponent<Rigidbody2D> ().velocity = direction * bulletSpeed;
				bulletTimer = 0;
		}
	}

	public void Damage(int damage) {
		curHealth -= damage;
		gameObject.GetComponent<Animation> ().Play ("PlayerDamageFlash");
	}
}
