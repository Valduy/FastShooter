using System;
using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

namespace MoreMountains.CorgiEngine
{	
	/// <summary>
	/// Add this component to an object and it will cause heal to objects that collide with it. 
	/// </summary>
	public class HealOnTouch : MonoBehaviour 
	{
		[Header("Targets")]
		[MMInformation("This component will make your object cause damage to objects that collide with it. Here you can define what layers will be affected by the damage (for a standard enemy, choose Player), how much damage to give, and how much force should be applied to the object that gets the damage on hit. You can also specify how long the post-hit invincibility should last (in seconds).",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// the layers that will be damaged by this object
		[Tooltip("the layers that will be damaged by this object")]
		public LayerMask TargetLayerMask;

		[Header("Damage Caused")]
		/// The minimum amount of health to remove from the player's health
		[FormerlySerializedAs("HealCaused")]
		[Tooltip("The minimum amount of health to remove from the player's health")]
		public float MinHealCaused = 10f;
		/// The maximum amount of health to remove from the player's health
		[Tooltip("The amount of health to remove from the player's health")]
		public float MaxHealCaused = 10f;

		[Header("Damage Taken")]
		[MMInformation("After having applied the heal to whatever it collided with, you can have this object hurt itself. A bullet will explode after hitting a wall for example. Here you can define how much damage it'll take every time it hits something, or only when hitting something that's damageable, or non damageable. Note that this object will need a Health component too for this to be useful.",MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// The amount of damage taken every time, whether what we collide with is damageable or not
		[Tooltip("The amount of damage taken every time, whether what we collide with is damageable or not")]
		public float DamageTakenEveryTime = 0;
		/// The amount of damage taken when colliding with a damageable object
		[Tooltip("The amount of damage taken when colliding with a damageable object")]
		public float DamageTakenDamageable = 0;
		/// The amount of damage taken when colliding with something that is not damageable
		[Tooltip("The amount of damage taken when colliding with something that is not damageable")]
		public float DamageTakenNonDamageable = 0;
		/// The duration of the invincibility frames after the hit (in seconds)
		[Tooltip("The duration of the invincibility frames after the hit (in seconds)")]
		public float DamageTakenInvincibilityDuration = 0.5f;

		[Header("Feedback")]

		/// the feedback to play when applying damage to a damageable
		[Tooltip("the feedback to play when applying damage to a damageable")]
		public MMFeedbacks HitDamageableFeedback;
		/// the feedback to play when applying damage to a non damageable
		[Tooltip("the feedback to play when applying damage to a non damageable")]
		public MMFeedbacks HitNonDamageableFeedback;
		/// the duration of freeze frames on hit (leave it at 0 to ignore)
		[Tooltip("the duration of freeze frames on hit (leave it at 0 to ignore)")]
		public float FreezeFramesOnHitDuration = 0f;

		/// the owner of the DamageOnTouch zone
		[MMReadOnly]
		[Tooltip("the owner of the DamageOnTouch zone")]
		public GameObject Owner;
		
		/// a delegate used to communicate hit events 
		public delegate void OnHitDelegate();
		public OnHitDelegate OnHit;
		public OnHitDelegate OnHitDamageable;
		public OnHitDelegate OnHitNonDamageable;
		public OnHitDelegate OnKill;

		// storage		
		protected Vector2 _lastPosition, _lastDamagePosition, _velocity, _damageDirection;
		protected float _startTime = 0f;
		protected Collider2D _collidingCollider;
		protected Health _colliderHealth;
		protected CorgiController _corgiController;
		protected Health _health;
		protected List<GameObject> _ignoredGameObjects;
		protected Color _gizmosColor;
		protected Vector3 _gizmoSize;

		protected CircleCollider2D _circleCollider2D;
		protected BoxCollider2D _boxCollider2D;
		protected bool _initializedFeedbacks = false;

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Awake()
		{
			if (_ignoredGameObjects == null)
			{
				_ignoredGameObjects = new List<GameObject>();	
			}
			_health = this.gameObject.GetComponent<Health>();
			_corgiController = this.gameObject.GetComponent<CorgiController> ();
			
			_boxCollider2D = this.gameObject.GetComponent<BoxCollider2D>();
			_circleCollider2D = this.gameObject.GetComponent<CircleCollider2D>();
			if (_boxCollider2D != null) { _boxCollider2D.isTrigger = true; }
			if (_circleCollider2D != null) { _circleCollider2D.isTrigger = true; }
            
			_gizmosColor = Color.red;
			_gizmosColor.a = 0.25f;
			_lastPosition = this.transform.position;
			_lastDamagePosition = this.transform.position;
			InitializeFeedbacks();
		}

		/// <summary>
		/// A public method you can use to set the controller from another class
		/// </summary>
		/// <param name="newController"></param>
		public virtual void SetCorgiController(CorgiController newController)
		{
			_corgiController = newController;
		}
		
		protected virtual void InitializeFeedbacks()
		{
			if (_initializedFeedbacks)
			{
				return;
			}
			HitDamageableFeedback?.Initialization(this.gameObject);
			HitNonDamageableFeedback?.Initialization(this.gameObject);
			
			_initializedFeedbacks = true;
		}

		/// <summary>
		/// OnEnable we set the start time to the current timestamp
		/// </summary>
		protected virtual void OnEnable()
		{
			_startTime = Time.time;
			_lastPosition = this.transform.position;
			_lastDamagePosition = this.transform.position;
		}

		/// <summary>
		/// On Disable we clear our ignore list
		/// </summary>
		protected void OnDisable()
		{
			ClearIgnoreList();
		}

		/// <summary>
		/// During last update, we store the position and velocity of the object
		/// </summary>
		protected virtual void Update () 
		{
			ComputeVelocity();
		}

		/// <summary>
		/// Adds the gameobject set in parameters to the ignore list
		/// </summary>
		/// <param name="newIgnoredGameObject">New ignored game object.</param>
		public virtual void IgnoreGameObject(GameObject newIgnoredGameObject)
		{
			if (_ignoredGameObjects == null)
			{
				_ignoredGameObjects = new List<GameObject>();
			}
			_ignoredGameObjects.Add(newIgnoredGameObject);
		}

		/// <summary>
		/// Removes the object set in parameters from the ignore list
		/// </summary>
		/// <param name="ignoredGameObject">Ignored game object.</param>
		public virtual void StopIgnoringObject(GameObject ignoredGameObject)
		{
			_ignoredGameObjects.Remove(ignoredGameObject);
		}

		/// <summary>
		/// Clears the ignore list.
		/// </summary>
		public virtual void ClearIgnoreList()
		{
			if (_ignoredGameObjects != null)
			{
				_ignoredGameObjects.Clear();	
			}
		}

		/// <summary>
		/// Computes the velocity based on the object's last position
		/// </summary>
		protected virtual void ComputeVelocity()
		{
			_velocity = (_lastPosition - (Vector2)transform.position) /Time.deltaTime;

			if (Vector3.Distance(_lastDamagePosition, this.transform.position) > 0.1f)
			{
				_damageDirection = (Vector2)this.transform.position - _lastDamagePosition;
				_lastDamagePosition = this.transform.position;
			}

			_lastPosition = transform.position;
		}
		
		/// <summary>
		/// When a collision with the player is triggered, we give damage to the player and knock it back
		/// </summary>
		/// <param name="collider">what's colliding with the object.</param>
		public virtual void OnTriggerStay2D(Collider2D collider)
		{			
			Colliding (collider);
		}

		public virtual void OnTriggerEnter2D(Collider2D collider)
		{			
			Colliding (collider);
		}

		protected virtual void Colliding(Collider2D collider)
		{
			if (!this.isActiveAndEnabled)
			{
				return;
			}

			// if the object we're colliding with is part of our ignore list, we do nothing and exit
			if (_ignoredGameObjects.Contains(collider.gameObject))
			{
				return;
			}

			// if what we're colliding with isn't part of the target layers, we do nothing and exit
			if (!MMLayers.LayerInLayerMask(collider.gameObject.layer,TargetLayerMask))
			{
				return;
			}

			_collidingCollider = collider;
			_colliderHealth = collider.gameObject.MMGetComponentNoAlloc<Health>();

			OnHit?.Invoke();
			
			// if what we're colliding with is damageable
			if ((_colliderHealth != null) && (_colliderHealth.enabled))
			{
				if(_colliderHealth.CurrentHealth > 0)
				{
					OnCollideWithHealable(_colliderHealth);
				}
			}
			// if what we're colliding with can't be damaged
			else
			{
				OnCollideWithNonHealable();
			}
		}

		/// <summary>
		/// Describes what happens when colliding with a damageable object
		/// </summary>
		/// <param name="health">Health.</param>
		protected virtual void OnCollideWithHealable(Health health)
		{	
			float randomHeal = UnityEngine.Random.Range(MinHealCaused, Mathf.Max(MaxHealCaused, MinHealCaused));
			
			OnHitDamageable?.Invoke();

			HitDamageableFeedback?.PlayFeedbacks(this.transform.position);

			if ((FreezeFramesOnHitDuration > 0) && (Time.timeScale > 0))
			{
				MMFreezeFrameEvent.Trigger(Mathf.Abs(FreezeFramesOnHitDuration));
			}

			_colliderHealth.Heal(randomHeal, gameObject);
			
			SelfDamage(DamageTakenEveryTime + DamageTakenDamageable);
		}

		/// <summary>
		/// Describes what happens when colliding with a non damageable object
		/// </summary>
		protected virtual void OnCollideWithNonHealable()
		{
			OnHitNonDamageable?.Invoke();
	        
			if (DamageTakenEveryTime + DamageTakenNonDamageable > 0)
			{
				HitNonDamageableFeedback?.PlayFeedbacks(this.transform.position);
				SelfDamage(DamageTakenEveryTime + DamageTakenNonDamageable);
			}
		}

		/// <summary>
		/// Applies damage to itself
		/// </summary>
		/// <param name="damage">Damage.</param>
		protected virtual void SelfDamage(float damage)
		{
			if (_health != null)
			{
				_damageDirection = Vector3.up;
				_health.Damage(damage,gameObject,0f,DamageTakenInvincibilityDuration, _damageDirection);
			}	
		}

		/// <summary>
		/// A method used to draw gizmos
		/// </summary>
		protected virtual void OnDrawGizmos()
		{
			Gizmos.color = _gizmosColor;

			if ((_boxCollider2D != null) && _boxCollider2D.enabled)
			{
				_gizmoSize.x =  _boxCollider2D.bounds.size.x ;
				_gizmoSize.y =  _boxCollider2D.bounds.size.y ;
				_gizmoSize.z = 1f;
				Gizmos.DrawCube(_boxCollider2D.bounds.center, _gizmoSize);
			}
			if (_circleCollider2D != null && _circleCollider2D.enabled)
			{
				Gizmos.DrawSphere((Vector2)this.transform.position + _circleCollider2D.offset, _circleCollider2D.radius);                
			}
		}
	}
}