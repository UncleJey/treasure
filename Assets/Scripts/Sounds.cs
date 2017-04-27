using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SoundType: byte {NONE = 0, SCORE = 1, DIE = 2, DROP = 3, START = 4}

[System.Serializable]
public class SoudEvent
{
	public SoundType type;
	public AudioClip clip;
}

public class Sounds : MonoBehaviour 
{
	public static Sounds instance;
	public SoudEvent[] clips;
	AudioSource source;

	void Awake()
	{
		instance = this;
		source = GetComponent<AudioSource>();
	}

	public static void Music()
	{
		instance.source.Stop ();
		instance.source.Play();
	}

	public static void Play(SoundType pEvent)
	{
		instance.source.Stop();
		foreach (SoudEvent se in instance.clips)
		{
			if (se.type == pEvent)
			{
				instance.source.PlayOneShot(se.clip);
				return;
			}
		}
	}
}
