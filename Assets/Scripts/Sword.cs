using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour 
{
	static int scale = 400;
	public bool flying = true;
	RectTransform rt;
	bool playerBy;

	Vector2 endPoint = Vector2.zero;
	Vector2 direction;
	TheCell myOwner;

	public void drop(Vector2 startPoint, Vector2 pDirection, bool isPlayer, TheCell owner)
	{
		myOwner = owner;
		rt = GetComponent<RectTransform> ();
		rt.anchoredPosition = startPoint;
		flying = true;
		playerBy = isPlayer;
		endPoint = startPoint + pDirection * 3 * 120;
		direction = pDirection;
		gameObject.SetActive (true);
		Sounds.Play (SoundType.DROP);

		if (pDirection.x > 0)
			rt.localScale = new Vector2 (-rt.localScale.x, rt.localScale.y);	// right
		else if (pDirection.y < 0)
			rt.localRotation = Quaternion.Euler (0, 0, 90);	// up
		else if (pDirection.y > 0)
			rt.localRotation = Quaternion.Euler (0, 0, -90);	// down
	}

	Vector4 Position
	{
		get
		{
			return new Vector4 (rt.anchoredPosition.x - rt.sizeDelta.x / 2, rt.anchoredPosition.y, rt.anchoredPosition.x + rt.sizeDelta.x / 2, rt.anchoredPosition.y + rt.sizeDelta.y);
		}
	}

	void Update () 
	{
		Vector4 pos = Position;

		if (flying) 
		{
			rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, endPoint, Time.deltaTime * scale);
			if (Vector2.SqrMagnitude(rt.anchoredPosition - endPoint) < 0.1f)
			{
				flying = false;
				rt.anchoredPosition = endPoint + Vector2.up * 20;
				if (playerBy)
					Destroy(gameObject);
			}
		}

		checkPoint (pos.x, pos.y); // LB
		checkPoint (pos.z, pos.y); // RB
		checkPoint (pos.x, pos.w); // LT
		checkPoint (pos.z, pos.w); // RT
	}

	void checkPoint(float pX, float pY)
	{
		int XX = pX < 0 ? -1 : (int)(pX / 120);
		int YY = pY < 0 ? -1 : (int)(pY / 120);

		// сначала проверяем не ударилось ли в препятствие
		hitType ht = hitType.NONE;
		if (XX < 0 || XX > 7 || YY < 0 || YY > 5)
			ht = hitType.FOREST;
		else
			ht = TheMap.instance.rows [YY].Columns[XX].hit(playerBy);

		if (playerBy)
		{
			switch (ht)
			{
				case hitType.FOREST:
					Sounds.Play(SoundType.DIE);
					Destroy(gameObject);
				break;
				case hitType.NONE:
				break;
				case hitType.PIRATE:
					TheMap.instance.Score += 1;
					Destroy(gameObject);
				break;
			}
		}
		else
		{
			if (flying)
			{
				switch (ht)
				{
				case hitType.FOREST:
					Sounds.Play(SoundType.DIE);
					flying = false;
					break;
				case hitType.NONE:
					break;
				case hitType.PIRATE:
					break;
				}

				if (Intersection(Player.instance.Position, Position))
				{
					TheMap.PlayerDie();
					Sounds.Play(SoundType.DIE);
					Destroy(gameObject);
				}
			}
			else
			{
				if (Intersection(Player.instance.Position, Position))
				{
					Player.instance.HaveSword = true;
					Sounds.Play(SoundType.SCORE);
					playerBy = true;
					if (myOwner != null)
						myOwner.nextState();
					Destroy(gameObject);
				}
			}
		}
	}

	bool Intersection(Vector4 a, Vector4 b)
	{
		float v1 = (b.z - b.x) * (a.y - b.y) - (b.w - b.y) * (a.x - b.x);
		float v2 = (b.z - b.x) * (a.w - b.y) - (b.w - b.y) * (a.z - b.x);
		float v3 = (a.z - a.x) * (b.y - a.y) - (a.w - a.y) * (b.x - a.x);
		float v4 = (a.z - a.x) * (b.w - a.y) - (a.w - a.y) * (b.z - a.x);
		return (v1 * v2 < 0) && (v3 * v4 < 0);
	}
}
