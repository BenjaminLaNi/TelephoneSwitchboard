using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueHandler : MonoBehaviour
{
    private TMP_Text speakerObject = null;
    private TMP_Text messageObject = null;
    private GameObject backgroundObject = null;

    public bool showingDialogue
    {
        get
        {
            return gameObject.activeInHierarchy;
        }
        private set
        {
            gameObject.SetActive(value);
            backgroundObject.SetActive(value);
        }
    }
    private List<Dialogue> _dialogueQueue = new List<Dialogue>();
    public Dialogue[] dialogueQueue
    {
        get
        {
            return _dialogueQueue.ToArray();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        backgroundObject = transform.parent.Find("UI Background").gameObject;
        showingDialogue = true;
        foreach (TMP_Text o in GetComponentsInChildren<TMP_Text>())
        {
            if (o.gameObject.name == "Speaker")
            {
                speakerObject = o;
                continue;
            }
            if (o.gameObject.name == "Message")
            {
                messageObject = o;
                continue;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space) && showingDialogue)
        {
            if (_dialogueQueue.Count > 1)
            {
                DisplayDialogue(_dialogueQueue[1]);
                _dialogueQueue[0].callback();
                _dialogueQueue.RemoveAt(0);
                return;
            }
            _dialogueQueue[0].callback();
            _dialogueQueue.RemoveAt(0);
            ToggleDialoguePanel(false);
            return;
        }
    }

    public void AddDialogue(Dialogue dialogue)
    {
        if (dialogueQueue.Length <= 0)
        {
            DisplayDialogue(dialogue);
        }
        _dialogueQueue.Add(dialogue);
    }

    public void AddDialogues(Dialogue[] dialogues)
    {
        if (dialogueQueue.Length <= 0)
        {
            DisplayDialogue(dialogues[0]);
        }
        _dialogueQueue.AddRange(dialogues);
    }

    private void DisplayDialogue(Dialogue dialogue)
    {
        showingDialogue = true;
        if (speakerObject is not null)
        {
            speakerObject.text = dialogue.speaker;
            messageObject.text = dialogue.message;
        }
    }

    private void ToggleDialoguePanel()
    {
        showingDialogue = !showingDialogue;
    }

    private void ToggleDialoguePanel(bool value)
    {
        showingDialogue = value;
    }
}

public class Dialogue
{
    public string speaker { get; set; }
    public string message { get; set; }
    public Action callback { get; set; } = new Action(() => { });

    public Dialogue(string speaker, string message)
    {
        this.speaker = speaker;
        this.message = message;
        this.callback = new Action(() => { });
    }
    public Dialogue(string speaker, string message, Action callback)
    {
        this.speaker = speaker;
        this.message = message;
        this.callback = callback;
    }
}