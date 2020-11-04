using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public Effect effect;

    public bool hostile = true;
    public int team = 0,life=60,boundlimit=1,bounddelay=10;
    public GameObject owner;
    private Vector2 movement;
    private Rigidbody2D rb;
    public float hitsize = 0.5f, speed = 1, direction = 0;





    private class boundlist
    {
        public GameObject obj;
        public int time;

       
        public boundlist(GameObject o, int t)
        {
            obj = o;
            time = t;
        }


        public bool proc()
        {
            if(obj != null)
            {
                if(time <= 0)
                {
                    return false;
                }
                else
                {
                    time--;
                    return true;
                }
            }
            return false;
        }

        public bool check(GameObject dest)
        {
            if(dest == null || obj == null)
            {
                return false;
            }

            if (obj.GetInstanceID() == dest.GetInstanceID())
            {
                return true;
            }
            return false;
        }
    };

    List<boundlist> blist = new List<boundlist>();

    void Start()
    {
        if(owner != null)
        {
            team = owner.GetComponent<Unit>().team;
        }   
        
        if(effect == null)
        {
            effect = GetComponent<Effect>();
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.rotation = direction * Mathf.Rad2Deg - 90f;
        }
        else
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }


    }

    
    private void FixedUpdate()
    {
        
        proc();
        move();
        hitcheck();
    }


    public void setdest(float dx, float dy)
    {
        direction = Mathf.Atan2(dy - transform.position.y, dx - transform.position.x);
        movement.Set(Mathf.Cos(direction), Mathf.Sin(direction));
        movement = movement * speed;
        if (rb != null)
        {
            rb.rotation = direction * Mathf.Rad2Deg - 90;
        }
    }

    private void proc()
    {
        if (life <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            life--;
        }


        List<boundlist> removelist = new List<boundlist>();
        foreach(boundlist b in blist)
        {
            if(!b.proc())
            {
                removelist.Add(b);
                continue;
            }
        }

        foreach(boundlist b in removelist)
        {
            blist.Remove(b);
        }
    }


    private void move()
    {
        if(speed == 0)
        {
            return;
        }
        if(rb != null)
        {
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }
        
    }

    private void hitcheck()
    {
        bool bounded;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] obj = Physics2D.OverlapCircleAll(pos, hitsize);
        foreach(Collider2D ob in obj)
        {
            

            bounded = false;
            var o = ob.gameObject;
            if(o == owner)
            {
                continue;
            }


            bool already = false;
            foreach(boundlist b in blist)
            {
                already = b.check(o);
                if(already)
                {
                    break;
                }
            }
            if(already)
            {
                continue;
            }

            switch(o.tag)
            {
                case "Unit":
                    {
                        Unit ui = o.GetComponent<Unit>();
                        if (ui != null)
                        {
                            bool e = false;
                            if (hostile && team != ui.team)
                            {
                                e = true;
                            }
                            else if (!hostile && team == ui.team)
                            {
                                e = true;
                            }

                            if (e)
                            {
                               
                                if (effect != null)
                                {
                                    
                                    Effect ue = o.GetComponent<Effect>();
                                    if(ue != null)
                                    {
                                        ue.load(effect.getmsg());
                                    }
                                }
                                bounded = true;                                                      
                            }

                        }
                    }
                    break;          
            }

            if(bounded)
            {
                if (--boundlimit <= 0)
                {
                    Destroy(gameObject);
                    return;
                }

                blist.Add(new boundlist(o, bounddelay));
            }
        }

    }

    public void setposition(float x, float y)
    {
        if(transform == null || rb == null)
        {
            return;
        }

        rb.position.Set(x, y);
    }



    
}

