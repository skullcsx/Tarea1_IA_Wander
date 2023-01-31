using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    public Rigidbody myRigidbody = null;
    GameObject PursuitTarget = null;
    Rigidbody PursuitTargetRB = null;
    public float fPredictionSteps = 10.0f;
    Vector3 v3SteeringForceAux = Vector3.zero;
    
    public enum SteeringBehavior { Seek, Flee, Pursue, Evade, Arrive, Wander }
    public SteeringBehavior currentBehavior = SteeringBehavior.Seek;

    //se declaran los valores que vamos a usar dandoles un valor inicial
    float wanderRadius = 10.0f;
    float wanderDistance = 20.0f;
    float wanderJitter = 1.0f;
    //declaramos una NavMeshAgent como un agente para que no se salga del mapa
    NavMeshAgent agent;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {   //el método start dara inicio al agent por donde puede estar, siempre siguiendo a la malla
        agent = this.GetComponent<NavMeshAgent>();
    }

    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);

    }
    //declaramos un vector3 que se llame wanderTarget y se pone en cero
    Vector3 wanderTarget = Vector3.zero;
    //creamos la clase Wander
    void Wander()
    {
    
        //aqui el vector que declaramos anteriormente estara haciendo que un valor random se multiplique con el movimiento brusco (cambio de direccion) del 
        //objeto, despues se pone en 0 y nuevamente realiza la misma acción
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);
        //normalizamos el vector
        wanderTarget.Normalize();
        //multiplicamos e igualamos el vector con el radio del wander
        wanderTarget *= wanderRadius;
        //se crea un vector como un objetivo local y lo igualamos con el wanderTarget mas una suma de un vector que contiene la distancia
        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        //se crea un vector con un objetivo global que contiene el objetivo local a la inversa
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);
        //se manda a llamar la clase Seek junto el vector global
        Seek(targetWorld);
    }
    private void OnDrawGizmos()
    {
        if (currentBehavior == SteeringBehavior.Pursue ||
            currentBehavior == SteeringBehavior.Evade)
        {
            Gizmos.color = Color.yellow;
            Vector3 nextPosition = PursuitTargetRB.transform.position +
                PursuitTargetRB.velocity * Time.fixedDeltaTime * fPredictionSteps;

            Gizmos.DrawSphere(nextPosition, 0.25f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + myRigidbody.velocity);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + v3SteeringForceAux);

        if (currentBehavior == SteeringBehavior.Wander)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + wanderTarget);
            Gizmos.DrawWireSphere(wanderTarget, wanderRadius);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //se ejecuta la clase Wander
        Wander();
        //Flee(target.transform.position);
        //Seek(target.transform.position);
    }
}
