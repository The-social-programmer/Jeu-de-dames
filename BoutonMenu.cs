using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoutonMenu : MonoBehaviour
{
    public GameObject avertissement;
    public void RetourAuMenu(GameObject bouton)
    {
        switch (bouton.tag)
        {
            case "home button":
                avertissement.SetActive(true);
                break;
            case "quitter":
                SceneManager.LoadScene("Menu");
                break;
            case "continuer":
                avertissement.SetActive(false);
                break;
        }
    }
}
