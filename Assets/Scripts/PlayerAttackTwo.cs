using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTwo : MonoBehaviour {

	public bool attackingTwo = false;

	private float attackTimer = 0;
	private float attackCd = 0.2f;

	public Collider2D attackTriggerTwo;

	private Animator anim2;
	public Player playerOne;

	void Awake() {
		
		anim2 = gameObject.GetComponent<Animator> ();
		attackTriggerTwo.enabled = false;
	}

	void Update() {
		
		if ((Input.GetKeyDown ("r") || Input.GetKeyDown("joystick button 0")) && !attackingTwo) {
			attackingTwo = true;
			attackTimer = attackCd;

			// Old way of attacking, new way has collider triggered on specific frame (Animation Even Handled)

			//attackTriggerTwo.enabled = true;
			//playerOne.SetAttackColliderAnim();
		}

		anim2.SetBool ("AttackingTwo", attackingTwo);

		if (attackingTwo) {

			if (attackTimer > 0) {
				attackTimer -= Time.deltaTime;
			} else {
				attackingTwo = false;
				attackTriggerTwo.enabled = false;
			}
		}


	}

}