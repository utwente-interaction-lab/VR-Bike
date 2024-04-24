using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Target : MonoBehaviour
    {
        private GameObject self;
        Vector3 velocity;
        public Target(GameObject g, Vector3 pos, Vector3 vel)
        {
            self = g;
            self.transform.position = pos;
            velocity = vel;
            //self = Instantiate(g);
        }
        public bool collides(GameObject o)
        {
            //Debug.Log((o.transform.position - self.transform.position).magnitude);
            return (o.transform.position - self.transform.position).magnitude < 2;
            //return false;
        }
        public void update()
        {
            self.transform.position += velocity;
        }
        public void destroy()
        {
            Destroy(self);
        }
    }
}
