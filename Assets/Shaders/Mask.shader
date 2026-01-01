Shader "Custom/Mask"
{
    Properties
    {
        
    }
    SubShader
    {
        Pass
        {
            stencil
            {
                Ref 1
                //让边框的模板值写入缓冲区
                Comp Always
                Pass Replace
            }
            Tags {"Queue"="Geometry"}
            ColorMask 0
            //防止遮挡物体
            Zwrite off
        }
    }
}