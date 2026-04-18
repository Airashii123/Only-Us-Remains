using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange = 3f;

    public TextMeshProUGUI interactText;

    private IInteractable currentInteractable;

    void Update()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);

        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                currentInteractable = interactObj;

                interactText.gameObject.SetActive(true);
                interactText.text = interactObj.GetInteractPrompt();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactObj.Interact();
                }

                return;
            }
        }

        currentInteractable = null;
        interactText.gameObject.SetActive(false);
    }
}
