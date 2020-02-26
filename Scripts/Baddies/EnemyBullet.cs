using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed = 10, damage;
    private Transform parentPlayer;


    void Start()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Baddie"))//Should set closest Baddie as owner
        {
            float temp = Vector2.Distance(go.transform.position, transform.position);
            if (!parentPlayer)
            {
                parentPlayer = go.transform;
            }
            else if (temp < Vector2.Distance(parentPlayer.transform.position, transform.position))
            {
                parentPlayer = go.transform;
            }
        }
        this.transform.SetParent(parentPlayer);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!this.transform.parent)
        {
            StartCoroutine(IsTriggerFalse(collision));
        }
        else
        {
            if (!collision.transform.IsChildOf(this.transform.parent))
                Destroy(this.gameObject);
        }
    }

    private IEnumerator IsTriggerFalse(Collision2D collision)
    {
        yield return new WaitForSeconds(.3f);
        if (!collision.transform.IsChildOf(this.transform.parent))
            Destroy(this.gameObject);
    }
    public void SetDamage(float d)
    {
        damage = d;
    }
    public float GetDamage()
    {
        return damage;
    }
}
