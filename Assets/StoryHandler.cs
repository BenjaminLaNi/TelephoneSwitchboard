using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryHandler : MonoBehaviour
{
    public DialogueHandler dialogueHandler;
    public PortsController portsController;
    // Start is called before the first frame update
    void Start()
    {
        DayOne();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DayOne()
    {
        portsController.RequireOperator("A1", () =>
        {
            dialogueHandler.AddDialogues(new Dialogue[] {
            new Dialogue("Unknown Caller", "Hello telephone switchboard operator!"),
            new Dialogue("Vlad", "I, Vlad, am from <b>A1</b> and would like to talk to the person in <b>A2</b>.", () => {portsController.RequireConnection("A1", "A2");})
        });
        });
    }
}
