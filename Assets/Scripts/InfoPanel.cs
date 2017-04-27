using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoPanel : MonoBehaviour 
{
	public GameObject gamePanel;

	void Start()
	{
		GetComponentInChildren<Text>().text = Language.Value(strings.INSTRUCTIONS);
	}

	public void ShowPreloader()
	{
		gamePanel.gameObject.SetActive (true);
		gamePanel.GetComponentInChildren<TheMap> ().ShowPreloader ();
		gameObject.SetActive (false);
	}
}
