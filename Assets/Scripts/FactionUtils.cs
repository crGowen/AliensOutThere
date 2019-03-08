using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FactionUtils
{
    // NOTE THAT many of these functions would be better as non-statics, but due to the random-generation of factions keeping as many declarations as static as possible makes it easier to work with
    public static List<Faction> allFactions;
    public static List<Color> facPalette;
    public static float beamRad;
    public static float beamRange;

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
            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This civilisation's star system is " + StarUtils.allStars[allFactions[index].homeStar].gObj.name + ". " + allFactions[index].moreInfo;
        else
            GameObject.Find("CapitalPlanet").GetComponent<UnityEngine.UI.Text>().text = "This civilisation's star system was " + StarUtils.allStars[allFactions[index].homeStar].gObj.name + ". " + allFactions[index].moreInfo;

        //GameObject.Find("ControlledStars").GetComponent<UnityEngine.UI.Text>().text = "Controlled Stars: " + allFactions[index].controlledSystems.Count.ToString();
        //GameObject.Find("ControlledStars").GetComponent<UnityEngine.UI.Text>().text = ""; // unecessary feature

        GameObject.Find("TechsigsFound").GetComponent<UnityEngine.UI.Text>().text = tsbuild;

        GameObject.Find("CivilisationsIdentified").GetComponent<UnityEngine.UI.Text>().text = cibuild;
    }

    public static void Init()
    {
        allFactions = new List<Faction>();
        facPalette = new List<Color>();
        beamRad = 0.0005f;
        beamRange = 7.5f;

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
                if (StarUtils.allStars[randN].controllingFaction == 95)
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

    // called right at the start, generates a NEW faction at the specified star
    public static Faction GenerateFactionAt(int stIndex, int fCol)
    {
        string cname = "";
        bool PVreached = false;

        foreach (char c in StarUtils.allStars[stIndex].gObj.name)
        {
            if (c == ' ') PVreached = true;
            if (!PVreached) cname = cname + c;
        }
        StarUtils.allStars[stIndex].controllingFaction = fCol;
        StarUtils.allStars[stIndex].factionColour = facPalette[fCol];
        return new Faction(cname, stIndex, fCol);
    }

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

                    if (fac != f && Vector3.Distance(StarUtils.allStars[fac.homeStar].gObj.transform.position, StarUtils.allStars[f.homeStar].gObj.transform.position) < beamRange)
                    {
                        // Generate random place to fire beam into... an easy way to ensure that areas of the star field that are the most
                        // dense are targetted most often, is to simply randomly target stars

                        // HOWEVER, 

                        direction = (StarUtils.allStars[fac.homeStar].gObj.transform.position - StarUtils.allStars[f.homeStar].gObj.transform.position) / Vector3.Distance(StarUtils.allStars[fac.homeStar].gObj.transform.position, StarUtils.allStars[f.homeStar].gObj.transform.position);
                        fudge = aimToward - direction;
                        if (fudge.x < beamRad && fudge.y < beamRad && fudge.z < beamRad && fudge.x > beamRad * (-1.0f) && fudge.y > beamRad * (-1.0f) && fudge.z > beamRad * (-1.0f))
                        {
                            //Debug.Log("Beam hit! from " + fac.name + " to " + f.name + ".");
                            // create incoming Techsig item here
                            float timeToReceive = Vector3.Distance(StarUtils.allStars[fac.homeStar].gObj.transform.position, StarUtils.allStars[randN].gObj.transform.position) * 110.0f;
                            f.AddTechSigToList(fac.index, Core.years + timeToReceive);

                            // Convert those 2 Debug.Logs to a notification
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
        fcol = FactionUtils.facPalette[colInd];
        incomingTechSig = new List<Technosignature>();
        isAlive = true;

        if (Vector3.Distance(StarUtils.allStars[homeStar].gObj.transform.position, new Vector3(0f, 0f, 0f)) < 3.0f)
            moreInfo = "This civilisation's region of space is dense with stars that may harbour intelligent life, but the danger of death by supernova is great.";
        else if (Vector3.Distance(StarUtils.allStars[homeStar].gObj.transform.position, new Vector3(0f, 0f, 0f)) < 5.0f)
            moreInfo = "This civilisation's region of space is moderately filled with stars that may harbour intelligent life, and the danger of death by supernova is moderate.";
        else
            moreInfo = "This civilisation's region of space has few nearby stars that may harbour intelligent life, but the danger of death by supernova is small.";

        if (homeStar >= (StarUtils.allStars.Count - StarUtils.numLatestars))
            moreInfo = "This civilisation's home star is nearing the end of its life, death by supernova is very likely!";
    }

    public void AddTechSigToList(int sender, float time)
    {
        incomingTechSig.Add(new Technosignature(sender, time));
    }

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
                    // if NO to above: state = 1, if YES: state = 2 (both states need to also send a notification explaining it)

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
                // alter some of these messages to the capital star's name!
                case '0':
                    //Debug.Log("The " + name + " civilisation has identified a technosignature originating from the region of space around the " + FactionUtils.allFactions[incomingTechSig[ind].sender].name + " civilisation!");
                    //Convert this message to notification!
                    identifiedTechSigs.Add(FactionUtils.allFactions[incomingTechSig[ind].sender].index);
                    if (FactionUtils.allFactions[incomingTechSig[ind].sender].isAlive)
                        NotificationUtils.AddTSI(StarUtils.allStars[FactionUtils.allFactions[incomingTechSig[ind].sender].homeStar].gObj.name, name, "");
                    else
                        NotificationUtils.AddTSI(StarUtils.allStars[FactionUtils.allFactions[incomingTechSig[ind].sender].homeStar].gObj.name, name, ", one of the last signals of a long dead civilisation");
                    break;
                case '1':
                    //Debug.Log("The " + name + " civilisation has now identified MULTIPLE technosignatures originating from the region of space around the " + FactionUtils.allFactions[incomingTechSig[ind].sender].name + " civilisation!");
                    //Convert this message to notification!
                    /*
                    if (FactionUtils.allFactions[incomingTechSig[ind].sender].isAlive)
                        NotificationUtils.AddTSM(StarUtils.allStars[FactionUtils.allFactions[incomingTechSig[ind].sender].homeStar].gObj.name, name, "");
                    else
                        NotificationUtils.AddTSM(StarUtils.allStars[FactionUtils.allFactions[incomingTechSig[ind].sender].homeStar].gObj.name, name, ", the last signals of a long dead civilisation");
                        */

                    //NOTIFICATION DISABLED!
                    break;
                case '2':
                    //Debug.Log("The " + name + " civilisation and the " + FactionUtils.allFactions[incomingTechSig[ind].sender].name + " civilisation have established first contact!");
                    //Convert this message to notification!
                    contactedCivs.Add(FactionUtils.allFactions[incomingTechSig[ind].sender].index);
                    FactionUtils.allFactions[incomingTechSig[ind].sender].contactedCivs.Add(index);
                    NotificationUtils.AddEC(FactionUtils.allFactions[incomingTechSig[ind].sender].name, name);
                    break;
                case '3':
                    //Debug.Log("The " + name + " civilisation and the " + FactionUtils.allFactions[incomingTechSig[ind].sender].name + " have continued in contact!");
                    // THIS PART ISN'T NECESSARY TO BE A NOTIFICATION!
                    break;
            }
        }

    }

    public void RemoveTechSig(int ind)
    {
        incomingTechSig.RemoveAt(ind);
    }

    public void Die(string killer, float date)
    {
        //the homeplanet is caught by a supernova, time to die.
        if (StarUtils.allStars[homeStar].controllingFaction!=99)
            StarUtils.allStars[homeStar].controllingFaction = 98;

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
