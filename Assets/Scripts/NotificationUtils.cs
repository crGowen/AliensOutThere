using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationUtils : MonoBehaviour
{
    // display time is how long to wait between notifications
    private const float DISPLAYTIME = 7.5f;

    private static string ncS1;
    private static float notificationDelta;
    private static string pendingAnnounces;

    private static List<Notification> lsnList;

    private static List<Notification> tsiList;
    private static List<Notification> ecList;

    // statics need to be in an Init function, since no default constructor call is made
    public static void Init()
    {
        ncS1 = "";

        notificationDelta = 0.0f;
        pendingAnnounces = "";

        lsnList = new List<Notification>();
        tsiList = new List<Notification>();
        ecList = new List<Notification>();
    }

    // this control allows each notification to get their turn, whilst also allowing the queuing up of several notifications,
    public static void NotificationControl()
    {
        if (notificationDelta >= DISPLAYTIME)
            GameObject.Find("NotificationDisplay").GetComponent<TMPro.TextMeshProUGUI>().SetText("");

        if (notificationDelta >= DISPLAYTIME + 0.5f)
        {
            if (pendingAnnounces.Length>0)
            {
                switch (pendingAnnounces[0])
                {
                    case 'i':
                        ncS1 = AnnounceTSidentify();
                        tsiList.RemoveAt(0);
                        break;
                    case 'e':
                        ncS1 = AnnounceEC();
                        ecList.RemoveAt(0);
                        break;
                    case 'l':
                        ncS1 = AnnounceLethalSupernova();
                        lsnList.RemoveAt(0);
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

    public static string AnnounceLethalSupernova()
    {
        if (lsnList[0].content2 == "")
            return lsnList[0].content3 + " nearby civilisations have been WIPED OUT as " + lsnList[0].content1 + " erupted into a supernova.";
        else if (lsnList[0].content3 == "")
            return "The " + lsnList[0].content2.Substring(0, lsnList[0].content2.Length - 7) + " have been WIPED OUT as nearby star " + lsnList[0].content1 + " erupted into a supernova.";
        else
            return "The " + lsnList[0].content2.Substring(0, lsnList[0].content2.Length - 7) + " and the " + lsnList[0].content3.Substring(0, lsnList[0].content3.Length - 7) + " have been WIPED OUT as nearby star " + lsnList[0].content1 + " erupted into a supernova.";
    }

    public static string AnnounceTSidentify()
    {
        return "The " + tsiList[0].content2 + " found a strange signal, originating from " + tsiList[0].content1 + tsiList[0].content3 + ".";
    }

    public static string AnnounceEC()
    {
        return "The " + ecList[0].content2 + " have established communication with the " + ecList[0].content1 + "!";
    }

    public static void AddLethalSupernova(string emittedAt, string killed1, string killed2)
    {
        if (killed2 != "")
            lsnList.Add(new Notification(emittedAt, killed1, killed2));
        else
            lsnList.Add(new Notification(emittedAt, killed1));

        pendingAnnounces = pendingAnnounces + "l";
    }

    public static void AddTSI(string emittedFrom, string receivedAt, string extra)
    {
        tsiList.Add(new Notification(emittedFrom, receivedAt, extra));
        pendingAnnounces = pendingAnnounces + "i";
    }

    public static void AddEC(string emittedFrom, string receivedAt)
    {
        ecList.Add(new Notification(emittedFrom, receivedAt));
        pendingAnnounces = pendingAnnounces + "e";
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
