using UnityEngine;

public class MovingBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Defines an agent that moves
    public class seek
    {
        public Vector2 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public float maxForce = 5; // rate of acceleration
        public float maxSpeed = 4; //grid squares the second
        public seek(Vector2 pos)
        {
            this.position = pos;
            this.velocity = Vector2.zero;
            this.maxForce = 5;
            this.maxSpeed = 4;
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
