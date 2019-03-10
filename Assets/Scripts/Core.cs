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
    public static float yearDelta;
    public static float notificationDelta;
    public static Vector3 focusOn;
    public static float years = 0.0f;
    public static List<int> toRemove;

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
        toRemove = new List<int>();
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

        switch (funcDiv)
        {
            case 'a':
                // nothing
                break;
            case 'b':
                break;
            case 'c':
                FactionUtils.GenerateAllFactions(NUMBEROFFACTIONS);
                FactionUtils.DisplayFacInfo(0);
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
                funcDiv = 'f';
                FactionUtils.StUpdate(clockDelta);
                FactionUtils.UpdateDisplay();
                clockDelta = 0.0f;
                break;
            case 'f':
                clockDelta = clockDelta + Time.deltaTime;
                rotationDelta = rotationDelta + Time.deltaTime;
                if ((years - yearDelta) > 1.0f)
                {
                    yearDelta = years;
                    StarUtils.LaunchAllSupernovae(NUMBEROFSTARS);
                    FactionUtils.LaunchTechnosignatures();
                }
                foreach (Faction fac in FactionUtils.allFactions)
                {
                    toRemove.Clear();
                    for (int i = 0; i < fac.incomingTechSig.Count; i++)
                    {
                        if (years > fac.incomingTechSig[i].timeToReceive)
                        {
                            fac.ReadTechSig(i);
                            toRemove.Add(i);
                        }
                    }
                    // has to be seperate to prevent a very rare index out of bounds error (where multiple TechSigs are due to be received)
                    foreach (int i in toRemove)
                    {
                        fac.RemoveTechSig(i);
                    }
                }
                funcDiv = 'd';
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

        if (Input.GetKey(KeyCode.W))
            CameraController.MoveForward();

        if (Input.GetKey(KeyCode.A))
            CameraController.MoveLeft();

        if (Input.GetKey(KeyCode.S))
            CameraController.MoveBack();

        if (Input.GetKey(KeyCode.D))
            CameraController.MoveRight();

        if (Input.GetKey(KeyCode.Z))
            CameraController.MoveDown();

        if (Input.GetKey(KeyCode.X))
            CameraController.MoveUp();

        if (Input.GetKey(KeyCode.Q))
            CameraController.RollClockwise();

        if (Input.GetKey(KeyCode.E))
            CameraController.RollCounterClockwise();

        if (Input.GetMouseButton(0))
            CameraController.Rotation();

        if (Input.GetKey(KeyCode.F) && StarUtils.facViewable != 99)
            DisplayFac.ViewFacSt(StarUtils.facViewable);

        CameraController.Scroll();

        // each frame call the notification control, which manages the display of notable events (e.g. Supernovae) to the user
        NotificationUtils.NotificationControl();

        StarUtils.CheckForClick();
    }

    private void IncreaseWarp()
    {
        switch (warp)
        {
            case 1.0f:
                warp = 2.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 1 month per sec");
                break;
            case 2.0f:
                warp = 4.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 months per sec");
                break;
            case 4.0f:
                warp = 26.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 1 year per sec");
                break;
            case 26.0f:
                warp = 130.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 5 years per sec");
                break;
            case 130.0f:
                warp = 130.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 5 years per sec");
                break;
            case 260.0f:
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 10 years per sec"); //this level of time warp no longer allowed! (causes a notifications backlog at this speed)
                break;

        }
    }

    private void DecreaseWarp()
    {
        switch (warp)
        {
            case 1.0f:
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 weeks per sec");
                break;
            case 2.0f:
                warp = 1.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 weeks per sec");
                break;
            case 4.0f:
                warp = 2.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 1 month per sec");
                break;
            case 26.0f:
                warp = 4.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 months per sec");
                break;
            case 130.0f:
                warp = 26.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 1 year per sec");
                break;
            case 260.0f:
                warp = 130.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 5 years per sec");
                break;

        }
    }

    static Core()
    {
        rotationDelta = 0.0f;
        clockDelta = 0.0f;
        yearDelta = 0.0f;
        NotificationUtils.Init();
        FactionUtils.Init();
    }
}
