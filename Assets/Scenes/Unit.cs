using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    public const int left = 1, right = 2, up = 3, down = 4;

    public bool is4direc = true;

    public string unittype;


    //state 목록
    public bool isbuilding = false;


    public float deltaspeed = 0;


    public string mainstate = "idle";
    public int mainstatetime = 0;
    public int team = 0, hp = 10, csize = 1;
    public bool moving = true, canaction = true, canmove = true;
    public string direction = "none" , direcname = null; //direction은 외ㅏ부에서 문자열로 방향을 입력할때 쓰고, direcname은 테스트를 위해 현재의 방향을 보여주기 위한 용도
    public int direcint = 0;
    public float speed = 0, sightrange = 2.5f;
    BoxCollider2D cd;
    Vector2 movement;
    Rigidbody2D rb;

    public bool blocked,moved = false; //플래그 스위치??? 뭐라 말해야할지 모르겠다


    public string mrdirecname;
    public float mrdirec;
    public int mrtime = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (direction == null)
        {
            direction = "none";
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }


        if (cd == null)
        {
            cd = gameObject.AddComponent<BoxCollider2D>();
            cd.size = new Vector2(csize, csize);
        }

        Effect e = gameObject.GetComponent<Effect>();
        if (e == null)
        {
            gameObject.AddComponent<Effect>();
        }

        UnitAction a = gameObject.GetComponent<UnitAction>();
        if (a == null)
        {
            gameObject.AddComponent<UnitAction>();
        }

        if (unittype == null)
        {
            unittype = "unit";
        }

        switch (unittype)
        {
            case "Unit":
            case "unit":
                {
                }
                break;

            case "Building":
            case "building":
                {
                }
                break;
        }


        blocked = false;





        //unitpattern
        if (gameObject.GetComponent<unitpattern>() == null)
        {
            gameObject.AddComponent<unitpattern>();
        }


        GameObject systemobj = GameObject.Find("system");
        if (systemobj != null)
        {
            system sys = systemobj.GetComponent<system>();
            if (sys != null)
            {
                sys.addnewunit(this);
            }
        }

    }

    private void OnDestroy()
    {
        GameObject systemobj = GameObject.Find("system");
        if (systemobj != null)
        {
            system sys = systemobj.GetComponent<system>();
            if (sys != null)
            {
                sys.delunit(this);
            }
        }
    }



    private void FixedUpdate()
    {
        proc();
        move();
        effectproc();
        reservactionproc();
    }


    private void proc()
    {
        if(mainstate == "idle") //뭐든 할 수있는 상태
        {
            canaction = true;
        }
        else if(mainstatetime > 0)
        {
            mainstatetime--;
            canaction = false;
        }
        else
        {
            mainstate = "idle";
            canaction = true;
        }


    }



    private void move()
    {
        

        if(speed <= 0 || !canmove)
        {
            return;
        }

        mreqproc();
        if(direction != null)
        {
            direcint = direcnametoint(direction);
            direcname = direction;
            direction = null;
        }

        blocked = false;
        direcname = direcname.ToLower();
        moving = true; //일단 참으로 하고 이동하지 못할 경우 false로
        switch (direcint)
        {
            case left:
                movement.Set(-1, 0);
                direcint = left;
                direcname = "left";
                break;


            case right:
                movement.Set(1, 0);
                direcint = right;
                direcname = "right";
                break;

            case up:
                movement.Set(0, 1);
                direcint = up;
                direcname = "up";
                break;

            case down:
                movement.Set(0, -1);
                direcint = down;
                direcname = "down";
                break;

            default:
                moving = false;
                direcint = 0;
                direcname = "none";
                return;
        }
     
        deltaspeed = speed * Time.fixedDeltaTime;
        movement = movement * deltaspeed;
        rb.MovePosition(rb.position + movement);
        blocked = false;
    }

    private bool mreqproc()
    {
        if(mrdirecname != null)
        {
            direction = mrdirecname;
            mrdirecname = null;
        }
        else if(mrtime > 0)
        {
            mrtime--;
        }
        else
        {
            stopmove();
            return false;                        
        }

        return true;
    }

    public bool movereq(string direcname,int time)
    {
        if(!canaction || speed <= 0 || direcname == null || time <= 0)
        {
            return false;
        }


        mrdirecname = direcname;
        mrtime = time;
        return true;
    }

    public void resetmovereq()
    {
        mrtime = 0;
        mrdirecname = null;
    }

    public void stopmove()
    {
        direction = "none";
        direcint = 0;
    }


    public bool moveonce(float dx, float dy)
    {
        if(!canaction || speed <= 0)
        {
            return false;
        }

        float x = transform.position.x, y = transform.position.y;

        Vector2 movement = new Vector2(dx - x, dy - y).normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        //벽처리? 등등

        return true;
    }

    public bool moveonce(string dname)
    {
        if(dname == null || speed <= 0 || !canaction)
        {
            return false;
        }

        switch(dname)
        {
            case "left":
                {

                }
                break;

            case "right":
                {

                }
                break;

            case "up":
                {
                }    
                break;

            case "down":
                {

                }
                break;

            default: return false;
        }

        return true;
    }

    
    public void effectproc()
    {
        Effect dest = GetComponent<Effect>();
        if (dest == null)
        {
            return;
        }

        Effect.msg[] list = dest.getmsg();
        if(list == null)
        {
            return;
        }
        foreach(Effect.msg e in list)
        {
            switch(e.type)
            {
                case "Damage":
                case "DAMAGE":
                case "damage":
                case "dmg":
                    {
                        hpupdate("minus", e.i[0]);
                    }
                    break;
            }

            
        }
        dest.proc();
    }




    private void hpupdate(string type,int dest)
    {
        if(type == null)
        {
            return;
        }
        switch (type)
        {
            case "plus":
                hp += dest;
                break;

            case "minus":
                hp -= dest;
                break;

            case "set":
                hp = dest;
                break;
        }


        if(hp <= 0)
        {
            death();
        }
    }


    private void death()
    {
        Destroy(gameObject);
    }

    





    private Vector2 getdirec(string dest)
    {
        if(dest == null)
        {
            return new Vector2(0, 0);
        }


        switch (dest)
        {
            case "left" :
                return new Vector2(-1, 0);    

            case "right":
                return new Vector2(1, 0);
            
            case "up":
                return new Vector2(0, 1);

            case "down":
                return new Vector2(0, -1);

            default:
                return new Vector2(0, 0);
                
        }

    }



    public bool entermsg(msg dest)
    {
        if(dest == null)
        {
            return false;
        }
        else if(dest.type == null)
        {
            return false;
        }


        switch(dest.type)
        {
            case "phit": //projectile hit
                {
                    //메시지 체크부터
                    if(dest.exist("bullet","owner"))
                    {
                        break;
                    }

                    GameObject bullet = (GameObject)dest.get("bullet"), owner = (GameObject)dest.get("owner");


                    //투사체 주인, 맞은 방향 등
                    unitpattern up = GetComponent<unitpattern>();
                    if(up != null)
                    {
                        var bi = bullet.GetComponent<projectile>();
                        var oi = owner.GetComponent<Unit>();
                        
                        
                        msg para = new msg();
                        para.add("owner", owner);
                        para.add("allyorenemy", bi.team == oi.team);

                        up.addincident(para);
                    }
                }
                break;



            default:
                return false;

        }



        return true;
    }


    private List<UnitAction.act> ract = new List<UnitAction.act>();

    public bool reserveaction(UnitAction.act dest)
    {
        if(dest == null)
        {
            return false;
        }

        ract.Add(dest);
        return true;
    }

    public bool reserveaction(string type,int time, int[] i, float[] f,string[] str)
    {
        return reserveaction(new UnitAction.act(type, time, i, f, str));
    }

    private void reservactionproc()
    {
        UnitAction action = GetComponent<UnitAction>();


        if(ract == null || action == null)
        {
            return;
        }


        foreach(UnitAction.act a in ract)
        {
            if(a == null)
            {
                continue;
            }
            action.add(a);
        }

        ract.Clear();
    }

    public int direcnametoint(string dest)
    {
        if(dest == null)
        {
            return 0;
        }

        dest = dest.ToLower();
        switch (dest)
        {
            case "left": return left;

            case "right": return right;

            case "up": return up;

            case "down": return down;

            default: return 0;
        }
    }

 
 
    
    public bool block() //이동을 멈추고 위치를 방향에 따라 방향이동 전의 정수좌표로 강제 변경하는걸로 해보겠습니다...
    {
        if(!moving)
        {
            return false;
        }


        int x,y;
        switch (direcint)
        {
            case left:
                x = (int)transform.position.x;
                if(x >= 0)
                {
                    x += 1;
                }
                transform.position.Set(x, transform.position.y,transform.position.z);

                break;
            case right:
                x = (int)transform.position.x;
                if(x < 0)
                {
                    x -= 1;
                }
                transform.position.Set(x, transform.position.y, transform.position.z);
                break;

            case up:
                y = (int)transform.position.y;
                if( y < 0)
                {
                    y -= 1;
                }
                transform.position.Set(transform.position.x, y, transform.position.z);
                break;

            case down:
                y = (int)transform.position.y;
                if(y >=0)
                {
                    y += 1;
                }
                transform.position.Set(transform.position.x,y, transform.position.z);
                break;

        }

   
        direcint = 0;
        blocked = true;
        Debug.Log("unit blocked");
        return true;
    }

}
