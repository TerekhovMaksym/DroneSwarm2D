using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneCountController : MonoBehaviour
{
    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private TMP_Text text;
    [SerializeField] private int DroneCount = 5;
    [SerializeField] private string DroneText = "Drones count: {x}";

    public static Action<int> OnDroneCountChanged;

    void Start()
    {
        plusButton.onClick.AddListener(() => ChangeDroneCount(DroneCount + 1));
        minusButton.onClick.AddListener(() => ChangeDroneCount(DroneCount - 1));
        ChangeDroneCount(DroneCount);
    }

    void ChangeDroneCount(int count)
    {
        DroneCount = count;
        text.text = DroneText.Replace("{x}", count.ToString());
        OnDroneCountChanged?.Invoke(count);
    }
}