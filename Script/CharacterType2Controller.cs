using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ĳ���� Ÿ��2. �ٰŸ� ���� Ÿ�� ĳ���� ��Ʈ�ѷ�
public class CharacterType2Controller : CharacterController
{
    int attackDamage = 20;

    int[] dx = { 0, 0, 2, -2, -2, -2, 2, 2 };
    int[] dy = { 2, -2, 0, 0, 2, -2, 2, -2 };

    /*
     * ���� ���� �ȿ� ��ǥ ����� �ִٸ� �����մϴ�.
     * ���� ������ �����ϱ� ������ ��� ��뿡 ���ؼ� ���� ������ �����ִ��� Ȯ���� ������ �����մϴ�.
     * ����� �ο��� 0���� �Ǹ� ������ ����� ���̱� ������ �߰����� Ž�� ���� ������ �����մϴ�.
     * ���� ���� ���� ĳ������ ���ݷ¸�ŭ ü���� �Ҹ�Ǹ�, ü���� 0���Ϸ� �������� ����� ������ ó���մϴ�.
     * ĳ���� ��� �� �ش� ĳ���ʹ� ��Ȱ��ȭ�Ǹ�, �ش� ĳ������ ���� �ο� ���� ���ҽ�ŵ�ϴ�.
     */
    protected override void AttackTarget(GameObject nearestTarget)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(nearestTarget.tag);

        for (int i = 0; i < 8; i++)
        {
            if (gd.CurrentPlayerNumber == 0 || gd.CurrentEnemyNumber == 0)
                break;

            for (int j = 0; j < targets.Length; j++)
            {
                if ((int)transform.position.x + dx[i] == targets[j].transform.position.x && (int)transform.position.y + dy[i] == targets[j].transform.position.y)
                {
                    CharacterController characterController = targets[j].GetComponent<CharacterController>();
                    characterController.currentHealth -= attackDamage;

                    if (characterController.currentHealth > 0)
                        characterController.AdjustHpBar();

                    else
                    {
                        targets[j].SetActive(false);
                        characterController.hpBarSlider.gameObject.SetActive(false);
                        gd.IsCharacterPosition[(int)targets[j].transform.position.x / 2, (int)targets[j].transform.position.y / 2] = false;
                        
                        if (targets[j].CompareTag("Player"))
                            gd.CurrentPlayerNumber--;
                        else if (targets[j].CompareTag("Enemy"))
                            gd.CurrentEnemyNumber--;
                    }
                }
            }           
        }
    }

    protected override bool IsTargetInArea(GameObject nearestTarget)
    {
        for (int i = 0; i < 8; i++)
            if ((int)transform.position.x + dx[i] == nearestTarget.transform.position.x && (int)transform.position.y + dy[i] == nearestTarget.transform.position.y)
                return true;

        return false;
    }
}
