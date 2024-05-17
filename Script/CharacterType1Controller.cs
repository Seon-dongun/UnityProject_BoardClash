using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 타입1. 근거리 타입 캐릭터 컨트롤러
public class CharacterType1Controller : CharacterController
{
    int attackDamage = 30;

    int[] dx = { 0, 0, 2, -2, -2, -2, 2, 2 };
    int[] dy = { 2, -2, 0, 0, 2, -2, 2, -2 };


    /*
     * 공격 범위 안에 목표 대상이 있다면 공격합니다.
     * 단일 공격만 가능하기 때문에 가장 가까운 목표 대상을 공격하고 동작이 마무리됩니다.
     * 공격 받은 상대는 캐릭터의 공격력만큼 체력이 소모되며, 체력이 0이하로 떨어지면 사망한 것으로 처리합니다.
     * 캐릭터 사망 시 해당 캐릭터는 비활성화되며, 해당 캐릭터의 진영 인원 수를 감소시킵니다.
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
