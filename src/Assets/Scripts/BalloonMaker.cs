using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class BalloonMaker : MonoBehaviour

{
    // User Inputs
    [SerializeField]
    private float dispersionHeight = .25f;
    [SerializeField]
    private float dispersionRadiusMin = 0.4f;
    [SerializeField]
    private float dispersionRadiusMax = 1f;

    [SerializeField]
    private HeadLockedUI headLockedUI = null;

    [SerializeField]
    private GameObject UI = null;

    [SerializeField]
    private GameObject BalloonPrefab = null;

    [SerializeField]
    private int InfiniteModeConstantBalloonCount = 10;

    private int NumberOfBalloons;

    [SerializeField]
    private float delay = 0.5f;

    [SerializeField]
    private AudioSource m_endGameSound = null;

    private bool m_inGame = false;
    private bool m_finiteNumberOfBalloons;
    private int m_numberOfBallonsPoppedSoFar;

    private List<Color> m_gameColors = new List<Color>()
    {
        Color.blue,
        Color.cyan,
        Color.green,
        Color.magenta,
        Color.red,
        Color.white,
        Color.yellow,
    };

    // Start is called before the first frame update
    void Start()
    {
        HideHeadsUpUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inGame)
        {
            if (m_finiteNumberOfBalloons && m_numberOfBallonsPoppedSoFar == NumberOfBalloons)
            {
                OnGameEnd();
            }
        }
    }

    public void StartNewGame(int balloonCount)
    {
        m_inGame = true;
        m_finiteNumberOfBalloons = balloonCount > 0;

        if (m_finiteNumberOfBalloons)
        {
            SpawnFiniteNumberOfBallons(balloonCount);
            headLockedUI.SetballoonPopCount(NumberOfBalloons);
        }
        else
        {
            StartInfiniteMode();
        }

        ShowUI(false);
        headLockedUI.gameObject.SetActive(true);
        headLockedUI.ResetTimer();
        headLockedUI.StartTimer();
    }

    private void ShowUI(bool doShow)
    {
        UI.SetActive(doShow);
        UI.transform.position = CameraCache.Main.transform.position + 1f * CameraCache.Main.transform.forward;
        var directionToTarget = CameraCache.Main.transform.position - UI.transform.position;
        UI.transform.rotation = Quaternion.LookRotation(-directionToTarget, CameraCache.Main.transform.up);
        var euler = UI.transform.eulerAngles;
        UI.transform.eulerAngles = new Vector3(0, euler.y, 0);
    }

    private void ShowUI()
    {
        ShowUI(true);
    }

    private void MakeABalloon()
    {
        var newBalloon = Instantiate(BalloonPrefab);
        newBalloon.transform.parent = transform;

        var color = m_gameColors[Random.Range(0, m_gameColors.Count)];
        var meshes = newBalloon.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshes)
        {
            mesh.material.color = color;
        }

        var rangeXZ = dispersionRadiusMin;
        var rangeY = dispersionHeight;
        var theta = Random.Range(0f, 2f * Mathf.PI);
        var r = Random.Range(dispersionRadiusMin, dispersionRadiusMax);
        newBalloon.transform.position = CameraCache.Main.transform.position + new Vector3(r * Mathf.Sin(theta), Random.Range(-rangeY, rangeY), r * Mathf.Cos(theta));

        var floater = newBalloon.GetComponent<Floater>();
        floater.HasPopped += OnPop;
    }

    private void OnPop(object sender, EventArgs e)
    {
        var floater = sender as Floater;
        if (floater != null)
        {
            Interlocked.Increment(ref m_numberOfBallonsPoppedSoFar);

            Destroy(floater.gameObject);
            if (m_finiteNumberOfBalloons)
            {
                headLockedUI.SetballoonPopCount(NumberOfBalloons - m_numberOfBallonsPoppedSoFar);
            }
            else
            {
                headLockedUI.SetballoonPopCount(m_numberOfBallonsPoppedSoFar);
                MakeABalloon();
            }
        }
    }

    private void OnGameEnd()
    {
        Invoke(nameof(ShowUI), 3f);
        Invoke(nameof(HideHeadsUpUI), 3f);
        m_endGameSound.Play();
        headLockedUI.StopTimer();
        m_inGame = false;
    }

    private void HideHeadsUpUI()
    {
        headLockedUI.gameObject.SetActive(false);
    }

    private void StartInfiniteMode()
    {
        SpawnFiniteNumberOfBallons(InfiniteModeConstantBalloonCount);
        headLockedUI.SetballoonPopCount(0);
    }

    private void SpawnFiniteNumberOfBallons(int balloonCount)
    {
        NumberOfBalloons = balloonCount;
        for (var i = 0; i < NumberOfBalloons; ++i)
        {
            Invoke(nameof(MakeABalloon), i * delay);
        }
    }
}
