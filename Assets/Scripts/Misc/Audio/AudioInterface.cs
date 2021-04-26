using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AudioInterface 
{
	void PlaySound(string clipName);

	void PlaySound(AudioClip clip);

	void Pause();

	void UnPause();
}
