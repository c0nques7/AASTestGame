using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldInteraction : MonoBehaviour {
    NavMeshAgent playerAgent;

    void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            GetInteraction();
    }

    void GetInteraction()
    {
        Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit InteractionInfo;
        if(Physics.Raycast(interactionRay, out InteractionInfo, Mathf.Infinity))
        {
            GameObject interactedObject = InteractionInfo.collider.gameObject;
            if(interactedObject.tag == "Interactable Object")
            {
                Debug.Log("Interactable interacted.");
            }
            else
            {
                playerAgent.destination = InteractionInfo.point;
            }
        }
    }
}
