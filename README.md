<div align="center"><h1><img width="600" height="131" alt="68747470733a2f2f70616e2e73616d7979632e6465762f732f56596d4d5845" src="https://github.com/user-attachments/assets/d0316faa-c2d0-478f-a642-1e3c3651f1d4" /></h1></div>

<div class="section">
<div align="center"><h1>Custom Zombie Sounds for Swiftlys2 ZombieRiot</h1></div>


<div align="center"><strong>为Swiftlys2僵尸暴动,支持扩展更多自定义丧尸音效</p></div>

<div align="center"><strong>支持多种自定义音效配置。</p></div>
<div align="center"><strong>根据僵尸暴动内的丧尸名字区分不同丧尸音效</p></div>
<div align="center"><strong>支持单独总体开关,各项音效不填写则不生效</p></div>
<div align="center"><strong>支持受伤音效,死亡音效,idle音效,手雷爆炸受伤音效,火焰灼烧音效,丧尸攻击音效,攻击墙壁音效,攻击无目标音效</p></div>
<div align="center"><strong>支持多项预缓存声音事件,音量调整</p></div>
</div>

<div align="center">

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/Z8Z31PY52N)
  

</div>

---

📦 创意工坊示例（音效）


插件可结合以下创意工坊资源使用（示例）：
3644652779
```
要使用创意工坊资源,需要服务器安装metamod插件 multiaddonmanager 来管理服务器和玩家使用下载和安装创意工坊资源

安装multiaddonmanager插件后 在game\csgo\cfg\multiaddonmanager\multiaddonmanager.cfg配置文件中
 
找到第一行 mm_extra_addons  "3644652779"

把资源ID填写上去 等待服务器下载资源完毕 玩家进服会自动下载资源

之后用 Source2Viewer 软件 打开资源包 查看资源内的soundevent名字

之后根据需要填写到音效配置内使用
```
---

配置示例

```
{
  "ZombieSounds": {
    "ZombieList": [
  	{
  		"Name": "Zombienormal1",  //填写与僵尸暴动内丧尸名字一致
  		"Enable": true, //总体开关
  		"Volume": 1.0, //整体音量比例
  		"HurtSound": "han.zombie.manclassic_hurt", //受伤音效
  		"DieSound": "han.zombie.manclassic_death", //死亡音效
  		"PainSound": "han.zombieplague.zpain", //被爆头痛苦音效
  		"IdleSound": "han.hl.nihilanth.idle,han.hl.nihilanth.idleb,han.hl.nihilanth.idlec", //idle音效 
  		"IdleInterval": 180.0, //idle 间隔时长 (会随机根据间隔 0-180 内随机某个数作为idle的起始 之后根据间隔播放下一次)
  		"BurnSound": "han.zombieplague.zburn", //被燃烧弹 烧伤时候播放的音效
  		"ExplodeSound": "han.zombieplague.zbrains", //被手雷炸到时候播放的音效
  		"HitSound": "han.zombie.classic_hit", //丧尸攻击人类或者队友时候音效
  		"HitWallSound": "han.zombie.classic_hitwall",  //丧尸攻击墙壁音效
  		"SwingSound": "han.zombie.classic_swing", //丧尸空挥时播放音效
  		"PrecacheSounds": "soundevents/hanzbplayers.vsndevts", //预缓存声音事件文件
  	},
  	{
  		"Name": "Zombienormal2",
  		"Enable": true,
  		"Volume": 1.0,
  		"HurtSound": "han.zombie.manclassic_hurt",
  		"DieSound": "han.zombie.manclassic_death",
  		"PainSound": "han.zombieplague.zpain",
  		"IdleSound": "han.hl.nihilanth.idle,han.hl.nihilanth.idleb,han.hl.nihilanth.idlec",
  		"IdleInterval": 180.0,
  		"BurnSound": "han.zombieplague.zburn",
  		"ExplodeSound": "han.zombieplague.zbrains",
  		"HitSound": "han.zombie.classic_hit",
  		"HitWallSound": "han.zombie.classic_hitwall",
  		"SwingSound": "han.zombie.classic_swing",
  		"PrecacheSounds": "soundevents/hanhl1zombies.vsndevts",
  	}
  ]
  }
}
```
---
<div align="center"><strong>For Swiftlys2 Zombie Riot, support for more custom zombie sound effects</p></div>
<div align="center"><strong>Supports multiple custom sound effect configurations.</p></div>
<div align="center"><strong>Distinguish different zombie sound effects based on zombie names within Zombie Riot</p></div>
<div align="center"><strong>Supports individual overall on/off settings; individual sound effects will not take effect if not specified</p></div>
<div align="center"><strong>Supports injury sound effects, death sound effects, idle sound effects, grenade explosion injury sound effects, fire burning sound effects, zombie attack sound effects, wall attack sound effects, and targetless attack sound effects</p></div>
<div align="center"><strong>Supports multiple pre-cached sound events and volume adjustment</p></div>
</div>


---

The plugin can be used in conjunction with the following Workshop resource (example):
3644652779

``` To use Workshop resources, the server needs to have the Metamod plugin MultiAddonManager installed to manage server and player downloads and installations of Workshop resources.

After installing the MultiAddonManager plugin, in the configuration file game\csgo\cfg\multiaddonmanager\multiaddonmanager.cfg

Find the first line mm_extra_addons "3644652779"

Enter the resource ID into it. Wait for the server to finish downloading the resource. Players will automatically download the resource upon joining the server.

Then, use Source2Viewer to open the resource package and view the soundevent name within the resource.

Then, enter it into the sound effect configuration as needed.

```
---
Configuration Example
```
{
  "ZombieSounds": {
    "ZombieList": [
  	{
        "Name": "Zombienormal1",  //Enter the name of the zombie that matches the name of the zombie in the zombie riot.
        "Enable": true, // Overall on/off switch
        "Volume": 1.0, // Overall volume ratio
        "HurtSound": "han.zombie.manclassic_hurt", // Hurt sound effect
        "DieSound": "han.zombie.manclassic_death", // Death sound effect
        "PainSound": "han.zombieplague.zpain", // Headshot pain sound effect
        "IdleSound": "han.hl.nihilanth.idle,han.hl.nihilanth.idleb,han.hl.nihilanth.idlec", // Idle sound effect
        "IdleInterval": 180.0, // Idle interval duration (randomly selects a number between 0 and 180 as the start of the idle interval, and then plays the next interval accordingly)
        "BurnSound": "han.zombieplague.zburn", // Sound effect played when burned by an incendiary bomb
        "ExplodeSound": "han.zombieplague.zbrains", // Sound effect played when hit by a grenade
        "HitSound": "han.zombie.classic_hit", // Sound effect played when a zombie attacks a human or teammate
        "HitWallSound": "han.zombie.classic_hitwall", // Sound effect played when a zombie attacks a wall
        "SwingSound": "han.zombie.classic_swing", // Sound effect played when a zombie swings its arms in mid-air
        "PrecacheSounds": "soundevents/hanzbplayers.vsndevts", // Pre-cached sound event files
  	},
  	{
  		"Name": "Zombienormal2",
  		"Enable": true,
  		"Volume": 1.0,
  		"HurtSound": "han.zombie.manclassic_hurt",
  		"DieSound": "han.zombie.manclassic_death",
  		"PainSound": "han.zombieplague.zpain",
  		"IdleSound": "han.hl.nihilanth.idle,han.hl.nihilanth.idleb,han.hl.nihilanth.idlec",
  		"IdleInterval": 180.0,
  		"BurnSound": "han.zombieplague.zburn",
  		"ExplodeSound": "han.zombieplague.zbrains",
  		"HitSound": "han.zombie.classic_hit",
  		"HitWallSound": "han.zombie.classic_hitwall",
  		"SwingSound": "han.zombie.classic_swing",
  		"PrecacheSounds": "soundevents/hanhl1zombies.vsndevts",
  	}
  ]
  }
}
```
