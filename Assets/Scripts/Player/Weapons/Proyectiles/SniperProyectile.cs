using UnityEngine;

public class SniperProyectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    private int level = 1;
    private SniperWeapon ownerWeapon;

    private int enemiesPierced = 0;
    private bool firstEnemyKilled = false;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void Start()
    {
        Destroy(gameObject, 10);
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void SetOwner(SniperWeapon weapon)
    {
        ownerWeapon = weapon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        ControladorEnemigos enemigo = other.GetComponent<ControladorEnemigos>();
        if (enemigo == null) return;

        float damage = CalculateDamage();

        enemigo.recibirDaño(damage);

        bool wasKilled = enemigo.vidaActual < 0;

        if (!firstEnemyKilled && wasKilled)
        {
            firstEnemyKilled = true;
            ownerWeapon.FirstKill();
        }

        enemiesPierced++;
    }

    private float CalculateDamage()
    {
        float damage = this.damage;

        // Nivel 2+: +10%, 20%, ..., hasta 50% (niveles 2 al 5)
        if (level >= 2)
        {
            int bonusLevels = Mathf.Clamp(level - 1, 1, 4);
            damage *= 1f + bonusLevels * 0.1f;
        }

        // Nivel 3: +50% al primer enemigo
        if (level >= 3 && enemiesPierced == 0)
        {
            damage *= 1.5f;
        }

        // Nivel 1-3: pierde 33% de daño por cada enemigo atravesado
        if (level <= 3 && enemiesPierced > 0)
        {
            damage *= Mathf.Pow(0.67f, enemiesPierced);
        }

        // Nivel 4+: no pierde daño al atravesar
        // Nivel 5: se maneja desde el arma con FirstKill()

        return damage;
    }
}
