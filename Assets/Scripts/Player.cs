using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum MobDir : byte {NONE = 0, UP = 1, DOWN=2, LEFT=3, RIGHT = 4}

[System.Serializable]
public class SpriteSettings
{
	public Sprite[] sprites;
	public bool reverse = false;
	public MobDir direction;
	public bool haveSword = false;
}

public class Player : MonoBehaviour 
{
	public SpriteSettings[] sprites;

	float scale = 400;
	float spriteIntervalChange = 0.01f;
	float _spriteIntervalChange = 0;

	float h,v;
	public RectTransform rt;

	int _sprNo = 0;
	Image _img;
	public static Player instance;
	int _lives = 0;

	bool _haveSword = false;
	public bool HaveSword
	{
		get
		{
			return _haveSword;
		}
		set
		{
			_haveSword = value;
			ChangeSprite();
		}
	}


	public int Lives
	{
		get => _lives;
		set
		{
			_lives = value;
			TheMap.instance.livesCount.text = $"Man: {value}";
		}
	}

	void Awake () 
	{
		rt = GetComponent<RectTransform> ();
		_img = GetComponent<Image> ();
		instance = this;
	}

	void ChangeSprite()
	{
		MobDir md = MobDir.NONE;
		if (_direction.x > 0)
			md = MobDir.RIGHT;
		else if (_direction.x < 0)
			md = MobDir.LEFT;
		else if (_direction.y > 0)
			md = MobDir.UP;
		else if (_direction.y < 0)
			md = MobDir.DOWN;

		_sprNo++;
		foreach (SpriteSettings ss in sprites)
		{
			if (ss.direction == md && ss.haveSword == _haveSword)
			{
				if (_sprNo >= ss.sprites.Length)
					_sprNo = 0;
				_img.sprite = ss.sprites[_sprNo];
				if (ss.reverse && rt.localScale.x > 0)
					rt.localScale = new Vector3(-1,1,1);
				else if (!ss.reverse && rt.localScale.x < 0)
					rt.localScale = Vector3.one;
				break;
			}
		}
	}

	public Vector2 Point
	{
		get
		{
			Vector4 pos = Position;
			return new Vector2((int)((pos.x + pos.z) / 240),(int)((pos.y + pos.w)/240));
		}
		set
		{
			var sizeDelta = rt.sizeDelta;
			rt.anchoredPosition = new Vector2(value.x * 120 + sizeDelta.x / 2, value.y * 120 + sizeDelta.y / 4);
		}
	}

	bool checkPoint(float pX, float pY)
	{
		int xx = pX < 0 ? -1 : (int)(pX / 120);
		int yy = pY < 0 ? -1 : (int)(pY / 120);
		return TheMap.CanStep(xx, yy);
	}

	public Vector4 Position
	{
		get
		{
			Vector2 anchoredPosition;
			return new Vector4 (rt.anchoredPosition.x - rt.sizeDelta.x / 2 + 10, (anchoredPosition = rt.anchoredPosition).y, anchoredPosition.x + rt.sizeDelta.x / 2 - 10, rt.anchoredPosition.y + rt.sizeDelta.y);
		}
	}

	Vector2 _direction = Vector2.right;
	public static KeyCode[] Joystics = new KeyCode[] 
	{
		 KeyCode.JoystickButton0	// треугольник
		,KeyCode.JoystickButton1	// круг
		,KeyCode.JoystickButton2	// квадрат
		,KeyCode.JoystickButton3	// крестик
		,KeyCode.JoystickButton4	// LD
		,KeyCode.JoystickButton5	// RD
		,KeyCode.JoystickButton6	// LU
		,KeyCode.JoystickButton7	// RU
		,KeyCode.JoystickButton8	// select
		,KeyCode.JoystickButton9	// start
		,KeyCode.JoystickButton10	// Left pad Click
		,KeyCode.JoystickButton11	// Right pad Click
		,KeyCode.Space				// Пробел
	};

	Vector3 _mousePos = Vector3.zero;
	void CheckMove()
	{
		if (Input.GetMouseButtonDown(0))
		{
//			if (mousePos == Vector3.zero)
				_mousePos = Input.mousePosition;
		}
		
		if (Input.GetMouseButtonUp (0)) 
		{
			_hh = 0;
			_vv = 0;
		}
		
		if (Input.GetMouseButton(0))// GetMouseButtonUp(0))
		{
			float dx = Input.mousePosition.x - _mousePos.x;
			float dy = Input.mousePosition.y - _mousePos.y;
			
			float adx = dx<0?0-dx:dx;
			float ady = dy<0?0-dy:dy;
			
			if (adx + ady > 25)
			{
				//Debug.Log(adx + ady);
				_mousePos = Input.mousePosition;
				if (adx > ady)
				{
					if (dx > 0)
						_hh = 1 ;//moveRight();
					else
						_hh = -1;//moveLeft();
				}
				else
				{
					if (dy < 0)
						_vv = -1;//moveBack();
					else
						_vv = 1;//	boost();
				}
			}
			//mousePos = Vector3.zero;
		}
	}

	void Update () 
	{
		foreach (KeyCode c in Joystics)
			if (Input.GetKeyDown (c))
				_space = true;

		if (HaveSword && _space) 
		{
			HaveSword = false;
			TheMap.instance.DropSword (rt.anchoredPosition + Vector2.up * 30, _direction, true, null);
			return;
		}

		_space = false;

		h = Input.GetAxisRaw("Horizontal");	// left -1 right +1
		v = Input.GetAxisRaw("Vertical");	// up + 1 down -1
		if (v == 0 && h == 0)
		{
			CheckMove();
			h = _hh; v = _vv;
			//hh = 0;
			//vv = 0;
			if (v == 0 && h == 0)
				return;
		}
		else if (TheMap.instance.buttons.activeSelf)
			TheMap.instance.buttons.SetActive(false);

		float delta = Time.deltaTime * scale;

		Vector4 pos = Position;

		if (v != 0) 
		{
			if (!checkPoint(pos.x, pos.y + delta * v)) // LB
				v = 0;
			else if (!checkPoint(pos.z, pos.y + delta * v)) // RB
				v = 0;
			else if (!checkPoint(pos.x, pos.w + delta * v)) // LT
				v = 0;
			else if (!checkPoint(pos.z, pos.w + delta * v)) // RT
				v = 0;

			if (v!=0)
				h = 0;
		}

		if (h!=0)
		{
			if (!checkPoint(pos.x + delta * h, pos.y)) // LB
				h = 0;
			else if (!checkPoint(pos.z + delta * h, pos.y)) // RB
				h = 0;
			else if (!checkPoint(pos.x + delta * h, pos.w)) // LT
				h = 0;
			else if (!checkPoint(pos.z + delta * h, pos.w)) // RT
				h = 0;
		}

		if (h > 0)
			_direction = Vector2.right;
		else if (h < 0)
			_direction = -Vector2.right;
		else if (v > 0)
			_direction = Vector2.up;
		else if (v < 0)
			_direction = -Vector2.up;
		else
			return;

		if (_spriteIntervalChange < 0)
		{
			_spriteIntervalChange = spriteIntervalChange;
			ChangeSprite ();
		}
		else
			_spriteIntervalChange -= Time.deltaTime;

		var anchoredPosition = rt.anchoredPosition;
		anchoredPosition = new Vector2(anchoredPosition.x + h * delta, anchoredPosition.y + v * delta);
		rt.anchoredPosition = anchoredPosition;

		TheMap.Step (Point);
	}

	int _hh=0;
	int _vv=0;
	bool _space = false;

	public void goDown()
	{
		//vv = -1;
	}

	public void goUp()
	{
		//vv=1;
	}

	public void goLeft()
	{
	//	hh=-1;
	}

	public void goRight()
	{
		//hh=1;
	}

	public void goFight()
	{
		_space = true;
	}
}
