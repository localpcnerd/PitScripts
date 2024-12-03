using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPitTarget : MonoBehaviour
#if H3VR_IMPORTED
        , IFVRDamageable
#endif 
{
    public bool isHit;
    public AudioSource source;
    public bool isFriendly;

#if H3VR_IMPORTED
    // Event for when we are damaged by something in the game
    void IFVRDamageable.Damage(Damage dam)
    {
        // If damage was non-projectile, ignore.
        if (dam.Class != Damage.DamageClass.Projectile)
        {
            return;
        }

        isHit = true;
        source.Play();
    }
#endif

    public void DeleteTarget()
    {
        //UnityEngine.Object.Destroy(base.gameObject, 3f);
    }

    public void Update()
    {
        if (isHit)
        {
            Vector3 vec = new Vector3(180, transform.localEulerAngles.y, transform.localEulerAngles.z);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, vec, 5 * Time.deltaTime);
            UnityEngine.Object.Destroy(base.gameObject, 0.2f);
        }
    }
}
