using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public PortsController portsController;
    public StoryHandler storyHandler;
    public Call[] calls;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetGrade()
    {
        Score score = new Score(portsController.registeredConnections.ToArray());
        Debug.Log(score.numScore + ": " + score.grade);
    }
}

public class Score
{
    public int numScore { get; private set; }
    public Grade grade { get; private set; }

    public Score(RegisteredConnection[] connections)
    {
        int score = 0;
        foreach (RegisteredConnection connection in connections)
        {
            int _score = 0;
            switch (connection.type)
            {
                case ConnectionType.Correct:
                    _score += 100 * (80 / (int)(connection.timeElapsed.TotalSeconds * 3));
                    break;
                case ConnectionType.Unnecessary:
                    _score -= 10;
                    break;
                case ConnectionType.Wrong:
                    _score -= 50 + 50 * (int)(connection.timeElapsed.TotalSeconds / 100);
                    break;
            }
            score += _score;
        }
        numScore = score;
        grade = numScore >= 100 * connections.Length ? Grade.A : numScore >= 80 * connections.Length ? Grade.B : numScore >= 60 * connections.Length ? Grade.C : numScore >= 40 * connections.Length ? Grade.D : Grade.F;
    }
}

public enum Grade
{
    A,
    B,
    C,
    D,
    F
}