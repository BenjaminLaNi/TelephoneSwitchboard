using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ContactList
{
    private List<PhoneContact> _contacts = new List<PhoneContact>();

    public PhoneContact[] contacts
    {
        get
        {
            return _contacts.ToArray();
        }
    }

    internal void AddContact(PhoneContact contact)
    {
        var existingContact = _contacts.Find((c) => c.portId == contact.portId);
        if (existingContact != null)
        {
            throw new Exception("Contact already exists");
        }
        _contacts.Add(contact);
    }

    internal void EditContact(string portId, PhoneContact contact)
    {
        var existingContact = _contacts.Find((c) => c.portId == portId);
        if (existingContact != null)
        {
            existingContact.name = contact.name;
            existingContact.portId = contact.portId;
            existingContact.note = contact.note;
            return;
        }
        throw new Exception("Contact not found");
    }

    internal void RemoveContact(string portId)
    {
        var existingContact = _contacts.Find((c) => c.portId == portId);
        if (existingContact != null)
        {
            _contacts.Remove(existingContact);
            return;
        }
        throw new Exception("Contact not found");
    }
}

public class Page
{
    public List<PhoneContact> contacts { get; set; }
    public int maxPageLines { get; private set; }

    public int pageLines
    {
        get
        {
            return EstimateLineCount(PageEntry());
        }
    }

    public int pageNumber { get; set; }

    private TMP_Text textComponent;

    public Page(int pageNumber, PhoneContact[] contacts, TMP_Text textComponent)
    {
        this.pageNumber = pageNumber;
        this.contacts = contacts.ToList();
        this.textComponent = textComponent;
        //this.pageLines = EstimateLineCount(this.PageEntry());
        this.maxPageLines = Mathf.CeilToInt(textComponent.GetComponent<RectTransform>().rect.height / textComponent.fontSize);
    }

    public int EstimateLineCount(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        // Get the preferred height of the text
        float preferredHeight = textComponent.GetPreferredValues(text, textComponent.GetComponent<RectTransform>().rect.width, textComponent.GetComponent<RectTransform>().rect.height).y;
        // Get the height of a single line
        float lineHeight = textComponent.fontSize + textComponent.lineSpacing;
        // Calculate the number of lines
        return Mathf.CeilToInt(preferredHeight / lineHeight);
    }

    public string PageEntry()
    {
        string entry = "";
        foreach (PhoneContact contact in contacts)
        {
            entry += contact.BookEntry() + "\n";
        }
        return entry;
    }
}

public class Phonebook
{
    private ContactList _contacts = new ContactList();
    private List<Page> _pages = new List<Page>();
    TMP_Text leftText = null;
    TMP_Text rightText = null;

    public ContactList contacts
    {
        get
        {
            return _contacts;
        }
    }

    public Page[] pages
    {
        get
        {
            return _pages.ToArray();
        }
    }

    public Phonebook(PhoneContact[] contacts, TMP_Text leftText, TMP_Text rightText)
    {
        this.leftText = leftText;
        this.rightText = rightText;
        _pages = new List<Page>();
        foreach (PhoneContact contact in contacts)
        {
            AddContact(contact);
        }
    }

    public void AddContact(PhoneContact contact)
    {
        _contacts.AddContact(contact);
        if (_pages.Count == 0)
        {
            AddPage(new PhoneContact[] { contact });
            return;
        }
        Page lastPage = _pages[_pages.Count - 1];
        if (lastPage.pageLines + (lastPage.EstimateLineCount(contact.BookEntry()) - 1) <= lastPage.maxPageLines)
        {
            lastPage.contacts.Add(contact);
        }
        else
        {
            AddPage(new PhoneContact[] { contact });
        }
    }

    public void EditContact(string portId, PhoneContact contact)
    {
        _contacts.EditContact(portId, contact);
        foreach (Page page in _pages)
        {
            if (page.contacts.Find((c) => c.portId == portId) != null)
            {
                page.contacts.Find((c) => c.portId == portId).name = contact.name;
                page.contacts.Find((c) => c.portId == portId).note = contact.note;
                if (page.pageLines > page.maxPageLines)
                {
                    SplitPage(page.pageNumber);
                }
                else if (page.pageLines < page.maxPageLines)
                {
                    MergePages(page.pageNumber);
                }
                return;
            }
        }
        throw new Exception("Contact not found");
    }

    public void RemoveContact(string portId)
    {
        _contacts.RemoveContact(portId);
        foreach (Page page in _pages)
        {
            if (page.contacts.Find((c) => c.portId == portId) != null)
            {
                page.contacts.Remove(page.contacts.Find((c) => c.portId == portId));
            }
        }
    }

    private void AddPage(PhoneContact[] contacts)
    {
        _pages.Add(new Page(_pages.Count + 1, contacts, (_pages.Count + 1) % 2 == 0 ? rightText : leftText));
    }

    private void MergePages(int pageNumber)
    {
        Page page = _pages[pageNumber - 1];
        Page nextPage = _pages[pageNumber];
        page.contacts.AddRange(nextPage.contacts);
        _pages.RemoveAt(pageNumber);
        if (page.pageLines > page.maxPageLines)
        {
            SplitPage(pageNumber);
        }
    }

    private void SplitPage(int pageNumber)
    {
        Page page = _pages[pageNumber - 1];
        PhoneContact contact = page.contacts.Last();
        page.contacts.RemoveAt(page.contacts.Count - 1);
        if (_pages.Count == pageNumber)
        {
            AddPage(new PhoneContact[] { contact });
        }
        else
        {
            _pages[pageNumber].contacts.Insert(0, contact);
        }
        if (_pages[pageNumber - 1].pageLines > _pages[pageNumber - 1].maxPageLines)
        {
            SplitPage(pageNumber);
            return;
        }
        if (_pages[pageNumber].pageLines > _pages[pageNumber].maxPageLines)
        {
            SplitPage(pageNumber + 1);
            return;
        }
    }
}

public class PhoneContact
{
    public string name { get; set; }
    public string note { get; set; }
    public string portId { get; set; }

    public string BookEntry()
    {
        return $"{portId} - {name}" + ((note != "") ? $" <i>({note})</i>" : "");
    }

    public PhoneContact(string portId, string name, string note)
    {
        this.name = name;
        this.portId = portId;
        this.note = note;
    }

    public PhoneContact(string portId, string name)
    {
        this.name = name;
        this.portId = portId;
    }
}