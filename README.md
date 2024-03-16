# RomajiConverter
WPF RomajiConverter

**！！！该项目不再更新，如果你使用win10以上系统，请使用新版本应用[RomajiConverter.WinUI](https://github.com/xyh20180101/RomajiConverter.WinUI)；如果你使用Android系统，请使用移动端应用[RomajiConverter.App](https://github.com/xyh20180101/RomajiConverter.App)**

![](https://raw.githubusercontent.com/xyh20180101/RomajiConverter/main/1.png)

下载见[release](https://github.com/xyh20180101/RomajiConverter/releases)，选择框架依赖版需要下载[.net core 3.1桌面运行时](https://dotnet.microsoft.com/zh-cn/download/dotnet/3.1)，独立版则不需要

## 功能
- 将假名、汉字转换为罗马音
- 支持直接转换带中文翻译的日语歌词（通过复制或导入）
- 支持网易云音乐PC端的当前播放的歌词导入
- 支持将转换结果导出为png图片

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

### 1.1.3
- 转换逻辑同步为与WinUI版本一致

### 1.1.2
- 新增支持网易云音乐3.0客户端歌词获取

### 1.1.1
- 新增仅显示汉字的平假名功能(包括编辑界面、生成图片、生成文本)

### 1.1.0
- 新增转换结果编辑功能（详细模式下可见）
- 新增生成图片功能（详细模式下可见）
- 新增解析元素：平假名
- 新增界面缩放功能
- 新增界面宽度拖拽功能
- 由于QQ音乐歌词接口已不可用，隐藏了获取QQ音乐歌词功能
- 具体功能说明，见``RomajiConverter/Help.md``或应用内[设置-帮助]选项

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
