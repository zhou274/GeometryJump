﻿




using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Class in charge of the score animation at start
/// </summary>
namespace AppAdvisory.GeometryJump
{
	public class AnimationScore : MonoBehaviour 
	{
		public float delay = 0;

		void Start()
		{
			CanvasManager.OnAnimationTransitionOutStart += DoAnimIn;
		}

		void OnDisable()
		{
			CanvasManager.OnAnimationTransitionOutStart -= DoAnimIn;
		}

		public void DoAnimIn()
		{
			CanvasManager.OnAnimationTransitionOutStart -= DoAnimIn;

			int mult = 1;

			var r = GetComponent<RectTransform>();
			if(r == null)
				return;
			var pos = r.anchoredPosition;

			float yOrigin = pos.y;

			pos.y= mult *  2 * Screen.height;
			mult *= -1;
			r.anchoredPosition = pos;
			r.DOAnchorPosY(yOrigin, 1)
				.SetDelay(delay)
				.SetEase(Ease.OutBack,0.6f,1);

		}
	}
}