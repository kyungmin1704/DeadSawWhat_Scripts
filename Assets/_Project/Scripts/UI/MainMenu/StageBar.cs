using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageBar : BasicButtonVisual
{
    public TextMeshProUGUI stageName;   //스테이지의 이름
    public TextMeshProUGUI description; //스테이지의 대한 설명
    public Image stageSprite;           //스테이지의 이미지

    public StageData data;              //스테이지의 데이터
    HomeMenu menu;

    public bool stageOpen;

    public void Init(StageData data, HomeMenu menu)
    {
        base.Init();

        this.data = data;
        this.menu = menu;

        stageSprite.sprite = data.stageSprite;      //Stage의 이미지 참조
        stageName.text = data.stageName;            //Stage의 이름
        description.text = data.description;        //Stage의 설명

        stageOpen = false;

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        Selected();
    }

    public override void Highlight()
    {
        base.Highlight();
        bgTween = mainImage.DOColor(targetColor, 0.2f); //부모에서 currentTween을 사용중이라 남아있는 Tween사용
    }
    public override void Selected()
    {
        base.Selected();
        menu.StageSelected(this);
    }
}
