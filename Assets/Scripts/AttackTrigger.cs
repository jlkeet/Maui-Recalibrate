using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour {

	public int dmg = 20;
	public Player player;

	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.isTrigger != true && col.CompareTag("Enemy")) {
			col.SendMessageUpwards ("Damage", dmg);

			// Bounce off enemy
			player.jump = true;
			player.playerJumping ();
		}
			
	}
}
