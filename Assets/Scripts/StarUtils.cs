using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarUtils : MonoBehaviour
{
    public static List<Star> allStars = new List<Star>();
    private static bool onRealisticView = false;
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

    // NOT YET IMPLEMENTED
    // this function builds a map for each star: a sorted list of all nearby stars, which is used to tell the controlling faction whom they can communicate with,
    // since there is a range limit for each technosignature. The fact that the list is sorted is important in the implementation of a faction's communication list

    // HOWEVER it needs to be said that this functionality is probably not going to be needed. Since all communications are going to be re-framed as between stars rather than factions
    // additionally, colonisation can be worked out on the fly (it will happen rarely enough that it really doesn't matter much)
    public static List<StarRoute> BuildStarRoutes(Star star)
    {
        List<StarRoute> srList = new List<StarRoute>();

        int counter = 0;
        while (counter < allStars.Count)
        {
            float distanceBetween = 0;
            // get distance to star s.position - this.position AND pythag?
            // if distance > [a limit] don't add to list
            // if 0, obviously also don't add (because it's THIS star)

            //srList.Add(new StarRoute(counter, distanceBetween));
            counter = counter + 1;
        }

        srList.Sort(delegate(StarRoute sr1, StarRoute sr2)
        {
            return sr1.GetDistance().CompareTo(sr2.GetDistance());
        });

        return srList;
    }

    // We'll depricate this
    public static void BuildAllStarMaps()
    {
        foreach (Star s in allStars)
        {
            //s.SetNearbyStars(BuildStarRoutes(s));
        }
    }

    // generates the stars for the "allStars" list
    public void GenerateAllStars(int nStars)
    {
        int counter = 0;
        while (counter<nStars)
        { 
            allStars.Add(new Star(Instantiate(star, GenerateStarLoc(), Quaternion.identity)));
            counter = counter + 1;
        }
    }

    // select star, when the user clicks a star, zoom in on it and show a popup of information?? maybe, or maybe this function is pointless
    public static void SelectStar(int starIndex)
    {
        //stub
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
    public List<StarRoute> nearbyStars = new List<StarRoute>();
    public string controllingFaction;

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
        controllingFaction = "none";
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

// this will be depricated
public class StarRoute
{
    public int destIndex;
    public float distance;

    public StarRoute(int inputIndex, float inputDist)
    {
        destIndex = inputIndex;
        distance = inputDist;
    }

    public int GetDestination()
    {
        return destIndex;
    }

    public float GetDistance()
    {
        return distance;
    }
}
