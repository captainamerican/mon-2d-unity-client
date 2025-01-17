﻿using UnityEngine;

public static class Easing {
	public static float Linear(float start, float end, float time) {
		return Mathf.Lerp(start, end, time);
	}

	public static float Linear01(float time) {
		return time;
	}

	public static float Spring(float start, float end, float value) {
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
		return start + (end - start) * value;
	}

	public static float QuadIn(float start, float end, float value) {
		end -= start;
		return end * value * value + start;
	}

	public static float QuadOut(float start, float end, float value) {
		end -= start;
		return -end * value * (value - 2) + start;
	}

	public static float QuadInOut(float start, float end, float value) {
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * value * value + start;
		value--;
		return -end * 0.5f * (value * (value - 2) - 1) + start;
	}

	public static float CubicIn(float start, float end, float value) {
		end -= start;
		return end * value * value * value + start;
	}

	public static float CubicOut(float start, float end, float value) {
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	public static float CubicInOut(float start, float end, float value) {
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value + 2) + start;
	}

	public static float QuartIn(float start, float end, float value) {
		end -= start;
		return end * value * value * value * value + start;
	}

	public static float QuartOut(float start, float end, float value) {
		value--;
		end -= start;
		return -end * (value * value * value * value - 1) + start;
	}

	public static float QuartInOut(float start, float end, float value) {
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * value * value * value * value + start;
		value -= 2;
		return -end * 0.5f * (value * value * value * value - 2) + start;
	}

	public static float QuintIn(float start, float end, float value) {
		end -= start;
		return end * value * value * value * value * value + start;
	}

	public static float QuintOut(float start, float end, float value) {
		value--;
		end -= start;
		return end * (value * value * value * value * value + 1) + start;
	}

	public static float QuintInOut(float start, float end, float value) {
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * value * value * value * value * value + start;
		value -= 2;
		return end * 0.5f * (value * value * value * value * value + 2) + start;
	}

	public static float SineIn(float start, float end, float value) {
		end -= start;
		return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
	}

	public static float SineIn01(float time) {
		return (-1f * Mathf.Cos(time * (Mathf.PI * 0.5f))) + 1;
	}

	public static float SineOut(float start, float end, float value) {
		end -= start;
		return (end * Mathf.Sin(value * (Mathf.PI * 0.5f))) + start;
	}

	public static float SineOut01(float time) {
		return Mathf.Sin(time * (Mathf.PI * 0.5f));
	}

	public static float SineInOut(float start, float end, float value) {
		end -= start;
		return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
	}

	public static float SineInOut01(float time) {
		return -1 * 0.5f * (Mathf.Cos(Mathf.PI * time) - 1) + 0;
	}

	public static float ExpoIn(float start, float end, float value) {
		end -= start;
		return end * Mathf.Pow(2, 10 * (value - 1)) + start;
	}

	public static float ExpoOut(float start, float end, float value) {
		end -= start;
		return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
	}

	public static float ExpoInOut(float start, float end, float value) {
		value /= .5f;
		end -= start;
		if (value < 1)
			return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}

	public static float CircIn(float start, float end, float value) {
		end -= start;
		return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
	}

	public static float CircOut(float start, float end, float value) {
		value--;
		end -= start;
		return end * Mathf.Sqrt(1 - value * value) + start;
	}

	public static float CircInOut(float start, float end, float value) {
		value /= .5f;
		end -= start;
		if (value < 1)
			return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}

	public static float BounceIn(float start, float end, float value) {
		end -= start;
		float d = 1f;
		return end - BounceOut(0, end, d - value) + start;
	}

	public static float BounceOut(float start, float end, float value) {
		value /= 1f;
		end -= start;
		if (value < (1 / 2.75f)) {
			return end * (7.5625f * value * value) + start;
		} else if (value < (2 / 2.75f)) {
			value -= (1.5f / 2.75f);
			return end * (7.5625f * (value) * value + .75f) + start;
		} else if (value < (2.5 / 2.75)) {
			value -= (2.25f / 2.75f);
			return end * (7.5625f * (value) * value + .9375f) + start;
		} else {
			value -= (2.625f / 2.75f);
			return end * (7.5625f * (value) * value + .984375f) + start;
		}
	}

	public static float BounceInOut(float start, float end, float value) {
		end -= start;
		float d = 1f;
		if (value < d * 0.5f)
			return BounceIn(0, end, value * 2) * 0.5f + start;
		else
			return BounceOut(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
	}

	public static float BackIn(float start, float end, float value) {
		end -= start;
		value /= 1;
		float s = 1.70158f;
		return end * (value) * value * ((s + 1) * value - s) + start;
	}

	public static float BackOut(float start, float end, float value) {
		float s = 1.70158f;
		end -= start;
		value = (value) - 1;
		return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}

	public static float BackInOut(float start, float end, float value) {
		float s = 1.70158f;
		end -= start;
		value /= .5f;
		if ((value) < 1) {
			s *= (1.525f);
			return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
		}
		value -= 2;
		s *= (1.525f);
		return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
	}

	public static float ElasticIn(float start, float end, float value) {
		end -= start;

		float d = 1f;
		float p = d * .3f;
		float s;
		float a = 0;

		if (value == 0)
			return start;

		if ((value /= d) == 1)
			return start + end;

		if (a == 0f || a < Mathf.Abs(end)) {
			a = end;
			s = p / 4;
		} else {
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}

		return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
	}

	public static float ElasticOut(float start, float end, float value) {
		end -= start;

		float d = 1f;
		float p = d * .3f;
		float s;
		float a = 0;

		if (value == 0)
			return start;

		if ((value /= d) == 1)
			return start + end;

		if (a == 0f || a < Mathf.Abs(end)) {
			a = end;
			s = p * 0.25f;
		} else {
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}

		return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
	}

	public static float ElasticInOut(float start, float end, float value) {
		end -= start;

		float d = 1f;
		float p = d * .3f;
		float s;
		float a = 0;

		if (value == 0)
			return start;

		if ((value /= d * 0.5f) == 2)
			return start + end;

		if (a == 0f || a < Mathf.Abs(end)) {
			a = end;
			s = p / 4;
		} else {
			s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
		}

		if (value < 1)
			return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
		return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
	}
}
