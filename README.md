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

## 程序简介
以下是代码内容和基本的一些程序作用介绍
<table border=0 cellpadding=0 cellspacing=0 width=836 style='border-collapse:
 collapse;table-layout:fixed;width:628pt'>
 <col width=122 style='mso-width-source:userset;mso-width-alt:3904;width:92pt'>
 <col width=145 style='mso-width-source:userset;mso-width-alt:4629;width:109pt'>
 <col width=194 style='mso-width-source:userset;mso-width-alt:6208;width:146pt'>
 <col width=375 style='mso-width-source:userset;mso-width-alt:11989;width:281pt'>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl65 width=122 style='height:18.0pt;width:92pt'>文件夹名</td>
  <td class=xl65 width=145 style='border-left:none;width:109pt'>子文件夹名</td>
  <td class=xl65 width=194 style='border-left:none;width:146pt'>文件名</td>
  <td class=xl65 width=375 style='border-left:none;width:281pt'>作用</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=122 style='height:18.0pt;border-top:none;
  width:92pt'>Materials</td>
  <td class=xl66 width=145 style='border-top:none;border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>---</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>一些测试合批用的材质，没啥大用</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=122 style='height:18.0pt;border-top:none;
  width:92pt'>Plugins</td>
  <td class=xl66 width=145 style='border-top:none;border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>---</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>无使用文件</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td rowspan=3 height=72 class=xl66 width=122 style='height:54.0pt;border-top:
  none;width:92pt'>Prefab</td>
  <td rowspan=3 class=xl66 width=145 style='border-top:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>Cube</td>
  <td class=xl67 width=375 style='border-top:none;border-left:none;width:281pt'>测试合批用的正方体预制件</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=194 style='height:18.0pt;border-top:none;
  border-left:none;width:146pt'>player</td>
  <td class=xl67 width=375 style='border-top:none;border-left:none;width:281pt'>玩家的预制件</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=194 style='height:18.0pt;border-top:none;
  border-left:none;width:146pt'>test_5</td>
  <td class=xl67 width=375 style='border-top:none;border-left:none;width:281pt'>一个可以6面不同贴图的正方体预制件</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td rowspan=3 height=72 class=xl66 width=122 style='height:54.0pt;border-top:
  none;width:92pt'>Resources</td>
  <td class=xl66 width=145 style='border-top:none;border-left:none;width:109pt'>LoadMenu</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>---</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>一些运行截图和加载时用的背景动画</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>201301031509_terrain.png</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>我的世界官网提供的贴图合集</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>test_5.fbx</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>3DMax制作的6面不同贴图的正方体模型</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td rowspan=2 height=48 class=xl66 width=122 style='height:36.0pt;border-top:
  none;width:92pt'>Scenes</td>
  <td class=xl66 width=145 style='border-top:none;border-left:none;width:109pt'>LoadingScene</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>---</td>
  <td class=xl68 width=375 style='border-top:none;border-left:none;width:281pt'>加载场景</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>SampleScene</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>---</td>
  <td class=xl68 width=375 style='border-top:none;border-left:none;width:281pt'>运行场景</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td rowspan=13 height=346 class=xl66 width=122 style='height:260.0pt;
  border-top:none;width:92pt'>Script</td>
  <td rowspan=3 class=xl66 width=145 style='border-top:none;width:109pt'>Hi-Z</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>blockCulling.compute</td>
  <td rowspan=3 class=xl68 width=375 style='border-top:none;width:281pt'>原本想用compute
  Shader做一个gpu遮挡剔除，来优化渲染，但还是没有研究的太明白，暂时没有启用这部分代码</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=194 style='height:18.0pt;border-top:none;
  border-left:none;width:146pt'>blockShader.shader</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=194 style='height:18.0pt;border-top:none;
  border-left:none;width:146pt'>depthMapGenerator.cs</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td rowspan=2 height=65 class=xl66 width=145 style='height:49.0pt;border-top:
  none;width:109pt'>PerlinNoizeMap</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>MapCreator.cs</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>纯C#代码，核心的地图参数生成脚本</td>
 </tr>
 <tr height=41 style='height:31.0pt'>
  <td height=41 class=xl66 width=194 style='height:31.0pt;border-top:none;
  border-left:none;width:146pt'>PerlinNoize.cs</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>纯C#代码，2D，3D柏林噪声代码参考自肯柏林源码，有考虑转用Unity的噪声库</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>UI</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>LoadingScene</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>加载场景的UI代码</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>UI</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>SampleScene</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>运行场景的UI代码</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>CameraRending.cs</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>未启用，利用深度图进行遮挡剔除的部分代码</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>cell.cs</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>纯C#代码，代表地图晶格，一个晶格记录一个block</td>
 </tr>
 <tr height=41 style='height:31.0pt'>
  <td height=41 class=xl66 width=145 style='height:31.0pt;border-top:none;
  border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>chunk.cs</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>纯C#代码，代表地图地块，一个地块包含16*16*128个晶格</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>mapLoader</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>核心的地图生成逻辑，地块异步演算逻辑，携程逻辑</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>newWorldLoader.cs</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>配置贴图资源，初始化地块，运行逻辑等代码</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=145 style='height:18.0pt;border-top:none;
  border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>playerBehaviour.cs</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>核心的玩家的运动逻辑脚本</td>
 </tr>
 <tr height=24 style='height:18.0pt'>
  <td height=24 class=xl66 width=122 style='height:18.0pt;border-top:none;
  width:92pt'>steamingAssets</td>
  <td class=xl66 width=145 style='border-top:none;border-left:none;width:109pt'>---</td>
  <td class=xl66 width=194 style='border-top:none;border-left:none;width:146pt'>BlockINFO.XML</td>
  <td class=xl65 width=375 style='border-top:none;border-left:none;width:281pt'>地块贴图信息</td>
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