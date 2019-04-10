using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {

	private Player player;
	private Rigidbody2D rb2d;

	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}

	void OnTriggerEnter2D(Collider2D col) {

		if (col.CompareTag("Player")) {
			player.DamageTaken (1);
			StartCoroutine (player.Knockback (0.05f, 270.0f, player.transform.position));
			}
		}

	}



