using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarUtils : MonoBehaviour
{
    private const int INVERSE_SN_CHANCE = 4000; // chance of supernova for EACH late star = 1/INVERSE_SN_CHANCE per year
    private const float PERCENTAGE_EARLY_STAGE_STARS = 0.9f;
    public static int NUMBER_OF_LATE_STAGE_STARS;

    private static readonly List<string> GEN_NAME_P1 = new List<string> { "Red", "Blu", "Lo", "Hi", "Mi", "Ni", "Mei", "Har", "Sof", "Of", "Brei", "Dar", "Lon", "Dul", "Sur", "Ex", "Int", "Ela", "Weit", "Wei", "Weu", "North", "Nor", "Su", "Sud", "Eas", "Ost", "Wes", "New", "Ol", "Sto", "Sta", "Mag", "Sil", "Go", "Vor", "Glas", "Ar", "Nar", "Ide", "Narro", "Wid", "Tal", "Klein", "Ein", "Schwarz", "Fau", "Gruen", "Wel", "Bie", "Gros", "Krasna", "Sam", "San", "Sanc", "Neu", "Alt", "Venta", "Ater", "Kol", "Kis", "Vila", "For", "Bas", "Ander", "Nis", "Las", "Hib", "Nos", "Sed", "Ara", "Au", "In", "Sid", "Il", "Ely", "Nov", "Ac", "Cara", "Sus" };
    private static readonly List<string> GEN_NAME_P2 = new List<string> { "tow", "dac", "lit", "scha", "ite", "is", "plex", "bid", "orta", "terra", "schild", "pen", "zen", "fort", "and", "key", "rut", "ei", "ear", "son", "ent", "ice", "katz", "hun", "heim", "este", "feld", "am", "as", "a", "koshka", "dom", "stin", "anz", "ko", "ja", "el", "tor", "strana", "gorod", "ae", "canis", "roma", "gallia", "nia", "don", "ton", "belga", "via", "null", "ra", "marit", "senex", "bus", "fir", "feuer", "lex", "loch", "tus", "domus", "ser", "ius", "ium", "il", "um", "us", "itz", "ca", "anov", "ski", "stein", "ev", "ine" };
    private static readonly List<string> GEN_NAME_P3 = new List<string> { "Nova", "Prime", "Beta", "Alpha", "Gamma", "Epsilon", "Delta", "Prospect", "Aster", "Eta", "Tau", "Omega", "Eden", "Mu", "Rho", "Phi" };

    public static List<Star> allStars = new List<Star>();

    public static int facViewable = 99;
    private static bool isMuted = false;
    public GameObject star;

    // Update is called once per frame
    void Update()
    {
        // Core.funcDiv is the functional-division, explained in the Core.cs file
        if (Core.funcDiv == 'b')
        {
            // Generate the galaxy with Core.N... stars
            GenerateAllStars(Core.NUMBER_OF_STARS);
            Core.funcDiv = 'c'; 
        }
    }

    // mute supernova blast sounds
    public static void MuteAll()
    {
        foreach (Star s in allStars)
        {
            s.gObj.GetComponent<AudioSource>().mute = !isMuted;
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
                if (hit.collider!=null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
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

                    if (s.factionColour==Color.black || s.factionColour == Color.white)
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
                            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This star system is home to the " + FactionUtils.allFactions[s.controllingFaction].name + " civilisation.";
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
    public void GenerateAllStars(int nStars)
    {
        int counter = 0;

        // Generate 90.0% of all stars as normal (early stage) stars
        while (counter<(nStars*PERCENTAGE_EARLY_STAGE_STARS))
        { 
            allStars.Add(new Star(Instantiate(star, GenerateStarLoc(), Quaternion.identity), false, counter, GenerateName()));
            counter = counter + 1;            
        }
        NUMBER_OF_LATE_STAGE_STARS = nStars - counter;
        // Generate the rest as late-stage stars
        while (counter<nStars)
        {
            allStars.Add(new Star(Instantiate(star, GenerateStarLoc(), Quaternion.identity), true, counter, GenerateName()));
            counter = counter + 1;            
        }
    }

    // launch all supernovae!
    public static void LaunchAllSupernovae(int nStars)
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
    public static void SupernovaAt(int ind)
    {
        List<string> factionsDying = new List<string>();
        float soundDelay = 0f;
        if (allStars[ind].controllingFaction!=99)
        {
            //Debug.Log("STUB: Supernova at " + allStars[ind].gObj.name);
            allStars[ind].controllingFaction = 99;
            
            // check the homeplanets of nearby civilisations, if they are in range... OOF!
            foreach (Faction fac in FactionUtils.allFactions)
            {
                if (Vector3.Distance(allStars[ind].gObj.transform.position, allStars[fac.homeStar].gObj.transform.position) < 0.85f) //range normally = 0.85f
                {
                    if (fac.isAlive)
                    {
                        fac.Die(allStars[ind].starName, Core.years);
                        factionsDying.Add(fac.name);
                    }
                }
            }
            if (factionsDying.Count>0)
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
            allStars[ind].gObj.transform.localScale = new Vector3 (0.005f, 0.005f, 0.005f);

            soundDelay = Vector3.Distance(allStars[ind].gObj.transform.position, GameObject.Find("ViewCam").transform.position);
            soundDelay = soundDelay / 15f;

            allStars[ind].gObj.GetComponent<AudioSource>().PlayDelayed(0.05f + soundDelay);

            Light lit = allStars[ind].gObj.GetComponent<Light>();
            lit.enabled = true;

            SuperNovaFlares.waitingFlareList.Add(new WaitingSNFlare(ind, allStars[ind].gObj.transform.position));

        }            
    }

    // generates the location of each star with increasing frequency toward the galactic nucleus (with a cutoff at the point where life should be basically unable to develop)
    public Vector3 GenerateStarLoc()
    {
        float x, y, z, fromCent;

        fromCent = Mathf.Pow(Random.Range(0.0f, 120.0f)/24.0f, 2f) + 2.0f;
        x = Random.Range(0.0f, fromCent);
        z = Mathf.Pow(fromCent, 2.0f) - Mathf.Pow(x, 2.0f);
        z = Mathf.Pow(z, 0.5f);

        float sign1 = 1.0f;
        float sign2 = 1;

        if (Random.Range(0, 2) == 1) sign1 = -1.0f;
        if (Random.Range(0, 2) == 1) sign2 = -1.0f;

        y = Random.Range(-1.0f / fromCent, 1.0f / fromCent);

        if (Random.Range(0, 2) == 1)
            return new Vector3(x*sign1, y, z*sign2);
        else
            return new Vector3(z * sign1, y, x * sign2);
    }
}

// the object of which, wait for it... stars are composed of.
public class Star
{
    public int controllingFaction;

    public GameObject gObj;
    public bool supernovaRisk;
    public string starName;

    public Color factionColour;


    public Star(GameObject inputObj, bool lateStar, int inputInd, string generated)
    {
        supernovaRisk = lateStar;
        gObj = inputObj;
        gObj.name = inputInd.ToString();
        starName = generated;
        factionColour = Color.white;
        ChangeColour(Color.white);
        controllingFaction = 95; //95 = none, 99 = gone supernova, 98 = uninhabitable by nearby supernova (only applied to home stars of "dead" civilisations), any other number represents a civilisation
    }

    // used to set faction colours
    public void ChangeColour(Color colour)
    {
        LODGroup lodG = gObj.GetComponent<LODGroup>();
        if (controllingFaction != 99 && colour == Color.black)
            colour = Color.grey;
        foreach (LOD lod in lodG.GetLODs())
        {
            foreach (Renderer r in lod.renderers)
            {
                r.material.SetColor("_Color", colour);
            }
        }
    }
}


