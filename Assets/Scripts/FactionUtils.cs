using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FactionUtils
{
    // NOTE THAT many of these functions would be better as non-statics, but due to the random-generation of factions keeping as many declarations as static as possible makes it easier to work with
    public static List<Faction> allFactions;
    private static bool KSneedsUpdate;
    public static List<Color> facPalette;

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

    public static void Init()
    {
        allFactions = new List<Faction>();
        KSneedsUpdate = false;
        facPalette = new List<Color>();

        facPalette.Add(new Color(1.0f, 0.0f, 0.0f));
        facPalette.Add(new Color(0.0f, 1.0f, 0.0f));
        facPalette.Add(new Color(0.0f, 0.0f, 1.0f));
        facPalette.Add(new Color(0.0f, 1.0f, 1.0f));
        facPalette.Add(new Color(1.0f, 1.0f, 0.0f));

        facPalette.Add(new Color(1.0f, 0.5f, 1.0f));
        facPalette.Add(new Color(0.5f, 0.5f, 0.0f));
        facPalette.Add(new Color(0.5f, 1.0f, 0.5f));
        facPalette.Add(new Color(0.3f, 0.3f, 1.0f));
        facPalette.Add(new Color(1.0f, 0.3f, 0.0f));

        facPalette.Add(new Color(1.0f, 0.0f, 0.5f));
        facPalette.Add(new Color(0.0f, 0.5f, 0.0f));
        facPalette.Add(new Color(0.5f, 0.0f, 0.5f));
        facPalette.Add(new Color(0.5f, 0.0f, 0.0f));
    }

    public static void StUpdate()
    {
        if (KSneedsUpdate) KilledSystemUpdate();
    }

    // self-explanatory
    public static void GenerateAllFactions(int nFac)
    {
        int counter = 0;
        bool setHome = false;
        int randN = 0;
        while (counter < nFac)
        {
            while (!setHome)
            {
                randN = Random.Range(0, StarUtils.allStars.Count);
                if (StarUtils.allStars[randN].controllingFaction == "none")
                {
                    allFactions.Add(GenerateFactionAt(randN, counter));
                    setHome = true;
                }
            }
            setHome = false;
            counter = counter + 1;
        }
        StarUtils.RefreshView();
    }

    // the colonising function: if a faction tries to colonise a nearby star, this is called.
    public static void AssignFactionTo(int stIndex, Faction faction)
    {
        if (StarUtils.allStars[stIndex].controllingFaction=="none")
        {
            StarUtils.allStars[stIndex].controllingFaction = faction.name;
            StarUtils.allStars[stIndex].factionColour = faction.fcol;
            faction.controlledSystems.Add(stIndex);
        }
    }

    // called right at the star, generates a NEW faction at the specified star
    public static Faction GenerateFactionAt(int stIndex, int fCol)
    {
        string cname = "";
        bool PVreached = false;

        foreach (char c in StarUtils.allStars[stIndex].gObj.name)
        {
            if (c == ' ') PVreached = true;
            if (!PVreached) cname = cname + c;
        }
        StarUtils.allStars[stIndex].controllingFaction = cname;
        StarUtils.allStars[stIndex].factionColour = facPalette[fCol];
        return new Faction(cname, stIndex, fCol);
    }

    public static void KilledSystemUpdate()
    {
        Debug.Log("KSU");
        foreach (Faction f in allFactions)
        {
            List<int> toRemove = new List<int>();
            foreach (int s in f.controlledSystems)
            {
                if (StarUtils.allStars[s].controllingFaction == "dead")
                {
                    toRemove.Add(s);
                }
            }
            toRemove.Reverse();
            foreach (int s in toRemove)
            {
                f.controlledSystems.RemoveAt(s);
            }
        }
        KSneedsUpdate = false;
    }

    // everytime a major event occurs that requires changing the UI faction list, call this
    public static void UpdateFactionList()
    {
        //stub
    }

    // select faction, when the user clicks a faction in the list, zoom into the barycenter of all the faction's stars and switch to civilisation view
    public static void SelectFaction(int factionIndex)
    {
        //stub
    }

    public static void SetKSUpdateOn()
    {
        KSneedsUpdate = true;
    }
}

public class Faction
{
    public string name;
    public Color fcol;
    public int techLvl;
    public List<int> controlledSystems;

    public Faction(string deriveHome, int homeIndex, int colInd)
    {
        name = deriveHome;
        techLvl = 1;
        controlledSystems = new List<int> { homeIndex };
        fcol = FactionUtils.facPalette[colInd];
    }
}
