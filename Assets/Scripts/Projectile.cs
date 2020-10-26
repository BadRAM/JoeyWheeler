using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15;
    [SerializeField] private float colliderRadius = 0.1f;
    [SerializeField] private string[] collisionLayerMask = new []{"Default"};
    private int _layerMask;
    private int _layerMaskTerrain;
    [SerializeField] private float damage;
    [SerializeField] private float splashRadius;
    [SerializeField] private float blastStrength;
    private Rigidbody _rigidbody;
    [SerializeField] private float lifetime; // how long to fly before destroying self.
    [SerializeField] private float persistence; // how long to wait after hit before destroying self.
    [SerializeField] private GameObject enableOnDeath;
    [SerializeField] private GameObject disableOnDeath;
    private bool _alive = true;
    private Vector3 _lastpos;
    private float _startTime; // is used for timers. updates to time of death on death of projectile, for persistence timing.

    
    void Start()
    {
        _lastpos = transform.position;
        _layerMask = LayerMask.GetMask(collisionLayerMask);
        _layerMaskTerrain = LayerMask.GetMask("Terrain");
        _startTime = Time.time;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        if (_alive)
        {
            // check for collisions
            Vector3 dir = transform.forward;
            RaycastHit hit;
            float dist = Vector3.Distance(transform.position, _lastpos);

            if (Physics.SphereCast(_lastpos, colliderRadius, dir, out hit, dist, _layerMask))
            {
                // move the transform to the appropriate collision position
                transform.position = hit.point + transform.forward * -colliderRadius;
                Collide(hit);
            }
            _lastpos = transform.position;
            
            
            // destroy self if at end of lifetime.
            if (Time.time - _startTime > lifetime)
            {
                Collide();
            }
        }
        else
        {
            if (Time.time > _startTime + persistence)
            {
                Destroy(gameObject);
            }
        }
    }
    
    private void Collide (RaycastHit collision)
    {
        bool playerHit = false;
        if (collision.transform.CompareTag("Enemy"))
        {
            collision.transform.parent.GetComponent<Enemy>().Hurt(damage);
        }
        else if (collision.transform.CompareTag("Player"))
        {
            collision.transform.parent.GetComponent<PlayerController>().Hurt(damage);
            playerHit = true;
        }
        
        if (splashRadius > 0)
        {
            Explode(collision.transform.GetComponent<Enemy>(), playerHit);
        }

        _alive = false;
        _startTime = Time.time;
        disableOnDeath.SetActive(false);
        enableOnDeath.SetActive(true);
        //TrailParticles.Stop();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    
    private void Collide()
    {
        Explode(null, false);
        
        _alive = false;
        _startTime = Time.time;
        disableOnDeath.SetActive(false);
        enableOnDeath.SetActive(true);
        //TrailParticles.Stop();
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void Explode(Enemy excludeEnemy, bool excludePlayer)
    {
        // check line of sight to player
        if (!Physics.Linecast(transform.position, GameInfo.Player.GetCenter(), _layerMaskTerrain))
        {
            // damage the player if the player did not take damage from a direct hit.
            if (!excludePlayer)
            {
                GameInfo.Player.Hurt(damageFalloff(GameInfo.Player.GetCenter()));
            }
            // push the player either way.
            _addExplosionForce(GameInfo.Player.GetComponent<Rigidbody>());
        }


        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy iEnemy = i.GetComponent<Enemy>();
            if (iEnemy == null ||
                iEnemy == excludeEnemy ||
                !i.GetComponent<Enemy>().GetAlive()) 
            {
                continue;
            }
            Vector3 ipos = i.GetComponent<Rigidbody>().centerOfMass + i.transform.position;
            float idist = Vector3.Distance(transform.position, ipos);
            if (idist < splashRadius &&
                !Physics.Linecast(transform.position, ipos, _layerMaskTerrain))
            {
                i.GetComponent<Enemy>().Hurt(damageFalloff(ipos));
                _addExplosionForce(i.GetComponent<Rigidbody>());
            }
        }
    }

    private float damageFalloff(Vector3 targetpos)
    {
        return Mathf.Max(0, damage * (-(Vector3.Distance(transform.position, targetpos) / splashRadius) + 1));
    }
    
    private void _addExplosionForce(Rigidbody target)
    {
        Vector3 delta = (target.centerOfMass + target.transform.position) - transform.position;
        Vector3 force = delta.normalized * blastStrength * (Mathf.Max(0, splashRadius - delta.magnitude) / splashRadius);
        target.AddForce(force, ForceMode.VelocityChange);
        Debug.Log("added force of " + force.magnitude + " from delta of " + delta.magnitude + " and distance factor of " + Mathf.Max(0, splashRadius - delta.magnitude) );
    }
}
