﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum lootType:byte {NONE = 0, SWORD = 1, SCORE = 2, PIRATE = 3, PirateWithSword = 4}
public enum GameMode:byte {NONE = 0, MENU = 1, GAME = 2, INFO = 3}

[System.Serializable]
public class SpriteSet
{
	public Sprite sprite;
	public bool reverse = false;
}

[System.Serializable]
public class cellData
{
	public string caption;
	public lootType loot = lootType.NONE;
	public Sprite mainSprite;
	public Sprite nextSprite;
	public SpriteSet[] sprites;
	public bool sworded = false;
}

public class TheMap : MonoBehaviour 
{
	public Sprite[] data;
	public static TheMap instance;
	public mapRow[] rows;
	public cellData[] specCells;
	public Text instrText;
	public Text PlayText;
	public Player player;

	public Text livesCount;
	public Text scoreText;
	public Sword swordPrefab;
	GameMode gameMode = GameMode.NONE;

#if UNITY_EDITOR
	[ContextMenuItem("Show", "showInfo")]
	[ContextMenuItem("Load", "loadInfo")]
	[ContextMenuItem("Clear", "clearInfo")]
	public int sector = 0;
#endif
	public byte[] map;
	static Vector2 XY=Vector2.zero;
	public GameObject infoPanel;
	public GameObject buttons;
	public GameObject CloserPanel;

	void InitMap()
	{
		map = new byte[65 * 48]
		{
		//last row
		/*00*/	04,01,01,01,03,01,01,02, 01,01,10,11,12,13,21,22, 02,01,29,33,52,00,00,00, 01,02,28,00,00,32,33,32, 01,01,29,00,34,35,00,00, 01,02,28,00,36,32,33,37,
		/*01*/	01,02,01,01,05,01,06,01, 16,17,19,16,12,13,21,22, 00,00,32,33,32,33,00,00, 33,00,00,00,00,00,00,36, 00,00,00,00,00,51,00,00, 38,40,38,39,39,40,38,40,
		/*02*/	01,03,01,01,01,01,02,01, 17,11,12,13,14,15,11,12, 00,00,32,31,33,00,00,00, 37,49,00,37,00,00,00,36, 00,00,00,00,00,00,00,00, 38,40,38,40,36,41,42,43,
		/*03*/	01,01,02,01,01,01,03,01, 17,11,12,13,14,15,11,12, 00,00,00,00,00,00,00,00, 32,33,36,48,00,44,32,33, 00,00,55,38,40,00,00,00, 46,45,00,44,46,00,45,44,
		/*04*/	01,01,05,01,06,01,01,01, 23,12,12,13,14,11,13,14, 00,00,00,00,49,47,32,33, 32,33,32,33,32,33,32,33, 00,00,00,00,00,00,00,00, 38,40,38,40,38,40,41,42,
		/*05*/  01,03,01,01,01,02,01,01, 12,12,12,13,14,15,14,15, 36,00,55,00,00,00,00,00, 37,36,32,33,36,37,41,43, 00,00,00,00,00,00,00,00, 38,40,38,40,38,40,38,40, 
		/*06*/  01,02,01,01,01,02,01,01, 11,12,12,13,14,15,14,15, 00,00,00,00,00,00,00,00, 66,67,67,68,68,67,68,67, 00,00,00,00,00,00,00,51, 38,40,36,37,38,40,38,40, 
		/*07*/  01,02,01,01,03,01,01,04, 11,12,12,13,73,01,01,01, 00,55,00,00,72,01,02,01, 68,68,69,00,71,01,01,01, 00,00,36,00,72,01,01,02, 44,00,37,00,70,02,01,01, 
			/*2 ряд*/
		/*08*/  01,01,28,00,36,32,33,37, 02,01,29,00,00,00,00,55, 01,01,25,26,27,24,26,26, 01,01,10,12,13,14,22,21, 01,02,28,00,00,00,00,00, 01,01,29,00,38,40,38,40,
		/*09*/	38,40,38,40,38,40,38,40, 00,00,00,00,00,00,00,00, 36,00,32,31,33,00,32,31, 30,00,00,37,00,00,00,37, 00,52,00,00,00,00,00,00, 38,40,38,40,38,40,38,40, 
		/*10*/	38,40,38,40,38,40,38,40, 00,00,00,36,00,00,49,46, 31,33,00,00,00,00,41,42, 37,00,00,00,00,41,42,43, 00,51,00,30,00,00,00,41, 38,40,38,40,38,40,38,40,
		/*11*/  32,46,00,45,46,52,45,44, 46,00,00,46,00,00,00,45, 39,00,47,47,00,00,00,45, 44,00,47,47,00,57,00,46, 40,00,00,45,00,00,00,46, 46,36,00,46,45,00,41,42, 
		/*12*/  45,46,44,30,38,40,38,40, 45,00,00,00,00,52,00,00, 46,00,00,00,00,00,38,39, 46,00,38,39,40,00,00,37, 45,00,00,36,00,00,00,00, 45,00,38,39,40,00,38,40, 
		/*13*/  38,40,38,40,38,40,38,40, 00,00,00,00,00,00,00,00, 38,40,00,49,00,00,36,37, 37,38,40,58,63,59,41,43, 00,00,00,00,00,00,51,00, 38,40,38,40,36,00,37,38, 
		/*14*/  38,40,38,40,36,32,33,32, 00,00,00,45,00,00,00,00, 30,55,00,00,32,33,32,31, 38,39,40,00,00,00,00,37, 00,62,38,40,00,00,00,38, 40,38,40,00,36,36,00,36, 
		/*15*/  36,00,36,00,72,02,01,01, 00,00,44,00,72,01,01,01, 30,00,37,00,72,01,02,01, 46,00,44,00,71,01,01,01, 45,00,46,00,72,05,01,06, 37,00,36,00,71,01,01,01, 
			/*3 ряд*/
		/*16*/  01,01,29,00,32,33,32,33, 03,01,28,00,00,37,00,00, 01,01,29,00,00,00,00,36, 01,01,28,33,00,00,32,33, 01,02,29,32,33,00,00,36, 01,01,18,36,37,52,36,37,
		/*17*/  32,33,32,33,57,32,33,44, 00,00,00,00,51,00,00,00, 39,40,38,40,38,40,38,39, 44,00,36,30,56,00,41,42, 40,00,37,00,00,00,00,38, 37,00,37,00,58,59,00,37, 
		/*18*/  38,40,38,40,38,40,38,40, 00,00,00,00,00,00,00,00, 32,33,32,33,32,33,32,33, 33,32,33,49,00,00,00,36, 32,33,55,00,00,00,00,00, 33,36,00,37,00,36,00,41, 
		/*19*/  46,45,00,46,45,00,30,30, 00,00,00,44,00,00,38,40, 38,40,38,40,00,00,00,30, 37,00,00,00,00,00,38,40, 00,00,00,00,00,00,00,00, 46,38,40,44,37,52,36,37,
		/*20*/  36,00,36,57,37,00,37,41, 30,49,00,00,00,00,00,00, 38,40,38,40,38,40,38,40, 40,00,00,00,00,00,00,41, 00,52,00,00,00,00,00,00, 37,38,40,38,40,00,00,44, 
		/*21*/  38,40,38,40,37,00,38,40, 00,00,00,00,00,00,00,55, 36,00,00,00,00,00,00,41, 38,40,38,40,30,41,43,42, 00,00,00,00,00,00,00,00, 33,32,33,32,33,32,33,32, 
		/*22*/  32,33,32,33,30,36,00,37, 00,00,00,46,44,00,00,36, 30,00,00,45,00,00,00,44, 36,00,00,45,52,00,00,32, 00,00,00,46,00,46,00,37, 37,00,32,33,00,45,00,36, 
		/*23*/  37,00,45,00,71,02,01,01, 37,00,00,00,72,01,01,01, 36,00,32,33,72,01,03,01, 60,00,32,33,71,01,01,01, 60,52,36,00,72,03,01,01, 60,00,37,00,71,01,01,02, 
			/*4 ряд*/
		/*24*/  01,01,28,31,57,00,32,33, 01,01,25,26,31,00,36,00, 01,08,09,01,28,00,32,33, 01,01,10,15,31,00,49,37, 03,01,28,31,31,00,00,00, 01,01,25,31,33,32,33,32,
		/*25*/  37,00,36,00,46,44,00,30, 00,00,00,00,45,46,00,00, 31,31,31,31,33,00,00,37, 33,32,33,32,33,32,33,32, 00,00,00,00,00,00,00,51, 32,33,32,33,37,38,40,38,
		/*26*/  38,40,00,36,00,44,00,36, 00,00,00,44,00,30,00,37, 31,31,31,33,61,30,00,00, 33,32,33,36,38,40,38,40, 00,00,00,00,00,00,00,55, 38,40,38,40,38,40,60,60, 
		/*27*/  38,40,38,40,36,00,38,40, 37,00,00,00,00,00,00,37, 00,00,00,00,00,00,00,37, 38,40,38,40,36,30,41,42, 00,00,00,00,00,00,00,51, 60,60,60,60,60,60,60,60, 
		/*28*/  30,38,40,38,40,00,00,38, 36,00,00,00,00,00,00,00, 37,49,00,00,00,38,40,38, 38,40,38,40,38,40,36,37, 00,00,00,00,00,53,00,00, 60,60,60,60,60,60,60,60, 
		/*29*/  66,67,68,68,67,67,67,68, 00,00,00,00,00,00,00,00, 37,56,00,00,00,00,00,38, 38,40,38,40,38,40,38,40, 00,00,00,00,00,00,52,00, 66,67,68,67,68,69,41,43, 
		/*30*/  69,00,38,40,00,46,51,60, 00,00,30,44,00,45,00,60, 38,40,38,40,00,00,00,60, 37,49,00,00,00,00,00,60, 00,00,00,00,00,45,46,60, 60,60,60,60,60,60,60,60, 
		/*31*/  60,00,36,00,71,01,01,02, 60,00,44,00,72,01,01,01, 60,00,37,00,72,02,01,01, 60,00,46,52,71,01,01,01, 60,00,37,00,72,05,01,06, 60,00,45,00,71,01,01,01, 
			/*5 ряд*/
		/*32*/  01,01,10,31,33,32,33,32, 01,02,29,33,51,00,00,00, 01,01,29,33,00,00,00,36, 01,01,25,27,37,00,00,41, 01,01,03,01,28,00,00,32, 02,01,01,01,29,33,00,37, 
		/*33*/  32,33,32,33,30,38,40,41, 00,00,00,00,00,00,00,00, 37,00,00,00,36,00,38,40, 36,00,32,31,33,51,00,00, 44,56,47,00,47,41,42,43, 33,32,33,00,32,33,32,33, 
		/*34*/  66,67,67,68,67,68,67,69, 00,00,00,00,00,00,00,00, 40,38,40,38,40,38,40,38, 00,00,00,00,00,00,00,36, 44,55,00,00,00,44,00,36, 33,00,37,36,00,36,00,37, 
		/*35*/  60,60,60,60,60,60,60,60, 00,00,00,38,39,40,48,00, 45,00,00,00,60,60,60,60, 46,45,00,00,00,60,60,60, 46,00,51,00,00,00,00,00, 45,00,66,68,68,67,67,68, 
		/*36*/  60,60,60,60,60,60,60,60, 00,00,00,00,00,00,00,00, 60,00,00,57,00,00,00,60, 66,68,68,67,68,67,67,69, 00,52,00,00,00,00,00,00, 60,60,60,66,67,69,00,60, 
		/*37*/  38,40,38,40,38,40,38,40, 00,55,00,00,00,00,00,00, 38,40,00,00,38,40,38,40, 45,00,00,00,30,49,30,41, 00,00,00,00,36,00,00,57, 60,60,00,37,37,33,00,37, 
		/*38*/  32,33,32,33,60,60,60,60, 00,51,00,00,00,00,00,00, 33,44,58,63,59,30,32,33, 45,44,00,00,00,00,00,00, 46,37,62,00,00,00,00,00, 37,45,36,45,32,33,00,32, 
		/*39*/  60,00,45,00,71,01,01,01, 00,00,44,00,72,01,01,02, 32,31,33,00,71,03,01,01, 00,37,00,00,70,01,01,01, 00,55,00,00,71,01,02,01, 32,33,00,00,70,01,01,01, 
		    /*6 ряд*/
		/*40*/  01,02,28,31,33,36,00,37, 01,01,25,31,00,00,00,41, 01,01,03,28,00,32,33,32, 01,01,01,18,00,37,00,00, 02,01,01,29,61,36,00,47, 01,01,01,18,00,37,52,47, 
		/*41*/  32,33,37,00,44,32,33,32, 36,00,00,00,00,00,00,00, 37,00,58,63,59,00,32,31, 00,51,00,49,00,00,00,37, 37,00,00,00,00,00,00,00, 66,67,68,68,67,67,68,67, 
		/*42*/  30,00,38,40,00,36,00,47, 00,00,44,00,00,47,00,37, 31,33,32,33,00,37,00,00, 37,62,36,00,51,00,00,00, 00,00,37,00,00,00,41,42, 68,68,67,67,67,68,67,68, 
		/*43*/  33,00,32,33,32,33,41,42, 37,00,00,55,36,00,64,30, 00,00,00,37,00,00,00,00, 00,00,32,31,33,00,32,31, 37,36,32,33,36,00,37,32, 68,67,67,68,68,68,67,69, 
		/*44*/  31,31,31,31,31,33,00,36, 33,32,33,32,33,44,00,00, 00,53,00,00,00,00,32,33, 30,00,37,00,36,00,00,37, 36,00,44,00,00,00,49,37, 37,00,32,31,33,00,32,33, 
		/*45*/  38,40,00,57,38,40,00,36, 00,00,00,00,00,00,00,38, 38,39,40,38,40,38,40,38, 00,37,00,00,00,00,52,00, 00,36,00,00,00,00,38,39, 40,37,00,38,40,38,40,38, 
		/*46*/  38,40,38,40,38,40,00,32, 30,00,00,00,00,00,51,00, 36,00,00,57,57,00,00,37, 00,00,00,00,00,00,00,36, 37,00,00,00,00,00,41,42, 38,39,40,38,40,38,39,40, 
		/*47*/  32,33,00,00,72,01,01,01, 00,00,00,00,71,05,01,06, 33,00,00,32,70,01,01,01, 36,00,32,33,72,01,01,02, 40,52,46,00,70,01,01,01, 37,00,45,00,71,02,01,01, 
			/*7 ряд*/
		/*48*/  01,02,01,28,00,30,00,32, 01,01,10,31,00,36,00,00, 03,01,28,31,33,37,00,00, 01,01,29,31,37,00,00,45, 01,01,28,55,00,00,45,46, 01,02,18,00,37,45,46,30, 
		/*49*/  66,67,67,68,68,68,67,68, 00,00,00,00,00,00,00,55, 00,37,57,38,40,38,40,38, 37,30,56,00,38,40,38,40, 31,33,00,00,00,00,00,00, 32,33,32,33,32,33,32,33, 
		/*50*/  68,67,67,68,68,67,67,68, 00,00,00,00,00,00,00,00, 38,40,38,40,38,40,38,40, 40,51,00,00,00,00,38,40, 00,00,36,47,37,00,00,00, 32,33,32,33,32,33,32,33, 
		/*51*/  68,68,67,68,67,67,68,69, 00,00,00,00,00,00,00,51, 38,40,38,40,44,57,38,40, 40,00,00,00,00,00,36,37, 00,00,36,37,00,00,37,37, 32,33,32,33,00,00,32,33, 
		/*52*/  37,00,46,45,46,00,36,37, 00,49,58,63,59,00,00,00, 37,30,45,46,44,46,45,37, 46,00,00,00,00,00,00,46, 45,00,51,00,00,00,00,45, 36,46,00,45,45,00,46,37, 
		/*53*/  32,33,00,37,32,33,32,33, 00,00,52,00,36,49,00,00, 32,33,00,00,32,31,33,32, 46,00,00,00,00,37,00,57, 45,00,00,00,00,00,00,00, 40,38,40,38,40,38,40,38, 
		/*54*/  38,40,38,40,38,40,38,40, 00,00,00,00,00,00,00,38, 33,32,33,32,33,00,00,37, 36,00,00,00,36,00,00,38, 00,00,00,55,56,37,00,37, 32,33,32,33,32,33,00,38, 
		/*55*/ 30,00,36,00,72,01,01,01, 45,00,46,00,71,01,02,01, 37,00,45,52,70,01,01,01, 44,00,36,00,71,03,01,01, 45,00,46,00,70,01,01,01, 36,00,45,00,71,02,01,01, 
			/*8 ряд*/
		/*56*/  01,01,28,00,41,42,43,42, 02,01,18,00,57,65,00,00, 01,01,29,00,45,41,42,43, 01,02,28,00,62,00,52,00, 01,01,25,27,24,74,26,74, 04,02,01,05,01,06,01,01, 
		/*57*/  30,38,40,38,40,57,38,40, 00,52,00,00,00,00,00,00, 36,32,33,32,33,32,33,37, 00,00,00,00,00,00,00,00, 74,26,27,24,27,24,26,27, 01,01,03,01,01,01,02,01, 
		/*58*/  38,40,38,40,38,40,38,40, 00,55,00,00,00,00,00,00, 36,37,37,00,00,00,36,37, 00,00,37,36,31,74,27,24, 74,26,27,24,75,01,02,01, 02,01,76,77,78,79,01,01, 
		/*59*/  38,40,44,47,00,00,38,40, 00,00,00,36,00,00,00,00, 33,37,00,37,00,00,00,32, 31,33,55,00,00,00,32,31, 25,74,74,27,24,74,26,75, 01,01,01,05,01,06,01,01, 
		/*60*/  38,40,00,45,46,00,37,38, 00,00,00,00,36,00,00,00, 33,37,00,00,32,33,32,33, 27,24,31,00,00,00,51,00, 01,03,25,27,24,74,74,27, 01,01,01,01,01,02,01,01, 
		/*61*/  40,38,40,38,40,38,40,38, 00,00,52,00,00,00,00,00, 32,33,37,36,32,33,32,33, 00,00,00,00,00,48,00,00, 74,27,24,27,24,26,74,27, 01,01,76,77,78,79,01,01, //40,38,40,38,40,38,40,38, 00,00,53,00,00,00,00,00, 32,33,37,36,32,33,32,33, 00,00,00,00,00,48,00,00, 74,27,24,27,24,26,74,27, 01,01,76,77,78,79,01,01, 
		/*62*/  40,38,40,38,40,37,00,37, 00,00,51,00,00,00,00,00, 32,33,57,32,33,47,32,33, 00,00,00,00,00,00,00,00, 74,27,24,27,24,26,74,27, 01,02,01,01,01,02,01,01, 
		/*63*/  37,00,45,00,71,01,02,01, 00,00,36,00,72,01,01,01, 32,33,00,00,70,02,01,01, 00,00,51,00,72,01,01,01, 74,27,24,26,75,01,02,01, 01,05,01,06,01,01,01,04, 
		/*Preloader*/
				04,01,01,01,05,01,06,01, 10,21,21,11,21,19,20,73, 28,37,45,38,40,60,37,71, 28,47,00,00,00,00,00,72, 25,26,27,24,26,27,24,75, 01,76,77,78,79,01,01,01, 
			
		};
	}

#region Modes / режимы
	public void ShowPreloader()
	{
		instrText.text = Language.Value(strings.INFO);
		instrText.transform.parent.gameObject.SetActive(true);
		PlayText.text = Language.Value(strings.PLAY);
		PlayText.transform.parent.gameObject.SetActive(true);
		loadScreen(new Vector2((map.Length / 48)-1,0), Vector2.zero);
		player.gameObject.SetActive(false);
		starting = false;
		Sounds.Music ();
		buttons.SetActive(false);
		CloserPanel.SetActive (false);

		gameMode = GameMode.MENU;
	}

	public void Quit()
	{
		Application.Quit ();
	}

	void showCloser()
	{
		CloserPanel.SetActive (true);
	}

	public void ShowInformation()
	{
		infoPanel.SetActive (true);
		transform.parent.gameObject.SetActive (false);
		gameMode = GameMode.INFO;
	}

	bool starting = false;
	public void doStartGame()
	{
		if (!starting) 
		{
			starting = true;
			StartCoroutine(StartGame());
		}
	}

	IEnumerator StartGame()
	{
		Sounds.Play (SoundType.START);
		yield return new WaitForSeconds (2f);

		PlayText.transform.parent.gameObject.SetActive(false);
		instrText.transform.parent.gameObject.SetActive(false);
		player.gameObject.SetActive(true);
		InitMap();
		loadScreen(Vector2.zero,new Vector2(6,4));
		player.haveSword = true;//false;
		player.lives = 5;
		score = 0;
		buttons.SetActive(true);
		gameMode = GameMode.GAME;
	}

#endregion modes

	void Awake () 
	{
		Language.rus = Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian	|| Application.systemLanguage == SystemLanguage.Belarusian;
		Player.instance = FindObjectOfType<Player>();
		instance = this;
		InitMap ();
		QualitySettings.vSyncCount = 1;
	}

	void Start()
	{
		ShowPreloader();
	}

	void Update()
	{
		/*
		foreach (KeyCode c in System.Enum.GetValues(typeof(KeyCode)))
			if (Input.GetKeyDown (c))
				Debug.Log (c);
*/
		if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Menu))
		{
			if (gameMode != GameMode.MENU)
				ShowPreloader();
			else
				showCloser();
		}

		if (gameMode == GameMode.MENU)
		{
			foreach (KeyCode c in Player.Joystics)
				if (Input.GetKeyDown (c))
					doStartGame();
		}
	}
#region map
	int offset(Vector2 pPoint)
	{
		return (int)(pPoint.x * 48 + pPoint.y * 48 * 8);
	}

	public cellData getSpecCell(string pName)
	{
		foreach (cellData cd in specCells)
		{
			if (cd.mainSprite.name.Equals(pName))
				return cd;
		}
		return null;
	}

	public static void updateCell(int pNo, string pNewName, TheCell who)
	{
		instance.map[pNo] = (byte)instance.getSpriteNo(pNewName);
		who.init(pNo);
	}

	public void loadScreen(Vector2 pPoint, Vector2 reposPlayer)
	{
		exSpawnPoint = spawnPoint;	spawnPoint = reposPlayer;
		exSpawnScreen = spawnScreen; spawnScreen = pPoint;
		XY = pPoint;
		int strt = offset(pPoint);
		Debug.Log("scene "+(pPoint.x + pPoint.y * 8).ToString());
		foreach (mapRow row in rows)
		{
			foreach (TheCell i in row.Columns)
			{
				if (data[map[strt]] == null)
					Debug.LogError("Empty "+map[strt].ToString());
				else
					i.init(strt);
				strt++;
			}
		}
		if (reposPlayer.sqrMagnitude > 0)
			Player.instance.Point = reposPlayer;

		Sword[] swords = GetComponentsInChildren<Sword> (false);
		if (swords != null && swords.Length>0)
		foreach (Sword sword in swords)
			Destroy(sword.gameObject);
	}

	public static bool canStep(int pX, int pY)
	{
		if (pX < 0 || pX > 7 || pY < 0 || pY > 5)
		{
			step(new Vector2(pX, pY));
			return false;
		}
		else
			return instance.rows [pY].Columns[pX].canStep();
	}

	static Vector2 exPoint = Vector2.zero;
	public static void step(Vector2 pPoint)
	{
		if (exPoint != pPoint)
		{
			exPoint = pPoint;
			if (pPoint.y < 0)
			{
				XY.y--;
				instance.loadScreen (XY, new Vector2(pPoint.x, 5));
			}
			else if (pPoint.y > 5)
			{
				XY.y++;
				instance.loadScreen (XY, new Vector2(pPoint.x, 0));
			}
			else if (pPoint.x < 0)
			{
				XY.x--;
				instance.loadScreen (XY, new Vector2(7, pPoint.y));
			}
			else if (pPoint.x > 7)
			{
				XY.x++;
				instance.loadScreen (XY, new Vector2(0, pPoint.y));
			}
			else
				instance.rows [(int) pPoint.y].Columns[(int)pPoint.x].step();
		}
	}
#endregion map

#if UNITY_EDITOR	
	void showInfo()
	{
		string res = "";
		int sn = 0;
		foreach (mapRow row in rows)
		{
			foreach (TheCell i in row.Columns)
			{
				sn = getSpriteNo(i.image.sprite.name);
				if (sn < 0)
					Debug.Log("empty !");
				res += sn.ToString().PadLeft(2,'0')+",";
			}
			res += " ";
		}
		Debug.Log (res);
	}

	void loadInfo()
	{
		InitMap ();
		int strt = 48 * sector;
		foreach (mapRow row in rows)
		{
			foreach (TheCell i in row.Columns)
			{
				if (data[map[strt]] == null)
					Debug.Log("Empty "+map[strt].ToString());
				else
					i.image.sprite = data[map[strt]];
				EditorUtility.SetDirty(i);
				strt++;
			}
		}
	}
	
	public void clearInfo()
	{
		foreach (mapRow row in rows)
			foreach (TheCell i in row.Columns) 
			{
				i.image.sprite = data [0];
				EditorUtility.SetDirty(i);
			}
	}
#endif

	#region images
	int getSpriteNo(string sName)
	{
		for (int i=0; i<data.Length; i++)
		{
			if (data[i] != null && data[i].name == sName)
				return i;

		}
		return -1;
	}

	#endregion images

#region GameEvents
	static Vector2 spawnScreen, exSpawnScreen = Vector2.zero;
	static Vector2 spawnPoint, exSpawnPoint = Vector2.zero;

	public static void playerDie()
	{
		instance.player.haveSword = false;
		instance.player.lives --;
		if (instance.player.lives > 0)
			instance.loadScreen (exSpawnScreen, exSpawnPoint);
		else
			instance.ShowPreloader ();
	}

	int _score = 0;
	public int score
	{
		get
		{
			return _score;
		}
		set
		{
			_score = value;
			scoreText.text = value.ToString().PadLeft(3,'0');
		}
	}

	public void dropSword(Vector2 pPosition, Vector2 pDirection, bool isPlayer, TheCell owner)
	{
		GameObject go = Instantiate (swordPrefab.gameObject);
		go.GetComponent<RectTransform> ().SetParent (transform);
		go.transform.localScale = Vector3.one;
		Sword sw = go.GetComponent <Sword> ();
		sw.drop (pPosition, pDirection, isPlayer, owner);
	}
#endregion
}