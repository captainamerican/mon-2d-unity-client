using System;
using System.Collections;

using UnityEngine;

public sealed class Go {
	static readonly WaitForEndOfFrame waitForEndOfFrame = new();

	static public IEnumerator To(Transform transform, Vector3 to, float duration, Func<float, float> easing = null) {
		easing ??= Easing.Linear01;

		//
		Vector3 from = transform.position;
		transform.position = Vector3.Lerp(from, to, 0);

		//
		float elapsed = 0;
		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			transform.position = Vector3.Lerp(from, to, elapsed / duration);
			yield return waitForEndOfFrame;
		}

		//
		transform.position = Vector3.Lerp(from, to, 1);
	}

	static public IEnumerator ToReal(Transform transform, Vector3 to, float duration, Func<float, float> easing = null) {
		easing ??= Easing.Linear01;

		//
		Vector3 from = transform.position;
		transform.position = Vector3.Lerp(from, to, 0);

		//
		float elapsed = 0;
		while (elapsed < duration) {
			elapsed += Time.unscaledDeltaTime;
			transform.position = Vector3.Lerp(from, to, elapsed / duration);
			yield return waitForEndOfFrame;
		}

		//
		transform.position = Vector3.Lerp(from, to, 1);
	}
}