using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Photon.Pun;

public class FlashLight : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;

    public new PhotonView photonView;
    public new Light2D light;

    public float duration = 0;
    public int rayCount;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        drawMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (duration <= 0)
        {
            light.enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            return;
        }

        if (photonView.IsMine)
        {
            lookAtMouse();
            GetComponent<MeshRenderer>().enabled = true;
        } else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        duration -= Time.deltaTime;

        light.enabled = true;

        handleRayCasts();
    }

    private void handleRayCasts()
    {
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 angle = Vector2.Lerp(this.transform.up - (this.transform.right / 4),
                this.transform.up + (this.transform.right / 4), (i * 1.0f) / (rayCount * 1.0f));

            RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, angle, light.pointLightOuterRadius);

            for (int k = 0; k < hits.Length; k++)
            {
                if (hits[k].transform != this.transform.parent)
                {
                    if (hits[k].collider.gameObject.layer == LayerMask.NameToLayer("Explorer") ||
                        hits[k].collider.gameObject.layer == LayerMask.NameToLayer("Captivator"))
                    {
                        Character character = null;

                        if (hits[k].collider.gameObject.GetComponent<Explorer>())
                        {
                            character = hits[k].collider.gameObject.GetComponent<Explorer>();
                        }
                        else if (hits[k].collider.gameObject.GetComponent<Captivator>())
                        {
                            character = hits[k].collider.gameObject.GetComponent<Captivator>();
                        }

                        if (character == null)
                        {
                            continue;
                        }

                        if (character.flashDuration < character.MAX_FLASH_DURATION)
                        {
                            character.flashDuration += (light.pointLightOuterRadius / Vector2.Distance(character.transform.position, this.transform.position)) * Time.deltaTime;
                        }
                    }
                }
            }
        }
    }

    private void drawMesh()
    {
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[vertices.Length * 3];

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 angle = Vector2.Lerp(this.transform.up - (this.transform.right / 4),
                this.transform.up + (this.transform.right / 4), (i * 1.0f) / (rayCount * 1.0f));

            vertices[0] = this.transform.localPosition;

            vertices[vertexIndex] = this.transform.localPosition + (angle * light.pointLightOuterRadius);

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
        mesh.bounds = new Bounds(this.transform.localPosition, Vector3.one * 1000f);
    }

    private void lookAtMouse()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 dir = position - this.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(duration);
        } else
        {
            duration = (float)stream.ReceiveNext();
        }
    }
}