using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Highscores : MonoBehaviour {

	const string privateCode = "0XcKToJ9MUOsChJ1AEinewxzGgjR4VQUOg1D-E2Xl3mA";
	const string publicCode = "5f337afbeb371809c4c28da6";
	const string webURL = "http://dreamlo.com/lb/";

	DisplayHighscores highscoreDisplay;
	public Highscore[] highscoresList;

	public Text networkErrorText;
	public GameObject reloadButton;
	
	void Awake() {
		highscoreDisplay = GetComponent<DisplayHighscores> ();

		networkErrorText.text = string.Empty;
	}

	public void AddNewHighscore(string username, int score) {
		StartCoroutine(UploadNewHighscore(username,score));
	}

	IEnumerator UploadNewHighscore(string username, int score) {
		WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) {
			print ("Upload Successful");
			//DownloadHighscores();
		}
		else {
			print ("Error uploading: " + www.error);
			StartCoroutine ("ErrorText");
		}
	}

	public void DownloadHighscores() {
		reloadButton.SetActive (false);
		StartCoroutine("DownloadHighscoresFromDatabase");
	}

	IEnumerator DownloadHighscoresFromDatabase() {
		WWW www = new WWW(webURL + publicCode + "/pipe/");
		yield return www;
		
		if (string.IsNullOrEmpty (www.error)) {
			FormatHighscores (www.text);
			highscoreDisplay.OnHighscoresDownloaded(highscoresList);
		}
		else {
			print ("Error Downloading: " + www.error);
			StartCoroutine ("ErrorText");
		}

		yield return new WaitForSeconds (2f);
		reloadButton.SetActive (true);
	}

	void FormatHighscores(string textStream) {
		string[] entries = textStream.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
		highscoresList = new Highscore[entries.Length];

		for (int i = 0; i <entries.Length; i ++) {
			string[] entryInfo = entries[i].Split(new char[] {'|'});
			string username = entryInfo[0];
			int score = int.Parse(entryInfo[1]);
			highscoresList[i] = new Highscore(username,score);
//			print (highscoresList[i].username + ": " + highscoresList[i].score);
		}
	}

	IEnumerator ErrorText(string text){
		networkErrorText.text = text;

		yield return new WaitForSeconds (2f);

		networkErrorText.text = "";
	}

}

public struct Highscore {
	public string username;
	public int score;

	public Highscore(string _username, int _score) {
		username = _username;
		score = _score;
	}

}
