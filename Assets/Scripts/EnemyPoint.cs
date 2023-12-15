using UnityEngine;

public class EnemyPoint : MonoBehaviour
{
    public Renderer enemyPoint;
    public LayerMask targetLayer;

    Renderer enemy;
    Transform player;


    private void Awake()
    {
        enemy = GetComponent<Renderer>();
    }

    private void Start()
    {
        player = GameManager.Instance.player.transform;
    }
    // Update is called once per frame
    void Update()
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

            if (Mathf.Abs(hitLocalPos.x) * 2 < 12.89f)
            {
                if(hitLocalPos.y > 0)
                {
                    pivot -= deltaVec;
                }
                else
                {
                    pivot += deltaVec;
                }
            }

            enemyPoint.transform.localRotation = Quaternion.FromToRotation(Vector3.up, transform.position - pivot);
            enemyPoint.transform.position = hit.point;
        }
    }
}
