using UnityEngine;
using System.Collections.Generic;
public class TopViewPanel : MonoBehaviour
{
    public List<TopViewButton> buttons = new List<TopViewButton>();


    public void Init()
    {
        foreach (var button in buttons)
        {
            button.Init();
            button.SetPanel(this);
        }
    }
    public void SelcetedPanelButton(TopViewButton button)
    {
        foreach (var btn in buttons)
        {
            if (btn == button)
            {
                button.isOn = true;
                button.panel.SetActive(button.isOn);
            }else
            {
                btn.Refresh();
                btn.panel.SetActive(false);
            }
        }
    }

}
