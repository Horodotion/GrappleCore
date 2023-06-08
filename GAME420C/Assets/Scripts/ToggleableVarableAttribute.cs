using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// Conditionally show or hide field in inspector, based on another variable

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ToggleableVarableAttribute : PropertyAttribute
{
    //The name of the field used to decide to show or hide
    public string ConditionalVariablefield = "";
    //Whether or not to hide in inspector
    public bool HideIfTrue = false;

    public ToggleableVarableAttribute(string ConditionalVariablefield)
    {
        this.ConditionalVariablefield = ConditionalVariablefield;
        this.HideIfTrue = false;
    }

    public ToggleableVarableAttribute(string ConditionalVariablefield, bool HideIfTrue)
    {
        this.ConditionalVariablefield = ConditionalVariablefield;
        this.HideIfTrue = HideIfTrue;
    }
}
