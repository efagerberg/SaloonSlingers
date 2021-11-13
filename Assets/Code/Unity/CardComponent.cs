using UnityEngine;

using GambitSimulator.Core;

namespace GambitSimulator.Unity
{
    public class CardComponent : MonoBehaviour
    {
        [SerializeField]
        private Card card;
        [SerializeField]
        private Renderer faceRenderer;
        [SerializeField]
        private float maxSpinSpeed = 50;
        [SerializeField]
        private bool shouldAddTracer = true;
        [SerializeField]
        private GameObject destroyEffectPrefab;
        [SerializeField]
        private float effectLifetime = 2f;
        [SerializeField]
        private float curveFactor = 0.01f;

        private Rigidbody m_rb;

        private void Start()
        {
            m_rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            name = card.ToString();
            UpdateGraphic();
        }

        private void FixedUpdate()
        {
            AddCurveForce();
        }

        private void AddCurveForce()
        {
            var curveForce = new Vector3(m_rb.angularVelocity.normalized.y,
                                         m_rb.angularVelocity.normalized.x,
                                         0) * curveFactor;

            m_rb.AddForce(curveForce);
        }

        public Card GetCard()
        {
            return card;
        }

        public void SetCard(Card inCard)
        {
            card = inCard;
        }

        public void UpdateGraphic()
        {
            if (card.Equals(null)) return;
            faceRenderer.material.mainTexture = Resources.Load<Texture>(GetTexturePath(card));
        }

        public void Throw(Vector3 linearVelocity)
        {
            m_rb = GetComponent<Rigidbody>();
            m_rb.maxAngularVelocity = maxSpinSpeed;
            m_rb.isKinematic = false;
            m_rb.AddForce(linearVelocity);

            // Spin card in direction of velocity
            m_rb.AddRelativeTorque(linearVelocity);

            Destroy(gameObject, 2f);
            transform.parent = null;

            if (shouldAddTracer)
                AddTracer();
        }

        private void AddTracer()
        {
            TrailRenderer tr = gameObject.AddComponent<TrailRenderer>();
            tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tr.time = 0.2f;
            var keyframes = new Keyframe[] {
            new Keyframe(0f, 0.1f),
            new Keyframe(0.2f, 0.05f),
            new Keyframe(0.8f, 0.003f),
            new Keyframe(1f, 0f)
        };
            tr.widthCurve = new AnimationCurve(keyframes);
            tr.material = Resources.Load<Material>("Materials/TracerMaterial");
        }

        public void Burn()
        {
            if (destroyEffectPrefab)
            {
                var _go = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
                Destroy(_go, effectLifetime);
            }
        }

        private string GetTexturePath(Card card)
        {
            return string.Format("Textures/{0}", card.ToString());
        }

        private string GetSpritePath(Card card)
        {
            return string.Format("Sprites/{0}", card.ToString());
        }
    }
}
