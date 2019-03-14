using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayFac : MonoBehaviour
{
    // When clicking on a civilisation in the left hand civilisation list, this function is called, which basically looks at the clicked civilisation
    public void ClickFunc(int nClicked)
    {
        Core.facViewable = 99;
        if (nClicked<Core.allFactions.Count)
        {
            if (Vector3.Distance(GameObject.Find("ViewCam").transform.position, Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform.position) > 6.0f)
                GameObject.Find("ViewCam").transform.position = new Vector3(Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform.position.x - 4f, Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform.position.y + 3f, Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform.position.z - 4f);

            GameObject.Find("ViewCam").transform.LookAt(Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform);
            Core.DisplayFacInfo(nClicked);
        }
        else Debug.Log("Button Should be inactive!");
    }

    // this is the same as above, but is adapted for when a star is clicked on (which contains a faction), and the user presses "F" to view the controlling faction
    public static void ViewFacSt(int nClicked)
    {
        Core.facViewable = 99;
        if (Vector3.Distance(GameObject.Find("ViewCam").transform.position, Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform.position) > 6.0f)
            GameObject.Find("ViewCam").transform.position = new Vector3(Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform.position.x - 4f, Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform.position.y + 3f, Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform.position.z - 4f);

        GameObject.Find("ViewCam").transform.LookAt(Core.allStars[Core.allFactions[nClicked].homeStar].gObj.transform);
        Core.DisplayFacInfo(nClicked);
    }
}
