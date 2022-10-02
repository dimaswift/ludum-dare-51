using Core;
using MobRoulette.Core.Events;
using UnityEngine;

namespace MobRoulette.Core.Behaviours
{
    public class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] private float followSpeed = 1f;
        [Range(0, 1)]
        [SerializeField] private float offsetX = 0.5f;

        [Range(0, 1)]
        [SerializeField] private float offsetY = 0.5f;

        [SerializeField] private float minSize;
        [SerializeField] private float maxSize;
        private Transform camTransform;
        private Transform playerTransform;
        private Camera cam;

        private float targetZoom;
        private float defaultZoom;
        
        private void Awake()
        {
            
            cam = Camera.main;
            targetZoom = cam.orthographicSize;
            defaultZoom = targetZoom;
            camTransform = cam.transform;
            EventBus.Subscribe<OnGameStateChanged, GameState>(state =>
            {
                if (state == GameState.Playing)
                {
                    targetZoom = defaultZoom;
                    cam.orthographicSize = defaultZoom;
                    playerTransform = Game.Instance.Player.transform;
                    camTransform.position = GetTargetPos();
                }
            });
        }

        private Vector2 GetOffset()
        {
            var playerPos = playerTransform.position;
            var mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            var offset = (mouse - playerPos);
            return new Vector2(offset.x * offsetX, offset.y * offsetY);
        }
        
        private Vector3 GetTargetPos()
        {
            var offset = GetOffset();
            var camPos = camTransform.position;
            var playerPos = playerTransform.position;
            return new Vector3(playerPos.x + offset.x, playerPos.y + offset.y, camPos.z);
        }
        
        private void Update()
        {
            if (playerTransform == null || Game.Instance.State.Value != GameState.Playing)
            {
                return;
            }

            targetZoom = Mathf.Clamp(targetZoom + -Input.mouseScrollDelta.y, minSize, maxSize);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 10f);
            camTransform.position = Vector3.Lerp(camTransform.position, GetTargetPos(), followSpeed * Time.deltaTime);
        }
    }
}