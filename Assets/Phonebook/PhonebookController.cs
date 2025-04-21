using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PhonebookController : MonoBehaviour
{
    GameObject phonebookObject = null;
    GameObject toggleObject = null;
    private GameObject backgroundObject = null;


    TMP_Text leftText = null;
    TMP_Text rightText = null;
    UnityEngine.UI.Button prevButton = null;
    UnityEngine.UI.Button nextButton = null;
    UnityEngine.UI.Button closeButton = null;
    Animator phonebookAnimator = null;
    Animator toggleAnimator = null;
    public Phonebook phonebook = null;
    public int currentPage = 1;
    public Action<bool> toggleCallback = null;
    public bool showBook
    {
        get
        {
            return phonebookAnimator.GetBool("IsShown");
        }
        set
        {
            ShowPages();
            phonebookAnimator.SetBool("IsShown", value);
            toggleAnimator.SetBool("IsShown", value);
            toggleCallback?.Invoke(value);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        phonebookObject = GetComponentsInChildren<RectTransform>().First((g) => g.gameObject.name == "Phonebook").gameObject;
        toggleObject = GetComponentsInChildren<RectTransform>().First((g) => g.gameObject.name == "Phonebook Toggle").gameObject;
        backgroundObject = transform.parent.Find("UI Background").gameObject;
        leftText = phonebookObject.GetComponentsInChildren<TMP_Text>().First((t) => t.gameObject.name == "Left Page Text");
        rightText = phonebookObject.GetComponentsInChildren<TMP_Text>().First((t) => t.gameObject.name == "Right Page Text");
        prevButton = phonebookObject.GetComponentsInChildren<UnityEngine.UI.Button>().First((b) => b.gameObject.name == "Previous Page Button");
        nextButton = phonebookObject.GetComponentsInChildren<UnityEngine.UI.Button>().First((b) => b.gameObject.name == "Next Page Button");
        closeButton = phonebookObject.GetComponentsInChildren<UnityEngine.UI.Button>().First((b) => b.gameObject.name == "Close Button");
        phonebook = new Phonebook(leftText, rightText);
        phonebookAnimator = phonebookObject.GetComponent<Animator>();
        toggleAnimator = toggleObject.GetComponent<Animator>();

        prevButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);
        closeButton.onClick.AddListener(ToggleBook);
        toggleObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ToggleBook);

        showBook = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.C) && !backgroundObject.activeInHierarchy)
        {
            ToggleBook();
            return;
        }
    }

    public void ShowPages()
    {
        if (phonebook.pages.Length == 0)
        {
            leftText.text = "No Contacts";
            rightText.text = "";
        }
        else
        {
            leftText.text = phonebook.pages[currentPage - 1].PageEntry();
            rightText.text = phonebook.pages.Length > currentPage + 1 ? phonebook.pages[currentPage].PageEntry() : "";
        }
        prevButton.interactable = currentPage >= 3;
        nextButton.interactable = currentPage <= phonebook.pages.Length - 2;
    }

    public void ToggleBook()
    {
        showBook = !showBook;
    }

    public void PrevPage()
    {
        if (currentPage >= 3)
        {
            currentPage -= 2;
            ShowPages();
        }
    }

    public void NextPage()
    {
        if (currentPage <= phonebook.pages.Length - 2)
        {
            currentPage += 2;
            ShowPages();
        }
    }
}