using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
   public void OnMenu()
    {
        gameObject.SetActive(true);
    }
    public void OffMenu()
    {
        gameObject.SetActive(false);

    }
}
