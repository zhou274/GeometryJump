﻿




using UnityEngine;
using System.Collections;

/// <summary>
/// Class in charge to shake the camera when the player failed
/// </summary>
namespace AppAdvisory.GeometryJump
{
	public class CameraShake : MonoBehaviour
	{

		private static Vector3 originPosition;
		private static Quaternion originRotation;

		private static float shakeDecay = 0.002f;
		private static float shakeIntensity;

		public static IEnumerator Shake(Transform t){
			originPosition = t.position;
			originRotation = t.rotation;
			shakeIntensity = 0.3f;
			while (shakeIntensity > 0) {
				t.position = originPosition + Random.insideUnitSphere * shakeIntensity;
				t.rotation = new Quaternion (
					originRotation.x + Random.Range (-shakeIntensity, shakeIntensity) * .2f,
					originRotation.y + Random.Range (-shakeIntensity, shakeIntensity) * .2f,
					originRotation.z + Random.Range (-shakeIntensity, shakeIntensity) * .2f,
					originRotation.w + Random.Range (-shakeIntensity, shakeIntensity) * .2f);
				shakeIntensity -= shakeDecay;
				yield return false;
			}
		}

		public static IEnumerator Shake(Transform t, float i){
			originPosition = t.position;
			originRotation = t.rotation;
			shakeIntensity = i;
			while (shakeIntensity > 0) {
				t.position = originPosition + Random.insideUnitSphere * shakeIntensity;
				t.rotation = new Quaternion (
					originRotation.x + Random.Range (-shakeIntensity, shakeIntensity) * .2f,
					originRotation.y + Random.Range (-shakeIntensity, shakeIntensity) * .2f,
					originRotation.z + Random.Range (-shakeIntensity, shakeIntensity) * .2f,
					originRotation.w + Random.Range (-shakeIntensity, shakeIntensity) * .2f);
				shakeIntensity -= shakeDecay;
				yield return false;
			}
		}
	}
}