using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 전반적인 게임을 관리하는 클래스
public class GameDirector : MonoBehaviour
{
    private int currentPlayerNumber, currentEnemyNumber; 
    private int n = 5, m = 10;
    private int maxPlayerNumber = 3;
    private float lastUpdateTime = 0.0f;
    private bool[,] isCharacterPosition; // 보드판에서 각각의 보드칸에 캐릭터가 위치하고 있는지 여부를 나타내는 배열
    public GameObject messageBox;
    public GameObject autoPlayingText;
    public GameObject timerText;
    public GameObject playButton;
    public GameObject gameEndUI;
    public Text winnerText;
    public Text guideText;

    public int N
    {
        get { return n; }
    }

    public int M
    {
        get { return m; }
    }

    public int CurrentPlayerNumber
    {
        get { return currentPlayerNumber; }
        set { currentPlayerNumber = value; }
    }

    public int CurrentEnemyNumber
    {
        get { return currentEnemyNumber; }
        set { currentEnemyNumber = value; }
    }
    public bool[,] IsCharacterPosition
    {
        get { return isCharacterPosition; }
        set { isCharacterPosition = value; }
    }

    void Start()
    {
        isCharacterPosition = new bool[m, n];
        messageBox.SetActive(false);
        autoPlayingText.SetActive(false);
        timerText.SetActive(false);
        gameEndUI.SetActive(false);

        for (int i = 0; i < m; i++)
            for (int j = 0; j < n; j++)
                isCharacterPosition[i, j] = false;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        currentPlayerNumber = players.Length;
        currentEnemyNumber = enemies.Length;

        foreach (GameObject enemy in enemies)
        {
            int posX = Mathf.RoundToInt(enemy.transform.position.x/2);
            int posY = Mathf.RoundToInt(enemy.transform.position.y/2);
            isCharacterPosition[posX,posY] = true;
        }
    }

    /*
     * 0명이 된 진영이 있는 경우, 게임 종료 함수를 수행합니다.
     */
    void Update()
    {
        if (currentPlayerNumber == 0 || currentEnemyNumber == 0)
            ShowWinner();
    }

    /*
     * Play 버튼의 OnClick 함수에 적용한 함수입니다.
     * 모든 플레이어 캐릭터를 배치 완료 후, Play 버튼을 누르면 모든 캐릭터들의 이동 및 공격 행동이 수행하도록 활성화합니다.
     * 모든 플레이어 캐릭터를 배치하지 않고, Play 버튼을 누르면 모두 배치하라는 메시지 UI를 보여줍니다.
     */
    public void allPlayerParticipate()
    {
        int cnt = 0;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (-1.0 < player.transform.position.x && player.transform.position.x < 2 * M - 1 && -1.0 <= player.transform.position.y && player.transform.position.y < 2 * N - 1)
                cnt++;
        }

        if (cnt == maxPlayerNumber)
        {
            autoPlayingText.SetActive(true);
            timerText.SetActive(true);
            playButton.SetActive(false);
            guideText.gameObject.SetActive(false);
            ActiveMoveAndAttack();
        }
        else
            ShowParticipantMessageBox();
    }

    /*
     * 모든 캐릭터 컨트롤러의 Update 함수를 활성화합니다.
     */
    public void ActiveMoveAndAttack()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        CharacterController characterController;

        foreach (GameObject character in players)
        {
            characterController = character.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = true;
                characterController.hpBarSlider.gameObject.SetActive(true);
            }
        }

        foreach (GameObject character in enemies)
        {
            characterController = character.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = true;
                characterController.hpBarSlider.gameObject.SetActive(true);
            }
        }
    }

    /*
     * 모든 캐릭터 불필요한 요소를 비활성화합니다.
     */
    public void InactiveMoveAndAttack()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        CharacterController characterController;

        foreach (GameObject character in players)
        {
            characterController = character.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
                characterController.hpBarSlider.gameObject.SetActive(false);
                characterController.animator.SetBool("canAttack", false);
                characterController.animator.SetBool("isMoving", false);
            }
        }

        foreach (GameObject character in enemies)
        {
            characterController = character.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
                characterController.hpBarSlider.gameObject.SetActive(false);
                characterController.animator.SetBool("canAttack", false);
                characterController.animator.SetBool("isMoving", false);
            }
        }
    }

    /*
     * 모든 플레이어 캐릭터를 배치하지 않았을 때 ParticipantMessageBox UI를 활성화합니다.
     */
    public void ShowParticipantMessageBox()
    {
        if (messageBox != null)
            messageBox.SetActive(true);
    }

    /*
     * ParticipantMessageBox의 X 버튼의 OnClick 함수에 등록해둔 함수로, 해당 UI를 비활성화합니다.
     */
    public void CloseParticipantMessageBox()
    {
        if (messageBox != null)
            messageBox.SetActive(false);
    }

    /*
     * 게임 종료 시, 모든 캐릭터의 움직임을 중지하고 불필요한 요소를 비활성화합니다.
     * 게임의 승자를 알리는 메시지 UI를 활성화합니다.
     * 각 진영의 인원 수를 비교해 승자를 알립니다.
     */
    public void ShowWinner()
    {
        InactiveMoveAndAttack();

        if (currentPlayerNumber < currentEnemyNumber)
            winnerText.text = "적이 승리했습니다!";
        else if(currentPlayerNumber > currentEnemyNumber)
            winnerText.text = "플레이어가 승리했습니다!";
        else
            winnerText.text = "비겼습니다!";

        gameEndUI.SetActive(true);

        TimerController timerController = timerText.GetComponent<TimerController>();
        if (timerController != null)
        {
            timerController.enabled = false;
        }
    }

    // 승자 메시지 UI의 Restart 버튼의 OnClick 함수에 등록해둔 함수로 게임을 재시작합니다
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 승자 메시지 UI의 Exit 버튼의 OnClick 함수에 등록해둔 함수로 게임을 종료합니다
    public void Exit()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
