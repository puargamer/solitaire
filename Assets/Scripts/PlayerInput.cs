using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public SolitaireGame solitaireGame;
    public GameObject previousSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if (hit.collider.CompareTag("Deck"))
                {
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck()
    {
        print("Click on Deck");
        solitaireGame.DealFromDeck();
        if (previousSelected != null)
        {
            previousSelected.GetComponent<CardPresenter>().spriteRenderer.color = Color.white;
            previousSelected = null;
        }
    }

    void Card(GameObject selected)
    {
        print("Click on Card");
        //if previous selected was a card, try to stack
        if (previousSelected != null)
        {
            if (previousSelected.TryGetComponent<CardPresenter>(out CardPresenter cardPresenter))
            {
                previousSelected.GetComponent<CardPresenter>().spriteRenderer.color = Color.white;

                if (solitaireGame.CanStack(previousSelected, selected))
                {
                    solitaireGame.Stack(previousSelected, selected);
                }
                previousSelected = null;
                return;
            }
        }
        previousSelected = selected;
        previousSelected.GetComponent<CardPresenter>().spriteRenderer.color = Color.yellow;
    }

    void Top(GameObject selected)
    {
        print("Click on Top");
        if (previousSelected != null)
        {
            if (previousSelected.TryGetComponent<CardPresenter>(out CardPresenter cardPresenter))
            {
                previousSelected.GetComponent<CardPresenter>().spriteRenderer.color = Color.white;
                solitaireGame.StackToTop(previousSelected, selected);
                previousSelected = null;
                return;
            }
        }
    }

    void Bottom(GameObject selected)
    {
        print("Click on Bottom");
        if (previousSelected != null)
        {
            if (previousSelected.TryGetComponent<CardPresenter>(out CardPresenter cardPresenter))
            {
                solitaireGame.StackToBottom(previousSelected, selected);
                previousSelected.GetComponent<CardPresenter>().spriteRenderer.color = Color.white;
                previousSelected = null;
                return;
            }
        }
    }

}
