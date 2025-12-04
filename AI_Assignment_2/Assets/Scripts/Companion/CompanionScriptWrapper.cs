using UnityEngine;
/// <summary>
/// Allows a CompanionScript to be put into Serialized Field to then be returned as object of type ICompanion with method GetCompanion()
/// </summary>
public class CompanionScriptWrapper : MonoBehaviour
{
    [SerializeField] CompanionScript companionScript;
    public ICompanion GetCompanion { get {  return (ICompanion)companionScript; } }
}
