using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class BaseUIManager : MonoBehaviour
{
    // Some Manager needs MinoUI arr, or just one instance.

    void OnEnable()
    {
        UIManagerEnable();
    }

    void OnDisable()
    {
        UIManagerDisable();
    }

    public abstract void UIManagerEnable();
    public abstract void UIManagerDisable();
}
