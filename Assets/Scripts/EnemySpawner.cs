using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour {
	
	public GameObject[] enemy;

	private Text waveText;
	private GameObject wavePanel;

	public int waveNumber = 1;
	private Vector2 screenSize;

	private float startWait = 1f;
	private float waveWait = 1.2f;
	private float unitWait = 0.5f;

	private PlayerControls pc;

	public GameObject[] powerUp;
	private bool isPowerUp;


	void Awake () {
		waveText = GameController.instance.WaveText ();
		wavePanel = GameController.instance.WavePanel ();

		wavePanel.SetActive (false);

		pc = GameObject.FindWithTag ("Player").GetComponent<PlayerControls>();
		pc.ShootingSystem (0);

		screenSize = GameController.instance.CameraBoundary ();

		StartCoroutine (GameLoop());
	}


	IEnumerator GameLoop(){
		yield return new WaitForSeconds (startWait);
		waveText.text = "Get Ready";
		wavePanel.SetActive (true);
		yield return new WaitForSeconds (startWait * 2);
		wavePanel.SetActive (false);

		bool isPlayer = true;
		while (isPlayer == true) {
			yield return StartCoroutine (SpawnWave ());

			if (GameObject.FindWithTag ("Player") == null)
				isPlayer = false;
		}
	}


	IEnumerator SpawnWave(){
		pc.ShootingSystem (0);
		waveText.text = "Wave " + waveNumber;
		wavePanel.SetActive (true);
		yield return new WaitForSeconds (waveWait);
		wavePanel.SetActive (false);
		pc.ShootingSystem (1);
		yield return new WaitForSeconds (waveWait / 2);

		StartCoroutine (UniteSpawner ());
			
		while (!NoEnemyLeft()) {
			yield return null;
		}
			
		yield return new WaitForSeconds (waveWait / 2);

		waveNumber++;
		GameController.instance.HighestWave (waveNumber);
	}


	IEnumerator UniteSpawner(){
		if (waveNumber % 3 == 0) {
			isPowerUp = true;
		} else {
			isPowerUp = false;
		}

		int poweUpNum = 0;
		if (pc.firePower >= 3)
			poweUpNum = 1;

		if (isPowerUp) {
			float yPosition = Random.Range (-screenSize.y + 1, screenSize.y - 1);
			Vector3 position = new Vector3 (transform.position.x, yPosition, 0);
			Instantiate (powerUp[poweUpNum], position, Quaternion.identity);
		}

		int enemyCount = 3 + waveNumber;

		int enemyType = 0;
		if (waveNumber < 3) {
			enemyType = 1;
		} else if (waveNumber < 7) {
			enemyType = 2;
		} else {
			enemyType = 3;
		}

		while (enemyCount > 0) {
			float yPosition = Random.Range (-screenSize.y + 1, screenSize.y - 1);
			Vector3 position = new Vector3 (transform.position.x, yPosition, 0);
			Quaternion rotation = Quaternion.Euler (180, 0, 180);
			int enemyNumber = Random.Range (0, enemyType);

			GameObject go = Instantiate (enemy [enemyNumber], position, rotation) as GameObject;
			MoveForward mf = go.GetComponent<MoveForward> ();
			EnemyShooter es = go.GetComponent<EnemyShooter> ();

			mf.maxSpeed *= Random.Range (0.9f, 1.1f);

			if ((waveNumber >= 10 && enemyNumber == 0 && Random.Range (0, 3) < 2) ||
				(waveNumber >= 15 && enemyNumber == 1 && Random.Range (0, 3) < 2) ||
				(waveNumber >= 20 && enemyNumber == 2 && Random.Range (0, 3) < 2)) {
				mf.isVertical = true;
				mf.vertSpeed = mf.maxSpeed / 2 * Random.Range (0.9f, 1.1f);
				mf.vertRange = mf.maxSpeed / 2 * Random.Range (0.9f, 1.1f);
			}

			if (waveNumber >= 20) {
				unitWait = 0.45f;
			}

			if (waveNumber >= 25 && enemyNumber == 1 && Random.Range (0, 3) == 0)
				es.baseLaserCooldown *= 0.7f;

			if (waveNumber >= 30 && enemyNumber == 2 && Random.Range (0, 3) == 0)
				es.baseMissileCooldown *= 0.7f;

			if (waveNumber >= 35) {
				unitWait = 0.35f;
			}

			enemyCount--;

			yield return new WaitForSeconds (unitWait);
		}

	}


	private bool NoEnemyLeft(){
		GameObject enemy = GameObject.FindWithTag ("Enemy");
		GameObject missile = GameObject.FindWithTag ("Missile");

		return (enemy == null && missile == null);
	}
}

