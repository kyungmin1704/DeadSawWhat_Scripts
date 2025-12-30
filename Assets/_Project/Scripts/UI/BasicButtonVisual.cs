using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasicButtonVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public Image mainImage;     //본인이 가지고 있는 이미지
    public Image btnBackground; //버튼에 마우스포인트가 올라갔을 때 활성화 될 이미지.

    protected Vector3 originalScale; //버튼의 기존 크기
    protected Color originalColor;   //버튼 오리지널 컬러
    protected Color bgOriginalColor;   //백그라운드 오리지널 컬러
    public Color targetColor;      //색상을 변경 할 컬러


    protected Tween currentTween; //현재 실행하고있는 DOTween
    protected Tween bgTween;      //BtnBackground의 색을 변경 할 DOTween

    public bool isOn;




    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Selected();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (isOn) { return; }
        Highlight();

    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (isOn) { return; }
        Refresh();
    }
    public virtual void Highlight() //hoverSound 플레이 / 현재 오브젝트 스케일이 커졌다 작아졌다만 하는 애니메이션
    {
        UISoundManager.Instance.HoverSoundPlay();
        currentTween?.Kill();
        currentTween = transform.DOScale(1.05f, .2f).OnKill(() => transform.localScale = originalScale);
    }
    public virtual void Selected()  //메인 이미지와, 버튼백그라운드 이미지의 색을 변경하는 애니메이션 / 현재오브젝트의 스케일을 기존 사이즈로
    {
        UISoundManager.Instance.ClickSoundPlay();
        currentTween?.Kill();
        bgTween?.Kill();


        btnBackground.enabled = true;
        currentTween = mainImage.DOColor(targetColor, 0.2f);
        bgTween = btnBackground.DOColor(targetColor, 0.2f);

        transform.localScale = originalScale;
    }
    public virtual void Refresh()   //메인이미지와, 백그라운드이미지의 색상을 기존색으로 초기화, 스케일도 기존 사이즈로.
    {
        currentTween?.Kill();
        bgTween?.Kill();

        currentTween = mainImage.DOColor(originalColor, 0.2f);
        bgTween = btnBackground.DOColor(bgOriginalColor, 0.2f);

        transform.localScale = originalScale;

        btnBackground.enabled = false;
        isOn = false;
    }

    public virtual void Init()
    {
        originalScale = transform.localScale;        //시작 시 기존 크기 저장
        originalColor = mainImage.color;             //최초 메인이미지의 색상
        bgOriginalColor = btnBackground.color;       //백그라운드의 최초 색상.
        targetColor = new Color32(255, 144, 0, 255); //기본 타겟 컬러.
        btnBackground.enabled = false;
        isOn = false;
    }

}
