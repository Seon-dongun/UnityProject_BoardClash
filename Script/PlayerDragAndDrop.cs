using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터의 드래그앤드롭 기능 수행을 위한 클래스
public class PlayerDragAndDrop : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset,lastPos;
    public GameDirector gd;

    public void Start()
    {
        lastPos = transform.position;
    }

    /*
     * 마우스를 누르고 있는 동안, 드래그 상태임을 체크하고, 현재 게임 오브젝트의 위치와 마우스 클릭 위치 사이의 차이를 구합니다. 
     * 이를 통해 해당 오브젝트의 이동 거리를 구합니다.
     */
    private void OnMouseDown()
    {
        isDragging = true;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    /*
     * 마우스를 때면, 드롭 상태가 되어 캐릭터를 올바른 보드칸 지점에 배치합니다.
     * 정확한 보드칸 정 중앙 지점이 아니더라도, 캐릭터를 놓은 좌표에 따라 가장 인접한 보드칸 중앙 좌표로 이동시킵니다.
     * 만약 캐릭터를 이미 다른 캐릭터가 있는 지점에 놓는 경우, 올바른 배치가 아니기 때문에 가장 최근 배근 지점으로 재배치합니다.
     */
    private void OnMouseUp()
    {
        isDragging = false;

        if(-1.0 < transform.position.x && transform.position.x < 2*gd.M -1&& -1.0 <=transform.position.y && transform.position.y < 2*gd.N -1)
        {
            float newX, newY;
            if ((int)transform.position.x % 2 == 0)
            {
                if(transform.position.x<0)
                    newX = Mathf.Ceil(transform.position.x);
                else
                    newX = Mathf.Floor(transform.position.x);
            }
            else
                newX = Mathf.Ceil(transform.position.x);
           

            if ((int)transform.position.y % 2 == 0)
            {
                if (transform.position.y < 0)
                    newY = Mathf.Ceil(transform.position.y);
                else
                    newY = Mathf.Floor(transform.position.y);
            }
            else
                newY = Mathf.Ceil(transform.position.y);


            if (gd.IsCharacterPosition[(int)newX / 2, (int)newY / 2] ==true)
                transform.position = lastPos;

            else 
            {
                gd.IsCharacterPosition[(int)newX / 2, (int)newY / 2] = true;

                if (-1.0 < lastPos.x && lastPos.x < 2 * gd.M - 1 && -1.0 <= lastPos.y && lastPos.y < 2 * gd.N - 1)
                    gd.IsCharacterPosition[(int)(lastPos.x/2), (int)(lastPos.y/2)] = false;

                transform.position = new Vector3(newX, newY, transform.position.z);
                lastPos = transform.position;
            }
        }
        else
        {
            if (-1.0 < lastPos.x && lastPos.x < 2 * gd.M - 1 && -1.0 <= lastPos.y && lastPos.y < 2 * gd.N - 1)
            {
                gd.IsCharacterPosition[(int)(lastPos.x / 2), (int)(lastPos.y / 2)] = false;
                lastPos = transform.position;
            }
        }
    }

    /*
     * 마우스를 드래그 상태라면, offset을 활용해 캐릭터를 이동시킵니다.
     */
    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
        }
    }
}
