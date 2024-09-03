using NanoidDotNet;

namespace Game {
	static public class Id {
		static readonly string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		static public string Generate() {
			return Nanoid.Generate(alphabet, 16);
		}
	}
}
