using UnityEngine;

public class CubeBehaviour : MonoBehaviour {

    void Start() {
        if (Physics.Raycast(transform.position, transform.up)) {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }
}
