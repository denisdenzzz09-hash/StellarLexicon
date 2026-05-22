using UnityEngine;

public class LoadButton : MonoBehaviour
{
    public void OnContinueClicked()
    {
        LoadSystem.Instance.LoadSavedGame();
    }
}