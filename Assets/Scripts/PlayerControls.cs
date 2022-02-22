using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

	private Vector3 touchPosition;
	private Rigidbody2D rb;
	private Vector3 direction;
	private float xOffset = 2f;
	public float moveSpeed = 5f;

	public GameObject bullet;
	public GameObject bulletFire;
	private float bulletTimer;
	public float bulletCooldown = 0.5f;
	public GameObject frontGunpoint;
	public GameObject backGunpoint;

	public GameObject missile;
	public GameObject missilePoint;
	public float missileCooldown = 5f;
	private float missileTimer;

	public int firePower = 1;

	private bool shootingSystem = true;

	public int coinValue = 1;
	public int bigCoinValue = 10;

	public AudioSource coinAudio;
	public AudioClip[] coinClips;
	public AudioSource powerAudio;

	public float coinMagnetRadius = 3f;
	public float coinMagnetStrength = 3f;



	void Start ()
	{
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update ()
	{
		if (bulletTimer > 0)
			bulletTimer -= Time.deltaTime;
		if (missileTimer > 0)
			missileTimer -= Time.deltaTime;

		// touch control
		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch (0);
			touchPosition = Camera.main.ScreenToWorldPoint (touch.position);
			touchPosition.x += xOffset;
			touchPosition.z = 0;
			direction = (touchPosition - transform.position);
			rb.velocity = new Vector2 (direction.x, direction.y) * moveSpeed * Time.deltaTime;

			if (touch.phase == TouchPhase.Ended)
				rb.velocity = Vector2.zero;

			PlayerShooting ();
		}

		// mouse control
		if (Input.GetMouseButton (0)) {
			touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			touchPosition.x += xOffset;
			touchPosition.z = 0;
			transform.position = Vector3.MoveTowards (transform.position, touchPosition, moveSpeed * Time.deltaTime);

			PlayerShooting ();
		}

		ScreenBoundary ();

		CoinMagnet ();
	}


	private void ScreenBoundary(){
		Vector2 screenSize = GameController.instance.CameraBoundary(0.1f, 0.1f);
		Vector3 pos = transform.position;

		if (transform.position.y > screenSize.y)
			pos.y = screenSize.y;
		if (transform.position.y < -screenSize.y)
			pos.y = -screenSize.y;
		if (transform.position.x > screenSize.x)
			pos.x = screenSize.x;
		if (transform.position.x < -screenSize.x)
			pos.x = -screenSize.x;

		transform.position = pos;
	}


	private void PlayerShooting ()
	{
		if (!shootingSystem)
			return;

		if (bulletTimer <= 0) {
			float rotation = Random.Range (0, 360);

			// front bullet & fire
			Instantiate (bullet, frontGunpoint.transform.position, Quaternion.identity);
			var newObject = Instantiate (bulletFire, frontGunpoint.transform.position, Quaternion.Euler (0, 0, rotation));
			newObject.transform.parent = gameObject.transform;

			// back bullet & fire
			if (firePower >= 2) {
				var newObject1 = Instantiate (bullet, backGunpoint.transform.position, Quaternion.identity);
				SpriteRenderer sr = newObject1.GetComponent<SpriteRenderer> ();
				sr.sortingOrder = 0;

				var newObject2 = Instantiate (bulletFire, backGunpoint.transform.position, Quaternion.Euler (0, 0, rotation));
				newObject2.transform.parent = gameObject.transform;
				SpriteRenderer sr1 = newObject2.GetComponent<SpriteRenderer> ();
				sr1.sortingLayerName = "Planes";
				sr1.sortingOrder = 9;
				AudioSource audio = newObject2.GetComponent<AudioSource> ();
				audio.mute = true;
			}
			bulletTimer = bulletCooldown;
		}

		if (missileTimer <= 0 && firePower >= 3) {
			if (GameObject.FindWithTag ("Enemy") != null) {
				GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
				Vector2 screenSize = GameController.instance.CameraBoundary(-0.05f, -0.05f);
				bool isEnemyClose = false;

				for (int i = 0; i < enemies.Length; i++) {
					if (enemies [i].transform.position.x < screenSize.x) {
						isEnemyClose = true;
						break;
					}
				}

				if (isEnemyClose) {
					Instantiate (missile, missilePoint.transform.position, Quaternion.Euler (0, 0, -45));
					missileTimer = missileCooldown;
				}
			}
		}
	}


	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Fire PowerUP") {
			Destroy (other.gameObject);
			firePower++;

			powerAudio.Play ();
		}

		if (other.tag == "Coin PowerUP") {
			Destroy (other.gameObject);

			coinAudio.clip = coinClips[Random.Range(0, coinClips.Length)];
			coinAudio.Play ();

			if (other.name.StartsWith("BigCoin")) {
				GameController.instance.AddCoin (bigCoinValue);
				GameController.instance.AddScore ((int) Mathf.Floor ((float) bigCoinValue / 10f));
			} else {
				GameController.instance.AddCoin (coinValue);
				GameController.instance.AddScore ((int) Mathf.Floor ((float) coinValue / 10f));
			}
		}
	}


	public void ShootingSystem(int state){
		if(state == 0)
			shootingSystem = false;
		else
			shootingSystem = true;
	}


	private void CoinMagnet(){
		GameObject[] coins = GameObject.FindGameObjectsWithTag ("Coin PowerUP");

		if (coins == null)
			return;

		foreach (var coin in coins) {
			Vector3 distanceVector = transform.position - coin.transform.position;
			float distance = distanceVector.magnitude;

			if (distance <= coinMagnetRadius) {
				coin.transform.Translate (distanceVector * Time.deltaTime * coinMagnetStrength / distance);
			}
		}
	}
}
