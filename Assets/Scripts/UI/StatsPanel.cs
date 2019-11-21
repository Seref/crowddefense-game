using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{

	public TextMeshProUGUI TemplateItem;

	private readonly List<TextMeshProUGUI> items = new List<TextMeshProUGUI>();    

	public TextMeshProUGUI AddItem(){
		var newItem = Instantiate<TextMeshProUGUI>(TemplateItem, Vector3.zero, Quaternion.identity, transform);
		newItem.gameObject.SetActive(true);				
		items.Add(newItem);
		return newItem;
	}

	public void RemoveItem(TextMeshProUGUI item) {
		items.Remove(item);
		Destroy(item);
	}
	
}
