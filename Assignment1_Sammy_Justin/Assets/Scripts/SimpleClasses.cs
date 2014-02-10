using UnityEngine;
using System.Collections;

// Class that allows for a SendMessage function to "return" a string
// From another script
public class StringWrapper {
	public string output = "";
	public StringWrapper() {
	}
}

// Class for generic pair data structure
public class Pair<T, U> {
	public Pair() {
	}
	public Pair(T first, U second) {
		this.First = first;
		this.Second = second;
	}
	public T First { get; set; }
	public U Second { get; set; }
};