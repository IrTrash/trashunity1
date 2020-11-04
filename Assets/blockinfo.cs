using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockinfo : MonoBehaviour
{


    public int width, height;
    bool[][] isblocked;

    public bool active, walladded = false;



    private void Start()
    {
        if(!active)
        {
            return;
        }
        if(width > 0 && height > 0)
        {
            isblocked = new bool[width][];
            for(int n=0;n<width;n++)
            {
                isblocked[n] = new bool[height];
                for(int m=0;m<height;m++)
                {
                    isblocked[n][m] = false;
                }
            }
        }

        updateblockinfo();
        
        
    }

    private void updateblockinfo()
    {
        if(isblocked == null)
        {
            return;
        }

        foreach(bool[] raw in isblocked)
        {
            for(int n=0;n < raw.Length;n++)
            {
                raw[n] = false;
            }
        }

        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Wall");
        if(blocks != null)
        {
            foreach(GameObject block in blocks)
            {
                float x = block.transform.position.x, y = block.transform.position.y;
                int ix = (x - (int)x > 0.5) ? (int)x + 1 : (int)x,
                    iy = (y - (int)y > 0.5) ? (int)y + 1 : (int)y;
                if(ix >= 0 && ix < width && iy >= 0 && iy < height)
                {
                    isblocked[ix][iy] = true;
                }
                
            }
        }
    }

    public bool blocked(int x, int y)
    {
        if(x < 0 || x >= width || y < 0 || y > height)
        {
            return true;
        }

        return isblocked[x][y];
    }

    private void FixedUpdate()
    {
        proc();
    }


    private void proc()
    {
        if(!active)
        {
            return;
        }

        List<GameObject> units = new List<GameObject>(GameObject.FindGameObjectsWithTag("Unit"));

        float x, y;
        int ix, iy;
        foreach(GameObject unit in units)
        {
            if(unit == null)
            {
                continue;
            }

            x = unit.transform.position.x;
            y = unit.transform.position.y;
            ix = getgridx(x);
            iy = getgridy(y);
            if (ix < 0 || iy < 0 || ix >= width || iy >= height)
            {
                continue;
            }

            Unit unitinfo = unit.GetComponent<Unit>();
            if(unitinfo == null)
            {
                continue;
            }

            unitinfo.blocked = isblocked[ix][iy];          
            
        }

        if(walladded)
        {
            updateblockinfo();
            walladded = false;
        }
    }


    public int getgridx(float x)
    {
        if(x >= 0)
        {
            return x - (int)x > 0.5 ? (int)x + 1 : (int)x;
        }

        return x - (int)x < -0.5 ? (int)x - 1 : (int)x;
    }

    public int getgridy(float y)
    {
        if(y >= 0)
        {
            return y - (int)y > 0.5 ? (int)y + 1 : (int)y;
        }

        return y - (int)y < -0.5 ? (int)y - 1 : (int)y;
    }
}
