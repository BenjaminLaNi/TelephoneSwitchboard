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
            if (!cables.ContainsKey(c.gameObject.name))
            {
                cables.Add(c.gameObject.name, new List<CableJackController>());
            }
            cables[c.gameObject.name].Add(c);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}