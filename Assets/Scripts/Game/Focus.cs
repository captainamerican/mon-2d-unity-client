using System;

using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game {
	static public class Focus {
		static public void Nothing() {
			EventSystem.current.SetSelectedGameObject(null);
		}

		static public void This(Slider slider) {
			if (slider == null) {
				throw new SystemException("Slider was null");
			}

			slider.Select();
			slider.OnSelect(null);

			//
			var informationButton = slider.GetComponent<InformationButton>();
			if (informationButton != null) {
				informationButton.OnSelect(null);
			}
		}

		static public void This(Button button) {
			if (button == null) {
				throw new SystemException("Button was null");
			}

			button.Select();
			button.OnSelect(null);

			//
			var informationButton = button.GetComponent<InformationButton>();
			if (informationButton != null) {
				informationButton.OnSelect(null);
			}
		}
	}
}
