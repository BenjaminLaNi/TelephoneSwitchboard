using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortsController : MonoBehaviour
{
    public Dictionary<PortController, Action> incomingCalls = new Dictionary<PortController, Action>();
    public List<Call> currentCalls = new List<Call>();

    List<PortController> ports;
    public Dictionary<string, List<CableJackController>> cables;
    public GameObject cablesParent;
    public PortController[] requiredConnection = new PortController[] { };
    public string requiredOperatorId = "";
    public Action requiredOperatorCallback = new Action(() => { });
    // Start is called before the first frame update
    void Awake()
    {
        ports = new List<PortController>(this.GetComponentsInChildren<PortController>());
        cables = new Dictionary<string, List<CableJackController>>();
        foreach (CableJackController c in cablesParent.GetComponentsInChildren<CableJackController>())
        {
            if (!cables.ContainsKey(c.gameObject.transform.parent.gameObject.name))
            {
                cables.Add(c.gameObject.transform.parent.gameObject.name, new List<CableJackController>());
            }
            cables[c.gameObject.transform.parent.gameObject.name].Add(c);
        }

        foreach (PortController port in ports)
        {
            port.OccupiedCallback = (isOccupied) =>
            {
                if (!isOccupied) { return; }
                if (incomingCalls.ContainsKey(port) && isOccupied && port.IsOperatorCable)
                {
                    incomingCalls[port]();
                    incomingCalls.Remove(port);
                    return;
                }
                if (requiredConnection.Contains(port))
                {
                    foreach (var cable in cables)
                    {
                        if (requiredConnection.Contains(cable.Value[0].connection) && requiredConnection.Contains(cable.Value[1].connection))
                        {

                            StartCall(requiredConnection[0], requiredConnection[1]);
                            requiredConnection = new PortController[] { };
                        }
                    }
                }

            };
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RequireConnection(PortController first, PortController second)
    {
        requiredConnection = new PortController[] { first, second };
    }

    public void RequireConnection(string firstId, string secondId)
    {
        RequireConnection(ports.Find(p => p.PortName == firstId), ports.Find(p => p.PortName == secondId));
    }

    public void RequireOperator(string id, Action callback)
    {
        PortController _p = ports.Where((p) => p.PortName == id).FirstOrDefault();
        incomingCalls.Add(_p, callback);
        _p.ToggleLight(true);
        //_p.BlinkLight();
    }

    public void StartCall(PortController caller, PortController callee)
    {
        Call call = new Call()
        {
            caller = caller,
            callee = callee,
            callerName = "",
            calleeName = ""
        };
        currentCalls.Add(call);
        callee.BlinkLight();

        IEnumerator endCall(Call call, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            currentCalls.Remove(call);
            call.caller.ToggleLight(false);
            call.callee.ToggleLight(false);
        }

        StartCoroutine(endCall(call, new System.Random().Next(10, 15)));
    }
}

public class Call
{
    public PortController caller { get; set; }
    public string callerName { get; set; }
    public PortController callee { get; set; }
    public string calleeName { get; set; }
}