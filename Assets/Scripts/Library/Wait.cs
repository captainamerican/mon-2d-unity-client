using System;
using System.Collections;

using UnityEngine;

public sealed class Wait {
	static readonly WaitForEndOfFrame waitForEndOfFrame = new();

	static public IEnumerator Until(Func<bool> callback) {
		while (!callback()) {
			yield return waitForEndOfFrame;
		}
	}

	static public IEnumerator For(float seconds) {
		yield return new WaitForSeconds(seconds);
	}

	static public IEnumerator ForReal(float seconds) {
		yield return new WaitForSecondsRealtime(seconds);
	}

	static public IEnumerator AndThen(float seconds, Action callback) {
		yield return For(seconds);
		callback();
	}

	static public IEnumerator AndThenReal(float seconds, Action callback) {
		yield return ForReal(seconds);
		callback();
	}
}