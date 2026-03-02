using UnityEngine;

[CreateAssetMenu(fileName = "FootstepDatabase", menuName = "Xlab/Audio/Footstep Database")]
public sealed class FootstepDatabase : ScriptableObject
{
    [SerializeField] private FootstepSurface[] m_surfaces;

    public FootstepSurface[] surfaces => m_surfaces;

    public FootstepSurface GetSurface(string layerName)
    {
        if (m_surfaces == null)
            return null;

        for (int i = 0; i < m_surfaces.Length; i++)
        {
            if (m_surfaces[i] == null)
                continue;

            if (m_surfaces[i].layerName == layerName)
                return m_surfaces[i];
        }

        return null;
    }
}