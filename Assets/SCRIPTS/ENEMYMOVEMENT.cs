using UnityEngine;

public class ENEMYMOVEMENT : MonoBehaviour
{
    public float SPEED1 = 5F;
    public float SPEED2 = 3F;
    public float SPEED3 = 4F;
    private Vector3 startpos;
  
    void Start()
    {
        startpos = transform.position;
        
    }

  
    void Update()
    {
        float NEWX=startpos.x + Mathf.PingPong(Time.time * SPEED1, 12);
        transform.position = new Vector3(NEWX, startpos.y, startpos.z);
    }
}
