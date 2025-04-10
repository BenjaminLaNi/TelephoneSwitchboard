using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public enum CableEnd
{
    start,
    end
}

public class CableJackController : MonoBehaviour
{

    bool mouseDown = false;
    bool mouseOver = false;
    CircleCollider2D portHover = null;
    public PortController connection = null;
    public CableEnd cableEnd = CableEnd.start;
    public Sprite unoccupiedSprite;
    public Sprite occupiedSprite;

    Vector3 beforePosition = Vector3.zero;
    Vector3 deltaPos = Vector3.zero;

    public GameObject boneAnchor;
    public string PortID = "";

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { Debug.Log("M1"); }
        if (Input.GetMouseButtonDown(0) && mouseOver && !mouseDown)
        {
            mouseDown = true;
            beforePosition = this.transform.position;
            deltaPos = this.transform.position - boneAnchor.transform.position;
            SetOccupied(false);
        }
        if (Input.GetMouseButtonUp(0) && mouseDown)
        {
            mouseDown = false;
            if (portHover != null && !portHover.gameObject.GetComponentInParent<PortController>().Occupied)
            {
                if (connection != null)
                {
                    connection.Occupied = false;
                    SetOccupied(false);
                }
                connection = portHover.gameObject.GetComponentInParent<PortController>();
                Vector2 pos = (Vector2)portHover.transform.position + (Vector2)portHover.offset / 2;
                if (connection.IsCableHolder)
                {
                    pos.y += .6f;
                }
                boneAnchor.transform.position = pos;
                rb.MovePosition(pos);
                PortID = connection.IsCableHolder ? "cableholder" : connection.PortName;
                connection.cableConnected = this;
                connection.Occupied = true;
                SetOccupied(true);
                portHover = null;
                return;
            }
            boneAnchor.transform.position = beforePosition;
            rb.MovePosition(beforePosition);
            if (connection != null)
            {
                SetOccupied(true);
            }
            portHover = null;
            PortID = "";
        }
        if (mouseDown)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            boneAnchor.transform.position = pos;
            rb.MovePosition(pos);
        }
    }

    void OnMouseEnter()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PortCollider" && mouseDown)
        {
            portHover = collision.gameObject.GetComponent<CircleCollider2D>();
            Debug.Log("Port");
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == portHover!)
        {
            portHover = null;
        }
    }

    void SetOccupied(bool value)
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = value ? occupiedSprite : unoccupiedSprite;
    }
}
