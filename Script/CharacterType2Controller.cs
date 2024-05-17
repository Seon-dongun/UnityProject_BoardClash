using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 타입2. 근거리 광역 타입 캐릭터 컨트롤러
public class CharacterType2Controller : CharacterController
{
    int attackDamage = 20;

    int[] dx = { 0, 0, 2, -2, -2, -2, 2, 2 };
    int[] dy = { 2, -2, 0, 0, 2, -2, 2, -2 };

    /*
     * 공격 범위 안에 목표 대상이 있다면 공격합니다.
     * 광격 공격이 가능하기 때문에 모든 상대에 대해서 공격 범위에 들어와있는지 확인해 공격을 수행합니다.
     * 상대의 인원이 0명이 되면 게임이 종료된 것이기 때문에 추가적인 탐색 없이 공격을 종료합니다.
     * 공격 받은 상대는 캐릭터의 공격력만큼 체력이 소모되며, 체력이 0이하로 떨어지면 사망한 것으로 처리합니다.
     * 캐릭터 사망 시 해당 캐릭터는 비활성화되며, 해당 캐릭터의 진영 인원 수를 감소시킵니다.
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
