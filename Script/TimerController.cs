using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

// Ÿ�̸� ���� ��Ʈ�ѷ�
public class TimerController : MonoBehaviour
{
    public float timeRemaining = 60.0f;
    public GameDirector gd;
    public Text timerText;

    /*
     * �� �����Ӹ��� �ð��� ���ҽ��� Ÿ�̸Ӹ� �����߽��ϴ�. Ÿ�̸� ���� ��, ���� ������ �����մϴ�.
     */
    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            float seconds = Mathf.Floor(timeRemaining);
            float milliseconds = Mathf.Floor((timeRemaining - seconds) * 100);
            timerText.text = string.Format("{0:00}.{1:00}", seconds, milliseconds);
        }
        else
        {
            timerText.text = string.Format("{0:00}.{1:00}", 0.0f, 0.0f);
            enabled = false;
            gd.ShowWinner();              
        }       
    }
}
