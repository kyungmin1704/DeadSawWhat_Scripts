using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class OptionButtonVisual : MonoBehaviour, IMenuButton
{
    public Image NormalBg;     //스테이지 바의 기본 백그라운드
    public Image highlightBg; //스테이지가 선택 됐을 때 나올 

    public OptionButtonVisualData visualData; //OptionButton에 애니메이션에 사용 할 비주얼데이타
    private Tween currentTween; //스테이지바가 실행 할 DoTween

    bool isOn;              //버튼이 클리이 되었는지의 대한 판단 여부
    bool IMenuButton.isOn => isOn;


    public OptionButtonData data;
    SettingOptions setOption; //SettingOptionButton을 관리
    public TextMeshProUGUI opButtonTitle;       //옵션 버튼의 이름.


    public void SetOption(SettingOptions option , OptionButtonData optData)
    {
        this.setOption = option;
        this.data = optData;
        opButtonTitle.text = data.optionName;
    }

    

    #region 마우스커서의 따라 OptionButton에 대한 애니메이션 관리
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isOn) return;
        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isOn) return;
        Refresh();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        ButtonClick();
    }


    public virtual void Refresh()
    {
        currentTween?.Kill();
        highlightBg.color = visualData.originalColor;
        isOn = false;
    }
    public virtual void Highlight()
    {
        currentTween?.Kill();
        currentTween = highlightBg.DOColor(visualData.highlightColor, 0.2f);
    }

    public virtual void ButtonClick()
    {
        isOn = true;
        currentTween?.Kill();
        currentTween = highlightBg.DOColor(visualData.highlightColor, 0.2f);

        setOption.SelectedButton(this);
    }

    public void InitializeColor()
    {
        visualData.originalColor = highlightBg.color;
    }
    #endregion
}
