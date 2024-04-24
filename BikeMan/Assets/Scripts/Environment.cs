using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class Environment : MonoBehaviour
    {

        //TEST VARIABLES:

        [SerializeField] private float targetAngle = 0; //how the target moves in respect to the player
        [SerializeField] private float targetVelocity = 5; //velocity of the target in KMH
        [SerializeField] private float targetSeparation = 20; //how far away the target spawns from the player
        [SerializeField] private int targetsVisible = 1; //how many targets are in the game at a time
        [SerializeField] private float targetWindowWidth = 20; //travel distance of the target
        [SerializeField] private float targetHeight = 2;

        [SerializeField] private bool lowPoly;
        [SerializeField] private bool tronGrid;
        [SerializeField] private int gridSize = 1; //in meters
        [SerializeField] private bool grass;
        [SerializeField] private int timeUntillTest = 10;
        
        private GameObject target;
        private GameObject player;
        private GameObject floor;

        private float timePassed;
        private float timeOffset;
        private bool testInProgress = false;

        List<Target> targetList = new List<Target>();
        //ArrayList targetList = new ArrayList();
        //public Target 

        void Start()
        {
            timePassed = 0;
            timeOffset = Time.realtimeSinceStartup;
            // target = GameObject.Find("Target");
            target = Instantiate(Resources.Load("Target", typeof(GameObject))) as GameObject;

            player = GameObject.Find("TestBike");
            floor = GameObject.Find("BasicGround");

            //(timeUntillTest, testStart);
            /*
            for (int i = 0; i < 10; i++)
            {
                targetList.Add(new Target(Instantiate(target), new Vector3(0, 2, i * 10f)));
            }

            for (int i = targetList.Count - 1; i >= 0; i--)
            {
                destroyTarget(i);
            }
            */
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void FixedUpdate()
        {
            timePassed = Time.realtimeSinceStartup - timeOffset;
            //Debug.Log(timePassed);
            if (timePassed > timeUntillTest && !testInProgress) startTest();

            foreach (Target t in targetList)
            {
                t.update();
                if (t.collides(player))
                {
                    destroyTarget(t);
                    spawnTarget();
                    Debug.Log("collision, spawned a new target");
                }
            }
        }

        void startTest()
        {
            for(int i = 0; i < targetsVisible; i++)
            {
                spawnTarget();
            }
            Debug.Log("test started");
            //sign the test as 'in progress'
            testInProgress = true;
        }

        //spawns a target based on the parameters
        void spawnTarget()
        {
            //this is so that multiple targets can be spawned
            int n = targetList.Count + 1;
            //first find out where the player is cycling (direction):
            Vector3 playerHeading = player.transform.localEulerAngles;
            //find the position one target separation in front of the player
            Vector3 separation = getAnglePos(playerHeading.y, targetSeparation * n);
            //add the player position to the separation
            Vector3 targetPos = player.transform.position + separation;
            //generate a random attack angle, base 90
            float randomAngle = ((Random.value - 0.5f) * targetAngle * 2) - 90;
            Vector3 windowDiff = getAnglePos(randomAngle, targetWindowWidth / 2);
            //add the window Diff
            targetPos += windowDiff;
            //velocity is opposite the window diff
            Vector3 targetVel = getAnglePos(randomAngle - 180, targetVelocity / 50f);
            //fix the height:
            targetPos.y = targetHeight;
            //now spawn a target
            targetList.Add(new Target(Instantiate(target), targetPos, targetVel));
            
        }

        private Vector3 getAnglePos(float angle, float dist)
        {
            float a = scale(0, 180, 0, Mathf.PI, angle);
            float dx = Mathf.Sin(a) * dist;
            float dz = Mathf.Cos(a) * dist;
            return new Vector3(dx, 0, dz);
        }

        //remove by target reference
        void destroyTarget(Target t)
        {
            t.destroy();
            targetList.Remove(t);
        }
        //remove by index
        void destroyTarget(int index)
        {
            //first destroy the Game Object
            targetList[index].destroy();
            targetList.RemoveAt(index);
        }

        void centerFloor()
        {
            Vector3 centerPos = new Vector3(player.transform.position.x, 0, player.transform.position.z);
            floor.transform.position = centerPos;
        }
        public float scale(float oldMin, float oldMax, float newMin, float newMax, float input)
        {
            float oldRange = (oldMax - oldMin);
            float newRange = (newMax - newMin);
            return (((input - oldMin) * newRange) / oldRange) + newMin;
        }
    }
}
