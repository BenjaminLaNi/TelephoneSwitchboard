using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClockHandler : MonoBehaviour
{
    public GameObject minuteHandObject;
    public GameObject hourHandObject;
    GameObject[] minuteHands;
    GameObject[] hourHands;
    public bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        minuteHands = minuteHandObject.GetComponentsInChildren<SpriteRenderer>().Select(x => x.gameObject).ToArray();
        hourHands = hourHandObject.GetComponentsInChildren<SpriteRenderer>().Select(x => x.gameObject).ToArray();
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator countTime(int startHour, int startMinute, int delaySeconds = 10)
    {
        yield return new WaitForSeconds(delaySeconds);
        isRunning = true;
        int currentHour = startHour;
        int currentMinute = startMinute;
        while (isRunning)
        {
            if (currentMinute == 11)
            {
                currentMinute = 12;
                currentHour = (currentHour + 1) % 12;
                if (currentHour == 0)
                {
                    currentHour = 12;
                }
            }
            else
            {
                currentMinute = (currentMinute + 1) % 12;
            }
            SetTime(currentHour, currentMinute);
            yield return new WaitForSeconds(delaySeconds);
        }
    }

    private void SetTime(int hourHandPosition, int minuteHandPosition)
    {
        if (hourHandPosition > 12 || hourHandPosition < 1)
        {
            throw new System.Exception("Hour hand position must be between 1 and 12");
        }
        if (minuteHandPosition > 12 || minuteHandPosition < 1)
        {
            throw new System.Exception("Minute hand position must be between 1 and 12");
        }
        for (int i = 0; i < minuteHands.Length; i++)
        {
            minuteHands[i].SetActive(i == minuteHandPosition - 1);
        }
        for (int i = 0; i < hourHands.Length; i++)
        {
            hourHands[i].SetActive(i == hourHandPosition - 1);
        }
    }

    public void SetStartTime(int hourHandPosition, int minuteHandPosition)
    {
        SetTime(hourHandPosition, minuteHandPosition);
        isRunning = false;
        StartCoroutine(countTime(hourHandPosition, minuteHandPosition));
    }
}
