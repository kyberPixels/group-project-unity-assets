using UnityEngine;

[CreateAssetMenu(fileName = "SupabaseConfig", menuName = "Supabase/Config")]
public class SupabaseConfig : ScriptableObject
{
    public string supabaseUrl;
    public string anonKey;
}
