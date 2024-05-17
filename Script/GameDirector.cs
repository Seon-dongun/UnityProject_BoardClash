using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// �������� ������ �����ϴ� Ŭ����
public class GameDirector : MonoBehaviour
{
    private int currentPlayerNumber, currentEnemyNumber; 
    private int n = 5, m = 10;
    private int maxPlayerNumber = 3;
    private float lastUpdateTime = 0.0f;
    private bool[,] isCharacterPosition; // �����ǿ��� ������ ����ĭ�� ĳ���Ͱ� ��ġ�ϰ� �ִ��� ���θ� ��Ÿ���� �迭
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
     * 0���� �� ������ �ִ� ���, ���� ���� �Լ��� �����մϴ�.
     */
    void Update()
    {
        if (currentPlayerNumber == 0 || currentEnemyNumber == 0)
            ShowWinner();
    }

    /*
     * Play ��ư�� OnClick �Լ��� ������ �Լ��Դϴ�.
     * ��� �÷��̾� ĳ���͸� ��ġ �Ϸ� ��, Play ��ư�� ������ ��� ĳ���͵��� �̵� �� ���� �ൿ�� �����ϵ��� Ȱ��ȭ�մϴ�.
     * ��� �÷��̾� ĳ���͸� ��ġ���� �ʰ�, Play ��ư�� ������ ��� ��ġ�϶�� �޽��� UI�� �����ݴϴ�.
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
     * ��� ĳ���� ��Ʈ�ѷ��� Update �Լ��� Ȱ��ȭ�մϴ�.
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
     * ��� ĳ���� ���ʿ��� ��Ҹ� ��Ȱ��ȭ�մϴ�.
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
     * ��� �÷��̾� ĳ���͸� ��ġ���� �ʾ��� �� ParticipantMessageBox UI�� Ȱ��ȭ�մϴ�.
     */
    public void ShowParticipantMessageBox()
    {
        if (messageBox != null)
            messageBox.SetActive(true);
    }

    /*
     * ParticipantMessageBox�� X ��ư�� OnClick �Լ��� ����ص� �Լ���, �ش� UI�� ��Ȱ��ȭ�մϴ�.
     */
    public void CloseParticipantMessageBox()
    {
        if (messageBox != null)
            messageBox.SetActive(false);
    }

    /*
     * ���� ���� ��, ��� ĳ������ �������� �����ϰ� ���ʿ��� ��Ҹ� ��Ȱ��ȭ�մϴ�.
     * ������ ���ڸ� �˸��� �޽��� UI�� Ȱ��ȭ�մϴ�.
     * �� ������ �ο� ���� ���� ���ڸ� �˸��ϴ�.
     */
    public void ShowWinner()
    {
        InactiveMoveAndAttack();

        if (currentPlayerNumber < currentEnemyNumber)
            winnerText.text = "���� �¸��߽��ϴ�!";
        else if(currentPlayerNumber > currentEnemyNumber)
            winnerText.text = "�÷��̾ �¸��߽��ϴ�!";
        else
            winnerText.text = "�����ϴ�!";

        gameEndUI.SetActive(true);

        TimerController timerController = timerText.GetComponent<TimerController>();
        if (timerController != null)
        {
            timerController.enabled = false;
        }
    }

    // ���� �޽��� UI�� Restart ��ư�� OnClick �Լ��� ����ص� �Լ��� ������ ������մϴ�
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ���� �޽��� UI�� Exit ��ư�� OnClick �Լ��� ����ص� �Լ��� ������ �����մϴ�
    public void Exit()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
