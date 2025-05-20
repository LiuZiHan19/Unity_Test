using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
[RequireComponent(typeof(RawImage))]
public class EraseHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    //擦除完成调用事件
    public Action eraseFinishEvent;
 
    //笔刷半径
    [SerializeField] int brushRadius = 50;
 
    //擦除比例，擦除比例高于该值，是为擦除完成，自动擦除剩余部分
    [SerializeField] float finishPercent = 0.9f;
 
    //擦除点偏移量,距离上个擦除点>=该值时开始新的擦除点
    [SerializeField] float drawOffset = 10f;
 
    //是否以擦除完成
    bool isFinish;
 
    //要擦除的图片
    RawImage eraseImage;
    Texture2D eraseTexture;
    //图片长宽
    int textureWidth;
    int textureHeight;
 
    //图片大小
    float textureLength;
    //擦除部分图片大小
    float eraseLength;
 
    Camera mainCamera;
 
    void Awake()
    {
        eraseImage = GetComponent<RawImage>();
        mainCamera = Camera.main;
    }
 
    void Start()
    {
        Init();
    }
 
    void Init()
    {
        isFinish = false;
        eraseLength = 0;
 
        //原擦除图片
        //Texture2D originalTexture = (Texture2D)eraseImage.mainTexture;
        Texture2D originalTexture = (Texture2D)eraseImage.texture;
 
        //被擦除的图片，展示擦除过程
        eraseTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.ARGB32, false);
        textureWidth = eraseTexture.width;
        textureHeight = eraseTexture.height;
        eraseTexture.SetPixels(originalTexture.GetPixels());
        eraseTexture.Apply();
 
        Debug.Log(textureWidth + " - " + textureHeight);
 
        eraseImage.texture = eraseTexture;
 
        textureLength = eraseTexture.GetPixels().Length;
    }
 
    #region Pointer Event
 
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isFinish)
            return;
 
        tempLastPoint = eventData.position;
        ErasePoint(eventData.position);
    }
 
    Vector2 tempEventPoint;
    Vector2 tempLastPoint;
    public void OnDrag(PointerEventData eventData)
    {
        if (isFinish)
            return;
 
        tempEventPoint = eventData.position;
 
        //距离上个擦除点 >= 该值时开始新的擦除点
        if ((tempEventPoint - tempLastPoint).sqrMagnitude < drawOffset * drawOffset)
            return;
 
        //擦除点
        ErasePoint(tempEventPoint);
        //记录点
        tempLastPoint = tempEventPoint;
    }
 
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isFinish)
            return;
 
        ErasePoint(eventData.position);
    }
 
    #endregion
 
    Vector3 tempWorldPoint;
    Vector3 tempLocalPoint;
    Vector2Int pixelPos;
    void ErasePoint(Vector2 screenPos)
    {
        //点击位置坐标转换，正交相机无效，试着改成平行相机试一下
        tempWorldPoint = mainCamera.ScreenToWorldPoint(screenPos);
        tempLocalPoint = transform.InverseTransformPoint(tempWorldPoint);
 
        //相对图片像素点坐标
        pixelPos.x = (int)tempLocalPoint.x + textureWidth / 2;
        pixelPos.y = (int)tempLocalPoint.y + textureHeight / 2;
 
        //点击位置是否在图片范围内
        if (pixelPos.x < 0 || pixelPos.x >= textureWidth || pixelPos.y < 0 || pixelPos.y >= textureHeight)
            return;
 
        //遍历笔刷长宽范围内像素点
        for (int i = -brushRadius; i <= brushRadius; i++)
        {
            //超左/右边界
            if (pixelPos.x + i < 0 || pixelPos.x + i >= textureWidth)
                continue;
 
            for (int j = -brushRadius; j <= brushRadius; j++)
            {
                //超上/下边界
                if (pixelPos.y + j < 0 || pixelPos.y + j >= textureHeight)
                    continue;
 
                //是否在圆形范围内
                if (Mathf.Pow(i, 2) + Mathf.Pow(j, 2) > Mathf.Pow(brushRadius, 2))
                    continue;
 
                //像素点色值
                Color color = eraseTexture.GetPixel(pixelPos.x + i, pixelPos.y + j);
 
                //判断透明度,是否已擦除
                if (Mathf.Approximately(color.a, 0))
                    continue;
 
                //修改像素点透明度
                color.a = 0;
                eraseTexture.SetPixel(pixelPos.x + i, pixelPos.y + j, color);
 
                //擦除数量统计
                eraseLength++;
            }
        }
        eraseTexture.Apply();
 
        //判断擦除进度
        RefreshErasePercent();
    }
 
    float tempPercent;
    void RefreshErasePercent()
    {
        if (isFinish)
            return;
 
        tempPercent = eraseLength / textureLength;
 
        tempPercent = (float)Math.Round(tempPercent, 2);
 
        Debug.Log("擦除百分比 : " + tempPercent);
 
        if (tempPercent >= finishPercent)
        {
            isFinish = true;
 
            eraseImage.enabled = false;
 
            //触发结束事件
            if (eraseFinishEvent != null)
                eraseFinishEvent.Invoke();
        }
    }
 
}