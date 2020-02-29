using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public float speed = 300f;
    public float NextWaypointDist=1f;

    Path path;
    int currentWaypoint = 0;
    //bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0, 0.5f);
    }

    void UpdatePath()
    {
        if(seeker.IsDone() && target!=null)
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))//Should set closest player as target
        {
            if (go.GetComponent<PlayerHPXP>().IsAlive())
            {
                float temp = Vector3.Distance(go.transform.position, transform.position);
                if (!target)
                {
                    target = go.transform;
                }
                else if (temp < Vector3.Distance(target.transform.position, transform.position))
                {
                    target = go.transform;
                }
            }
        }

        if (path == null)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count)
        {
           // reachedEndOfPath = true;
            return;
        }
        else
        {
           // reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < NextWaypointDist)
        {
            currentWaypoint++;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            transform.position = Vector2.MoveTowards(transform.position, collision.transform.position, -1 * 50 * Time.deltaTime);
        }
    }

    public void SetParent(Transform p)
    {
        this.transform.SetParent(p);
    }
}
