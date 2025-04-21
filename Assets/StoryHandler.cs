using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoryHandler : MonoBehaviour
{
    public DialogueHandler dialogueHandler;
    public PortsController portsController;
    public PhonebookController phonebookController;
    public ObjectivesHandler objectivesHandler;
    public ClockHandler clockHandler;

    // Start is called before the first frame update
    void Start()
    {
        clockHandler.SetStartTime(3, 12);
        phonebookController.toggleCallback = (show) =>
        {
            if (show)
            {
                Objective objective = objectivesHandler.activeObjectives.Find((o) => o.type == ObjectiveType.OpenPhonebook);
                if (objective is not null)
                {
                    objectivesHandler.CompleteObjective(objective.name);
                }
            }
            else
            {
                Objective objective = objectivesHandler.activeObjectives.Find((o) => o.type == ObjectiveType.ClosePhonebook);
                if (objective is not null)
                {
                    objectivesHandler.CompleteObjective(objective.name);
                }
            }
        };
        DayOne();
    }

    // Update is called once per frame
    void Update()
    {

    }

    List<CableJackController> sortedCables(List<CableJackController> cables)
    {
        if (cables.Count != 2)
        {
            throw new Exception("Cables must contain 2 items");
        }
        if (cables[0].cableEnd == cables[1].cableEnd)
        {
            throw new Exception("Cables must not be the same");
        }
        if (cables[0].cableEnd == CableEnd.start)
        {
            return cables;
        }
        else
        {
            return new List<CableJackController>() { cables[1], cables[0] };
        }
    }

    public void DayOne()
    {
        dialogueHandler.AddDialogues(new Dialogue[] {
            new Dialogue("Big Boss", "Hello There. I'm the Big Boss."),
            new Dialogue("You", "Hello? Who... and where are you?"),
            new Dialogue("Big Boss", "Don't worry about where I am. And as I said, I'm the Big Boss."),
            new Dialogue("Big Boss", "As you see in front of you, the cable mysteriously floating to your right is the operator cable. If someone is calling, you can connect that cable and speak with the caller, to be able to identify which port to connect them to."),
            new Dialogue("Big Boss", "When your operator cable is connected to a port, you can simply click on it and it will jump back to its regular position."),
            new Dialogue("Big Boss", "Beneath you, you have the phone book, which will fill up during your shifts with people calling. It's pretty handy if a caller doesn't know which port somebody else operates in. You can open it by pressing the button, or by pressing <color=yellow>C</color> on your keyboard."),
            new Dialogue("Big Boss", "Every worker has their own phone book, because we've had complaints of workers screwing up the phone book."),
            new Dialogue("Big Boss", "Anyway... It's important you connect the callers as quick as possible, and don't screw up by connecting them with someone else."),
            new Dialogue("Big Boss", "You should be getting a call at any time now, so keep your focus. I'll be watching you and evaluating your performance.", () => {
                objectivesHandler.AddObjective(new Objective("Connect To A1", "Connect the Operator Cable to the A1 port."));
                portsController.RequireOperator("A1", () =>
                {
                    objectivesHandler.CompleteObjective("Connect To A1");
                    dialogueHandler.AddDialogues(new Dialogue[] {
                        new Dialogue("Unknown Caller", "Hello telephone switchboard operator!"),
                        new Dialogue("You", "Hello, sir. Where to?"),
                        new Dialogue("Vlad McGowan", "I, Vlad, am from <b>A1</b> and would like to talk to the person in <b>A2</b>.", () => {
                            phonebookController.phonebook.AddContacts(new PhoneContact[]{
                                new PhoneContact("A1", "Vlad McGowan"),
                                new PhoneContact("A2", "Unknown")
                            });
                            objectivesHandler.AddObjective(new Objective("Connect Vlad To A2", "Connect a cable from Vlad to the A2 port."));
                            portsController.RequireConnection("A1", "A2", () => {
                                objectivesHandler.CompleteObjective("Connect Vlad To A2");
                            }, () => {
                                List<CableJackController> cables = sortedCables(new List<CableJackController>(){portsController.ports.Find((p) => p.PortName == "A1").cableConnected, portsController.ports.Find((p) => p.PortName == "A2").cableConnected});
                                objectivesHandler.AddObjective(new Objective("Put The Cable Back", "Great. Vlad's call has ended. Put the cable back where it came from will you?"));
                                portsController.RequireCableHolder(cables, () => {
                                    objectivesHandler.CompleteObjective("Put The Cable Back");
                                    IEnumerator moreDialogue()
                                    {
                                        yield return new WaitForSeconds(5);
                                        portsController.RequireOperator("B1", () => {
                                            dialogueHandler.AddDialogues(new Dialogue[] {
                                                new Dialogue("Unknown Caller", "Hello?"),
                                                new Dialogue("You", "Hello there! Where to?"),
                                                new Dialogue("Fynn Pham", "Hi, I'm Fynn and I would like to speak to <b>Miss Graham</b>.", () => {
                                                    phonebookController.phonebook.AddContact(new PhoneContact("B1", "Fynn Pham"));
                                                    objectivesHandler.AddObjective(new Objective("Open the Phonebook", "Open the phonebook to see if you can find Miss Graham's port identifier.", () => {
                                                        objectivesHandler.AddObjective(new Objective("Close the Phonebook", "Yeah, no. There's definitely no Miss Graham in the phonebook. Maybe close the phonebook and ask Fynn if he knows her port identifier?", () => {
                                                            dialogueHandler.AddDialogues(new Dialogue[] {
                                                                new Dialogue("You", "Sorry Fynn, but do you know Miss Graham's port identifier?"),
                                                                new Dialogue("Fynn Pham", "I don't know her port identifier. But I know she's in <b>D2</b>.", () => {
                                                                    phonebookController.phonebook.AddContact(new PhoneContact("D2", "Lelia Graham"));
                                                                }),
                                                                new Dialogue("You", "Uhm... Thank you Fynn. I'll connect you to Miss Graham now.", () => {
                                                                    objectivesHandler.AddObjective(new Objective("Connect Fynn To Miss Graham", "He's a bit confused that one. Connect a cable from Fynn to the D2 port."));
                                                                    portsController.RequireConnection("B1", "D2", () => {
                                                                        objectivesHandler.CompleteObjective("Connect Fynn To Miss Graham");
                                                                    }, () => {
                                                                        List<CableJackController> cables = sortedCables(new List<CableJackController>(){portsController.ports.Find((p) => p.PortName == "B1").cableConnected, portsController.ports.Find((p) => p.PortName == "D2").cableConnected});
                                                                        objectivesHandler.AddObjective(new Objective("Put The Cable Back", ""));
                                                                        portsController.RequireCableHolder(cables, () => {
                                                                            objectivesHandler.CompleteObjective("Put The Cable Back");
                                                                            dialogueHandler.AddDialogues(new Dialogue[] {
                                                                                new Dialogue("Big Boss", "Good job! That's all for today. Go home and rest. I'll be watching you and evaluating your lack of performance."),
                                                                                new Dialogue("You", "Thanks?"),
                                                                                new Dialogue("Big Boss", "Yes, you're welcome. I'll see you tomorrow."),
                                                                            });
                                                                        });
                                                                    });
                                                                })
                                                            });
                                                        }){type = ObjectiveType.ClosePhonebook});
                                                    }){type = ObjectiveType.OpenPhonebook});
                                                    // This is so hacky. I'm sorry. Readable code is overrated. JEG HAR LYST TIL AT DØ
                                                }),
                                            });
                                        });
                                    }
                                    StartCoroutine(moreDialogue());
                                });
                            });
                        })
                    });
                });
            }),
        });


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
}
