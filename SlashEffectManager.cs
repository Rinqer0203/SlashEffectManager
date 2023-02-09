using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;

public class SlashEffectManager : MonoBehaviour
{
    public static SlashEffectManager instance = null;

    [Header("アタッチするエフェクトは自動再生をオフにして終了時コールバックするように設定する")]
    [SerializeField][Min(0)] private int _preLoadNum = 10;
    [SerializeField] private List<SlashEffectObject> _slashEffects;

    [System.Serializable]
    private class SlashEffectObject
    {
        public SlashEffectController slashEffectObj;
        public Vector2 scaleMagnification = Vector2.one;
        public RotationType type;
        [System.NonSerialized] public SlashEffectPool pool;

        //QuaternionとlocalScaleをキャッシュする
        public Quaternion defaultQuaternion => _defaultQuaternion ??= slashEffectObj.transform.rotation;
        public Vector2 defaultScale => _defaultScale ??= slashEffectObj.transform.localScale;
        private Vector2? _defaultScale = null;
        private Quaternion? _defaultQuaternion = null;
    }

    public class SlashEffectPool : ObjectPool<SlashEffectController>
    {
        private readonly Transform _parent;
        private readonly SlashEffectController _original;

        public SlashEffectPool(Transform parent, SlashEffectController original)
        {
            _parent = parent;
            _original = original;
        }

        protected override SlashEffectController CreateInstance()
        {
            return GameObject.Instantiate(_original, _parent);
        }
    }

    private enum RotationType
    {
        Random,
        Direction,
        None
    }

    private void Awake()
    {
        //シングルトン
        if (instance != null)
            Destroy(this);
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < _slashEffects.Count; i++)
        {
            if (_slashEffects[i].slashEffectObj != null)
            {
                _slashEffects[i].pool = new SlashEffectPool(transform, _slashEffects[i].slashEffectObj);
                _slashEffects[i].pool.PreloadAsync(_preLoadNum, _preLoadNum).Subscribe();
            }
        }
    }

    public void CreateSlashEffect(Vector2 attackerPos, Vector2 takePos)
    {
        for (int i = 0; i < _slashEffects.Count; i++)
        {
            var effect = _slashEffects[i].pool.Rent();
            effect.transform.position = takePos;
            effect.transform.localScale = _slashEffects[i].defaultScale;
            effect.transform.localScale *= _slashEffects[i].scaleMagnification;

            float zRotate = _slashEffects[i].type switch
            {
                RotationType.Random => Random.Range(0f, 360f),
                RotationType.Direction => (-GetAngle(attackerPos, takePos) + 90),
                _ => 0f
            };
            effect.transform.rotation = _slashEffects[i].defaultQuaternion;
            effect.transform.Rotate(0, 0, zRotate);

            effect.Play(_slashEffects[i].pool);
        }
    }

    private float GetAngle(Vector2 origin, Vector2 target)
    {
        Vector2 dt = target - origin;
        float rad = Mathf.Atan2(dt.x, dt.y);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }
}
