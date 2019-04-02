using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction
{
    public string name;
    public Color fcol;
    public int homeStar;
    public int index;
    public List<Technosignature> incomingTechSig;
    public List<int> identifiedTechSigs;
    public List<int> contactedCivs;
    public bool isAlive;
    public string moreInfo;

    public Faction(string deriveHome, int homeIndex, Color colour, int ind)
    {
        name = deriveHome;
        homeStar = homeIndex;
        index = ind;
        identifiedTechSigs = new List<int>();
        contactedCivs = new List<int>();
        fcol = colour;
        incomingTechSig = new List<Technosignature>();
        isAlive = true;

        // Generate a little extra snippet of info on how likely the faction is to find nearby intelligent life that they can communicate with, and also get fried by gamma rays when nearby stars go supernova.
        if (Vector3.Distance(Core.allStars[homeStar].gObj.transform.position, new Vector3(0f, 0f, 0f)) < 3.0f)
            moreInfo = "This civilisation's region of space is dense with stars that may harbour intelligent life, but the danger of death by supernova is great.";
        else if (Vector3.Distance(Core.allStars[homeStar].gObj.transform.position, new Vector3(0f, 0f, 0f)) < 5.0f)
            moreInfo = "This civilisation's region of space is moderately filled with stars that may harbour intelligent life, and the danger of death by supernova is moderate.";
        else
            moreInfo = "This civilisation's region of space has few nearby stars that may harbour intelligent life, but the danger of death by supernova is small.";

        if (homeStar >= (Core.allStars.Count - Core.NUMBER_OF_LATE_STAGE_STARS))
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
                if (identified == incomingTechSig[ind].sender)
                {
                    state = '1';
                    // if the techsig being read comes from a region from which we've already received a tech sig, we do THIS: (this is ALREADY an area of interest)
                    // Check if the sender has technosignaures from us, if they do... this message was a "reply" and therefore we upgrade the state of awareness to "contact established"!
                    // if NO to above: state = 1, if YES: state = 2

                    foreach (int senderKnows in Core.allFactions[incomingTechSig[ind].sender].identifiedTechSigs)
                    {
                        if (senderKnows == index)
                        {
                            if (Core.allFactions[incomingTechSig[ind].sender].isAlive)
                                state = '2';
                            foreach (int contacted in contactedCivs)
                            {
                                if (contacted == incomingTechSig[ind].sender) state = '3';
                            }
                        }
                    }
                }
                // if not state remains '0'
            }

            switch (state)
            {
                case '0':
                    identifiedTechSigs.Add(incomingTechSig[ind].sender);
                    if (Core.allFactions[incomingTechSig[ind].sender].isAlive)
                        NotificationUtils.AddTSI(Core.allStars[Core.allFactions[incomingTechSig[ind].sender].homeStar].starName, name, "");
                    else
                        NotificationUtils.AddTSI(Core.allStars[Core.allFactions[incomingTechSig[ind].sender].homeStar].starName, name, ", one of the last signals of a long dead civilisation");
                    break;
                case '2':
                    contactedCivs.Add(incomingTechSig[ind].sender);
                    Core.allFactions[incomingTechSig[ind].sender].contactedCivs.Add(index);
                    NotificationUtils.AddEC(Core.allFactions[incomingTechSig[ind].sender].name, name);
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
        if (Core.allStars[homeStar].controllingFaction != 99) // 99 means the star went supernova and turns into nebula
            Core.allStars[homeStar].controllingFaction = 98; // 98 means the super was caught by nearby supernova

        name = name + " (dead)";
        fcol = Color.black;
        isAlive = false;
        moreInfo = "This civilisation was wiped out at T= +" + (date * 10f).ToString("0") + " years, when nearby star " + killer + " went supernova.";
        Core.allStars[homeStar].factionColour = Color.black;
        Core.allStars[homeStar].ChangeColour(Color.black);
    }
}
