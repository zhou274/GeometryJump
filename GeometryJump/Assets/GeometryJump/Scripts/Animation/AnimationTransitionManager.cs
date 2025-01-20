




using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Class to managed transition animations
/// </summary>
namespace AppAdvisory.GeometryJump
{
	public class AnimationTransitionManager : MonoBehaviour 
	{
		public AnimationTransition animationTransition;

		void Awake()
		{
			animationTransition.gameObject.SetActive(true);
		}

		void Start()
		{
			animationTransition.gameObject.SetActive(true);
			animationTransition.DoAnimationOut(null);
		}

		public void DoAnimIn(Action callback)
		{
			animationTransition.gameObject.SetActive(true);
			animationTransition.DoAnimationIn(callback);
		}

	}
}