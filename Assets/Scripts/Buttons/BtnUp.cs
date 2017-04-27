using UnityEngine.UI;

public class BtnUp : Button
{

	void Update () 
	{
		if (IsPressed())
			Player.instance.goUp();
	}
}
