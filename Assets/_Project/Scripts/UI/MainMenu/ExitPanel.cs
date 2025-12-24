using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPanel : MonoBehaviour
{
    public void OnClickCancel()
    {
        gameObject.SetActive(false);
    }
    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 실행 종료
#else
    Application.Quit(); // 빌드된 게임 종료
#endif
    }
}
