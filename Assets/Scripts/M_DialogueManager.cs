using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class M_DialogueManager : MonoBehaviour
{
    Dictionary<int, string> _testDictionary = new Dictionary<int, string>();  // Stored in pairs of Key & Value
    void Test()
    {
        // Adding elements
        _testDictionary.Add(1, "C#");
        _testDictionary.Add(2, "Javascript");
        _testDictionary.Add(3, "Dart");

        // Displaying dictionary
        foreach (var ele in _testDictionary)
        {
            Console.WriteLine($"Key: {ele.Key}, Value: {ele.Value}");
        }

        if (_testDictionary.ContainsKey(0))
        {
            Debug.Log("Key found");
        }
        else
        {
            Debug.Log("Error: Key not found.");
        }
    }

    // Enter Dialogue
    // Open Dialogue Box UI
}
