This is a work in progress!

主要功能：
1.多伴侣（包括前配偶）， 支持让俘虏或者同伴变成伴侣。
2.生育成长，支持多配偶（包括前配偶）婚育控制，和孩子快速成长。
3.无名NPC, 可以招募无名NPC，使其变成配偶或者同伴， 并且有很好的成长属性。
4.NPC成长, 支持NPC 战斗统计和历史记录。



2.3.0
1.调整无名NPC获取防具逻辑，确保必有装备。
2.调整招募无名NPC时其职业属性，从领主改成流浪者（他的原始模板将会绑定随机流浪者）。
3.修正一个通过该MOD让一个家族最后一人加入时，导致无法发放工资影响士气BUG。
4.设置界面调整。
5.



2.1.5版本内容(20200818)
1.支持玩家兄弟姐妹的快速成长
2.修正孩子成长逻辑没触发系统事件
3.修正成为伴侣后，没有即时添加到家人列表（游戏版本1.4.3）
4.修正玩家孩子，属性为0的概率，会使用MOD 逻辑进行补全技能
5.保存设置数据到配置文件
6.配偶界面：可以指定女孩怀孕， 指定主配偶 
7.使用MOD方式结婚，主配偶将不会变

1.Support the fastgrowth of player brothers and sisters.
2.Fixed child growth logic not triggering system events.
3.Fixed no immediate addition to family list after becoming a partner.
4.Fixed the probability that the player's child had a 0 attribute and would use MOD logic to complete the skill.
5.Save the Settings data to the configuration file.
6.Spouse interface: The girl can be designated pregnant, the main spouse can be designated.
7.Marry in MOD mode, the primaryspouse will not change

2.0.1版本内容
1.移除了MCM, 增加了一个配置界面，可以点击家族面板左上角的按钮打开(目前配置只是修改内存数据)。
2.重新调整了本地化代码。
3.去掉了可能操作严重问题的选项
测试: 100天， 摧毁了一个国家和众多家族（包括小阵营）
存在问题：主要后面会莫名发生无法支付全部薪水，导致士气降低6


2.0.1 Version Content
1. Remove MCM and add a configuration interface. Click the button in the upper left corner of the Clan panel to open it
(The current configuration only modifies the memory data)
2. Readjusted localization code.
3. Remove options that may operate on serious problems
Test: 100 days, destroy a country and many clans (including a small clan),
Existing problem： Failure to pay all salaries will occur later, resulting in a reduction in morale by 6

1.3.2版本内容
1.支持前配偶怀孕控制
2.支持主要玩家孩子快速成长

1.3.2 Version contents
1.Support ex-spouse pregnancy control
2. Support the children of MainPlayer to fast growth

1.2 版本内容：
1.修正新建游戏没有加载本MOD逻辑
2.支持使年轻没结婚男子领主囚犯成为伴侣

1.2 version contents:
1. Fixed new game not loading this MOD logic
2. Support to partner young unmarried male overlord prisoners

1.3 Version Contents:
1.修正了前配偶们不能生育问题， 这个逻辑可能会和相关控制怀孕概率的MOD 有冲突。概率算法是游戏算法加0.2f
2.增加让囚犯加入家族的选项， 这个支持派系领袖，和家族领袖。 并且强制控制玩家家族不能离开自己国家
3.和同伴结婚， 将会将其添加到贵族列表， 在家族面板家人那里会显示。

1. Fixed an issue where ex-spouses were unable to have children, a logic that might conflict with the MOD that controls the probability of pregnancy.The probability algorithm is the game algorithm plus 0.2f
2. Increased the option for prisoners to join clans, which supports faction leaders, and clan leaders.And force the controlling player family not to leave their country
3. If you marry a companion, you will add it to the noble list, which will be displayed in the family panel.


1.1版本内容
1.修正从俘虏变成伴侣后， 该英雄还是俘虏状态；
2.支持把伙伴NPC 变成伴侣；



From  YouDao Translate
Main functions:
1. In the chat option with the captive, you can turn it becom a spouse
2. In chat options with companions (NPC), you can turn itbecom  spouse
3. Support ex-spouses to have children

1.1 version content
1. Fixed the hero to remain captive when he changed from captive to spouse;
2. Support turning  companion NPC becom a spouse;



注意：多备份存档
Note: multiple backup archives

Helpful information:

(CTRL+Left Click on link to open in your browser)
Bannerlord Modding Documentation:   https://docs.bannerlordmodding.com/
Harmony Patching Documentation:     https://harmony.pardeike.net/articles/patching.html
TaleWorlds Modding Forums:          https://forums.taleworlds.com/index.php?pages/modding/
Mount and Blade Discord:            https://discordapp.com/invite/mountandblade
TW Forum - Modding Discord:         https://discordapp.com/invite/ykFVJGQ

功能测试
1. Change companion become spouse.
2. Defeat a Lord, and change it become spouse.
3. Defeat a Lord, and Let it join clan.
4. Destroy a clan 