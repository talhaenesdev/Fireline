using FlowIoC.BaseModule.Root;
using UnityEngine;

namespace Modules.MainModule.Entities
{
    /// <summary>
    /// Keeps a Root that makes itself persistent - AssetServiceRoot - to one instance. MainScene is
    /// loaded again when a player leaves a match, and the copy authored in it would bind the
    /// persistent context's signals a second time. This runs before the Root's own Awake and
    /// destroys the copy while it has bound nothing.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public class PersistentRootGuard : MonoBehaviour
    {
        private void Awake()
        {
            RootBase root = GetComponent<RootBase>();
            if (root == null)
                return;

            foreach (RootBase other in FindObjectsByType<RootBase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (other == root || other.GetType() != root.GetType())
                    continue;

                // The survivor is the one already moved out of the scenes.
                if (other.gameObject.scene.name != "DontDestroyOnLoad")
                    continue;

                DestroyImmediate(gameObject);
                return;
            }
        }
    }
}
