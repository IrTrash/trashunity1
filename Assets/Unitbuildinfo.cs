using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unitbuildinfo : MonoBehaviour
{
    public GameObject result;
    public int time;



    


    public bool check()
    {
        if(result == null)
        {
            return false;
        }



        return true;

    }

    public GameObject createresult(float x, float y)
    {
        if(!check())
        {
            return null;
        }

        

        GameObject buf = Instantiate(result, new Vector2(x,y),Quaternion.identity);

        return buf;
    }
}
