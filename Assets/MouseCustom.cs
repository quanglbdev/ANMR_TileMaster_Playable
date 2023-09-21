using UnityEngine;

public class MouseCustom : MonoBehaviour
{
    private SpriteRenderer mouse;
    void Start()
    {
        mouse = GetComponent<SpriteRenderer>();
        Cursor.visible = false;
    }
    void Update()
    {
        Vector2 customPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(customPos.x + 0.4f, customPos.y -0.4f) ;
    }
}
