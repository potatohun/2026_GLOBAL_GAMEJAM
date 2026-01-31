using NUnit.Framework;
using System;
using System.Drawing;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum CursorType
{
    Hand,
    Knife,
    Brush,
    BigBrush,
}
public class CursorManager : MonoBehaviour
{
    private static CursorManager instance;
    public GameObject[] _cursor;
    public CursorType _cursorType;

    [Header("UI Cursor (optional)")]
    public RectTransform cursorRect;

    public Image pen_tip_1;
    public Image pen_tip_2;
    public ParticleSystem pen_Particle;

    void Awake()
    {
        // 🔒 중복 생성 방지
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // ⭐ 씬 유지 핵심
        EquipTool(_cursorType);
    }

    void Start()
    {
        Cursor.visible = false; // UI 커서 쓰면 OS 커서 숨김
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (cursorRect == null) return;
        if (Mouse.current == null) return;

        cursorRect.position = Mouse.current.position.ReadValue();
    }

    void OnDestroy()
    {
        // 혹시 앱 종료/강제 제거 시 원복
        if (instance == this)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SetColor(UnityEngine.Color color)
    {
        pen_tip_1.color = color;
        pen_tip_2.color = color;
        var main = pen_Particle.main;
        main.startColor = color;
    }

    public void initial_Color()
    {
        pen_tip_1.color = UnityEngine.Color.white;
        pen_tip_2.color = UnityEngine.Color.white;
        var main = pen_Particle.main;
        main.startColor = UnityEngine.Color.white;
    }
    public void EquipTool(CursorType type)
    {
        int index = type.GetHashCode();
        for (int i = 0; i < _cursor.Length; i++)
        {
             _cursor[i].SetActive(i == index);
            if(i == index)
            {
                cursorRect = _cursor[i].GetComponent<RectTransform>();
            }
            if(type == CursorType.Brush)
            {
                initial_Color();
            }
            if (type == CursorType.BigBrush)
            {
                initial_Color();
            }


        }
    }
}
