using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private float puntForce = 100;

    [SerializeField]
    private int damage = 1;

    [SerializeField]
    private float noticeDistance = 5;

    [SerializeField]
    private FloatRange waitRange = new FloatRange(0.75f, 2.0f);

    [SerializeField]
    private FloatRange pickNewLocationRange = new FloatRange(5.0f, 10.0f);

    private Vector2 push = new Vector2();
    private float pickNewLocationTime = 0;

    private GameObject player;
    private AIPath ai;
    private AIDestinationSetter destinationSetter;

    private Bounds bounds = new Bounds(Vector3.zero, Vector2.one * 30);

    void Start()
    {
        player = GameObject.Find("Player");
        ai = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
    }

    void Update()
    {
        if(destinationSetter.target == null)
        {
            if(Vector2.Distance(transform.position, player.transform.position) < noticeDistance)
            {
                destinationSetter.target = player.transform;
            }
            else if(pickNewLocationTime <= Time.time)
            {
                // Wait for a sec, then pick a new location to move to
                StartCoroutine(WaitForStandingStillDelay());
                pickNewLocationTime = pickNewLocationRange.Random;
            }
        }
    }

    IEnumerator WaitForStandingStillDelay()
    {
        yield return new WaitForSeconds(waitRange.Random);

        if (destinationSetter.target == null)
        {
            GraphNode graphNode;
            GridGraph graph = AstarPath.active.graphs[0] as GridGraph;
            bounds.center = transform.position;

            List<GraphNode> nodes = graph.GetNodesInRegion(bounds).FindAll(node => node.Walkable);
            if (nodes.Count != 0)
            {
                int index = Random.Range(0, nodes.Count - 1);
                ai.destination = (Vector3)nodes[index].position;
            }
        }
    }

    void FixedUpdate()
    {
        ai.Move(push * Time.fixedDeltaTime);
        float length = push.magnitude;
        length -= length * 0.6f * Time.fixedDeltaTime;
        push = push.normalized * length;
    }

    public void Punt(Vector2 velocity)
    {
        push += velocity;
        Debug.Log(push);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject == player)
        {
            collider.GetComponent<PlayerMovement>().Punt((collider.transform.position - transform.position).normalized * puntForce);
            collider.gameObject.SendMessage("OnTakeDamage", new DamageInfo(gameObject, damage));
        }
    }
}