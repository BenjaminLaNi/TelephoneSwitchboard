using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PortsController : MonoBehaviour
{
    List<PortController> ports;
    public Dictionary<string, List<CableJackController>> cables;
    public GameObject cablesParent;
    public string[] requiredConnection = new string[] { "", "" };
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
                if (requiredOperatorId == port.PortName && isOccupied && port.IsOperatorCable)
                {
                    requiredOperatorCallback();
                    requiredOperatorId = "";
                    requiredOperatorCallback = new Action(() => { });
                    return;
                }
                if (requiredConnection.Contains(port.PortName) && isOccupied)
                {
                    foreach (var cable in cables)
                    {
                        if (requiredConnection.Contains(cable.Value[0].PortID) && requiredConnection.Contains(cable.Value[1].PortID))
                        {
                            Debug.Log("yipee " + cable.Key);
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

    public void RequireConnection(string firstId, string secondId)
    {
        requiredConnection = new string[] { firstId, secondId };
        Debug.Log(firstId + " -> " + secondId);
    }

    public void RequireOperator(string id, Action callback)
    {
        requiredOperatorId = id;
        ports.Where((p) => p.PortName == id).FirstOrDefault().ToggleLight(true);
        requiredOperatorCallback = callback;
    }
}