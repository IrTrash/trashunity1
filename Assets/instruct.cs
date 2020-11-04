using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;

public class instruct 
{
    public bool valid = true;
    public string type = "none";
    public int remaintime = 1;
    public Dictionary<string, object> msg;

    

    public instruct()
    {
        init();
    }


    public instruct(params object[] dest)
    {
        init();

        if(dest == null)
        {
            return;
        }

        if(dest.Length < 2 || dest.Length%2 == 1)
        {
            return;
        }


        int mode = 0;
        string ctype = null;
        foreach(object o in dest)
        {
            switch(mode)
            {
                case 0: //type input
                    if(o.GetType() != typeof(string))
                    {
                        ctype = (string)o;
                    }
                    break;

                case 1: //var input
                    {
                        if(ctype != null)
                        {
                            add(ctype, o);
                            ctype = null;
                        }
                    }
                    break;
            }
        }
    }



    class v
    {
        public string type;
        public object var;
    };


    private List<v> vlist = new List<v>();

    public List<KeyValuePair<string,object>> addlist = new List<KeyValuePair<string, object>>();

    private void init()
    {
        msg = new Dictionary<string, object>();
    }

    private void disablethis()
    {
        valid = false;
    }

    public bool add(string key,object value)
    {
        if(key == null)
        {
            return false;
        }

        if(vlist == null)
        {

        }




        return true;
    }


    public void proc()
    {
        if(!valid)
        {
            return;
        }

        if(execute())
        {
            if(--remaintime <= 0)
            {
                disablethis();
            }
        }
    }

    public bool execute()
    {
        addproc();


        if(type == null)
        {
            disablethis();
            return false;
        }

        switch(type)
        {
            case "setdata":
                break;


            default:
                disablethis();
                return false;
        }


        return true;
    }



    private void addproc()
    {
        if(addlist == null)
        {
            return;
        }


       foreach(KeyValuePair<string,object> p in addlist)
        {
            add(p.Key, p.Value);
            addlist.Remove(p);
        }

    }
}
