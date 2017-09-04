using System;
using UnityEngine;

// Realized after writing this that I should never need to manually populate 
// soft attribute entries. Delete soon if no other use case comes up.
[Serializable]
public class SoftAttributeEntry
{
    public SoftEntityAttributes SoftAttribute;
    public string value;
}