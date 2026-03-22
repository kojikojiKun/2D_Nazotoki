using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    WatarDrop
}

[System.Serializable]
public class ParticleData
{
    public ParticleType Type;
    public ParticleSystem PlayingParticle;
}

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] ParticleData[] m_data;
    private Dictionary<ParticleType, ParticleSystem> m_dictionary;
    public static ParticlePlayer s_instance;

    private void Awake()
    {
        //インスタンス化.
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(s_instance);

        //ListをDictionaryに変換.
        m_dictionary = new Dictionary<ParticleType, ParticleSystem>();
        foreach(var data in m_data)
        {
            m_dictionary[data.Type] = data.PlayingParticle;
        }
    }

    //typeに応じたParticleを再生.
    public void PlayParticle(ParticleType type)
    {
        if (m_dictionary.TryGetValue(type, out var data))
        {
            data.Play();
        }
    }
}
