using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class AutoType : MonoBehaviour
{

	public float letterPause = 0.2f;
	public AudioClip sound;
	public AudioSource audio;

	string message;

	Text textObj;

	// Use this for initialization
	void Start()
	{
		textObj = gameObject.GetComponent<Text>();
		
	}

	public void typeText(string message)
    {
		StopCoroutine("TypeText");
		textObj.text = "";
		StartCoroutine(TypeText(message));
	}

	IEnumerator TypeText(string message)
	{
		foreach (char letter in message.ToCharArray())
		{
			textObj.text += letter;
			if (sound)
				audio.PlayOneShot(sound);
			yield return 0;
			yield return new WaitForSeconds(letterPause);
		}
	}

	public void clearText()
    {
		StopCoroutine("TypeText");
		textObj.text = "";
	}
}
