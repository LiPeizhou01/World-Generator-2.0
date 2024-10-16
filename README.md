# World-Generator-2.0
Personal Perlin Noise project
****
## 项目简介
这是一个基于柏林噪声生成随机世界的个人项目，最终目标是可以达到类似我的世界的，参数驱使且可控的随机地图生成。
该项目并不专注于可玩性设计，我的核心目标是希望通过数学演算与程式设计来达到可控的随机，所以并没有为其注入太多的游戏元素。

该项目有母项目 *World-Generator-1.0*，他对算法有一些有趣的尝试。　　　

如果你对他感兴趣请查看 [*@World-Generator -1.0*](https://github.com/LiPeizhou01/World-Generator)

## release 版本简介
* [*@World-Generator-v0.1.0-alpha*](https://github.com/LiPeizhou01/World-Generator-2.0/releases/tag/v0.1.0-alpha)
> (24/09/06)   
> 这是我认为可以作为展示的第一个版本，其具备了基本的异步演算，携程生成随机地形生成的能力。
> 其已经可以生成出看起来不错的随机地形了，并且我已经完成了部分的参数可视化。  
> 但参数而来的可控性仍然不足，只有部分参数达到了我想要的效果。还没有进行含水演算导致地形有些缺乏多样性，由于赶工，地块状态部分的程序也没有完全模块化。
> 这些会成为下一个版本开发的目标。

* [*@World-Generator-v0.2.4*](https://github.com/LiPeizhou01/World-Generator-2.0/releases/tag/v0.2.4)
> (24/10/16)   
>  v0.2版本的目标是使项目支持更复杂更大的地图演算与实时加载逻辑，v0.2.4经过了一些优化表现的比较稳定。
> 虽有很多不足但我认为这对于大世界生成的实时演算，实时加载是一个极具参考性的版本。   
> 从运行的角度上来讲v0.1.0虽然更加流畅，但我认为代码架构上限制了它的上限，且代码凌乱可扩展性薄弱。
> 在v0.2版本中我重写了大量的底层代码，把所有地图的演算逻辑集中到"MapAlgorithm.cs"文件中，不再分散到多个脚本里。
> 对于地图加载采用了有限状态机模式的设计方法，现在会根据玩家所处的地块位置更新地块的需求等级，并进行异步演算。
> 地块实例化则采用发送委托至主线调度器的方式，由主线程调度器调度并分帧生成地块。      
> 
> 其他的一些优化:
> * 改进了一些跳跃上的手感
> * 添加了显示噪声图功能
> * 改进了UI设计使其可以自适应16；10的屏幕
> * 修改了地图噪声算法使用Unity自带的柏林噪声算法(效果不好)
> * 修改了地图种子的生成逻辑
> * 修改了由种子获取噪声值的演算方法
> * 修改了柏林噪声插值频率使其初步适配地图生成，使各个参数起到应有的作用
> * 修正了大陆压力挤压系数等一系列地图相关参数的取值。
> * 改良了海拔演算方法，使地形生成更加真实
> * 移除了一些bug式的地图演算逻辑，等待后续开发
> * 从新模块化地图的演算程序架构，提升了程序扩展性

## 程序简介
以下是代码内容和基本的一些程序作用介绍(当前文件说明对应版本v0.2.4)
<table border=0 cellpadding=0 cellspacing=0 width=836 style='border-collapse:
 collapse;table-layout:fixed;width:628pt'>
 <col width=122 style='mso-width-source:userset;mso-width-alt:3904;width:92pt'>
 <col width=145 style='mso-width-source:userset;mso-width-alt:4640;width:109pt'>
 <col width=194 style='mso-width-source:userset;mso-width-alt:6208;width:146pt'>
 <col width=375 style='mso-width-source:userset;mso-width-alt:12000;width:281pt'>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl65 width=122 style='height:18.75pt;width:92pt'>文件夹名</td>
  <td class=xl65 width=145 style='border-left:none;width:109pt'>子文件夹名</td>
  <td class=xl65 width=194 style='border-left:none;width:146pt'>文件名</td>
  <td class=xl65 width=375 style='border-left:none;width:281pt'>作用</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl67 style='height:18.75pt;border-top:none'>Materials</td>
  <td class=xl67 style='border-top:none;border-left:none'>---</td>
  <td class=xl67 style='border-top:none;border-left:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>一些测试合批用的材质，没啥大用</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl67 style='height:18.75pt;border-top:none'>Plugins</td>
  <td class=xl67 style='border-top:none;border-left:none'>---</td>
  <td class=xl67 style='border-top:none;border-left:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>无使用文件</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td rowspan=3 height=75 class=xl68 style='height:56.25pt;border-top:none'>Prefab</td>
  <td rowspan=3 class=xl68 style='border-top:none'>---</td>
  <td class=xl68 style='border-top:none;border-left:none'>Cube</td>
  <td class=xl69 style='border-top:none;border-left:none'>测试合批用的正方体预制件</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl68 style='height:18.75pt;border-top:none;border-left:
  none'>player</td>
  <td class=xl69 style='border-top:none;border-left:none'>玩家的预制件</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl68 style='height:18.75pt;border-top:none;border-left:
  none'>test_5</td>
  <td class=xl69 style='border-top:none;border-left:none'>一个可以6面不同贴图的正方体预制件</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td rowspan=3 height=75 class=xl68 style='height:56.25pt;border-top:none'>Resources</td>
  <td class=xl67 style='border-top:none;border-left:none'>LoadMenu</td>
  <td class=xl66 style='border-top:none;border-left:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>一些运行截图和加载时用的背景动画</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl67 style='height:18.75pt;border-top:none;border-left:
  none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>201301031509_terrain.png</td>
  <td class=xl66 style='border-top:none;border-left:none'>我的世界官网提供的贴图合集</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl67 style='height:18.75pt;border-top:none;border-left:
  none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>test_5.fbx</td>
  <td class=xl66 style='border-top:none;border-left:none'>3DMax制作的6面不同贴图的正方体模型</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td rowspan=2 height=50 class=xl68 style='height:37.5pt;border-top:none'>Scenes</td>
  <td class=xl68 style='border-top:none;border-left:none'>LoadingScene</td>
  <td class=xl68 style='border-top:none;border-left:none'>---</td>
  <td class=xl68 style='border-top:none;border-left:none'>加载场景</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl68 style='height:18.75pt;border-top:none;border-left:
  none'>SampleScene</td>
  <td class=xl68 style='border-top:none;border-left:none'>---</td>
  <td class=xl68 style='border-top:none;border-left:none'>运行场景</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td rowspan=16 height=400 class=xl70 style='border-bottom:.5pt solid black;
  height:300.0pt;border-top:none'>Script</td>
  <td rowspan=3 class=xl68 style='border-top:none'>Hi-Z</td>
  <td class=xl66 style='border-top:none;border-left:none'>blockCulling.compute</td>
  <td rowspan=3 class=xl68 style='border-top:none'>原本想用compute
  Shader做一个gpu遮挡剔除，来优化渲染，但还是没有研究的太明白，暂时没有启用这部分代码</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none;border-left:
  none'>blockShader.shader</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none;border-left:
  none'>depthMapGenerator.cs</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td rowspan=2 height=50 class=xl68 style='height:37.5pt;border-top:none'>PerlinNoizeMap</td>
  <td class=xl72 style='border-top:none;border-left:none'>MapCreator.cs</td>
  <td class=xl72 style='border-top:none;border-left:none'>纯C#代码，核心的地图参数生成脚本</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl72 style='height:18.75pt;border-top:none;border-left:
  none'>PerlinNoize.cs</td>
  <td class=xl72 style='border-top:none;border-left:none'>纯C#代码，2D，3D柏林噪声代码参考自肯柏林源码，有<span
  style='display:none'>考虑转用Unity的噪声库</span></td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl72 style='height:18.75pt;border-top:none'>UI</td>
  <td class=xl72 style='border-top:none;border-left:none'>LoadingScene</td>
  <td class=xl72 style='border-top:none;border-left:none'>加载场景的UI代码</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl72 style='height:18.75pt;border-top:none'>UI</td>
  <td class=xl72 style='border-top:none;border-left:none'>SampleScene</td>
  <td class=xl72 style='border-top:none;border-left:none'>运行场景的UI代码</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>UI</td>
  <td class=xl66 style='border-top:none;border-left:none'>InitCameraAsepect</td>
  <td class=xl66 style='border-top:none;border-left:none'>信箱模式代码，用于自适应16:10比例屏幕</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>CameraRending.cs</td>
  <td class=xl66 style='border-top:none;border-left:none'>未启用，利用深度图进行遮挡剔除的部分代码</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>cell.cs</td>
  <td class=xl66 style='border-top:none;border-left:none'>纯C#代码，代表地图晶格，一个晶格记录一个block</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>chunk.cs</td>
  <td class=xl66 style='border-top:none;border-left:none'>地块对象
  一个地块包含16*16*256个晶格</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>mapLoader</td>
  <td class=xl66 style='border-top:none;border-left:none'>核心的地块异步演算逻辑，携程逻辑</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>newWorldLoader.cs</td>
  <td class=xl66 style='border-top:none;border-left:none'>配置贴图资源，初始化地块，运行逻辑等代码</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>playerBehaviour.cs</td>
  <td class=xl66 style='border-top:none;border-left:none'>核心的玩家的运动逻辑脚本</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>MapAlgorithm.cs</td>
  <td class=xl66 style='border-top:none;border-left:none'>所有的地块演算逻辑</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>MainThreadDispatcher.cs</td>
  <td class=xl66 style='border-top:none;border-left:none'>主线程调度器</td>
 </tr>
 <tr height=25 style='height:18.75pt'>
  <td height=25 class=xl66 style='height:18.75pt;border-top:none'>steamingAssets</td>
  <td class=xl66 style='border-top:none;border-left:none'>---</td>
  <td class=xl66 style='border-top:none;border-left:none'>BlockINFO.XML</td>
  <td class=xl66 style='border-top:none;border-left:none'>地块贴图信息</td>
 </tr>
 <![if supportMisalignedColumns]>
 <tr height=0 style='display:none'>
  <td width=122 style='width:92pt'></td>
  <td width=145 style='width:109pt'></td>
  <td width=194 style='width:146pt'></td>
  <td width=375 style='width:281pt'></td>
 </tr>
 <![endif]>
</table>

## 问题反馈与讨论
有问题请留言或发送邮件至 chaokedsky@gmail.com

## 柏林噪声和随机地图生成的一些相关文献推荐
(待更新)