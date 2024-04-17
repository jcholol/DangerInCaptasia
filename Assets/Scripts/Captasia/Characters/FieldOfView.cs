using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Photon.Pun;

public class FieldOfView : MonoBehaviour
{

    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;

    [Header("My PhotonView Player")]
    public PhotonView myPhotonView;

    [Header("Player List")]
    public GameObject[] playerList;

    public GameObject playerToFollow;
    public new Light2D light;

    private int spectateIndex = 0;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        findPhotonView();
        findLight();
        handleSpectate();

        if (playerToFollow == null)
        {
            if (myPhotonView != null)
            {
                playerToFollow = myPhotonView.gameObject;
            }
            return;
        }

        if (light == null)
        {
            return;
        }

        handleRayCast();
    }

    private void handleRayCast()
    {
        Transform originTransform = light.transform;
        int rayCount = 360;
        float viewDistance = light.pointLightOuterRadius;

        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[vertices.Length * 3];

        // Middle of fov circle
        vertices[0] = originTransform.transform.position;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            var rot = Quaternion.AngleAxis((360.0f / rayCount) * i, originTransform.transform.forward);
            var wDirection = transform.TransformDirection(rot * originTransform.transform.right);

            RaycastHit2D raycastHit2D = Physics2D.Raycast(originTransform.transform.position, wDirection, viewDistance, layerMask);

            if (raycastHit2D.collider != null)
            {
                if (light.transform.position.y < raycastHit2D.point.y &&
                    Mathf.Abs(light.transform.position.x - raycastHit2D.point.x) <= 1)
                {
                    vertices[vertexIndex] = raycastHit2D.point + 
                        new Vector2(0, wDirection.y + 0.5f);
                } else
                {
                    vertices[vertexIndex] = raycastHit2D.point;
                }
            }
            else
            {
                vertices[vertexIndex] = originTransform.transform.position + (wDirection * viewDistance);
            }

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(originTransform.transform.position, Vector3.one * 1000f);
    }

    private void findPhotonView()
    {
        if (myPhotonView == null)
        {
            GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in playerList)
            {
                PhotonView photonView = player.GetComponent<PhotonView>();

                if (photonView.IsMine)
                {
                    myPhotonView = photonView;
                }
            }
        }
    }

    private void handleSpectate()
    {
        if (myPhotonView == null)
        {
            return;
        }

        if (myPhotonView.gameObject.GetComponent<Explorer>() != null)
        {
            if (myPhotonView.gameObject.GetComponent<Explorer>().completelyCapsuled)
            {
                if (Input.GetButtonDown("Interact") || Input.GetKeyDown(KeyCode.Mouse0))
                {
                    spectateIndex++;
                    List<GameObject> explorers = getExplorerRefs();

                    if (spectateIndex >= explorers.Count || spectateIndex < 0)
                    {
                        spectateIndex = 0;
                    }

                    light.enabled = false;
                    playerToFollow = explorers[spectateIndex];
                    light = playerToFollow.GetComponent<Explorer>().light;
                    light.enabled = true;
                    Camera.main.gameObject.GetComponent<CameraFollowPlayer>().playerToFollow = playerToFollow;
                }
            }

            if (playerToFollow != null)
            {
                Explorer explorer = playerToFollow.GetComponent<Explorer>();

                explorer.light.pointLightOuterRadius = explorer.lightRadius;
            }
        }
    }

    private GameObject getCaptivatorRef()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playerList)
        {
            if (player.GetComponent<Captivator>() != null)
            {
                return player;
            }
        }

        return null;
    }

    private List<GameObject> getExplorerRefs()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> explorerList = new List<GameObject>();

        foreach (GameObject player in playerList)
        {
            if (player.GetComponent<Explorer>() != null)
            {
                explorerList.Add(player);
            }
        }

        return explorerList;
    }

    private void findLight()
    {
        if (light == null && playerToFollow != null)
        {
            if (playerToFollow.GetComponent<Explorer>() != null)
            {
                light = playerToFollow.GetComponent<Explorer>().light;
            } else if (playerToFollow.GetComponent<Captivator>() != null)
            {
                light = playerToFollow.GetComponent<Captivator>().light;
            }
        }
    }
}