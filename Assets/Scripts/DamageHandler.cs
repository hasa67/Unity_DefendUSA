using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour {

	public int health = 1;
	public int damage = 1;

	public GameObject bulletHit;
	public GameObject Explosion;

	public int scoreValue = 0;

	private DamageHandler damagehandler;

	public GameObject coinPrefab;
	public int coinCount = 1;



	void Start(){
		coinCount = health;
	}


	void Update () {
		if (health <= 0)
			Die ();
	}


	private void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.layer == 12)
			return;

		if (gameObject.tag == "Bullet") {
			float rotation = Random.Range (0, 360);
			Instantiate (bulletHit, gameObject.transform.position, Quaternion.Euler (0, 0, rotation));
		}

		if (gameObject.tag == "Missile") {
			float rotation = Random.Range (0, 360);
			Instantiate (Explosion, gameObject.transform.position, Quaternion.Euler (0, 0, rotation));

			ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem> ();
			if (ps != null) {
				ps.transform.parent = null;
				Destroy (ps.gameObject, 1f);
			}
		}

		damagehandler = other.GetComponent<DamageHandler> ();
		health -= damagehandler.damage;

		if (other.tag == "Player")
			GameController.instance.HealthBar (damage);
	}


	private void Die(){
		if (gameObject.tag == "Enemy" || gameObject.tag == "Player") {
			float rotation = Random.Range (0, 360);
			Instantiate (Explosion, gameObject.transform.position, Quaternion.Euler (0, 0, rotation));
		}

		if (gameObject.tag == "Enemy") {
			ParticleSystem ps = gameObject.GetComponentInChildren<ParticleSystem> ();

			if (ps != null) {
				ps.transform.parent = null;
				Destroy (ps.gameObject, 1f);
			}

			for (int i = 0; i < coinCount; i++) {
				Vector3 position = transform.position;
				position += new Vector3 (Random.Range (-1f, 1f), Random.Range (-1f, 1f), 0) * 0.5f;
				GameObject coin = Instantiate (coinPrefab, position, Quaternion.identity);
				coin.transform.localScale *= Random.Range (0.9f, 1.1f);
			}
		}

		if (gameObject.tag == "Player")
			GameController.instance.RestartMenu ();

		if (scoreValue > 0)
			GameController.instance.AddScore (scoreValue);

		Destroy (gameObject);
	}
}
