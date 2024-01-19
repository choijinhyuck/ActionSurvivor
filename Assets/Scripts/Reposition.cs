using Unity.VisualScripting;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area")) return;

        Vector2 areaSize = collision.GetComponent<BoxCollider2D>().size;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;


        switch (transform.tag)
        {
            case "Ground":
                myPos += new Vector3(7.5f, 7.5f, 0f);
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > areaSize.x / 2)
                {
                    transform.parent.Translate(60 * dirX * Vector3.right);
                }
                if (diffY > areaSize.y / 2)
                {
                    transform.parent.Translate(60 * dirY * Vector3.up);
                }
                break;
            case "Enemy":
                if (coll.enabled)
                {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
                    transform.Translate(ran + dist * 2);
                }
                break;
            case "DropItem":
                diffX = playerPos.x - myPos.x;
                diffY = playerPos.y - myPos.y;
                dirX = diffX < 0 ? -1 : 1;
                dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > areaSize.x / 2)
                {
                    transform.Translate(60 * dirX * Vector3.right);
                }
                if (diffY > areaSize.y / 2)
                {
                    transform.Translate(60 * dirY * Vector3.up);
                }
                break;
        }

    }
}
