using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitAction : MonoBehaviour
{
    public string currentactionname = null;

    const int left = 1, right = 2, up = 3, down = 4;

    private const int actvarmax = 16;
    public class act
    {
        

        public act()
        {
            started = false;
            valid = false;
            i = new int[actvarmax];
            f = new float[actvarmax];
            slist = new List<string>();
        }

        public act(act dest)
        {
            started = false;
            valid = false;
            copy(dest);
        }

        public act(string str, int time, int[] i, float[] f)
        {
            started = false;
            valid = false;
            set(str, time, i, f);
        }

        public act(string name, int time, int[] i, float[] f, string[] str)
        {

            set(name, time, i, f, str);
        }


        public bool valid=false, started=true;
        public string type = "none";
        public List<string> slist = new List<string>();
        public int time = 0;
        public int[] i;
        public float[] f;
        public UnitAction body;
       

        public void execute()
        {
            if(!valid || body == null)
            {
                return;
            }
            Unit info = body.GetComponent<Unit>();
            if(info == null)
            {
                return;
            }
            else if(!info.canaction)
            {
                return;
            }

            bool complete = false;
            switch (type)
            {
                case "move4direc":
                    {
                        if(!started)
                        {
                            if (time <= 0)
                            {
                                complete = true;
                                break;
                            }

                            if (slist.Count < 1)
                            {
                                if(i == null)
                                {
                                    complete = true;
                                    break;
                                }
                                else if (i.Length < 1)
                                {
                                    complete = true;
                                    break;
                                }

                                slist.Add(body.inttodirec(i[0]));
                            }
                        }                       
                        

                        if (info.movereq(slist[0], time))
                        {
                            Debug.Log("move4direc action completed(Unit.movereq is called and returned true)");
                            complete = true;
                        }
                        else
                        {

                        }
                      
                    }
                    break;



                case "movedest":
                    {
                        if(!started)
                        {
                            if(f.Length < 3) // x,y,이동할 거리(실제 목적지까지의 거리를 초과할 수 없음)
                            {
                                complete = true;
                                break;
                            }
                            if(f[2] < 0)
                            {
                                f[0] = -f[0];
                            }
                            
                            f = new float[] { f[0], f[1], f[2], f[0] - body.gameObject.transform.position.x, f[1] - body.gameObject.transform.position.y, info.speed / 50 }; //x,y,dx-x,dy-y,speed, 50번 이동해야 speed*1만큼 이동하는듯
                            f[2] = Mathf.Min(f[2], Mathf.Abs(f[3] + f[4]));
                        }

                        if(f[2] <= f[5])
                        {
                            complete = true;
                            Debug.Log("movedest end(distance completed)");
                            break;
                        }

                        if(info.moving)
                        {
                            complete = false;
                            break;
                        }

                        
                        if (f[5] <= 0)
                        {
                            complete = true;
                            break;
                        }
                        else if (Mathf.Abs(f[3]) < f[5] && Mathf.Abs(f[4]) < f[5])
                        {
                            complete = true;
                            break;
                        }


                        bool ishor = UnityEngine.Random.Range(0, 2) == 0; 
                        if(ishor)
                        {
                            ishor = Mathf.Abs(f[3]) >= f[5];
                        }
                        else
                        {
                            ishor = Mathf.Abs(f[4]) < f[5];
                        }


                        int dbuf,tbuf;
                        if(ishor)
                        {
                            dbuf = f[3] > 0 ? body.stringtointdirec("right") : body.stringtointdirec("left");
                            tbuf = Mathf.Min(Mathf.Abs((int)(f[3] / f[5])), (int)(f[2]/f[5]));
                        }
                        else
                        {
                            dbuf = f[4] > 0 ? body.stringtointdirec("up") : body.stringtointdirec("down");
                            tbuf = Mathf.Min(Mathf.Abs((int)(f[4] / f[5])), (int)(f[2] / f[5]));
                        }

                        int[] ibuf = new int[] { dbuf };
                        body.add("move4direc", tbuf, ibuf, null);
                        f[2] -= tbuf * f[5]; //f[5] is deltaspeed, distance must be decreased by moving distance from f[2]( planned distance).
                        body.pushed = true;

                    }
                    break;

                


                case "moverandom": // movedest를 사용, float로 거리를받아서 그만큼 랜덤한 곳에 떨어진 곳으로 movedest push 후  컴플리트
                    {
                        if(f.Length < 1)
                        {
                            complete = true;
                            break;
                        }

                        float x = body.transform.position.x, y = body.transform.position.y, distance = f[0], speed = info.speed;
                        float mposx, mposy;

                        if(speed <= 0)
                        {
                            complete = true;
                            break;
                        }


                        float angle = UnityEngine.Random.Range(0, Mathf.PI*2);
                        mposx = x + Mathf.Cos(angle) * distance;
                        mposy = y + Mathf.Sin(angle) * distance;

                        body.add("movedest", 1, null, new float[] { mposx, mposy,distance });
                        body.pushed = true;
                        complete = true;
                    }                
                    break;

                case "useweapon":
                    {
                        if(i == null || f == null)
                        {
                            complete = true;
                            break;
                        }
                        else if(i.Length < 1 || f.Length < 2)
                        {
                            complete = true;
                            break;
                        }

                        weapon[] wps = body.GetComponents<weapon>();
                        int index = i[0];
                        if(index < 0 || index >= wps.Length)
                        {
                            complete = true;
                            break;
                        }

                        weapon wp = wps[index];
                        if(wp.able())
                        {
                            wp.start(f[0],f[1]);
                            complete = true;
                        }
                        else
                        {
                            complete = false;
                        }

                    }
                    break;


                case "wait":
                    {
                        if(i == null)
                        {
                            complete = true;
                            break;
                        }
                        if (i[0]-- <= 0)
                        {
                            complete = true;
                        }
                    }
                    break;

                case "stop":
                    {
                        body.reset = true;
                        complete = true;
                    }
                    break;

                case "startunitbuild":
                    {
                        if(i == null)
                        {
                            complete = true;
                            break;
                        }
                        else if(i.Count() < 1)
                        {
                            complete = true;
                            break;
                        }

                        if(!info.canaction)
                        {
                            complete = false;
                            break;
                        }

                        Unitbuilder builder = body.GetComponent<Unitbuilder>();
                        if(builder == null)
                        {
                            complete = true;
                            break;
                        }

                        if(builder.getstarted(i[0]))
                        {
                            complete = true;
                            break;
                        }
                    }
                    break;

                default :
                    complete = true;
                    break;

            }

            if(!started)
            {
                started = true;
            }
            


            if(complete)
            {
                reset();
                return;
            }
        }

        public void copy(act dest)
        {
            if(dest == null)
            {
                return;
            }

            type = new string(dest.type.ToCharArray());
            valid = false;
            if(type != null)
            {
                if(type != "none")
                {
                    valid = true;
                }
            }
            time = dest.time;
            if(dest.i != null)
            {
                i = (int[])dest.i.Clone();
            }
            if(dest.f != null)
            {
                f = (float[])dest.f.Clone();
            }
            

            if(dest.slist != null)
            {
                slist.Clear();
                slist.AddRange(dest.slist.ToArray());
            }
        }


        public void reset()
        {
            type = "none";
            started = false;
            valid = false;
            time = 0;
            i = null;
            f = null;
            slist.Clear();
        }

        public void set(string str,int time,int[] i,float[] f)
        {
            if(str ==null)
            {
                return;
            }

            type = new string(str.ToCharArray());
            this.time = time;
            if(i != null)
            {
                this.i = (int[])i.Clone();
            }
            if(f != null)
            {
                this.f = (float[])f.Clone();
            }

            if(type != "none")
            {
                valid = true;
            }
            started = false;
        }

        public void set(string name,int time, int[] i, float[] f,string[] str)
        {
            set(name, time, i, f);
            slist.Clear();
            addstr(str);
        }


        public bool addstr(string dest)
        {
            if(dest == null)
            {
                return false;
            }

            slist.Add(dest);
            return true;
        }


        public int addstr(string[] dest)
        {
            if(dest == null)
            {
                return 0;
            }

            int r = 0;
            foreach(string s in dest)
            {
                if(addstr(s))
                {
                    r++;
                }
            }

            return r;
        }

        public int addstr(List<string> dest)
        {
            if(dest == null)
            {
                return 0;
            }

            return addstr(dest.ToArray());
        }
         
        
        public bool replacestr(string[] dest)
        {
            if(dest == null)
            {
                return false;
            }

            slist.Clear();
            slist.AddRange(dest);
            return true;
        }

        public bool replacestr(List<string> dest)
        {
            if (dest != null)
            {
                return replacestr(dest.ToArray());
            }

            return false;
        }
    };

    public act cact;
    private act[] actque;
    public int actindex=0, actnum=0;
    private const int actquemax = 16;
    private bool pushed = false, reset = false;

    void Start()
    {
        cact = null;
        actque = new act[actquemax];
        for(int n=0;n<actquemax;n++)
        {
            actque[n] = new act();
            actque[n].body = this;
        }
    }

    private void FixedUpdate()
    {
        proc();
    }


    void proc()
    {
        if (reset)
        {
            clearact();
            reset = false;
        }


        if (pushed)
        {
            loadtocact();
            pushed = false;
            return;
        }

        if (cact == null)
        {
            if (!loadtocact())
            {
                return;
            }
        }
        else if(!cact.valid)
        {
            if(!loadtocact())
            {
                return;
            }
        }

        cact.execute();

        currentactionname = null;
        if(cact != null)
        {
            if (cact.type != null)
            {
                currentactionname = cact.type;
            }
        }
        
    }


    public void add(string type,int time, int[] i, float[] f)
    {
        if(actnum >= actquemax)
        {
            return;
        }
        actquesort();
        actque[actnum++].set(type, time, i, f);
        actindex = actnum-1;
    }



    public void add(string type,int time, int[] i, float[] f,string[] str)
    {
        if (actnum >= actquemax)
        {
            return;
        }
        actquesort();
        actque[actnum].set(type, time, i, f,str);
    }

    public void add(act dest)
    {
        if(dest == null || actnum >= actquemax)
        {
            return;
        }

        actquesort();
        actque[actnum++].copy(dest);
        actindex = actnum - 1;
    }


    private bool loadtocact()
    {
        
        if (actnum <= 0)
        {
            return false;
        }

        cact = actque[actindex];
        actindex = --actnum - 1;
        if(actindex < 0)
        {
            actindex = 0;
        }
        if(!cact.valid)
        {
            cact.reset();
            return false;
        }
        actquesort();
        return true;
    }

    private void actquesort()
    {
        int r = 0;
        for(int n = 0; n<actquemax;n++)
        {
            if(actque[n].valid)
            {
                if(r < n)
                {
                    actque[r].copy(actque[n]);
                }
                r++;
            }            
        }

        actnum = r;
        if(actnum == 0)
        {
            actindex = 0;
        }
        else
        {
            actindex = actnum - 1;
        }

        for(int n=r;n<actquemax;n++)
        {
            actque[n].reset();
        }
    }


    public string inttodirec(int dest)
    {
        
        switch(dest)
        {
            case 1:
                return "left";
            case 2:
                return "right";
            case 3:
                return "up";
            case 4:
                return "down";
            default:
                return "none";
        }
    }

    public int stringtointdirec(string dest)
    {
        if(dest == null)
        {
            return 0;
        }
        switch(dest)
        {
            case "LEFT":
            case "Left":
            case "left":
                return 1;

            case "RIGHT":
            case "Right":
            case "right":
                return 2;

            case "UP":
            case "Up":
            case "up":
                return 3;

            case "DOWN":
            case "Down":
            case "down":
                return 4;

            default:
                return 0;
        }
    }


    private void clearact()
    {
        foreach(act a in actque)
        {
            if(a == null)
            {
                continue;
            }

            a.reset();
        }
        cact = null;
    }
}
