using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeadLockedUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro m_timer = null;
    private float m_backingTimer;
    private bool m_timerTicking = false;

    [SerializeField]
    private TextMeshPro m_balloonCounter = null;

    // Start is called before the first frame update
    void Start()
    {
        m_backingTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timerTicking)
        {
            m_backingTimer += Time.unscaledDeltaTime;
            var mins = (int)m_backingTimer / 60;
            var seconds = (int)(m_backingTimer - 60 * mins);

            m_timer.text = $"{mins}:" + (seconds < 10 ? $"0{seconds}" : $"{seconds}");
        }
    }

    public void ResetTimer()
    {
        m_backingTimer = 0f;
    }

    public void StartTimer()
    {
        m_timerTicking = true;
    }

    public void StopTimer()
    {
        m_timerTicking = false;
    }

    public void SetballoonPopCount(int count)
    {
        m_balloonCounter.text = count.ToString();
    }

}
