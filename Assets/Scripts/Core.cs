using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
    private static List<string> GEN_NAME_P1 = new List<string> { "Red", "Blu", "Lo", "Hi", "Mi", "Ni", "Mei", "Har", "Sof", "Of", "Brei", "Dar", "Lon", "Dul", "Sur", "Ex", "Int", "Ela", "Weit", "Wei", "Weu", "North", "Nor", "Su", "Sud", "Eas", "Ost", "Wes", "New", "Ol", "Sto", "Sta", "Mag", "Sil", "Go", "Vor", "Glas", "Ar", "Nar", "Ide", "Narro", "Wid", "Tal", "Klein", "Ein", "Schwarz", "Fau", "Gruen", "Wel", "Bie", "Gros", "Krasna", "Sam", "San", "Sanc", "Neu", "Alt", "Venta", "Ater", "Kol", "Kis", "Vila", "For", "Bas", "Ander", "Nis", "Las", "Hib", "Nos", "Sed", "Ara", "Au", "In", "Sid", "Il", "Ely", "Nov", "Ac", "Cara", "Sus" };
    private static List<string> GEN_NAME_P2 = new List<string> { "tow", "dac", "lit", "scha", "ite", "is", "plex", "bid", "orta", "terra", "schild", "pen", "zen", "fort", "and", "key", "rut", "ei", "ear", "son", "ent", "ice", "katz", "hun", "heim", "este", "feld", "am", "as", "a", "koshka", "dom", "stin", "anz", "ko", "ja", "el", "tor", "strana", "gorod", "ae", "canis", "roma", "gallia", "nia", "don", "ton", "belga", "via", "null", "ra", "marit", "senex", "bus", "fir", "feuer", "lex", "loch", "tus", "domus", "ser", "ius", "ium", "il", "um", "us", "itz", "ca", "anov", "ski", "stein", "ev", "ine" };
    private static List<string> GEN_NAME_P3 = new List<string> { "Nova", "Prime", "Beta", "Alpha", "Gamma", "Epsilon", "Delta", "Prospect", "Aster", "Eta", "Tau", "Omega", "Eden", "Mu", "Rho", "Phi" };

    private const int INVERSE_SN_CHANCE = 4000; // chance of supernova for EACH late star = 1/INVERSE_SN_CHANCE per year
    private const float PERCENTAGE_EARLY_STAGE_STARS = 0.9f;
    private const float BEAM_RADIUS = 0.0005f;
    private const float BEAM_RANGE = 7.5f;
    public static int NUMBER_OF_LATE_STAGE_STARS;

    public static List<Star> allStars = new List<Star>();
    public static List<Faction> allFactions = new List<Faction>();
    public static List<SuperNovaFlare> flareList = new List<SuperNovaFlare>();

    public static int facViewable = 99;
    public static bool isMuted = false;

    public GameObject star;
    public GameObject SNflare;
    public GameObject soundSource;

    private static float warp = 1.0f;
    private static float yearDelta = 0.0f;
    public static float years = 0.0f;

    void Start()
    {
        GameObject.Find("ViewCam").transform.localPosition = new Vector3(-10.0f, 16.0f, -42.0f);
        GameObject.Find("ViewCam").transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~5 months per sec");
        GenerateAllStars();
        GenerateAllFactions();
        DisplayFacInfo(0);
        NotificationUtils.Init();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Star s in allStars)
        {
            s.gObj.transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * (-0.002f) * warp);
        }
        years = years + (Time.deltaTime * warp / 26.0f);
        GameObject.Find("ElapsedTime").GetComponent<TMPro.TextMeshProUGUI>().SetText("T: +" + (years*10f).ToString("0.00") + " years");
        StUpdate(Time.deltaTime);
        UpdateDisplay();
        if ((years - yearDelta) > 1.0f)
        {
            yearDelta = years;
            LaunchAllSupernovae(allStars.Count);
            LaunchTechnosignatures();
        }
        for (int i = 0; i < flareList.Count; i++)
        {
            if (flareList[i].timeLeft <= -4.5f)
            {
                flareList[i].Kill();
                flareList.RemoveAt(i);
                i = i - 1;
            }
            else if (flareList[i].timeLeft <= 0.0f)
            {
                flareList[i].flash.GetComponent<Light>().intensity = 10f - (-20f * flareList[i].timeLeft);
                flareList[i].timeLeft = flareList[i].timeLeft - Time.deltaTime;
            }
            else
                flareList[i].timeLeft = flareList[i].timeLeft - Time.deltaTime;
        }
        foreach (Faction fac in allFactions)
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
        // each frame call the notification control, which manages the display of notable events (e.g. Supernovae) to the user
        NotificationUtils.NotificationControl();
        InputController.CheckInputs();
    }

    // mute supernova blast sounds
    public static void MuteAll()
    {
        foreach (SuperNovaFlare s in flareList)
        {
            s.audio.GetComponent<AudioSource>().mute = !isMuted;
        }
        if (isMuted)
            GameObject.Find("MutedIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("");
        else
            GameObject.Find("MutedIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("SOUND MUTED");
        isMuted = !isMuted;
    }

    // self explanatory... generates a random name for the star
    public static string GenerateName()
    {
        bool uniqueName = false;
        string generated = "";
        while (!uniqueName)
        {
            generated = "";
            generated = generated + GEN_NAME_P1[Random.Range(0, GEN_NAME_P1.Count)];
            generated = generated + GEN_NAME_P2[Random.Range(0, GEN_NAME_P2.Count)];
            generated = generated + " " + GEN_NAME_P3[Random.Range(0, GEN_NAME_P3.Count)];
            uniqueName = true;
            foreach (Star s in allStars)
            {
                if (s.starName == generated)
                {
                    uniqueName = false;
                }
            }
        }
        return generated;
    }

    // uses a raycast to check if a star was clicked on (if so, zoom in on star and display info about it)
    public static void CheckForClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    facViewable = 99;
                    if (Vector3.Distance(GameObject.Find("ViewCam").transform.position, hit.collider.gameObject.transform.position) > 3.0f)
                        GameObject.Find("ViewCam").transform.position = new Vector3(hit.collider.gameObject.transform.position.x - 1.5f, hit.collider.gameObject.transform.position.y + 1f, hit.collider.gameObject.transform.position.z - 1.5f);
                    GameObject.Find("ViewCam").transform.LookAt(hit.collider.gameObject.transform);

                    Star s = allStars[int.Parse(hit.collider.gameObject.name)];

                    if (s.controllingFaction == 99)
                        GameObject.Find("CivilisationName").GetComponent<UnityEngine.UI.Text>().text = s.starName + " Nebula";
                    else
                        GameObject.Find("CivilisationName").GetComponent<UnityEngine.UI.Text>().text = s.starName;

                    if (s.factionColour == Color.black || s.factionColour == Color.white)
                        GameObject.Find("CivilisationName").GetComponent<UnityEngine.UI.Text>().color = Color.grey;
                    else
                        GameObject.Find("CivilisationName").GetComponent<UnityEngine.UI.Text>().color = s.factionColour;

                    GameObject.Find("CivilisationsIdentified").GetComponent<UnityEngine.UI.Text>().text = "";

                    switch (s.controllingFaction)
                    {
                        case 99:
                            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This nebula is what remains of the star " + s.starName + ", which went supernova.";
                            break;
                        case 98:
                            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "It seems this star system was once home to intelligent life, but all that remains now is ruins.";
                            break;
                        case 95:
                            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This star system is not inhabited by any civilisation.";
                            break;
                        default:
                            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This star system is home to the " + allFactions[s.controllingFaction].name + " civilisation.";
                            GameObject.Find("CivilisationsIdentified").GetComponent<UnityEngine.UI.Text>().text = "Press F to view faction.";
                            GameObject.Find("CivilisationsIdentified").GetComponent<UnityEngine.UI.Text>().color = Color.yellow;
                            facViewable = s.controllingFaction;
                            break;
                    }

                    if (s.controllingFaction != 99 && s.supernovaRisk)
                    {
                        GameObject.Find("TechsigsFound").GetComponent<UnityEngine.UI.Text>().text = "Supernova risk! This star is approaching the end of its life and has high mass.";
                        GameObject.Find("TechsigsFound").GetComponent<UnityEngine.UI.Text>().color = Color.red;
                    }
                    else
                    {
                        GameObject.Find("TechsigsFound").GetComponent<UnityEngine.UI.Text>().text = "";
                    }
                }
            }
        }
    }

    // generates the stars for the "allStars" list
    public void GenerateAllStars()
    {
        const int NUMBER_OF_STARS = 2500;
        int counter = 0;

        // Generate 90.0% of all stars as normal (early stage) stars
        while (counter < (NUMBER_OF_STARS * PERCENTAGE_EARLY_STAGE_STARS))
        {
            allStars.Add(new Star(Instantiate(star, GenerateStarLoc(), Quaternion.identity), false, counter, GenerateName()));
            counter = counter + 1;
        }
        NUMBER_OF_LATE_STAGE_STARS = NUMBER_OF_STARS - counter;
        // Generate the rest as late-stage stars
        while (counter < NUMBER_OF_STARS)
        {
            allStars.Add(new Star(Instantiate(star, GenerateStarLoc(), Quaternion.identity), true, counter, GenerateName()));
            counter = counter + 1;
        }

        GEN_NAME_P1.Clear();
        GEN_NAME_P2.Clear();
        GEN_NAME_P3.Clear();
    }

    // launch all supernovae!
    public void LaunchAllSupernovae(int nStars)
    {
        int randN;
        for (int i = nStars - NUMBER_OF_LATE_STAGE_STARS; i < nStars; i++)
        {
            randN = Random.Range(0, INVERSE_SN_CHANCE);
            if (randN == 0)
                SupernovaAt(i);
        }
    }

    // called by the above function, and checks who (if anybody) should die when the supernova goes off
    public void SupernovaAt(int ind)
    {
        List<string> factionsDying = new List<string>();
        float soundDelay = 0f;
        if (allStars[ind].controllingFaction != 99)
        {
            //Debug.Log("STUB: Supernova at " + allStars[ind].gObj.name);
            allStars[ind].controllingFaction = 99;

            // check the homeplanets of nearby civilisations, if they are in range... OOF!
            foreach (Faction fac in allFactions)
            {
                if (Vector3.Distance(allStars[ind].gObj.transform.position, allStars[fac.homeStar].gObj.transform.position) < 0.85f) //range normally = 0.85f
                {
                    if (fac.isAlive)
                    {
                        fac.Die(allStars[ind].starName, years);
                        factionsDying.Add(fac.name);
                    }
                }
            }
            if (factionsDying.Count > 0)
            {
                if (factionsDying.Count > 2)
                    NotificationUtils.AddLethalSupernova(allStars[ind].starName, "", factionsDying.Count.ToString());
                else if (factionsDying.Count > 1)
                    NotificationUtils.AddLethalSupernova(allStars[ind].starName, factionsDying[0], factionsDying[1]);
                else
                    NotificationUtils.AddLethalSupernova(allStars[ind].starName, factionsDying[0], "");
            }
            //else
            //NotificationUtils.AddSupernova(allStars[ind].gObj.name);
            //NOTIFICATION DISABLED!

            allStars[ind].factionColour = Color.black;
            allStars[ind].ChangeColour(Color.black);
            allStars[ind].gObj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

            soundDelay = Vector3.Distance(allStars[ind].gObj.transform.position, GameObject.Find("ViewCam").transform.position);
            soundDelay = soundDelay / 15f;
            soundDelay = soundDelay + 0.05f;

            if (soundDelay > 2.5f)
                soundDelay = 0.0f;

            Light lit = allStars[ind].gObj.GetComponent<Light>();
            lit.enabled = true;

            flareList.Add(new SuperNovaFlare(Instantiate(SNflare, allStars[ind].gObj.transform.position, Quaternion.identity),
                Instantiate(soundSource, allStars[ind].gObj.transform.position, Quaternion.identity), soundDelay));
        }
    }

    // generates the location of each star with increasing frequency toward the galactic nucleus (with a cutoff at the point where life should be basically unable to develop)
    public Vector3 GenerateStarLoc()
    {
        float x, y, z, fromCent;

        fromCent = Mathf.Pow(Random.Range(0.0f, 120.0f) / 24.0f, 2f) + 2.0f;
        x = Random.Range(0.0f, fromCent);
        z = Mathf.Pow(fromCent, 2.0f) - Mathf.Pow(x, 2.0f);
        z = Mathf.Pow(z, 0.5f);

        float sign1 = 1.0f;
        float sign2 = 1;

        if (Random.Range(0, 2) == 1) sign1 = -1.0f;
        if (Random.Range(0, 2) == 1) sign2 = -1.0f;

        y = Random.Range(-1.0f / fromCent, 1.0f / fromCent);

        if (Random.Range(0, 2) == 1)
            return new Vector3(x * sign1, y, z * sign2);
        else
            return new Vector3(z * sign1, y, x * sign2);
    }


    // Updates the left-hand faction list
    public static void UpdateDisplay()
    {
        for (int i = 0; i < 14; i++)
        {
            if (i < allFactions.Count)
            {
                GameObject.Find("Entry" + i.ToString()).GetComponent<UnityEngine.UI.Text>().text = allFactions[i].name;
                GameObject.Find("Entry" + i.ToString()).GetComponent<UnityEngine.UI.Text>().color = allFactions[i].fcol;
                GameObject.Find("BE" + i.ToString()).transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            else
            {
                GameObject.Find("BE" + i.ToString()).transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
    }

    // when a faction is clicked on, this function is called, which basically fills in the lower pane with the faction's info
    public static void DisplayFacInfo(int index)
    {
        string tsbuild = "";
        string cibuild = "";

        if (allFactions[index].identifiedTechSigs.Count > 0)
        {
            if (allFactions[index].identifiedTechSigs.Count > 1) tsbuild = "Identified technosignatures from the ";
            else tsbuild = "Identified a technosignature from the ";
            tsbuild = tsbuild + allFactions[allFactions[index].identifiedTechSigs[0]].name;

            for (int i = 1; i < allFactions[index].identifiedTechSigs.Count; i++)
            {
                tsbuild = tsbuild + ", " + allFactions[allFactions[index].identifiedTechSigs[i]].name;
            }

            tsbuild = tsbuild + ".";
            GameObject.Find("TechsigsFound").GetComponent<UnityEngine.UI.Text>().color = Color.green;
        }
        else
        {
            tsbuild = "No technosignatures identified.";
            GameObject.Find("TechsigsFound").GetComponent<UnityEngine.UI.Text>().color = Color.red;
        }

        if (allFactions[index].contactedCivs.Count > 0)
        {
            cibuild = "Communicated with the ";
            cibuild = cibuild + allFactions[allFactions[index].contactedCivs[0]].name;

            for (int i = 1; i < allFactions[index].contactedCivs.Count; i++)
            {
                cibuild = cibuild + ", " + allFactions[allFactions[index].contactedCivs[i]].name;
            }

            cibuild = cibuild + ".";
            GameObject.Find("CivilisationsIdentified").GetComponent<UnityEngine.UI.Text>().color = Color.green;
        }
        else
        {
            cibuild = "Communicated with no other civilisations.";
            GameObject.Find("CivilisationsIdentified").GetComponent<UnityEngine.UI.Text>().color = Color.red;
        }

        GameObject.Find("CivilisationName").GetComponent<UnityEngine.UI.Text>().text = "The " + allFactions[index].name;
        if (allFactions[index].fcol == Color.black)
            GameObject.Find("CivilisationName").GetComponent<UnityEngine.UI.Text>().color = Color.grey;
        else
            GameObject.Find("CivilisationName").GetComponent<UnityEngine.UI.Text>().color = allFactions[index].fcol;


        if (allFactions[index].isAlive)
            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This civilisation's home star is " + allStars[allFactions[index].homeStar].starName + ". " + allFactions[index].moreInfo;
        else
            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This civilisation's home star was " + allStars[allFactions[index].homeStar].starName + ". " + allFactions[index].moreInfo;

        //GameObject.Find("ControlledStars").GetComponent<UnityEngine.UI.Text>().text = "Controlled Stars: " + allFactions[index].controlledSystems.Count.ToString();
        //GameObject.Find("ControlledStars").GetComponent<UnityEngine.UI.Text>().text = ""; // unecessary feature

        GameObject.Find("TechsigsFound").GetComponent<UnityEngine.UI.Text>().text = tsbuild;

        GameObject.Find("CivilisationsIdentified").GetComponent<UnityEngine.UI.Text>().text = cibuild;
    }

    // this function counts down the timers on the receiving of techsignatures. In real life a signal has to travel at the speed of light and can take centuries to reach us,
    // in Aliens Out There, this is faked: essentially the technosignature is "added" to the civilisation as soon as it is emitted,
    // BUT the civilisation doesn't get to acknowledge it until a timer counts down to 0. The starting value of the timer is a function of the distance between the sender and receiver of the signal... larger distances have larger timers
    public static void StUpdate(float timeDelta)
    {
        foreach (Faction fac in allFactions)
        {
            foreach (Technosignature techsig in fac.incomingTechSig)
            {
                techsig.timeToReceive = techsig.timeToReceive - timeDelta;
            }
        }
    }

    // generates all factions
    public static void GenerateAllFactions()
    {
        List<Color> FACTION_PALETTE = new List<Color>() {
        new Color(1.0f, 0.0f, 0.0f), new Color(0.0f, 1.0f, 0.0f), new Color(0.0f, 0.0f, 1.0f),
        new Color(0.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 0.0f), new Color(1.0f, 0.5f, 1.0f), new Color(0.5f, 0.5f, 0.0f), new Color(0.5f, 1.0f, 0.5f),
        new Color(0.3f, 0.3f, 1.0f), new Color(1.0f, 0.3f, 0.0f), new Color(1.0f, 0.0f, 0.5f), new Color(0.0f, 0.5f, 0.0f), new Color(0.5f, 0.0f, 0.5f),
        new Color(0.5f, 0.0f, 0.0f)
        };

        int counter = 0;
        const int NUMBER_OF_FACTIONS = 14;
        bool setHome = false;
        int randN = 0;
        while (counter < NUMBER_OF_FACTIONS)
        {
            while (!setHome)
            {
                randN = Random.Range(0, allStars.Count);
                if (allStars[randN].controllingFaction == 95)
                {
                    setHome = true;
                    foreach (Faction fac in allFactions)
                    {
                        if (GetFactionAdj(randN) == fac.name)
                            setHome = false;

                    }
                }
            }
            allFactions.Add(new Faction(GetFactionAdj(randN), randN, FACTION_PALETTE[counter], counter));

            allStars[randN].controllingFaction = counter;
            allStars[randN].factionColour = FACTION_PALETTE[counter];
            allStars[randN].ChangeColour(allStars[randN].factionColour);

            setHome = false;
            counter = counter + 1;
        }
    }

    // Takes the name of a star and creates a string representing the faction name / adjective of any civilisation that would call this star home... Eg: Star="Norgallia Tau" -> Faction="Norgallia"
    // It's a very simple function, just takes away the greek letter.
    public static string GetFactionAdj(int stIndex)
    {
        string cname = "";
        bool PVreached = false;

        foreach (char c in allStars[stIndex].starName)
        {
            if (c == ' ') PVreached = true;
            if (!PVreached) cname = cname + c;
        }

        return cname;
    }

    // self explanatary
    public static void LaunchTechnosignatures()
    {
        int randN;
        foreach (Faction fac in allFactions)
        {
            if (fac.isAlive)
            {
                // if statements that makes it more likely to search regions of space from which we've already recieved tech sigs!
                if (fac.identifiedTechSigs.Count > 0)
                {
                    randN = Random.Range(0, 200);
                    if (randN > 0) randN = Random.Range(0, allStars.Count);
                    else
                    {
                        randN = Random.Range(0, fac.identifiedTechSigs.Count);
                        randN = allFactions[fac.identifiedTechSigs[randN]].homeStar;
                    }
                }
                else
                {
                    randN = Random.Range(0, allStars.Count);
                }

                Vector3 aimToward = new Vector3();
                foreach (Faction f in allFactions)
                {
                    Vector3 direction = new Vector3();
                    Vector3 fudge = new Vector3();
                    aimToward = (allStars[fac.homeStar].gObj.transform.position - allStars[randN].gObj.transform.position) / Vector3.Distance(allStars[fac.homeStar].gObj.transform.position, allStars[randN].gObj.transform.position);

                    if (fac != f && Vector3.Distance(allStars[fac.homeStar].gObj.transform.position, allStars[f.homeStar].gObj.transform.position) < BEAM_RANGE)
                    {
                        direction = (allStars[fac.homeStar].gObj.transform.position - allStars[f.homeStar].gObj.transform.position) / Vector3.Distance(allStars[fac.homeStar].gObj.transform.position, allStars[f.homeStar].gObj.transform.position);
                        fudge = aimToward - direction;
                        if (fudge.x < BEAM_RADIUS && fudge.y < BEAM_RADIUS && fudge.z < BEAM_RADIUS && fudge.x > BEAM_RADIUS * (-1.0f) && fudge.y > BEAM_RADIUS * (-1.0f) && fudge.z > BEAM_RADIUS * (-1.0f))
                        {
                            //Debug.Log("Beam hit! from " + fac.name + " to " + f.name + ".");
                            // create incoming Techsig item here
                            float timeToReceive = Vector3.Distance(allStars[fac.homeStar].gObj.transform.position, allStars[randN].gObj.transform.position) * 110.0f;
                            f.AddTechSigToList(fac.index, years + timeToReceive);
                        }
                    }
                }
            }
        }
    }

    // self explanatory
    public static void IncreaseWarp()
    {
        switch (warp)
        {
            case 1.0f:
                warp = 2.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~10 months per sec");
                break;
            case 2.0f:
                warp = 4.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~20 months per sec");
                break;
            case 4.0f:
                warp = 26.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~10 years per sec");
                break;
            case 26.0f:
                warp = 130.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~50 years per sec");
                break;
            case 130.0f:
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~50 years per sec");
                break;

        }
    }

    // self explanatory
    public static void DecreaseWarp()
    {
        switch (warp)
        {
            case 1.0f:
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~5 months per sec");
                break;
            case 2.0f:
                warp = 1.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~5 months per sec");
                break;
            case 4.0f:
                warp = 2.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~10 months per sec");
                break;
            case 26.0f:
                warp = 4.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~20 months per sec");
                break;
            case 130.0f:
                warp = 26.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~10 years per sec");
                break;
            case 260.0f:
                warp = 130.0f;
                GameObject.Find("WarpIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("Time warp: ~50 years per sec");
                break;

        }

    }
}

