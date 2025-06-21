using UnityEngine;

public class DoubleShotProyectile : Projectile
{
    public float speed = 10f;      
    public bool pierce = false;    

    private void Start()
    {
        Destroy(gameObject,10);
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Enemy"))
            if(!pierce)
                Destroy(gameObject);
    }
}
