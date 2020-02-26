using UnityEngine;

public class FireBullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;
    public Transform parentPlayer;
    private float damageDealt;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = (transform.parent.GetComponent<Rigidbody2D>().velocity) / 4;
        Destroy(this.gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.IsChildOf(this.transform.parent)) //If not siblings
        {
            Destroy(this.gameObject);
        } 
        if (collision.gameObject.tag == "Baddie" || collision.gameObject.tag == "Baddie1")
        {
            collision.gameObject.GetComponent<BaddieHealth>().TakeDamage(damageDealt);
            if (transform.parent && collision.gameObject.GetComponent<BaddieHealth>().getHP() <= 0) {
                transform.parent.GetComponent<PlayerHPXP>().AddExp(collision.gameObject.GetComponent<BaddieHealth>().GetExp());
            }
        }
    }

    public void SetDamageDAndPlayer(float damage, Transform player)
    {
        damageDealt = damage;
        this.transform.SetParent(player);
    }
}
