using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationUtils : MonoBehaviour
{
    // display time is how long to wait between notifications
    public static float DISPLAYTIME = 4.5f;

    private static string ncS1;
    private static float notificationDelta;
    private static string pendingAnnounces;

    private static bool pendingSupernovaNot;
    private static List<Notification> snList;
    private static int SNAffectedSystems;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // statics need to be in an Init function, since no default constructor call is made
    public static void Init()
    {
        ncS1 = "";

        notificationDelta = 0.0f;
        pendingAnnounces = "";

        pendingSupernovaNot = false;
        snList = new List<Notification>();
        SNAffectedSystems = 0;
    }

    // this control allows each notification to get their turn, whilst also allowing the queuing up of several notifications,
    // finally, notifications get grouped together (see "AnnounceSupernovas()" ) for how that is done
    public static void NotificationControl()
    {
        if (notificationDelta >= DISPLAYTIME)
        {
            if (pendingAnnounces.Length>0)
            {
                switch (pendingAnnounces[0])
                {
                    case 's':
                        ncS1 = AnnounceSupernovas();
                        snList.Clear();
                        SNAffectedSystems = 0;
                        break;
                }
                if (pendingAnnounces.Length > 1) pendingAnnounces = pendingAnnounces.Substring(1);
                else pendingAnnounces = "";
            }
            else
                ncS1 = "";
            GameObject.Find("NotificationDisplay").GetComponent<TMPro.TextMeshProUGUI>().SetText(ncS1);
            notificationDelta = 0.0f;
        }
        else
            notificationDelta = notificationDelta + Time.deltaTime;
    }

    public static string AnnounceSupernovas()
    {
        pendingSupernovaNot = false;
        // if there are multiple supernova events waiting to be announced, announce them all at once:
        if (snList.Count>1)
        {
            return snList.Count.ToString() + " supernovae occured, rendering " + SNAffectedSystems + " systems uninhabitable.";
        } 
        else //if there is only one, announce it singly in full detail:
        {
            return "The star \'" + snList[0].content1 + "\' has gone supernova, rendering " + SNAffectedSystems + " nearby systems uninhabitable.";
        }
    }

    public static void AddSupernova(string emittedAt, int nAffected)
    {
        snList.Add(new Notification(emittedAt));
        SNAffectedSystems = SNAffectedSystems + nAffected;

        if (!pendingSupernovaNot)
        {
            // if a supernova notification is NOT already waiting to be announced do, this:
            pendingSupernovaNot = true;
            // adding an 's' to the pendingAnnounces string basically queues up supernova announcement (as 's') for NotificationControl
            pendingAnnounces = pendingAnnounces + "s";
        }
    }

}

public class Notification
{
    public string content1;
    public string content2;
    public string content3;

    public Notification(string c1, string c2, string c3)
    {
        content1 = c1;
        content2 = c2;
        content3 = c3;
    }

    public Notification(string c1, string c2)
    {
        content1 = c1;
        content2 = c2;
        content3 = "";
    }

    public Notification(string c1)
    {
        content1 = c1;
        content2 = "";
        content3 = "";
    }
}
