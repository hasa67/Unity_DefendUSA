using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class GameController : MonoBehaviour {

	private int coins;
	private int totalCoins;
	public Text coinsText;
	public Text totalCoinsText;

	public Text scoreText;
	private int score;
	private int highScore;
	public Text yourScoreText;
	public Text highscoreText;

	private int highestWave;
	public Text waveText;
	public Text highestWaveText;

	[Header("Panels:")]
	public GameObject restartPanel;
	public GameObject mainMenuPanel;
	public GameObject wavePanel;
	public GameObject healthPanel;
	public GameObject coinPanel;
	public GameObject upgradePanel;
	public GameObject infoPanel;
	public GameObject leaderboardPanel;
	public GameObject usernamePanel;

	public Slider healthSlider;
	public Image fillImage;

	private float heightOrtho;
	private float widthOrtho;

	public GameObject player;
	public GameObject enemySpawner;

	public AudioClip[] musicClips;
	public AudioSource musicAudio;
	public AudioSource buttonAudio;

	public static GameController instance = null;
	private PlayerControls pc;
	private DamageHandler dh;


	[Header("Upgrades:")]
	private int healthLevel;
	public Text healthInfoText;
	public Text healthPriceText;
	private int[] healthPrice = { -100, -500, -7000, -10000, -50000, 0 };
	private int[] healthValue = { 1, 2, 5, 8, 12, 20 };

	private int speedLevel;
	public Text speedInfoText;
	public Text speedPriceText;
	private int[] speedPrice = { -25, -100, -500, -1000, -5000, 0 };
	private float[] speedValue = { 5f, 7f, 8f, 10f, 12f, 15f };

	private int missileLevel;
	public Text missileInfoText;
	public Text missilePriceText;
	private int[] missilePrice = { -50, -200, -700, -4500, -10000, 0 };
	private float[] missileValue = { 6f, 5f, 4.5f, 4f, 3.5f, 3f };

	private int bulletLevel;
	public Text bulletInfoText;
	public Text bulletPriceText;
	private int[] bulletPrice = { -20, -150, -1000, -6000, -12000, 0 };
	private float[] bulletValue = { 0.8f, 0.7f, 0.6f, 0.5f, 0.45f, 0.4f };

	private int coinLevel;
	public Text coinInfoText;
	public Text coinPriceText;
	private int[] coinPrice = { -50, -100, -1000, -2000, -4000, 0 };
	private int[] coinValue = { 1, 2, 4, 8, 16, 32 };

	private int magnetLevel;
	public Text magnetInfoText;
	public Text magnetPriceText;
	private int[] magnetPrice = { -30, -50, -150, -500, -1000, 0 };
	private float[] magnetValue = { 1f, 3f, 5f, 7f, 9f, 15f };

	public AudioSource purchaseAudio;
	public AudioClip[] purchaseClip;

	private string username;
	public InputField inputField;
	public Text usernameText;

	public GameObject resetButton;

	Highscores hs;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this)
		{
			Destroy (gameObject);
		}

		username = PlayerPrefs.GetString ("Username", "");
		if (username == "") {
			GoToUsernamePanel ();
		} else {
			GoToMainPanel ();
		}
			
		musicAudio.clip = musicClips [0];
		musicAudio.Play ();

		totalCoins = PlayerPrefs.GetInt ("TotalCoins", 0);
		AddTotalCoins (0);

		highScore = PlayerPrefs.GetInt ("Highscore", 0);
		highscoreText.text = highScore.ToString ();

		highestWave = PlayerPrefs.GetInt ("HighestWave", 0);
		highestWaveText.text = highestWave.ToString ();

		usernameText.text = username;
		hs = GetComponent<Highscores> ();
		hs.AddNewHighscore (username, highScore);

		// player stats
		LoadUpgradePanelInfo();

//		DontDestroyOnLoad (gameObject);
	}


	public void InitializeGame () {
		buttonAudio.Play ();

		musicAudio.clip = musicClips [1];
		musicAudio.Play ();

		AddTotalCoins (coins);

		DeleteGameobjects ();

		GoToGamePanel ();

		score = 0;
		AddScore (0);
		coins = 0;
		AddCoin (0);

		Vector2 screenSize = CameraBoundary ();

		GameObject playerGo = Instantiate (player, new Vector3 (-screenSize.x / 2, 0, 0), Quaternion.identity) as GameObject;
		pc = playerGo.GetComponent<PlayerControls> ();
		dh = playerGo.GetComponent<DamageHandler> ();
		LoadUpgradePanelInfo();

		Instantiate (enemySpawner, new Vector3 (1.4f * screenSize.x, 0, 0), Quaternion.identity);

		healthSlider.maxValue = playerGo.GetComponent<DamageHandler> ().health;
		healthSlider.value = healthSlider.maxValue;
		fillImage.color = Color.green;
	}


	public void ExitToMainMenu(){
		GoToMainPanel ();

		buttonAudio.Play ();
		musicAudio.clip = musicClips [0];
		musicAudio.Play ();

		AddTotalCoins (coins);
		coins = 0;
	}

	public void BackToMainMenu(){
		GoToMainPanel ();

		buttonAudio.Play ();
	}


	public void UpgradesPanel(){
		HideAllPanels ();

		upgradePanel.SetActive (true);
		infoPanel.SetActive (true);

		buttonAudio.Play ();
	}
		

	void DeleteGameobjects(){
		string[] tags = new string[]{ "Spawner", "Enemy", "Missile", "Laser", "Bullet", "Fire PowerUP", "Coin PowerUP" };

		foreach (var tag in tags) {
			GameObject[] objects = GameObject.FindGameObjectsWithTag (tag);
			if (objects != null) {
				foreach (var obj in objects) {
					Destroy (obj);
				}
			}
		}
	}


	public void AddScore(int scoreValue){
		if (restartPanel.activeSelf == false) {
			score += scoreValue;
			scoreText.text = "Score: " + score.ToString();
		}
	}


	public void AddCoin(int coinValue){
		coins += coinValue;
		coinsText.text = "x " + coins.ToString ();
	}


	private void AddTotalCoins(int newCoins){
		totalCoins += newCoins;
		totalCoinsText.text = "x " + totalCoins.ToString ();
		PlayerPrefs.SetInt ("TotalCoins", totalCoins);
	}


	public Vector2 CameraBoundary (float xOffset = 0, float yOffset = 0)
	{
		heightOrtho = Camera.main.orthographicSize;
		widthOrtho = 19f / 9f * heightOrtho;

		Vector2 screenSize = new Vector2 (widthOrtho * (1 - xOffset), heightOrtho * (1 - yOffset));

		return screenSize;
	}


	public void HealthBar(int damage){
		healthSlider.value -= damage;
		fillImage.color = Color.Lerp (Color.red, Color.green, healthSlider.value / healthSlider.maxValue);
	}


	public void RestartMenu(){
		restartPanel.SetActive (true);

		highScore = PlayerPrefs.GetInt ("Highscore", 0);

		if (score > highScore) {
			highScore = score;
			yourScoreText.text = "New Highscore: " + highScore;
			PlayerPrefs.SetInt ("Highscore", highScore);
			highscoreText.text = highScore.ToString ();

			hs.AddNewHighscore (username, highScore);
		} else {
			yourScoreText.text = "Your Score: " + score + "\n" + "\n" + "Highscore: " + highScore;
		}
	}


	public Text WaveText(){
		return waveText;
	}


	public GameObject WavePanel(){
		return wavePanel;
	}


	public void HighestWave(int waveNumber){
		highestWave = PlayerPrefs.GetInt ("HighestWave", 0);

		if (waveNumber > highestWave) {
			highestWave = waveNumber;
			PlayerPrefs.SetInt ("HighestWave", highestWave);
			highestWaveText.text = highestWave.ToString ();
		}
	}


	void LoadUpgradePanelInfo(){
		CoinUpdate ();
		MagnetUpdate ();
		HealthUpdate ();
		MissileUpdate ();
		BulletUpdate ();
		SpeedUpdate ();
	}


	void CoinUpdate(){
		coinLevel =  PlayerPrefs.GetInt ("DoubleCoin", 0);

		if (coinLevel < coinValue.Length - 1) {
			coinInfoText.text = coinValue [coinLevel].ToString () + " -> " + coinValue [coinLevel + 1].ToString () + "x";
		} else {
			coinInfoText.text = coinValue [coinLevel].ToString () + "x";
		}
		coinPriceText.text = coinPrice [coinLevel].ToString ();

		if (pc != null && coinLevel > 0) {
			pc.coinValue *= coinValue [coinLevel];
			pc.bigCoinValue *= coinValue [coinLevel];
		}
	}


	public void UpgradeCoin(){
		if (coinLevel < coinPrice.Length - 1 && totalCoins >= Mathf.Abs (coinPrice [coinLevel])) {
			purchaseAudio.clip = purchaseClip [1];
			purchaseAudio.Play ();

			AddTotalCoins (coinPrice [coinLevel]);

			coinLevel++;
			PlayerPrefs.SetInt ("DoubleCoin", coinLevel);

			CoinUpdate ();
		} else {
			purchaseAudio.clip = purchaseClip [0];
			purchaseAudio.Play ();
		}

	}


	void MagnetUpdate(){
		magnetLevel = PlayerPrefs.GetInt ("MagnetLevel", 0);

		if (magnetLevel < magnetValue.Length - 1) {
			magnetInfoText.text = magnetValue [magnetLevel].ToString () + " -> " + magnetValue [magnetLevel + 1].ToString () + "m";
		} else {
			magnetInfoText.text = magnetValue [magnetLevel].ToString () + "m";
		}
		magnetPriceText.text = magnetPrice [magnetLevel].ToString ();

		if (pc != null) {
			pc.coinMagnetRadius = magnetValue [magnetLevel];
			pc.coinMagnetStrength = magnetValue [magnetLevel];
		}
	}


	public void UpgradeMagnet(){
		if (magnetLevel < magnetPrice.Length - 1 && totalCoins >= Mathf.Abs (magnetPrice [magnetLevel])) {
			purchaseAudio.clip = purchaseClip [1];
			purchaseAudio.Play ();

			AddTotalCoins (magnetPrice [magnetLevel]);

			magnetLevel++;
			PlayerPrefs.SetInt ("MagnetLevel", magnetLevel);

			MagnetUpdate ();
		} else {
			purchaseAudio.clip = purchaseClip [0];
			purchaseAudio.Play ();
		}
	}


	void HealthUpdate(){
		healthLevel = PlayerPrefs.GetInt ("HealthLevel", 0);

		if (healthLevel < healthValue.Length - 1) {
			healthInfoText.text = healthValue [healthLevel].ToString () + " -> " + healthValue [healthLevel + 1].ToString ();
		} else {
			healthInfoText.text = healthValue [healthLevel].ToString ();
		}
		healthPriceText.text = healthPrice [healthLevel].ToString ();

		if (dh != null) {
			dh.health = healthValue [healthLevel];
		}
	}


	public void UpgradeHealth(){
		if (healthLevel < healthPrice.Length - 1 && totalCoins >= Mathf.Abs (healthPrice [healthLevel])) {
			purchaseAudio.clip = purchaseClip [1];
			purchaseAudio.Play ();

			AddTotalCoins (healthPrice [healthLevel]);

			healthLevel++;
			PlayerPrefs.SetInt ("HealthLevel", healthLevel);

			HealthUpdate ();
		} else {
			purchaseAudio.clip = purchaseClip [0];
			purchaseAudio.Play ();
		}
	}


	void MissileUpdate(){
		missileLevel = PlayerPrefs.GetInt ("MissileLevel", 0);

		if (missileLevel < missileValue.Length - 1) {
			missileInfoText.text = missileValue [missileLevel].ToString () + " -> " + missileValue [missileLevel + 1].ToString () + "s";
		} else {
			missileInfoText.text = missileValue [missileLevel].ToString () + "s";
		}
		missilePriceText.text = missilePrice [missileLevel].ToString ();

		if (pc != null) {
			pc.missileCooldown = missileValue [missileLevel];
		}
	}


	public void UpgradeMissile(){
		if (missileLevel < missilePrice.Length - 1 && totalCoins >= Mathf.Abs (missilePrice [missileLevel])) {
			purchaseAudio.clip = purchaseClip [1];
			purchaseAudio.Play ();

			AddTotalCoins (missilePrice [missileLevel]);

			missileLevel++;
			PlayerPrefs.SetInt ("MissileLevel", missileLevel);

			MissileUpdate ();
		} else {
			purchaseAudio.clip = purchaseClip [0];
			purchaseAudio.Play ();
		}
	}


	void BulletUpdate(){
		bulletLevel = PlayerPrefs.GetInt ("BulletLevel", 0);

		if (bulletLevel < bulletValue.Length - 1) {
			bulletInfoText.text = bulletValue [bulletLevel].ToString () + " -> " + bulletValue [bulletLevel + 1].ToString () + "s";
		} else {
			bulletInfoText.text = bulletValue [bulletLevel].ToString () + "s";
		}
		bulletPriceText.text = bulletPrice [bulletLevel].ToString ();

		if (pc != null) {
			pc.bulletCooldown = bulletValue [bulletLevel];
		}
	}


	public void UpgradeBullet(){
		if (bulletLevel < bulletPrice.Length - 1 && totalCoins >= Mathf.Abs (bulletPrice [bulletLevel])) {
			purchaseAudio.clip = purchaseClip [1];
			purchaseAudio.Play ();

			AddTotalCoins (bulletPrice [bulletLevel]);

			bulletLevel++;
			PlayerPrefs.SetInt ("BulletLevel", bulletLevel);

			BulletUpdate ();
		} else {
			purchaseAudio.clip = purchaseClip [0];
			purchaseAudio.Play ();
		}
	}


	void SpeedUpdate(){
		speedLevel = PlayerPrefs.GetInt ("SpeedLevel", 0);

		if (speedLevel < speedValue.Length - 1) {
			speedInfoText.text = speedValue [speedLevel].ToString () + " -> " + speedValue [speedLevel + 1].ToString () + "mph";
		} else {
			speedInfoText.text = speedValue [speedLevel].ToString () + "mph";
		}
		speedPriceText.text = speedPrice [speedLevel].ToString ();

		if (pc != null) {
			pc.moveSpeed = speedValue [speedLevel];
		}
	}


	public void UpgradeSpeed(){
		if (speedLevel < speedPrice.Length - 1 && totalCoins >= Mathf.Abs (speedPrice [speedLevel])) {
			purchaseAudio.clip = purchaseClip [1];
			purchaseAudio.Play ();

			AddTotalCoins (speedPrice [speedLevel]);

			speedLevel++;
			PlayerPrefs.SetInt ("SpeedLevel", speedLevel);

			SpeedUpdate ();
		} else {
			purchaseAudio.clip = purchaseClip [0];
			purchaseAudio.Play ();
		}
	}


	private void HideAllPanels(){
		usernamePanel.SetActive (false);
		healthPanel.SetActive (false);
		coinPanel.SetActive (false);
		restartPanel.SetActive (false);
		wavePanel.SetActive (false);
		upgradePanel.SetActive (false);
		mainMenuPanel.SetActive (false);
		infoPanel.SetActive (false);
		leaderboardPanel.SetActive (false);
	}


	public void GoToUsernamePanel(){
		HideAllPanels ();

		usernamePanel.SetActive (true);
		buttonAudio.Play ();

		if (username != string.Empty) {
			resetButton.SetActive (true);
		} else {
			resetButton.SetActive (false);
		}
	}


	public void GoToMainPanel(){
		HideAllPanels ();

		leaderboardPanel.SetActive (true);
		mainMenuPanel.SetActive (true);
		infoPanel.SetActive (true);
	}


	public void GoToGamePanel(){
		HideAllPanels ();

		healthPanel.SetActive (true);
		coinPanel.SetActive (true);
	}


	public void SetUsername(){
		username = inputField.text.Trim ().ToUpper ().Replace (" ", string.Empty);
		username = Regex.Replace (username, @"[^a-zA-Z0-9]", "");

		if (username != string.Empty) {
			usernameText.text = username;
			PlayerPrefs.SetString ("Username", username);

			buttonAudio.Play ();

			PlayerPrefs.SetInt ("Highscore", 0);
			highScore = 0;
			highscoreText.text = highScore.ToString ();

			GoToMainPanel ();
		} else {
			purchaseAudio.clip = purchaseClip [0];
			purchaseAudio.Play ();
		}
	}


	public void ResetUsername(){
		GoToUsernamePanel ();
	}


	public void ResetAll(){
		PlayerPrefs.DeleteAll ();
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
		

	/*
	private void ItemUpdate(int itemLevel, float[] itemValue, int[] itemPrice, Text infoText, Text priceText){
		if (itemLevel < itemValue.Length - 1) {
			infoText.text = itemValue [itemLevel].ToString () + " -> " + itemValue [itemLevel + 1].ToString ();
		} else {
			infoText.text = itemValue [itemLevel].ToString ();
		}
		priceText.text = itemPrice [itemLevel].ToString ();
	}

	private void UpgradeItem(int itemLevel, int[] itemPrice, Text infoText, Text priceText){
		if (itemLevel < itemPrice.Length - 1 && totalCoins >= Mathf.Abs (itemPrice [itemLevel])) {
			itemLevel++;
//			PlayerPrefs.SetInt ("MagnetLevel", magnetLevel);

			purchaseAudio.clip = purchaseClip [1];
			purchaseAudio.Play ();

			AddTotalCoins (itemPrice [itemLevel]);

			MagnetUpdate ();
		} else {
			purchaseAudio.clip = purchaseClip [0];
			purchaseAudio.Play ();
		}
	}
	*/

}
