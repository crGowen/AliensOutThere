using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    public const int NUMBER_OF_STARS = 1500;
    public const int NUMBER_OF_FACTIONS = 14;

    private float warp;
    public static char funcDiv = 'a';
    private static float rotationDelta;
    private static float clockDelta;
    private static float yearDelta;
    private static float notificationDelta;
    public static float years = 0.0f;

    // Set camera position and look at the galactic nucleus
    // and set warp to 2 Weeks Per Second
    void Start()
    {
        GameObject.Find("ViewCam").transform.localPosition = new Vector3(-10.0f, 16.0f, -42.0f);
        GameObject.Find("ViewCam").transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        warp = 1.0f;
        funcDiv = 'b';
        GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 2 weeks per sec");
    }

    // Update is called once per frame
    void Update()
    {
        // function divider splits various functions up so that they are not all performed in one frame
        // these functions really have no need to take place every single frame, so to lighten the load on the CPU we'll
        // just alternate between functions between each frame.

        // note that this costs a tiny bit of memory, but saves the CPU usage

        switch (funcDiv)
        {
            // Generation of factions (happens after generation of stars)
            // "why isn't GenerateAllStars here too?" -- because I'm an idiot and started out tying StarUtils to a gameobject, and it's too late to decouple, it makes no functional or performance difference though
            case 'c':
                FactionUtils.GenerateAllFactions(NUMBER_OF_FACTIONS);
                FactionUtils.DisplayFacInfo(0);
                funcDiv = 'd';
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
                // time keeping, updates the clock: "t = + [] years"
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
                // launches supernovae and technosignatures... this is done once every year
                clockDelta = clockDelta + Time.deltaTime;
                rotationDelta = rotationDelta + Time.deltaTime;
                if ((years - yearDelta) > 1.0f)
                {
                    yearDelta = years;
                    StarUtils.LaunchAllSupernovae(NUMBER_OF_STARS);
                    FactionUtils.LaunchTechnosignatures();
                }
                foreach (Faction fac in FactionUtils.allFactions)
                {
                    for (int i = 0; i < fac.incomingTechSig.Count; i++)
                    {
                        if (years > fac.incomingTechSig[i].timeToReceive)
                        {
                            fac.ReadTechSig(i);
                            fac.RemoveTechSig(i);
                            i--;
                        }
                    }
                }
                funcDiv = 'd';
                break;
        }


        // Input functions here... in future projects let's put all this stuff in a dedicated file for neatness. Lessons learned.
        if (Input.GetKeyDown(KeyCode.Period))
        {
            IncreaseWarp();
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            DecreaseWarp();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
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

        if (Input.GetKeyDown(KeyCode.M))
            StarUtils.MuteAll();

        if (Input.GetKeyDown(KeyCode.F) && StarUtils.facViewable != 99)
            DisplayFac.ViewFacSt(StarUtils.facViewable);

        CameraController.Scroll();

        // each frame call the notification control, which manages the display of notable events (e.g. Supernovae) to the user
        NotificationUtils.NotificationControl();

        // function to check if a star was clicked on, it's in StarUtils because it's actually a fairly long function involving raycasts, and to avoid having to specify "StarUtils." for variables
        // however, just look most other things in this area, should be wrapped in a faction in a dedicated controls file for future projects.
        StarUtils.CheckForClick();
    }

    // self explanatory
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
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: 5 years per sec");
                break;

        }
    }

    // self explanatory
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

    // initialise values
    static Core()
    {
        rotationDelta = 0.0f;
        clockDelta = 0.0f;
        yearDelta = 0.0f;
        NotificationUtils.Init();
        FactionUtils.Init();
    }
}
