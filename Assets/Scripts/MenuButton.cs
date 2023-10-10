using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Quaternion oldRotation;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private float reactionTimeEnter = 7f;
    [SerializeField] private float reactionTimeExit = 10f;
    [SerializeField]  bool rotateOnEnter = false;
    [SerializeField]  bool rotateOnExit = false;
    Color hoverColor = new Color(203, 0, 0, 255);
    Color oldColor;
    
    private void Start()
    {
        oldColor = buttonText.color;
        oldRotation = transform.rotation;
    }

    private void Update()
    {
        
        if (rotateOnEnter)
        {
            RotateObjectOnEnter();
        }
        else if (rotateOnExit)
        {
            RotateObjectOnExit();
        }
    }

    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        rotateOnEnter = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = oldColor;
        rotateOnExit = true;
        rotateOnEnter = false;
    }
    private void RotateObjectOnEnter()
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, Random.Range(-25f, 25f));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, reactionTimeEnter * Time.deltaTime);
        
    }

    private void RotateObjectOnExit()
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, oldRotation, reactionTimeExit * Time.deltaTime);
        if (transform.rotation == oldRotation)
        {
            rotateOnExit = false;
        }
    }
}
