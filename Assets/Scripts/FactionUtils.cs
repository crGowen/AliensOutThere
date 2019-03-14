using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FactionUtils
{
    private const float BEAM_RADIUS = 0.0005f;
    private const float BEAM_RANGE = 7.5f;
    public static readonly List<Color> FACTION_PALETTE = new List<Color>() {
        new Color(1.0f, 0.0f, 0.0f), new Color(0.0f, 1.0f, 0.0f), new Color(0.0f, 0.0f, 1.0f),
        new Color(0.0f, 1.0f, 1.0f), new Color(1.0f, 1.0f, 0.0f), new Color(1.0f, 0.5f, 1.0f), new Color(0.5f, 0.5f, 0.0f), new Color(0.5f, 1.0f, 0.5f),
        new Color(0.3f, 0.3f, 1.0f), new Color(1.0f, 0.3f, 0.0f), new Color(1.0f, 0.0f, 0.5f), new Color(0.0f, 0.5f, 0.0f), new Color(0.5f, 0.0f, 0.5f),
        new Color(0.5f, 0.0f, 0.0f)
    };

    public static List<Faction> allFactions;

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
        
        if (allFactions[index].identifiedTechSigs.Count>0)
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
            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This civilisation's home star is " + StarUtils.allStars[allFactions[index].homeStar].starName + ". " + allFactions[index].moreInfo;
        else
            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This civilisation's home star was " + StarUtils.allStars[allFactions[index].homeStar].starName + ". " + allFactions[index].moreInfo;

        //GameObject.Find("ControlledStars").GetComponent<UnityEngine.UI.Text>().text = "Controlled Stars: " + allFactions[index].controlledSystems.Count.ToString();
        //GameObject.Find("ControlledStars").GetComponent<UnityEngine.UI.Text>().text = ""; // unecessary feature

        GameObject.Find("TechsigsFound").GetComponent<UnityEngine.UI.Text>().text = tsbuild;

        GameObject.Find("CivilisationsIdentified").GetComponent<UnityEngine.UI.Text>().text = cibuild;
    }

    public static void Init()
    {
        allFactions = new List<Faction>();
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
                if (StarUtils.allStars[randN].controllingFaction == 95)
                {
                    setHome = true;
                    foreach(Faction fac in allFactions)
                    {
                        if (GetFactionAdj(randN)==fac.name)
                            setHome = false;

                    }
                }
            }
            allFactions.Add(new Faction(GetFactionAdj(randN), randN, counter));

            StarUtils.allStars[randN].controllingFaction = counter;
            StarUtils.allStars[randN].factionColour = FACTION_PALETTE[counter];
            StarUtils.allStars[randN].ChangeColour(StarUtils.allStars[randN].factionColour);

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

        foreach (char c in StarUtils.allStars[stIndex].starName)
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
                    if (randN > 0) randN = Random.Range(0, StarUtils.allStars.Count);
                    else
                    {
                        randN = Random.Range(0, fac.identifiedTechSigs.Count);
                        randN = allFactions[fac.identifiedTechSigs[randN]].homeStar;
                    }
                }
                else
                {
                    randN = Random.Range(0, StarUtils.allStars.Count);
                }

                Vector3 aimToward = new Vector3();
                foreach (Faction f in allFactions)
                {
                    Vector3 direction = new Vector3();
                    Vector3 fudge = new Vector3();
                    aimToward = (StarUtils.allStars[fac.homeStar].gObj.transform.position - StarUtils.allStars[randN].gObj.transform.position) / Vector3.Distance(StarUtils.allStars[fac.homeStar].gObj.transform.position, StarUtils.allStars[randN].gObj.transform.position);

                    if (fac != f && Vector3.Distance(StarUtils.allStars[fac.homeStar].gObj.transform.position, StarUtils.allStars[f.homeStar].gObj.transform.position) < BEAM_RANGE)
                    {
                        direction = (StarUtils.allStars[fac.homeStar].gObj.transform.position - StarUtils.allStars[f.homeStar].gObj.transform.position) / Vector3.Distance(StarUtils.allStars[fac.homeStar].gObj.transform.position, StarUtils.allStars[f.homeStar].gObj.transform.position);
                        fudge = aimToward - direction;
                        if (fudge.x < BEAM_RADIUS && fudge.y < BEAM_RADIUS && fudge.z < BEAM_RADIUS && fudge.x > BEAM_RADIUS * (-1.0f) && fudge.y > BEAM_RADIUS * (-1.0f) && fudge.z > BEAM_RADIUS * (-1.0f))
                        {
                            //Debug.Log("Beam hit! from " + fac.name + " to " + f.name + ".");
                            // create incoming Techsig item here
                            float timeToReceive = Vector3.Distance(StarUtils.allStars[fac.homeStar].gObj.transform.position, StarUtils.allStars[randN].gObj.transform.position) * 110.0f;
                            f.AddTechSigToList(fac.index, Core.years + timeToReceive);
                        }
                    }
                }
            }
        }
    }
}

public class Faction
{
    public string name;
    public Color fcol;
    public int index;
    public int homeStar;
    public List<Technosignature> incomingTechSig;
    public List<int> identifiedTechSigs;
    public List<int> contactedCivs;
    public bool isAlive;
    public string moreInfo;

    public Faction(string deriveHome, int homeIndex, int colInd)
    {
        name = deriveHome;
        homeStar = homeIndex;
        index = colInd;
        identifiedTechSigs = new List<int>();
        contactedCivs = new List<int>();
        fcol = FactionUtils.FACTION_PALETTE[colInd];
        incomingTechSig = new List<Technosignature>();
        isAlive = true;

        // Generate a little extra snippet of info on how likely the faction is to find nearby intelligent life that they can communicate with, and also get fried by gamma rays when nearby stars go supernova.
        if (Vector3.Distance(StarUtils.allStars[homeStar].gObj.transform.position, new Vector3(0f, 0f, 0f)) < 3.0f)
            moreInfo = "This civilisation's region of space is dense with stars that may harbour intelligent life, but the danger of death by supernova is great.";
        else if (Vector3.Distance(StarUtils.allStars[homeStar].gObj.transform.position, new Vector3(0f, 0f, 0f)) < 5.0f)
            moreInfo = "This civilisation's region of space is moderately filled with stars that may harbour intelligent life, and the danger of death by supernova is moderate.";
        else
            moreInfo = "This civilisation's region of space has few nearby stars that may harbour intelligent life, but the danger of death by supernova is small.";

        if (homeStar >= (StarUtils.allStars.Count - StarUtils.NUMBER_OF_LATE_STAGE_STARS))
            moreInfo = "This civilisation's home star is nearing the end of its life, death by supernova is very likely!";
    }

    // self explanatary, this gets called at the exact time that the tech sig is emitted
    public void AddTechSigToList(int sender, float time)
    {
        incomingTechSig.Add(new Technosignature(sender, time));
    }

    // this gets called after the signal has "travelled" through space (the travel is faked with a simple timer)
    public void ReadTechSig(int ind)
    {
        if (isAlive)
        {
            char state = '0';
            foreach (int identified in identifiedTechSigs)
            {
                if (identified == FactionUtils.allFactions[incomingTechSig[ind].sender].index)
                {
                    state = '1';
                    // if the techsig being read comes from a region from which we've already received a tech sig, we do THIS: (this is ALREADY an area of interest)
                    // Check if the sender has technosignaures from us, if they do... this message was a "reply" and therefore we upgrade the state of awareness to "contact established"!
                    // if NO to above: state = 1, if YES: state = 2

                    foreach (int senderKnows in FactionUtils.allFactions[incomingTechSig[ind].sender].identifiedTechSigs)
                    {
                        if (senderKnows == index)
                        {
                            if (FactionUtils.allFactions[incomingTechSig[ind].sender].isAlive)
                                state = '2';
                            foreach (int contacted in contactedCivs)
                            {
                                if (contacted == FactionUtils.allFactions[incomingTechSig[ind].sender].index) state = '3';
                            }
                        }
                    }
                }
                // if not state remains '0'
            }

            switch (state)
            {
                case '0':
                    identifiedTechSigs.Add(FactionUtils.allFactions[incomingTechSig[ind].sender].index);
                    if (FactionUtils.allFactions[incomingTechSig[ind].sender].isAlive)
                        NotificationUtils.AddTSI(StarUtils.allStars[FactionUtils.allFactions[incomingTechSig[ind].sender].homeStar].starName, name, "");
                    else
                        NotificationUtils.AddTSI(StarUtils.allStars[FactionUtils.allFactions[incomingTechSig[ind].sender].homeStar].starName, name, ", one of the last signals of a long dead civilisation");
                    break;
                case '2':
                    contactedCivs.Add(FactionUtils.allFactions[incomingTechSig[ind].sender].index);
                    FactionUtils.allFactions[incomingTechSig[ind].sender].contactedCivs.Add(index);
                    NotificationUtils.AddEC(FactionUtils.allFactions[incomingTechSig[ind].sender].name, name);
                    break;
            }
        }

    }

    // self explanatary, always gets called after ReadTechSig()
    public void RemoveTechSig(int ind)
    {
        incomingTechSig.RemoveAt(ind);
    }

    // the homeplanet is caught by a supernova, time to die.
    public void Die(string killer, float date)
    {
        if (StarUtils.allStars[homeStar].controllingFaction!=99) // 99 means the star went supernova and turns into nebula
            StarUtils.allStars[homeStar].controllingFaction = 98; // 98 means the super was caught by nearby supernova

        name = name + " (dead)";
        fcol = Color.black;
        isAlive = false;
        moreInfo = "This civilisation was wiped out at T= +" + date.ToString("0") + " years, when nearby star " + killer + " went supernova.";
        StarUtils.allStars[homeStar].factionColour = Color.black;
        StarUtils.allStars[homeStar].ChangeColour(Color.black);
    }
}

public class Technosignature
{
    public int sender;
    public float timeToReceive;
    //timeToReceive in YEARS

    public Technosignature(int sen, float tim)
    {
        sender = sen;
        timeToReceive = tim;
        //Debug.Log("It will arrive at +" + tim.ToString());
    }
}
