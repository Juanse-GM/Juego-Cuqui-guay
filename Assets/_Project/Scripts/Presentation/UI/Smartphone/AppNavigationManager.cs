using UnityEngine;

namespace DatingSim.Presentation.UI
{
    /// <summary>
    /// Navegación básica entre pantallas del teléfono (Home / Chat).
    /// </summary>
    public class AppNavigationManager : MonoBehaviour
    {
        [SerializeField] private GameObject homeScreen;
        [SerializeField] private GameObject chatScreen;

        public void ShowHome()
        {
            if (homeScreen != null) homeScreen.SetActive(true);
            if (chatScreen != null) chatScreen.SetActive(false);
        }

        public void ShowChat()
        {
            if (homeScreen != null) homeScreen.SetActive(false);
            if (chatScreen != null) chatScreen.SetActive(true);
        }
    }
}
