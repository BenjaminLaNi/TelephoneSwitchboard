using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PortController : MonoBehaviour
{
    public string PortName;
    TMP_Text Identifier;
    public System.Action<bool> OccupiedCallback = (v) => { };
    private bool _occupied = false;
    public bool Occupied { get { return _occupied; } set { _occupied = value; OccupiedCallback(value); } }
    public bool Light = false;
    public CableJackController cableConnected = null;
    public bool IsOperatorCable = false;
    public bool IsCableHolder = false;
    public bool blinking = false;
    // Start is called before the first frame update
    void Start()
    {
        Occupied = false;
        if (!IsCableHolder)
        {
            Identifier = this.GetComponentsInChildren<TMP_Text>().Where((o) => o.name == "Identifier").ToArray<TMP_Text>()[0];
            Identifier.text = PortName;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleLight(bool value)
    {
        Light = value;
        _toggleLight(Light);
        blinking = false;
    }

    public void ToggleLight()
    {
        Light = !Light;
        _toggleLight(Light);
        blinking = false;
    }

    private void _toggleLight(bool value)
    {
        this.gameObject.GetComponentsInChildren<UnityEngine.UI.Image>().Where((i) => i.gameObject.name == "Light").ToArray()[0].sprite = Resources.Load<Sprite>((value ? "Light On" : "Lamp Off"));
    }

    public void BlinkLight()
    {
        blinking = true;
        int count = (new System.Random()).Next(5, 10);
        IEnumerator task(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!blinking) { Debug.Log(blinking); yield break; }
                _toggleLight(true);
                yield return new WaitForSeconds(0.25f);
                _toggleLight(false);
                if (!blinking) { yield break; }
                yield return new WaitForSeconds(0.5f);
            }
            ToggleLight(true);
        }
        StartCoroutine(task(count));
    }
}
