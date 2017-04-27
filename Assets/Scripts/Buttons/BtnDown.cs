using UnityEngine.UI;

public class BtnDown : Button
{

	void Update () 
	{
		if (IsPressed())
			Player.instance.goDown();
	}
}
