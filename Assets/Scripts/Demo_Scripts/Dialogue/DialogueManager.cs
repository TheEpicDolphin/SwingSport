using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    Queue sentences = new Queue();

    public GameObject nameDisplayGO;
    public GameObject imageDisplayGO;
    public AutoType typer;

    private string currentName;
    private bool displayEnabled;

    // Start is called before the first frame update
    void Start()
    {
        loadDialogue("Test_dialogue");
        loadImage("pilot");
        setEnabled(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (displayEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("AutoContinueDialogue");
                continueDialogue();
            }
        }
    }

    public void setEnabled(bool enabled)
    {
        displayEnabled = enabled;
        gameObject.SetActive(enabled);
    }

    public void loadImage(String filename)
    {
        Sprite img = Resources.Load<Sprite>("DialogueText/" + filename);
        imageDisplayGO.GetComponent<Image>().sprite = img;
    }

    public void loadDialogue(string filename)
    {
        TextAsset textFile = Resources.Load<TextAsset>("DialogueText/" + filename);
        string[] lines = textFile.text.Split('\n');
        Debug.Log("Read dialogue file " + filename + ".");

        // clear previous sentences
        sentences.Clear();
        
        // assume that name of npc is first line of dialogue text
        currentName = lines[0];

        for (int i = 1; i < lines.Length; i++)
        {
            sentences.Enqueue(lines[i]);
        }

        continueDialogue();
    }

    public void continueDialogue()
    {
        if (sentences.Count != 0)
        {
            nameDisplayGO.GetComponent<Text>().text = currentName;
            string nextSentence = (string)sentences.Dequeue();
            if (nextSentence.Contains("="))
            {
                float autoDialogueDelay = float.Parse(nextSentence.Substring(nextSentence.LastIndexOf("=") + 1));
                IEnumerator coroutine = AutoContinueDialogue(autoDialogueDelay);
                StartCoroutine(coroutine);
                typer.typeText(nextSentence.Substring(0, nextSentence.LastIndexOf("=")));
            } else
            {
                typer.typeText(nextSentence);
            }
        } else
        {
            setEnabled(false);
        }
    }

    public void clearDialogue()
    {
        sentences.Clear();
        imageDisplayGO.GetComponent<Image>().sprite = null;
        nameDisplayGO.GetComponent<Text>().text = "";
        typer.clearText();
        currentName = "";
        setEnabled(false);
    }

    IEnumerator AutoContinueDialogue(float time)
    {
        yield return new WaitForSeconds(time);
        continueDialogue();
    }
}
