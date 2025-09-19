using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputDataType
{ 
    SingleLine,
    SingleLineClean, // single line, but each line is cleaned (toLower, removed special chars, etc)
    MultiLine
}
