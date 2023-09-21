using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class Bee : MonoBehaviour
{
    [FormerlySerializedAs("_skeleton")] public SkeletonAnimation skeleton;
    [SerializeField] private AnimationReferenceAsset _shortJump, _fly, _end, _idle;
    [SerializeField] private ItemTile _currentTile;
    private float _distanceFromOrigin = 50f;
    
    private Vector3 _direction;
    private Vector3 _targetPosition;

    public void JumpToTile()
    {
        var isOut = GameLevelManager.Instance.CheckTileCount();
        _currentTile.HasBee = true;
       
        if (isOut)
        {
            _targetPosition = Random.insideUnitSphere.normalized * _distanceFromOrigin;
            JumpOutScreen();
           
        }
        else
        {
            ItemTile target;
            do
            {
                target = GameLevelManager.Instance.GetRandomTileCanTouch();
            } while (target == _currentTile);
            _currentTile = target;
            _currentTile.HasBee = false;
            _targetPosition = target.transform.position;
            Jump();
        }

    }

    private void Jump()
    {
        _direction = (_targetPosition - transform.position).normalized;
        transform.right = (Vector2)_direction;
        transform.parent = _currentTile.bg.transform;
        
        transform.DOMove(_targetPosition, _shortJump.Animation.Duration);
        
        skeleton.AnimationState.SetAnimation(0, _shortJump, false);
        skeleton.AnimationState.AddAnimation(0, _idle, true, 0);
    }

    private void JumpOutScreen()
    {
        _direction = (_targetPosition - transform.position).normalized;
        transform.right = (Vector2)_direction;
        transform.DOMove(_targetPosition, 5);
        skeleton.AnimationState.SetAnimation(0, _fly, true);
        Invoke(nameof(Destroy), 10f);
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }
}