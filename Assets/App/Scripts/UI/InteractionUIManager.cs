using System.Collections.Generic;
using UnityEngine;

public class InteractionUIManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int startingPointerCount = 3;

    [Header("References")]
    [SerializeField] PointerUI pointerUIPrefab;
    [SerializeField] Transform content;

    Queue<PointerUI> pointers = new();

    public static InteractionUIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < startingPointerCount; i++)
        {
            PointerUI pointer = CreatePointerUI();
            pointer.gameObject.SetActive(false);
            pointers.Enqueue(pointer);
        }
    }

    public PointerUI GetPointer()
    {
        if (pointers.Count <= 0)
        {
            return CreatePointerUI();
        }

        PointerUI pointer = pointers.Dequeue();
        pointer.gameObject.SetActive(true);
        return pointer;
    }

    public void ReturnPointer(PointerUI pointer)
    {
        pointer.gameObject.SetActive(false);
        pointers.Enqueue(pointer);
    }

    public PointerUI CreatePointerUI()
    {
        return Instantiate(pointerUIPrefab, content);
    }
}
