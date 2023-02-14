# RomajiConverter
WPF RomajiConverter

**需安装.net core 3.1框架**

![](https://raw.githubusercontent.com/xyh20180101/RomajiConverter/main/1.png)

## 功能
- 将假名、汉字转换为罗马音
- 支持直接转换带中文翻译的日语歌词（通过复制或导入）
- 支持网易云音乐PC端，QQ音乐PC端（不稳定）的当前播放的歌词导入

## 缺陷
- 汉字和多音假名（例如：は）的转换不一定准确，以歌曲发音为准
- 导入歌词会有短暂延迟，而且有可能报错（可能与正在播放的歌曲有关）

## 常见问题
```
本能が云(い)う、嫌々(いやいや)  =>  honnou ga云(i)u、iyaiya(iyaiya)
どこかで微(かす)か伝うメーデー  =>  doko ka de bi(kasu)ka tsutau mee dee
```
- 上面这种汉字没转换、读音错误的情况的原因为：歌词上传者已经在括号里标注了读音。这种情况下直接使用括号里的读音就可以了
```
思い浮かぶアナタの颜...  =>  omoiukabu anata no颜...
```
- ~~"颜"应为"顔"，歌词中的汉字被简化后可能无法识别~~
- 1.0.6版本已加入自动简繁变体处理，可解决部分该类问题，但目前发现unihan-database有部分数据仍不是有效的日文字符，所以仍需要手动解决该类错误

## 更新日志

### 1.0.6
- 新增"是否自动识别变体"功能。如果含有无法匹配到词典的字符，会尝试进行简繁转换后再进行转换（不保证完美，默认关闭）

![](https://raw.githubusercontent.com/xyh20180101/RomajiConverter/main/2.png)

## 项目引用
[AduSkin WPF UI](https://github.com/aduskin/AduSkin)  
[MeCab.DotNet](https://github.com/kekyo/MeCab.DotNet)  
[LrcParser](https://github.com/OpportunityLiu/LrcParser)  
[WanaKanaSharp](https://github.com/caguiclajmg/WanaKanaSharp)  
[UniDic](https://unidic.ninjal.ac.jp/)  
[unihan-database](https://github.com/unicode-org/unihan-database)