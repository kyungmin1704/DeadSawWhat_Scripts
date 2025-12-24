using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using UnityEngine.EventSystems;

public class ValueOptionButton : OptionButtonVisual
{


    public Slider optionSlider;
    public TMP_InputField optionInput;

    float optionFloatValue;
    float integerValue;


    private void Start()
    {
        Initialize();
    }

    #region 옵션의 값이 실시간 변경에 대한 스크립트
    public void Initialize()
    {
        InitializeColor();

        if (data.ftValue != -999f && data.itgValue == -999) //float 타입의 값이 OptionButtonData에 저장되어 있다면
        {
            optionFloatValue = 2.5f; //기본값은 2.5f로 설정.

            optionFloatValue = data.ftValue;               //OptionButtonData에 저장된 값이 있다면 그 값을 사용.
            optionSlider.minValue = 0f;                             //감도조절 최소값.
            optionSlider.maxValue = 100f;                             //감도조절 최대값
            optionSlider.value = optionFloatValue; //시작시 최초 감도

            optionInput.text = optionSlider.value.ToString("0.0");    //감도조절 텍스트 2자리까지.

        }
        else if (data.itgValue != -999 && data.ftValue == -999f) //int 타입의 값이 OptionButtonData에 저장되어 있다면
        {
            integerValue = 100; //기본값은 100으로 설정.

            integerValue = data.itgValue;                    //OptionButtonData에 저장된 값이 있다면 그 값을 사용.
            optionSlider.minValue = (int)0;                             //감도조절 최소값.
            optionSlider.maxValue = (int)100;                             //감도조절 최대값
            optionSlider.value = integerValue;
            optionInput.text = optionSlider.value.ToString();
        }


        optionInput.onEndEdit.AddListener(OnInputChanged);          //텍스트로 감도조절시 입력끝났을 때
        optionSlider.onValueChanged.AddListener(OnSliderChanged);   //슬라이더로 감도조절시 실시간.
    }


    public void OnInputChanged(string input)
    {
        if (data.ftValue != -999f && data.itgValue == -999)
        {
            if (float.TryParse(input, out float value))
            {
                optionFloatValue = Mathf.Clamp(value, 0.001f, 100f);
                data.ftValue = optionFloatValue;

                if (!Mathf.Approximately(optionSlider.value, optionFloatValue))
                    optionSlider.value = optionFloatValue;


                optionInput.text = optionFloatValue.ToString("0.0");
            }
        }
        else if (data.itgValue != -999 && data.ftValue == -999f) //int 타입의 값이 OptionButtonData에 저장되어 있다면
        {
            if (int.TryParse(input, out int value))
            {
                integerValue = Mathf.Clamp(value, 0, 100);

                if (!Mathf.Approximately(optionSlider.value, integerValue))
                    optionSlider.value = integerValue;


                optionInput.text = integerValue.ToString();
            }
        }

    }
    public void OnSliderChanged(float value)
    {
        if (data.ftValue != -999f && data.itgValue == -999)
        {
            optionSlider.value = value;
            data.ftValue = optionSlider.value;
            optionInput.text = value.ToString("0.0");
        }
        else if (data.ftValue == -999f && data.itgValue != -999)
        {
            optionSlider.value = value;
            optionInput.text = value.ToString("0");
        }
    }
    #endregion

}
