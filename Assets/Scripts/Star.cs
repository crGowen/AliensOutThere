using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
