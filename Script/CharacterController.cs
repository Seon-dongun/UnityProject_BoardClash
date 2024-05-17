using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 기본 캐릭터 컨트롤러
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
     * 1초에 한 번씩 이동 혹은 공격을 수행합니다.
     * 상대 진영의 캐릭터 중 가장 가까운 캐릭터를 찾고, 가장 가까운 상대 진영의 캐릭터가 공격 범위 안에 있다면 공격합니다.
     * 공격 범위 안에 상대가 없다면 이동합니다.
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
     * 태그를 활용하여 상대 진영에 해당되는 오브젝트들을 가져오고, 좌표를 활용하여 가장 가까운 대상을 찾습니다
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
     * 이동과 공격이 동시에 수행되는 경우, 공격 판정이 우선적으로 적용될 수 있도록 이동 시 0.1초의 딜레이를 적용했습니다.
     */
    IEnumerator MoveWithDelay(GameObject nearestTarget)
    {
        yield return new WaitForSeconds(0.1f); 
        MoveToTarget(nearestTarget);
    }

    /*
     * 상하좌우 중 상대 캐릭터에게 가장 가까이 갈 수 있는 지점을 얻고, 해당 위치까지 이동을 수행합니다
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
     * 상하좌우 방향 중 이동이 가능하면서 목표 대상과 가장 가까운 지점을 찾습니다.
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
     * 캐릭터의 이동을 담당하는 코루틴 함수입니다. 경과한 시간 비율을 활용하여 시작 지점에서 목표 지점까지 자연스럽게 이동하도록 구현했습니다
     * 이동 하는 동안에는 이동 애니메이션이 재생됩니다.
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
     * 변경된 체력에 맞게 체력바를 조절합니다
     */
    public void AdjustHpBar()
    {
        float hpRatio = (float)currentHealth / maxHealth;
        hpBarSlider.value = hpRatio;
    }
}
