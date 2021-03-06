﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    GameObject owner;
    int RecentTeam = 0;
    Rigidbody Body;
    public AudioClip Bounce;
    AudioSource Source;
    public int TouchCount = 0;

    public GameObject Owner
    {
        get
        {
            return owner;
        }

        set
        {
            TouchCount = 0;
            var prevOwner = owner;

            owner = value;

            if (owner != null && GameManager.Instance.Phase == GamePhase.OutlinePass)
            {
                GameManager.Instance.Phase = GamePhase.InGame;
            }

            if (prevOwner == null ||
               (owner != null && prevOwner != null 
               && owner.GetComponent<Player>().Team != prevOwner.GetComponent<Player>().Team))
            {
                GameManager.Instance.TimeOut = 30.0f;
                PlayerManager.InitAutoMove();
            }
            else
            {
                prevOwner.GetComponent<Player>().ChangeToAutoMove();
                RecentTeam = prevOwner.GetComponent<Player>().Team;
            }
        }
    }

    public float XCut;
    public float ZCut;

    public int Score = 2;

    public bool BallGettingEnd;

    void Start()
    {
        Body = GetComponent<Rigidbody>();
        Source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Phase != GamePhase.InGame)
            return;

        bool isOut = false;
        var outlinePos = new Vector3();

        if (transform.position.x <= -XCut)
        {
            isOut = true;
            outlinePos.x = -XCut - 0.1f;
            outlinePos.z = transform.position.z;
        }
        else if (transform.position.x >= XCut)
        {
            isOut = true;
            outlinePos.x = XCut + 0.1f;
            outlinePos.z = transform.position.z;
        }
        else if (transform.position.z <= -ZCut)
        {
            isOut = true;
            outlinePos.x = transform.position.x;
            outlinePos.z = -ZCut - 0.1f;
        }
        else if (transform.position.z >= ZCut)
        {
            isOut = true;
            outlinePos.x = transform.position.z;
            outlinePos.z = ZCut + 0.1f;
        }

        if (isOut)
        {
            GameManager.Instance.Phase = GamePhase.Wait;
            GameManager.Instance.OutlinePass((RecentTeam + 1) % 2, outlinePos);
        }

        if (transform.position.y > 0.0f)
        {
            if (Body.velocity.magnitude == 0.0f)
            {
                Body.AddForce(0.0f, 0.0f, 1.0f);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!BallGettingEnd)
        {
            BallGettingEnd = true;
            GameManager.Instance.Phase = GamePhase.InGame;
        }

        if (collision.gameObject.tag != Tags.Player)
        {
            TouchCount++;
            PlayBounceSound();

            if (GameManager.Instance.Phase == GamePhase.OutlinePass)
            {
                GameManager.Instance.Phase = GamePhase.InGame;
            }
        }
    }

    public void PlayBounceSound()
    {
        Source.PlayOneShot(Bounce);
    }
}
