using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitscan : Weapon
{
    [SerializeField] private Transform RayCastOrigin;
    [SerializeField] private float Damage;   // how much damage a raycast will do

    protected override void Fire()
    {
        Vector3 hitpoint = HitScan(Damage, RayCastOrigin.position, RandomSpread(RayCastOrigin.forward, Spread), 10000);
        GameObject beam = Instantiate(Projectile, ProjectileSpawn.position, ProjectileSpawn.rotation);
        beam.GetComponent<Beam>().endPoint = hitpoint;
        beam.GetComponent<Beam>().startPoint = ProjectileSpawn.position;
    }
    
    protected Vector3 HitScan(float damage, Vector3 origin, Vector3 direction, float length)
    {
        LayerMask layerMask = LayerMask.GetMask("HitBox");
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, length))
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponentInParent<Player>().Hurt(Damage);
            }
            
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponentInParent<Enemy>().Hurt(Damage);
            }
            return hit.point;
        }
        return origin + direction * length;
    }
}
