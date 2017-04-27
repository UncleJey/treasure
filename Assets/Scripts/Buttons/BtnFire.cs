using UnityEngine.UI;

public class BtnFire : Button
{

	void Update () 
	{
		if (IsPressed())
			Player.instance.goFight();
	}
}
