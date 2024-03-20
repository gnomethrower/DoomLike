using System.Collections;
using UnityEngine;

public class FragmentationExplosion_Script : MonoBehaviour
{
    public GameObject fragmentPrefab;
    public int fragmentCount = 50;
    public float explosionForce = 500f;
    public float explosionRadius = 5f;
    public float fragmentSize = 0.01f; // Adjust this to match your Unity PPU
    public float fragmentLifetime = 3f; // Lifetime of fragments in seconds
    public float fragmentMass = 0.1f;
    public float airDrag = 0.1f;



    [SerializeField] private bool renderLine = true;
    public float renderStepCount = 5f;

    //[SerializeField] private bool renderSphere = true;
    [SerializeField] private float collisionSphereScale = 0.1f;

    public Color trajectoryColor = Color.red;
    public float trajectoryWidth = 0.02f;



    private void Update()
    {
        if (renderStepCount <= 0)
        {
            renderStepCount = 1;
        }
        // Check for LeftAlt key press
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Debug.Log("Explode!");
            Explode();
        }
    }

    void Explode()
    {
        Vector3 explosionPosition = this.transform.position;

        // Generate all fragments
        for (int i = 0; i < fragmentCount; i++)
        {
            GameObject fragment = Instantiate(fragmentPrefab, explosionPosition, Quaternion.identity);

            // Add Rigidbody component
            Rigidbody rb = fragment.GetComponent<Rigidbody>();
            if (rb == null)
                rb = fragment.AddComponent<Rigidbody>();

            rb.mass = fragmentMass; // Set mass
            rb.drag = airDrag; // Set drag

            // Random direction
            Vector3 randomDirection = Random.onUnitSphere;

            // Apply force
            rb.AddForce(randomDirection * explosionForce);

            // Add LineRenderer component for trajectory visualization
            LineRenderer trajectoryLine = fragment.AddComponent<LineRenderer>();
            trajectoryLine.startWidth = trajectoryWidth;
            trajectoryLine.endWidth = trajectoryWidth;
            trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
            trajectoryLine.startColor = trajectoryColor;
            trajectoryLine.endColor = trajectoryColor;

            StartCoroutine(UpdateTrajectory(trajectoryLine, explosionPosition, rb));

            // Destroy the fragment itself after fragmentLifetime seconds
            Destroy(fragment, fragmentLifetime);
        }
    }

    IEnumerator UpdateTrajectory(LineRenderer lineRenderer, Vector3 startPos, Rigidbody rb)
    {
        Vector3 previousPosition = startPos;
        bool hasCollided = false;
        LayerMask groundLayer = LayerMask.GetMask("Ground"); // Assuming "Ground" is the name of your layer

        renderStepCount = 5; // Render every 5 steps (adjust as needed)
        if (renderLine)
        {
            while (!hasCollided)
            {
                if (rb == null || rb.velocity == Vector3.zero)
                {
                    yield return null;
                    continue;
                }

                lineRenderer.positionCount++;
                previousPosition = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
                RaycastHit hit;
                if (Physics.Raycast(previousPosition, rb.velocity.normalized, out hit, 2f, groundLayer))
                {
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                    InstantiateCollisionPrefab(hit.point);
                    hasCollided = true;
                }
                else
                {
                    Vector3 newPos = previousPosition + rb.velocity.normalized * 0.1f;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
                }

                // Check if it's time to render a new point
                if (lineRenderer.positionCount % renderStepCount == 0)
                {
                    lineRenderer.Simplify(0.1f); // Simplify the line renderer to reduce the number of points
                    yield return null; // Yield to avoid overloading the frame
                }
            }
        }
        
    }


    void InstantiateCollisionPrefab(Vector3 collisionPoint)
    {
        GameObject collisionPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        collisionPrefab.transform.position = collisionPoint;
        collisionPrefab.transform.localScale = new Vector3(collisionSphereScale, collisionSphereScale, collisionSphereScale);
        collisionPrefab.GetComponent<Renderer>().material.color = Color.red;
        Destroy(collisionPrefab, fragmentLifetime); // Destroy after 3 seconds
    }

}
