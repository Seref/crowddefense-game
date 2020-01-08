using System;
using UnityEngine;
using UnityEngine.UI;

public class SliderItem : MonoBehaviour
{
	public string Title;
	public string Type;
	public int Max;
	public int Min;
	public int currentValue;
	public Action<int> onValueChanged;

	public TMPro.TextMeshProUGUI MaxText;
	public TMPro.TextMeshProUGUI MinText;
	public TMPro.TextMeshProUGUI CurrentText;
	public TMPro.TextMeshProUGUI TitleText;

	public Slider slider;
	void Start()
	{
		TitleText.text = Title;		
		slider.minValue = Min;
		slider.maxValue = Max;
		MaxText.text = Max + "" + Type;
		MinText.text = Min + "" + Type;
		slider.onValueChanged.AddListener((a) => { CurrentText.text = "" + a + Type; currentValue = 0; onValueChanged?.Invoke((int) a); });
	}

	public void SetValue(int a) {
		slider.value = a;
		currentValue = a;
		CurrentText.text = "" + a + Type;
	}

}
