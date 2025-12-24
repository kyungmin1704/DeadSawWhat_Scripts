using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckButton : Toggle
{
    Tween currentTween;

    Image checkbox => transform.Find("SelectedBox").GetComponentInChildren<Image>(true);

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Highlight();
    }

    private void OnToggleValueChanged(bool value)
    {
        checkbox.gameObject.SetActive(value);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        onValueChanged.AddListener(OnToggleValueChanged);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        onValueChanged.RemoveListener(OnToggleValueChanged);
    }
    public virtual void Highlight()
    {
        currentTween?.Kill();
        currentTween = DOTween.Sequence()
            .Append(targetGraphic.transform.DOScale(1.05f, 0.2f))
            .Append(targetGraphic.transform.DOScale(1f, 0.2f))
            .OnKill(() => targetGraphic.transform.localScale = Vector3.one);
    }
}
