using System;
using UnityEngine;
using System.Collections.Generic;
using QFramework;

[RequireComponent(typeof(Collider2D))]
public class SoyoWaterEffector : MonoBehaviour
{
    [System.Serializable]
    public class WaterLayerParams
    {
        public string layerName = "Default";
        public float baseForce = 5f; // 浮力基值
        public float amplitude = 3f; // 正弦波振幅
        public float frequency = 1f; // 正弦波频率
        public float drag = 1f; // 线性阻力
        public float angularDrag = 1f; // 角阻力
    }

    [Tooltip("设置配置数据")] public bool initConfig = false;
    public WaterLayerParams[] layerParams;

    // 为了快速查找
    Dictionary<int, WaterLayerParams> paramDict;

    private void Update()
    {
        if (initConfig)
        {
            initConfig = false;
            paramDict = new Dictionary<int, WaterLayerParams>();
            foreach (var param in layerParams)
            {
                int layer = LayerMask.NameToLayer(param.layerName);
                if (!paramDict.ContainsKey(layer))
                {
                    paramDict.Add(layer, param);
                }
            }
        }
    }

    void Awake()
    {
        paramDict = new Dictionary<int, WaterLayerParams>();
        foreach (var param in layerParams)
        {
            int layer = LayerMask.NameToLayer(param.layerName);
            if (!paramDict.ContainsKey(layer))
            {
                paramDict.Add(layer, param);
            }
        }

        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (rb == null) return;

        int layer = other.gameObject.layer;
        WaterLayerParams param;
        if (!paramDict.TryGetValue(layer, out param))
        {
            // 如果没有专属参数，用第一个参数做默认
            param = layerParams.Length > 0 ? layerParams[0] : null;
        }

        if (param == null) return;

        // 施加浮力（正弦波效果）
        float sinForce = param.baseForce + Mathf.Cos(Time.time * param.frequency) * param.amplitude;
        rb.AddForce(Vector2.up * sinForce, ForceMode2D.Force);

        // 施加阻力（用力模拟，而不是直接改 drag）
        Vector2 velocity = rb.velocity;
        Vector2 dragForce = -velocity * param.drag; // drag可以调成一个阻力系数
        rb.AddForce(dragForce, ForceMode2D.Force);

        // 角阻力同理 有bug，暂时禁用了
        // float angularDragForce = -rb.angularVelocity * param.angularDrag;
        // rb.AddTorque(angularDragForce, ForceMode2D.Force);
    }
}