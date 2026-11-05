using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Assets._Project.Code.Runtime.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float smoothTime = 0.15f;
        private Transform player;
        private PixelPerfectCamera pixelPerfect;

        private Vector3 velocity;
        private Vector3 followPosition; // posição suave, fora da grade; a câmera usa ela arredondada

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            pixelPerfect = GetComponent<PixelPerfectCamera>();
            followPosition = transform.position;
        }

        private void LateUpdate()
        {
            if (player == null) return;

            Vector3 goal = new Vector3(player.position.x, player.position.y, transform.position.z);
            followPosition = Vector3.SmoothDamp(followPosition, goal, ref velocity, smoothTime);

            // Câmera sempre num pixel da arte: o cenário parado não vibra enquanto ela anda (AD-017)
            transform.position = pixelPerfect != null ? pixelPerfect.RoundToPixel(followPosition) : followPosition;
        }
    }
}
