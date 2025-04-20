using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperatorJackHandler : MonoBehaviour
{
    bool mouseDown = false;
    bool mouseOver = false;
    CircleCollider2D portHover = null;
    public PortController connection = null;
    public CableEnd cableEnd = CableEnd.end;
    public Sprite unoccupiedSprite;
    public Sprite occupiedSprite;
    public Transform centerBoneTransform;

    Vector3 regularPosition = Vector3.zero;
    Vector3 deltaPos = Vector3.zero;

    public GameObject boneAnchor;
    public GameObject uiBackground;
    public string PortID = "";

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        regularPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        setAngle(this.transform.position);
        if (Input.GetMouseButtonDown(0) && mouseOver && !mouseDown && !uiBackground.activeInHierarchy)
        {
            mouseDown = true;
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
                connection.IsOperatorCable = true;
                connection.Occupied = true;
                SetOccupied(true);
                portHover = null;
                return;
            }
            if (connection != null)
            {
                connection.Occupied = false;
                connection.IsOperatorCable = false;
                connection = null;
            }
            boneAnchor.transform.position = regularPosition;
            rb.MovePosition(regularPosition);
            SetOccupied(false);
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

        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.gameObject.GetComponent<CircleCollider2D>()! == portHover!)
        {
            portHover = null;
        }
    }

    void SetOccupied(bool value)
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = value ? occupiedSprite : unoccupiedSprite;
    }

    void setAngle(Vector2 pos)
    {
        Vector2 _delta = new Vector2(pos.x - centerBoneTransform.position.x, pos.y - centerBoneTransform.position.y);
        float ang = (Mathf.Atan(_delta.y / _delta.x));
        if (_delta.x < 0f)
            ang += Mathf.PI;
        if (_delta.y < 0f)
            ang += 2 * Mathf.PI;
        ang += (cableEnd == CableEnd.start ? Mathf.PI : 0);
        boneAnchor.transform.rotation = Quaternion.Euler(0, 0, ang * (180 / Mathf.PI));
        this.transform.rotation = Quaternion.Euler(0, 0, ang * (180 / Mathf.PI));
    }
}
