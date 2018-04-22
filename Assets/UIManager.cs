﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CerfGames.Utils;

public class UIManager : Singleton<UIManager> {

	//The canvas group from each UI Screen
	public CanvasGroup gameGroup;
	public CanvasGroup creditsGroup;
	public CanvasGroup insertNameGroup;
	public CanvasGroup highScoreGroup;
	//--------------------------------------------
	//Variables from UI Elements
	public TMP_InputField nameInput;
	public Slider hungrySlider,thristSlider,hpSlider; //All the on screen slider
	public TextMeshProUGUI scoreBoardText; //The game Score
	//--------------------------------------------

	//LeaderBoard UI
	public TextMeshProUGUI goalScoreText;
	public TextMeshProUGUI survivalTimeText;
	public TextMeshProUGUI monsterScoreText;
	public TextMeshProUGUI finalScoreText;
	public TextMeshProUGUI bestScoreText;
	public TextMeshProUGUI[] top5Text;

	Highscores highscoreManager;	
	//----------------
	float time;

	protected override void Awake() {
		IsPersistentBetweenScenes = false;
		base.Awake();
	}
	private void Start() {
		gameGroup.alpha = 0;
		creditsGroup.alpha = 0;
		for(int i=0;i<top5Text.Length;i++){
			top5Text[i].text = (i+1)+". Fetching data....";
		}

		highscoreManager = GetComponent<Highscores>();

		//StartCoroutine("RefreshHighScores");
	}

	public void OnHighScoresDownloaded(HighScoreData[] highscoreList){
		for(int i=0;i<top5Text.Length;i++){
			top5Text[i].text = (i+1)+". ";
			if(highscoreList.Length > i){
				top5Text[i].text +=highscoreList[i].username+"\n"+"Points: "+highscoreList[i].score;
			}
		}
	}

	//Update the slider choosed by flag
	public void UpdateSliderValue(string flag,float value){
		if(flag == "hungry"){
			hungrySlider.value = value;
		}else if(flag == "thirst"){
			thristSlider.value = value;
		}else{
			hpSlider.value -= value;
		}
	}

	//Update the scoreboard
	public void ChangeScore(int home, int visitor){
		scoreBoardText.text = home+" x "+visitor;
	}

	//In credits menu, go back to start menu
	public void BackCredits(){
		creditsGroup.alpha = 0;
		creditsGroup.blocksRaycasts = false;
		StartUIManager.Instance.startGameGroup.alpha = 1;
	}
	public void ShowHighScore(){
		highScoreGroup.alpha = 1;
		highScoreGroup.blocksRaycasts = true;
		highScoreGroup.interactable = true;
		int goalScore = GameManager.Instance.score - GameManager.Instance.enemy_score;
		goalScoreText.text = "Goal Score: "+GameManager.Instance.score+"-"+GameManager.Instance.enemy_score+" = "+goalScore;
		survivalTimeText.text ="Survival Time: "+((int)SurvivalManager.Instance.survivalTime).ToString()+"s";
		monsterScoreText.text = "Points from Monsters: "+GameManager.Instance.kill_monster_score.ToString();
		finalScoreText.text = "Final Score (Monster+0.7*Goal+0.3*Time) = "+GameManager.Instance.finalScore.ToString();
		if(PlayerPrefs.HasKey("bestscore")){ //Procura o bestScore e seta a variável dependendo do seu valor
			if(PlayerPrefs.GetFloat("bestScore") < GameManager.Instance.finalScore){
				PlayerPrefs.SetFloat("bestScore", GameManager.Instance.finalScore);
			}
		}else{
			PlayerPrefs.SetFloat("bestScore", GameManager.Instance.finalScore);
		}
		bestScoreText.text = "Best Score: "+PlayerPrefs.GetFloat("bestScore");
		highscoreManager.DownloadHighScore();
	}

	public void ShowInsertName(){
		gameGroup.alpha = 0;
		insertNameGroup.alpha = 1;
		insertNameGroup.blocksRaycasts = true;
		insertNameGroup.interactable = true;
	}

	public void InsertName(){
		GameManager.Instance.playerName = UIManager.Instance.nameInput.text;
		insertNameGroup.blocksRaycasts = false;
		insertNameGroup.interactable = false;
		insertNameGroup.alpha = 0;
		SaveHighScore();
		ShowHighScore();
	}

	public void SaveHighScore(){
		highscoreManager.AddNewHighScore(GameManager.Instance.playerName,GameManager.Instance.finalScore);
	}

	IEnumerator RefreshHighScores(){
		WaitForSeconds wait = new WaitForSeconds(30f);
		while(true){
			highscoreManager.DownloadHighScore();
			yield return wait;
		}
	}
}
