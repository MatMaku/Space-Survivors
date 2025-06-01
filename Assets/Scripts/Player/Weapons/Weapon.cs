using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponType weaponType;

    [Header("Control de activación")]
    [SerializeField]
    protected bool isActive = false;

    [Header("Parámetros generales")]
    public float fireRate = 1f;
    protected float nextFireTime = 0f;
    public float damage = 1f;

    protected GameObject owner;
    protected MeshRenderer[] meshRenderers;

    public virtual void Initialize(GameObject owner)
    {
        this.owner = owner;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        UpdateVisualState();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        UpdateVisualState();
        
    }

    protected virtual void UpdateVisualState()
    {
        if (meshRenderers == null) return;

        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = isActive;
        }
    }

    
    public abstract void UpdateWeapon();
}
