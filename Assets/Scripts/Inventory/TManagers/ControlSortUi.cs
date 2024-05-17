using UnityEngine.UI;
using UnityEngine;

public class ControlSortUi : MonoBehaviour
{
    public Image[] highlights; 
    public Image[] icons; 
    [Space]

    [SerializeField] private Button organizeButton = null; 
    [SerializeField] private Button transferAllButton = null; 
    [SerializeField] private Button transferSameButton = null; 
    [SerializeField] private GameObject transferBox = null;

    public void ActiveHighlight(int sort)
    {
        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].enabled = false;
            icons[i].color = new Color32(255,255,255,33);
        }

        highlights[sort].enabled = true;
         icons[sort].color = new Color32(255,255,255,215);

        if (sort == 0)
        {
            organizeButton.interactable = true;
            transferAllButton.interactable = true;
            transferSameButton.interactable = true;
        }
        else
        {
            organizeButton.interactable = false;
            transferAllButton.interactable = false;
            transferSameButton.interactable = false;
        }

        print(sort);
    }

    public void DisableOrderslot()
    {
        transferBox.SetActive(false);
    }
}
