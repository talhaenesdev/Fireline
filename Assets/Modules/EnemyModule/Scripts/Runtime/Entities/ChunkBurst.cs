using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>
    /// Little cubes thrown out of a point, falling and bouncing on the ground before they shrink away: a
    /// hit's blood, a zombie falling apart, a wall breaking. One particle system serves every burst.
    /// </summary>
    public class ChunkBurst : MonoBehaviour
    {
        private const string MATERIAL_PATH = "Effects/MT_EnemyChunk";

        private ParticleSystem _system;

        public static ChunkBurst Create(Transform parent)
        {
            var holder = new GameObject("ChunkBurst");
            holder.transform.SetParent(parent, false);
            var burst = holder.AddComponent<ChunkBurst>();
            burst.Build();
            return burst;
        }

        public void Burst(Vector3 position, Color color, int count, float speed, float size)
        {
            if (_system == null)
                return;

            var emit = new ParticleSystem.EmitParams();
            for (int i = 0; i < count; i++)
            {
                Vector3 direction = Random.insideUnitSphere;
                direction.y = Mathf.Abs(direction.y) + 0.6f;
                emit.position = position + Vector3.up * 0.8f + Random.insideUnitSphere * 0.25f;
                emit.velocity = direction.normalized * (speed * Random.Range(0.5f, 1.1f));
                emit.startColor = color * Random.Range(0.75f, 1.15f);
                emit.startSize = size * Random.Range(0.6f, 1.3f);
                emit.rotation3D = new Vector3(Random.value, Random.value, Random.value) * 360f;
                emit.angularVelocity3D = Random.insideUnitSphere * 400f;
                emit.startLifetime = Random.Range(0.9f, 1.6f);
                _system.Emit(emit, 1);
            }
        }

        private void Build()
        {
            _system = gameObject.AddComponent<ParticleSystem>();
            _system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            ParticleSystem.MainModule main = _system.main;
            main.loop = false;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startRotation3D = true;
            main.gravityModifier = 2.2f;
            main.maxParticles = 800;

            ParticleSystem.EmissionModule emission = _system.emission;
            emission.enabled = false;

            ParticleSystem.ShapeModule shape = _system.shape;
            shape.enabled = false;

            // The ground stops them; they skid, settle and shrink away.
            Transform ground = new GameObject("Ground").transform;
            ground.SetParent(transform, false);
            ground.position = Vector3.zero;
            ParticleSystem.CollisionModule collision = _system.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.Planes;
            collision.SetPlane(0, ground);
            collision.bounce = 0.3f;
            collision.dampen = 0.5f;
            collision.radiusScale = 0.5f;

            ParticleSystem.SizeOverLifetimeModule shrink = _system.sizeOverLifetime;
            shrink.enabled = true;
            shrink.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(
                new Keyframe(0f, 1f), new Keyframe(0.7f, 1f), new Keyframe(1f, 0f)));

            var particleRenderer = GetComponent<ParticleSystemRenderer>();
            particleRenderer.renderMode = ParticleSystemRenderMode.Mesh;
            particleRenderer.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            particleRenderer.alignment = ParticleSystemRenderSpace.World;
            particleRenderer.sharedMaterial = Resources.Load<Material>(MATERIAL_PATH);
            particleRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            _system.Play();
        }
    }
}
