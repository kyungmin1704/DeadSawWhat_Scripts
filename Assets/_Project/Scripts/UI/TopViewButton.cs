using UnityEngine.EventSystems;
using UnityEngine;

public class TopViewButton : BasicButtonVisual
{
    public GameObject panel;       //버튼이 눌렸을 때 활성화 될 패널

    TopViewPanel topPanel;

    public void SetPanel(TopViewPanel panel)
    {
        this.topPanel = panel;
        this.panel.SetActive(false) ;
        
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Selected();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
    }
    public override void Highlight()
    {
        base.Highlight();
        if (btnBackground.enabled == false) btnBackground.enabled = true;
    }
    public override void Selected()
    {
        base.Selected();
        topPanel.SelcetedPanelButton(this);
    }


}
