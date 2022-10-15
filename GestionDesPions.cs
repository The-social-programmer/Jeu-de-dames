using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GestionDesPions : MonoBehaviour
{
    public GameObject[] buttons;

    public GameObject ordinateurGagne;
    public GameObject joueurGagne;
    
    private bool ordinateur;

    private GameObject boutonAnterieur;

    private float[,] pionsPositions; // [x, 0] : position x, [x, 1] : position y, [x, 2] : noir ou blanc (0 = noir, 1 = blanc), [x, 3] : dame ou pas (0 = pion, 1 = dame)
    private float[,] pionsPositionsSub;

    private GameObject case1;
    private RectTransform case1Position;
    private int compte;

    private RectTransform canvas;

    public GameObject[] pion;
    public GameObject[] dames;

    private GameObject[] pions;
    private GameObject[] pionsEnnemis;

    private List<List<GameObject>> pionEnnemiDetruire;
    private GameObject superBouton;
    private GameObject superBouton2;
    private List<List<GameObject>> superBoutons;
    private List<GameObject> dameSuperBoutons;
    private int compteur;
    private int compteurDame;
    private bool deuxiemeTour;
    private int compteur2;
    private bool manger;
    private List<GameObject> desactiver;
    private List<GameObject> desactiverDame;
    private int verifierIncrementation1;
    private int verifierIncrementation2;
    private int dameSoustraire;
    List<List<GameObject>> superBoutonsFin;
    private bool passerProchaineCase;

    private bool dame;

    private List<List<GameObject>> pionsAManger;
    private List<List<GameObject>> pionsAMangerOrdinateur;
    private List<GameObject> list;
    private bool premiereCase;

    public static int PROFONDEUR = -1;

    private int compteurProfondeur;
    private int compteurHeuristique;
    private int compteurHeuristiqueInactif;
    private List<Mouvement> mouvements;
    private List<Mouvement> nePasAcceder;
    private Mouvement leAncetre;
    private bool ennemiEnVue;
    private List<GameObject> reInitialiser;
    private Mouvement ancienMouvement;
    private List<Mouvement> desactiverAncetres;
    private bool profondeurChange;
    private List<int> pionsPositionsIndice;

    private GameObject resteBlanc;
    private GameObject resteNoir;

    private int tourCompteur;
    private int tour; // si tour == 2, ordinateur a les pions blancs, si tour == 3, joueur a les pions blancs

    void Start()
    {
        case1 = GameObject.FindWithTag("case1");
        case1Position = case1.GetComponent<RectTransform>();

        tour = 1;
        tourCompteur = 1;
        compte = 0;

        pionsPositions = new float[40, 4];
        pionsPositionsSub = new float[40, 4];

        pions = new GameObject[20];
        pionsEnnemis = new GameObject[20];
        pionEnnemiDetruire = new List<List<GameObject>>();
        pionsAManger = new List<List<GameObject>>();
        pionsAMangerOrdinateur = new List<List<GameObject>>();
        desactiver = new List<GameObject>();
        desactiverDame = new List<GameObject>();
        reInitialiser = new List<GameObject>();
        desactiverAncetres = new List<Mouvement>();
        pionsPositionsIndice = new List<int>();
        superBoutons = new List<List<GameObject>>();
        dameSuperBoutons = new List<GameObject>();
        superBoutonsFin = new List<List<GameObject>>();
        compteurHeuristique = 1;

        canvas = GameObject.FindWithTag("canvas").GetComponent<RectTransform>();

        pion[0].GetComponent<RectTransform>().anchoredPosition = case1Position.anchoredPosition;
        pion[1].GetComponent<RectTransform>().anchoredPosition = case1Position.anchoredPosition;

        mouvements = new List<Mouvement>();
        compteurProfondeur = 1;
        nePasAcceder = new List<Mouvement>();
        passerProchaineCase = true;

        resteBlanc = GameObject.FindGameObjectWithTag("pieces restantes blanc");
        resteNoir = GameObject.FindGameObjectWithTag("pieces restantes noir");

        CreerDesPions(0, 0, 0);
        CreerDesPions(150, 150, 0);
        CreerDesPions(0, 300, 0);
        CreerDesPions(150, 450, 0);

        CreerDesPions(0, 900, 1);
        CreerDesPions(150, 1050, 1);
        CreerDesPions(0, 1200, 1);
        CreerDesPions(150, 1350, 1);

        Verification();

        if(PROFONDEUR != 0)
        {
            System.Random random = new System.Random();
            int commencement = random.Next(1, 3);
            if (commencement == 1)
            {
                ordinateur = false;
                ReActiver("pion noir", false);
                tour = 3;
            }
            else if(commencement == 2)
            {
                ReActiver("pion blanc", false);
                tour = 2;
                BougerLePionOrdinateur();
                ordinateur = false;
            }
        }
        else
        {
            tour = 3;
            ReActiver("pion noir", false);
        }
    }
    void Update()
    {
        if(PROFONDEUR == -1)
        {
            return;
        }
        else if (ordinateur && PROFONDEUR != 0)
        {
            BougerLePionOrdinateur();
            ordinateur = false;
        }
    }
    void CreerDesPions(float x, float y, int numero)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                pion[numero].GetComponent<RectTransform>().anchoredPosition = new Vector2(pion[numero].GetComponent<RectTransform>().anchoredPosition.x + x, pion[numero].GetComponent<RectTransform>().anchoredPosition.y + y);
            }
            else
            {
                pion[numero].GetComponent<RectTransform>().anchoredPosition = new Vector2(pion[numero].GetComponent<RectTransform>().anchoredPosition.x + 300, pion[numero].GetComponent<RectTransform>().anchoredPosition.y);
            }
            Instantiate(pion[numero], canvas);
            pionsPositions[compte, 0] = pion[numero].GetComponent<RectTransform>().anchoredPosition.x;
            pionsPositions[compte, 1] = pion[numero].GetComponent<RectTransform>().anchoredPosition.y;
            pionsPositions[compte, 2] = numero;
            pionsPositions[compte, 3] = 0;

            compte++;
        }
        pion[numero].GetComponent<RectTransform>().anchoredPosition = case1Position.anchoredPosition;
    }
    void Verification()
    {
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                if (buttons[i].GetComponent<RectTransform>().anchoredPosition.x == pionsPositions[j, 0] && buttons[i].GetComponent<RectTransform>().anchoredPosition.y == pionsPositions[j, 1])
                {
                    break;
                }
                else if (j == 39)
                {
                    buttons[i].GetComponent<Button>().interactable = false;
                }
            }
        }
        DesactiverManger();
        if (pionsAManger.Count != 0)
        {
            pionsAManger.Sort((a, b) => b.Count.CompareTo(a.Count));
            for (int i = 0; i < 50; i++)
            {
                bool desactiver = true;
                for (int j = 0; j < pionsAManger.Count; j++)
                {
                    if (pionsAManger[j].Count == pionsAManger[0].Count && pionsAManger[j].Contains(buttons[i]))
                    {
                        desactiver = false;
                    }
                }
                if(desactiver)
                    buttons[i].GetComponent<Button>().interactable = false;
            }
        }
        int compteur = 0;
        for (int i = 0; i < 50; i++)
        {
            if (buttons[i].GetComponent<Button>().interactable == false)
            {
                compteur++;
            }
            if(compteur == 50)
            {
                PionNoirOuBlanc(3);

                foreach (GameObject pion in pions)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        if (buttons[j].GetComponent<RectTransform>().anchoredPosition.x == pion.GetComponent<RectTransform>().anchoredPosition.x && buttons[j].GetComponent<RectTransform>().anchoredPosition.y == pion.GetComponent<RectTransform>().anchoredPosition.y)
                        {
                            buttons[j].GetComponent<Button>().interactable = true;
                        }
                    }
                }
            }
        }
        pionsAManger.Clear();
    }
    // cette méthode montre les mouvements possible
    public void Mouvement()
    {
        if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>().colors.normalColor == new Color32(68, 128, 61, 255))
        {
            StartCoroutine(BougerLePionJoueur());
        }
        else
        {
            if (boutonAnterieur == EventSystem.current.currentSelectedGameObject)
            {
                return;
            }
            else if (boutonAnterieur != EventSystem.current.currentSelectedGameObject)
            {
                for (int i = 0; i < 50; i++)
                {
                    if (buttons[i].GetComponent<Button>().colors.normalColor == new Color32(81, 144, 255, 255) || buttons[i].GetComponent<Button>().colors.normalColor == new Color32(68, 128, 61, 255))
                    {
                        MettreLaCouleurBrun(buttons[i], false);
                    }
                }
            }
            boutonAnterieur = EventSystem.current.currentSelectedGameObject;

            for (int i = 0; i < 40; i++)
            {
                if (EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition.x == pionsPositions[i, 0] && EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition.y == pionsPositions[i, 1])
                {
                    if (pionsPositions[i, 3] == 1)
                    {
                        dame = true;
                    }
                    else
                    {
                        dame = false;
                    }
                    if (pionsPositions[i, 2] == 0)
                    {
                        BougerLePionJoueurDecision("pion noir");
                        break;
                    }
                    else
                    {
                        BougerLePionJoueurDecision("pion blanc");
                        break;
                    }
                }
            }
        }
    }
    void MettreLaCouleurVert(GameObject bouton)
    {
        bouton.GetComponent<Button>().interactable = true;
        ColorBlock colors = bouton.GetComponent<Button>().colors;
        colors.normalColor = new Color32(68, 128, 61, 255); // vert
        colors.disabledColor = new Color32(118, 77, 27, 255); // brun
        bouton.GetComponent<Button>().colors = colors;
    }
    void MettreLaCouleurBrun(GameObject bouton, bool interactable)
    {
        bouton.GetComponent<Button>().interactable = interactable;
        ColorBlock colors = bouton.GetComponent<Button>().colors;
        colors.normalColor = new Color32(118, 77, 27, 255); // brun
        colors.disabledColor = new Color32(118, 77, 27, 255); // brun
        bouton.GetComponent<Button>().colors = colors;
    }
    void MettreLaCouleurBleu(GameObject bouton)
    {
        bouton.GetComponent<Button>().interactable = false;
        ColorBlock colors = bouton.GetComponent<Button>().colors;
        colors.normalColor = new Color32(81, 144, 255, 255); // bleu
        colors.disabledColor = new Color32(81, 144, 255, 255); // bleu
        bouton.GetComponent<Button>().colors = colors;
    }
    IEnumerator BougerLePionJoueur()
    {
        PionNoirOuBlanc(3);
        List<GameObject> pionEnnemiDetruireDef = new List<GameObject>();
        for (int i = 0; i < superBoutons.Count; i++)
        {
            if (superBoutons[i].Contains(EventSystem.current.currentSelectedGameObject) || superBoutonsFin[i].Contains(EventSystem.current.currentSelectedGameObject))
            {
                pionEnnemiDetruireDef = pionEnnemiDetruire[i];
            }
        }
        for (int i = 0; i < 40; i++)
        {
            if (boutonAnterieur.GetComponent<RectTransform>().anchoredPosition.x == pionsPositions[i, 0] && boutonAnterieur.GetComponent<RectTransform>().anchoredPosition.y == pionsPositions[i, 1])
            {
                float posAnterieurX = pionsPositions[i, 0];
                float posAnterieurY = pionsPositions[i, 1];
                pionsPositions[i, 0] = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition.x;
                pionsPositions[i, 1] = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchoredPosition.y;
                float nouvellePositionX = pionsPositions[i, 0];
                float nouvellePositionY = pionsPositions[i, 1];
                for (int j = 0; j < 20; j++)
                {
                    if (pions[j].GetComponent<RectTransform>().anchoredPosition.x == posAnterieurX && pions[j].GetComponent<RectTransform>().anchoredPosition.y == posAnterieurY)
                    {
                        if (pionEnnemiDetruireDef.Count != 0)
                        {
                            for (int k = 0; k < pionEnnemiDetruireDef.Count; k++)
                            {
                                for (int l = 0; l < 40; l++)
                                {
                                    if (pionEnnemiDetruireDef[k].GetComponent<RectTransform>().anchoredPosition.x == pionsPositions[l, 0] && pionEnnemiDetruireDef[k].GetComponent<RectTransform>().anchoredPosition.y == pionsPositions[l, 1])
                                    {
                                        pionsPositions[l, 0] = -1;
                                        pionsPositions[l, 1] = -1;
                                        if (pionEnnemiDetruireDef[k].CompareTag("pion noir"))
                                        {
                                            int nombre = int.Parse(resteNoir.GetComponent<Text>().text) - 1;
                                            resteNoir.GetComponent<Text>().text = nombre.ToString();
                                        }
                                        else
                                        {
                                            int nombre = int.Parse(resteBlanc.GetComponent<Text>().text) - 1;
                                            resteBlanc.GetComponent<Text>().text = nombre.ToString();
                                        }
                                        Destroy(pionEnnemiDetruireDef[k]);
                                    }
                                }
                            }
                        }
                        TransformerEnDame(nouvellePositionX, nouvellePositionY, pions[j], i);
                        break;
                    }
                }
                break;
            }
        }
        foreach (GameObject pion in pions)
        {
            for (int i = 0; i < 50; i++)
            {
                if (pion.GetComponent<RectTransform>().anchoredPosition.x == buttons[i].GetComponent<RectTransform>().anchoredPosition.x && pion.GetComponent<RectTransform>().anchoredPosition.y == buttons[i].GetComponent<RectTransform>().anchoredPosition.y)
                {
                    buttons[i].GetComponent<Button>().interactable = false;
                    break;
                }
            }
        }
        boutonAnterieur.GetComponent<Button>().interactable = false;

        if(PROFONDEUR == 0)
        {
            if (tourCompteur % 2 == 0)
            {
                ReActiver("pion blanc", true);
            }
            else
            {
                ReActiver("pion noir", true);
            }
        }
        tourCompteur++;

        for (int i = 0; i < 50; i++)
        {
            if (buttons[i].GetComponent<Button>().colors.normalColor == new Color32(81, 144, 255, 255) || buttons[i].GetComponent<Button>().colors.normalColor == new Color32(68, 128, 61, 255))
            {
                MettreLaCouleurBrun(buttons[i], false);
            }
        }
        yield return new WaitForSeconds(0.3f);

        if(int.Parse(resteBlanc.GetComponent<Text>().text) == 0 || int.Parse(resteNoir.GetComponent<Text>().text) == 0)
        {
            Instantiate(joueurGagne, canvas);
            for (int i = 0; i < 50; i++)
            {
                MettreLaCouleurBrun(buttons[i], false);
            }
            yield break;
        }
        ordinateur = true;
    }
    void BougerLePionOrdinateur()
    {
        if(compteurProfondeur == 1)
        {
            for (int i = 0; i < 40; i++)
            {
                pionsPositionsSub[i, 0] = pionsPositions[i, 0];
                pionsPositionsSub[i, 1] = pionsPositions[i, 1];
                pionsPositionsSub[i, 2] = pionsPositions[i, 2];
                pionsPositionsSub[i, 3] = pionsPositions[i, 3];
            }
        }
        if (tour == 3)
        {
            InitialiserPions("pion noir", "pion blanc");
        }
        else if (tour == 2)
        {
            InitialiserPions("pion blanc", "pion noir");
        }
        bool passage = true;
        if (compteurProfondeur == PROFONDEUR + 1)
        {
            superBouton = null;
            passage = false;
        }
        if (passage)
        {
            pionsAMangerOrdinateur = new List<List<GameObject>>();
        }
        bool manger = true;
        ennemiEnVue = false;
        int compteur = 0;
        compteur2 = 0;
        foreach (GameObject pion in pions)
        {
            if (compteurProfondeur == PROFONDEUR + 1)
            {
                break;
            }
            if (ennemiEnVue)
            {
                ennemiEnVue = false;
                ancienMouvement = null;
            }
            manger = true;
            superBouton = null;
            List<GameObject> listOrd = new List<GameObject>();

            if (pion.CompareTag("pion noir"))
            {
                while (manger)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (BougerLePionOrdinateurDecisif(150, 150, "pion blanc", "pion noir", buttons[i], pion, 1, 0, false, listOrd) == true)
                        {
                            break;
                        }
                        if (BougerLePionOrdinateurDecisif(-150, 150, "pion blanc", "pion noir", buttons[i], pion, 1, 0, false, listOrd) == true)
                        {
                            break;
                        }
                        if (BougerLePionOrdinateurDecisif(150, -150, "pion blanc", "pion noir", buttons[i], pion, 1, 0, true, listOrd) == true)
                        {
                            break;
                        }
                        if (BougerLePionOrdinateurDecisif(-150, -150, "pion blanc", "pion noir", buttons[i], pion, 1, 0, true, listOrd) == true)
                        {
                            break;
                        }
                    }
                    if (listOrd.Count != 0)
                        pionsAMangerOrdinateur.Add(listOrd);
                    compteur++;
                    compteur2++;
                    if (compteur2 >= 2)
                    {
                        manger = false;
                        compteurHeuristique = 1;
                    }
                }
            }
            else
            {
                while (manger)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (BougerLePionOrdinateurDecisif(-150, -150, "pion noir", "pion blanc", buttons[i], pion, 0, 1, false, listOrd) == true)
                        {
                            break;
                        }
                        if (BougerLePionOrdinateurDecisif(150, -150, "pion noir", "pion blanc", buttons[i], pion, 0, 1, false, listOrd) == true)
                        {
                            break;
                        }
                        if (BougerLePionOrdinateurDecisif(-150, 150, "pion noir", "pion blanc", buttons[i], pion, 0, 1, true, listOrd) == true)
                        {
                            break;
                        }
                        if (BougerLePionOrdinateurDecisif(150, 150, "pion noir", "pion blanc", buttons[i], pion, 0, 1, true, listOrd) == true)
                        {
                            break;
                        }
                    }
                    if (listOrd.Count != 0)
                        pionsAMangerOrdinateur.Add(listOrd);
                    compteur++;
                    compteur2++;
                    if (compteur2 >= 2)
                    {
                        manger = false;
                        compteurHeuristique = 1;
                    }
                }
            }
        }
        if (pionsAMangerOrdinateur.Count != 0 && compteurProfondeur == 1)
        {
            for (int i = 0; i < 40; i++)
            {
                pionsPositionsSub[i, 0] = pionsPositions[i, 0];
                pionsPositionsSub[i, 1] = pionsPositions[i, 1];
                pionsPositionsSub[i, 3] = pionsPositions[i, 3];
            }
            pionsAMangerOrdinateur.Sort((a, b) => b.Count.CompareTo(a.Count));
            for (int i = 0; i < 40; i++)
            {
                if (pionsAMangerOrdinateur[0][0].GetComponent<RectTransform>().anchoredPosition.x == pionsPositions[i, 0] && pionsAMangerOrdinateur[0][0].GetComponent<RectTransform>().anchoredPosition.y == pionsPositions[i, 1])
                {
                    pionsPositions[i, 0] = pionsAMangerOrdinateur[0][pionsAMangerOrdinateur[0].Count - 1].GetComponent<RectTransform>().anchoredPosition.x;
                    pionsPositions[i, 1] = pionsAMangerOrdinateur[0][pionsAMangerOrdinateur[0].Count - 1].GetComponent<RectTransform>().anchoredPosition.y;
                    float pionNouvellePositionX = pionsPositions[i, 0];
                    float pionNouvellePositionY = pionsPositions[i, 1];
                    TransformerEnDame(pionNouvellePositionX, pionNouvellePositionY, pionsAMangerOrdinateur[0][0], i);
                    foreach (GameObject o in pionsAMangerOrdinateur[0])
                    {
                        if (tour == 2 && o.tag == "pion noir")
                        { 
                            for (int j = 0; j < 40; j++)
                            {
                                if (pionsPositions[j, 0] == o.GetComponent<RectTransform>().anchoredPosition.x && pionsPositions[j, 1] == o.GetComponent<RectTransform>().anchoredPosition.y)
                                {
                                    pionsPositions[j, 0] = -1;
                                    pionsPositions[j, 1] = -1;
                                }
                            }

                            int nombre = int.Parse(resteNoir.GetComponent<Text>().text) - 1;
                            resteNoir.GetComponent<Text>().text = nombre.ToString();
                            Destroy(o);
                        }
                        else if (tour == 3 && o.tag == "pion blanc")
                        {
                            for (int j = 0; j < 40; j++)
                            {
                                if (pionsPositions[j, 0] == o.GetComponent<RectTransform>().anchoredPosition.x && pionsPositions[j, 1] == o.GetComponent<RectTransform>().anchoredPosition.y)
                                {
                                    pionsPositions[j, 0] = -1;
                                    pionsPositions[j, 1] = -1;
                                }
                            }
                            int nombre = int.Parse(resteBlanc.GetComponent<Text>().text) - 1;
                            resteBlanc.GetComponent<Text>().text = nombre.ToString();

                            Destroy(o);
                        }
                    }
                }
            }
            if (tour == 2)
                ReActiver("pion noir", true);
            else if (tour == 3)
                ReActiver("pion blanc", true);
            leAncetre = null;
            mouvements.Clear();
            if (int.Parse(resteBlanc.GetComponent<Text>().text) == 0 || int.Parse(resteNoir.GetComponent<Text>().text) == 0)
            {
                Instantiate(ordinateurGagne, canvas);
                return;
            }
            Verification();
        }
        else if (compteurProfondeur == PROFONDEUR + 1)
        {
            for (int i = 1; i < PROFONDEUR; i++)
            {
                for (int j = 0; j < mouvements.Count; j++)
                {
                    if (mouvements[j].Profondeur == PROFONDEUR - i)
                    {
                        mouvements[j].Descendants.Sort((a, b) => a.Heuristique.CompareTo(b.Heuristique));
                        if (mouvements[j].MinOuMax == "max")
                        {
                            mouvements[j].Heuristique = mouvements[j].Descendants[mouvements[j].Descendants.Count - 1].Heuristique;
                        }
                        else if (mouvements[j].MinOuMax == "min")
                        {
                            mouvements[j].Heuristique = mouvements[j].Descendants[0].Heuristique;
                        }
                    }
                }
            }
            List<Mouvement> mouvementDecision = new List<Mouvement>();

            foreach (Mouvement mouvement in mouvements)
            {
                if (mouvement.Profondeur == 1)
                {
                    mouvementDecision.Add(mouvement);
                }
                else
                {
                    break;
                }
            }
            mouvementDecision.Sort((a, b) => b.Heuristique.CompareTo(a.Heuristique));
            Mouvement mouvementDecisif = mouvementDecision[0];
            
            List<Mouvement> mouvementDecisionSub = new List<Mouvement>();
            mouvementDecisionSub.Add(mouvementDecisif);
            for (int i = 1; i < mouvementDecision.Count; i++)
            {
                if (mouvementDecision[i].Heuristique == mouvementDecision[0].Heuristique)
                {
                    mouvementDecisionSub.Add(mouvementDecision[i]);
                }
            }
            if(mouvementDecisionSub.Count > 1)
            {
                System.Random random = new System.Random();
                int indiceMouvement = random.Next(0, mouvementDecisionSub.Count);
                mouvementDecisif = mouvementDecisionSub[indiceMouvement];
            }
            
            for (int i = 0; i < 40; i++)
            {
                pionsPositionsSub[i, 0] = pionsPositions[i, 0];
                pionsPositionsSub[i, 1] = pionsPositions[i, 1];
                pionsPositionsSub[i, 3] = pionsPositions[i, 3];
            }
            for (int i = 0; i < 40; i++)
            {
                if (pionsPositions[i, 0] == mouvementDecisif.Pion.GetComponent<RectTransform>().anchoredPosition.x && pionsPositions[i, 1] == mouvementDecisif.Pion.GetComponent<RectTransform>().anchoredPosition.y)
                {
                    pionsPositions[i, 0] = mouvementDecisif.Bouton.GetComponent<RectTransform>().anchoredPosition.x;
                    pionsPositions[i, 1] = mouvementDecisif.Bouton.GetComponent<RectTransform>().anchoredPosition.y;
                    TransformerEnDame(pionsPositions[i, 0], pionsPositions[i, 1], mouvementDecisif.Pion, i);
                    break;
                }
            }
            if (tour == 2)
            {
                pions = GameObject.FindGameObjectsWithTag("pion blanc");
                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                ReActiver("pion noir", true);
               
            }
            else if (tour == 3)
            {
                pions = GameObject.FindGameObjectsWithTag("pion noir");
                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                ReActiver("pion blanc", true);
            }
            compteurProfondeur = 1;
            leAncetre = null;
            mouvements.Clear();
            nePasAcceder.Clear();
            Verification();
        }
        else
        {
            bool prochaineProfondeur = true;
            foreach (Mouvement mouvement in mouvements)
            {
                if (!nePasAcceder.Contains(mouvement) && mouvement.Profondeur == compteurProfondeur - 1)
                {
                    prochaineProfondeur = false;
                    break;
                }
            }
            if (prochaineProfondeur)
            {
                compteurProfondeur++;
                profondeurChange = true; 
            }
            BougerLePionOrdinateur();            
        }
    }
    void PionNoirOuBlanc(int tour)
    {
        if(PROFONDEUR == 0)
        {
            if(tourCompteur % 2 == 0)
            {
                pions = GameObject.FindGameObjectsWithTag("pion noir");
            }
            else
            {
                pions = GameObject.FindGameObjectsWithTag("pion blanc");
            }
            return;
        }
        if (this.tour == tour)
        {
            pions = GameObject.FindGameObjectsWithTag("pion blanc");
        }
        else
        {
            pions = GameObject.FindGameObjectsWithTag("pion noir");
        }
    }
    void ReActiver(string pionType, bool activer)
    {
        pions = GameObject.FindGameObjectsWithTag(pionType);
        foreach (GameObject pion in pions)
        {
            for (int i = 0; i < 50; i++)
            {
                if (pion.GetComponent<RectTransform>().anchoredPosition.x == buttons[i].GetComponent<RectTransform>().anchoredPosition.x && pion.GetComponent<RectTransform>().anchoredPosition.y == buttons[i].GetComponent<RectTransform>().anchoredPosition.y)
                {
                    buttons[i].GetComponent<Button>().interactable = activer;
                    break;
                }
            }
        }
    }
    void BougerLePionJoueurDecision(string pionType)
    {
        manger = true;
        compteur2 = 1;
        verifierIncrementation1 = -1;
        verifierIncrementation2 = -1;
        deuxiemeTour = false;
        superBoutons.Clear();
        dameSuperBoutons.Clear();
        superBouton = null;
        compteur = 0;
        compteurDame = 0;
        pionEnnemiDetruire.Clear();
        desactiver.Clear();
        desactiverDame.Clear();
        superBoutonsFin.Clear();
        passerProchaineCase = true;
        dameSoustraire = 1;

        superBoutons.Add(new List<GameObject>()); superBoutons.Add(new List<GameObject>()); superBoutons.Add(new List<GameObject>()); superBoutons.Add(new List<GameObject>()); superBoutons.Add(new List<GameObject>()); superBoutons.Add(new List<GameObject>()); superBoutons.Add(new List<GameObject>()); superBoutons.Add(new List<GameObject>());
        pionEnnemiDetruire.Add(new List<GameObject>()); pionEnnemiDetruire.Add(new List<GameObject>()); pionEnnemiDetruire.Add(new List<GameObject>()); pionEnnemiDetruire.Add(new List<GameObject>()); pionEnnemiDetruire.Add(new List<GameObject>()); pionEnnemiDetruire.Add(new List<GameObject>()); pionEnnemiDetruire.Add(new List<GameObject>()); pionEnnemiDetruire.Add(new List<GameObject>());
        superBoutonsFin.Add(new List<GameObject>()); superBoutonsFin.Add(new List<GameObject>()); superBoutonsFin.Add(new List<GameObject>()); superBoutonsFin.Add(new List<GameObject>()); superBoutonsFin.Add(new List<GameObject>()); superBoutonsFin.Add(new List<GameObject>()); superBoutonsFin.Add(new List<GameObject>()); superBoutonsFin.Add(new List<GameObject>());

        if (pionType == "pion noir")
        {
            while (manger)
            {
                for (int i = 0; i < 50; i++)
                {
                    BougerLePionJoueurDecisif(-150, -150, "pion blanc", 1, 0, buttons[i], false);
                }
                for (int i = 0; i < 50; i++)
                {
                    BougerLePionJoueurDecisif(150, -150, "pion blanc", 1, 0, buttons[i], false);
                }
                for (int i = 0; i < 50; i++)
                {
                    BougerLePionJoueurDecisif(-150, 150, "pion blanc", 1, 0, buttons[i], true);
                }
                for (int i = 0; i < 50; i++)
                {
                    BougerLePionJoueurDecisif(150, 150, "pion blanc", 1, 0, buttons[i], true);
                }
                BougerLePionJoueurConclusion();
            }
        }
        else
        {
            while (manger)
            {
                for (int i = 0; i < 50; i++)
                {
                    BougerLePionJoueurDecisif(150, 150, "pion noir", 0, 1, buttons[i], false);
                }
                for (int i = 0; i < 50; i++)
                {
                    BougerLePionJoueurDecisif(-150, 150, "pion noir", 0, 1, buttons[i], false);
                }
                for (int i = 0; i < 50; i++)
                {
                    BougerLePionJoueurDecisif(150, -150, "pion noir", 0, 1, buttons[i], true);
                }
                for (int i = 0; i < 50; i++)
                {
                    BougerLePionJoueurDecisif(-150, -150, "pion noir", 0, 1, buttons[i], true);
                }
                BougerLePionJoueurConclusion();
            }
        }
    }
    void BougerLePionJoueurConclusion()
    {
        compteur2++;
        if (compteur2 >= 2)
        {
            if (superBouton != null)
            {
                compteur2 = 1;
                if (dameSuperBoutons.Count != 0 && dameSuperBoutons.Count > compteurDame && dame)
                {
                    superBouton = dameSuperBoutons[compteurDame];
                    compteurDame++;
                }
                else
                {
                    superBouton = null;
                    compteur++;
                }
            }
            else
            {
                manger = false;
                if (superBoutons.Count != 0)
                {
                    foreach (GameObject g in dameSuperBoutons)
                    {
                        MettreLaCouleurBrun(g, false);
                    }
                    List<GameObject> max = superBoutons[0];
                    foreach (List<GameObject> list in superBoutons)
                    {
                        if (max.Count < list.Count)
                            max = list;
                    }
                    for (int i = 0; i < superBoutons.Count; i++)
                    {
                        if (superBoutons[i].Count != max.Count)
                        {
                            foreach (GameObject b in superBoutons[i])
                            {
                                MettreLaCouleurBrun(b, false);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < superBoutons[i].Count - dameSoustraire; j++)
                                MettreLaCouleurBleu(superBoutons[i][j]);

                            for (int j = 0; j < superBoutonsFin[i].Count; j++)
                                MettreLaCouleurVert(superBoutonsFin[i][j]);
                        }
                    }
                }
            }
        }
        if (!passerProchaineCase)
        {
            foreach (GameObject bouton in desactiver)
            {
                bouton.GetComponent<Button>().interactable = false;
            }
        }
    }
    void BougerLePionJoueurDecisif(int incrementation1, int incrementation2, string pionEnnemiType, int pionType, int pionTypeAmi, GameObject bouton, bool caseArriere)
    {
        int counter = 0;
        int incrementation11 = incrementation1;
        int incrementation22 = incrementation2;
        bool dameVert = true;
        if(verifierIncrementation1 != -1 && verifierIncrementation2 != -1)
        {
            if ((verifierIncrementation1 != incrementation1 || verifierIncrementation2 != incrementation2) && dame)
                dameVert = false;
        }
        do
        {
            counter++;
            bool passer = true;
            GameObject positionDuPion;
            if (counter >= 2)
            {
                for (int i = 0; i < 50; i++)
                {
                    if (buttons[i].GetComponent<RectTransform>().anchoredPosition.x + incrementation1 == bouton.GetComponent<RectTransform>().anchoredPosition.x && buttons[i].GetComponent<RectTransform>().anchoredPosition.y + incrementation2 == bouton.GetComponent<RectTransform>().anchoredPosition.y)
                    {
                        bouton = buttons[i];
                        break;
                    }
                }
            }
            if (superBouton != null)
            {
                positionDuPion = superBouton;
            }
            else
            {
                positionDuPion = EventSystem.current.currentSelectedGameObject;
            }
            if (bouton.GetComponent<RectTransform>().anchoredPosition.x + incrementation11 == positionDuPion.GetComponent<RectTransform>().anchoredPosition.x && bouton.GetComponent<RectTransform>().anchoredPosition.y + incrementation22 == positionDuPion.GetComponent<RectTransform>().anchoredPosition.y)
            {
                counter++;
                for (int j = 0; j < 40; j++)
                {
                    if (bouton.GetComponent<RectTransform>().anchoredPosition.x == pionsPositions[j, 0] && bouton.GetComponent<RectTransform>().anchoredPosition.y == pionsPositions[j, 1] && pionsPositions[j, 2] == pionType)
                    {
                        pionsEnnemis = GameObject.FindGameObjectsWithTag(pionEnnemiType);
                        foreach (GameObject pionEnnemi in pionsEnnemis)
                        {
                            if (pionsPositions[j, 0] == pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x && pionsPositions[j, 1] == pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y)
                            {
                                if ((pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x + 150 > 2010 || pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x - 150 < 660 || pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y - 150 < 168 || pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y + 150 > 1518))
                                {
                                    return;
                                }
                                else if (pionEnnemiDetruire.Count != 0)
                                {
                                    if (pionEnnemiDetruire[compteur].Contains(pionEnnemi))
                                    {
                                        return;
                                    }
                                }
                                for (int k = 0; k < 40; k++)
                                {
                                    if (pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x - incrementation1 == pionsPositions[k, 0] && pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y - incrementation2 == pionsPositions[k, 1])
                                    {
                                        return;
                                    }
                                }
                                for (int l = 0; l < 50; l++)
                                {
                                    if (pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x - incrementation1 == buttons[l].GetComponent<RectTransform>().anchoredPosition.x && pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y - incrementation2 == buttons[l].GetComponent<RectTransform>().anchoredPosition.y)
                                    {
                                        verifierIncrementation1 = incrementation1;
                                        verifierIncrementation2 = incrementation2;
                                        passerProchaineCase = false;
                                        passer = false;
                                        deuxiemeTour = true;
                                        bool verif = false;
                                        if (compteur > 0)
                                        {
                                            foreach (List<GameObject> list in superBoutons)
                                            {
                                                if (list.Contains(buttons[l]))
                                                {
                                                    verif = true;
                                                }
                                            }
                                        }
                                        if (verif == false && compteur2 == 1)
                                        {
                                            pionEnnemiDetruire[compteur].Add(pionEnnemi);
                                            superBoutons[compteur].Add(buttons[l]);
                                            superBouton = buttons[l];
                                            superBoutonsFin[compteur].Clear();
                                            superBoutonsFin[compteur].Add(buttons[l]);
                                            compteur2 = 0;
                                        }
                                        foreach (GameObject b in desactiverDame)
                                        {
                                            b.GetComponent<Button>().interactable = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (bouton.GetComponent<RectTransform>().anchoredPosition.x == pionsPositions[j, 0] && bouton.GetComponent<RectTransform>().anchoredPosition.y == pionsPositions[j, 1] && pionsPositions[j, 2] == pionTypeAmi)
                    {
                        return;
                    }
                    if (!caseArriere && !dame)
                    {
                        if (j == 39 && passer && !deuxiemeTour && dameVert)
                        {
                            desactiver.Add(bouton);
                            MettreLaCouleurVert(bouton);
                        }
                    }
                    if (dame && passer && j == 39)
                    {
                        if (dameVert)
                        {
                            if (superBouton != null)
                            {
                                dameSuperBoutons.Add(bouton);
                                if(compteur2 == 0)
                                    superBoutonsFin[compteur].Add(bouton);
                            }
                            if(compteurDame == 0)
                            {
                                MettreLaCouleurVert(bouton);
                                desactiverDame.Add(bouton);
                                if (superBouton != null)
                                {
                                    superBoutons[compteur].Add(bouton);
                                    dameSoustraire++;
                                }
                            }    
                        }   
                    }
                }
            }
            if (counter == 1)
            {
                return;
            }
            if (dame)
            {
                if (incrementation11 < 0)
                    incrementation11 -= 150;
                else
                    incrementation11 += 150;
                if (incrementation22 < 0)
                    incrementation22 -= 150;
                else
                    incrementation22 += 150;
            }
        } while (bouton.GetComponent<RectTransform>().anchoredPosition.x - incrementation1 <= 2010 && bouton.GetComponent<RectTransform>().anchoredPosition.x - incrementation1 >= 660 && bouton.GetComponent<RectTransform>().anchoredPosition.y - incrementation2 >= 168 && bouton.GetComponent<RectTransform>().anchoredPosition.y - incrementation2 <= 1518 && dame);
    }
    bool BougerLePionOrdinateurDecisif(int incrementation1, int incrementation2, string pionEnnemiType, string pionAmiType, GameObject bouton, GameObject pion, int pionType, int pionTypeAmi, bool caseArriere, List<GameObject> listOrd)
    {
        int counter = 0;
        int incrementation11 = incrementation1;
        int incrementation22 = incrementation2;
        dame = false;
        GameObject boutonDuPion = null;
        List<Mouvement> mouvementsDame = new List<Mouvement>();
        for (int i = 0; i < 40; i++)
        {
            if (pion.GetComponent<RectTransform>().anchoredPosition.x == pionsPositionsSub[i, 0] && pion.GetComponent<RectTransform>().anchoredPosition.y == pionsPositionsSub[i, 1])
            {
                if(pionsPositionsSub[i, 3] == 1)
                {
                    dame = true;
                    break;
                }
            }
        }
        do
        {
            double heuristique = 0;
            counter++;
            if (counter >= 2)
            {
                for (int i = 0; i < 50; i++)
                {
                    if (buttons[i].GetComponent<RectTransform>().anchoredPosition.x == bouton.GetComponent<RectTransform>().anchoredPosition.x + incrementation1 && buttons[i].GetComponent<RectTransform>().anchoredPosition.y == bouton.GetComponent<RectTransform>().anchoredPosition.y + incrementation2)
                    {
                        bouton = buttons[i];
                        break;
                    }
                }
            }
            if (superBouton != null)
            {
                pion = superBouton;
            }
            if ((pion.GetComponent<RectTransform>().anchoredPosition.x + incrementation11 == bouton.GetComponent<RectTransform>().anchoredPosition.x) && (pion.GetComponent<RectTransform>().anchoredPosition.y + incrementation22 == bouton.GetComponent<RectTransform>().anchoredPosition.y))
            {
                foreach (GameObject boutonP in buttons)
                {
                    if (pion.GetComponent<RectTransform>().anchoredPosition.x == boutonP.GetComponent<RectTransform>().anchoredPosition.x && pion.GetComponent<RectTransform>().anchoredPosition.y == boutonP.GetComponent<RectTransform>().anchoredPosition.y)
                    {
                        boutonDuPion = boutonP;
                        break;
                    }
                }
                double heuristiqueExtremite = 0;
                if (bouton.GetComponent<RectTransform>().anchoredPosition.x == 660 || bouton.GetComponent<RectTransform>().anchoredPosition.x == 2010 || bouton.GetComponent<RectTransform>().anchoredPosition.y == 1518 || bouton.GetComponent<RectTransform>().anchoredPosition.y == 168)
                {
                    if (compteurProfondeur % 2 == 1)
                        heuristiqueExtremite = 0.2;
                    else
                        heuristiqueExtremite = -0.2;
                }
                for (int j = 0; j < 40; j++)
                {
                    if ((pionsPositionsSub[j, 0] == bouton.GetComponent<RectTransform>().anchoredPosition.x) && (pionsPositionsSub[j, 1] == bouton.GetComponent<RectTransform>().anchoredPosition.y) && (pionsPositionsSub[j, 2] == pionTypeAmi))
                    {
                        return false;
                    }
                    else if ((pionsPositionsSub[j, 0] == bouton.GetComponent<RectTransform>().anchoredPosition.x) && (pionsPositionsSub[j, 1] == bouton.GetComponent<RectTransform>().anchoredPosition.y) && (pionsPositionsSub[j, 2] == pionType))
                    {
                        foreach (GameObject pionEnnemi in pionsEnnemis)
                        {
                            if (pionsPositionsSub[j, 0] == pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x && pionsPositionsSub[j, 1] == pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y)
                            {
                                if (pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x + 150 > 2010 || pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x - 150 < 660 || pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y - 150 < 168 || pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y + 150 > 1518)
                                {
                                    return false;
                                }
                                for (int k = 0; k < 40; k++)
                                {
                                    if (pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x + incrementation1 == pionsPositionsSub[k, 0] && pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y + incrementation2 == pionsPositionsSub[k, 1])
                                    {
                                        return false;
                                    }
                                }
                                if (listOrd.Count != 0)
                                {
                                    foreach (GameObject o in listOrd)
                                    {
                                        if (o == pionEnnemi)
                                        {
                                            return false;
                                        }
                                    }
                                }
                                if (superBouton == null)
                                {
                                    listOrd.Add(pion);
                                }
                                listOrd.Add(pionEnnemi);
                                for (int l = 0; l < 50; l++)
                                {
                                    if (pionEnnemi.GetComponent<RectTransform>().anchoredPosition.x + incrementation1 == buttons[l].GetComponent<RectTransform>().anchoredPosition.x && pionEnnemi.GetComponent<RectTransform>().anchoredPosition.y + incrementation2 == buttons[l].GetComponent<RectTransform>().anchoredPosition.y)
                                    {
                                        if (buttons[l].GetComponent<RectTransform>().anchoredPosition.x == 660 || buttons[l].GetComponent<RectTransform>().anchoredPosition.x == 2010 || buttons[l].GetComponent<RectTransform>().anchoredPosition.y == 1518 || buttons[l].GetComponent<RectTransform>().anchoredPosition.y == 168)
                                        {
                                            if (compteurProfondeur % 2 == 1)
                                                heuristiqueExtremite = 0.2;
                                            else
                                                heuristiqueExtremite = -0.2;
                                        }
                                        listOrd.Add(buttons[l]);
                                        ennemiEnVue = true;
                                        superBouton = buttons[l];
                                        
                                        double heuristiqueDame = 0;
                                        if(pionEnnemi.GetComponent<RectTransform>().localScale.z == 2 && ancienMouvement != null)
                                        {
                                            if (ancienMouvement.Profondeur % 2 == 1)
                                                heuristiqueDame = 0.5;
                                            else
                                                heuristiqueDame = -0.5;
                                        }
                                        if (compteurProfondeur == PROFONDEUR)
                                        {
                                            PionNoirOuBlanc(2);
                                            if (tour == 3)
                                            {
                                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                                                if (pionEnnemi.CompareTag("pion blanc"))
                                                {
                                                    heuristique = pions.Length - pionsEnnemis.Length + compteurHeuristique + compteurHeuristiqueInactif + heuristiqueExtremite + heuristiqueDame;
                                                }
                                                else
                                                {
                                                    heuristique = pions.Length - pionsEnnemis.Length - compteurHeuristique + compteurHeuristiqueInactif + heuristiqueExtremite + heuristiqueDame;
                                                }
                                            }
                                            else
                                            {
                                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                                                if (pionEnnemi.CompareTag("pion noir"))
                                                {
                                                    heuristique = pions.Length - pionsEnnemis.Length + compteurHeuristique + compteurHeuristiqueInactif + heuristiqueExtremite + heuristiqueDame;
                                                }
                                                else
                                                {
                                                    heuristique = pions.Length - pionsEnnemis.Length - compteurHeuristique + compteurHeuristiqueInactif + heuristiqueExtremite + heuristiqueDame;
                                                }
                                            }
                                            compteurHeuristique++;
                                        }
                                        if (compteurProfondeur % 2 == 0)
                                        {
                                            if (tour == 2)
                                            {
                                                pions = GameObject.FindGameObjectsWithTag("pion noir");
                                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                                            }
                                            else if (tour == 3)
                                            {
                                                pions = GameObject.FindGameObjectsWithTag("pion blanc");
                                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                                            }
                                        }
                                        else
                                        {
                                            if (tour == 2)
                                            {
                                                pions = GameObject.FindGameObjectsWithTag("pion blanc");
                                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                                            }
                                            else if (tour == 3)
                                            {
                                                pions = GameObject.FindGameObjectsWithTag("pion noir");
                                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                                            }
                                        }
                                        if (ancienMouvement != null)
                                        {
                                            if (mouvements.Contains(ancienMouvement))
                                            {
                                                if (dame)
                                                    mouvementsDame.Remove(ancienMouvement);
                                                else
                                                    mouvements.Remove(ancienMouvement);

                                                ancienMouvement.Bouton = buttons[l];
                                                ancienMouvement.Heuristique = heuristique;
                                                
                                                foreach (GameObject piece in ancienMouvement.PiecesADetruire)
                                                {
                                                    if (piece.GetComponent<RectTransform>().localScale.z == 2)
                                                    {
                                                        if(ancienMouvement.Profondeur % 2 == 1)
                                                            ancienMouvement.Heuristique += 0.5;
                                                        else
                                                            ancienMouvement.Heuristique -= 0.5;
                                                    }
                                                }
                                                ancienMouvement.PiecesADetruire.Add(pionEnnemi);

                                                if (dame)
                                                    mouvementsDame.Add(ancienMouvement);
                                                else
                                                    mouvements.Add(ancienMouvement);
                                            }
                                        }
                                        else
                                        {
                                            Mouvement mouvement = new Mouvement(pion, boutonDuPion, buttons[l], heuristique, compteurProfondeur);
                                            
                                            mouvement.PiecesADetruire.Add(pionEnnemi);
                                            if (leAncetre != null)
                                            {
                                                mouvement.Ancetre = leAncetre;
                                                leAncetre.Descendants.Add(mouvement);
                                            }
                                            if (compteurProfondeur % 2 == 0)
                                                mouvement.MinOuMax = "max";
                                            else
                                                mouvement.MinOuMax = "min";
                                            ancienMouvement = mouvement;
                                            if (dame)
                                            {
                                                mouvementsDame.Add(mouvement);
                                            }
                                            else
                                            {
                                                mouvements.Add(mouvement);
                                            }
                                        }
                                        break;
                                    }
                                }
                                compteur2 = 0;
                                if (compteurProfondeur == 1)
                                    return true;
                                else
                                    return false;
                            }
                        }
                        break;
                    }
                    else if ((j == 39 && !caseArriere) || (dame && j == 39))
                    {
                        if (superBouton == null)
                        {
                            if (compteurProfondeur != PROFONDEUR)
                            {
                                Mouvement mouvement = new Mouvement(pion, boutonDuPion, bouton, 0, compteurProfondeur);
                                
                                if (leAncetre != null)
                                {
                                    mouvement.Ancetre = leAncetre;
                                    leAncetre.Descendants.Add(mouvement);
                                }
                                if (compteurProfondeur % 2 == 0)
                                    mouvement.MinOuMax = "max";
                                else
                                    mouvement.MinOuMax = "min";
                                
                                if (dame)
                                {
                                    mouvementsDame.Add(mouvement);
                                }
                                else
                                {
                                    mouvements.Add(mouvement);
                                }
                                compteur2++;
                            }
                            else
                            {
                                PionNoirOuBlanc(2);
                                if (tour == 3)
                                {
                                    pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                                }
                                else
                                {
                                    pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                                }

                                heuristique += pions.Length - pionsEnnemis.Length + compteurHeuristiqueInactif + heuristiqueExtremite;
                                Mouvement mouvement = new Mouvement(pion, boutonDuPion, bouton, heuristique, compteurProfondeur);
                                
                                if (leAncetre != null)
                                {
                                    mouvement.Ancetre = leAncetre;
                                    leAncetre.Descendants.Add(mouvement);
                                }
                                if (mouvement.Ancetre != null)
                                {
                                    Mouvement ancetreExtremite = mouvement.Ancetre;
                                    for (int i = 0; i < PROFONDEUR - 1; i++)
                                    {
                                        if (ancetreExtremite.Bouton.GetComponent<RectTransform>().anchoredPosition.x == 660 || ancetreExtremite.Bouton.GetComponent<RectTransform>().anchoredPosition.x == 2010 || ancetreExtremite.Bouton.GetComponent<RectTransform>().anchoredPosition.y == 1518 || ancetreExtremite.Bouton.GetComponent<RectTransform>().anchoredPosition.y == 168)
                                        {
                                            if (ancetreExtremite.Profondeur % 2 == 1)
                                                mouvement.Heuristique += 0.2;
                                            else
                                                mouvement.Heuristique -= 0.2;
                                        }
                                        if (ancetreExtremite.Ancetre != null)
                                            ancetreExtremite = ancetreExtremite.Ancetre;
                                        else
                                            break;
                                    }
                                }
                                if (compteurProfondeur % 2 == 0)
                                    mouvement.MinOuMax = "max";
                                else
                                    mouvement.MinOuMax = "min";
                                
                                if (dame)
                                    mouvementsDame.Add(mouvement);
                                else
                                    mouvements.Add(mouvement);

                                compteur2++;
                            }
                            if (compteurProfondeur % 2 == 0)
                            {
                                if (tour == 2)
                                {
                                    pions = GameObject.FindGameObjectsWithTag("pion noir");
                                    pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                                }
                                else if (tour == 3)
                                {
                                    pions = GameObject.FindGameObjectsWithTag("pion blanc");
                                    pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                                }
                            }
                            else
                            {
                                if (tour == 2)
                                {
                                    pions = GameObject.FindGameObjectsWithTag("pion blanc");
                                    pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                                }
                                else if (tour == 3)
                                {
                                    pions = GameObject.FindGameObjectsWithTag("pion noir");
                                    pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                                }
                            }
                            if(!dame)
                                return false;
                        }
                    }
                }
            }
            if (dame)
            {
                if (incrementation11 < 0)
                    incrementation11 -= 150;
                else
                    incrementation11 += 150;
                if (incrementation22 < 0)
                    incrementation22 -= 150;
                else
                    incrementation22 += 150;
            }
        } while (bouton.GetComponent<RectTransform>().anchoredPosition.x + incrementation1 <= 2010 && bouton.GetComponent<RectTransform>().anchoredPosition.x + incrementation1 >= 660 && bouton.GetComponent<RectTransform>().anchoredPosition.y + incrementation2 >= 168 && bouton.GetComponent<RectTransform>().anchoredPosition.y + incrementation2 <= 1518 && dame);
        
        if(compteurProfondeur % 2 == 0 && dame && mouvementsDame.Count != 0)
        {
            mouvementsDame.Sort((a, b) => a.Heuristique.CompareTo(b.Heuristique));
            mouvements.Add(mouvementsDame[0]);
        }
        else if(compteurProfondeur % 2 == 1 && dame && mouvementsDame.Count != 0)
        {
            mouvementsDame.Sort((a, b) => b.Heuristique.CompareTo(a.Heuristique));
            mouvements.Add(mouvementsDame[0]);
        }
        return false;
    }
    void DesactiverManger()
    {
        premiereCase = true;
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 40; j++)
            {
                if (buttons[i].GetComponent<RectTransform>().anchoredPosition.x == pionsPositions[j, 0] && buttons[i].GetComponent<RectTransform>().anchoredPosition.y == pionsPositions[j, 1])
                {
                    if ((tour == 2 && pionsPositions[j, 2] == 1))
                    {
                        continue;
                    }
                    else if ((tour == 2 && pionsPositions[j, 2] == 0))
                    {
                        DesactiverManger2("pion noir", 1);
                    }
                    else if((tour == 3 && pionsPositions[j, 2] == 0))
                    {
                        continue;
                    }
                    else if ((tour == 3 && pionsPositions[j, 2] == 1))
                    {
                        DesactiverManger2("pion blanc", 0);
                    }
                }
            }
        }
    }
    void DesactiverManger2(string pionTypeAmi, int indicePionEnnemi)
    {
        pions = GameObject.FindGameObjectsWithTag(pionTypeAmi);
        foreach (GameObject pion in pions)
        {
            list = new List<GameObject>();
            superBouton2 = null;
            premiereCase = true;

            DesactiverManger2Verifier(150, 150, pion, indicePionEnnemi);
            DesactiverManger2Verifier(150, -150, pion, indicePionEnnemi);
            DesactiverManger2Verifier(-150, 150, pion, indicePionEnnemi);
            DesactiverManger2Verifier(-150, -150, pion, indicePionEnnemi);
        }
    }
    void DesactiverManger2Verifier(int incrementation1, int incrementation2, GameObject pion, int indicePionEnnemi)
    {
        bool manger = true;
        while (manger)
        {
            if(superBouton2 != null)
            {
                pion = superBouton2;
            }
            for (int i = 0; i < 40; i++)
            {
                if ((pion.GetComponent<RectTransform>().anchoredPosition.x + incrementation1 == pionsPositions[i, 0] && pion.GetComponent<RectTransform>().anchoredPosition.y + incrementation2 == pionsPositions[i, 1]) && pionsPositions[i, 2] == indicePionEnnemi)
                {
                    for (int j = 0; j < 40; j++)
                    {
                        if ((pion.GetComponent<RectTransform>().anchoredPosition.x + (2 * incrementation1) == pionsPositions[j, 0] && pion.GetComponent<RectTransform>().anchoredPosition.y + (2 * incrementation2) == pionsPositions[j, 1]) || (pion.GetComponent<RectTransform>().anchoredPosition.x + (2 * incrementation1) > 2010 || pion.GetComponent<RectTransform>().anchoredPosition.x + (2 * incrementation1) < 660 || pion.GetComponent<RectTransform>().anchoredPosition.y + (2 * incrementation2) < 168 || pion.GetComponent<RectTransform>().anchoredPosition.y + (2 * incrementation2) > 1518))
                        {
                            return;
                        }
                    }
                    for(int j = 0; j < 50; j++)
                    {
                        if (pion.GetComponent<RectTransform>().anchoredPosition.x + (2 * incrementation1) == buttons[j].GetComponent<RectTransform>().anchoredPosition.x && pion.GetComponent<RectTransform>().anchoredPosition.y + (2 * incrementation2) == buttons[j].GetComponent<RectTransform>().anchoredPosition.y)
                        {
                            if (premiereCase)
                            {
                                for (int l = 0; l < 50; l++)
                                {
                                    if (pion.GetComponent<RectTransform>().anchoredPosition.x == buttons[l].GetComponent<RectTransform>().anchoredPosition.x && pion.GetComponent<RectTransform>().anchoredPosition.y == buttons[l].GetComponent<RectTransform>().anchoredPosition.y)
                                    {
                                        list.Add(buttons[l]);
                                        premiereCase = false;
                                    }
                                }
                            }
                            list.Add(buttons[j]);
                            superBouton2 = buttons[j];
                            pion = superBouton2;
                            break;
                        }
                    }
                }
                else if(i == 39)
                {
                    manger = false;
                }
            }
        }
        if(list.Count != 0)
        {
            pionsAManger.Add(list);
        }
    }
    void TransformerEnDame(float nouvellePositionX, float nouvellePositionY, GameObject pion, int indice)
    {
        if (nouvellePositionY == 1518 && pion.CompareTag("pion noir"))
        {
            Destroy(pion);
            dames[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(nouvellePositionX, nouvellePositionY);
            Instantiate(dames[0], canvas);
            pionsPositions[indice, 3] = 1;
        }
        else if (nouvellePositionY == 168 && pion.CompareTag("pion blanc"))
        {
            Destroy(pion);
            dames[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(nouvellePositionX, nouvellePositionY);
            Instantiate(dames[1], canvas);
            pionsPositions[indice, 3] = 1;
        }
        else
        {
            pion.GetComponent<RectTransform>().anchoredPosition = new Vector2(nouvellePositionX, nouvellePositionY);
        }
    }
    void InitialiserPions(string pionType, string pionTypeEnnemi)
    {
        if (compteurProfondeur == 1)
        {
            pions = GameObject.FindGameObjectsWithTag(pionType);
            pionsEnnemis = GameObject.FindGameObjectsWithTag(pionTypeEnnemi);
        }
        else
        {
            compteurHeuristiqueInactif = 0;
            foreach(Mouvement mouvement in mouvements)
            {
                if (nePasAcceder.Contains(mouvement))
                {
                    continue;
                }
                if(mouvement.Profondeur == compteurProfondeur - 1 && compteurProfondeur != PROFONDEUR + 1)
                {
                    for(int i = 0; i < desactiverAncetres.Count; i++)
                    {
                        desactiverAncetres[i].Pion.GetComponent<RectTransform>().anchoredPosition = new Vector2(desactiverAncetres[i].BoutonDuPion.GetComponent<RectTransform>().anchoredPosition.x, desactiverAncetres[i].BoutonDuPion.GetComponent<RectTransform>().anchoredPosition.y);
                    }
                    desactiverAncetres.Clear();
                    
                    if(reInitialiser.Count != 0)
                    {
                        foreach(GameObject initialiser in reInitialiser)
                        {
                            initialiser.SetActive(true);
                        }
                    }
                    reInitialiser.Clear();
                    if (profondeurChange)
                    {
                        if(compteurProfondeur % 2 == 0)
                        {
                            if(tour == 2)
                            {
                                pions = GameObject.FindGameObjectsWithTag("pion noir");
                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                            }                                
                            else if(tour == 3)
                            {
                                pions = GameObject.FindGameObjectsWithTag("pion blanc");
                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                            }
                        }
                        else
                        {
                            if (tour == 2)
                            {
                                pions = GameObject.FindGameObjectsWithTag("pion blanc");
                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion noir");
                            }                                
                            else if (tour == 3)
                            {
                                pions = GameObject.FindGameObjectsWithTag("pion noir");
                                pionsEnnemis = GameObject.FindGameObjectsWithTag("pion blanc");
                            }
                        }
                        profondeurChange = false;
                    }
                    for(int i = 0; i < pionsPositionsIndice.Count; i++)
                    {
                        pionsPositionsSub[pionsPositionsIndice[i], 0] = pionsPositions[pionsPositionsIndice[i], 0];
                        pionsPositionsSub[pionsPositionsIndice[i], 1] = pionsPositions[pionsPositionsIndice[i], 1];
                        pionsPositionsSub[pionsPositionsIndice[i], 3] = pionsPositions[pionsPositionsIndice[i], 3];
                    }
                    leAncetre = mouvement;
                    nePasAcceder.Add(mouvement);
                    pionsPositionsIndice.Clear();

                    for (int k = 0; k < 40; k++)
                    {
                        if (pionsPositionsSub[k, 0] == mouvement.Pion.GetComponent<RectTransform>().anchoredPosition.x && pionsPositionsSub[k, 1] == mouvement.Pion.GetComponent<RectTransform>().anchoredPosition.y)
                        {
                            pionsPositionsSub[k, 0] = mouvement.Bouton.GetComponent<RectTransform>().anchoredPosition.x;
                            pionsPositionsSub[k, 1] = mouvement.Bouton.GetComponent<RectTransform>().anchoredPosition.y;
                            pionsPositionsIndice.Add(k);
                            break;
                        }
                    }
                    mouvement.Pion.GetComponent<RectTransform>().anchoredPosition = new Vector2(mouvement.Bouton.GetComponent<RectTransform>().anchoredPosition.x, mouvement.Bouton.GetComponent<RectTransform>().anchoredPosition.y);
                    desactiverAncetres.Add(mouvement);
                    if(mouvement.PiecesADetruire.Count != 0)
                    {
                        foreach (GameObject detruire in mouvement.PiecesADetruire)
                        {
                            for (int i = 0; i < 40; i++)
                            {
                                if(pionsPositionsSub[i, 0] == detruire.GetComponent<RectTransform>().anchoredPosition.x && pionsPositionsSub[i, 1] == detruire.GetComponent<RectTransform>().anchoredPosition.y)
                                {
                                    pionsPositionsSub[i, 0] = -1;
                                    pionsPositionsSub[i, 1] = -1;
                                    pionsPositionsIndice.Add(i);
                                    break;
                                }
                            }
                            reInitialiser.Add(detruire);
                            detruire.SetActive(false);
                            if(mouvement.Profondeur % 2 == 0)
                            {
                                compteurHeuristiqueInactif--;
                            }
                            else
                            {
                                compteurHeuristiqueInactif++;
                            }
                        }
                    }
                    if(mouvement.Ancetre != null)
                    {
                        Mouvement ancetreSub = mouvement.Ancetre;
                        for (int i = 0; i < mouvement.Profondeur - 1; i++)
                        {
                            for (int k = 0; k < 40; k++)
                            {
                                if (pionsPositionsSub[k, 0] == ancetreSub.Pion.GetComponent<RectTransform>().anchoredPosition.x && pionsPositionsSub[k, 1] == ancetreSub.Pion.GetComponent<RectTransform>().anchoredPosition.y)
                                {
                                    pionsPositionsSub[k, 0] = ancetreSub.Bouton.GetComponent<RectTransform>().anchoredPosition.x;
                                    pionsPositionsSub[k, 1] = ancetreSub.Bouton.GetComponent<RectTransform>().anchoredPosition.y;
                                    pionsPositionsIndice.Add(k);
                                    break;
                                }
                            }
                            ancetreSub.Pion.GetComponent<RectTransform>().anchoredPosition = new Vector2(ancetreSub.Bouton.GetComponent<RectTransform>().anchoredPosition.x, ancetreSub.Bouton.GetComponent<RectTransform>().anchoredPosition.y);
                            desactiverAncetres.Add(ancetreSub);
                            if (ancetreSub.PiecesADetruire.Count != 0)
                            {
                                foreach (GameObject detruire in ancetreSub.PiecesADetruire)
                                {
                                    for (int j = 0; j < 40; j++)
                                    {
                                        if (pionsPositionsSub[j, 0] == detruire.GetComponent<RectTransform>().anchoredPosition.x && pionsPositionsSub[j, 1] == detruire.GetComponent<RectTransform>().anchoredPosition.y)
                                        {
                                            pionsPositionsSub[j, 0] = -1;
                                            pionsPositionsSub[j, 1] = -1;
                                            pionsPositionsIndice.Add(j);
                                            break;
                                        }
                                    }
                                    reInitialiser.Add(detruire);
                                    detruire.SetActive(false);
                                    if (ancetreSub.Profondeur % 2 == 0)
                                    {
                                        compteurHeuristiqueInactif--;
                                    }
                                    else
                                    {
                                        compteurHeuristiqueInactif++;
                                    }
                                }
                            }
                            if (ancetreSub.Ancetre != null)
                                ancetreSub = ancetreSub.Ancetre;
                        }
                    }
                    return;
                }
            }
            for (int i = 0; i < desactiverAncetres.Count; i++)
            {
                desactiverAncetres[i].Pion.GetComponent<RectTransform>().anchoredPosition = new Vector2(desactiverAncetres[i].BoutonDuPion.GetComponent<RectTransform>().anchoredPosition.x, desactiverAncetres[i].BoutonDuPion.GetComponent<RectTransform>().anchoredPosition.y);
            }
            for (int i = 0; i < pionsPositionsIndice.Count; i++)
            {
                pionsPositionsSub[pionsPositionsIndice[i], 0] = pionsPositions[pionsPositionsIndice[i], 0];
                pionsPositionsSub[pionsPositionsIndice[i], 1] = pionsPositions[pionsPositionsIndice[i], 1];
                pionsPositionsSub[pionsPositionsIndice[i], 3] = pionsPositions[pionsPositionsIndice[i], 3];
            }
            desactiverAncetres.Clear();
            pionsPositionsIndice.Clear();

            if (reInitialiser.Count != 0)
            {
                foreach (GameObject initialiser in reInitialiser)
                {
                    initialiser.SetActive(true);
                }
            }
            reInitialiser.Clear();
        }
    }
}
public class Mouvement
{
    public GameObject Pion { get; set; }
    public GameObject BoutonDuPion { get; set; }
    public GameObject Bouton { get; set; }
    public List<Mouvement> Descendants { get; set; }
    public Mouvement Ancetre { get; set; }
    public double Heuristique { get; set; }
    public int Profondeur { get; set; }
    public string MinOuMax;
    public List<GameObject> PiecesADetruire { get; set; }

    public Mouvement(GameObject pion, GameObject boutonDuPion, GameObject bouton, double heuristique, int profondeur)
    {
        Descendants = new List<Mouvement>();
        PiecesADetruire = new List<GameObject>();
        Pion = pion;
        BoutonDuPion = boutonDuPion;
        Bouton = bouton;
        Heuristique = heuristique;
        Profondeur = profondeur;
        Ancetre = null;
    }
}
