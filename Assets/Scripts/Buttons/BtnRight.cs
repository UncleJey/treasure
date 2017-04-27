using UnityEngine.UI;

public class BtnRight : Button
{

	void Update () 
	{
		if (IsPressed())
			Player.instance.goRight();
	}
}
