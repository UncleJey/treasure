using UnityEngine;

public enum hitType : byte { NONE = 0, PIRATE = 1, FOREST = 2 }

public class TheCell
{
    public cellData myCell;
    public bool animated = false;
    int _frame;
    float _counter;
    public Vector2Int point;
    bool sworded;
    bool needUpdate = false;
    int myCellNo = 0;

    public TheCell(Vector2Int pPoint, cellData pData, int pCellNo)
    {
        myCellNo = pCellNo;
        point = pPoint;
        myCell = pData;
        sworded = myCell.sworded;
        needUpdate = myCell != null && myCell.loot != lootType.NONE && myCell.sprites.Length > 1;
        _frame = -1;
        _counter = 1;
    }
    float _check = 0f;
    public void Update()
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
                TheMap.SetTile(point, myCell.sprites[_frame].tile, myCell.sprites[_frame].reverse);
                //image.rectTransform.localScale = new Vector3(myCell.sprites[_frame].reverse ? -1 : 1, 1, 1);
            }
            _check += Time.deltaTime;
            if (_check > 0.2f)
            {
                _check = 0;
                if (sworded)
                {
                    Vector2 pl = Player.instance.Point;
                    if ((int)pl.y == point.y)
                    {
                        if (pl.x - point.x > 0 && pl.x - point.x < 3)
                        {
                            sworded = false;
                            //TheMap.instance.dropSword(rt.anchoredPosition, Vector2.right, false, this);
                        }
                        else if (pl.x - point.x < 0 && pl.x - point.x > -3)
                        {
                            sworded = false;
                            //TheMap.instance.dropSword(rt.anchoredPosition, -Vector2.right, false, this);
                        }
                    }
                }
            }
        }
    }

    public bool canStep()
    {
        if (myCell != null)
            return myCell.loot != lootType.NONE;

        return false;
    }

    public void step()
    {
        if (myCell != null)
        {
            switch (myCell.loot)
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
                    Debug.LogError("unknow loot " + myCell.loot.ToString());
                    break;
            }
        }
    }

    // Попали ли во что-то мечём
    public hitType hit(bool isPlayer)
    {
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
                        return hitType.PIRATE;
                    }
                    else
                        return hitType.NONE;
                    break;
                case lootType.NONE: // хз что
                    break;
                default:
                    return hitType.NONE;    // через лут пролетает
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

    public void Stop()
    {

    }

}
