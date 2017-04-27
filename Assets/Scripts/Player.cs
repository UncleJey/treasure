using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum mobDir : byte {NONE = 0, UP = 1, DOWN=2, LEFT=3, RIGHT = 4}

[System.Serializable]
public class spriteSettings
{
	public Sprite[] sprites;
	public bool reverse = false;
	public mobDir direction;
	public bool haveSword = false;
}

public class Player : MonoBehaviour 
{
	public spriteSettings[] sprites;

	float scale = 400;
	float spriteIntervalChange = 0.01f;
	float _spriteIntervalChange = 0;

	float h,v;
	public RectTransform rt;

	int sprNo = 0;
	Image img;
	public static Player instance;
	int _lives = 0;

	bool _haveSword = false;
	public bool haveSword
	{
		get
		{
			return _haveSword;
		}
		set
		{
			_haveSword = value;
			changeSprite();
		}
	}


	public int lives
	{
		get
		{
			return _lives;
		}
		set
		{
			_lives = value;
			TheMap.instance.livesCount.text = value.ToString();
		}
	}

	void Awake () 
	{
		rt = GetComponent<RectTransform> ();
		img = GetComponent<Image> ();
		instance = this;
	}

	void changeSprite()
	{
		mobDir md = mobDir.NONE;
		if (direction.x > 0)
			md = mobDir.RIGHT;
		else if (direction.x < 0)
			md = mobDir.LEFT;
		else if (direction.y > 0)
			md = mobDir.UP;
		else if (direction.y < 0)
			md = mobDir.DOWN;

		sprNo++;
		foreach (spriteSettings ss in sprites)
		{
			if (ss.direction == md && ss.haveSword == _haveSword)
			{
				if (sprNo >= ss.sprites.Length)
					sprNo = 0;
				img.sprite = ss.sprites[sprNo];
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
			rt.anchoredPosition = new Vector2(value.x * 120 + rt.sizeDelta.x / 2, value.y * 120 + rt.sizeDelta.y / 4);
		}
	}

	bool checkPoint(float pX, float pY)
	{
		int XX = pX < 0 ? -1 : (int)(pX / 120);
		int YY = pY < 0 ? -1 : (int)(pY / 120);
		return TheMap.canStep(XX, YY);
	}

	public Vector4 Position
	{
		get
		{
			return new Vector4 (rt.anchoredPosition.x - rt.sizeDelta.x / 2 + 10, rt.anchoredPosition.y, rt.anchoredPosition.x + rt.sizeDelta.x / 2 - 10, rt.anchoredPosition.y + rt.sizeDelta.y);
		}
	}

	Vector2 direction = Vector2.right;
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

	Vector3 mousePos = Vector3.zero;
	float distance = 0;
	void checkMove()
	{
		if (Input.GetMouseButtonDown(0))
		{
//			if (mousePos == Vector3.zero)
				mousePos = Input.mousePosition;
			distance = 0;
		}
		
		if (Input.GetMouseButtonUp (0)) 
		{
			distance = 0;
			hh = 0;
			vv = 0;
		}
		
		if (Input.GetMouseButton(0))// GetMouseButtonUp(0))
		{
			float dx = Input.mousePosition.x - mousePos.x;
			float dy = Input.mousePosition.y - mousePos.y;
			
			float adx = dx<0?0-dx:dx;
			float ady = dy<0?0-dy:dy;
			
			if (adx + ady > 25)
			{
				//Debug.Log(adx + ady);
				distance = 45;
				mousePos = Input.mousePosition;
				if (adx > ady)
				{
					if (dx > 0)
						hh = 1 ;//moveRight();
					else
						hh = -1;//moveLeft();
				}
				else
				{
					if (dy < 0)
						vv = -1;//moveBack();
					else
						vv = 1;//	boost();
				}
			}
			//mousePos = Vector3.zero;
		}
	}

	void Update () 
	{
		foreach (KeyCode c in Joystics)
			if (Input.GetKeyDown (c))
				space = true;

		if (haveSword && space) 
		{
			haveSword = false;
			TheMap.instance.dropSword (rt.anchoredPosition + Vector2.up * 30, direction, true, null);
			return;
		}

		space = false;

		h = Input.GetAxisRaw("Horizontal");	// left -1 right +1
		v = Input.GetAxisRaw("Vertical");	// up + 1 down -1
		if (v == 0 && h == 0)
		{
			checkMove();
			h = hh; v = vv;
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
			direction = Vector2.right;
		else if (h < 0)
			direction = -Vector2.right;
		else if (v > 0)
			direction = Vector2.up;
		else if (v < 0)
			direction = -Vector2.up;
		else
			return;

		if (_spriteIntervalChange < 0)
		{
			_spriteIntervalChange = spriteIntervalChange;
			changeSprite ();
		}
		else
			_spriteIntervalChange -= Time.deltaTime;

		rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + h * delta, rt.anchoredPosition.y + v * delta);

		TheMap.step (Point);
	}

	int hh=0;
	int vv=0;
	bool space = false;

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
		space = true;
	}
}
