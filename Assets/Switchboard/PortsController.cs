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
    // Start is called before the first frame update
    void Start()
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
        string index1 = ports[new System.Random().Next(0, ports.Count)].PortName;
        string index2 = ports[new System.Random().Next(0, ports.Count)].PortName;
        Debug.Log(index1 + " -> " + index2);
        foreach (PortController port in ports)
        {
            port.OccupiedCallback = (isOccupied) =>
            {
                if (port.PortName == index1 && isOccupied)
                {
                    foreach (var cable in cables)
                    {
                        if (cable.Value[0].PortID == index1 || cable.Value[1].PortID == index1)
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
}