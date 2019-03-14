using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperNovaFlares : MonoBehaviour
{
    public static List<SNFlare> flareList = new List<SNFlare>();
    public static List<WaitingSNFlare> waitingFlareList = new List<WaitingSNFlare>();
    public GameObject flare;


    // Any flares that are "queued up" are generated. It's done this way because of static vs. dynamic types (but again, lessons learned, this will be done differently in future projects)
    void Update()
    {
        while (waitingFlareList.Count > 0)
        {
            flareList.Add(new SNFlare(Instantiate(flare, waitingFlareList[waitingFlareList.Count - 1].location, Quaternion.identity), waitingFlareList[waitingFlareList.Count - 1].slot));
            waitingFlareList.RemoveAt(waitingFlareList.Count - 1);
        }

        // fades the flares out once they've been lit for a second
        if (Core.funcDiv=='f')
        {
            for (int i = 0; i < SuperNovaFlares.flareList.Count; i++)
            {
                if (SuperNovaFlares.flareList[i].timeLeft <= -0.4f)
                {
                    SuperNovaFlares.flareList[i].Kill();
                    SuperNovaFlares.flareList.RemoveAt(i);
                    i = i - 1;
                }
                else if (SuperNovaFlares.flareList[i].timeLeft <= 0.0f)
                {
                    SuperNovaFlares.flareList[i].gObj.GetComponent<Light>().intensity = 10f - (-20f * SuperNovaFlares.flareList[i].timeLeft);
                    SuperNovaFlares.flareList[i].timeLeft = SuperNovaFlares.flareList[i].timeLeft - Time.deltaTime;
                }
                else
                    SuperNovaFlares.flareList[i].timeLeft = SuperNovaFlares.flareList[i].timeLeft - Time.deltaTime;
            }
        }
    }


}

public class SNFlare
{
    public float timeLeft;
    public GameObject gObj;

    public SNFlare(GameObject inputObj, int ind)
    {
        timeLeft = 1.0f;
        gObj = inputObj;
        gObj.name = "SNFlare " + ind.ToString(); // this is only done for debug purposes

    }

    public void Kill()
    {
        GameObject.Destroy(gObj);
    }
}

public class WaitingSNFlare
{
    public int slot;
    public Vector3 location;

    public WaitingSNFlare(int ind, Vector3 inputL)
    {
        slot = ind;
        location = inputL;
    }
}

