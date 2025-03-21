using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PortController : MonoBehaviour
{
    public string PortName;
    TMP_Text Identifier;
    // Start is called before the first frame update
    void Start()
    {
        Identifier = this.GetComponentsInChildren<TMP_Text>().Where((o) => o.name == "Identifier").ToArray<TMP_Text>()[0];
        Identifier.text = PortName;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
