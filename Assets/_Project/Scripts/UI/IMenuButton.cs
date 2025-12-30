using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IMenuButton : IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    void InitializeColor();
    void Refresh();
    void Highlight();
    void ButtonClick();
    bool isOn { get; }
}
