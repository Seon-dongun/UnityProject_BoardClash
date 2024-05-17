using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ĳ������ �巡�׾ص�� ��� ������ ���� Ŭ����
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
     * ���콺�� ������ �ִ� ����, �巡�� �������� üũ�ϰ�, ���� ���� ������Ʈ�� ��ġ�� ���콺 Ŭ�� ��ġ ������ ���̸� ���մϴ�. 
     * �̸� ���� �ش� ������Ʈ�� �̵� �Ÿ��� ���մϴ�.
     */
    private void OnMouseDown()
    {
        isDragging = true;
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


    /*
     * ���콺�� ����, ��� ���°� �Ǿ� ĳ���͸� �ùٸ� ����ĭ ������ ��ġ�մϴ�.
     * ��Ȯ�� ����ĭ �� �߾� ������ �ƴϴ���, ĳ���͸� ���� ��ǥ�� ���� ���� ������ ����ĭ �߾� ��ǥ�� �̵���ŵ�ϴ�.
     * ���� ĳ���͸� �̹� �ٸ� ĳ���Ͱ� �ִ� ������ ���� ���, �ùٸ� ��ġ�� �ƴϱ� ������ ���� �ֱ� ��� �������� ���ġ�մϴ�.
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
     * ���콺�� �巡�� ���¶��, offset�� Ȱ���� ĳ���͸� �̵���ŵ�ϴ�.
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
