using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

// 타이머 관리 컨트롤러
public class TimerController : MonoBehaviour
{
    public float timeRemaining = 60.0f;
    public GameDirector gd;
    public Text timerText;

    /*
     * 매 프레임마다 시간을 감소시켜 타이머를 구현했습니다. 타이머 종료 시, 승자 판정을 수행합니다.
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
