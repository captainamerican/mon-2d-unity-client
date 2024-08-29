using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public sealed class Do {
	static readonly WaitForEndOfFrame waitForEndOfFrame = new();

	static public IEnumerator For(float duration, Action<float> callback, Func<float, float> easing = null) {
		easing ??= Easing.Linear01;

		//
		callback(easing(0));

		//
		float elapsed = 0;
		while (elapsed < duration) {
			elapsed += Time.deltaTime;
			callback(easing(Mathf.Clamp01(elapsed / duration)));

			//
			yield return waitForEndOfFrame;
		}

		//
		callback(easing(1));
	}

	static public IEnumerator ForReal(float duration, Action<float> callback, Func<float, float> easing = null) {
		easing ??= Easing.Linear01;

		//
		callback(easing(0));

		//
		float elapsed = 0;
		while (elapsed < duration) {
			elapsed += Time.unscaledDeltaTime;
			callback(easing(Mathf.Clamp01(elapsed / duration)));

			//
			yield return waitForEndOfFrame;
		}

		//
		callback(easing(1));
	}

	static public void Times(int times, Action callback) {
		for (int i = 0; i < times; i++) {
			callback();
		}
	}

	static public void Times(int times, Action<int> callback) {
		for (int i = 0; i < times; i++) {
			callback(i);
		}
	}

	static public List<T> Times<T>(int times, Func<int, T> callback) {
		List<T> returnValue = new();

		for (int i = 0; i < times; i++) {
			returnValue.Add(callback(i));
		}

		//
		return returnValue;
	}

	static public List<T> Times<T>(int times, Func<T> callback) {
		List<T> returnValue = new();

		for (int i = 0; i < times; i++) {
			returnValue.Add(callback());
		}

		//
		return returnValue;
	}

	static public IEnumerator ForEach<T>(IEnumerable<T> enumerable, Func<T, IEnumerator> callback) {
		foreach (T item in enumerable) {
			yield return callback(item);
		}
	}

	static public IEnumerator ForEach<T>(IEnumerable<T> enumerable, Func<T, int, int, IEnumerator> callback) {
		int total = enumerable.Count();
		int index = 0;
		foreach (T item in enumerable) {
			yield return callback(item, index, total);
			index += 1;
		}
	}

	static public IEnumerator ForEach<T>(IEnumerable<T> enumerable, Func<T, int, IEnumerator> callback) {
		int index = 0;
		foreach (T item in enumerable) {
			yield return callback(item, index);
			index += 1;
		}
	}

	static public void ForEach<T>(IEnumerable<T> enumerable, Action<T> callback) {
		foreach (T item in enumerable) {
			callback(item);
		}
	}

	static public void ForEach<T>(IEnumerable<T> enumerable, Action<T, int, int> callback) {
		int total = enumerable.Count();
		int index = 0;
		foreach (T item in enumerable) {
			callback(item, index, total);
			index += 1;
		}
	}

	static public void ForEach<T>(IEnumerable<T> enumerable, Action<T, int> callback) {
		int index = 0;
		foreach (T item in enumerable) {
			callback(item, index);
			index += 1;
		}
	}

	static public IEnumerator After(float duration, Action callback) {
		yield return Wait.For(duration);
		callback();
	}

	static public IEnumerator AfterReal(float duration, Action callback) {
		yield return Wait.ForReal(duration);
		callback();
	}
}