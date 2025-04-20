using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.AI;

public class ObjectivesHandler : MonoBehaviour
{
    TMP_Text objectivesObject = null;

    public List<Objective> objectives = new List<Objective>();

    public List<Objective> activeObjectives
    {
        get
        {
            return objectives.Where(o => !o.completed).ToList();
        }
    }

    public bool showObjectives
    {
        get
        {
            return gameObject.activeSelf;
        }
        set
        {
            gameObject.SetActive(value);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        objectivesObject = GetComponentsInChildren<TMP_Text>().FirstOrDefault(t => t.name == "Objectives");
        showObjectives = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddObjective(Objective objective)
    {
        Objective previousObjective = objectives.Find((o) => o.name == objective.name);
        if (previousObjective != null)
        {
            if (previousObjective.completed)
            {
                objectives.Remove(previousObjective);
            }
            else
            {
                throw new Exception("Objective already exists and is not completed");
            }
        }
        objectives.Add(objective);
        UpdateObjectivesText();
    }

    public void CompleteObjective(string name)
    {
        var objective = objectives.FirstOrDefault(o => o.name == name);
        if (objective != null)
        {
            objective.Complete();
            UpdateObjectivesText();
        }
    }

    void UpdateObjectivesText()
    {
        List<string> objectivesText = objectives.Where(o => !o.completed).Select(o => "<b>" + o.name + "</b>\n" + o.description).ToList();
        if (objectivesText.Count == 0)
        {
            objectivesObject.text = "No objectives";
            showObjectives = false;
        }
        else
        {
            objectivesObject.text = string.Join("\n", objectivesText);
            showObjectives = true;
        }
    }
}

public class Objective
{
    public string name { get; set; }
    public string description { get; set; }
    public bool completed { get; private set; }
    public ObjectiveType type { get; set; } = ObjectiveType.Unspecified;
    public Action completedCallback { get; set; }

    public Objective(string name, string description)
    {
        this.name = name;
        this.description = description;
        this.completed = false;
    }

    public Objective(string name, string description, Action completedCallback)
    {
        this.name = name;
        this.description = description;
        this.completed = false;
        this.completedCallback = completedCallback;
    }

    public void Complete()
    {
        completed = true;
        completedCallback?.Invoke();
    }
}

public enum ObjectiveType
{
    Unspecified,
    Connect,
    OpenPhonebook,
    ClosePhonebook,
}

