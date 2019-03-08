using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarUtils : MonoBehaviour
{
    public static List<Star> allStars = new List<Star>();
    private static bool onRealisticView = false;
    public static int chanceOfSN = 15000; // chance of supernova for EACH late star = 1/chanceOfSN per year
    public static int numLatestars;
    public static bool allowViewChange = false;
    public GameObject star;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Core.funcDiv is the functional-division, explained in the Core.cs file

        // this is specifically in the Update function so that it will definitely be called after everything else is loaded
        if (Core.funcDiv == 'b')
        {
            // Generate the galaxy with Core.N... stars
            GenerateAllStars(Core.NUMBEROFSTARS);
            Core.funcDiv = 'c'; 
        }
        else if (Core.funcDiv == 'f')
        {
            if (Random.Range(0.0f, 120.0f)<0.005)
            {
                // Generate technosignature emission in 'random' direction (but not up or down on the galactic plane!)
            }
        }
    }

    // this function allows the user to toggle between the 'realistic' view and the 'civilisations' view
    public static void ChangeView()
    {
        if (allowViewChange)
        {
            if (onRealisticView)
                foreach (Star s in allStars)
                {
                    s.ChangeColour(s.factionColour); //Color.faction color when factions are implemented.
                    s.ChangeEmission(Color.black);
                    GameObject.Find("ViewIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("View Mode: Civilisations");
                    onRealisticView = false;
                }
            else
                foreach (Star s in allStars)
                {
                    s.ChangeColour(Color.red); // black when realistic is implemented
                    s.ChangeEmission(Color.black); // white when realistic is implemented
                    GameObject.Find("ViewIndicator").GetComponent<TMPro.TextMeshProUGUI>().SetText("View Mode: Realistic");
                    onRealisticView = true;
                }
            Core.focusOn = new Vector3(0.0f, 0.0f, 0.0f);
            GameObject.Find("ViewCam").transform.localPosition = new Vector3(-10.0f, 16.0f, -42.0f);
            GameObject.Find("ViewCam").transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        }
    }

    public static void RefreshView()
    {
        if(!onRealisticView)
        {
            foreach (Star s in allStars)
            {
                s.ChangeColour(s.factionColour); //Color.faction color when factions are implemented.
            }
        }
    }

    // generates the stars for the "allStars" list
    public void GenerateAllStars(int nStars)
    {
        int counter = 0;
        // Generate 90.0% of all stars as normal stars
        while (counter<(nStars*0.9f))
        { 
            allStars.Add(new Star(Instantiate(star, GenerateStarLoc(), Quaternion.identity)));
            counter = counter + 1;            
        }
        numLatestars = nStars - counter;
        // Generate the rest as late-stage stars
        while (counter<nStars)
        {
            allStars.Add(new Star(Instantiate(star, GenerateStarLoc(), Quaternion.identity)));
            counter = counter + 1;            
        }
    }

    public static void LaunchAllSupernovae(int nStars)
    {
        int randN;
        for (int i = nStars - numLatestars; i < nStars; i++)
        {
            randN = Random.Range(0, chanceOfSN);
            if (randN == 0)
                SupernovaAt(i);
        }
    }

    public static void SupernovaAt(int ind)
    {
        List<string> factionsDying = new List<string>();
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
                        fac.Die(allStars[ind].gObj.name, Core.years);
                        factionsDying.Add(fac.name);
                    }
                }
            }
            if (factionsDying.Count>0)
            {
                if (factionsDying.Count > 2)
                    NotificationUtils.AddLethalSupernova(allStars[ind].gObj.name, "", factionsDying.Count.ToString());
                else if (factionsDying.Count > 1)
                    NotificationUtils.AddLethalSupernova(allStars[ind].gObj.name, factionsDying[0], factionsDying[1]);
                else
                    NotificationUtils.AddLethalSupernova(allStars[ind].gObj.name, factionsDying[0], "");
            }
            //else
            //NotificationUtils.AddSupernova(allStars[ind].gObj.name);
            //NOTIFICATION DISABLED!

            allStars[ind].factionColour = Color.black;
            allStars[ind].ChangeColour(Color.black);
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

    // Over 70,000 unique names
    private static List<string> adjectives = new List<string> { "Red", "Blu", "Lo", "Hi", "Mi", "Ni", "Mei", "Har", "Sof", "Of", "Brei", "Dar", "Lon", "Dul", "Sur", "Ex", "Int", "Ela", "Weit", "Wei", "Weu", "North", "Nor", "Su", "Sud", "Eas", "Ost", "Wes", "New", "Ol", "Sto", "Sta", "Mag", "Sil", "Go", "Vor", "Glas", "Ar", "Nar", "Ide", "Narro", "Wid", "Tal", "Klein", "Ein", "Schwarz", "Fau", "Gruen", "Wel", "Bie", "Gros", "Krasna", "Sam", "San", "Sanc", "Neu", "Alt", "Venta", "Ater", "Kol", "Kis", "Vila", "For", "Bas", "Ander", "Nis", "Las", "Hib", "Nos", "Sed", "Ara", "Au", "In", "Sid", "Il", "Ely", "Nov", "Ac", "Cara", "Sus" };
    private static List<string> nouns = new List<string> { "tow", "dac", "lit", "scha", "ite", "is", "plex", "bid", "orta", "terra", "schild", "pen", "zen", "fort", "and", "key", "rut", "ei", "ear", "son", "ent", "ice", "katz", "hun", "heim", "este", "feld", "am", "as", "a", "koshka", "dom", "stin", "anz", "ko", "ja", "el", "tor", "strana", "gorod", "ae", "canis", "roma", "gallia", "nia", "don", "ton", "belga", "via", "null", "ra", "marit", "senex", "bus", "fir", "feuer", "lex", "loch", "tus", "domus", "ser", "ius", "ium", "il", "um", "us", "itz", "ca", "anov", "ski", "stein", "ev", "ine" };
    private static List<string> postverbus = new List<string> { "Nova", "Prime", "Beta", "Alpha", "Gamma", "Epsilon", "Delta", "Prospect", "Aster", "Eta", "Tau", "Omega", "Eden", "Mu", "Rho", "Phi" };
    public GameObject gObj;

    public Color factionColour;


    public Star(GameObject inputObj)
    {
        gObj = inputObj;
        gObj.name = GenerateName();
        factionColour = Color.white;
        ChangeColour(Color.white);
        ChangeEmission(Color.black);
        controllingFaction = 95; //95 = none, 99 = gone supernova, 98 = uninhabitable by nearby supernova (only applied to home stars of "dead" civilisations
    }

    // used for the change view function, but it as of yet unimplemented
    public void ChangeColour(Color colour)
    {
        LODGroup lodG = gObj.GetComponent<LODGroup>();
        foreach (LOD lod in lodG.GetLODs())
        {
            foreach (Renderer r in lod.renderers)
            {
                r.material.SetColor("_Color", colour);
            }
        }
    }

    // used for the change view function, but it as of yet unimplemented
    public void ChangeEmission(Color colour)
    {
        LODGroup lodG = gObj.GetComponent<LODGroup>();
        foreach (LOD lod in lodG.GetLODs())
        {
            foreach (Renderer r in lod.renderers)
            {
                r.material.SetColor("_EmissionColor", colour);
            }
        }
    }

    // Generates a random name of format: Adjnoun Postverbus (adjectives and nouns were actually originally used, but ultimately it changed)
    private string GenerateName()
    {
        string generated = "";
        generated = generated + adjectives[Random.Range(0, adjectives.Count)];
        generated = generated + nouns[Random.Range(0, nouns.Count)];
        generated = generated + " " + postverbus[Random.Range(0, postverbus.Count)];

        return generated;
    }
}
