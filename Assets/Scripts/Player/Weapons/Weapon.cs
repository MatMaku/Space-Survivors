using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponType weaponType;

    [Header("Control de activaci�n")]
    [SerializeField]
    protected bool isActive = false;

    [Header("Par�metros generales")]
    public float fireRate = 1f;
    protected float nextFireTime = 0f;
    public float damage = 1f;
    public int level = 1;
    public int maxLevel = 5;

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

    public bool GetActive()
    {
        return isActive;
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

    internal void LevelUp()
    {
        level++;
    }
}
