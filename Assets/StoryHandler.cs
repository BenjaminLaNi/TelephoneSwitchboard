using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryHandler : MonoBehaviour
{
    public DialogueHandler dialogueHandler;
    public PortsController portsController;
    public PhonebookController phonebookController;
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
        phonebookController.ShowPages();
        portsController.RequireOperator("A1", () =>
        {
            dialogueHandler.AddDialogues(new Dialogue[] {
            new Dialogue("Unknown Caller", "Hello telephone switchboard operator!"),
            new Dialogue("Vlad", "I, Vlad, am from <b>A1</b> and would like to talk to the person in <b>A2</b>.", () => {portsController.RequireConnection("A1", "A2"); phonebookController.phonebook.EditContact("A2", new PhoneContact("A2", "Jane Doe", "")); phonebookController.phonebook.EditContact("A3", new PhoneContact("A3", "Jeff Doe", "")); phonebookController.ShowPages();})
        });
        });
    }
}

public static class mockData
{
    public static PhoneContact[] contacts = new PhoneContact[] {
        new PhoneContact("A1", "John Doe", "Doctor"),
        new PhoneContact("A2", "Jane Doe", "Nurse"),
        new PhoneContact("A3", "Jim Doe", "Lawyer"),
        new PhoneContact("A4", "Jill Doe", "Teacher"),
        new PhoneContact("A5", "Jack Doe", "Engineer"),
        new PhoneContact("A6", "Jill Doe", "Teacher"),
        new PhoneContact("A7", "Jack Doe", "Engineer"),
        new PhoneContact("A8", "Jill Doe", "Teacher"),
        new PhoneContact("A9", "Jack Doe", "Engineer"),
        new PhoneContact("A10", "Jill Doe", "Teacher"),
        new PhoneContact("A11", "Jack Doe", "Engineer"),
        new PhoneContact("A12", "Jill Doe", "Teacher"),
        new PhoneContact("A13", "Jack Doe", "Engineer"),
        new PhoneContact("A14", "Jill Doe", "Teacher"),
        new PhoneContact("A15", "Jack Doe", "Engineer"),
        new PhoneContact("A16", "Jill Doe", "Teacher"),
        new PhoneContact("A17", "Jack Doe", "Engineer"),
        new PhoneContact("A18", "Jill Doe", "Teacher"),
        new PhoneContact("A19", "Jack Doe", "Engineer"),
        new PhoneContact("A20", "Jill Doe", "Teacher")
    };
}