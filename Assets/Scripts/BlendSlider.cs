using UnityEngine;

public class BlendSlider : MonoBehaviour {
    //~ inspector (private)
    [SerializeField][Tooltip("List of Materials with the \"Sprite Blender\" shader")]
    private Material[] blendMaterials;

#if UNITY_EDITOR
    [SerializeField][Range(0f, 1f)][Tooltip("Modifies the value of every sprite (for testing)")]
    private float testHere = 1f;

    //~ unity methods (private)
    private void OnValidate() => this.BlendEnvironment(this.testHere);
#endif

    //~ public methods
    /// <summary> Changes the Lerp value for every <see cref="Material"/> in <see cref="blendMaterials"/> to given <paramref name="value"/> </summary>
    /// <param name="value"> The new value - gets clamped to [0:1] </param>
    public void BlendEnvironment(float value){
        value = Mathf.Clamp01(value);
        foreach(Material mat in this.blendMaterials)
            mat.SetFloat("_Lerp", value);
    }
}
