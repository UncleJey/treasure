﻿using UnityEngine;
using System.Collections;

public enum strings : byte {NONE=0, INFO=1, PLAY=2, INSTRUCTIONS=3}

public static class Language 
{

	public static bool rus = false;
	public static string Value(strings pName)
	{
		switch (pName)
		{
			case strings.INFO:
				return rus?"И - Инструкция":"I - Instructions";
			break;
			case strings.PLAY:
				return rus?"Н - Начало игры":"S - Start game";
			break;
			case strings.INSTRUCTIONS:
				if (rus)
				return @"              О С Т Р О В   С О К Р О В И Щ
                            (С)1989 С.А.Ларионов, г.Свердловск

        Вам нужно провести Джима по извилистым тропам Острова
сокровищ,  найти  спрятанный  в  пещере клад и отнести его на 
корабль.  Остерегайтесь  встречающихся на Вашем  пути пиратов.
Некоторые  из  них  умеют  метать свои сабли и могут поразить
Вас.  Однако  при известной сноровке можно увернуться и подо-
брать упавшую на землю  саблю.  В э том случае Вы сами можете
поразить пирата, метнув саблю с помощью клавиши <ПРОБЕЛ>.
        Чтобы попасть в пещеру, Вам нужно найти спрятанный на
острове  ключ,  а чтобы выкопать сокровища, надо прихватить с
собо  лопату.  На  обратном  пути  Вы должны спастись от пре-
следований одноногого Джона Силвера.
        Удачи Вам!
                                     Нажмите <ПРОБЕЛ>
";
			else
				return
				@"TREASURE ISLAND
---------------

An exciting game based upon the classic adventure by Robert Louis Stevenson,
using the memorable screens from the original story as a backdrop for
arcade style action.

This exciting game is based upon the classic adventure by Robert Louis
Stevenson.

First Jim Hawkins must avoid Blind Pew to board the good ship Hispaniola.
Then he has to avoid capture having heard the crew's mutinous plans. Once
these challenges have been overcome the main game can be played on Treasure
Island itself.

The quest for the treasure starts at the stockade which the adventurers have
built on the island. Then whilst tackling the evil murderous pirates Jim must
find Ben Gunn's cave. The chase and search takes place upon an attractive
'Treasure Map' which is displayed on screen in sections. There are a total of
sixty-four sections to play through.

Once Ben Gunn's cave has been found the treasure is located, and must then be
taken to the ship which is anchored in Spy Glass Cove, to complete the arcade
adventure.

At the end of each game a percentage score will be given which reflects the
speed and fighting spirit with which it was played. Treasure Island can be
played time and time again to attempt to reach a 100 per cent score, which
represents total player perfection.

Loading the game

1 Put the cassette in the recorder.
2 To load, type LOAD""
  This can be achieved by:
  (a) Pressing the L key which will produce the word LOAD on the screen.
  (b) Holding down the SYMBOL SHIFT key which is at the bottom right-hand
  corner of the Spectrum - and while holding it down press the P key twice.
  This will produce two sets of quotation marks on the screen.
  Then take your finger off the SYMBOL SHIFT and press the ENTER key. The
  screen on the Spectrum will now clear, indicating that it is waiting to
  receive the program from the tape recorder.
3 Press PLAY on the tape recorder. The screen border will slowly change
  colours while searching for the information. The border changes to
  multicoloured stripes when it has found the first part of the program. The
  screen remains blank until a picture of Long John Silver appears - after
  approximately one minute. The rest of the program will then load
  automatically, and the process takes a few minutes. The Long John Silver
  picture will then disappear, to be replaced by the first screen of the
  game, and the game can commence.

If you experience any difficulty we suggest that you try loading with only
one plug in the EAR socket of the cassette deck and the same coloured plug
in the EAR socket of the Spectrum.

Please ensure that your tape is stored well away from any source of magnetic
fields - such as televisions, hi-fis, radios, and electric motors.

Controls
Jim Hawkins is moved using a joystick and Sinclair Interface 2 or by the
keyboard where Q or A are up or down and O or P move left or right.

Just avoiding the pirates is not enough to ensure success. You must perfect a
technique which involves getting close to a pirate so that he throws a
cutlass at Jim. You must move Jim swiftly out of the cutlass' path to avoid
injury.

To pick up a weapon or the treasure Jim must be moved over the object. To
throw a cutlass Jim must first be moved in the direction required and then
the fire button or M pressed.

Pressing H will hold the game. Pressing T will terminate the hold. Holding
both B and T down together will restart the game.

(C) Mr Micro Ltd 1984

(Transcribed by Robin Stuart)";
			break;

		}
		return pName.ToString();
	}
}
