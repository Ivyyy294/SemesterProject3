using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ivyyy.Network;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkBehaviour))]
[RequireComponent(typeof(Ball))]
[RequireComponent(typeof(Rigidbody))]
public class CrawlyBrain : MonoBehaviour
{

    [Header("Parameters")] 
    [SerializeField] private TimeCounter idleTime;
    [SerializeField] private float directionRandomization;
    [Min(0)][SerializeField] private float chaosSpeed = 1f;
    [SerializeField] private float randomBurstFrequencySeconds = 5f;
    [SerializeField] private float randomBurstForce = 10f;
    [SerializeField] private TimeCounter randomBurstTime;
    [SerializeField] private AnimationCurve randomBurstDistribution;
    [SerializeField] private float wallEvadeStrength = 1;

    [Header("References")] 
    [SerializeField] private Transform ballModel;
    
    private NetworkBehaviour _networkBehaviour;
    private Ball _ball;
    private Rigidbody _rigidbody;
    private Vector3 _randomDirection;
    private float _initialSeed;
    
    private RaycastHit _raycastHit;
    private bool _raycastIsHit;
    private bool _isSleeping;
    private List<Transform> _goals;

    public bool BrainActive => !_isSleeping && _networkBehaviour.Owner && _ball.CurrentPlayerId == -1;
    public bool CanMove => BrainActive && idleTime.TimeRemaining < 0;
	public bool IsSleeping => _isSleeping;

    void Start()
    {
        _networkBehaviour = GetComponent<NetworkBehaviour>();
        _ball = GetComponent<Ball>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        ResetBrain();
        _isSleeping = true;
    }

    public void WakeUp()
    {
        if (!_isSleeping) return;
        _isSleeping = false;
        _goals = FindObjectsOfType<Goal>().Select(x => x.transform).ToList();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_ball.CurrentPlayerId != -1) WakeUp();
        
        if (BrainActive)
        {
            idleTime.Update();
            randomBurstTime.Update();
        }
        if (CanMove) Move();
    }

    Vector3 ChangeVelocity(Vector3 currentVelocity)
    {
        var seed = Time.time * chaosSpeed + _initialSeed;
        var x = Mathf.PerlinNoise1D(seed) - 0.5f;
        var y = Mathf.PerlinNoise1D(seed + 124) - 0.5f;
        var z = Mathf.PerlinNoise1D(seed -45) - 0.5f;
        _randomDirection = (new Vector3(x, y, z)).normalized;
        var velocity = currentVelocity;
        
        if (randomBurstTime.ProgressNormalized < 1)
        {
            var burstSpeed = randomBurstDistribution.Evaluate(randomBurstTime.ProgressNormalized);
            burstSpeed *= Time.deltaTime * randomBurstForce;
            velocity += ballModel.transform.forward * burstSpeed;
            _rigidbody.velocity = velocity;
        }

        return velocity;
    }

    Vector3 Steer(Vector3 currentVelocity, float speed)
    {
        //randomize direction
        if (speed > 1f)
        {
            var velocityOffset = directionRandomization * Time.deltaTime * _randomDirection;
            currentVelocity += velocityOffset * Mathf.Clamp01(speed);
        }
        
        //evade walls
        Ray ray = new(transform.position, ballModel.forward);
        _raycastIsHit = Physics.Raycast(ray, out _raycastHit, 5);
        if (_raycastIsHit)
        {

            var evadeStrength = 1 / Mathf.Pow(_raycastHit.distance, 2);
            evadeStrength *= wallEvadeStrength;
            currentVelocity += Time.deltaTime * evadeStrength * _raycastHit.normal;
        }
        return currentVelocity;
    }
    void Move()
    {
        var newVelocity = ChangeVelocity(_rigidbody.velocity);
        var speed = newVelocity.magnitude;
        
        newVelocity = Steer(newVelocity, speed).normalized * speed;
        
        _rigidbody.velocity = newVelocity;
        
        if (newVelocity.magnitude < 0.4f)
        {
            var shouldBoost = Random.value < randomBurstFrequencySeconds * Time.deltaTime;
            if (shouldBoost)
            {
                AvoidGoals();
                randomBurstTime.Reset();
            }
        }
    }

    void AvoidGoals()
    {
        if (_goals.Count == 0) return;
        
        var nearest = _goals.OrderBy(x => (x.position - transform.position).sqrMagnitude).First();
        var delta = nearest.position - transform.position;
        if (delta.magnitude > 6) return;
      
        transform.forward = -delta.normalized;
    }
    
    public void ResetBrain()
    {
        idleTime.Reset();
        randomBurstTime.MakeReady();
        _initialSeed = Random.value * 1000;
        _isSleeping = true;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position, transform.position + _randomDirection * 4);
        // if (_isRaycasting)
        // {
        //     Gizmos.color = _raycastIsHit? Color.yellow : Color.black;
        //     Gizmos.DrawLine(transform.position, _raycastHit.point);
        //     Gizmos.DrawSphere(_raycastHit.point, 0.1f);
        // }
    }
    #endif
}
