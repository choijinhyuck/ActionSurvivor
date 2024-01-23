using UnityEngine;
using UnityEngine.UI;

public class EnemyPoint : MonoBehaviour
{
    public Image enemyPoint;
    public LayerMask targetLayer;

    Renderer enemy;
    Transform player;


    private void Awake()
    {
        enemy = GetComponent<Renderer>();
    }

    private void Start()
    {
        player = GameManager.instance.player.transform;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (enemy.isVisible)
        {
            if (enemyPoint.gameObject.activeSelf)
            {
                enemyPoint.gameObject.SetActive(false);
            }
        }
        else
        {
            if (!enemyPoint.gameObject.activeSelf)
            {
                enemyPoint.gameObject.SetActive(true);
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position - transform.position, Mathf.Infinity, targetLayer);
            //Debug.Log(hit.collider.transform.InverseTransformPoint(hit.point));
            Vector2 hitLocalPos = hit.collider.transform.InverseTransformPoint(hit.point);
            Vector3 pivot = player.position;
            Vector3 deltaVec = new Vector3(0, 10f, 0);

            if (Mathf.Abs(hitLocalPos.x) * 2f < hit.collider.GetComponent<BoxCollider2D>().size.x * 0.98f)
            {
                if (hitLocalPos.y > 0)
                {
                    pivot -= deltaVec;
                }
                else
                {
                    pivot += deltaVec;
                }
            }

            float rot = Vector2.SignedAngle(Vector2.up, transform.position - pivot);
            enemyPoint.transform.localEulerAngles = new Vector3(0, 0, rot);
            //enemyPoint.transform.localRotation = Quaternion.FromToRotation(Vector3.up, transform.position - pivot);
            enemyPoint.transform.position = Camera.main.WorldToScreenPoint(hit.point);
        }
    }
}
