using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public int hp = 2, sightrange = 1;
    // Start is called before the first frame update
    
    public void affecthp(int dest)
    {
        hp += dest;
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
