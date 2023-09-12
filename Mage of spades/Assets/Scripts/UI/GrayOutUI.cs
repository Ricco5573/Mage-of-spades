using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GrayOutUI : MonoBehaviour
{
	[SerializeField] private Image fillImage;
	private Color startingColor;
	private Color gray = Color.gray;

	private void Start()
	{
		startingColor = fillImage.color;
	}

	public void GrayOutFill()
	{
		fillImage.color = Color.gray;
	}
	public void ResetFillColor()
	{
		fillImage.color = startingColor;
	}

	public IEnumerator TimedColoredFill(float timeInSeconds, Color color)
	{
		fillImage.color = color;
		yield return new WaitForSeconds(timeInSeconds);
		fillImage.color = startingColor;
	}
}
