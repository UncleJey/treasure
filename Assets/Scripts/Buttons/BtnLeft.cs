using UnityEngine.UI;

public class BtnLeft : Button
{

	void Update () 
	{
		if (IsPressed())
			Player.instance.goLeft();
	}
}
