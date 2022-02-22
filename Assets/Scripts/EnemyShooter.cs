using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour {
	
	public GameObject laser;
	public GameObject laserCharge;
	public GameObject laserPoint;
	public bool isLaser = false;
	public float baseLaserCooldown = 2f;
	private float currentLaserCooldown;
	private float laserTimer;

	public GameObject missile;
	public GameObject missilePoint;
	public bool isMissile = false;
	public float baseMissileCooldown = 5f;
	private float currentMissileCooldown;
	private float missileTimer;

	private Vector2 screenSize;


	void Awake(){
		RandomCooldownTime ();

		laserTimer = currentLaserCooldown;
		missileTimer = currentMissileCooldown;

		screenSize = GameController.instance.CameraBoundary (0.1f, 0.1f);
	}

	void Update () {
		if (GameObject.FindWithTag ("Player") == null)
			return;
		
		if (isLaser) {
			laserTimer -= Time.deltaTime;

			if (laserTimer <= 0 && transform.position.x <= screenSize.x) {
				laserTimer = currentLaserCooldown;
				float rotation = Random.Range (0, 360);
				var newObject = Instantiate (laserCharge, laserPoint.transform.position, Quaternion.Euler(0, 0, rotation));
				newObject.transform.parent = gameObject.transform;

				Invoke ("LaserShooting", 0.5f);
			}
		}

		if (isMissile) {
			missileTimer -= Time.deltaTime;

			if (missileTimer <= 0 && transform.position.x <= screenSize.x) {
				missileTimer = currentMissileCooldown;
				Instantiate (missile, missilePoint.transform.position, Quaternion.Euler (0, 0, 130));
			}
		}

		RandomCooldownTime ();
	}


	void LaserShooting(){
		Instantiate (laser, laserPoint.transform.position, Quaternion.Euler (0, 0, 180));
	}

	void RandomCooldownTime(){
		float variaton = Random.Range (0.9f, 1.1f);
		currentLaserCooldown = baseLaserCooldown * variaton;
		currentMissileCooldown = baseMissileCooldown * variaton;
	}
}
