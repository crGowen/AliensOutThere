using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public float warp;
    public static char funcDiv = 'a';
    public static int NUMBEROFSTARS;
    public static int NUMBEROFFACTIONS;
    public static float rotationDelta;
    public static float clockDelta;
    public static float notificationDelta;
    public static Vector3 focusOn;
    public float years = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        focusOn = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject.Find("ViewCam").transform.localPosition = new Vector3(-10.0f, 16.0f, -42.0f);
        GameObject.Find("ViewCam").transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        warp = 1.0f;
        NUMBEROFSTARS = 5000; // maximum of 8000
        NUMBEROFFACTIONS = 14; // maximum of 16
        funcDiv = 'b';
        GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 weeks per sec");
        GameObject.Find("ViewIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("View Mode: Civilisations");
    }

    // Update is called once per frame
    void Update()
    {
        // function divider splits various functions up so that they are not all performed in one frame
        // these functions really have no need to take place at 60fps, so to lighten the load on the CPU we'll
        // just alternate between functions between each frame.

        // note that this costs memory, but saves the CPU usage
        switch(funcDiv)
        {
            case 'a':
                // nothing
                break;
            case 'b':
                break;
            case 'c':
                FactionUtils.GenerateAllFactions(NUMBEROFFACTIONS);
                funcDiv = 'd';
                StarUtils.allowViewChange = true;
                break;
            case 'd':
                // rotate the stars around the galactic nucleus (speed of rotation depends on time warp)
                rotationDelta = rotationDelta + Time.deltaTime;
                clockDelta = clockDelta + Time.deltaTime;
                foreach (Star s in StarUtils.allStars)
                {
                    s.gObj.transform.RotateAround(Vector3.zero, Vector3.up, rotationDelta * (-0.002f) * warp);
                }
                rotationDelta = 0.0f;
                funcDiv = 'e';
                break;
            case 'e':
                // time keeping
                clockDelta = clockDelta + Time.deltaTime;
                rotationDelta = rotationDelta + Time.deltaTime;
                years = years + (clockDelta * warp/26.0f);
                GameObject.Find("ElapsedTime").GetComponent<TMPro.TextMeshProUGUI>().SetText("T: +" + years.ToString("0.00") + " years");
                clockDelta = 0.0f;
                funcDiv = 'd';
                FactionUtils.StUpdate();
                FactionUtils.UpdateDisplay();
                break;
            default:
                // catch error
                break;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // press space to change view
            StarUtils.ChangeView();
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            // press . (>) to increase warp
            IncreaseWarp();
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            // press , (<) to decrease warp
            DecreaseWarp();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // press , (<) to decrease warp
            focusOn = new Vector3(0.0f, 0.0f, 0.0f);
            GameObject.Find("ViewCam").transform.localPosition = new Vector3(-10.0f, 16.0f, -42.0f);
            GameObject.Find("ViewCam").transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        }

        GameObject.Find("ViewCam").transform.localPosition = Vector3.MoveTowards(GameObject.Find("ViewCam").transform.localPosition, focusOn, Input.GetAxis("Mouse ScrollWheel")*8.0f);
        

        // each frame call the notification control, which manages the display of notable events (e.g. Supernovae) to the user
        NotificationUtils.NotificationControl();
    }

    private void IncreaseWarp()
    {
        switch (warp)
        {
            case 1.0f:
                // 2 weeks a second
                warp = 2.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 1 month per sec");
                break;
            case 2.0f:
                // 4 weeks a second
                warp = 4.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 months per sec");
                break;
            case 4.0f:
                // 2 months a second
                warp = 24.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 1 year per sec");
                break;
            case 24.0f:
                // 1 year a second
                warp = 48.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 years per sec");
                break;
            case 48.0f:
                // 2 years a second
                warp = 240.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 10 years per sec");
                break;
            case 240.0f:
                // 10 years a second
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 10 years per sec");
                break;

        }
    }

    private void DecreaseWarp()
    {
        switch (warp)
        {
            case 1.0f:
                // 2 weeks a second
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 weeks per sec");
                break;
            case 2.0f:
                // 4 weeks a second
                warp = 1.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 weeks per sec");
                break;
            case 4.0f:
                // 2 months a second
                warp = 2.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 1 month per sec");
                break;
            case 24.0f:
                // 1 year a second
                warp = 4.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 months per sec");
                break;
            case 48.0f:
                // 2 years a second
                warp = 24.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 1 year per sec");
                break;
            case 240.0f:
                // 10 years a second
                warp = 48.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 years per sec");
                break;

        }
    }

    static Core()
    {
        rotationDelta = 0.0f;
        clockDelta = 0.0f;
        NotificationUtils.Init();
        FactionUtils.Init();
    }
}
