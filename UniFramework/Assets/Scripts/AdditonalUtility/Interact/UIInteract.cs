using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteract : MonoBehaviour
{
    [SerializeField] private GameObject interactPanel;
    [SerializeField] private PlayerInteract PlayerInteract;


    // Update is called once per frame
    void Update()
    {
        var interactable = PlayerInteract.GetInteractableObj();
        if (interactable != null)
        {
            Show(interactable);
        }
        else
        {
            Hide();
        }
    }

    private void Show(InteractBase interactBase)
    {
        interactPanel.SetActive(true);
    }

    private void Hide()
    {
        interactPanel.SetActive(false);
    }
}