##  一、运行
1、下载仓库 <code>git clone git@github.com:Myshrimp/RM_Labeling_Tool.git</code>
2、进入RM_Labeling_Tool文件夹，运行 <code>git submodule update --init</code>
3、下载UnityHub， 安装editor版本2021.3.42f1c1
4、在UnityHub上方点击Open Project，打开包含Assets文件夹的目录。打开后，工作目录就是Assets/
5、打开Scenes文件夹，双击Init.scene，进入初始化场景   
6、点击运行

## 二、操作指南
-P: 打开能量机关
-(deprecated)O：截屏（摄像机视角），会保存到Assets/StreamingAssets目录下。如果是打包运行后截屏，则在exe文件的目录下找到XX_Data/StreamingAssets文件夹。（XX是你的项目名）
-F1: 截屏并生成标注数据以及其他文件（可设置不生成，待会介绍），都保存在StreamingAssets目录下。
-R：能量机关旋转/停止旋转
-T：能量机关切换状态
-WASD:控制移动
-Alpha 1(数字键1)：选择操作对象为红方机关
-Alpha 2:                 选择操作对象为蓝方机关
-Alpha 3:                 选择操作对象为两个机关
-Esc :显示或隐藏鼠标

## 三、打包运行
打开一个场景后，左上角File选项可选择Build Settings，点击Add Open Scene可以添加当前场景，确保Init和SampleScene都被勾选，且Init是第一个

## 四、配置
### 配置方式
这个项目中使用了两种配置方式，一是直接选中Scene中的物体，可以在Inspector面板中看到这个物体的相关属性，包括挂载了哪些脚本，脚本中暴露的字段赋予哪些默认值；二是使用**Config**文件夹中的ScriptableObject文件作为配置文件。   
第一种方法的缺点是要在场景中手动找物体，比较麻烦，优点是比较直观。后续优化可以考虑转成第二种方式。（或者用excel、json文件配置）

### Config文件夹下三个配置文件的含义
1、FanConfig: 代表的是能量机关单个扇叶的配置，每个element有FanState和Lights两个属性，含义是这个扇叶在处于某个状态时，应该点亮哪些灯。  
Lights是整型数组，每个元素代表的是灯的位置。打开Scenes文件夹下的SampleScene，在 Hierararchy（层级）面板下找到Scene/Scene(1)/PowerRune/Rotator/blue/fan1，你会发现这个物体下有很多子物体，从上往下数就是0 ~ 13位灯。最后一个子物体points(1)是关键点，用于导出label数据。你可以看看它有哪些子物体，**手动调整关键点的位置**   
2、FanControllerConfig: 用于配置能量机关旋转的速度和频率。具体怎么运转可以看FanController.cs,该脚本挂载于Hierararchy（层级）Scene/Scene(1)/PowerRune/Rotator/blue(or red)   
3、Scene: 配置的是Scene文件的索引和相对路径
### Camera与导出label数据的配置
进入SampleScene场景下，左侧选中Main Camera可以在右侧Inspector面板中看到这个摄像机挂载的脚本。   

-FP Camera(First person perspective) : 用于相机移动和旋转控制   

-CameraCapture: 用于截图和导出数据。可以看到有两个勾选项，第一个Skybox开头的，是控制相机拍摄到的背景是纯色的还是环境色（对Skybox贴图采样）；第二个就是控制是否生成plot img，即将关键点画出来。这个图片会放在StreamingAssets/Plot文件夹中。如果觉得拍照时卡顿，请关闭此选项

## 代码框架 && 如何扩展
修改代码主要关注Assets/Scripts文件夹下的内容。进入Scripts文件夹，可以看到如下：  

Base: 存放游戏入口类MyGameEntry和一些扩展，在其他文件中可以通过它来获取所有Component单例。创建并注册Component的方法可参考MyGameEntry.Custom文件。   

Camera:
 存放控制相机的代码，这部分由Deepseek生成，后续扩展最好是给相机添加额外的MonoBehaivour脚本   

Controller:
 能量机关的控制器，可以响应输入，控制能量机关的状态   

Input:
 存放InputComponent，输入控制中心， 该文件中包含所有输入事件，其他组件都通过MyGameEntry.Input.GetXX获取输入，可以在这个文件进行绑定按键的修改或扩展   

Procedure:
 存放游戏流程的各个状态，本质是一个有限状态机(FSM)其中的状态（即继承自FsmState<T>），游戏流程状态机控制整个游戏流程。   

Scene: 
存放CustomSceneComponent，主要是因为GF自带的SceneComponent需要与ResourceComponent结合使用，而ResourceComponent在单机模式下必须将资源打包成AB包，这个过程要进行一系列配置，并且每次build运行之前都需要先打包资源并放在StreamingAssets下，总之非常麻烦，所以我自己写了个最简单的场景管理Component，只负责加载、卸载场景   

ScreenShot: 
目前是直接挂载到了SampleScene场景下的Camera上。后续应考虑作为一个单独的Component，用MyGameEntry来获取。   

ScriptableObjects:
 Unity的持久化管理系统，创建一个SO文件后，可以指定在Editor的菜单中显示的资源名称和路径。在Assets/Config目录下可以看到一些配置文件，这些配置文件是继承自SO类的，选中配置文件，可以在Inspector面板中看到它的属性，修改配置不会造成重新编译。

 ## 关于场景
1、如果想自制一些场景，可以考虑更换天空盒(skyBox)，[相关链接](https://blog.csdn.net/Jeffxu_lib/article/details/95477352);   

2、 在场景中添加光照物体。在Hierarchy（层级）面板下，点击"+"号可以创建物体，找到Light，可以看到有多种类型的光源，比如点光源、直射光、聚光灯...

