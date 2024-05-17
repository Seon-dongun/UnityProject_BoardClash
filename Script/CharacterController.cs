using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �⺻ ĳ���� ��Ʈ�ѷ�
public abstract class CharacterController : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float currentHealth;
    public Slider hpBarSlider;

    public Animator animator;
    protected GameDirector gd;
    private Vector3[] directions = { Vector3.left * 2, Vector3.right * 2, Vector3.up * 2, Vector3.down * 2 };
    private float lastUpdateTime = 0.0f;

    protected abstract bool IsTargetInArea(GameObject nearestTarget);
    protected abstract void AttackTarget(GameObject nearestTarget);

    void Start()
    {
        enabled = false;
        hpBarSlider.gameObject.SetActive(false);

        gd = FindObjectOfType<GameDirector>();
        if (gd == null)
            Debug.Log("No GameDirector");

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.Log("No animator");
 
        currentHealth = maxHealth;
    }

    /*
     * 1�ʿ� �� ���� �̵� Ȥ�� ������ �����մϴ�.
     * ��� ������ ĳ���� �� ���� ����� ĳ���͸� ã��, ���� ����� ��� ������ ĳ���Ͱ� ���� ���� �ȿ� �ִٸ� �����մϴ�.
     * ���� ���� �ȿ� ��밡 ���ٸ� �̵��մϴ�.
     */
    void Update()
    {
        if (Time.time - lastUpdateTime >= 1.0f)
        {
            if (gd.CurrentPlayerNumber == 0 || gd.CurrentEnemyNumber == 0)
                return;

            animator.SetBool("canAttack", false);
            GameObject nearestTarget = null;

            if (gameObject.CompareTag("Player"))
                nearestTarget = FindNearestTarget("Enemy");
            else if (gameObject.CompareTag("Enemy"))
                nearestTarget = FindNearestTarget("Player");

            if (IsTargetInArea(nearestTarget) == true)
            {
                animator.SetBool("canAttack", true);
                AttackTarget(nearestTarget);
            }
            else 
                StartCoroutine(MoveWithDelay(nearestTarget));

            lastUpdateTime = Time.time;
        }
    }

    /*
     * �±׸� Ȱ���Ͽ� ��� ������ �ش�Ǵ� ������Ʈ���� ��������, ��ǥ�� Ȱ���Ͽ� ���� ����� ����� ã���ϴ�
     */
    GameObject FindNearestTarget(string targetTag)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        Vector2Int currentPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        GameObject nearestTarget = null;

        float minDistanceSquared = float.MaxValue;

        foreach (GameObject target in targets)
        {
            Vector2Int targetPosition = new Vector2Int((int)target.transform.position.x, (int)target.transform.position.y);

            float dx = targetPosition.x - currentPosition.x;
            float dy = targetPosition.y - currentPosition.y;
            float distanceSquared = dx * dx + dy * dy;
            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                nearestTarget = target;
            }
        }
        return nearestTarget;
    }

    /*
     * �̵��� ������ ���ÿ� ����Ǵ� ���, ���� ������ �켱������ ����� �� �ֵ��� �̵� �� 0.1���� �����̸� �����߽��ϴ�.
     */
    IEnumerator MoveWithDelay(GameObject nearestTarget)
    {
        yield return new WaitForSeconds(0.1f); 
        MoveToTarget(nearestTarget);
    }

    /*
     * �����¿� �� ��� ĳ���Ϳ��� ���� ������ �� �� �ִ� ������ ���, �ش� ��ġ���� �̵��� �����մϴ�
     */
    void MoveToTarget(GameObject target)
    {
        Vector3 movePosition = GetMovePosition(transform.position, target.transform.position);
        if (movePosition.Equals(new Vector3(-1,-1,0)))
            return;

        gd.IsCharacterPosition[(int)transform.position.x / 2, (int)transform.position.y / 2] = false;
        StartCoroutine(MoveCoroutine(transform.position, movePosition));
        gd.IsCharacterPosition[(int)movePosition.x / 2, (int)movePosition.y / 2] = true;
    }

    /*
     * �����¿� ���� �� �̵��� �����ϸ鼭 ��ǥ ���� ���� ����� ������ ã���ϴ�.
     */
    Vector3 GetMovePosition(Vector3 currentPosition, Vector3 targetPosition)
    {
        float minDistanceSquared = float.MaxValue;
        Vector3 newPosition = new Vector3(-1, -1, 0);

        foreach (Vector3 direction in directions)
        {
            Vector3 movePosition = currentPosition + direction;
            if (movePosition.x < 0 || movePosition.x >= 2 * gd.M || movePosition.y < 0 || movePosition.y >= 2 * gd.N)
                continue;

            float dx = targetPosition.x - movePosition.x;
            float dy = targetPosition.y - movePosition.y;
            float newDistanceSquared = dx * dx + dy * dy;
            if (gd.IsCharacterPosition[(int)movePosition.x / 2, (int)movePosition.y / 2] == false && minDistanceSquared > newDistanceSquared)
            {
                newPosition = movePosition;
                minDistanceSquared = newDistanceSquared;
            }
        }
        return newPosition;
    }

    /*
     * ĳ������ �̵��� ����ϴ� �ڷ�ƾ �Լ��Դϴ�. ����� �ð� ������ Ȱ���Ͽ� ���� �������� ��ǥ �������� �ڿ������� �̵��ϵ��� �����߽��ϴ�
     * �̵� �ϴ� ���ȿ��� �̵� �ִϸ��̼��� ����˴ϴ�.
     */
    IEnumerator MoveCoroutine(Vector3 startPosition, Vector3 targetPosition)
    {
        float moveTime = 0.8f;
        float elapsedTime = 0.0f;

        animator.SetBool("isMoving", true);

        while (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveTime);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        animator.SetBool("isMoving", false);

        transform.position = targetPosition;
    }

    /*
     * ����� ü�¿� �°� ü�¹ٸ� �����մϴ�
     */
    public void AdjustHpBar()
    {
        float hpRatio = (float)currentHealth / maxHealth;
        hpBarSlider.value = hpRatio;
    }
}
