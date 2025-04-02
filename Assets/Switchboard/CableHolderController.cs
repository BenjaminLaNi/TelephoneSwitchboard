using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CableHolderController : MonoBehaviour
{
    public bool Occupied { get { return _occupied; } set { _occupied = value; OccupiedCallback(value); } }
    private bool _occupied = false;
    public System.Action<bool> OccupiedCallback;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
