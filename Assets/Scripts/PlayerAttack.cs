using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

	public bool attacking = false;

	private float attackTimer = 0;
	private float attackCd = 0.6f;

	public Collider2D attackTrigger;

	private Player player;
	private Animator anim;
	private Rigidbody2D rb2d;

	void Awake() {
		
		rb2d = GetComponent<Rigidbody2D> ();
		anim = gameObject.GetComponent<Animator> ();
		attackTrigger.enabled = false;
	}

	void Update() {

		player = gameObject.GetComponentInParent<Player> ();

		if (Input.GetKeyDown ("f") || Input.GetKeyDown("joystick button 1") && !attacking && !player.grounded) {
			attacking = true;
			attackTimer = attackCd;
			rb2d.velocity = Vector3.down * 8;
			attackTrigger.enabled = true;
		}

		if (attacking) {

			if (attackTimer > 0) {
				attackTimer -= Time.deltaTime;
			} else {
				attacking = false;
				attackTrigger.enabled = false;
			}
		}

		anim.SetBool ("Attacking", attacking);
	}
}
