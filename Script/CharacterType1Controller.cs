using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ĳ���� Ÿ��1. �ٰŸ� Ÿ�� ĳ���� ��Ʈ�ѷ�
public class CharacterType1Controller : CharacterController
{
    int attackDamage = 30;

    int[] dx = { 0, 0, 2, -2, -2, -2, 2, 2 };
    int[] dy = { 2, -2, 0, 0, 2, -2, 2, -2 };


    /*
     * ���� ���� �ȿ� ��ǥ ����� �ִٸ� �����մϴ�.
     * ���� ���ݸ� �����ϱ� ������ ���� ����� ��ǥ ����� �����ϰ� ������ �������˴ϴ�.
     * ���� ���� ���� ĳ������ ���ݷ¸�ŭ ü���� �Ҹ�Ǹ�, ü���� 0���Ϸ� �������� ����� ������ ó���մϴ�.
     * ĳ���� ��� �� �ش� ĳ���ʹ� ��Ȱ��ȭ�Ǹ�, �ش� ĳ������ ���� �ο� ���� ���ҽ�ŵ�ϴ�.
     */
    protected override void AttackTarget(GameObject nearestTarget)
    {
        for (int i = 0; i < 8; i++)
        {
            if ((int)transform.position.x + dx[i] == nearestTarget.transform.position.x && (int)transform.position.y + dy[i] == nearestTarget.transform.position.y)
            {
                CharacterController characterController = nearestTarget.GetComponent<CharacterController>();
                characterController.currentHealth -= attackDamage;

                if (characterController.currentHealth > 0)
                    characterController.AdjustHpBar();

                else
                {
                    nearestTarget.SetActive(false);
                    characterController.hpBarSlider.gameObject.SetActive(false);
                    gd.IsCharacterPosition[(int)nearestTarget.transform.position.x / 2, (int)nearestTarget.transform.position.y / 2] = false;
                    
                    if (nearestTarget.CompareTag("Player"))
                        gd.CurrentPlayerNumber--;
                    else if (nearestTarget.CompareTag("Enemy"))
                        gd.CurrentEnemyNumber--;
                }         
                break;
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
