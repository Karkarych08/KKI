using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;


public class CardMovementScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    public CardController CC;
    Camera MainCamera;
    Vector3 offset;
    public Transform DefaultParent, DefaultGameCardParent;
    GameObject TempCardGO;
    public bool isDraggable;
    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset=transform.position - MainCamera.ScreenToWorldPoint(eventData.position);
        DefaultParent = DefaultGameCardParent=transform.parent;

        isDraggable = ((DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.SELF_HAND && 
            GameManagerScript.instance.PlayerMana >= CC.Card.Manacost) ||
            (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.SELF_FIELD &&
            CC.Card.CanAttack)) &&
            GameManagerScript.instance.IsPlayerTurn;
        if (!isDraggable)
            return;
        if (CC.Card.isSpell || CC.Card.CanAttack)
            GameManagerScript.instance.HighlightAsTarget(CC,true);
        TempCardGO.transform.SetParent(DefaultParent);
        TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());
        transform.SetParent(DefaultParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable)
            return;
        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos+offset;
        if(!CC.Card.isSpell)
        {
            if (DefaultParent.GetComponent<DropPlaceScript>().Type != FieldType.SELF_FIELD)
                CheckPosition();
            if (TempCardGO.transform.parent != DefaultGameCardParent)
                TempCardGO.transform.SetParent(DefaultGameCardParent);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable)
            return;
        GameManagerScript.instance.HighlightAsTarget(CC, false);
        transform.SetParent(DefaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(2500, 0);
    }

    void CheckPosition()
    {
        int newIndex = DefaultGameCardParent.childCount;
        for (int i = 0; i < DefaultGameCardParent.childCount; i++)
        {
            if (transform.position.x < DefaultGameCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if (TempCardGO.transform.GetSiblingIndex() < newIndex)
                    newIndex--;
                break;
            }
        }
        TempCardGO.transform.SetSiblingIndex(newIndex);
    }
    public void MoveToField(Transform field)
    {
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.DOMove(field.position, .5f);
        transform.SetParent(GameObject.Find("Canvas").transform);
    }

    public void MoveToTarget(Transform target)
    {
        StartCoroutine(MoveToTargetCore(target));
    }

    IEnumerator MoveToTargetCore(Transform target) 
    {
        Vector3 pos = target.position;
        Transform parent = transform.parent;
        int index = transform.GetSiblingIndex();
        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        transform.SetParent(GameObject.Find("Canvas").transform);
        transform.DOMove(target.position, .25f);
        yield return new WaitForSeconds(.25f);
        transform.DOMove(pos, .25f);
        yield return new WaitForSeconds(.25f);
        transform.SetParent(parent);
        transform.SetSiblingIndex(index);
        transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;

    }
}
