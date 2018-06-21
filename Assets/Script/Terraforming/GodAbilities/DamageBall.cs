using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DigitalRuby.LightningBolt;
using System;

public class DamageBall : GodAbility
{
    public int DMG;
    public float GodHeight;
    public int InitialDelay;
    public int Duration;
    public UnityEvent ThunderStart;
    public UnityEvent ThunderEnd;
    public AudioClip ThunderSound;

    private LightningBoltScript lightningBolt;
    private Transform boldStart;
    private Transform boldEnd;
    private AudioSource sound;
    private Queue<AudioSource> sources;

    public override void SetUp()
    {
        lightningBolt = GetComponentInChildren<LightningBoltScript>();
        Transform[] boldPoints = lightningBolt.GetComponentsInChildren<Transform>();
        boldStart = boldPoints[0];
        boldEnd = boldPoints[1];
        sources = new Queue<AudioSource>();
        ThunderStart.AddListener(StartEventCorutines);
    }

    public override void UseAbility(Vector3 Impact)
    {
        StartCoroutine(DoThunder(Impact));
    }

    private IEnumerator DoThunder(Vector3 Impact)
    {
        yield return new WaitForSeconds(InitialDelay);
        ThunderStart.Invoke();
        boldStart.position = new Vector3(Impact.x, Impact.y + GodHeight, Impact.z);
        boldEnd.position = Impact;

        lightningBolt.ManualMode = false;

        Collider[] colls = Physics.OverlapSphere(Impact, Radius);
        foreach (Collider col in colls)
        {
            Health h = col.GetComponent<Health>();
            if (h != null)
            {
                h.AddDamage(DMG);
            }
        }

        StartCoroutine(EndThunder());
    }

    private IEnumerator EndThunder()
    {
        yield return new WaitForSeconds(Duration);
        ThunderEnd.Invoke();
        lightningBolt.ManualMode = true;
    }

    private void StartEventCorutines()
    {
        StartCoroutine(CreateSound());
    }

    private IEnumerator CreateSound()
    {
        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = ThunderSound;
        source.PlayDelayed(0.5f);
        sources.Enqueue(source);
        yield return new WaitForSeconds(ThunderSound.length);
        Destroy(sources.Dequeue());
    }
}