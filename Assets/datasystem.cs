using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class datasystem
{
    private List<data> list = new List<data>();
    private int count = 0;


    public GameObject sys;


    public datasystem(GameObject dest) 
    {
        sys = dest;
    }




    public data getonce()
    {
        if(list == null)
        {
            return null;
        }
        else if(list.Count == 0)
        {
            return null;
        }

        if(list.Count <= count)
        {
            count = 0;
        }


        return list[count++];
    }




}
