using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleUI : MonoBehaviour
{
    CanvasScaler m_scaler;

    // Start is called before the first frame update
    void Start()
    {
        m_scaler = GetComponent<CanvasScaler>();    
    }

    // Update is called once per frame
    void Update()
    {
        float borderY = Screen.height - 500.0f;
        float borderX = Screen.width - 500.0f;
        if (borderY < borderX)
            m_scaler.matchWidthOrHeight = 1.0f;
        else
            m_scaler.matchWidthOrHeight = 0.0f;
    }
}
