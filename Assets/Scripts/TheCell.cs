using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum hitType: byte {NONE = 0, PIRATE = 1, FOREST = 2}

public class TheCell : MonoBehaviour 
{
	Image _image = null;
	public cellData myCell;
	public bool animated = false;
	int myCellNo = 0;
	int _frame;
	float _counter;
	public int rowNo = 0;
	public int colNo = 0;

	bool needUpdate = false;
	RectTransform rt;
	bool sworded;

	public Image image
	{
		get
		{
			if (_image == null)
				_image = GetComponent<Image> ();
			return _image;
		}
	}

	void Awake()
	{
		rt = GetComponent<RectTransform> ();
	}

	public void init (int pCellNo)
	{
		myCellNo = pCellNo;
		sworded = false;
		image.sprite = TheMap.instance.data[TheMap.instance.map[pCellNo]];
		if (myCell != null)
			image.rectTransform.localScale = Vector3.one;
		myCell = TheMap.instance.getSpecCell(image.sprite.name);

		needUpdate = myCell != null && myCell.loot != lootType.NONE && myCell.sprites.Length > 1;
		_frame = -1;
		_counter = 1;
		if (myCell != null)
			sworded = myCell.sworded;
	}
	float _check = 0f;
	void Update()
	{
		if (needUpdate && myCell != null)
		{
			_counter += Time.deltaTime;
			if (_counter > 0.5f)
			{
				_counter = 0;
				_frame++;
				if (_frame >= myCell.sprites.Length)
					_frame = 0;
				image.sprite = myCell.sprites[_frame].sprite;
				image.rectTransform.localScale = new Vector3(myCell.sprites[_frame].reverse?-1:1,1,1);
			}
			_check+= Time.deltaTime;
			if (_check > 0.2f)
			{
				_check = 0;
				if (sworded)
				{
					Vector2 pl = Player.instance.Point;
					if ((int)pl.y == rowNo)
					{
						if (pl.x - colNo > 0 && pl.x - colNo < 3)
						{
							sworded = false;
							TheMap.instance.dropSword(rt.anchoredPosition, Vector2.right, false, this);
						}
						else if (pl.x - colNo < 0 && pl.x - colNo > -3)
						{
							sworded = false;
							TheMap.instance.dropSword(rt.anchoredPosition, -Vector2.right, false, this);
						}
					}
				}
			}
		}
	}

	public bool canStep()
	{
		if (image.sprite.name == "empty")
			return true;

		if (myCell != null)
			return myCell.loot != lootType.NONE;

		return false;
	}

	public void step()
	{
		if (myCell != null)
		{
			switch(myCell.loot)
			{
				case lootType.PirateWithSword:
				case lootType.PIRATE:
					Sounds.Play(SoundType.DIE);
					TheMap.playerDie();
				break;
				case lootType.SCORE:
					Sounds.Play(SoundType.SCORE);
					TheMap.instance.score += 3;
					TheMap.updateCell(myCellNo, myCell.nextSprite.name, this);	
				break;
				case lootType.SWORD:
					Sounds.Play(SoundType.SCORE);
					Player.instance.haveSword = true;
					TheMap.updateCell(myCellNo, myCell.nextSprite.name, this);	
				break;
				case lootType.NONE:
				break;
				default:
					Debug.LogError("unknow loot "+myCell.loot.ToString());
				break;
			}
		}
	}

	// Попали ли во что-то мечём
	public hitType hit(bool isPlayer)
	{
		if (image.sprite.name == "empty")
			return hitType.NONE;
		
		if (myCell != null)
		{
			switch (myCell.loot)
			{
				case lootType.PIRATE:
				case lootType.PirateWithSword:
					if (isPlayer)
					{
						TheMap.updateCell(myCellNo, "empty", this);
						Sounds.Play(SoundType.DIE);
						return  hitType.PIRATE;
					}
					else
						return hitType.NONE;
				break;
				case lootType.NONE:	// хз что
				break;
				default:
					return hitType.NONE;	// через лут пролетает
				break;
			}
		}
		return hitType.FOREST;
	}

	public void nextState()
	{
		if (myCell != null && myCell.nextSprite != null)
			TheMap.updateCell(myCellNo, myCell.nextSprite.name, this);	
	}

}
